Imports System
Imports System.Runtime.InteropServices

Namespace Internals.ALTREP
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Friend Structure SEXPREC_HEADER
        Public sxpinfo As sxpinfo
        Public attrib As IntPtr
        Public gengc_next_node As IntPtr
        Public gengc_prev_node As IntPtr
    End Structure
End Namespace
