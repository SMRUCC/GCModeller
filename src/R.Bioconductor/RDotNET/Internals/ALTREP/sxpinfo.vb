Imports System.Runtime.InteropServices

Namespace Internals.ALTREP
    ' Definition of the struct available at: https://cran.r-project.org/doc/manuals/r-release/R-ints.html#Rest-of-header
    ' Formally defined in Rinternals.h: https://github.com/wch/r-source/blob/trunk/src/include/Rinternals.h
    ' Note that this structure was greatly changed in the R 3.5 release, using the platform-dependent pointer size (represented
    '   here as IntPtr), with fields added and the order changed.
    <StructLayout(LayoutKind.Sequential)>
    Friend Structure sxpinfo
        Private bits As ULong
        Private Const NAMED_BITS As Integer = 16

        Public ReadOnly Property type As SymbolicExpressionType  ' 5 bits
            Get
                Return bits And 31UI
            End Get
        End Property

        Public ReadOnly Property scalar As UInteger    ' 1 bit
            Get
                Return (bits And 32UI) / 32
            End Get
        End Property

        Public ReadOnly Property obj As UInteger   ' 1 bit
            Get
                Return (bits And 64UI) / 64
            End Get
        End Property

        Public ReadOnly Property alt As UInteger   ' 1 bit
            Get
                Return (bits And 128UI) / 128
            End Get
        End Property

        Public ReadOnly Property gp As UInteger    ' 16 bits
            Get
                Return (bits And 16776960UI) / 256
            End Get
        End Property

        Public ReadOnly Property mark As UInteger  ' 1 bit
            Get
                Return (bits And 16777216UI) / 16777216
            End Get
        End Property

        Public ReadOnly Property debug As UInteger ' 1 bit
            Get
                Return (bits And 33554432UI) / 33554432
            End Get
        End Property

        Public ReadOnly Property trace As UInteger ' 1 bit
            Get
                Return (bits And 67108864UI) / 67108864
            End Get
        End Property

        Public ReadOnly Property spare As UInteger ' 1 bit
            Get
                Return (bits And 134217728UI) / 134217728
            End Get
        End Property

        Public ReadOnly Property gcgen As UInteger ' 1 bit
            Get
                Return (bits And 268435456UI) / 268435456
            End Get
        End Property

        Public ReadOnly Property gccls As UInteger ' 3 bits
            Get
                Return (bits And 3758096384UI) / 536870912
            End Get
        End Property

        Public ReadOnly Property named As UInteger ' NAMED_BITS
            Get
                Return (bits And 281470681743360UL) / 4294967296
            End Get
        End Property

        Public ReadOnly Property extra As UInteger ' 32 - NAMED_BITS
            Get
                Return (bits And 18446462598732800000UL) / 281474976710656
            End Get
        End Property
    End Structure
End Namespace
