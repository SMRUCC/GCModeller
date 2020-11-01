Imports System
Imports System.Runtime.InteropServices

Namespace Internals.PreALTREP
    <StructLayout(LayoutKind.Sequential)>
    Friend Structure SEXPREC_HEADER
        Public sxpinfo As sxpinfo
        Public attrib As IntPtr
        Public gengc_next_node As IntPtr
        Public gengc_prev_node As IntPtr
    End Structure
End Namespace
