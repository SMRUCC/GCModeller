#Region "Microsoft.VisualBasic::8071c629305eb34be5fcebc148def3e0, ..\R.Bioconductor\RDotNET\R.NET\Diagnostics\DataFrameDebugView.vb"

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
Imports System.Linq

Namespace Diagnostics
	Friend Class DataFrameDebugView
		Private ReadOnly dataFrame As DataFrame

		Public Sub New(dataFrame As DataFrame)
			Me.dataFrame = dataFrame
		End Sub

		<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)> _
		Public ReadOnly Property Column() As DataFrameColumnDisplay()
			Get
				Return Enumerable.Range(0, Me.dataFrame.ColumnCount).[Select](Function(column__1) New DataFrameColumnDisplay(Me.dataFrame, column__1)).ToArray()
			End Get
		End Property
	End Class
End Namespace
