Imports System.Runtime.InteropServices

Namespace Internals
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure SEXPREC
		Private header As SEXPREC_HEADER
		Private u As u

		Public ReadOnly Property sxpinfo() As sxpinfo
			Get
				Return Me.header.sxpinfo
			End Get
		End Property

		Public ReadOnly Property attrib() As IntPtr
			Get
				Return Me.header.attrib
			End Get
		End Property

		Public ReadOnly Property gengc_next_node() As IntPtr
			Get
				Return Me.header.gengc_next_node
			End Get
		End Property

		Public ReadOnly Property gengc_prev_node() As IntPtr
			Get
				Return Me.header.gengc_prev_node
			End Get
		End Property

		Friend ReadOnly Property primsxp() As primsxp
			Get
				Return Me.u.primsxp
			End Get
		End Property

		Friend ReadOnly Property symsxp() As symsxp
			Get
				Return Me.u.symsxp
			End Get
		End Property

		Friend ReadOnly Property listsxp() As listsxp
			Get
				Return Me.u.listsxp
			End Get
		End Property

		Friend ReadOnly Property envsxp() As envsxp
			Get
				Return Me.u.envsxp
			End Get
		End Property

		Friend ReadOnly Property closxp() As closxp
			Get
				Return Me.u.closxp
			End Get
		End Property

		Friend ReadOnly Property promsxp() As promsxp
			Get
				Return Me.u.promsxp
			End Get
		End Property
	End Structure

	<StructLayout(LayoutKind.Explicit)> _
	Friend Structure u
		<FieldOffset(0)> _
		Friend primsxp As primsxp

		<FieldOffset(0)> _
		Friend symsxp As symsxp

		<FieldOffset(0)> _
		Friend listsxp As listsxp

		<FieldOffset(0)> _
		Friend envsxp As envsxp

		<FieldOffset(0)> _
		Friend closxp As closxp

		<FieldOffset(0)> _
		Friend promsxp As promsxp
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure vecsxp
		Public length As Integer
		Public truelength As Integer
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure primsxp
		Public offset As Integer
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure symsxp
		Public pname As IntPtr
		Public value As IntPtr
		Public internal As IntPtr
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure listsxp
		Public carval As IntPtr
		Public cdrval As IntPtr
		Public tagval As IntPtr
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure envsxp
		Public frame As IntPtr
		Public enclos As IntPtr
		Public hashtab As IntPtr
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure closxp
		Public formals As IntPtr
		Public body As IntPtr
		Public env As IntPtr
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure promsxp
		Public value As IntPtr
		Public expr As IntPtr
		Public env As IntPtr
	End Structure
End Namespace
