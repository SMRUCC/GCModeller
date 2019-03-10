Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.ConstrainedExecution
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text

Namespace NativeLibrary.DynamicInterop
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
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

        ''' <summary>
        ''' Unloads a library
        ''' </summary>
        ''' <param name="handle">The pointer resulting from loading the library</param>
        ''' <returns>True if the function dlclose returned 0</returns>
        Public Function FreeLibrary(handle As IntPtr) As Boolean Implements IDynamicLibraryLoader.FreeLibrary
            ' according to the manual page on a Debian box
            ' The function dlclose() returns 0 on success, and nonzero on error.
            Dim status = dlclose(handle)
            Return status = 0
        End Function

        Public Function GetFunctionAddress(hModule As IntPtr, lpProcName As String) As IntPtr Implements IDynamicLibraryLoader.GetFunctionAddress
            Return dlsym(hModule, lpProcName)
        End Function

        Friend Shared Function InternalLoadLibrary(filename As String) As IntPtr
            Const RTLD_LAZY As Integer = &H1
            '            if (filename.StartsWith ("/")) {
            '                return dlopen (filename, RTLD_LAZY);
            '            }
            '            var searchPaths = getSearchPaths ("LD_LIBRARY_PATH");
            '            searchPaths.AddRange (getSearchPaths ("PATH"));
            '            var dll = searchPaths.Select (directory => Path.Combine (directory, filename)).FirstOrDefault (File.Exists);
            '            if (dll == null) {
            '                throw new DllNotFoundException ("Could not find the file: " + filename + " on the search path.  Checked these directories:\n "
            '                + String.Join ("\n", searchPaths));
            '            }

            Dim result = dlopen(filename, RTLD_LAZY)
            Return result

        End Function

        Private Shared Function getSearchPaths(pathsEnvVar As String) As List(Of String)
            Dim searchPaths = (If(Environment.GetEnvironmentVariable(pathsEnvVar), "")).Split(Path.PathSeparator).ToList()
            Return searchPaths
        End Function

        <DllImport("libdl")>
        Private Shared Function dlopen(<MarshalAs(UnmanagedType.LPStr)> filename As String, flag As Integer) As IntPtr
        End Function

        <DllImport("libdl")>
        Private Shared Function dlerror() As <MarshalAs(UnmanagedType.LPStr)> String
        End Function

        <DllImport("libdl", EntryPoint:="dlclose")>
        <ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)>
        Private Shared Function dlclose(hModule As IntPtr) As Integer
        End Function

        <DllImport("libdl", EntryPoint:="dlsym")>
        Private Shared Function dlsym(hModule As IntPtr, <MarshalAs(UnmanagedType.LPStr)> lpProcName As String) As IntPtr
        End Function
    End Class
End Namespace
