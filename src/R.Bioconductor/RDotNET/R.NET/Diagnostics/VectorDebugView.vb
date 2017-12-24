#Region "Microsoft.VisualBasic::32599588353d107482699292d14fb5ef, ..\R.Bioconductor\RDotNET\R.NET\Diagnostics\VectorDebugView.vb"

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

Imports System.Diagnostics

Namespace Diagnostics
	Friend Class VectorDebugView(Of T)
		Private ReadOnly vector As Vector(Of T)

		Public Sub New(vector As Vector(Of T))
			Me.vector = vector
		End Sub

		<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)> _
		Public ReadOnly Property Value() As T()
			Get
				Dim array = New T(Me.vector.Length - 1) {}
				Me.vector.CopyTo(array, array.Length)
				Return array
			End Get
		End Property
	End Class
End Namespace
