//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace VersionControl
{
    /// <summary>
    /// This simple example illustrates how to create a workspace, check-in
    /// a new folder and file, check it out, change it and check it back in.
    /// Lastly, it deletes the folder from the repository and the workspace
    /// it created (if appropriate).
    /// </summary>
    class BasicExample
    {
        /// <summary>
        /// Entry point for the console application.
        /// </summary>
        /// <param name="args">Optional parameter is the Name or Uri of a 
        /// registered project collection to connect to.</param>
        static void Main(string[] args)
        {
            List<RegisteredProjectCollection> projectCollections = null;

            if (args.Count() > 0)
            {
                // try the specified argument as the name or Uri of a registered project collection
                projectCollections = new List<RegisteredProjectCollection>();
                projectCollections.Add(RegisteredInstances.GetProjectCollection(args[1]));
            }
            else
            {
                // get all registered project collections (previously connected to from Team Explorer)
                projectCollections = new List<RegisteredProjectCollection>(
                    (IEnumerable<RegisteredProjectCollection>)(RegisteredInstances.GetProjectCollections()));
            }

            // filter down to only those collections that are currently on-line
            var onlineCollections =
                from collection in projectCollections
                where collection.Offline == false
                select collection;

            // fail if there are no registered collections that are currently on-line
            if (onlineCollections.Count() < 1)
            {
                Console.Error.WriteLine("Error: There are no on-line registered project collections");
                Environment.Exit(1);
            }

            // find a project collection with at least one team project
            foreach (var projectCollection in onlineCollections)
            {
                using (TeamFoundationServer tfs = TeamFoundationServerFactory.GetServer(projectCollection))
                {
                    Workspace workspace = null;
                    Boolean createdWorkspace = false;
                    String newFolder = String.Empty;

                    try
                    {
                        var versionControl = (VersionControlServer)tfs.GetService(typeof(VersionControlServer));

                        var teamProjects = new List<TeamProject>(
                            (IEnumerable<TeamProject>)(versionControl.GetAllTeamProjects(false)));

                        // if there are no team projects in this collection, skip it
                        if (teamProjects.Count < 1) continue;

                        RegisterEventHandlers(versionControl);

                        // Create a workspace
                        String assemblyName = GetAssemblyName();
                        String workspaceName = String.Format("{0}-{1}", Environment.MachineName, assemblyName);

                        try
                        {
                            workspace = versionControl.GetWorkspace(workspaceName,
                                versionControl.AuthenticatedUser);
                        }
                        catch (WorkspaceNotFoundException)
                        {
                            workspace = versionControl.CreateWorkspace(workspaceName,
                                versionControl.AuthenticatedUser);

                            createdWorkspace = true;
                        }

                        var serverFolder = String.Format("$/{0}", teamProjects[0].Name);
                        var localFolder = Path.Combine(Path.GetTempPath(), assemblyName);
                        var workingFolder = new WorkingFolder(serverFolder, localFolder);

                        // Create a workspace mapping
                        workspace.CreateMapping(workingFolder);

                        if (!workspace.HasReadPermission)
                        {
                            throw new SecurityException(
                                String.Format("{0} does not have read permission for {1}",
                                    versionControl.AuthenticatedUser, serverFolder));
                        }

                        // Get the files from the repository
                        workspace.Get();

                        // Create a file
                        newFolder = Path.Combine(workspace.Folders[0].LocalItem, "For Test Purposes");
                        Directory.CreateDirectory(newFolder);
                        String newFilename = Path.Combine(newFolder, "Safe to Delete.txt");

                        AddNewFile(workspace, newFilename);
                        ModifyFile(workspace, newFilename);
                        BranchFile(workspace, newFilename);
                        DeleteFolder(workspace, newFolder);
                    }
                    finally
                    {
                        if ((workspace != null) && createdWorkspace)
                        {
                            workspace.Delete();
                        }

                        if (!String.IsNullOrEmpty(newFolder) && Directory.Exists(newFolder))
                        {
                            Directory.Delete(newFolder, true);
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Get a friendly name of the current assembly to use in forming the name
        /// of the version control workspace to be created.
        /// </summary>
        /// <returns>Short/friendly name of assembly (e.g. BasicExample)</returns>
        private static String GetAssemblyName()
        {
            String assemblyName = AppDomain.CurrentDomain.FriendlyName;
            int dotPosition = assemblyName.IndexOf('.');

            if (dotPosition > 0)
            {
                assemblyName = assemblyName.Substring(0, dotPosition);
            }

            return assemblyName;
        }

        /// <summary>
        /// Add event handlers for the key events we'll be monitoring to report
        /// progress to the user.
        /// </summary>
        /// <param name="versionControl">The version control server instance to 
        /// listen for events from.</param>
        private static void RegisterEventHandlers(VersionControlServer versionControl)
        {
            Debug.Assert(versionControl != null);

            // Listen for the Source Control events
            versionControl.BeforeCheckinPendingChange += BasicExample.OnBeforeCheckinPendingChange;
            versionControl.NonFatalError += BasicExample.OnNonFatalError;
            versionControl.Getting += BasicExample.OnGetting;
            versionControl.NewPendingChange += BasicExample.OnNewPendingChange;
        }

        /// <summary>
        /// 'Pends' the add of a new folder and file and then checks it into the 
        /// repository.
        /// </summary>
        /// <param name="workspace">Version control workspace to use when 
        /// adding the folder and file.</param>
        /// <param name="newFilename">Full path to the file to add (the path
        /// of the folder will be derived from the file's path.</param>
        /// <exception cref="SecurityException">If the user doesn't have
        /// check-in permission for the specified <paramref name="workspace"/>.</exception>
        /// <exception cref="IOException">If there's a problem creating the file.</exception>
        /// <exception cref="VersionControlException">If </exception>
        private static void AddNewFile(Workspace workspace, String newFilename)
        {
            Debug.Assert(workspace != null);
            Debug.Assert(!String.IsNullOrEmpty(newFilename));
            Debug.Assert(!File.Exists(newFilename));

            if (!workspace.HasCheckInPermission)
            {
                throw new SecurityException(
                    String.Format("{0} does not have check-in permission for workspace: {1}",
                        workspace.VersionControlServer.AuthenticatedUser,
                        workspace.DisplayName));
            }

            try
            {
                // create the new file
                using (var streamWriter = new StreamWriter(newFilename))
                {
                    streamWriter.WriteLine("Revision 1");
                }

                // Now pend the add of our new folder and file
                workspace.PendAdd(Path.GetDirectoryName(newFilename), true);

                // Show our pending changes
                var pendingAdds = new List<PendingChange>((IEnumerable<PendingChange>)
                    workspace.GetPendingChanges());

                pendingAdds.ForEach(delegate(PendingChange add)
                {
                    Console.WriteLine("\t{1}: {0}",
                        add.LocalItem, PendingChange.GetLocalizedStringForChangeType(add.ChangeType));
                });

                // Checkin the items we added
                int changesetForAdd = workspace.CheckIn(pendingAdds.ToArray(), "Initial check-in");
                Console.WriteLine("Checked in changeset {0}", changesetForAdd);
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("Error writing {1}: {0}", ex.Message, newFilename);
                throw;
            }
            catch (VersionControlException ex)
            {
                Console.Error.WriteLine("Error adding file: {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 'Pends' a change to a file that was previously checked-in and then checks
        /// that change into the repository.
        /// </summary>
        /// <param name="workspace">Version control workspace to use when 
        /// changing the folder and file.</param>
        /// <param name="newFilename">Full path to the file to change</param>
        /// <exception cref="SecurityException">If the user doesn't have
        /// check-in permission for the specified <paramref name="workspace"/>.</exception>
        /// <exception cref="IOException">If there's a problem creating the file.</exception>
        /// <exception cref="VersionControlException">If </exception>
        private static void ModifyFile(Workspace workspace, String newFilename)
        {
            Debug.Assert(workspace != null);
            Debug.Assert(!String.IsNullOrEmpty(newFilename));

            try
            {
                // Checkout and modify the file
                workspace.PendEdit(newFilename);

                using (var streamWriter = new StreamWriter(newFilename))
                {
                    streamWriter.WriteLine("Revision 2");
                }

                // Get the pending change and check in the new revision.
                var pendingChanges = workspace.GetPendingChanges();
                int changesetForChange = workspace.CheckIn(pendingChanges, "Modified file contents");
                Console.WriteLine("Checked in changeset {0}", changesetForChange);
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("Error writing {1}: {0}", ex.Message, newFilename);
                throw;
            }
            catch (VersionControlException ex)
            {
                Console.Error.WriteLine("Error modifying file: {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 'Pends' a branch of a file that was previously checked-in and then checks
        /// that branch into the repository.
        /// </summary>
        /// <param name="workspace">Version control workspace to use when 
        /// branching the folder and file.</param>
        /// <param name="newFilename">Full path to the file to branch</param>
        /// <exception cref="VersionControlException">If there's a problem performing
        /// the branch operation.</exception>
        private static void BranchFile(Workspace workspace, String newFilename)
        {
            Debug.Assert(workspace != null);
            Debug.Assert(!String.IsNullOrEmpty(newFilename));

            String branchedFilename = Path.Combine(Path.GetDirectoryName(newFilename),
                Path.GetFileNameWithoutExtension(newFilename)) + "-branch" +
                Path.GetExtension(newFilename);

            workspace.PendBranch(newFilename, branchedFilename, VersionSpec.Latest,
                LockLevel.Checkin, true);

            var pendingChanges = workspace.GetPendingChanges();
            int changesetForBranch = workspace.CheckIn(pendingChanges, "Branched file");
            Console.WriteLine("Branched {0} to {1} in changeset {2}", newFilename,
                branchedFilename, changesetForBranch);
        }

        /// <summary>
        /// 'Pends' a delete of a folder and its contents that was previously 
        /// checked-in and then commits that deletion to the repository.
        /// </summary>
        /// <param name="workspace">Version control workspace to use when 
        /// deleting the folder and its contents.</param>
        /// <param name="newFilename">Full path to the folder to delete</param>
        /// <exception cref="SecurityException">If the user doesn't have
        /// check-in permission for the specified <paramref name="workspace"/>.</exception>
        /// <exception cref="IOException">If there's a problem creating the file.</exception>
        /// <exception cref="VersionControlException">If </exception>
        private static void DeleteFolder(Workspace workspace, String newFolder)
        {
            Debug.Assert(workspace != null);
            Debug.Assert(!String.IsNullOrEmpty(newFolder));

            try
            {
                // Delete the items
                workspace.PendDelete(workspace.GetServerItemForLocalItem(newFolder), RecursionType.Full);
                var pendingDeletes = workspace.GetPendingChanges();

                if (pendingDeletes.Length > 0)
                {
                    workspace.CheckIn(pendingDeletes, "Clean up!");
                }
            }
            catch (VersionControlException ex)
            {
                Console.Error.WriteLine("Error deleting file: {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Handles non-fatal errors by writing a message describing the exception or
        /// failure to stderror.
        /// </summary>
        /// <param name="sender">Source of the error</param>
        /// <param name="e">Specifics of the exception or failure</param>
        internal static void OnNonFatalError(Object sender, ExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                Console.Error.WriteLine("Non-fatal exception: {0}", e.Exception.Message);
            }
            else
            {
                Console.Error.WriteLine("Non-fatal failure: {0}", e.Failure.Message);
            }
        }

        /// <summary>
        /// Called as TFS retrieves files from the version control repository. The
        /// event is handled by writing information about the "get" operation to the
        /// console.
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Specifics of the get operation</param>
        internal static void OnGetting(Object sender, GettingEventArgs e)
        {
            Console.WriteLine("Getting: {0}, status: {1}", e.TargetLocalItem, e.Status);
        }

        /// <summary>
        /// Process new pending changes by writing details about them to the console.
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Specifics of the pending changes</param>
        internal static void OnNewPendingChange(Object sender, PendingChangeEventArgs e)
        {
            Console.WriteLine("Pending {0} on {1}",
                PendingChange.GetLocalizedStringForChangeType(e.PendingChange.ChangeType),
                e.PendingChange.LocalItem);
        }

        /// <summary>
        /// Called as TFS prepares to commit a changeset to the version control repository. 
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Specifics of the pending changes</param>
        internal static void OnBeforeCheckinPendingChange(Object sender, ProcessingChangeEventArgs e)
        {
            Console.WriteLine("Checking in {0}", e.PendingChange.LocalItem);
        }
    }
}