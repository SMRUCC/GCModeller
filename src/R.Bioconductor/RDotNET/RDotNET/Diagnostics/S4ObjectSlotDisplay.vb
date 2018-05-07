Imports System.Diagnostics
Imports System.Linq

Namespace Diagnostics
	<DebuggerDisplay("{Display,nq}")> _
	Friend Class S4ObjectSlotDisplay
		<DebuggerBrowsable(DebuggerBrowsableState.Never)> _
		Private ReadOnly s4obj As S4Object

		<DebuggerBrowsable(DebuggerBrowsableState.Never)> _
		Private ReadOnly name As String

		Public Sub New(obj As S4Object, name As String)
			Me.s4obj = obj
			Me.name = name
		End Sub

		<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)> _
		Public ReadOnly Property Value() As SymbolicExpression
			Get
				Return s4obj(name)
			End Get
		End Property

		<DebuggerBrowsable(DebuggerBrowsableState.Never)> _
		Public ReadOnly Property Display() As String
			Get
				Dim slot = Me.Value
				Dim names = Me.s4obj.SlotNames
				If names Is Nothing OrElse Not names.Contains(name) Then
					Return [String].Format("NA ({0})", slot.Type)
				Else
					Return [String].Format("""{0}"" ({1})", name, slot.Type)
				End If
			End Get
		End Property
	End Class
End Namespace
