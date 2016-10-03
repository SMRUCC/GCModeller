#Region "Microsoft.VisualBasic::397347ecc56464d85d048f6a3134f8d1, ..\R.Bioconductor\RDotNET\R.NET\Diagnostics\FactorDebugView.vb"

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

Imports System.Diagnostics
Imports System.Linq

Namespace Diagnostics
	Friend Class FactorDebugView
		Private ReadOnly factor As Factor

		Public Sub New(factor As Factor)
			Me.factor = factor
		End Sub

		<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)> _
		Public ReadOnly Property Value() As String()
			Get
				Return Me.factor.GetFactors().ToArray()
			End Get
		End Property
	End Class
End Namespace
