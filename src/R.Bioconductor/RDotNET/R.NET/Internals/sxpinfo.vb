#Region "Microsoft.VisualBasic::d328606d5b20683cd2327a045455190d, ..\R.Bioconductor\RDotNET\R.NET\Internals\sxpinfo.vb"

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

Namespace Internals
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure sxpinfo
		Private bits As UInteger

		Public ReadOnly Property type() As SymbolicExpressionType
			Get
				Return CType(Me.bits And 31UI, SymbolicExpressionType)
			End Get
		End Property

		Public ReadOnly Property obj() As UInteger
			Get
				Return ((Me.bits And 32UI) \ 32)
			End Get
		End Property

		Public ReadOnly Property named() As UInteger
			Get
				Return ((Me.bits And 192UI) \ 64)
			End Get
		End Property

		Public ReadOnly Property gp() As UInteger
			Get
				Return ((Me.bits And 16776960UI) \ 256)
			End Get
		End Property

		Public ReadOnly Property mark() As UInteger
			Get
				Return ((Me.bits And 16777216UI) \ 16777216)
			End Get
		End Property

		Public ReadOnly Property debug() As UInteger
			Get
				Return ((Me.bits And 33554432UI) \ 33554432)
			End Get
		End Property

		Public ReadOnly Property trace() As UInteger
			Get
				Return ((Me.bits And 67108864UI) \ 67108864)
			End Get
		End Property

		Public ReadOnly Property spare() As UInteger
			Get
				Return ((Me.bits And 134217728UI) \ 134217728)
			End Get
		End Property

		Public ReadOnly Property gcgen() As UInteger
			Get
				Return ((Me.bits And 268435456UI) \ 268435456)
			End Get
		End Property

		Public ReadOnly Property gccls() As UInteger
			Get
				Return ((Me.bits And 3758096384UI) \ 536870912)
			End Get
		End Property
	End Structure
End Namespace
