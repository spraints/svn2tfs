Imports svn2tfs.Importer

Public Class MainWindow

    Private _uiState As uiStateEnum

    Private cancelled As Boolean

    Private Enum uiStateEnum
        Idle
        Working
    End Enum

    Public Class UserMap

        Public SvnUserName As String
        Public TfsUserName As String

    End Class

#Region " UI state "

    ''' <summary>
    ''' Changes UI elements states (i.e. Enabled)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property uiState As uiStateEnum
        Get
            Return _uiState
        End Get
        Set(ByVal value As uiStateEnum)

            _uiState = value

            upperTabControl.Enabled = (value = uiStateEnum.Idle)
            importButton.Enabled = (value = uiStateEnum.Idle)
            aboutButton.Enabled = (value = uiStateEnum.Idle)

            abortButton.Enabled = False '(value = uiStateEnum.Working)

            If uiState = uiStateEnum.Working Then

                ClearWarnings()

                SetProgress(0, 0)
                SetProgress(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Indeterminate)

            Else

                SetProgress(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress)

            End If

            cancelled = False

        End Set
    End Property

    ''' <summary>
    ''' Update controls' state
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateUI()

        'Custom temporary folder
        customTemporaryFolderTextBox.Enabled = customTemporaryFolderCheckBox.Checked
        customTemporaryFolderBrowseButton.Enabled = customTemporaryFolderCheckBox.Checked

    End Sub

    <PreEmptive.Attributes.Teardown()>
    Private Sub MainWindow_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'NOOP
    End Sub

    <PreEmptive.Attributes.Setup(CustomEndpoint:="so-s.info/PreEmptive.Web.Services.Messaging/MessagingServiceV2.asm")>
    Private Sub MainWindow_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'set console out
        Console.SetOut(New TextBoxWriter(Me.logTextBox))

        'set title
        Me.Text = String.Format("{0} {1}", My.Application.Info.Title, GetVersion(False))

        'set state
        UpdateUI()
        Me.uiState = uiStateEnum.Idle
        ClearWarnings()

        'ask agreement
        If Not My.Settings.hasAcceptedAgreement Then
            AgreementDialog.ShowDialog(Me)
            My.Settings.hasAcceptedAgreement = (AgreementDialog.DialogResult = Windows.Forms.DialogResult.OK)
            If Not My.Settings.hasAcceptedAgreement Then End
            My.Settings.Save()
        End If

    End Sub

#End Region

