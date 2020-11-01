Imports System
Imports System.Runtime.InteropServices

Namespace Internals.ALTREP
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Friend Structure SEXPREC
        Private header As SEXPREC_HEADER
        Private u As u

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

        Friend ReadOnly Property primsxp As primsxp
            Get
                Return u.primsxp
            End Get
        End Property

        Friend ReadOnly Property symsxp As symsxp
            Get
                Return u.symsxp
            End Get
        End Property

        Friend ReadOnly Property listsxp As listsxp
            Get
                Return u.listsxp
            End Get
        End Property

        Friend ReadOnly Property envsxp As envsxp
            Get
                Return u.envsxp
            End Get
        End Property

        Friend ReadOnly Property closxp As closxp
            Get
                Return u.closxp
            End Get
        End Property

        Friend ReadOnly Property promsxp As promsxp
            Get
                Return u.promsxp
            End Get
        End Property
    End Structure

    <StructLayout(LayoutKind.Explicit, Pack:=1)>
    Friend Structure u
        <FieldOffset(0)>
        Friend primsxp As primsxp
        <FieldOffset(0)>
        Friend symsxp As symsxp
        <FieldOffset(0)>
        Friend listsxp As listsxp
        <FieldOffset(0)>
        Friend envsxp As envsxp
        <FieldOffset(0)>
        Friend closxp As closxp
        <FieldOffset(0)>
        Friend promsxp As promsxp
    End Structure

    ' In R 3.5, the length & true length values went from pure 32-bit int to platform-dependent pointer length (32 or 64 bits in length).
    ' These are defined in R as R_xlen_t (previously R_len_t) - https://github.com/wch/r-source/blob/trunk/src/include/Rinternals.h
    ' Here we use the .NET equivalent - IntPtr.
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Friend Structure vecsxp
        Public length As IntPtr
        Public truelength As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Friend Structure primsxp
        Public offset As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Friend Structure symsxp
        Public pname As IntPtr
        Public value As IntPtr
        Public internal As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Friend Structure listsxp
        Public carval As IntPtr
        Public cdrval As IntPtr
        Public tagval As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Friend Structure envsxp
        Public frame As IntPtr
        Public enclos As IntPtr
        Public hashtab As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Friend Structure closxp
        Public formals As IntPtr
        Public body As IntPtr
        Public env As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Friend Structure promsxp
        Public value As IntPtr
        Public expr As IntPtr
        Public env As IntPtr
    End Structure
End Namespace
