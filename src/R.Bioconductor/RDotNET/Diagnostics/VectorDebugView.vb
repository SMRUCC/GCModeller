Imports System.Diagnostics

Namespace Diagnostics
    Friend Class VectorDebugView(Of T)
        Private ReadOnly vector As Vector(Of T)

        Public Sub New(ByVal vector As Vector(Of T))
            Me.vector = vector
        End Sub

        <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
        Public ReadOnly Property Value As T()
            Get
                Dim array = New T(vector.Length - 1) {}
                vector.CopyTo(array, array.Length)
                Return array
            End Get
        End Property
    End Class
End Namespace
