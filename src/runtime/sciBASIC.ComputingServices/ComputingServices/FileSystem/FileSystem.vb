#Region "Microsoft.VisualBasic::64e8d35cbdeeb0e3c5c1baa15d1b76d9, ..\sciBASIC.ComputingServices\ComputingServices\FileSystem\FileSystem.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Serialization.JSON
Imports sciBASIC.ComputingServices.FileSystem.Protocols

Namespace FileSystem

    ''' <summary>
    ''' Provides properties and methods for working with drives, files, and directories on remote server.
    ''' (分布式文件系统)
    ''' </summary>
    Public Class FileSystem

        ''' <summary>
        ''' 远端服务器的开放的句柄端口
        ''' </summary>
        Public ReadOnly Property Portal As IPEndPoint

        Sub New(portal As IPEndPoint)
            _Portal = portal
        End Sub

        Public Overrides Function ToString() As String
            Return $"{CurrentDirectory}@{_Portal.ToString}"
        End Function

        ' Exceptions:
        '   T:System.IO.DirectoryNotFoundException:
        '     The path is not valid.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user lacks necessary permissions.
        ''' <summary>
        ''' Gets or sets the current directory.
        ''' </summary>
        ''' <returns>The current directory for file I/O operations.</returns>
        Public Property CurrentDirectory As String
            Get
                Dim req As RequestStream = New RequestStream(ProtocolEntry, FileSystemAPI.CurrentDirectory)
                Dim invoke As New AsynInvoke(_Portal)
                Dim rep As RequestStream = invoke.SendMessage(req)
                Return req.GetUTF8String
            End Get
            Set(value As String)
                Dim req As RequestStream = New RequestStream(ProtocolEntry, FileSystemAPI.CurrentDirectory, value)
                Dim invoke As New AsynInvoke(_Portal)
                Call invoke.SendMessage(req)
            End Set
        End Property

        ''' <summary>
        ''' Returns a read-only collection of all available drive names.
        ''' </summary>
        ''' <returns>A read-only collection of all available drives as System.IO.DriveInfo objects.</returns>
        Public ReadOnly Property Drives As ReadOnlyCollection(Of System.IO.DriveInfo)
            Get
                Dim req As RequestStream = New RequestStream(ProtocolEntry, FileSystemAPI.Drives)
                Dim invoke As New AsynInvoke(_Portal)
                Dim rep As RequestStream = invoke.SendMessage(req)
                Dim array As String() = req.GetUTF8String.LoadObject(Of String())
                Dim lst As DriveInfo() = array.ToArray(Function(s) s.LoadObject(Of DriveInfo))
                Return New ReadOnlyCollection(Of System.IO.DriveInfo)(CType(lst, IList(Of System.IO.DriveInfo)))
            End Get
        End Property

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The new name specified for the directory contains a colon (:) or slash (\ or
        '     /).
        '
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     destinationDirectoryName or sourceDirectoryName is Nothing or an empty string.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The source directory does not exist.
        '
        '   T:System.IO.IOException:
        '     The source directory is a root directory
        '
        '   T:System.IO.IOException:
        '     The combined path points to an existing file.
        '
        '   T:System.IO.IOException:
        '     The source path and target path are the same.
        '
        '   T:System.InvalidOperationException:
        '     The operation is cyclic.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A folder name in the path contains a colon (:) or is in an invalid format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     A destination file exists but cannot be accessed.
        ''' <summary>
        ''' Copies the contents of a directory to another directory.
        ''' </summary>
        ''' <param name="sourceDirectoryName">The directory to be copied.</param>
        ''' <param name="destinationDirectoryName">The location to which the directory contents should be copied.</param>
        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String)

        End Sub

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The new name specified for the directory contains a colon (:) or slash (\ or
        '     /).
        '
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     destinationDirectoryName or sourceDirectoryName is Nothing or an empty string.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The source directory does not exist.
        '
        '   T:System.IO.IOException:
        '     The source directory is a root directory
        '
        '   T:System.IO.IOException:
        '     The combined path points to an existing file.
        '
        '   T:System.IO.IOException:
        '     The source path and target path are the same.
        '
        '   T:System.InvalidOperationException:
        '     The operation is cyclic.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A folder name in the path contains a colon (:) or is in an invalid format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     A destination file exists but cannot be accessed.
        ''' <summary>
        ''' Copies the contents of a directory to another directory.
        ''' </summary>
        ''' <param name="sourceDirectoryName">The directory to be copied.</param>
        ''' <param name="destinationDirectoryName">The location to which the directory contents should be copied.</param>
        ''' <param name="overwrite">True to overwrite existing files; otherwise False. Default is False.</param>
        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String, overwrite As Boolean)

        End Sub

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentException:
        '     The system could not retrieve the absolute path.
        '
        '   T:System.ArgumentException:
        '     destinationFileName contains path information.
        '
        '   T:System.ArgumentNullException:
        '     destinationFileName or sourceFileName is Nothing or an empty string.
        '
        '   T:System.IO.FileNotFoundException:
        '     The source file is not valid or does not exist.
        '
        '   T:System.IO.IOException:
        '     The combined path points to an existing directory.
        '
        '   T:System.IO.IOException:
        '     The user does not have sufficient permissions to access the file.
        '
        '   T:System.IO.IOException:
        '     A file in the target directory with the same name is in use.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user does not have required permission.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.

        ''' <summary>
        ''' Copies a file to a new location.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be copied.</param>
        ''' <param name="destinationFileName">The location to which the file should be copied.</param>
        Public Sub CopyFile(sourceFileName As String, destinationFileName As String)
            Call CopyFile(sourceFileName, destinationFileName)
        End Sub

        '
        ' Summary:
        '     Copies a file to a new location.
        '
        ' Parameters:
        '   sourceFileName:
        '     The file to be copied.
        '
        '   destinationFileName:
        '     The location to which the file should be copied.
        '
        '   overwrite:
        '     True if existing files should be overwritten; otherwise False. Default is False.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentException:
        '     The system could not retrieve the absolute path.
        '
        '   T:System.ArgumentException:
        '     destinationFileName contains path information.
        '
        '   T:System.ArgumentNullException:
        '     destinationFileName or sourceFileName is Nothing or an empty string.
        '
        '   T:System.IO.FileNotFoundException:
        '     The source file is not valid or does not exist.
        '
        '   T:System.IO.IOException:
        '     The combined path points to an existing directory.
        '
        '   T:System.IO.IOException:
        '     The user does not have sufficient permissions to access the file.
        '
        '   T:System.IO.IOException:
        '     A file in the target directory with the same name is in use.
        '
        '   T:System.IO.IOException:
        '     The destination file exists and overwrite is set to False.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user does not have required permission.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sourceFileName"></param>
        ''' <param name="destinationFileName"></param>
        ''' <param name="overwrite"></param>
        Public Sub CopyFile(sourceFileName As String, destinationFileName As String, overwrite As Boolean)
            Dim source As FileURI = New FileURI(sourceFileName)
            Dim destination As FileURI = New FileURI(destinationFileName)

            If source.IsLocal Then
                If destination.IsLocal Then
                    Call FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, overwrite)
                Else    ' 进行文件上传
                    Call NetTransfer.Upload(source.File, destination.File, destination.EntryPoint)
                End If
            Else
                If destination.IsLocal Then  ' 文件下载，然后执行删除
                    Call NetTransfer.Download(source.File, destination.File, source.EntryPoint)
                Else    ' 远程文件系统之上的文件复制

                End If
            End If
        End Sub

        '
        ' Summary:
        '     Creates a directory.
        '
        ' Parameters:
        '   directory:
        '     Name and location of the directory.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The directory name is malformed. For example, it contains illegal characters
        '     or is only white space.
        '
        '   T:System.ArgumentNullException:
        '     directory is Nothing or an empty string.
        '
        '   T:System.IO.PathTooLongException:
        '     The directory name is too long.
        '
        '   T:System.NotSupportedException:
        '     The directory name is only a colon (:).
        '
        '   T:System.IO.IOException:
        '     The parent directory of the directory to be created is read-only
        '
        '   T:System.UnauthorizedAccessException:
        '     The user does not have permission to create the directory.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory"></param>
        Public Sub CreateDirectory(directory As String)
            Dim req As New RequestStream(ProtocolEntry, FileSystemAPI.CreateDirectory, directory)
            Dim invoke As New AsynInvoke(Portal)
            Dim rep As RequestStream = invoke.SendMessage(req)
        End Sub
        '
        ' Summary:
        '     Deletes a directory.
        '
        ' Parameters:
        '   directory:
        '     Directory to be deleted.
        '
        '   onDirectoryNotEmpty:
        '     Specifies what should be done when a directory that is to be deleted contains
        '     files or directories. Default is DeleteDirectoryOption.DeleteAllContents.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is a zero-length string, is malformed, contains only white space, or
        '     contains invalid characters (including wildcard characters). The path is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     directory is Nothing or an empty string.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The directory does not exist or is a file.
        '
        '   T:System.IO.IOException:
        '     The directory is not empty, and onDirectoryNotEmpty is set to ThrowIfDirectoryNonEmpty.
        '
        '   T:System.IO.IOException:
        '     The user does not have permission to delete the directory or subdirectory.
        '
        '   T:System.IO.IOException:
        '     A file in the directory or subdirectory is in use.
        '
        '   T:System.NotSupportedException:
        '     The directory name contains a colon (:).
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.Security.SecurityException:
        '     The user does not have required permissions.
        '
        '   T:System.OperationCanceledException:
        '     The user cancels the operation or the directory cannot be deleted.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <param name="onDirectoryNotEmpty"></param>
        Public Sub DeleteDirectory(directory As String, onDirectoryNotEmpty As DeleteDirectoryOption)
            Dim op As New DeleteArgs With {
                .obj = directory,
                .option = onDirectoryNotEmpty
            }
            Dim req As RequestStream = RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.DeleteDirectory, op)
            Dim invoke As New AsynInvoke(Portal)
            Dim rep As RequestStream = invoke.SendMessage(req)
        End Sub

        '
        ' Summary:
        '     Deletes a file.
        '
        ' Parameters:
        '   file:
        '     Name and path of the file to be deleted.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; it has a trailing
        '     slash where a file must be specified; or it is a device path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing or an empty string.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.IO.IOException:
        '     The file is in use.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file does not exist.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user does not have permission to delete the file or the file is read-only.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file"></param>
        Public Sub DeleteFile(file As String)
            Dim req As RequestStream = New RequestStream(ProtocolEntry, FileSystemAPI.DeleteFile, file)
            Dim invoke As New AsynInvoke(Portal)
            Dim rep As RequestStream = invoke.SendMessage(req)
        End Sub

        '
        ' Summary:
        '     Moves a directory from one location to another.
        '
        ' Parameters:
        '   sourceDirectoryName:
        '     Path of the directory to be moved.
        '
        '   destinationDirectoryName:
        '     Path of the directory to which the source directory is being moved.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     sourceDirectoryName or destinationDirectoryName is Nothing or an empty string.
        '
        '   T:System.ArgumentNullException:
        '     sourceDirectoryName or destinationDirectoryName is Nothing or an empty string.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The directory does not exist.
        '
        '   T:System.IO.IOException:
        '     The source is a root directory or The source path and the target path are the
        '     same.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.InvalidOperationException:
        '     The operation is cyclic.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user does not have required permission.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sourceDirectoryName"></param>
        ''' <param name="destinationDirectoryName"></param>
        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String)

        End Sub
        '
        ' Summary:
        '     Moves a directory from one location to another.
        '
        ' Parameters:
        '   sourceDirectoryName:
        '     Path of the directory to be moved.
        '
        '   destinationDirectoryName:
        '     Path of the directory to which the source directory is being moved.
        '
        '   overwrite:
        '     True if existing directories should be overwritten; otherwise False. Default
        '     is False.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     sourceDirectoryName or destinationDirectoryName is Nothing or an empty string.
        '
        '   T:System.ArgumentNullException:
        '     sourceDirectoryName or destinationDirectoryName is Nothing or an empty string.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The directory does not exist.
        '
        '   T:System.IO.IOException:
        '     The source is a root directory or The source path and the target path are the
        '     same.
        '
        '   T:System.IO.IOException:
        '     The target directory already exists and overwrite is set to False.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.InvalidOperationException:
        '     The operation is cyclic.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user does not have required permission.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sourceDirectoryName"></param>
        ''' <param name="destinationDirectoryName"></param>
        ''' <param name="overwrite"></param>
        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String, overwrite As Boolean)

        End Sub

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\); it ends with a trailing slash.
        '
        '   T:System.ArgumentNullException:
        '     destinationFileName is Nothing or an empty string.
        '
        '   T:System.IO.FileNotFoundException:
        '     The source file is not valid or does not exist.
        '
        '   T:System.IO.IOException:
        '     The file is in use by another process, or an I/O error occurs.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        ''' <summary>
        ''' Moves a file to a new location.
        ''' </summary>
        ''' <param name="sourceFileName">Path of the file to be moved.</param>
        ''' <param name="destinationFileName">Path of the directory into which the file should be moved.</param>
        Public Sub MoveFile(sourceFileName As String, destinationFileName As String)
            Call MoveFile(sourceFileName, destinationFileName, False)
        End Sub

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\); it ends with a trailing slash.
        '
        '   T:System.ArgumentNullException:
        '     destinationFileName is Nothing or an empty string.
        '
        '   T:System.IO.FileNotFoundException:
        '     The source file is not valid or does not exist.
        '
        '   T:System.IO.IOException:
        '     The destination file exists and overwrite is set to False.
        '
        '   T:System.IO.IOException:
        '     The file is in use by another process, or an I/O error occurs.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        ''' <summary>
        ''' Moves a file to a new location.
        ''' </summary>
        ''' <param name="sourceFileName">Path of the file to be moved.</param>
        ''' <param name="destinationFileName">Path of the directory into which the file should be moved.</param>
        ''' <param name="overwrite">True to overwrite existing files; otherwise False. Default is False.</param>
        Public Sub MoveFile(sourceFileName As String, destinationFileName As String, overwrite As Boolean)
            Dim source As FileURI = New FileURI(sourceFileName)
            Dim destination As FileURI = New FileURI(destinationFileName)

            If source.IsLocal Then
                If destination.IsLocal Then
                    Call FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, overwrite)
                Else    ' 进行文件上传
                    Call NetTransfer.Upload(source.File, destination.File, destination.EntryPoint)
                End If
            Else
                If destination.IsLocal Then  ' 文件下载，然后执行删除
                    Call NetTransfer.Download(source.File, destination.File, source.EntryPoint)
                    Call DeleteFile(source.File)
                Else    ' 远程文件系统之上的文件移动


                End If
            End If
        End Sub

        '
        ' Summary:
        '     Renames a directory.
        '
        ' Parameters:
        '   directory:
        '     Path and name of directory to be renamed.
        '
        '   newName:
        '     New name for directory.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentException:
        '     newName contains path information.
        '
        '   T:System.ArgumentNullException:
        '     directory is Nothing.-or-newName is Nothing or an empty string.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The directory does not exist.
        '
        '   T:System.IO.IOException:
        '     There is an existing file or directory with the name specified in newName.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds 248 characters.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user does not have required permission.

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <param name="newName"></param>
        Public Sub RenameDirectory(directory As String, newName As String)
            Dim rename As New RenameArgs With {
                .old = directory,
                .[New] = newName
            }
            Dim req As RequestStream = RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.RenameDirectory, rename)
            Dim invoke As New AsynInvoke(Portal)
            Dim rep As RequestStream = invoke.SendMessage(req)
        End Sub
        '
        ' Summary:
        '     Renames a file.
        '
        ' Parameters:
        '   file:
        '     File to be renamed.
        '
        '   newName:
        '     New name of file.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentException:
        '     newName contains path information or ends with a backslash (\).
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing.-or-newName is Nothing or an empty string.
        '
        '   T:System.IO.FileNotFoundException:
        '     The directory does not exist.
        '
        '   T:System.IO.IOException:
        '     There is an existing file or directory with the name specified in newName.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user does not have required permission.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file"></param>
        ''' <param name="newName"></param>
        Public Sub RenameFile(file As String, newName As String)
            Dim rename As New RenameArgs With {
             .old = file,
             .[New] = newName
         }
            Dim req As RequestStream = RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.RenameFile, rename)
            Dim invoke As New AsynInvoke(Portal)
            Dim rep As RequestStream = invoke.SendMessage(req)
        End Sub

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\); it ends with a trailing slash.
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file does not exist.
        '
        '   T:System.IO.IOException:
        '     The file is in use by another process, or an I/O error occurs.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.OutOfMemoryException:
        '     There is not enough memory to write the string to buffer.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        ''' <summary>
        ''' Writes data to a binary file.
        ''' </summary>
        ''' <param name="file">Path and name of the file to be written to.</param>
        ''' <param name="data">Data to be written to the file.</param>
        ''' <param name="append">True to append to the file contents; False to overwrite the file contents. Default
        ''' is False.</param>
        Public Sub WriteAllBytes(file As String, data() As Byte, append As Boolean)
            Dim mode As FileMode = If(append, FileMode.Append, FileMode.OpenOrCreate)
            Dim remoteFile As New IO.RemoteFileStream(file, mode, Me)
            Call remoteFile.Write(data, Scan0, data.Length)
        End Sub

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\); it ends with a trailing slash.
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file does not exist.
        '
        '   T:System.IO.IOException:
        '     The file is in use by another process, or an I/O error occurs.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.OutOfMemoryException:
        '     There is not enough memory to write the string to buffer.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        ''' <summary>
        ''' Writes text to a file.
        ''' </summary>
        ''' <param name="file">File to be written to.</param>
        ''' <param name="text">Text to be written to file.</param>
        ''' <param name="append">True to append to the contents of the file; False to overwrite the contents of
        ''' the file.</param>
        Public Sub WriteAllText(file As String, text As String, append As Boolean)
            Call WriteAllText(file, text, append, Encoding.Default)
        End Sub

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\); it ends with a trailing slash.
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file does not exist.
        '
        '   T:System.IO.IOException:
        '     The file is in use by another process, or an I/O error occurs.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.OutOfMemoryException:
        '     There is not enough memory to write the string to buffer.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        ''' <summary>
        ''' Writes text to a file.
        ''' </summary>
        ''' <param name="file">File to be written to.</param>
        ''' <param name="text">Text to be written to file.</param>
        ''' <param name="append">True to append to the contents of the file; False to overwrite the contents of
        ''' the file.</param>
        ''' <param name="encoding">What encoding to use when writing to file.</param>
        Public Sub WriteAllText(file As String, text As String, append As Boolean, encoding As Encoding)
            Dim buf As Byte() = encoding.GetBytes(text)
            Call WriteAllBytes(file, buf, append)
        End Sub

        '
        ' Summary:
        '     Returns True if the specified directory exists.
        '
        ' Parameters:
        '   directory:
        '     Path of the directory.
        '
        ' Returns:
        '     True if the directory exists; otherwise False.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <returns></returns>
        Public Function DirectoryExists(directory As String) As Boolean
            Dim req As RequestStream = New RequestStream(ProtocolEntry, FileSystemAPI.DirectoryExists, directory)
            Dim invoke As New AsynInvoke(Portal)
            Dim rep As RequestStream = invoke.SendMessage(req)
            If rep.ChunkBuffer.First = 1 Then
                Return True
            Else
                Return False
            End If
        End Function

        '
        ' Summary:
        '     Returns True if the specified file exists.
        '
        ' Parameters:
        '   file:
        '     Name and path of the file.
        '
        ' Returns:
        '     Returns True if the file exists; otherwise this method returns False.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The name of the file ends with a backslash (\).
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function FileExists(file As String) As Boolean
            Dim req As RequestStream = New RequestStream(ProtocolEntry, FileSystemAPI.FileExists, file)
            Dim invoke As New AsynInvoke(Portal)
            Dim rep As RequestStream = invoke.SendMessage(req)
            If rep.ChunkBuffer.First = 1 Then
                Return True
            Else
                Return False
            End If
        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     directory is Nothing or an empty string.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The specified directory does not exist.
        '
        '   T:System.IO.IOException:
        '     The specified directory points to an existing file.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     The specified directory path contains a colon (:) or is in an invalid format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user lacks necessary permissions.
        ''' <summary>
        ''' Returns a read-only collection of strings representing the names of files containing
        ''' the specified text.
        ''' </summary>
        ''' <param name="directory">The directory to be searched.</param>
        ''' <param name="containsText">The search text.</param>
        ''' <param name="ignoreCase">True if the search should be case-sensitive; otherwise False. Default is True.</param>
        ''' <param name="searchType">Whether to include subfolders. Default is SearchOption.SearchTopLevelOnly.</param>
        ''' <returns>Read-only collection of the names of files containing the specified text..</returns>
        Public Function FindInFiles(directory As String, containsText As String, ignoreCase As Boolean, searchType As FileIO.SearchOption) As ReadOnlyCollection(Of String)
            Return FindInFiles(directory, containsText, ignoreCase, searchType, "*.*")
        End Function
        '
        ' Summary:
        '     Returns a read-only collection of strings representing the names of files containing
        '     the specified text.
        '
        ' Parameters:
        '   directory:
        '     The directory to be searched.
        '
        '   containsText:
        '     The search text.
        '
        '   ignoreCase:
        '     True if the search should be case-sensitive; otherwise False. Default is True.
        '
        '   searchType:
        '     Whether to include subfolders. Default is SearchOption.SearchTopLevelOnly.
        '
        '   fileWildcards:
        '     Pattern to be matched.
        '
        ' Returns:
        '     Read-only collection of the names of files containing the specified text..
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     directory is Nothing or an empty string.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The specified directory does not exist.
        '
        '   T:System.IO.IOException:
        '     The specified directory points to an existing file.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     The specified directory path contains a colon (:) or is in an invalid format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user lacks necessary permissions.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <param name="containsText"></param>
        ''' <param name="ignoreCase"></param>
        ''' <param name="searchType"></param>
        ''' <param name="fileWildcards"></param>
        ''' <returns></returns>
        Public Function FindInFiles(directory As String, containsText As String, ignoreCase As Boolean, searchType As FileIO.SearchOption, ParamArray fileWildcards() As String) As ReadOnlyCollection(Of String)

        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     directory is Nothing or an empty string.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The specified directory does not exist.
        '
        '   T:System.IO.IOException:
        '     The specified directory points to an existing file.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user lacks necessary permissions.
        ''' <summary>
        ''' Returns a collection of strings representing the path names of subdirectories
        ''' within a directory.
        ''' </summary>
        ''' <param name="directory">Name and path of directory.</param>
        ''' <returns>Read-only collection of the path names of subdirectories within the specified
        ''' directory.</returns>
        Public Function GetDirectories(directory As String) As ReadOnlyCollection(Of String)
            Return GetDirectories(directory, FileIO.SearchOption.SearchTopLevelOnly)
        End Function
        '
        ' Summary:
        '     Returns a collection of strings representing the path names of subdirectories
        '     within a directory.
        '
        ' Parameters:
        '   directory:
        '     Name and path of directory.
        '
        '   searchType:
        '     Whether to include subfolders. Default is SearchOption.SearchTopLevelOnly.
        '
        '   wildcards:
        '     Pattern to match names.
        '
        ' Returns:
        '     Read-only collection of the path names of subdirectories within the specified
        '     directory.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     directory is Nothing or an empty string.
        '
        '   T:System.ArgumentNullException:
        '     One or more of the specified wildcard characters is Nothing, an empty string,
        '     or contains only spaces.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The specified directory does not exist.
        '
        '   T:System.IO.IOException:
        '     The specified directory points to an existing file.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user lacks necessary permissions.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <param name="searchType"></param>
        ''' <param name="wildcards"></param>
        ''' <returns></returns>
        Public Function GetDirectories(directory As String, searchType As FileIO.SearchOption, ParamArray wildcards() As String) As ReadOnlyCollection(Of String)

        End Function
        '
        ' Summary:
        '     Returns a System.IO.DirectoryInfo object for the specified path.
        '
        ' Parameters:
        '   directory:
        '     String. Path of directory.
        '
        ' Returns:
        '     System.IO.DirectoryInfo object for the specified path.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     directory is Nothing or an empty string.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     The directory path contains a colon (:) or is in an invalid format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <returns></returns>
        Public Function GetDirectoryInfo(directory As String) As System.IO.DirectoryInfo

        End Function
        '
        ' Summary:
        '     Returns a System.IO.DriveInfo object for the specified drive.
        '
        ' Parameters:
        '   drive:
        '     Drive to be examined.
        '
        ' Returns:
        '     System.IO.DriveInfo object for the specified drive.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     drive is Nothing or an empty string.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="drive"></param>
        ''' <returns></returns>
        Public Function GetDriveInfo(drive As String) As System.IO.DriveInfo

        End Function
        '
        ' Summary:
        '     Returns a System.IO.FileInfo object for the specified file.
        '
        ' Parameters:
        '   file:
        '     Name and path of the file.
        '
        ' Returns:
        '     System.IO.FileInfo object for the specified file
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path name is malformed. For example, it contains invalid characters or is
        '     only white space. The file name has a trailing slash mark.
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing or an empty string.
        '
        '   T:System.NotSupportedException:
        '     The path contains a colon in the middle of the string.
        '
        '   T:System.IO.PathTooLongException:
        '     The path is too long.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user lacks ACL (access control list) access to the file.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function GetFileInfo(file As String) As System.IO.FileInfo

        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     directory is Nothing.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The directory to search does not exist.
        '
        '   T:System.IO.IOException:
        '     directory points to an existing file.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user lacks necessary permissions.
        ''' <summary>
        ''' Returns a read-only collection of strings representing the names of files within
        ''' a directory.
        ''' </summary>
        ''' <param name="directory">Directory to be searched.</param>
        ''' <returns>Read-only collection of file names from the specified directory.</returns>
        Public Function GetFiles(directory As String) As ReadOnlyCollection(Of String)
            Return GetFiles(directory, FileIO.SearchOption.SearchTopLevelOnly)
        End Function
        '
        ' Summary:
        '     Returns a read-only collection of strings representing the names of files within
        '     a directory.
        '
        ' Parameters:
        '   directory:
        '     Directory to be searched.
        '
        '   searchType:
        '     Whether to include subfolders. Default is SearchOption.SearchTopLevelOnly.
        '
        '   wildcards:
        '     Pattern to be matched.
        '
        ' Returns:
        '     Read-only collection of file names from the specified directory.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentNullException:
        '     directory is Nothing.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The directory to search does not exist.
        '
        '   T:System.IO.IOException:
        '     directory points to an existing file.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user lacks necessary permissions.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <param name="searchType"></param>
        ''' <param name="wildcards"></param>
        ''' <returns></returns>
        Public Function GetFiles(directory As String, searchType As FileIO.SearchOption, ParamArray wildcards() As String) As ReadOnlyCollection(Of String)

        End Function

        '
        ' Summary:
        '     Returns the parent path of the provided path.
        '
        ' Parameters:
        '   path:
        '     Path to be examined.
        '
        ' Returns:
        '     The parent path of the provided path.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\).
        '
        '   T:System.ArgumentException:
        '     Path does not have a parent path because it is a root path.
        '
        '   T:System.ArgumentNullException:
        '     path is Nothing.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function GetParentPath(path As String) As String

        End Function
        '
        ' Summary:
        '     Creates a uniquely named zero-byte temporary file on disk and returns the full
        '     path of that file.
        '
        ' Returns:
        '     String containing the full path of the temporary file.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Function GetTempFileName() As String

        End Function

        ''' <summary>
        ''' 在远程服务器上面打开一个文件句柄
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function OpenFileHandle(file As String, mode As FileMode, access As FileAccess) As FileStreamInfo
            Return OpenFileHandle(file, mode, access, Me.Portal)
        End Function

        ''' <summary>
        ''' 在远程服务器上面打开一个文件句柄
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Shared Function OpenFileHandle(file As String, mode As FileMode, access As FileAccess, portal As IPEndPoint) As FileStreamInfo
            Dim req As RequestStream = Protocols.API.OpenHandle(file, mode, access)
            Dim invoke As New AsynInvoke(portal)
            Dim rep As RequestStream = invoke.SendMessage(req)
            Return rep.GetUTF8String.LoadObject(Of FileStreamInfo)
        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\); it ends with a trailing slash.
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file does not exist.
        '
        '   T:System.IO.IOException:
        '     The file is in use by another process, or an I/O error occurs.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:Microsoft.VisualBasic.FileIO.MalformedLineException:
        '     A row cannot be parsed using the specified format. The exception message specifies
        '     the line causing the exception, while the Microsoft.VisualBasic.FileIO.TextFieldParser.ErrorLine
        '     property is assigned the text contained in the line.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        ''' <summary>
        ''' The OpenTextFieldParser method allows you to create a Microsoft.VisualBasic.FileIO.TextFieldParser
        ''' object, which provides a way to easily and efficiently parse structured text
        ''' files, such as logs. The TextFieldParser object can be used to read both delimited
        ''' and fixed-width files.
        ''' </summary>
        ''' <param name="file">The file to be opened with the TextFieldParser.</param>
        ''' <param name="delimiters">Delimiters for the fields.</param>
        ''' <returns>Microsoft.VisualBasic.FileIO.TextFieldParser to read the specified file.</returns>
        Public Function OpenTextFieldParser(file As String, ParamArray delimiters() As String) As TextFieldParser
            Dim fileStream As New IO.RemoteFileStream(file, FileMode.Open, Me)
            Dim parser As New TextFieldParser(fileStream, System.Text.Encoding.Default, True)
            parser.Delimiters = delimiters
            Return parser
        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\); it ends with a trailing slash.
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file does not exist.
        '
        '   T:System.IO.IOException:
        '     The file is in use by another process, or an I/O error occurs.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:Microsoft.VisualBasic.FileIO.MalformedLineException:
        '     A row cannot be parsed using the specified format. The exception message specifies
        '     the line causing the exception, while the Microsoft.VisualBasic.FileIO.TextFieldParser.ErrorLine
        '     property is assigned the text contained in the line.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        ''' <summary>
        ''' The OpenTextFieldParser method allows you to create a Microsoft.VisualBasic.FileIO.TextFieldParser
        ''' object, which provides a way to easily and efficiently parse structured text
        ''' files, such as logs. The TextFieldParser object can be used to read both delimited
        ''' and fixed-width files.
        ''' </summary>
        ''' <param name="file">The file to be opened with the TextFieldParser.</param>
        ''' <param name="fieldWidths">Widths of the fields.</param>
        ''' <returns>Microsoft.VisualBasic.FileIO.TextFieldParser to read the specified file.</returns>
        Public Function OpenTextFieldParser(file As String, ParamArray fieldWidths() As Integer) As TextFieldParser
            Dim fileStream As New IO.RemoteFileStream(file, FileMode.Open, Me)
            Dim parser As New TextFieldParser(fileStream, System.Text.Encoding.Default, True)
            parser.FieldWidths = fieldWidths
            Return parser
        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The file name ends with a backslash (\).
        '
        '   T:System.IO.FileNotFoundException:
        '     The specified file cannot be found.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to read from the file.
        ''' <summary>
        ''' Opens a System.IO.StreamReader object to read from a file.
        ''' </summary>
        ''' <param name="file">File to be read.</param>
        ''' <returns>System.IO.StreamReader object to read from the file</returns>
        Public Function OpenTextFileReader(file As String) As System.IO.StreamReader
            Return OpenTextFileReader(file, Encoding.ASCII)
        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The file name ends with a backslash (\).
        '
        '   T:System.IO.FileNotFoundException:
        '     The specified file cannot be found.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to read from the file.
        ''' <summary>
        ''' Opens a System.IO.StreamReader object to read from a file.
        ''' </summary>
        ''' <param name="file">File to be read.</param>
        ''' <param name="encoding">The encoding to use for the file contents. Default is ASCII.</param>
        ''' <returns>System.IO.StreamReader object to read from the file</returns>
        Public Function OpenTextFileReader(file As String, encoding As Encoding) As System.IO.StreamReader
            Dim fileStream As New IO.RemoteFileStream(file, FileMode.OpenOrCreate, Me)
            Dim reader As New System.IO.StreamReader(fileStream, encoding)
            Return reader
        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The file name ends with a trailing slash.
        ''' <summary>
        ''' Opens a System.IO.StreamWriter object to write to the specified file.
        ''' </summary>
        ''' <param name="file">File to be written to.</param>
        ''' <param name="append">True to append to the contents of the file; False to overwrite the contents of
        ''' the file. Default is False.</param>
        ''' <returns>System.IO.StreamWriter object to write to the specified file.</returns>
        Public Function OpenTextFileWriter(file As String, append As Boolean) As System.IO.StreamWriter
            Return OpenTextFileWriter(file, append, System.Text.Encoding.ASCII)
        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The file name ends with a trailing slash.
        ''' <summary>
        ''' Opens a System.IO.StreamWriter to write to the specified file.
        ''' </summary>
        ''' <param name="file">File to be written to.</param>
        ''' <param name="append">True to append to the contents in the file; False to overwrite the contents of
        ''' the file. Default is False.</param>
        ''' <param name="encoding">Encoding to be used in writing to the file. Default is ASCII.</param>
        ''' <returns>System.IO.StreamWriter object to write to the specified file.</returns>
        Public Function OpenTextFileWriter(file As String, append As Boolean, encoding As Encoding) As System.IO.StreamWriter
            Dim mode As FileMode = If(append, FileMode.Append, FileMode.OpenOrCreate)
            Dim fileStream As New IO.RemoteFileStream(file, mode, Me)
            Return New StreamWriter(fileStream, encoding)
        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\); it ends with a trailing slash.
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file does not exist.
        '
        '   T:System.IO.IOException:
        '     The file is in use by another process, or an I/O error occurs.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.OutOfMemoryException:
        '     There is not enough memory to write the string to buffer.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        ''' <summary>
        ''' Returns the contents of a file as a byte array.
        ''' </summary>
        ''' <param name="file">File to be read.</param>
        ''' <returns>Byte array containing the contents of the file.</returns>
        Public Function ReadAllBytes(file As String) As Byte()
            Dim fileStream As New IO.RemoteFileStream(file, FileMode.Open, Me)
            Dim buffer As Byte() = New Byte(fileStream.Length - 1) {}
            Call fileStream.Read(buffer, Scan0, buffer.Length)
            Return buffer
        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\); it ends with a trailing slash.
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file does not exist.
        '
        '   T:System.IO.IOException:
        '     The file is in use by another process, or an I/O error occurs.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.OutOfMemoryException:
        '     There is not enough memory to write the string to buffer.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.

        ''' <summary>
        ''' Returns the contents of a text file as a String.
        ''' </summary>
        ''' <param name="file">Name and path of the file to read.</param>
        ''' <returns>String containing the contents of the file.</returns>
        Public Function ReadAllText(file As String) As String
            Return ReadAllText(file, Encoding.ASCII)
        End Function

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The path is not valid for one of the following reasons: it is a zero-length string;
        '     it contains only white space; it contains invalid characters; or it is a device
        '     path (starts with \\.\); it ends with a trailing slash.
        '
        '   T:System.ArgumentNullException:
        '     file is Nothing.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file does not exist.
        '
        '   T:System.IO.IOException:
        '     The file is in use by another process, or an I/O error occurs.
        '
        '   T:System.IO.PathTooLongException:
        '     The path exceeds the system-defined maximum length.
        '
        '   T:System.NotSupportedException:
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.OutOfMemoryException:
        '     There is not enough memory to write the string to buffer.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        ''' <summary>
        ''' Returns the contents of a text file as a String.
        ''' </summary>
        ''' <param name="file">Name and path of the file to read.</param>
        ''' <param name="encoding">Character encoding to use in reading the file. Default is UTF-8.</param>
        ''' <returns>String containing the contents of the file.</returns>
        Public Function ReadAllText(file As String, encoding As Encoding) As String
            Dim reader = OpenTextFileReader(file, encoding)
            Return reader.ReadToEnd
        End Function
    End Class
End Namespace
