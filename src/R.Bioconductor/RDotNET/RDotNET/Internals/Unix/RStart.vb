#Region "Microsoft.VisualBasic::99494830691a12a6ce27c06792b75650, RDotNET\RDotNET\Internals\Unix\RStart.vb"

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

Namespace Internals.Unix
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure RStart
		<MarshalAs(UnmanagedType.Bool)> _
		Public R_Quiet As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public R_Slave As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public R_Interactive As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public R_Verbose As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public LoadSiteFile As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public LoadInitFile As Boolean

		<MarshalAs(UnmanagedType.Bool)> _
		Public DebugInitFile As Boolean

		Public RestoreAction As StartupRestoreAction
		Public SaveAction As StartupSaveAction
		Friend vsize As UIntPtr
		Friend nsize As UIntPtr
		Friend max_vsize As UIntPtr
		Friend max_nsize As UIntPtr
		Friend ppsize As UIntPtr

		<MarshalAs(UnmanagedType.Bool)> _
		Public NoRenviron As Boolean
	End Structure
End Namespace

