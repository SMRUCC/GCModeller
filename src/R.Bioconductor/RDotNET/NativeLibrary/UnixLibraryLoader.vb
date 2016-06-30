Imports System.Runtime.ConstrainedExecution
Imports System.Runtime.InteropServices
Imports System.Linq
Imports System.IO
Imports System.Security.Permissions

Namespace NativeLibrary

    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
    Friend Class UnixLibraryLoader
        Implements IDynamicLibraryLoader
        Public Function LoadLibrary(filename As String) As IntPtr Implements IDynamicLibraryLoader.LoadLibrary
            Return InternalLoadLibrary(filename)
        End Function

        ''' <summary>
        ''' Gets the last error. NOTE: according to http://tldp.org/HOWTO/Program-Library-HOWTO/dl-libraries.html, returns NULL if called more than once after dlopen.
        ''' </summary>
        ''' <returns>The last error.</returns>
        Public Function GetLastError() As String Implements IDynamicLibraryLoader.GetLastError
            Return dlerror()
        End Function

        Public Function FreeLibrary(hModule As IntPtr) As Boolean Implements IDynamicLibraryLoader.FreeLibrary
            ' according to the manual page on a Debian box
            ' The function dlclose() returns 0 on success, and nonzero on error.
            Dim status = dlclose(hModule)
            Return status = 0
        End Function

        Public Function GetFunctionAddress(hModule As IntPtr, lpProcName As String) As IntPtr Implements IDynamicLibraryLoader.GetFunctionAddress
            Return dlsym(hModule, lpProcName)
        End Function

        Friend Shared Function InternalLoadLibrary(filename As String) As IntPtr
            Const RTLD_LAZY As Integer = &H1
            If filename.StartsWith("/") Then
                Return dlopen(filename, RTLD_LAZY)
            End If
            Dim searchPaths As String() = (If(Environment.GetEnvironmentVariable("PATH"), "")).Split(Path.PathSeparator)
            Dim dll = searchPaths.[Select](Function(directory) Path.Combine(directory, filename)).FirstOrDefault(AddressOf File.Exists)
            If dll Is Nothing Then
                Throw New DllNotFoundException("Could not find the file: " & filename & " on the search path.  Checked these directories:" & vbLf & " " & [String].Join(vbLf, searchPaths))
            End If
            Return dlopen(dll, RTLD_LAZY)
        End Function

        <DllImport("libdl")> _
        Private Shared Function dlopen(<MarshalAs(UnmanagedType.LPStr)> filename As String, flag As Integer) As IntPtr
        End Function

        <DllImport("libdl")> _
        Private Shared Function dlerror() As <MarshalAs(UnmanagedType.LPStr)> String
        End Function

        '[return: MarshalAs(UnmanagedType.u)]
        <DllImport("libdl", EntryPoint:="dlclose")> _
        <ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)> _
        Private Shared Function dlclose(hModule As IntPtr) As Integer
        End Function

        <DllImport("libdl", EntryPoint:="dlsym")> _
        Private Shared Function dlsym(hModule As IntPtr, <MarshalAs(UnmanagedType.LPStr)> lpProcName As String) As IntPtr
        End Function
    End Class
End Namespace