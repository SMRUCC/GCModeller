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
