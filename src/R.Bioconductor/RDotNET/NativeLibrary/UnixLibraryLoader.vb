#Region "Microsoft.VisualBasic::7d254dfa1a5cf18eb97b7126c980dbc9, RDotNET\NativeLibrary\UnixLibraryLoader.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class UnixLibraryLoader
    ' 
    '         Function: dlclose, dlerror, dlopen, dlsym, FreeLibrary
    '                   GetFunctionAddress, GetLastError, InternalLoadLibrary, LoadLibrary
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
