Imports System
Imports System.ComponentModel
Imports System.Runtime.ConstrainedExecution
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text

Namespace ApplicationServices.DynamicInterop
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
    Friend Class WindowsLibraryLoader
        Implements IDynamicLibraryLoader

        Public Function LoadLibrary(ByVal filename As String) As IntPtr Implements IDynamicLibraryLoader.LoadLibrary
            'new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            Dim handle = Win32.LoadLibrary(filename)

            If handle = IntPtr.Zero Then
                Dim [error] = New Win32Exception(Marshal.GetLastWin32Error()).Message
                Console.WriteLine([error])
            End If

            Return handle
        End Function

        Public Function GetLastError() As String Implements IDynamicLibraryLoader.GetLastError
            ' see for instance http://blogs.msdn.com/b/shawnfa/archive/2004/09/10/227995.aspx 
            ' and http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
            ' TODO: does this work as expected with Mono+Windows stack?
            Return New Win32Exception().Message
        End Function

        Public Function FreeLibrary(ByVal handle As IntPtr) As Boolean Implements IDynamicLibraryLoader.FreeLibrary
            Return Win32.FreeLibrary(handle)
        End Function

        Public Function GetFunctionAddress(ByVal hModule As IntPtr, ByVal lpProcName As String) As IntPtr Implements IDynamicLibraryLoader.GetFunctionAddress
            Return GetProcAddress(hModule, lpProcName)
        End Function

        Public Shared Function GetShortPath(ByVal path As String) As String
            Dim shortPath = New StringBuilder(MaxPathLength)
            GetShortPathName(path, shortPath, MaxPathLength)
            Return shortPath.ToString()
        End Function
    End Class

    Friend Module Win32
        <DllImport("kernel32.dll", SetLastError:=True)>
        Public Function LoadLibrary(
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String) As IntPtr
        End Function

        <DllImport("kernel32.dll")>
        <ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)>
        <MarshalAs(UnmanagedType.Bool)>
        Public Function FreeLibrary(ByVal hModule As IntPtr) As Boolean
        End Function

        <DllImport("kernel32.dll")>
        Public Function GetProcAddress(ByVal hModule As IntPtr,
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpProcName As String) As IntPtr
        End Function

        Public Const MaxPathLength As Integer = 248 'MaxPath is 248. MaxFileName is 260.

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto)>
        Public Function GetShortPathName(
        <MarshalAs(UnmanagedType.LPTStr)> ByVal path As String,
        <MarshalAs(UnmanagedType.LPTStr)> ByVal shortPath As StringBuilder, ByVal shortPathLength As Integer) As Integer
        End Function
    End Module
End Namespace
