#Region "Microsoft.VisualBasic::b855ea8d44d5a5f1be158d2a0f14fe01, ..\R.Bioconductor\RDotNET\R.NET\Internals\Windows\Delegates.vb"

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
Imports System.Text

Namespace Internals.Windows
	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function blah1(<[In]> <MarshalAs(UnmanagedType.LPStr)> prompt As String, <MarshalAs(UnmanagedType.LPStr)> buffer As StringBuilder, length As Integer, <MarshalAs(UnmanagedType.Bool)> history As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub blah2(<[In]> <MarshalAs(UnmanagedType.LPStr)> buffer As String, length As Integer)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub blah3()

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub blah4(<[In]> <MarshalAs(UnmanagedType.LPStr)> message As String)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function blah5(<[In]> <MarshalAs(UnmanagedType.LPStr)> question As String) As YesNoCancel

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub blah6(which As BusyType)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub blah7(<[In]> <MarshalAs(UnmanagedType.LPStr)> buffer As String, length As Integer, outputType As ConsoleOutputType)
End Namespace