#Region " Validation and import "

    <PreEmptive.Attributes.Feature("TestSVNAction")>
    Private Sub testSvnButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles testSvnButton.Click

        uiState = uiStateEnum.Working

        Console.WriteLine("Testing SubVersion connection...")

        Try

            Dim svn = CreateSvnWorker()

            Console.WriteLine("There are {0} revisions.", svn.GetCurrentRevision)

            'Check revisions are ok
            Dim fromRevision = ParseTextToRevisionNumber(svn, fromRevisionTextBox.Text)
            Dim toRevision = ParseTextToRevisionNumber(svn, toRevisionTextBox.Text)
            If fromRevision > toRevision Then Throw New ArgumentException("Invalid revision range. 'From revision' must be less or equal than 'To revision'.")
            Dim svnRevisionCount = svn.GetCurrentRevision

            If userMapFileNameTextBox.Text <> String.Empty Then

                Console.WriteLine("Loading user map file...")

                Dim userMapList = LoadUserMapFromFile(userMapFileNameTextBox.Text)

                Console.WriteLine("Searching involved users from r{0} to r{1}... (this might take a while)", fromRevision, toRevision)

                Dim users = svn.GetInvolvedUsers(fromRevision, toRevision)

                Dim someUserNotMapped As Boolean = False
                For Each user In users

                    Dim userIsMapped = (userMapList.ContainsKey(user))

                    Console.WriteLine("{0}{1}",
                                       user,
                                       IIf(Not userIsMapped, " (User not mapped)", String.Empty))

                    someUserNotMapped = someUserNotMapped Or (Not userIsMapped)

                Next

                Console.WriteLine()
                If someUserNotMapped Then
                    Console.WriteLine("Warning: some user has not been mapped correctly. Fix this or we will FAIL!")
                Else
                    Console.WriteLine("All users are correctly mapped.")
                End If

                Console.WriteLine()
                Console.WriteLine("Check completed.")

            End If

        Catch ex As Exception

            Console.WriteLine()
            Console.WriteLine("Failed: {0}", ex.Message)

            If TypeOf ex Is SharpSvn.SvnRepositoryIOException Then

                Dim svnEx = DirectCast(ex, SharpSvn.SvnRepositoryIOException)

                If svnEx.SvnErrorCode = SharpSvn.SvnErrorCode.SVN_ERR_RA_DAV_REQUEST_FAILED Then

                    Console.WriteLine("Are you using the wrong protocol?")

                End If

            End If

        End Try

        Console.WriteLine()

        uiState = uiStateEnum.Idle

    End Sub

    <PreEmptive.Attributes.Feature("TestTFSAction")>
    Private Sub testTfsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles testTfsButton.Click

        uiState = uiStateEnum.Working

        Console.WriteLine("Testing Team Foundation Server connection...")

        Try

            Dim tfs = CreateTfsWorker()

            DumpTfsProjectCollections(tfs)

            DumpTfsTeamProjects(tfs, tfsCollectionTextBox.Text)

            tfs.TeamProjectExistsOrThrowException(
                tfsCollectionTextBox.Text,
                tfsProjectTextBox.Text)

            Console.WriteLine()
            Console.WriteLine("Connected to team project successfully.")

        Catch ex As Exception

            Console.WriteLine()
            Console.WriteLine("Failed: {0}", ex.Message)

        End Try

        Console.WriteLine()

        uiState = uiStateEnum.Idle

    End Sub

    Private currentRevisionWarnings As String = String.Empty

    <PreEmptive.Attributes.Feature("ImportAction")>
    Private Sub importButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles importButton.Click

        My.Settings.Save()

        Me.uiState = uiStateEnum.Working

        Try

            'checks that temporary folder exits
            If Not (New IO.DirectoryInfo(Common.TempPath)).Exists Then
                Throw New IO.DirectoryNotFoundException("Custom temporary path not found.")
            End If

            'let's go
            Dim importer = New Importer

            AddHandler importer.Message, AddressOf import_message
            AddHandler importer.ProgressChange, AddressOf SetProgress
            AddHandler importer.ProgressStateChange, AddressOf SetProgress

            Dim fromRevision = Long.Parse(fromRevisionTextBox.Text)

            Dim toRevision As Nullable(Of Long)
            If (toRevisionTextBox.Text.ToLower = "head".ToLower) Then
                toRevision = Nothing
            Else
                toRevision = Long.Parse(toRevisionTextBox.Text)
            End If

            Dim authors As New Dictionary(Of String, String)
            If (userMapFileNameTextBox.Text <> "") Then
                authors = LoadUserMapFromFile(userMapFileNameTextBox.Text)
            End If

            importer.Import(New Uri(svnUrlTextBox.Text),
                            svnUserNameTextBox.Text,
                            svnPasswordTextBox.Text,
                            svnDirectoryTextBox.Text,
                            fromRevision,
                            toRevision,
                            New Uri(tfsUrlTextBox.Text),
                            tfsCollectionTextBox.Text,
                            tfsProjectTextBox.Text,
                            tfsDirectoryTextBox.Text,
                            authors,
                            cleanUpTFSCheckBox.Checked,
                            overrideFirstRevisionRadioButton.Checked)

        Catch ex As Exception

            Dim sb = New System.Text.StringBuilder
            sb.AppendFormat(ex.Message)
            If ex.InnerException IsNot Nothing Then
                sb.AppendLine()
                sb.AppendFormat("({0})", ex.InnerException.Message)
            End If

            Console.WriteLine(sb.ToString)

            SetProgress(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Error)

        End Try

        Console.WriteLine("Exited.")

        Console.WriteLine()

        Me.uiState = uiStateEnum.Idle

    End Sub

    Private Sub import_message(ByVal message As String)

        AddWarning(message)

    End Sub

