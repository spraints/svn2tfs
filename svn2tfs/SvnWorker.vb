Imports SharpSvn

Public Class SvnWorker

    Public ReadOnly RepositoryUri As Uri
    Public ReadOnly UserName As String
    Public ReadOnly Password As String
    Public ReadOnly Directory As String
    Public ReadOnly Property Uri As Uri
        Get
            Return New Uri(String.Concat(RepositoryUri.AbsoluteUri, Directory))
        End Get
    End Property

    Private client As SvnClient

    Public Structure RevisionInfo
        Public Author As String
        Public Revision As Long
        Public Comment As String
        Public [Date] As DateTime
    End Structure

    Public Sub New(ByVal repositoryUri As Uri, ByVal directory As String, ByVal username As String, ByVal password As String)

        'check input
        If repositoryUri Is Nothing Then
            Throw New ArgumentNullException("repositoryUri")
        End If

        'set 
        Me.RepositoryUri = repositoryUri
        Me.UserName = username
        Me.Password = password
        Me.Directory = directory

        'create svn client
        CreateClient()

        'open
        Open()

    End Sub

    Private Sub CreateClient()

        client = New SvnClient

        client.Authentication.ClearAuthenticationCache()

        client.Authentication.DefaultCredentials =
            New System.Net.NetworkCredential(UserName,
                                             Password)

    End Sub

    Private Sub Open()

        Console.Write("Connecting to SubVersion... ")

        Dim currentRevision = GetCurrentRevision()

        Console.WriteLine("Connected.")

    End Sub

    Public Function GetCurrentRevision() As Long

        Dim info As SvnInfoEventArgs
        If Not client.GetInfo(Uri, info) Then
            Throw New ApplicationException("Unable to execute GetInfo method on SvnClient.")
        End If
        Return info.Revision

    End Function

    Public Function GetLog(ByVal fromRevision As Long, ByVal toRevision As Long, ByVal getChanges As Boolean) As List(Of LogItem)

        Dim ret = New List(Of LogItem)

        For revision As Long = fromRevision To toRevision

            ret.Add(GetLog(revision, getChanges))

        Next

        Return ret

    End Function

    Public Function GetLog(ByVal revision As Long, ByVal getChanges As Boolean) As LogItem

        Dim args = New SvnLogArgs
        args.RetrieveChangedPaths = getChanges
        args.RetrieveAllProperties = True
        args.RetrieveMergedRevisions = True

        args.Start = New SvnRevision(revision)
        args.End = New SvnRevision(revision)

        getLogResetEvent = New System.Threading.ManualResetEvent(False)

        If Not client.Log(Uri, args, AddressOf GetLogReturnEventHandler) Then
            Throw New ApplicationException("Unable to execute Log method on SvnClient.")
        End If

        getLogResetEvent.WaitOne()

        Return getLogResult

    End Function

    Private getLogResult As LogItem

    Private getLogResetEvent As System.Threading.ManualResetEvent

    Public Class LogItem
        Public Revision As Long
        Public Author As String
        Public Changes As SharpSvn.Implementation.SvnChangeItemCollection
        Public Comment As String
        Public [Date] As Date
    End Class

    Public Sub GetLogReturnEventHandler(ByVal sender As Object, ByVal eventArgs As SvnLogEventArgs)

        Dim ret As New LogItem

        With ret

            .Revision = eventArgs.Revision
            .Author = eventArgs.Author
            .Changes = eventArgs.ChangedPaths
            .Comment = eventArgs.LogMessage
            .Date = eventArgs.Time

        End With

        getLogResult = ret

        getLogResetEvent.Set()

    End Sub

    'Public Function GetRevisionInfo(ByVal revision As Long) As RevisionInfo

    '    Dim ret As RevisionInfo
    '    ret.Revision = revision

    '    Const AuthorKey As String = "svn:author"
    '    Const CommentKey As String = "svn:log"
    '    Const DateKey As String = "svn:date"

    '    Dim client = New SvnClient
    '    Dim properties As SvnPropertyCollection
    '    client.GetRevisionPropertyList(uri, revision, properties)

    '    Dim propertiesEnumerator = properties.GetEnumerator
    '    While propertiesEnumerator.MoveNext

    '        Dim [property] = propertiesEnumerator.Current

    '        Select Case [property].Key.ToLower
    '            Case AuthorKey.ToLower
    '                ret.Author = [property].StringValue
    '            Case CommentKey.ToLower
    '                ret.Comment = [property].StringValue
    '            Case DateKey.ToLower
    '                ret.Date = Date.Parse([property].StringValue)
    '            Case Else
    '                'NOOP: ignore
    '        End Select

    '    End While

    '    Return ret

    'End Function

    ' ''' <summary>
    ' ''' Returns a list of all revision info. Might take a while.
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Function GetAllRevisionInfo() As List(Of RevisionInfo)

    '    Dim ret = New List(Of RevisionInfo)

    '    For revision = 1 To GetCurrentRevision()

    '        ret.Add(GetRevisionInfo(revision))

    '        Debug.WriteLine(revision)

    '    Next

    '    Return ret

    'End Function

    ''' <summary>
    ''' Returns a list of all involved users from revision <c>fromRevision</c> to revision <c>toRevision</c>. This might take a while, depending on the lenght of the range specified.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInvolvedUsers(ByVal fromRevision As Long, ByVal toRevision As Long) As List(Of String)

        Dim users = New List(Of String)

        For Each logItem In GetLog(fromRevision, toRevision, False)

            users.Put(logItem.Author)

        Next

        Return users

    End Function

    ' ''' <summary>
    ' ''' Returns a list of all involved users. Since it scans all revisions it might take a while.
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Function GetInvolvedUsers() As List(Of String)

    '    Return GetInvolvedUsers(1, GetCurrentRevision)

    'End Function

    Public Sub Revert(ByVal folder As String)

        If Not client.Revert(folder) Then
            Throw New ApplicationException("Unable to execute Revert method on SvnClient.")
        End If

    End Sub

    Public Sub Checkout(ByVal folder As String, ByVal revision As Long)

        Dim args As New SvnCheckOutArgs
        args.Revision = New SvnRevision(revision)

        If Not client.CheckOut(Uri, folder, args) Then
            Throw New ApplicationException("Unable to execute CheckOut method on SvnClient.")
        End If

    End Sub

    Private updateConflicts As SvnConflictEventArgs

    Public Class ConflictException
        Inherits SvnException

        Public ReadOnly Conficts As SvnConflictEventArgs

        Public Sub New(ByVal conflicts As SvnConflictEventArgs)
            Me.Conficts = conflicts
        End Sub

        Public Overrides ReadOnly Property Message As String
            Get
                Return "SVN update operation encountered conflicts."
            End Get
        End Property

    End Class

    Public Sub Update(ByVal folder As String, ByVal revision As Long)

        updateConflicts = Nothing

        AddHandler client.Conflict, AddressOf Update_OnConflict

        Dim args As New SvnUpdateArgs
        args.Revision = New SvnRevision(revision)
        args.Depth = SvnDepth.Unknown

        If Not client.Update(folder, args) Then
            Throw New ApplicationException("Unable to execute Update method on SvnClient.")
        End If

        If updateConflicts IsNot Nothing Then
            Throw New ConflictException(updateConflicts)
        End If

    End Sub

    Private Sub Update_OnConflict(ByVal sender As Object, ByVal e As SvnConflictEventArgs)
        e.Detach()
        updateConflicts = e
    End Sub

    Public Function GetLocalWorkspaceDirectory() As String

        Static cachedPath As String = Nothing

        If cachedPath Is Nothing Then

            Dim workspaceName = String.Concat("svn2tfs_svn_", Now.Ticks)

            cachedPath = IO.Path.Combine(Common.TempPath, workspaceName)

        End If

        Return cachedPath

    End Function

    Public Class SvnChangeItemSorter

        ''' <summary>
        ''' More exaustive enum of changes. As a benefit, the enum is sorted on action priority
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum SvnChangeVerboseActionEnum

            Add = 0 'First, add things
            Branch = 1 'Second, branch: if user modifies files, they have to be already mapped to TFS

            'Then all the others
            Replace = 2
            Modify = 3
            Delete = 4
            None = 5

        End Enum

        ''' <summary>
        ''' Converts an <c>SvnChangeItem</c> item to a <c>SvnChangeActionInternalEnum</c> item.
        ''' </summary>
        ''' <param name="item"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSvnChangeVerboseAction(ByVal item As SvnChangeItem) As SvnChangeVerboseActionEnum

            Select Case item.Action
                Case SvnChangeAction.Add
                    If Not String.IsNullOrEmpty(item.CopyFromPath) Then
                        If item.CopyFromRevision > 0 Then
                            Return SvnChangeVerboseActionEnum.Branch
                        Else
                            Throw New NotSupportedException(String.Format("The change action '{0}' is not supported with these parameters.",
                                                            item.Action.ToString))
                        End If
                    Else
                        Return SvnChangeVerboseActionEnum.Add
                    End If
                Case SvnChangeAction.Delete
                    Return SvnChangeVerboseActionEnum.Delete
                Case SvnChangeAction.Modify
                    Return SvnChangeVerboseActionEnum.Modify
                Case SvnChangeAction.None
                    Return SvnChangeVerboseActionEnum.None
                Case SvnChangeAction.Replace
                    Return SvnChangeVerboseActionEnum.Replace
                Case Else
                    Throw New NotSupportedException(String.Format("The change action '{0}' is not recognized.",
                                                    item.Action.ToString))
            End Select

        End Function

        Public Shared Function SortChanges(ByVal changes As List(Of SvnChangeItem)) As List(Of SvnChangeItem)

            'we want a list sorted by path, then by action
            Dim ret = New List(Of SvnChangeItem)(changes)

            '1. sort on action
            ret.Sort(New SvnChangeItemActionComparer)

            '2. sort on path
            ret.Sort(New SvnChangeItemPathComparer)

            'returns
            Return ret

        End Function

        ''' <summary>
        ''' Sort change items on action replay priority.
        ''' </summary>
        ''' <remarks></remarks>
        Private Class SvnChangeItemActionComparer
            Inherits Comparer(Of SvnChangeItem)

            Public Overloads Overrides Function Compare(ByVal x As SharpSvn.SvnChangeItem, ByVal y As SharpSvn.SvnChangeItem) As Integer

                Dim xPos As Integer = GetSvnChangeVerboseAction(x)

                Dim yPos As Integer = GetSvnChangeVerboseAction(y)

                Return (xPos - yPos)

            End Function

        End Class

        ''' <summary>
        ''' Sort change items on path priority.
        ''' </summary>
        ''' <remarks></remarks>
        Private Class SvnChangeItemPathComparer
            Inherits Comparer(Of SvnChangeItem)

            Public Overloads Overrides Function Compare(ByVal x As SharpSvn.SvnChangeItem, ByVal y As SharpSvn.SvnChangeItem) As Integer

                Return x.Path.CompareTo(y.Path)

            End Function

        End Class

    End Class

End Class