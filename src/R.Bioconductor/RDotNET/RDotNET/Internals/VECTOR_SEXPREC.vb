#Region "Microsoft.VisualBasic::0096c2aa8c2698ca5749eab1d69f6784, RDotNET\RDotNET\Internals\VECTOR_SEXPREC.vb"

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

    ' 	Structure VECTOR_SEXPREC
    ' 
    ' 	    Properties: attrib, gengc_next_node, gengc_prev_node, Length, sxpinfo
    '                  TrueLength
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices

Namespace Internals
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure VECTOR_SEXPREC
		Private header As SEXPREC_HEADER
		Private vecsxp As vecsxp

		Public ReadOnly Property sxpinfo() As sxpinfo
			Get
				Return header.sxpinfo
			End Get
		End Property

		Public ReadOnly Property attrib() As IntPtr
			Get
				Return header.attrib
			End Get
		End Property

		Public ReadOnly Property gengc_next_node() As IntPtr
			Get
				Return header.gengc_next_node
			End Get
		End Property

		Public ReadOnly Property gengc_prev_node() As IntPtr
			Get
				Return header.gengc_prev_node
			End Get
		End Property

		Public ReadOnly Property Length() As Integer
			Get
				Return vecsxp.length
			End Get
		End Property

		Public ReadOnly Property TrueLength() As Integer
			Get
				Return vecsxp.truelength
			End Get
		End Property
	End Structure
End Namespace

