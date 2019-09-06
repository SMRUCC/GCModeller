#Region "Microsoft.VisualBasic::6439cb894f4546bb63ef863745042262, RDotNET\RDotNET\Internals\SEXPREC.vb"

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

    ' 	Structure SEXPREC
    ' 
    ' 	    Properties: attrib, closxp, envsxp, gengc_next_node, gengc_prev_node
    '                  listsxp, primsxp, promsxp, sxpinfo, symsxp
    ' 
    ' 	Structure u
    ' 
    ' 
    ' 
    ' 	Structure vecsxp
    ' 
    ' 
    ' 
    ' 	Structure primsxp
    ' 
    ' 
    ' 
    ' 	Structure symsxp
    ' 
    ' 
    ' 
    ' 	Structure listsxp
    ' 
    ' 
    ' 
    ' 	Structure envsxp
    ' 
    ' 
    ' 
    ' 	Structure closxp
    ' 
    ' 
    ' 
    ' 	Structure promsxp
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices

Namespace Internals
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure SEXPREC
		Private header As SEXPREC_HEADER
		Private u As u

		Public ReadOnly Property sxpinfo() As sxpinfo
			Get
				Return Me.header.sxpinfo
			End Get
		End Property

		Public ReadOnly Property attrib() As IntPtr
			Get
				Return Me.header.attrib
			End Get
		End Property

		Public ReadOnly Property gengc_next_node() As IntPtr
			Get
				Return Me.header.gengc_next_node
			End Get
		End Property

		Public ReadOnly Property gengc_prev_node() As IntPtr
			Get
				Return Me.header.gengc_prev_node
			End Get
		End Property

		Friend ReadOnly Property primsxp() As primsxp
			Get
				Return Me.u.primsxp
			End Get
		End Property

		Friend ReadOnly Property symsxp() As symsxp
			Get
				Return Me.u.symsxp
			End Get
		End Property

		Friend ReadOnly Property listsxp() As listsxp
			Get
				Return Me.u.listsxp
			End Get
		End Property

		Friend ReadOnly Property envsxp() As envsxp
			Get
				Return Me.u.envsxp
			End Get
		End Property

		Friend ReadOnly Property closxp() As closxp
			Get
				Return Me.u.closxp
			End Get
		End Property

		Friend ReadOnly Property promsxp() As promsxp
			Get
				Return Me.u.promsxp
			End Get
		End Property
	End Structure

	<StructLayout(LayoutKind.Explicit)> _
	Friend Structure u
		<FieldOffset(0)> _
		Friend primsxp As primsxp

		<FieldOffset(0)> _
		Friend symsxp As symsxp

		<FieldOffset(0)> _
		Friend listsxp As listsxp

		<FieldOffset(0)> _
		Friend envsxp As envsxp

		<FieldOffset(0)> _
		Friend closxp As closxp

		<FieldOffset(0)> _
		Friend promsxp As promsxp
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure vecsxp
		Public length As Integer
		Public truelength As Integer
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure primsxp
		Public offset As Integer
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure symsxp
		Public pname As IntPtr
		Public value As IntPtr
		Public internal As IntPtr
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure listsxp
		Public carval As IntPtr
		Public cdrval As IntPtr
		Public tagval As IntPtr
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure envsxp
		Public frame As IntPtr
		Public enclos As IntPtr
		Public hashtab As IntPtr
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure closxp
		Public formals As IntPtr
		Public body As IntPtr
		Public env As IntPtr
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure promsxp
		Public value As IntPtr
		Public expr As IntPtr
		Public env As IntPtr
	End Structure
End Namespace

