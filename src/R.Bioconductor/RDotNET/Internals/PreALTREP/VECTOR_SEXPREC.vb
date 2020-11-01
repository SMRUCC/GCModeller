Imports System
Imports System.Runtime.InteropServices

Namespace Internals.PreALTREP
    ' In R 3.5, the length & true length values went from pure 32-bit int to platform-dependent pointer length (32 or 64 bits in length).
    ' These are defined in R as R_xlen_t (previously R_len_t) - https://github.com/wch/r-source/blob/trunk/src/include/Rinternals.h
    ' Here we use the .NET equivalent - IntPtr.

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure VECTOR_SEXPREC
        Private header As SEXPREC_HEADER
        Private vecsxp As vecsxp

        Public ReadOnly Property sxpinfo As sxpinfo
            Get
                Return header.sxpinfo
            End Get
        End Property

        Public ReadOnly Property attrib As IntPtr
            Get
                Return header.attrib
            End Get
        End Property

        Public ReadOnly Property gengc_next_node As IntPtr
            Get
                Return header.gengc_next_node
            End Get
        End Property

        Public ReadOnly Property gengc_prev_node As IntPtr
            Get
                Return header.gengc_prev_node
            End Get
        End Property

        Public ReadOnly Property Length As Long
            Get
                Return vecsxp.length
            End Get
        End Property

        Public ReadOnly Property TrueLength As Long
            Get
                Return vecsxp.truelength
            End Get
        End Property
    End Structure
End Namespace
