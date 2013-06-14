Imports System.Runtime.CompilerServices
Imports System.Globalization

Module ExtensionMethods

    <Extension()>
    Public Function ToDictionary(ByVal list As List(Of MainWindow.UserMap)) As Dictionary(Of String, String)

        Dim ret = New Dictionary(Of String, String) 'TODO: Maybe this should return a case insensitive key comparer dictionary

        For Each item In list

            ret.Add(item.SvnUserName, item.TfsUserName)

        Next

        Return ret

    End Function

    <Extension()> _
    Public Function ToDelimitedString(Of T)(ByVal source As IEnumerable(Of T)) As String
        Return source.ToDelimitedString(Function(x) x.ToString(), CultureInfo.CurrentCulture.TextInfo.ListSeparator)
    End Function

    <Extension()> _
    Public Function ToDelimitedString(Of T)(ByVal source As IEnumerable(Of T), ByVal converter As Func(Of T, String)) As String
        Return source.ToDelimitedString(converter, CultureInfo.CurrentCulture.TextInfo.ListSeparator)
    End Function

    <Extension()> _
    Public Function ToDelimitedString(Of T)(ByVal source As IEnumerable(Of T), ByVal separator As String) As String
        Return source.ToDelimitedString(Function(x) x.ToString(), separator)
    End Function

    <Extension()> _
    Public Function ToDelimitedString(Of T)(ByVal source As IEnumerable(Of T), ByVal converter As Func(Of T, String), ByVal separator As String) As String
        Return String.Join(separator, source.[Select](converter).ToArray())
    End Function

    <Extension()>
    Public Sub Put(Of T)(ByVal list As IList(Of T), ByVal Item As T)
        If Not list.Contains(Item) Then
            list.Add(Item)
        End If
    End Sub

    <Extension()>
    Public Function Create(ByVal info As IO.FileSystemInfo) As IO.FileStream

        If TypeOf info Is IO.FileInfo Then

            Dim fileInfo = DirectCast(info, IO.FileInfo)

            Return fileInfo.Create()

        ElseIf TypeOf info Is IO.DirectoryInfo Then

            Dim directoryInfo = DirectCast(info, IO.DirectoryInfo)

            directoryInfo.Create()

            Return Nothing

        Else

            Throw New NotSupportedException("Method is implemented only for directories and files.")

        End If

    End Function

    <Extension()>
    Public Sub Delete(ByVal info As IO.FileSystemInfo, ByVal Recursive As Boolean)

        If TypeOf info Is IO.FileInfo Then

            Dim fileInfo = DirectCast(info, IO.FileInfo)

            fileInfo.Delete()

        ElseIf TypeOf info Is IO.DirectoryInfo Then

            Dim directoryInfo = DirectCast(info, IO.DirectoryInfo)

            directoryInfo.Delete(Recursive)

        Else

            Throw New NotSupportedException("Method is implemented only for directories and files.")

        End If

    End Sub

    <Extension()>
    Public Sub CopyTo(ByVal info As IO.FileSystemInfo, ByVal Destination As String, ByVal Overwrite As Boolean)

        If TypeOf info Is IO.FileInfo Then

            Dim fileInfo = DirectCast(info, IO.FileInfo)

            Dim destinationFileInfo = New IO.FileInfo(Destination)
            If destinationFileInfo.Exists Then
                destinationFileInfo.Attributes = 0
            End If

            fileInfo.CopyTo(Destination, Overwrite)

        ElseIf TypeOf info Is IO.DirectoryInfo Then

            Dim directoryInfo = DirectCast(info, IO.DirectoryInfo)

            directoryInfo.CopyTo(Destination, Overwrite)

        Else

            Throw New NotSupportedException("Method is implemented only for directories and files.")

        End If

    End Sub

    <Extension()>
    Public Sub CopyTo(ByVal Source As IO.DirectoryInfo, ByVal Destination As String, ByVal Overwrite As Boolean)

        'copied from http://episteme.arstechnica.com/eve/forums/a/tpc/f/6330927813/m/2340985975

        Dim DestDir = New IO.DirectoryInfo(Destination)

        ' the source directory must exist, otherwise throw an exception
        If Source.Exists Then
            ' if destination SubDir's parent SubDir does not exist throw an exception
            If Not DestDir.Parent.Exists Then
                Throw New IO.DirectoryNotFoundException _
                    ("Destination directory does not exist: " + DestDir.Parent.FullName)
            End If

            If Not DestDir.Exists Then
                DestDir.Create()
            End If

            ' copy all the files of the current directory
            Dim ChildFile As IO.FileInfo
            For Each ChildFile In Source.GetFiles()
                If Overwrite Then
                    ChildFile.CopyTo(IO.Path.Combine(DestDir.FullName, ChildFile.Name), True)
                Else
                    ' if Overwrite = false, copy the file only if it does not exist
                    ' this is done to avoid an IOException if a file already exists
                    ' this way the other files can be copied anyway...
                    If Not IO.File.Exists(IO.Path.Combine(DestDir.FullName, ChildFile.Name)) Then
                        ChildFile.CopyTo(IO.Path.Combine(DestDir.FullName, ChildFile.Name), False)
                    End If
                End If
            Next

            ' copy all the sub-directories by recursively calling this same routine
            For Each SubDir As IO.DirectoryInfo In Source.GetDirectories()

                'except .svn directories
                If SubDir.Name.ToLower = ".svn".ToLower Then
                    'skip 
                Else
                    CopyTo(New IO.DirectoryInfo(SubDir.FullName), IO.Path.Combine(DestDir.FullName, _
                        SubDir.Name), Overwrite)
                End If
            Next
        Else
            Throw New IO.DirectoryNotFoundException("Source directory does not exist: " + Source.FullName)
        End If
    End Sub

End Module
