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
