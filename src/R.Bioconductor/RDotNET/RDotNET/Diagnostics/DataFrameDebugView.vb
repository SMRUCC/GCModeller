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
