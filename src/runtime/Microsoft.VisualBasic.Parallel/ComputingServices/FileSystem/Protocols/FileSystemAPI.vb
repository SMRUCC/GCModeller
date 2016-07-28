#Region "Microsoft.VisualBasic::0602f8ff01f58050c101d73777336fd2, ..\ComputingServices\FileSystem\Protocols\FileSystemAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.Net.Protocols.Reflection

Namespace FileSystem.Protocols

    Public Enum FileSystemAPI As Long

        ''' <summary>
        ''' 在服务器上面打开一个文件句柄
        ''' </summary>
        OpenHandle
        GetFileStreamInfo
        ''' <summary>
        ''' 获取或者得到文件流里面的指针的位置
        ''' </summary>
        FilePosition
        FileStreamLength
        StreamSeek
        StreamLock
        ''' <summary>
        ''' Reads a block of bytes from the stream and writes the data in a given buffer.
        ''' </summary>
        ReadBuffer
        WriteBuffer
        CloseHandle
        Flush
        ''' <summary>
        ''' Gets or sets the current directory.
        ''' </summary>
        CurrentDirectory

        ''' <summary>
        ''' Returns a read-only collection of all available drive names.
        ''' </summary>
        Drives

        '
        ' Summary:
        '     Copies the contents of a directory to another directory.
        '
        ' Parameters:
        '   sourceDirectoryName:
        '     The directory to be copied.
        '
        '   destinationDirectoryName:
        '     The location to which the directory contents should be copied.
        '
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
        CopyDirectory

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
        CopyFile

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
        CreateDirectory
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
        DeleteDirectory


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
        DeleteFile

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
        MoveDirectory
        '
        ' Summary:
        '     Moves a file to a new location.
        '
        ' Parameters:
        '   sourceFileName:
        '     Path of the file to be moved.
        '
        '   destinationFileName:
        '     Path of the directory into which the file should be moved.
        '
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
        MoveFile
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
        RenameDirectory
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
        RenameFile

        '
        ' Summary:
        '     Combines two paths and returns a properly formatted path.
        '
        ' Parameters:
        '   baseDirectory:
        '     String. First path to be combined.
        '
        '   relativePath:
        '     String. Second path to be combined.
        '
        ' Returns:
        '     The combination of the specified paths.
        '
        ' Exceptions:
        '   T:System.ArgumentException:
        '     baseDirectory or relativePath are malformed paths.
        CombinePath
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
        DirectoryExists
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
        FileExists
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
        FindInFiles

        '
        ' Summary:
        '     Returns a collection of strings representing the path names of subdirectories
        '     within a directory.
        '
        ' Parameters:
        '   directory:
        '     Name and path of directory.
        '
        ' Returns:
        '     Read-only collection of the path names of subdirectories within the specified
        '     directory..
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
        '     A file or directory name in the path contains a colon (:) or is in an invalid
        '     format.
        '
        '   T:System.Security.SecurityException:
        '     The user lacks necessary permissions to view the path.
        '
        '   T:System.UnauthorizedAccessException:
        '     The user lacks necessary permissions.
        GetDirectories

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
        GetDirectoryInfo
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
        GetDriveInfo
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
        GetFileInfo
        '
        ' Summary:
        '     Returns a read-only collection of strings representing the names of files within
        '     a directory.
        '
        ' Parameters:
        '   directory:
        '     Directory to be searched.
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
        GetFiles

        '
        ' Summary:
        '     Parses the file name out of the path provided.
        '
        ' Parameters:
        '   path:
        '     Required. Path to be parsed. String.
        '
        ' Returns:
        '     The file name from the specified path.
        GetName
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
        GetParentPath
        '
        ' Summary:
        '     Creates a uniquely named zero-byte temporary file on disk and returns the full
        '     path of that file.
        '
        ' Returns:
        '     String containing the full path of the temporary file.
        GetTempFileName
    End Enum
End Namespace
