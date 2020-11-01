#Region "Microsoft.VisualBasic::12d48f1e89603d1c7d30a4f04268925c, RDotNET\RDotNET\Internals\Unix\Delegates.vb"

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

    ' 	Delegate Sub
    ' 
    ' 
    ' 	Delegate Sub
    ' 
    ' 
    ' 	Delegate Function
    ' 
    ' 
    ' 	Delegate Sub
    ' 
    ' 
    ' 	Delegate Sub
    ' 
    ' 
    ' 	Delegate Sub
    ' 
    ' 
    ' 	Delegate Sub
    ' 
    ' 
    ' 	Delegate Sub
    ' 
    ' 
    ' 	Delegate Sub
    ' 
    ' 
    ' 	Delegate Sub
    ' 
    ' 
    ' 	Delegate Function
    ' 
    ' 
    ' 	Delegate Function
    ' 
    ' 
    ' 	Delegate Sub
    ' 
    ' 
    ' 	Delegate Function
    ' 
    ' 
    ' 	Delegate Function
    ' 
    ' 
    ' 	Delegate Function
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports System.Text

Namespace Internals.Unix
	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub ptr_R_Suicide(<[In]> <MarshalAs(UnmanagedType.LPStr)> message As String)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub ptr_R_ShowMessage(<[In]> <MarshalAs(UnmanagedType.LPStr)> message As String)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function ptr_R_ReadConsole(<[In]> <MarshalAs(UnmanagedType.LPStr)> prompt As String, <MarshalAs(UnmanagedType.LPStr)> buffer As StringBuilder, length As Integer, <MarshalAs(UnmanagedType.Bool)> history As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub ptr_R_WriteConsole(<[In]> <MarshalAs(UnmanagedType.LPStr)> buffer As String, length As Integer)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub ptr_R_WriteConsoleEx(<[In]> <MarshalAs(UnmanagedType.LPStr)> buffer As String, length As Integer, outputType As ConsoleOutputType)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub ptr_R_ResetConsole()

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub ptr_R_FlushConsole()

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub ptr_R_ClearerrConsole()

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub ptr_R_Busy(which As BusyType)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub ptr_R_CleanUp(saveAction As StartupSaveAction, status As Integer, <MarshalAs(UnmanagedType.Bool)> runLast As Boolean)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function ptr_R_ShowFiles(count As Integer, <[In]> <MarshalAs(UnmanagedType.LPArray, ArraySubType := UnmanagedType.LPStr)> files As String(), <[In]> <MarshalAs(UnmanagedType.LPArray, ArraySubType := UnmanagedType.LPStr)> headers As String(), <[In]> <MarshalAs(UnmanagedType.LPStr)> title As String, <MarshalAs(UnmanagedType.Bool)> delete As Boolean, <[In]> <MarshalAs(UnmanagedType.LPStr)> pager As String) As <MarshalAs(UnmanagedType.Bool)> Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function ptr_R_ChooseFile(<MarshalAs(UnmanagedType.Bool)> create As Boolean, buffer As StringBuilder, length As Integer) As <MarshalAs(UnmanagedType.Bool)> Integer

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub ptr_R_EditFile(<MarshalAs(UnmanagedType.LPStr)> file As String)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function ptr_R_loadhistory([call] As IntPtr, operation As IntPtr, args As IntPtr, environment As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function ptr_R_savehistory([call] As IntPtr, operation As IntPtr, args As IntPtr, environment As IntPtr) As IntPtr

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function ptr_R_addhistory([call] As IntPtr, operation As IntPtr, args As IntPtr, environment As IntPtr) As IntPtr
End Namespace

