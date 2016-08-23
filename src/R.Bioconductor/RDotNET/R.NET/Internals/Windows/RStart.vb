#Region "Microsoft.VisualBasic::6ac3a7c013aeeb822b69eff68e3eef99, ..\R.Bioconductor\RDotNET\R.NET\Internals\Windows\RStart.vb"

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

Imports System.Runtime.InteropServices
Imports UnixRStruct = RDotNet.Internals.Unix.RStart

Namespace Internals.Windows
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure RStart
		Friend Common As UnixRStruct
		Public rhome As IntPtr
		Public home As IntPtr

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public ReadConsole As blah1

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public WriteConsole As blah2

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public CallBack As blah3

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public ShowMessage As blah4

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public YesNoCancel As blah5

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public Busy As blah6

		Public CharacterMode As UiMode

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public WriteConsoleEx As blah7
	End Structure
End Namespace

