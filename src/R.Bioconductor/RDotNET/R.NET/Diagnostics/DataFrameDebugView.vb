#Region "Microsoft.VisualBasic::48ddd9e9a0e52d63960cc0a4efec659e, RDotNET\R.NET\Diagnostics\DataFrameDebugView.vb"

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
    ' 	    Sub: New
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
				Return Enumerable.Range(0, Me.dataFrame.ColumnCount).[Select](Function(column__1) New DataFrameColumnDisplay(Me.dataFrame, column__1)).ToArray()
			End Get
		End Property
	End Class
End Namespace
