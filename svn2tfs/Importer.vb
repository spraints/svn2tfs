Imports Microsoft.TeamFoundation.VersionControl.Client

Public Class Importer

    Public Event ProgressChange(ByVal value As Long, ByVal maximum As Long)
    Public Event ProgressStateChange(ByVal state As Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState)
    Public Event Message(ByVal message As String)

    ''' <summary>
    ''' Maps SVN revisions to TFS changesets and provides some logic.
    ''' </summary>
    ''' <remarks>If the SVN revision is not found, the .Item property returns the TFS changeset for the nearest SVN revision happened before the specified one.
    ''' This behaviour is correct since not all revisions are imported to TFS (not those that have no replayable action in them), so referring to previous revisions is correct.</remarks>
    Public Class RevisionMap
        Inherits Dictionary(Of Long, Long)

        Default Public Shadows Property Item(ByVal SvnRevision As Long) As Long
            Get
                Return MyBase.Item(GetKeyOrNearestBefore(SvnRevision))
            End Get
            Set(ByVal value As Long)
                MyBase.Item(SvnRevision) = value
            End Set
        End Property

        Public Function GetKeyOrNearestBefore(ByVal SvnRevision As Long) As Long

            While SvnRevision > 0

                If Me.ContainsKey(SvnRevision) Then Return SvnRevision

                SvnRevision -= 1

            End While

            Throw New KeyNotFoundException(
                String.Format("Unable to find {0} key or any other smaller one.",
                              SvnRevision))

        End Function

    End Class

    Private Sub messageManager_message(ByVal message As String)

        RaiseEvent Message(message)

    End Sub

    Public Class MessageManagerClass

        Public CurrentStep As Long
        Public TotalSteps As Long
        Public CurrentSvnRevision As Long
        Public Action As String
        Public Path As String

        Public Event Message(ByVal message As String)

        Friend Sub tfsService_NonFatalError(ByVal sender As Object, ByVal e As Microsoft.TeamFoundation.VersionControl.Client.ExceptionEventArgs)

            If e.Exception IsNot Nothing Then
                ThrowMessageAndAddToRevisionImportLog(MessageKindEnum.Warning, e.Exception.Message)
            Else
                ThrowMessageAndAddToRevisionImportLog(MessageKindEnum.Warning, e.Failure.Message)
            End If

        End Sub

        Public Enum MessageKindEnum
            Warning
            [Error]
        End Enum

        Private revisionLogMessages As New List(Of String)

        Public Function GetRevisionLogMessages() As String
            Try
                Return revisionLogMessages.ToDelimitedString(Environment.NewLine)
            Catch ex As System.OutOfMemoryException
                Const errorMessage = "Unable to return revision log messages: an OutOfMemoryException has been thrown."
                ThrowMessageAndAddToRevisionImportLog(MessageKindEnum.Warning, errorMessage)
                Return errorMessage
            End Try
        End Function

        Public Sub ClearRevisionLogMessages()
            revisionLogMessages.Clear()
        End Sub

        Public Sub ThrowMessageAndAddToRevisionImportLog(ByVal kind As MessageKindEnum, ByVal message As String)

            'add prefix

            Dim prefix As String
            Select Case kind
                Case MessageKindEnum.Warning
                    prefix = "Warning"
                Case MessageKindEnum.Error
                    prefix = "Error"
                Case Else
                    Throw New NotSupportedException("Specified message kind is not supported.")
            End Select

            'format message

            Dim formattedMessage = String.Format("[{0}: {1}] ", prefix, message)

            Dim context = String.Format("In SVN revision {0} while replaying actions {1} on path '{2}'.",
                                        Me.CurrentSvnRevision,
                                        Me.Action,
                                        Me.Path)

            Dim formattedMessageWithContext As String = String.Format("{1}: {2}{0}{3}",
                                                                      Environment.NewLine,
                                                                      prefix,
                                                                      message,
                                                                      context)

            'add to revision log 

            revisionLogMessages.Add(formattedMessage)

            'output to user

            Console.Write(formattedMessage)
            RaiseEvent Message(formattedMessageWithContext)

        End Sub

    End Class

    Public Class TfsNoteFieldsHelper

        Public Enum NoteFieldsEnum
            Revision
            Log
            [Date]
        End Enum

        Private Shared noteFieldNames As New Dictionary(Of NoteFieldsEnum, String)

        Private Shared alreadyLoaded As Boolean = False

        Private Shared Sub InitShared()

            If Not alreadyLoaded Then

                noteFieldNames.Add(NoteFieldsEnum.Date, "SvnDate")
                noteFieldNames.Add(NoteFieldsEnum.Log, "SvnLog")
                noteFieldNames.Add(NoteFieldsEnum.Revision, "SvnRevision")

            End If

            alreadyLoaded = True

        End Sub

        Public Shared Function Exists(ByVal versionControlService As VersionControlServer,
                               ByVal noteFieldName As String) As Boolean

            Return versionControlService.GetAllCheckinNoteFieldNames.Contains(noteFieldName)

        End Function

        Public Shared Function GetNoteFieldName(ByVal noteFieldType As NoteFieldsEnum) As String

            InitShared()

            Return noteFieldNames(noteFieldType)

        End Function

    End Class

    ''' <summary>
    ''' This is the comment that will be used for the TFS clean up check-in before import.
    ''' </summary>
    ''' <remarks></remarks>
    Public TfsCleanUpComment As String = "SVN 2 TFS automated clean-up before import."

    Private messageManager As MessageManagerClass

    Private ReadOnly tfsWorkspaceLocalPath As String
    Private ReadOnly svnWorkspaceLocalPath As String

    Public Sub New()

        messageManager = New MessageManagerClass

        AddHandler messageManager.Message, AddressOf messageManager_message

        tfsWorkspaceLocalPath = IO.Path.Combine(Common.TempPath, String.Format("svn2tfs_{0}_tfs", Now.Ticks))
        svnWorkspaceLocalPath = IO.Path.Combine(Common.TempPath, String.Format("svn2tfs_{0}_svn", Now.Ticks))

    End Sub

    Public Sub Import(ByVal svnUri As Uri,
                      ByVal svnUserName As String,
                      ByVal svnPassword As String,
                      ByVal svnDirectory As String,
                      ByVal svnFromRevision As Long,
                      ByVal svnToRevision As Nullable(Of Long),
                      ByVal tfsUri As Uri,
                      ByVal tfsCollection As String,
                      ByVal tfsProject As String,
                      ByVal tfsDirectory As String,
                      ByVal userMap As Dictionary(Of String, String),
                      ByVal tfsCleanUp As Boolean,
                      ByVal hasFirstRevisionToBeOverriddenWithRecursiveAdds As Boolean)

        Console.WriteLine("Starting importing...")
        RaiseEvent ProgressStateChange(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Indeterminate)

        'Create workers
        Using tfs = New TfsWorker(tfsUri, tfsWorkspaceLocalPath)

            Dim svn = New SvnWorker(svnUri, svnDirectory, svnUserName, svnPassword)
            Dim tfsWorkspace = tfs.GetWorkspace(tfsCollection, tfsProject, tfsDirectory)
            Dim svn2tfsRevisionMap = New RevisionMap

            'Workers handlers
            AddHandler tfsWorkspace.VersionControlServer.NonFatalError, AddressOf messageManager.tfsService_NonFatalError

            'Transform input
            If Not svnToRevision.HasValue Then
                svnToRevision = svn.GetCurrentRevision
            End If

            'Check input
            Dim svnRevisionCount = svn.GetCurrentRevision
            CheckRevisionValidity(svnFromRevision, svnRevisionCount)
            CheckRevisionValidity(svnToRevision.Value, svnRevisionCount)
            If svnFromRevision > svnToRevision Then Throw New ArgumentException("Invalid revision range. 'From revision' must be less or equal than 'To revision'.")

            'Check note fields
            For Each noteField In [Enum].GetNames(GetType(TfsNoteFieldsHelper.NoteFieldsEnum))
                Dim noteFieldName = TfsNoteFieldsHelper.GetNoteFieldName(
                                        [Enum].Parse(GetType(TfsNoteFieldsHelper.NoteFieldsEnum),
                                                     noteField))
                If Not TfsNoteFieldsHelper.Exists(tfsWorkspace.VersionControlServer, noteFieldName) Then
                    Console.WriteLine("Warning: note field '{0}' does not exist in the specified team project, information will not be imported.", noteFieldName)
                End If
            Next

            'Clean up
            If tfsCleanUp Then
                Console.WriteLine("Deleting tfs project content...")
                DeleteTfsWorkspaceContent(tfs.LocalWorkspaceDirectory, tfsWorkspace, False)
                Dim cleanUpPendingChanges = tfsWorkspace.GetPendingChanges
                If cleanUpPendingChanges.Length > 0 Then
                    tfsWorkspace.CheckIn(cleanUpPendingChanges, TfsCleanUpComment)
                End If
            End If

            'Reproduce all revisions to tfs
            Console.WriteLine("Ok, let's start!")
            For svnRevision As Integer = svnFromRevision To svnToRevision

                'init counters
                Dim currentStep = (svnRevision - svnFromRevision + 1)
                Dim totalSteps = (svnToRevision - svnFromRevision + 1)

                'init messages
                Console.Write("{0} / {1} r{2}: ", currentStep, totalSteps, svnRevision)
                messageManager.CurrentStep = currentStep
                messageManager.TotalSteps = totalSteps
                messageManager.CurrentSvnRevision = svnRevision
                messageManager.ClearRevisionLogMessages()

                'init variables
                Dim isFirstRevision = (svnFromRevision = svnRevision)

                'do the work
                tfsReproduceRevision(svn.GetLog(svnRevision, True),
                                     svn,
                                     tfsWorkspace,
                                     userMap,
                                     svn2tfsRevisionMap,
                                     isFirstRevision,
                                     hasFirstRevisionToBeOverriddenWithRecursiveAdds)

                'progress
                RaiseEvent ProgressChange(currentStep, totalSteps)

            Next

        End Using

        RaiseEvent ProgressChange(1, 1)
        RaiseEvent ProgressStateChange(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress)

    End Sub

    Private Sub tfsReproduceRevision(ByVal svnLog As SvnWorker.LogItem,
                                     ByVal svn As SvnWorker,
                                     ByVal tfsWorkspace As Workspace,
                                     ByVal userMap As Dictionary(Of String, String),
                                     ByVal svn2tfsRevisionMap As RevisionMap,
                                     ByVal isFirstRevision As Boolean,
                                     ByVal hasFirstRevisionToBeOverriddenWithRecursiveAdds As Boolean)

        'init messages
        Dim startTime = Now

        'init variables
        Dim mappedUser As String
        If (Not String.IsNullOrEmpty(svnLog.Author)) Then
            userMap.TryGetValue(svnLog.Author, mappedUser)
        End If
        If (String.IsNullOrEmpty(mappedUser)) Then
            mappedUser = Nothing
            Console.WriteLine("Unable to map SVN author {0}", svnLog.Author)
        End If

        'SVN Update / Checkout
        svnUpdate(svnLog.Revision,
                  isFirstRevision,
                  svn,
                  svnWorkspaceLocalPath)

        'TFS Add / Replay
        If isFirstRevision AndAlso hasFirstRevisionToBeOverriddenWithRecursiveAdds Then

            'TFS Recursive add of everything
            Console.Write("tfs reproduce state... ")

            Throw New NotImplementedException("not implemented.")

            'tfsWorkspace.PendAdd(localWorkspaceDirectory, True) 'TODO: Change this to copy non-.svn only

        Else

            'TFS Replay actions
            Console.Write("tfs replay... ")

            'sort changes
            If svnLog.Changes IsNot Nothing Then

                Dim sortedChanges = SvnWorker.SvnChangeItemSorter.SortChanges(svnLog.Changes.ToList)

                'replay actions
                For Each change In sortedChanges

                    Dim needSvnUpdate = tfsReplayAction(
                                            change,
                                            tfsWorkspace,
                                            svn.Directory,
                                            svn2tfsRevisionMap)

                    If needSvnUpdate Then
                        svnUpdate(svnLog.Revision,
                                  False,
                                  svn,
                                  svnWorkspaceLocalPath)
                    End If

                Next

            Else

                messageManager.ThrowMessageAndAddToRevisionImportLog(MessageManagerClass.MessageKindEnum.Warning, "Revision has not action.")

            End If

        End If

        'Checkin to TFS
        Console.Write("tfs checkin... ")

        Dim tfsChangeSet = tfsCheckIn(tfsWorkspace,
                                      svnLog,
                                      mappedUser,
                                      svn2tfsRevisionMap)

        If tfsChangeSet = -1 Then

            Console.Write(String.Format("[Warning: no action could be replayed for this revision, the revision will not be imported in TFS.] "))

        ElseIf tfsChangeSet = 0 Then

            Console.Write(String.Format("[Warning: returned TFS changeset is 0, which is an invalid return value.] "))

        Else

            Dim endTime = Now

            Console.Write("imported {0} {3} to cs{1} in {2}''.",
                              svnLog.Changes.Count,
                              tfsChangeSet,
                              Math.Round((endTime - startTime).TotalSeconds, 1),
                              IIf(svnLog.Changes.Count = 1, "action", "actions"))


        End If

        Console.WriteLine()

    End Sub

    Private Sub svnUpdate(ByVal svnRevision As Long,
                          ByVal isFirstRevision As Boolean,
                          ByVal svn As SvnWorker,
                          ByVal localWorkspaceDirectory As String)

        Try
            If isFirstRevision Then
                Console.Write("svn checkout... ")
                svn.Checkout(localWorkspaceDirectory, svnRevision)
            Else
                Try
                    'try an update
                    Console.Write("svn update... ")
                    svn.Update(localWorkspaceDirectory, svnRevision)
                Catch ex As SharpSvn.SvnException
                    'if update fails, try a revert and then update
                    Console.Write(String.Format("failed ({0}), trying to svn revert... ", ex.GetType.ToString))
                    svn.Revert(localWorkspaceDirectory)
                    Console.Write("svn update... ")
                    svn.Update(localWorkspaceDirectory, svnRevision)
                    MsgBox("Something strange happened and the update failed." & Environment.NewLine & "Please, check for me that the working folder is SVN ok and press OK.", MsgBoxStyle.Exclamation)
                End Try
            End If
        Catch ex As SharpSvn.SvnObstructedUpdateException
            Throw New ApplicationException("Unable to checkout/update: usually this is caused by a file/folder name collision between TFS and SVN.",
                                           ex)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="change"></param>
    ''' <param name="tfsWorkspace"></param>
    ''' <param name="svn2tfsRevisionMap"></param>
    ''' <returns></returns>
    ''' <remarks>True if an SVN update is needed after this call.</remarks>
    Private Function tfsReplayAction(ByVal change As SharpSvn.SvnChangeItem,
                                     ByVal tfsWorkspace As Workspace,
                                     ByVal svnDirectory As String,
                                     ByVal svn2tfsRevisionMap As RevisionMap) As Boolean

        Dim updateSvnAfterReplay = False 'returned value

        'init variables
        Dim itemLocalRelativePath = change.Path.
            Replace("/", "\").
            Remove(0, svnDirectory.Length)

        Dim svnItemLocalFullPath = String.Concat(svnWorkspaceLocalPath, itemLocalRelativePath)
        Dim tfsItemLocalFullPath = String.Concat(tfsWorkspaceLocalPath, itemLocalRelativePath)

        Dim svnItemInfo = GetFileSystemInfo(svnItemLocalFullPath)
        Dim tfsItemInfo As IO.FileSystemInfo
        If TypeOf svnItemInfo Is IO.FileInfo Then
            tfsItemInfo = New IO.FileInfo(tfsItemLocalFullPath)
        ElseIf TypeOf svnItemInfo Is IO.DirectoryInfo Then
            tfsItemInfo = New IO.DirectoryInfo(tfsItemLocalFullPath)
        Else
            Throw New NotSupportedException("File system item not supported.")
        End If

        Dim tfsWorkspaceServerRoot = tfsWorkspace.GetTeamProjectForLocalPath(tfsWorkspaceLocalPath).ServerItem


        Dim verboseAction = SvnWorker.SvnChangeItemSorter.GetSvnChangeVerboseAction(change)

        'init messages
        messageManager.Action = change.Action
        messageManager.Path = svnItemLocalFullPath

        'ensures path exists
        Select Case change.Action
            Case SharpSvn.SvnChangeAction.Add, SharpSvn.SvnChangeAction.Modify, SharpSvn.SvnChangeAction.Replace
                'the file / folder should exist
                If (Not My.Computer.FileSystem.FileExists(svnItemLocalFullPath)) AndAlso (Not My.Computer.FileSystem.DirectoryExists(svnItemLocalFullPath)) Then
                    Throw New IO.FileNotFoundException(String.Format("Internal error: Unable to find '{0}' file.", svnItemLocalFullPath))
                End If
            Case Else
                'don't care
        End Select

        'replay
        Dim resultingActionsCount As Integer

        Select Case verboseAction

            Case SvnWorker.SvnChangeItemSorter.SvnChangeVerboseActionEnum.Branch

                Console.Write("branch from r{0}... ", change.CopyFromRevision)

                'init
                Dim tfsSourceLocalFullPath = String.Concat(tfsWorkspaceLocalPath, change.CopyFromPath.Replace("/", "\"))
                'Dim copyFromFullServerPath = tfsWorkspace.GetServerItemForLocalItem(copyFromFullPath)
                'Dim copyFromRelativePath = copyFromFullPath.Substring(tfsWorkspaceLocalPath.Length)

                Dim branchFromTfsChangeSet = Microsoft.TeamFoundation.VersionControl.Client.VersionSpec.ParseSingleSpec(
                                                svn2tfsRevisionMap(change.CopyFromRevision),
                                                tfsWorkspace.OwnerName)

                resultingActionsCount = tfsWorkspace.PendBranch(tfsSourceLocalFullPath,
                                                                tfsItemLocalFullPath,
                                                                branchFromTfsChangeSet,
                                                                Microsoft.TeamFoundation.VersionControl.Client.LockLevel.Unchanged,
                                                                True)

                If TypeOf svnItemInfo Is IO.FileInfo Then

                    'Branch of a file, could have been edited before commit: copy and pend an edit

                    svnItemInfo.CopyTo(tfsItemLocalFullPath, True)

                    resultingActionsCount += tfsWorkspace.PendEdit(tfsItemLocalFullPath,
                                                                  Microsoft.TeamFoundation.VersionControl.Client.RecursionType.None)

                End If

            Case SvnWorker.SvnChangeItemSorter.SvnChangeVerboseActionEnum.Add

                If TypeOf svnItemInfo Is IO.FileInfo Then

                    svnItemInfo.CopyTo(tfsItemLocalFullPath, False)

                Else

                    tfsItemInfo.Create()

                End If

                resultingActionsCount = tfsWorkspace.PendAdd(tfsItemLocalFullPath,
                                                             False)

            Case SvnWorker.SvnChangeItemSorter.SvnChangeVerboseActionEnum.Modify

                If TypeOf svnItemInfo Is IO.DirectoryInfo Then

                    messageManager.ThrowMessageAndAddToRevisionImportLog(
                                MessageManagerClass.MessageKindEnum.Warning,
                                "Modifing a directory does not make sense, probably changed SVN attributes, ignored.")

                Else

                    svnItemInfo.CopyTo(tfsItemLocalFullPath, True)

                    resultingActionsCount = tfsWorkspace.PendEdit(tfsItemLocalFullPath,
                                                                  Microsoft.TeamFoundation.VersionControl.Client.RecursionType.None)

                End If

            Case SvnWorker.SvnChangeItemSorter.SvnChangeVerboseActionEnum.Delete

                resultingActionsCount = tfsWorkspace.PendDelete({tfsItemLocalFullPath},
                                             Microsoft.TeamFoundation.VersionControl.Client.RecursionType.None,
                                             Microsoft.TeamFoundation.VersionControl.Client.LockLevel.Unchanged,
                                             True)

            Case SvnWorker.SvnChangeItemSorter.SvnChangeVerboseActionEnum.Replace

                messageManager.ThrowMessageAndAddToRevisionImportLog(MessageManagerClass.MessageKindEnum.Warning,
                                    "Found a 'replace', this action will be skipped as it is not implemented. ")

            Case SvnWorker.SvnChangeItemSorter.SvnChangeVerboseActionEnum.None

                messageManager.ThrowMessageAndAddToRevisionImportLog(MessageManagerClass.MessageKindEnum.Warning,
                                    "Found a 'none' action, this action will be skipped as it is not implemented, but this should be fine. ")

            Case Else

                Throw New NotSupportedException(String.Format("Unsupported SVN action: {0}.", verboseAction.ToString))

        End Select

        Return updateSvnAfterReplay

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="tfsWorkspace"></param>
    ''' <param name="svnLog"></param>
    ''' <param name="mappedUser"></param>
    ''' <param name="svn2tfsRevisionMap"></param>
    ''' <returns></returns>
    ''' <remarks>Returns TFS changset. -1 if no changeset has been checked-in.</remarks>
    Private Function tfsCheckIn(ByVal tfsWorkspace As Workspace,
                           ByVal svnLog As SvnWorker.LogItem,
                           ByVal mappedUser As String,
                           ByVal svn2tfsRevisionMap As RevisionMap) As Long

        'TODO: Set checkin time

        Dim tfsPendingChanges = tfsWorkspace.GetPendingChanges()

        If tfsPendingChanges.Count > 0 Then

            'comment
            Dim tfsComment = svnLog.Comment

            'notes
            Dim tfsNoteList = New List(Of Microsoft.TeamFoundation.VersionControl.Client.CheckinNoteFieldValue)

            tfsNoteList.Add(New Microsoft.TeamFoundation.VersionControl.Client.CheckinNoteFieldValue(
                TfsNoteFieldsHelper.GetNoteFieldName(TfsNoteFieldsHelper.NoteFieldsEnum.Revision),
                svnLog.Revision))

            tfsNoteList.Add(New Microsoft.TeamFoundation.VersionControl.Client.CheckinNoteFieldValue(
                TfsNoteFieldsHelper.GetNoteFieldName(TfsNoteFieldsHelper.NoteFieldsEnum.Date),
                svnLog.Date))

            tfsNoteList.Add(New Microsoft.TeamFoundation.VersionControl.Client.CheckinNoteFieldValue(
                TfsNoteFieldsHelper.GetNoteFieldName(TfsNoteFieldsHelper.NoteFieldsEnum.Log),
                messageManager.GetRevisionLogMessages))

            Dim tfsNote = New Microsoft.TeamFoundation.VersionControl.Client.CheckinNote(tfsNoteList.ToArray)

            'checkin
            Dim tfsChangeSet = tfsWorkspace.CheckIn(tfsPendingChanges,
                                                         mappedUser,
                                                         tfsComment,
                                                         tfsNote,
                                                         Nothing,
                                                         Nothing)

            svn2tfsRevisionMap.Add(svnLog.Revision, tfsChangeSet)

            Return tfsChangeSet

        Else

            Return -1

        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="rootDirectory"></param>
    ''' <param name="workspace">If <c>Nothing</c>, deletes will not be pendeded to <c>workspace</c>.</param>
    ''' <param name="excludeSvnDirectories">Set to True to preserve .svn directories and their content</param>
    ''' <returns>True if directory has been emptied.</returns>
    ''' <remarks></remarks>
    Private Shared Function DeleteTfsWorkspaceContent(ByVal rootDirectory As String,
                                           ByVal workspace As Workspace,
                                           ByVal excludeSvnDirectories As Boolean) As Boolean

        'Attrib -*
        For Each file In My.Computer.FileSystem.GetFiles(rootDirectory,
                                                         FileIO.SearchOption.SearchTopLevelOnly)
            Dim fileInfo = New IO.FileInfo(file)
            fileInfo.Attributes = 0
        Next
        For Each directory In My.Computer.FileSystem.GetDirectories(rootDirectory,
                                                                    FileIO.SearchOption.SearchTopLevelOnly)
            Dim directoryInfo = New IO.DirectoryInfo(directory)
            directoryInfo.Attributes = 0
        Next

        'Delete root files
        For Each file In My.Computer.FileSystem.GetFiles(rootDirectory)
            Dim fileInfo = New IO.FileInfo(file)
            fileInfo.Delete()

            If workspace IsNot Nothing Then
                workspace.PendDelete(file)
            End If

        Next

        'Recursively delete directories
        Dim rootHasBeenEmptied As Boolean = True

        For Each subDirectory In My.Computer.FileSystem.GetDirectories(rootDirectory)

            Dim subDirectoryInfo = New IO.DirectoryInfo(subDirectory)

            If excludeSvnDirectories AndAlso subDirectoryInfo.Name.ToLower = ".svn".ToLower Then

                'this has to be excluded
                rootHasBeenEmptied = False

            Else

                'this has to be deleted
                Dim isSubDirectoryEmpty = DeleteTfsWorkspaceContent(subDirectory, workspace, excludeSvnDirectories)

                'if content is now empty, delete the folder from filesystem
                If isSubDirectoryEmpty Then
                    subDirectoryInfo.Delete()
                End If

                'delete folder from TFS anyway
                If workspace IsNot Nothing Then
                    workspace.PendDelete(subDirectory,
                                                 Microsoft.TeamFoundation.VersionControl.Client.RecursionType.Full)
                End If

            End If

        Next

        'Returns
        Return rootHasBeenEmptied

    End Function

    Private Shared Sub CheckRevisionValidity(ByVal revision As Long, ByVal totalRevisions As Long)

        If revision > totalRevisions Then
            Throw New IndexOutOfRangeException("Revision number can not be greater than last revision.")
        End If

        If revision < 1 Then
            Throw New IndexOutOfRangeException("Revision number can not be less than 1.")
        End If

    End Sub

End Class
