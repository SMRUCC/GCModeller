Imports System.Diagnostics

Namespace Diagnostics
    Friend Class MatrixDebugView(Of T)
        Private ReadOnly matrix As Matrix(Of T)

        Public Sub New(ByVal matrix As Matrix(Of T))
            Me.matrix = matrix
        End Sub

        <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
        Public ReadOnly Property Value As T(,)
            Get
                Dim array = New T(matrix.RowCount - 1, matrix.ColumnCount - 1) {}
                matrix.CopyTo(array, matrix.RowCount, matrix.ColumnCount)
                Return array
            End Get
        End Property
    End Class
End Namespace
