Imports System.Diagnostics

Namespace Diagnostics
	Friend Class MatrixDebugView(Of T)
		Private ReadOnly matrix As Matrix(Of T)

		Public Sub New(matrix As Matrix(Of T))
			Me.matrix = matrix
		End Sub

		<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)> _
		Public ReadOnly Property Value() As T(,)
			Get
				Dim array = New T(Me.matrix.RowCount - 1, Me.matrix.ColumnCount - 1) {}
				Me.matrix.CopyTo(array, Me.matrix.RowCount, Me.matrix.ColumnCount)
				Return array
			End Get
		End Property
	End Class
End Namespace
