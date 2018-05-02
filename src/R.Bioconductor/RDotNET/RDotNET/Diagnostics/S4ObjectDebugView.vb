Imports System.Diagnostics
Imports System.Linq

Namespace Diagnostics
	Friend Class S4ObjectDebugView
		Private ReadOnly s4obj As S4Object

		Public Sub New(obj As S4Object)
			Me.s4obj = obj
		End Sub

		<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)> _
		Public ReadOnly Property Slots() As S4ObjectSlotDisplay()
			Get
				Return Me.s4obj.SlotNames.AsEnumerable().[Select](Function(name) New S4ObjectSlotDisplay(Me.s4obj, name)).ToArray()
			End Get
		End Property
	End Class
End Namespace