#End Region

#Region " Simple buttons and events "

    '<PreEmptive.Attributes.Feature("Cancel")>
    Private Sub cancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles abortButton.Click

        cancelled = True

        DirectCast(sender, Button).Enabled = False

        Console.WriteLine("Cancelling...")

    End Sub

    '<PreEmptive.Attributes.Feature("MapFileHelp")>
    Private Sub helpUserMappingButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles helpUserMappingButton.Click

        Console.WriteLine("Here it is an example of map file:")

        Dim exampleUserMapList = New List(Of UserMap)
        exampleUserMapList.Add(New UserMap With {.SvnUserName = "svnUser1", .TfsUserName = "tfsUser1"})
        exampleUserMapList.Add(New UserMap With {.SvnUserName = "svnUser2", .TfsUserName = "tfsUser2"})

        Dim serializationStream = New IO.MemoryStream
        Dim serializer = New Xml.Serialization.XmlSerializer(exampleUserMapList.GetType)
        serializer.Serialize(serializationStream, exampleUserMapList)

        serializationStream.Seek(0, IO.SeekOrigin.Begin)
        Dim reader = New IO.StreamReader(serializationStream)
        Console.WriteLine(reader.ReadToEnd)
        Console.WriteLine()

    End Sub

    '<PreEmptive.Attributes.Feature("OpenMapFile")>
    Private Sub openUserMappingFileNameButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles openUserMappingFileNameButton.Click

        If userMapOpenFileDialog.ShowDialog = DialogResult.OK Then

            userMapFileNameTextBox.Text = userMapOpenFileDialog.FileName

            Console.WriteLine("User mapping file changed. You may check if all users are mapped pressing the SubVersion 'Check' button.")
            Console.WriteLine()

        End If

    End Sub

    <PreEmptive.Attributes.Feature("OpenAboutDialog")>
    Private Sub aboutButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles aboutButton.Click
        AboutDialog.ShowDialog(Me)
    End Sub

    '<PreEmptive.Attributes.Feature("FillBug")>
    Private Sub bugCommand_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bugCommand.Click
        Diagnostics.Process.Start("http://svn2tfs.codeplex.com/WorkItem/Create.aspx")
    End Sub

    <PreEmptive.Attributes.Feature("ErrorListItemDoubleClick")>
    Private Sub errorsListView_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles errorsListView.MouseDoubleClick
        Dim listView = DirectCast(sender, ListView)
        If listView.SelectedItems.Count = 1 Then
            Dim item = listView.SelectedItems(0)
            MsgBox(item.Text, MsgBoxStyle.Information)
        End If
    End Sub

    <PreEmptive.Attributes.Feature("CleanUpTFSOption")>
    Private Sub cleanUpTFSCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cleanUpTFSCheckBox.CheckedChanged
        'NOOP
    End Sub

    <PreEmptive.Attributes.Feature("StandardImportOption")>
    Private Sub standardImportRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles standardImportRadioButton.CheckedChanged
        'NOOP
    End Sub

    <PreEmptive.Attributes.Feature("OverrideFirstRevisionImportOption")>
    Private Sub overrideFirstRevisionRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles overrideFirstRevisionRadioButton.CheckedChanged
        'NOOP
    End Sub

#End Region

