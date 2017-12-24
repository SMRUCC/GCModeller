#Region "Microsoft.VisualBasic::440e1a62fd5ef8538159020fe202dfab, ..\R.Bioconductor\RDotNET\NativeLibrary\WindowsLibraryLoader.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports System.Runtime.ConstrainedExecution
Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.Security.Permissions
Imports System.Text

Namespace NativeLibrary

    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
    Friend Class WindowsLibraryLoader
        Implements IDynamicLibraryLoader
        Public Function LoadLibrary(filename As String) As IntPtr Implements IDynamicLibraryLoader.LoadLibrary
            Return InternalLoadLibrary(filename)
        End Function

        Public Function GetLastError() As String Implements IDynamicLibraryLoader.GetLastError
            ' see for instance http://blogs.msdn.com/b/shawnfa/archive/2004/09/10/227995.aspx 
            ' and http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
            ' TODO: does this work as expected with Mono+Windows stack?
            Return New Win32Exception().Message
        End Function

        Public Function FreeLibrary(handle As IntPtr) As Boolean Implements IDynamicLibraryLoader.FreeLibrary
            Return InternalFreeLibrary(handle)
        End Function

        Public Function GetFunctionAddress(hModule As IntPtr, lpProcName As String) As IntPtr Implements IDynamicLibraryLoader.GetFunctionAddress
            Return InternalGetProcAddress(hModule, lpProcName)
        End Function

        <DllImport("kernel32.dll", EntryPoint:="LoadLibrary", SetLastError:=True)> _
        Private Shared Function InternalLoadLibrary(<MarshalAs(UnmanagedType.LPStr)> lpFileName As String) As IntPtr
        End Function

        <DllImport("kernel32.dll", EntryPoint:="FreeLibrary")> _
        <ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)> _
        Private Shared Function InternalFreeLibrary(hModule As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        <DllImport("kernel32.dll", EntryPoint:="GetProcAddress")> _
        Private Shared Function InternalGetProcAddress(hModule As IntPtr, <MarshalAs(UnmanagedType.LPStr)> lpProcName As String) As IntPtr
        End Function

        <DllImport("kernel32.dll", EntryPoint:="GetLastError")> _
        Private Shared Function InternalGetLastError() As <MarshalAs(UnmanagedType.LPStr)> String
        End Function

        Const MAX_PATH_LENGTH As Integer = 255

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto)> _
        Private Shared Function GetShortPathName(<MarshalAs(UnmanagedType.LPTStr)> path As String, <MarshalAs(UnmanagedType.LPTStr)> shortPath As StringBuilder, shortPathLength As Integer) As Integer
        End Function

        ''' <summary>
        ''' Gets the old style DOS short path (8.3 format) given a path name
        ''' </summary>
        ''' <param name="path">A path</param>
        ''' <returns>The short path name according to the Windows kernel32 API</returns>
        Protected Friend Shared Function GetShortPath(path As String) As String
            Dim shortPath = New StringBuilder(MAX_PATH_LENGTH)
            GetShortPathName(path, shortPath, MAX_PATH_LENGTH)
            Return shortPath.ToString()
        End Function


    End Class
End Namespace
