Module Common

    Public ReadOnly Property TempPath As String
        Get
            If MainWindow.customTemporaryFolderCheckBox.Checked Then
                Return MainWindow.customTemporaryFolderTextBox.Text
            Else
                Return IO.Path.GetTempPath
            End If
        End Get
    End Property

    Public Function GetFileSystemInfo(ByVal path As String) As IO.FileSystemInfo

        Dim directoryInfo = New IO.DirectoryInfo(path)
        If directoryInfo.Exists Then
            Return directoryInfo
        End If

        Return New IO.FileInfo(path)

    End Function

    Public Function GetVersion(ByVal verbose As Boolean) As String

        With My.Application.Info.Version

            If verbose Then
                
                Try

                    'returns deployment version

                    With My.Application.Info.Version

                        Return String.Format(
                                    "{0}.{1} rev{2}",
                                    .Major,
                                    .Minor,
                                    My.Application.Deployment.CurrentVersion.Revision)

                    End With

                Catch ex As Deployment.Application.InvalidDeploymentException

                    With My.Application.Info.Version

                        Return String.Format(
                                    "{0}.{1} (not deployed)",
                                    .Major,
                                    .Minor)

                    End With

                End Try

            Else

                Return String.Format(
                            "{0}.{1}",
                            .Major,
                            .Minor)

            End If

        End With

    End Function

End Module