#Region " Helpers "

    Public Function ParseTextToRevision(ByVal revisionString As String) As Nullable(Of Long)

        Dim comparer = StringComparer.CurrentCultureIgnoreCase

        If comparer.Compare(revisionString, "HEAD") = 0 Then
            Return Nothing
        End If

        Dim revisionNumber = Long.Parse(revisionString)

        If revisionNumber < 1 Then
            Throw New IndexOutOfRangeException("Revision number can not be less than 1.")
        End If

        Return revisionNumber

    End Function

    <PreEmptive.Attributes.Feature("WarningAdded")>
    Private Sub AddWarning(ByVal message As String)

        Dim lvi = New ListViewItem(message)
        errorsListView.Items.Add(lvi)

        UpdateWarningCount()

    End Sub

    '<PreEmptive.Attributes.Feature("WarningsCleared")>
    Private Sub ClearWarnings()

        errorsListView.Items.Clear()

        UpdateWarningCount()

    End Sub

    Private Sub UpdateWarningCount()

        WarningsTabPage.Text = String.Format("Warnings ({0})", errorsListView.Items.Count)

    End Sub

    Private Function CreateTfsWorker(Optional ByVal userToImpersonate As String = Nothing) As TfsWorker

        Return New TfsWorker(
                            New Uri(tfsUrlTextBox.Text),
                            userToImpersonate)

    End Function

    Private Function CreateSvnWorker() As SvnWorker

        Return New SvnWorker(New Uri(svnUrlTextBox.Text), svnDirectoryTextBox.Text, svnUserNameTextBox.Text, svnPasswordTextBox.Text)

    End Function

    Private Shared Sub DumpTfsProjectCollections(ByVal tfs As TfsWorker)

        Console.WriteLine("Reading project collections list...")

        For Each projectCollection In tfs.GetProjectCollectionNames

            Console.WriteLine("Found project collection: '{0}'.",
                              projectCollection)

        Next

    End Sub

    Private Shared Sub DumpTfsTeamProjects(ByVal tfs As TfsWorker, ByVal teamCollectionDisplayName As String)

        Console.WriteLine("Reading team projects list...")

        Dim teamProjects = tfs.GetTeamProjectNames(teamCollectionDisplayName)

        For Each teamProject In teamProjects

            Console.WriteLine("Found project '{0}'.", teamProject)

        Next

    End Sub

    Private Shared Function LoadUserMapFromFile(ByVal path As String) As Dictionary(Of String, String)

        'Load
        Dim fileStream = New IO.FileStream(path, IO.FileMode.Open)
        Dim serializer = New Xml.Serialization.XmlSerializer(GetType(List(Of UserMap)))
        Dim userMapList = DirectCast(serializer.Deserialize(fileStream), List(Of UserMap))
        fileStream.Close()

        'Returns
        Return userMapList.ToDictionary

    End Function

    Private Sub SetProgress(ByVal value As Long,
                            ByVal maximum As Long)

        If Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported Then

            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressValue(value, maximum)

        End If

    End Sub

    Private Sub SetProgress(ByVal state As Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState)

        If Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported Then

            Try

                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(state)

            Catch ex As Exception

                'NOOP

            End Try

        End If

    End Sub

    Private Shared Function ParseTextToRevisionNumber(ByVal svn As SvnWorker, ByVal revisionString As String) As Long

        Dim comparer = StringComparer.CurrentCultureIgnoreCase

        If comparer.Compare(revisionString, "HEAD") = 0 Then
            Return svn.GetCurrentRevision
        End If

        Dim revisionNumber = Integer.Parse(revisionString)

        If revisionNumber < 1 Then
            Throw New IndexOutOfRangeException("Revision number can not be less than 1.")
        End If

        If revisionNumber > svn.GetCurrentRevision Then
            Throw New IndexOutOfRangeException("Revision number can not be greater than last revision.")
        End If

        Return revisionNumber

    End Function

#End Region

#Region " Custom temporary folder "

    <PreEmptive.Attributes.Feature("UseCustomTemporaryFolderOption")>
    Private Sub customTemporaryFolderCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles customTemporaryFolderCheckBox.CheckedChanged
        UpdateUI()
    End Sub

    '<PreEmptive.Attributes.Feature("BrowseCustomTemporaryFolder")>
    Private Sub customTemporaryFolderBrowseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles customTemporaryFolderBrowseButton.Click

        If customTemporaryFolderOpenDialog.ShowDialog = DialogResult.OK Then

            customTemporaryFolderTextBox.Text = customTemporaryFolderOpenDialog.SelectedPath

        End If

    End Sub

#End Region

End Class
