Imports System.Runtime.InteropServices

Namespace Internals
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure sxpinfo
		Private bits As UInteger

		Public ReadOnly Property type() As SymbolicExpressionType
			Get
				Return CType(Me.bits And 31UI, SymbolicExpressionType)
			End Get
		End Property

		Public ReadOnly Property obj() As UInteger
			Get
				Return ((Me.bits And 32UI) \ 32)
			End Get
		End Property

		Public ReadOnly Property named() As UInteger
			Get
				Return ((Me.bits And 192UI) \ 64)
			End Get
		End Property

		Public ReadOnly Property gp() As UInteger
			Get
				Return ((Me.bits And 16776960UI) \ 256)
			End Get
		End Property

		Public ReadOnly Property mark() As UInteger
			Get
				Return ((Me.bits And 16777216UI) \ 16777216)
			End Get
		End Property

		Public ReadOnly Property debug() As UInteger
			Get
				Return ((Me.bits And 33554432UI) \ 33554432)
			End Get
		End Property

		Public ReadOnly Property trace() As UInteger
			Get
				Return ((Me.bits And 67108864UI) \ 67108864)
			End Get
		End Property

		Public ReadOnly Property spare() As UInteger
			Get
				Return ((Me.bits And 134217728UI) \ 134217728)
			End Get
		End Property

		Public ReadOnly Property gcgen() As UInteger
			Get
				Return ((Me.bits And 268435456UI) \ 268435456)
			End Get
		End Property

		Public ReadOnly Property gccls() As UInteger
			Get
				Return ((Me.bits And 3758096384UI) \ 536870912)
			End Get
		End Property
	End Structure
End Namespace
