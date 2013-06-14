Imports System.IO
Imports Microsoft.TeamFoundation.Framework.Client
Imports Microsoft.TeamFoundation.VersionControl.Client
Imports Microsoft.TeamFoundation.WorkItemTracking.Client
Imports Microsoft.TeamFoundation.Client
Imports Microsoft.TeamFoundation.Framework.Common
Imports System.Collections.ObjectModel

Public Class TfsWorker
    Implements IDisposable

    ''' <summary>
    ''' If specified, requests inside project collections will be impersonated by him.
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly ImpersonificationUserName As String

    ''' <summary>
    ''' List of created workspaces, used to delete them when disposing the worker (we want /temporary/ workspaces).
    ''' </summary>
    ''' <remarks></remarks>
    Private createdWorkspaces As New List(Of Workspace)

    Public ReadOnly LocalWorkspaceDirectory As String

    Public tfs As TfsConfigurationServer
    Private tfsLocationService As ILocationService

    ''' <summary>
    ''' Connects to Team Foundation Server using specified URI and optional impersonificationuserName.
    ''' </summary>
    ''' <param name="uri"></param>
    ''' <param name="impersonificationUserName">If specified, requests inside project collections will be impersonated by him.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal uri As Uri, ByVal localWorkspacePath As String, Optional ByVal impersonificationUserName As String = Nothing)

        Console.Write("Connecting to Team Foundation Server... ")

        tfs = TfsConfigurationServerFactory.GetConfigurationServer(
                uri,
                New UICredentialsProvider)

        tfsLocationService = tfs.GetService(Of ILocationService)()

        Me.ImpersonificationUserName = impersonificationUserName

        Me.LocalWorkspaceDirectory = localWorkspacePath

        If impersonificationUserName Is Nothing Then
            Console.WriteLine("Connected as {0}.",
                              tfs.AuthorizedIdentity.DisplayName.ToString)
        Else
            Console.WriteLine("Connected as {0}, will impersonate {1}.",
                              tfs.AuthorizedIdentity.DisplayName.ToString,
                              impersonificationUserName)
        End If

    End Sub

    Public ReadOnly Property AuthorizedIdentityDisplayName As String
        Get
            Return tfs.AuthorizedIdentity.DisplayName.ToString
        End Get
    End Property

    Private Function GetProjectCollectionCatalogNodes() As ReadOnlyCollection(Of CatalogNode)

        Dim nodeType As Guid() = {CatalogResourceTypes.ProjectCollection}

        Dim projectCollectionsNodes = tfs.CatalogNode.QueryChildren(
                nodeType,
                False,
                CatalogQueryOptions.None)

        Return projectCollectionsNodes

    End Function

    Private Function GetProjectCollection(ByVal displayName As String) As TfsTeamProjectCollection

        Dim projectCollectionCatalogNodes = GetProjectCollectionCatalogNodes()

        For Each node In projectCollectionCatalogNodes

            If node.Resource.DisplayName.ToLower = displayName.ToLower Then

                Return GetProjectCollection(node, ImpersonificationUserName)

            End If

        Next

        Throw New ItemNotFoundException(String.Format("Unable to find '{0}' project collection.", displayName))

    End Function

    ''' <summary>
    ''' Gets a project collection. If userName is specified, the returned object is impersonated by him.
    ''' </summary>
    ''' <param name="projectCollectionNode"></param>
    ''' <param name="impersonificationUserName">Username to be impersonated. Nothing or String.Empty otherwise.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetProjectCollection(ByVal projectCollectionNode As CatalogNode, ByVal impersonificationUserName As String) As TfsTeamProjectCollection

        Dim projectCollectionLocationDefinition = projectCollectionNode.Resource.ServiceReferences("Location")

        Dim projectCollectionURI = New Uri(tfsLocationService.LocationForCurrentConnection(
                                           projectCollectionLocationDefinition))

        Dim projectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(projectCollectionURI)

        If Not String.IsNullOrEmpty(impersonificationUserName) Then

            'Returns impersonated project collection

            Dim identityManagementService = projectCollection.GetService(Of IIdentityManagementService)()

            Dim identity = identityManagementService.ReadIdentity(
                                IdentitySearchFactor.AccountName,
                                impersonificationUserName,
                                MembershipQuery.None,
                                ReadIdentityOptions.None)

            If identity Is Nothing Then
                Throw New ApplicationException(String.Format("Unable to find user {0} on Team Foundation Server.", impersonificationUserName))
            End If

            Dim impersonatedProjectCollection = New TfsTeamProjectCollection(projectCollection.Uri,
                                                                             identity.Descriptor)

            Return impersonatedProjectCollection

        Else

            'Returns current user project collection

            Return projectCollection

        End If

    End Function

    Public Function GetProjectCollectionNames() As IEnumerable(Of String)

        Dim projectCollectionsNodes = GetProjectCollectionCatalogNodes()

        Dim displayNames = From node In projectCollectionsNodes Select node.Resource.DisplayName

        Return displayNames

    End Function

    Public Function GetTeamProjectNames(ByVal projectCollectionDisplayName As String) As IEnumerable(Of String)

        Dim projectCollection = GetProjectCollection(projectCollectionDisplayName)

        Dim versionControlService = projectCollection.GetService(Of VersionControlServer)()

        Dim teamProjects = versionControlService.GetAllTeamProjects(False)

        Dim teamProjectNames = From tp In teamProjects Select tp.Name

        Return teamProjectNames

    End Function

    Private Shared Function GetTeamProject(ByVal projectCollection As TfsTeamProjectCollection, ByVal teamProjectName As String) As TeamProject

        Dim versionControlService = projectCollection.GetService(Of VersionControlServer)()

        For Each teamProject In versionControlService.GetAllTeamProjects(False)

            If teamProject.Name.ToLower = teamProjectName.ToLower Then

                Return teamProject

            End If

        Next

        Throw New ItemNotFoundException(String.Format("Unable to find '{0}' team project inside the '{1}' project collection.",
                                                      teamProjectName,
                                                      projectCollection.CatalogNode.Resource.DisplayName))

    End Function

    ''' <summary>
    ''' Throws an Exception if team project does not exist.
    ''' </summary>
    ''' <param name="projectCollectionDisplayName"></param>
    ''' <param name="teamProjectName"></param>
    ''' <remarks></remarks>
    Public Sub TeamProjectExistsOrThrowException(ByVal projectCollectionDisplayName As String, ByVal teamProjectName As String)

        Dim projectCollection = GetProjectCollection(projectCollectionDisplayName)

        Dim teamProject = GetTeamProject(projectCollection, teamProjectName)

    End Sub

    Private Shared Function GetWorkspaceName() As String
        Return String.Format("svn2tfsTempWorkspace.{0}.{1}.{2}",
                                          Environment.MachineName,
                                          AppDomain.CurrentDomain.FriendlyName,
                                          "tfs")
    End Function

    Private Shared Sub PrepareWorkspace(ByVal versionControlService As VersionControlServer,
                                        ByVal workspaceName As String,
                                        ByVal localFolder As String)

        'TODO: I think we remove too many workspaces here :P

        'Ensures there is no existing workspace named workspaceName
        Try
            versionControlService.DeleteWorkspace(workspaceName, versionControlService.AuthorizedUser)
        Catch
            'NOOP
        End Try

        'Ensures there is no existing workspace mapped to localFolder
        Try
            Dim folderWorkspace = versionControlService.GetWorkspace(localFolder)
            If folderWorkspace IsNot Nothing Then
                versionControlService.DeleteWorkspace(folderWorkspace.Name, folderWorkspace.OwnerName)
            End If
        Catch
            'NOOP
        End Try

        'Ensures folder does not exist
        Dim localFolderInfo = New DirectoryInfo(localFolder)
        If localFolderInfo.Exists Then
            localFolderInfo.Delete(True)
        End If

    End Sub

    Public Function GetService(ByVal projectCollectionDisplayName As String, ByVal teamProjectName As String) As VersionControlServer

        Dim projectCollection = GetProjectCollection(projectCollectionDisplayName)

        Dim teamProject = GetTeamProject(projectCollection, teamProjectName)

        Dim versionControlService = projectCollection.GetService(Of VersionControlServer)()

        Return versionControlService

    End Function

    Public Function GetWorkspace(ByVal projectCollectionDisplayName As String,
                                 ByVal teamProjectName As String,
                                 ByVal subFolder As String) As Workspace

        Static alreadyCreated As Boolean = False

        If alreadyCreated Then
            Throw New ApplicationException("Sorry, only one workspace can be created by a TfsWorker.")
        End If

        Dim projectCollection = GetProjectCollection(projectCollectionDisplayName)

        Dim teamProject = GetTeamProject(projectCollection, teamProjectName)

        Dim versionControlService = projectCollection.GetService(Of VersionControlServer)()

        'Determine serverFolder
        subFolder = subFolder.Trim
        subFolder = subFolder.Replace("\", "/")
        If subFolder.StartsWith("/") Then subFolder = subFolder.Remove(0, 1)
        Dim serverFolder = String.Format("$/{0}", teamProject.Name)
        If subFolder IsNot String.Empty Then serverFolder = serverFolder + "/" + subFolder

        'Creates workspace
        Dim workspaceName = GetWorkspaceName()
        Console.WriteLine("Creating workspace '{0}'...", workspaceName)
        PrepareWorkspace(versionControlService, workspaceName, LocalWorkspaceDirectory)
        Dim workspace = versionControlService.CreateWorkspace(workspaceName)
        Dim workingFolder = New WorkingFolder(serverFolder, LocalWorkspaceDirectory)
        workspace.CreateMapping(workingFolder)
        Console.WriteLine("Project mapped to '{0}'.", LocalWorkspaceDirectory)

        'Check permissions
        If Not workspace.HasReadPermission Then
            Throw New SecurityException(
                String.Format("{0} does not have read permission for {1}.",
                    versionControlService.AuthorizedUser,
                    serverFolder))
        End If

        'Get all files!
        Console.WriteLine("Retrieving files from server...")
        Dim getOperationResult = workspace.Get
        Console.WriteLine("Workspace ready.")

        createdWorkspaces.Add(workspace)

        alreadyCreated = True

        Return workspace

    End Function

#Region "IDisposable Support"

    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)

        If Not Me.disposedValue Then

            If disposing Then

                ' dispose managed state (managed objects).
                createdWorkspaces.ForEach(Function(x As Workspace) x.Delete)

            End If

            ' free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' set large fields to null.

        End If

        Me.disposedValue = True

    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose

        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.

        Dispose(True)
        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class