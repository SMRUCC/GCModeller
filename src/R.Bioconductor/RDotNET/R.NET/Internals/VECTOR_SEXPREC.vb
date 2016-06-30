Imports System.Runtime.InteropServices

Namespace Internals
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure VECTOR_SEXPREC
		Private header As SEXPREC_HEADER
		Private vecsxp As vecsxp

		Public ReadOnly Property sxpinfo() As sxpinfo
			Get
				Return header.sxpinfo
			End Get
		End Property

		Public ReadOnly Property attrib() As IntPtr
			Get
				Return header.attrib
			End Get
		End Property

		Public ReadOnly Property gengc_next_node() As IntPtr
			Get
				Return header.gengc_next_node
			End Get
		End Property

		Public ReadOnly Property gengc_prev_node() As IntPtr
			Get
				Return header.gengc_prev_node
			End Get
		End Property

		Public ReadOnly Property Length() As Integer
			Get
				Return vecsxp.length
			End Get
		End Property

		Public ReadOnly Property TrueLength() As Integer
			Get
				Return vecsxp.truelength
			End Get
		End Property
	End Structure
End Namespace
