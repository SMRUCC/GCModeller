#Region "Microsoft.VisualBasic::6153cac70b8cded4e46b8e3ce1297622, RDotNET\RDotNET\Internals\Windows\RStart.vb"

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

    ' 	Structure RStart
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

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

