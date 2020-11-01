Imports System.Diagnostics
Imports System.Linq

Namespace Diagnostics
    Friend Class DataFrameDebugView
        Private ReadOnly dataFrame As DataFrame

        Public Sub New(ByVal dataFrame As DataFrame)
            Me.dataFrame = dataFrame
        End Sub

        <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
        Public ReadOnly Property Column As DataFrameColumnDisplay()
            Get
                Return Enumerable.Range(0, dataFrame.ColumnCount).[Select](Function(i) New DataFrameColumnDisplay(dataFrame, i)).ToArray()
            End Get
        End Property
    End Class
End Namespace
