#Region "Microsoft.VisualBasic::3116933f314cb7d83323498f5442697a, RDotNET\RDotNET\Diagnostics\DataFrameDebugView.vb"

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

    ' 	Class DataFrameDebugView
    ' 
    ' 	    Properties: Column
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

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
                Return Enumerable.Range(0, Me.dataFrame.ColumnCount).[Select](Function(col) New DataFrameColumnDisplay(Me.dataFrame, col)).ToArray()
            End Get
		End Property
	End Class
End Namespace

