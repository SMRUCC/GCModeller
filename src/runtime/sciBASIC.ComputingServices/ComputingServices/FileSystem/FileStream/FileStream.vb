#Region "Microsoft.VisualBasic::c3b2bdae269fbc69e00db1143aaed3a4, ..\sciBASIC.ComputingServices\ComputingServices\FileSystem\FileStream\FileStream.vb"

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

Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization
Imports System.Security
Imports System.Threading
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Serialization.JSON
Imports sciBASIC.ComputingServices.FileSystem.FileSystem
Imports sciBASIC.ComputingServices.FileSystem.Protocols

Namespace FileSystem.IO

    ''' <summary>
    ''' Provides a System.IO.Stream for a file, supporting both synchronous and asynchronous
    ''' read and write operations.To browse the .NET Framework source code for this type,
    ''' see the Reference Source.
    ''' </summary>
    ''' 
    <XmlType("RemoteFileStream")>
    Public Class RemoteFileStream : Inherits BaseStream

        ' Exceptions:
        '   T:System.ArgumentException:
        '     path is an empty string (""), contains only white space, or contains one or more
        '     invalid characters. -or-path refers to a non-file device, such as "con:", "com1:",
        '     "lpt1:", etc. in an NTFS environment.
        '
        '   T:System.NotSupportedException:
        '     path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        '     non-NTFS environment.
        '
        '   T:System.ArgumentNullException:
        '     path is null.
        '
        '   T:System.Security.SecurityException:
        '     The caller does not have the required permission.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open,
        '     and the file specified by path does not exist. The file must already exist in
        '     these modes.
        '
        '   T:System.IO.IOException:
        '     An I/O error, such as specifying FileMode.CreateNew when the file specified by
        '     path already exists, occurred.-or-The stream has been closed.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The specified path is invalid, such as being on an unmapped drive.
        '
        '   T:System.IO.PathTooLongException:
        '     The specified path, file name, or both exceed the system-defined maximum length.
        '     For example, on Windows-based platforms, paths must be less than 248 characters,
        '     and file names must be less than 260 characters.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     mode contains an invalid value.

        ''' <summary>
        ''' port@remote_IP://hash+&lt;path>
        ''' </summary>
        ''' <returns>Using for json serialization</returns>
        <XmlText> Public Property hashInfo As String
            Get
                Return $"{FileSystem.Port}@{FileSystem.IPAddress}://{FileHandle.HashCode}+{FileHandle.FileName}"
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) Then
                    Return
                End If

                Dim handle As FileHandle = Nothing
                Call FileURI.FileStreamParser(value, FileSystem, handle)
                Dim req As RequestStream = RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.GetFileStreamInfo, handle)
                Dim invoke As New AsynInvoke(FileSystem)
                Dim rep As RequestStream = invoke.SendMessage(req)
                _Info = rep.LoadObject(Of FileStreamInfo)(AddressOf LoadObject)
                _Name = handle.FileName
            End Set
        End Property

        Sub New()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the System.IO.FileStream class with the specified
        ''' path and creation mode.
        ''' </summary>
        ''' <param name="path">A relative or absolute path for the file that the current FileStream object will
        ''' encapsulate.（远程机器上面的文件）</param>
        ''' <param name="mode">A constant that determines how to open or create the file.</param>
        <SecuritySafeCritical> Public Sub New(path As String, mode As FileMode, remote As FileSystem)
            Call Me.New(path, mode, FileAccess.Read, remote)
        End Sub

        <SecuritySafeCritical> Public Sub New(path As String, mode As FileMode, remote As IPEndPoint)
            Call Me.New(path, mode, FileAccess.Read, remote)
        End Sub

        ''' <summary>
        ''' File handle on the remote machine file system
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FileHandle As FileHandle
            Get
                Return Info
            End Get
        End Property
        Public ReadOnly Property Info As FileStreamInfo

        ' Exceptions:
        '   T:System.ArgumentException:
        '     access is not a field of System.IO.FileAccess.
        '
        '   T:System.Security.SecurityException:
        '     The caller does not have the required permission.
        '
        '   T:System.IO.IOException:
        '     An I/O error, such as a disk error, occurred.-or-The stream has been closed.
        '
        '   T:System.UnauthorizedAccessException:
        '     The access requested is not permitted by the operating system for the specified
        '     file handle, such as when access is Write or ReadWrite and the file handle is
        '     set for read-only access.

        ''' <summary>
        ''' Initializes a new instance of the System.IO.FileStream class for the specified
        ''' file handle, with the specified read/write permission.
        ''' </summary>
        ''' <param name="handle">A file handle for the file that the current FileStream object will encapsulate.</param>
        ''' <param name="remote"></param>
        <SecuritySafeCritical> Public Sub New(handle As FileHandle, remote As FileSystem)
            Call MyBase.New(remote)
            Dim req As RequestStream = RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.GetFileStreamInfo, handle)
            Dim invoke As New AsynInvoke(remote.Portal)
            Dim rep As RequestStream = invoke.SendMessage(req)
            Info = rep.LoadObject(Of FileStreamInfo)(AddressOf LoadObject)
            Name = handle.FileName
        End Sub

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     path is null.
        '
        '   T:System.ArgumentException:
        '     path is an empty string (""), contains only white space, or contains one or more
        '     invalid characters. -or-path refers to a non-file device, such as "con:", "com1:",
        '     "lpt1:", etc. in an NTFS environment.
        '
        '   T:System.NotSupportedException:
        '     path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        '     non-NTFS environment.
        '
        '   T:System.IO.FileNotFoundException:
        '     The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open,
        '     and the file specified by path does not exist. The file must already exist in
        '     these modes.
        '
        '   T:System.IO.IOException:
        '     An I/O error, such as specifying FileMode.CreateNew when the file specified by
        '     path already exists, occurred. -or-The stream has been closed.
        '
        '   T:System.Security.SecurityException:
        '     The caller does not have the required permission.
        '
        '   T:System.IO.DirectoryNotFoundException:
        '     The specified path is invalid, such as being on an unmapped drive.
        '
        '   T:System.UnauthorizedAccessException:
        '     The access requested is not permitted by the operating system for the specified
        '     path, such as when access is Write or ReadWrite and the file or directory is
        '     set for read-only access.
        '
        '   T:System.IO.PathTooLongException:
        '     The specified path, file name, or both exceed the system-defined maximum length.
        '     For example, on Windows-based platforms, paths must be less than 248 characters,
        '     and file names must be less than 260 characters.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     mode contains an invalid value.

        ''' <summary>
        ''' Initializes a new instance of the System.IO.FileStream class with the specified
        ''' path, creation mode, and read/write permission.
        ''' </summary>
        ''' <param name="path">A relative or absolute path for the file that the current FileStream object will
        ''' encapsulate.</param>
        ''' <param name="mode">A constant that determines how to open or create the file.</param>
        ''' <param name="access">A constant that determines how the file can be accessed by the FileStream object.
        ''' This also determines the values returned by the System.IO.FileStream.CanRead
        ''' and System.IO.FileStream.CanWrite properties of the FileStream object. System.IO.FileStream.CanSeek
        ''' is true if path specifies a disk file.</param>
        ''' <param name="remote"></param>
        <SecuritySafeCritical>
        Public Sub New(path As String, mode As FileMode, access As FileAccess, remote As FileSystem)
            Call MyBase.New(remote)
            Name = path
            Info = remote.OpenFileHandle(path, mode, access)
        End Sub

        <SecuritySafeCritical>
        Public Sub New(path As String, mode As FileMode, access As FileAccess, remote As IPEndPoint)
            Call MyBase.New(remote)
            Name = path
            Info = OpenFileHandle(path, mode, access, remote)
        End Sub

        <SecuritySafeCritical>
        Public Sub New(path As String, mode As FileMode, access As FileAccess)
            Call Me.New(New FileURI(path), mode, access)
        End Sub

        <SecuritySafeCritical>
        Public Sub New(uri As FileURI, mode As FileMode, access As FileAccess)
            Call Me.New(uri.File, mode, access, uri.EntryPoint)
        End Sub

        ''
        '' Summary:
        ''     Initializes a new instance of the System.IO.FileStream class for the specified
        ''     file handle, with the specified read/write permission, and buffer size.
        ''
        '' Parameters:
        ''   handle:
        ''     A file handle for the file that the current FileStream object will encapsulate.
        ''
        ''   access:
        ''     A System.IO.FileAccess constant that sets the System.IO.FileStream.CanRead and
        ''     System.IO.FileStream.CanWrite properties of the FileStream object.
        ''
        ''   bufferSize:
        ''     A positive System.Int32 value greater than 0 indicating the buffer size. The
        ''     default buffer size is 4096.
        ''
        '' Exceptions:
        ''   T:System.ArgumentException:
        ''     The handle parameter is an invalid handle.-or-The handle parameter is a synchronous
        ''     handle and it was used asynchronously.
        ''
        ''   T:System.ArgumentOutOfRangeException:
        ''     The bufferSize parameter is negative.
        ''
        ''   T:System.IO.IOException:
        ''     An I/O error, such as a disk error, occurred.-or-The stream has been closed.
        ''
        ''   T:System.Security.SecurityException:
        ''     The caller does not have the required permission.
        ''
        ''   T:System.UnauthorizedAccessException:
        ''     The access requested is not permitted by the operating system for the specified
        ''     file handle, such as when access is Write or ReadWrite and the file handle is
        ''     set for read-only access.
        '<SecuritySafeCritical>
        'Public Sub New(handle As SafeFileHandle, access As FileAccess, bufferSize As Integer)

        'End Sub

        ''
        '' Summary:
        ''     Initializes a new instance of the System.IO.FileStream class with the specified
        ''     path, creation mode, read/write permission, and sharing permission.
        ''
        '' Parameters:
        ''   path:
        ''     A relative or absolute path for the file that the current FileStream object will
        ''     encapsulate.
        ''
        ''   mode:
        ''     A constant that determines how to open or create the file.
        ''
        ''   access:
        ''     A constant that determines how the file can be accessed by the FileStream object.
        ''     This also determines the values returned by the System.IO.FileStream.CanRead
        ''     and System.IO.FileStream.CanWrite properties of the FileStream object. System.IO.FileStream.CanSeek
        ''     is true if path specifies a disk file.
        ''
        ''   share:
        ''     A constant that determines how the file will be shared by processes.
        ''
        '' Exceptions:
        ''   T:System.ArgumentNullException:
        ''     path is null.
        ''
        ''   T:System.ArgumentException:
        ''     path is an empty string (""), contains only white space, or contains one or more
        ''     invalid characters. -or-path refers to a non-file device, such as "con:", "com1:",
        ''     "lpt1:", etc. in an NTFS environment.
        ''
        ''   T:System.NotSupportedException:
        ''     path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        ''     non-NTFS environment.
        ''
        ''   T:System.IO.FileNotFoundException:
        ''     The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open,
        ''     and the file specified by path does not exist. The file must already exist in
        ''     these modes.
        ''
        ''   T:System.IO.IOException:
        ''     An I/O error, such as specifying FileMode.CreateNew when the file specified by
        ''     path already exists, occurred. -or-The system is running Windows 98 or Windows
        ''     98 Second Edition and share is set to FileShare.Delete.-or-The stream has been
        ''     closed.
        ''
        ''   T:System.Security.SecurityException:
        ''     The caller does not have the required permission.
        ''
        ''   T:System.IO.DirectoryNotFoundException:
        ''     The specified path is invalid, such as being on an unmapped drive.
        ''
        ''   T:System.UnauthorizedAccessException:
        ''     The access requested is not permitted by the operating system for the specified
        ''     path, such as when access is Write or ReadWrite and the file or directory is
        ''     set for read-only access.
        ''
        ''   T:System.IO.PathTooLongException:
        ''     The specified path, file name, or both exceed the system-defined maximum length.
        ''     For example, on Windows-based platforms, paths must be less than 248 characters,
        ''     and file names must be less than 260 characters.
        ''
        ''   T:System.ArgumentOutOfRangeException:
        ''     mode contains an invalid value.
        '<SecuritySafeCritical>
        'Public Sub New(path As String, mode As FileMode, access As FileAccess, share As FileShare)

        'End Sub
        ''
        '' Summary:
        ''     Initializes a new instance of the System.IO.FileStream class for the specified
        ''     file handle, with the specified read/write permission, buffer size, and synchronous
        ''     or asynchronous state.
        ''
        '' Parameters:
        ''   handle:
        ''     A file handle for the file that this FileStream object will encapsulate.
        ''
        ''   access:
        ''     A constant that sets the System.IO.FileStream.CanRead and System.IO.FileStream.CanWrite
        ''     properties of the FileStream object.
        ''
        ''   bufferSize:
        ''     A positive System.Int32 value greater than 0 indicating the buffer size. The
        ''     default buffer size is 4096.
        ''
        ''   isAsync:
        ''     true if the handle was opened asynchronously (that is, in overlapped I/O mode);
        ''     otherwise, false.
        ''
        '' Exceptions:
        ''   T:System.ArgumentException:
        ''     The handle parameter is an invalid handle.-or-The handle parameter is a synchronous
        ''     handle and it was used asynchronously.
        ''
        ''   T:System.ArgumentOutOfRangeException:
        ''     The bufferSize parameter is negative.
        ''
        ''   T:System.IO.IOException:
        ''     An I/O error, such as a disk error, occurred.-or-The stream has been closed.
        ''
        ''   T:System.Security.SecurityException:
        ''     The caller does not have the required permission.
        ''
        ''   T:System.UnauthorizedAccessException:
        ''     The access requested is not permitted by the operating system for the specified
        ''     file handle, such as when access is Write or ReadWrite and the file handle is
        ''     set for read-only access.
        '<SecuritySafeCritical>
        'Public Sub New(handle As SafeFileHandle, access As FileAccess, bufferSize As Integer, isAsync As Boolean)

        'End Sub
        ''
        '' Summary:
        ''     Initializes a new instance of the System.IO.FileStream class for the specified
        ''     file handle, with the specified read/write permission, FileStream instance ownership,
        ''     and buffer size.
        ''
        '' Parameters:
        ''   handle:
        ''     A file handle for the file that this FileStream object will encapsulate.
        ''
        ''   access:
        ''     A constant that sets the System.IO.FileStream.CanRead and System.IO.FileStream.CanWrite
        ''     properties of the FileStream object.
        ''
        ''   ownsHandle:
        ''     true if the file handle will be owned by this FileStream instance; otherwise,
        ''     false.
        ''
        ''   bufferSize:
        ''     A positive System.Int32 value greater than 0 indicating the buffer size. The
        ''     default buffer size is 4096.
        ''
        '' Exceptions:
        ''   T:System.ArgumentOutOfRangeException:
        ''     bufferSize is negative.
        ''
        ''   T:System.IO.IOException:
        ''     An I/O error, such as a disk error, occurred.-or-The stream has been closed.
        ''
        ''   T:System.Security.SecurityException:
        ''     The caller does not have the required permission.
        ''
        ''   T:System.UnauthorizedAccessException:
        ''     The access requested is not permitted by the operating system for the specified
        ''     file handle, such as when access is Write or ReadWrite and the file handle is
        ''     set for read-only access.
        '<Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")>
        'Public Sub New(handle As IntPtr, access As FileAccess, ownsHandle As Boolean, bufferSize As Integer)

        'End Sub
        ''
        '' Summary:
        ''     Initializes a new instance of the System.IO.FileStream class for the specified
        ''     file handle, with the specified read/write permission, FileStream instance ownership,
        ''     buffer size, and synchronous or asynchronous state.
        ''
        '' Parameters:
        ''   handle:
        ''     A file handle for the file that this FileStream object will encapsulate.
        ''
        ''   access:
        ''     A constant that sets the System.IO.FileStream.CanRead and System.IO.FileStream.CanWrite
        ''     properties of the FileStream object.
        ''
        ''   ownsHandle:
        ''     true if the file handle will be owned by this FileStream instance; otherwise,
        ''     false.
        ''
        ''   bufferSize:
        ''     A positive System.Int32 value greater than 0 indicating the buffer size. The
        ''     default buffer size is 4096.
        ''
        ''   isAsync:
        ''     true if the handle was opened asynchronously (that is, in overlapped I/O mode);
        ''     otherwise, false.
        ''
        '' Exceptions:
        ''   T:System.ArgumentOutOfRangeException:
        ''     access is less than FileAccess.Read or greater than FileAccess.ReadWrite or bufferSize
        ''     is less than or equal to 0.
        ''
        ''   T:System.ArgumentException:
        ''     The handle is invalid.
        ''
        ''   T:System.IO.IOException:
        ''     An I/O error, such as a disk error, occurred.-or-The stream has been closed.
        ''
        ''   T:System.Security.SecurityException:
        ''     The caller does not have the required permission.
        ''
        ''   T:System.UnauthorizedAccessException:
        ''     The access requested is not permitted by the operating system for the specified
        ''     file handle, such as when access is Write or ReadWrite and the file handle is
        ''     set for read-only access.
        '<Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")> <SecuritySafeCritical>
        'Public Sub New(handle As IntPtr, access As FileAccess, ownsHandle As Boolean, bufferSize As Integer, isAsync As Boolean)

        'End Sub
        ''
        '' Summary:
        ''     Initializes a new instance of the System.IO.FileStream class with the specified
        ''     path, creation mode, read/write and sharing permission, and buffer size.
        ''
        '' Parameters:
        ''   path:
        ''     A relative or absolute path for the file that the current FileStream object will
        ''     encapsulate.
        ''
        ''   mode:
        ''     A constant that determines how to open or create the file.
        ''
        ''   access:
        ''     A constant that determines how the file can be accessed by the FileStream object.
        ''     This also determines the values returned by the System.IO.FileStream.CanRead
        ''     and System.IO.FileStream.CanWrite properties of the FileStream object. System.IO.FileStream.CanSeek
        ''     is true if path specifies a disk file.
        ''
        ''   share:
        ''     A constant that determines how the file will be shared by processes.
        ''
        ''   bufferSize:
        ''     A positive System.Int32 value greater than 0 indicating the buffer size. The
        ''     default buffer size is 4096.
        ''
        '' Exceptions:
        ''   T:System.ArgumentNullException:
        ''     path is null.
        ''
        ''   T:System.ArgumentException:
        ''     path is an empty string (""), contains only white space, or contains one or more
        ''     invalid characters. -or-path refers to a non-file device, such as "con:", "com1:",
        ''     "lpt1:", etc. in an NTFS environment.
        ''
        ''   T:System.NotSupportedException:
        ''     path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        ''     non-NTFS environment.
        ''
        ''   T:System.ArgumentOutOfRangeException:
        ''     bufferSize is negative or zero.-or- mode, access, or share contain an invalid
        ''     value.
        ''
        ''   T:System.IO.FileNotFoundException:
        ''     The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open,
        ''     and the file specified by path does not exist. The file must already exist in
        ''     these modes.
        ''
        ''   T:System.IO.IOException:
        ''     An I/O error, such as specifying FileMode.CreateNew when the file specified by
        ''     path already exists, occurred. -or-The system is running Windows 98 or Windows
        ''     98 Second Edition and share is set to FileShare.Delete.-or-The stream has been
        ''     closed.
        ''
        ''   T:System.Security.SecurityException:
        ''     The caller does not have the required permission.
        ''
        ''   T:System.IO.DirectoryNotFoundException:
        ''     The specified path is invalid, such as being on an unmapped drive.
        ''
        ''   T:System.UnauthorizedAccessException:
        ''     The access requested is not permitted by the operating system for the specified
        ''     path, such as when access is Write or ReadWrite and the file or directory is
        ''     set for read-only access.
        ''
        ''   T:System.IO.PathTooLongException:
        ''     The specified path, file name, or both exceed the system-defined maximum length.
        ''     For example, on Windows-based platforms, paths must be less than 248 characters,
        ''     and file names must be less than 260 characters.
        '<SecuritySafeCritical>
        'Public Sub New(path As String, mode As FileMode, access As FileAccess, share As FileShare, bufferSize As Integer)

        'End Sub
        ''
        '' Summary:
        ''     Initializes a new instance of the System.IO.FileStream class with the specified
        ''     path, creation mode, read/write and sharing permission, buffer size, and synchronous
        ''     or asynchronous state.
        ''
        '' Parameters:
        ''   path:
        ''     A relative or absolute path for the file that the current FileStream object will
        ''     encapsulate.
        ''
        ''   mode:
        ''     A constant that determines how to open or create the file.
        ''
        ''   access:
        ''     A constant that determines how the file can be accessed by the FileStream object.
        ''     This also determines the values returned by the System.IO.FileStream.CanRead
        ''     and System.IO.FileStream.CanWrite properties of the FileStream object. System.IO.FileStream.CanSeek
        ''     is true if path specifies a disk file.
        ''
        ''   share:
        ''     A constant that determines how the file will be shared by processes.
        ''
        ''   bufferSize:
        ''     A positive System.Int32 value greater than 0 indicating the buffer size. The
        ''     default buffer size is 4096..
        ''
        ''   useAsync:
        ''     Specifies whether to use asynchronous I/O or synchronous I/O. However, note that
        ''     the underlying operating system might not support asynchronous I/O, so when specifying
        ''     true, the handle might be opened synchronously depending on the platform. When
        ''     opened asynchronously, the System.IO.FileStream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        ''     and System.IO.FileStream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        ''     methods perform better on large reads or writes, but they might be much slower
        ''     for small reads or writes. If the application is designed to take advantage of
        ''     asynchronous I/O, set the useAsync parameter to true. Using asynchronous I/O
        ''     correctly can speed up applications by as much as a factor of 10, but using it
        ''     without redesigning the application for asynchronous I/O can decrease performance
        ''     by as much as a factor of 10.
        ''
        '' Exceptions:
        ''   T:System.ArgumentNullException:
        ''     path is null.
        ''
        ''   T:System.ArgumentException:
        ''     path is an empty string (""), contains only white space, or contains one or more
        ''     invalid characters. -or-path refers to a non-file device, such as "con:", "com1:",
        ''     "lpt1:", etc. in an NTFS environment.
        ''
        ''   T:System.NotSupportedException:
        ''     path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        ''     non-NTFS environment.
        ''
        ''   T:System.ArgumentOutOfRangeException:
        ''     bufferSize is negative or zero.-or- mode, access, or share contain an invalid
        ''     value.
        ''
        ''   T:System.IO.FileNotFoundException:
        ''     The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open,
        ''     and the file specified by path does not exist. The file must already exist in
        ''     these modes.
        ''
        ''   T:System.IO.IOException:
        ''     An I/O error, such as specifying FileMode.CreateNew when the file specified by
        ''     path already exists, occurred.-or- The system is running Windows 98 or Windows
        ''     98 Second Edition and share is set to FileShare.Delete.-or-The stream has been
        ''     closed.
        ''
        ''   T:System.Security.SecurityException:
        ''     The caller does not have the required permission.
        ''
        ''   T:System.IO.DirectoryNotFoundException:
        ''     The specified path is invalid, such as being on an unmapped drive.
        ''
        ''   T:System.UnauthorizedAccessException:
        ''     The access requested is not permitted by the operating system for the specified
        ''     path, such as when access is Write or ReadWrite and the file or directory is
        ''     set for read-only access.
        ''
        ''   T:System.IO.PathTooLongException:
        ''     The specified path, file name, or both exceed the system-defined maximum length.
        ''     For example, on Windows-based platforms, paths must be less than 248 characters,
        ''     and file names must be less than 260 characters.
        '<SecuritySafeCritical>
        'Public Sub New(path As String, mode As FileMode, access As FileAccess, share As FileShare, bufferSize As Integer, useAsync As Boolean)

        'End Sub
        ''
        '' Summary:
        ''     Initializes a new instance of the System.IO.FileStream class with the specified
        ''     path, creation mode, read/write and sharing permission, the access other FileStreams
        ''     can have to the same file, the buffer size, and additional file options.
        ''
        '' Parameters:
        ''   path:
        ''     A relative or absolute path for the file that the current FileStream object will
        ''     encapsulate.
        ''
        ''   mode:
        ''     A constant that determines how to open or create the file.
        ''
        ''   access:
        ''     A constant that determines how the file can be accessed by the FileStream object.
        ''     This also determines the values returned by the System.IO.FileStream.CanRead
        ''     and System.IO.FileStream.CanWrite properties of the FileStream object. System.IO.FileStream.CanSeek
        ''     is true if path specifies a disk file.
        ''
        ''   share:
        ''     A constant that determines how the file will be shared by processes.
        ''
        ''   bufferSize:
        ''     A positive System.Int32 value greater than 0 indicating the buffer size. The
        ''     default buffer size is 4096.
        ''
        ''   options:
        ''     A value that specifies additional file options.
        ''
        '' Exceptions:
        ''   T:System.ArgumentNullException:
        ''     path is null.
        ''
        ''   T:System.ArgumentException:
        ''     path is an empty string (""), contains only white space, or contains one or more
        ''     invalid characters. -or-path refers to a non-file device, such as "con:", "com1:",
        ''     "lpt1:", etc. in an NTFS environment.
        ''
        ''   T:System.NotSupportedException:
        ''     path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        ''     non-NTFS environment.
        ''
        ''   T:System.ArgumentOutOfRangeException:
        ''     bufferSize is negative or zero.-or- mode, access, or share contain an invalid
        ''     value.
        ''
        ''   T:System.IO.FileNotFoundException:
        ''     The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open,
        ''     and the file specified by path does not exist. The file must already exist in
        ''     these modes.
        ''
        ''   T:System.IO.IOException:
        ''     An I/O error, such as specifying FileMode.CreateNew when the file specified by
        ''     path already exists, occurred.-or-The stream has been closed.
        ''
        ''   T:System.Security.SecurityException:
        ''     The caller does not have the required permission.
        ''
        ''   T:System.IO.DirectoryNotFoundException:
        ''     The specified path is invalid, such as being on an unmapped drive.
        ''
        ''   T:System.UnauthorizedAccessException:
        ''     The access requested is not permitted by the operating system for the specified
        ''     path, such as when access is Write or ReadWrite and the file or directory is
        ''     set for read-only access. -or-System.IO.FileOptions.Encrypted is specified for
        ''     options, but file encryption is not supported on the current platform.
        ''
        ''   T:System.IO.PathTooLongException:
        ''     The specified path, file name, or both exceed the system-defined maximum length.
        ''     For example, on Windows-based platforms, paths must be less than 248 characters,
        ''     and file names must be less than 260 characters.
        '<SecuritySafeCritical>
        'Public Sub New(path As String, mode As FileMode, access As FileAccess, share As FileShare, bufferSize As Integer, options As FileOptions)

        'End Sub
        ''
        '' Summary:
        ''     Initializes a new instance of the System.IO.FileStream class with the specified
        ''     path, creation mode, access rights and sharing permission, the buffer size, and
        ''     additional file options.
        ''
        '' Parameters:
        ''   path:
        ''     A relative or absolute path for the file that the current System.IO.FileStream
        ''     object will encapsulate.
        ''
        ''   mode:
        ''     A constant that determines how to open or create the file.
        ''
        ''   rights:
        ''     A constant that determines the access rights to use when creating access and
        ''     audit rules for the file.
        ''
        ''   share:
        ''     A constant that determines how the file will be shared by processes.
        ''
        ''   bufferSize:
        ''     A positive System.Int32 value greater than 0 indicating the buffer size. The
        ''     default buffer size is 4096.
        ''
        ''   options:
        ''     A constant that specifies additional file options.
        ''
        '' Exceptions:
        ''   T:System.ArgumentNullException:
        ''     path is null.
        ''
        ''   T:System.ArgumentException:
        ''     path is an empty string (""), contains only white space, or contains one or more
        ''     invalid characters. -or-path refers to a non-file device, such as "con:", "com1:",
        ''     "lpt1:", etc. in an NTFS environment.
        ''
        ''   T:System.NotSupportedException:
        ''     path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        ''     non-NTFS environment.
        ''
        ''   T:System.ArgumentOutOfRangeException:
        ''     bufferSize is negative or zero.-or- mode, access, or share contain an invalid
        ''     value.
        ''
        ''   T:System.IO.FileNotFoundException:
        ''     The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open,
        ''     and the file specified by path does not exist. The file must already exist in
        ''     these modes.
        ''
        ''   T:System.PlatformNotSupportedException:
        ''     The current operating system is not Windows NT or later.
        ''
        ''   T:System.IO.IOException:
        ''     An I/O error, such as specifying FileMode.CreateNew when the file specified by
        ''     path already exists, occurred. -or-The stream has been closed.
        ''
        ''   T:System.Security.SecurityException:
        ''     The caller does not have the required permission.
        ''
        ''   T:System.IO.DirectoryNotFoundException:
        ''     The specified path is invalid, such as being on an unmapped drive.
        ''
        ''   T:System.UnauthorizedAccessException:
        ''     The access requested is not permitted by the operating system for the specified
        ''     path, such as when access is Write or ReadWrite and the file or directory is
        ''     set for read-only access. -or-System.IO.FileOptions.Encrypted is specified for
        ''     options, but file encryption is not supported on the current platform.
        ''
        ''   T:System.IO.PathTooLongException:
        ''     The specified path, file name, or both exceed the system-defined maximum length.
        ''     For example, on Windows-based platforms, paths must be less than 248 characters,
        ''     and file names must be less than 260 characters.
        '<SecuritySafeCritical>
        'Public Sub New(path As String, mode As FileMode, rights As FileSystemRights, share As FileShare, bufferSize As Integer, options As FileOptions)

        'End Sub
        ''
        '' Summary:
        ''     Initializes a new instance of the System.IO.FileStream class with the specified
        ''     path, creation mode, access rights and sharing permission, the buffer size, additional
        ''     file options, access control and audit security.
        ''
        '' Parameters:
        ''   path:
        ''     A relative or absolute path for the file that the current System.IO.FileStream
        ''     object will encapsulate.
        ''
        ''   mode:
        ''     A constant that determines how to open or create the file.
        ''
        ''   rights:
        ''     A constant that determines the access rights to use when creating access and
        ''     audit rules for the file.
        ''
        ''   share:
        ''     A constant that determines how the file will be shared by processes.
        ''
        ''   bufferSize:
        ''     A positive System.Int32 value greater than 0 indicating the buffer size. The
        ''     default buffer size is 4096.
        ''
        ''   options:
        ''     A constant that specifies additional file options.
        ''
        ''   fileSecurity:
        ''     A constant that determines the access control and audit security for the file.
        ''
        '' Exceptions:
        ''   T:System.ArgumentNullException:
        ''     path is null.
        ''
        ''   T:System.ArgumentException:
        ''     path is an empty string (""), contains only white space, or contains one or more
        ''     invalid characters. -or-path refers to a non-file device, such as "con:", "com1:",
        ''     "lpt1:", etc. in an NTFS environment.
        ''
        ''   T:System.NotSupportedException:
        ''     path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        ''     non-NTFS environment.
        ''
        ''   T:System.ArgumentOutOfRangeException:
        ''     bufferSize is negative or zero.-or- mode, access, or share contain an invalid
        ''     value.
        ''
        ''   T:System.IO.FileNotFoundException:
        ''     The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open,
        ''     and the file specified by path does not exist. The file must already exist in
        ''     these modes.
        ''
        ''   T:System.IO.IOException:
        ''     An I/O error, such as specifying FileMode.CreateNew when the file specified by
        ''     path already exists, occurred. -or-The stream has been closed.
        ''
        ''   T:System.Security.SecurityException:
        ''     The caller does not have the required permission.
        ''
        ''   T:System.IO.DirectoryNotFoundException:
        ''     The specified path is invalid, such as being on an unmapped drive.
        ''
        ''   T:System.UnauthorizedAccessException:
        ''     The access requested is not permitted by the operating system for the specified
        ''     path, such as when access is Write or ReadWrite and the file or directory is
        ''     set for read-only access. -or-System.IO.FileOptions.Encrypted is specified for
        ''     options, but file encryption is not supported on the current platform.
        ''
        ''   T:System.IO.PathTooLongException:
        ''     The specified path, file name, or both exceed the system-defined maximum length.
        ''     For example, on Windows-based platforms, paths must be less than 248 characters,
        ''     and file names must be less than 260 characters.
        ''
        ''   T:System.PlatformNotSupportedException:
        ''     The current operating system is not Windows NT or later.
        '<SecuritySafeCritical>
        'Public Sub New(path As String, mode As FileMode, rights As FileSystemRights, share As FileShare, bufferSize As Integer, options As FileOptions, fileSecurity As FileSecurity)

        'End Sub

        ''' <summary>
        ''' Gets a value indicating whether the current stream supports reading.
        ''' </summary>
        ''' <returns>true if the stream supports reading; false if the stream is closed or was opened
        ''' with write-only access.</returns>
        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Return Info.CanRead
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the current stream supports seeking.
        ''' </summary>
        ''' <returns>true if the stream supports seeking; false if the stream is closed or if the
        ''' FileStream was constructed from an operating-system handle such as a pipe or
        ''' output to the console.</returns>
        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Return Info.CanSeek
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the current stream supports writing.
        ''' </summary>
        ''' <returns>true if the stream supports writing; false if the stream is closed or was opened
        ''' with read-only access.</returns>
        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Return Info.CanWrite
            End Get
        End Property

        ' Exceptions:
        '   T:System.Security.SecurityException:
        '     The caller does not have the required permission.
        ''' <summary>
        ''' Gets the operating system file handle for the file that the current FileStream
        ''' object encapsulates.
        ''' </summary>
        ''' <returns>The operating system file handle for the file encapsulated by this FileStream
        ''' object, or -1 if the FileStream has been closed.</returns>
        <Obsolete("This property has been deprecated.  Please use FileStream's SafeFileHandle property instead.  http://go.microsoft.com/fwlink/?linkid=14202")>
        Public Overridable ReadOnly Property Handle As IntPtr
            Get
                Return Info.FileHandle
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the FileStream was opened asynchronously or synchronously.
        ''' </summary>
        ''' <returns>true if the FileStream was opened asynchronously; otherwise, false.</returns>
        Public Overridable ReadOnly Property IsAsync As Boolean
            Get
                Return Info.IsAsync
            End Get
        End Property

        ' Exceptions:
        '   T:System.NotSupportedException:
        '     System.IO.FileStream.CanSeek for this stream is false.
        '
        '   T:System.IO.IOException:
        '     An I/O error, such as the file being closed, occurred.
        ''' <summary>
        ''' Gets the length in bytes of the stream.
        ''' </summary>
        ''' <returns>A long value representing the length of the stream in bytes.</returns>
        Public Overrides ReadOnly Property Length As Long
            Get
                Dim args = Protocols.GetSetLength(FileStreamPosition.GET, FileHandle)
                Dim invoke As New AsynInvoke(FileSystem)
                Dim rep As RequestStream = invoke.SendMessage(args)
                Return CTypeDynamic(Of Long)(rep.GetUTF8String)
            End Get
        End Property

        ''' <summary>
        ''' Gets the name of the FileStream that was passed to the constructor.
        ''' </summary>
        ''' <returns>A string that is the name of the FileStream.</returns>
        Public ReadOnly Property Name As String

        ' Exceptions:
        '   T:System.NotSupportedException:
        '     The stream does not support seeking.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurred. - or -The position was set to a very large value beyond
        '     the end of the stream in Windows 98 or earlier.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     Attempted to set the position to a negative value.
        '
        '   T:System.IO.EndOfStreamException:
        '     Attempted seeking past the end of a stream that does not support this.
        ''' <summary>
        ''' Gets or sets the current position of this stream.
        ''' </summary>
        ''' <returns>The current position of this stream.</returns>
        <XmlIgnore>
        <SoapIgnore>
        <IgnoreDataMember>
        Public Overrides Property Position As Long
            Get
                Dim args = Protocols.GetSetReadPosition(FileStreamPosition.GET, FileHandle)
                Dim invoke As New AsynInvoke(FileSystem)
                Dim rep As RequestStream = invoke.SendMessage(args)
                Return CTypeDynamic(Of Long)(rep.GetUTF8String)
            End Get
            Set(value As Long)
                Dim args = Protocols.GetSetReadPosition(value, FileHandle)
                Dim invoke As New AsynInvoke(FileSystem)
                Dim rep As RequestStream = invoke.SendMessage(args)
            End Set
        End Property

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     asyncResult is null.
        '
        '   T:System.ArgumentException:
        '     This System.IAsyncResult object was not created by calling System.IO.Stream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        '     on this class.
        '
        '   T:System.InvalidOperationException:
        '     System.IO.FileStream.EndWrite(System.IAsyncResult) is called multiple times.
        '
        '   T:System.IO.IOException:
        '     The stream is closed or an internal error has occurred.
        ''' <summary>
        ''' Ends an asynchronous write operation and blocks until the I/O operation is complete.
        ''' (Consider using System.IO.FileStream.WriteAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)
        ''' instead; see the Remarks section.)
        ''' </summary>
        ''' <param name="asyncResult">The pending asynchronous I/O request.</param>
        <SecuritySafeCritical>
        Public Overrides Sub EndWrite(asyncResult As IAsyncResult)
            Call __writeHandle.EndInvoke(asyncResult)
        End Sub

        ' Exceptions:
        '   T:System.IO.IOException:
        '     An I/O error occurred.
        '
        '   T:System.ObjectDisposedException:
        '     The stream is closed.
        ''' <summary>
        ''' Clears buffers for this stream and causes any buffered data to be written to
        ''' the file.
        ''' </summary>
        Public Overrides Sub Flush()
            Dim req As RequestStream = RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.Flush, FileHandle)
            Dim invoke As New AsynInvoke(FileSystem)
            Call invoke.SendMessage(req)
        End Sub

        ''' <summary>
        ''' Clears buffers for this stream and causes any buffered data to be written to
        ''' the file, and also clears all intermediate file buffers.
        ''' </summary>
        ''' <param name="flushToDisk">true to flush all intermediate file buffers; otherwise, false.</param>
        <SecuritySafeCritical> Public Overloads Sub Flush(flushToDisk As Boolean)
            Call Flush()
        End Sub
        '
        ' Summary:
        '     Prevents other processes from reading from or writing to the System.IO.FileStream.
        '
        ' Parameters:
        '   position:
        '     The beginning of the range to lock. The value of this parameter must be equal
        '     to or greater than zero (0).
        '
        '   length:
        '     The range to be locked.
        '
        ' Exceptions:
        '   T:System.ArgumentOutOfRangeException:
        '     position or length is negative.
        '
        '   T:System.ObjectDisposedException:
        '     The file is closed.
        '
        '   T:System.IO.IOException:
        '     The process cannot access the file because another process has locked a portion
        '     of the file.
        <SecuritySafeCritical>
        Public Overridable Sub Lock(position As Long, length As Long)
            Call __lock(position, length, True)
        End Sub

        Private Sub __lock(position As Long, length As Long, lock As Boolean)
            Dim args As New LockArgs(FileHandle) With {.Lock = lock, .length = length, .position = position}
            Dim req As RequestStream = RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.StreamLock, args)
            Dim invoke As New AsynInvoke(FileSystem)
            Dim rep As RequestStream = invoke.SendMessage(req)
        End Sub

        '
        ' Summary:
        '     Allows access by other processes to all or part of a file that was previously
        '     locked.
        '
        ' Parameters:
        '   position:
        '     The beginning of the range to unlock.
        '
        '   length:
        '     The range to be unlocked.
        '
        ' Exceptions:
        '   T:System.ArgumentOutOfRangeException:
        '     position or length is negative.
        <SecuritySafeCritical>
        Public Overridable Sub Unlock(position As Long, length As Long)
            Call __lock(position, length, False)
        End Sub

        ' Exceptions:
        '   T:System.IO.IOException:
        '     An I/O error has occurred.
        '
        '   T:System.NotSupportedException:
        '     The stream does not support both writing and seeking.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     Attempted to set the value parameter to less than 0.
        ''' <summary>
        ''' Sets the length of this stream to the given value.
        ''' </summary>
        ''' <param name="value">The new length of the stream.</param>
        <SecuritySafeCritical>
        Public Overrides Sub SetLength(value As Long)
            Dim args = Protocols.GetSetLength(value, FileHandle)
            Dim invoke As New AsynInvoke(FileSystem)
            Dim rep As RequestStream = invoke.SendMessage(args)
        End Sub

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     array is null.
        '
        '   T:System.ArgumentException:
        '     offset and count describe an invalid range in array.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     offset or count is negative.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurred. - or -Another thread may have caused an unexpected change
        '     in the position of the operating system's file handle.
        '
        '   T:System.ObjectDisposedException:
        '     The stream is closed.
        '
        '   T:System.NotSupportedException:
        '     The current stream instance does not support writing.
        ''' <summary>
        ''' Writes a block of bytes to the file stream.
        ''' </summary>
        ''' <param name="array">The buffer containing data to write to the stream.</param>
        ''' <param name="offset">The zero-based byte offset in array from which to begin copying bytes to the
        ''' stream.</param>
        ''' <param name="count">The maximum number of bytes to write.</param>
        <SecuritySafeCritical>
        Public Overrides Sub Write(array() As Byte, offset As Integer, count As Integer)
            Dim args As WriteStream = New WriteStream With {
                .Handle = FileHandle,
                .buffer = array,
                .length = count,
                .offset = offset
            }
            Dim req As New RequestStream(ProtocolEntry, FileSystemAPI.WriteBuffer, args)
            Dim invoke As New AsynInvoke(FileSystem)
            Call invoke.SendMessage(req)
        End Sub

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream is closed.
        '
        '   T:System.NotSupportedException:
        '     The stream does not support writing.
        ''' <summary>
        ''' Writes a byte to the current position in the file stream.
        ''' </summary>
        ''' <param name="value">A byte to write to the stream.</param>
        <SecuritySafeCritical> Public Overrides Sub WriteByte(value As Byte)
            Call Write({value}, Scan0, 1)
        End Sub

        ''' <summary>
        ''' Releases the unmanaged resources used by the System.IO.FileStream and optionally
        ''' releases the managed resources.
        ''' </summary>
        ''' <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged
        ''' resources.</param>
        <SecuritySafeCritical>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Dim req As RequestStream = RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.CloseHandle, FileHandle)
            Dim invoke As New AsynInvoke(FileSystem)
            Call invoke.SendMessage(req)
        End Sub

        ''' <summary>
        ''' Ensures that resources are freed and other cleanup operations are performed when
        ''' the garbage collector reclaims the FileStream.
        ''' </summary>
        <SecuritySafeCritical> Protected Overrides Sub Finalize()
            ' DO NOTHING
        End Sub

        ' Exceptions:
        '   T:System.ArgumentException:
        '     The array length minus offset is less than numBytes.
        '
        '   T:System.ArgumentNullException:
        '     array is null.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     offset or numBytes is negative.
        '
        '   T:System.IO.IOException:
        '     An asynchronous read was attempted past the end of the file.
        ''' <summary>
        ''' Begins an asynchronous read operation. (Consider using System.IO.FileStream.ReadAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)
        ''' instead; see the Remarks section.)
        ''' </summary>
        ''' <param name="array">The buffer to read data into.</param>
        ''' <param name="offset">The byte offset in array at which to begin reading.</param>
        ''' <param name="numBytes">The maximum number of bytes to read.</param>
        ''' <param name="userCallback">The method to be called when the asynchronous read operation is completed.</param>
        ''' <param name="stateObject">A user-provided object that distinguishes this particular asynchronous read request
        ''' from other requests.</param>
        ''' <returns>An object that references the asynchronous read.</returns>
        <SecuritySafeCritical>
        Public Overrides Function BeginRead(array() As Byte, offset As Integer, numBytes As Integer, userCallback As AsyncCallback, stateObject As Object) As IAsyncResult
            Return __readHandle.BeginInvoke(array, offset, numBytes, userCallback, stateObject)
        End Function

        ReadOnly __readHandle As Func(Of Byte(), Integer, Integer, Integer) = AddressOf Read

        ' Exceptions:
        '   T:System.ArgumentException:
        '     array length minus offset is less than numBytes.
        '
        '   T:System.ArgumentNullException:
        '     array is null.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     offset or numBytes is negative.
        '
        '   T:System.NotSupportedException:
        '     The stream does not support writing.
        '
        '   T:System.ObjectDisposedException:
        '     The stream is closed.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurred.
        ''' <summary>
        ''' Begins an asynchronous write operation. (Consider using System.IO.FileStream.WriteAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)
        ''' instead; see the Remarks section.)
        ''' </summary>
        ''' <param name="array">The buffer containing data to write to the current stream.</param>
        ''' <param name="offset">The zero-based byte offset in array at which to begin copying bytes to the current
        ''' stream.</param>
        ''' <param name="numBytes">The maximum number of bytes to write.</param>
        ''' <param name="userCallback">The method to be called when the asynchronous write operation is completed.</param>
        ''' <param name="stateObject">A user-provided object that distinguishes this particular asynchronous write
        ''' request from other requests.</param>
        ''' <returns>An object that references the asynchronous write.</returns>
        <SecuritySafeCritical> Public Overrides Function BeginWrite(array() As Byte,
                                                                    offset As Integer,
                                                                    numBytes As Integer,
                                                                    userCallback As AsyncCallback,
                                                                    stateObject As Object) As IAsyncResult
            Return __writeHandle.BeginInvoke(array, offset, numBytes, userCallback, stateObject)
        End Function

        ReadOnly __writeHandle As Action(Of Byte(), Integer, Integer) = AddressOf Write ' Sub() Call Write(array, offset, numBytes)

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     asyncResult is null.
        '
        '   T:System.ArgumentException:
        '     This System.IAsyncResult object was not created by calling System.IO.FileStream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        '     on this class.
        '
        '   T:System.InvalidOperationException:
        '     System.IO.FileStream.EndRead(System.IAsyncResult) is called multiple times.
        '
        '   T:System.IO.IOException:
        '     The stream is closed or an internal error has occurred.
        ''' <summary>
        ''' Waits for the pending asynchronous read operation to complete. (Consider using
        ''' System.IO.FileStream.ReadAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)
        ''' instead; see the Remarks section.)
        ''' </summary>
        ''' <param name="asyncResult">The reference to the pending asynchronous request to wait for.</param>
        ''' <returns>The number of bytes read from the stream, between 0 and the number of bytes you
        ''' requested. Streams only return 0 at the end of the stream, otherwise, they should
        ''' block until at least 1 byte is available.</returns>
        <SecuritySafeCritical> Public Overrides Function EndRead(asyncResult As IAsyncResult) As Integer
            Return __readHandle.EndInvoke(asyncResult)
        End Function

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream has been disposed.
        ''' <summary>
        ''' Asynchronously clears all buffers for this stream, causes any buffered data to
        ''' be written to the underlying device, and monitors cancellation requests.
        ''' </summary>
        ''' <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        ''' <returns>A task that represents the asynchronous flush operation.</returns>
        <ComVisible(False)> <SecuritySafeCritical>
        Public Overrides Function FlushAsync(cancellationToken As CancellationToken) As Task
            Return New Task(AddressOf Flush, cancellationToken)
        End Function

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     array is null.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     offset or count is negative.
        '
        '   T:System.NotSupportedException:
        '     The stream does not support reading.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurred.
        '
        '   T:System.ArgumentException:
        '     offset and count describe an invalid range in array.
        '
        '   T:System.ObjectDisposedException:
        '     Methods were called after the stream was closed.
        ''' <summary>
        ''' Reads a block of bytes from the stream and writes the data in a given buffer.
        ''' </summary>
        ''' <param name="array">
        ''' When this method returns, contains the specified byte array with the values between
        ''' offset and (offset + count - 1) replaced by the bytes read from the current source.
        ''' </param>
        ''' <param name="offset">The byte offset in array at which the read bytes will be placed.</param>
        ''' <param name="count">The maximum number of bytes to read.</param>
        ''' <returns>The total number of bytes read into the buffer. This might be less than the number
        ''' of bytes requested if that number of bytes are not currently available, or zero
        ''' if the end of the stream is reached.</returns>
        <SecuritySafeCritical>
        Public Overrides Function Read(array() As Byte, offset As Integer, count As Integer) As Integer
            Dim args As ReadBuffer = New ReadBuffer(Me.FileHandle) With {
                .length = count,
                .offset = offset
            }
            Dim req As RequestStream =
                New RequestStream(ProtocolEntry, FileSystemAPI.ReadBuffer, args.GetJson)
            Dim invoke As New AsynInvoke(FileSystem)
            Dim rep As RequestStream = invoke.SendMessage(req)
            Call System.Array.ConstrainedCopy(rep.ChunkBuffer, Scan0, array, offset, count)
            Return rep.Protocol ' 在host的协议里面会将读函数的返回值放在protocol属性里面返回
        End Function

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     buffer is null.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     offset or count is negative.
        '
        '   T:System.ArgumentException:
        '     The sum of offset and count is larger than the buffer length.
        '
        '   T:System.NotSupportedException:
        '     The stream does not support reading.
        '
        '   T:System.ObjectDisposedException:
        '     The stream has been disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream is currently in use by a previous read operation.
        ''' <summary>
        ''' Asynchronously reads a sequence of bytes from the current stream, advances the
        ''' position within the stream by the number of bytes read, and monitors cancellation
        ''' requests.
        ''' </summary>
        ''' <param name="buffer">The buffer to write the data into.</param>
        ''' <param name="offset">The byte offset in buffer at which to begin writing data from the stream.</param>
        ''' <param name="count">The maximum number of bytes to read.</param>
        ''' <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        ''' <returns>A task that represents the asynchronous read operation. The value of the TResult
        ''' parameter contains the total number of bytes read into the buffer. The result
        ''' value can be less than the number of bytes requested if the number of bytes currently
        ''' available is less than the requested number, or it can be 0 (zero) if the end
        ''' of the stream has been reached.</returns>
        <ComVisible(False)> <SecuritySafeCritical>
        Public Overrides Function ReadAsync(buffer() As Byte, offset As Integer, count As Integer, cancellationToken As CancellationToken) As Task(Of Integer)
            Return New Task(Of Integer)(Function() Read(buffer, offset, count), cancellationToken)
        End Function

        ' Exceptions:
        '   T:System.NotSupportedException:
        '     The current stream does not support reading.
        '
        '   T:System.ObjectDisposedException:
        '     The current stream is closed.
        ''' <summary>
        ''' Reads a byte from the file and advances the read position one byte.
        ''' </summary>
        ''' <returns>The byte, cast to an System.Int32, or -1 if the end of the stream has been reached.</returns>
        <SecuritySafeCritical> Public Overrides Function ReadByte() As Integer
            Dim buf As Byte() = New Byte(0) {}
            Dim i As Integer = Read(buf, Scan0, 1)
            If i = -1 Then
                Return -1
            Else
                Return CType(buf(Scan0), Integer)
            End If
        End Function

        ' Exceptions:
        '   T:System.IO.IOException:
        '     An I/O error occurred.
        '
        '   T:System.NotSupportedException:
        '     The stream does not support seeking, such as if the FileStream is constructed
        '     from a pipe or console output.
        '
        '   T:System.ArgumentException:
        '     Seeking is attempted before the beginning of the stream.
        '
        '   T:System.ObjectDisposedException:
        '     Methods were called after the stream was closed.
        ''' <summary>
        ''' Sets the current position of this stream to the given value.
        ''' </summary>
        ''' <param name="offset">The point relative to origin from which to begin seeking.</param>
        ''' <param name="origin">Specifies the beginning, the end, or the current position as a reference point
        ''' for offset, using a value of type System.IO.SeekOrigin.</param>
        ''' <returns>The new position in the stream.</returns>
        <SecuritySafeCritical> Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
            Dim args As New SeekArgs(FileHandle) With {
                .offset = offset,
                .origin = origin
            }
            Dim invoke As New AsynInvoke(FileSystem)
            Dim req As RequestStream = RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.StreamSeek, args)
            Dim rep As RequestStream = invoke.SendMessage(req)
            Dim l As Long = BitConverter.ToInt64(rep.ChunkBuffer, Scan0)
            Return l
        End Function

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     buffer is null.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     offset or count is negative.
        '
        '   T:System.ArgumentException:
        '     The sum of offset and count is larger than the buffer length.
        '
        '   T:System.NotSupportedException:
        '     The stream does not support writing.
        '
        '   T:System.ObjectDisposedException:
        '     The stream has been disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream is currently in use by a previous write operation.
        ''' <summary>
        ''' Asynchronously writes a sequence of bytes to the current stream, advances the
        ''' current position within this stream by the number of bytes written, and monitors
        ''' cancellation requests.
        ''' </summary>
        ''' <param name="buffer">The buffer to write data from.</param>
        ''' <param name="offset">The zero-based byte offset in buffer from which to begin copying bytes to the
        ''' stream.</param>
        ''' <param name="count">The maximum number of bytes to write.</param>
        ''' <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)> <SecuritySafeCritical>
        Public Overrides Function WriteAsync(buffer() As Byte, offset As Integer, count As Integer, cancellationToken As CancellationToken) As Task
            Return New Task(Sub() Call Write(buffer, offset, count), cancellationToken)
        End Function
    End Class
End Namespace
