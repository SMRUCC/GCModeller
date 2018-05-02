Imports System.Runtime.InteropServices

Namespace Internals
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure SEXPREC_HEADER
		Public sxpinfo As sxpinfo
		Public attrib As IntPtr
		Public gengc_next_node As IntPtr
		Public gengc_prev_node As IntPtr
	End Structure
End Namespace
