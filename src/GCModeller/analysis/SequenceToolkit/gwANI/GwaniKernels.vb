#Region "Microsoft.VisualBasic::4d6e8f0a1b3c5d7e9f1a3b5c7d9e1f3a, analysis\SequenceToolkit\gwANI\GwaniKernels.vb"

' Author:
'
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
'
' Copyright (c) 2018 GPL3 Licensed
'
' GNU GENERAL PUBLIC LICENSE (GPL3)
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

Option Strict On

Imports System.Numerics
Imports System.Runtime.CompilerServices
Imports System.Runtime.Intrinsics
Imports System.Runtime.Intrinsics.X86


    ''' <summary>
    ''' The vectorized nucleotide identity kernel.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' For a pair of encoded rows the kernel counts two quantities in a single pass:
    ''' </para>
    ''' <list type="bullet">
    ''' <item> <description> <c>matches</c> - the sites at which both bases are known and equal </description> </item>
    ''' <item> <description> <c>valids</c>  - the sites at which both bases are known </description> </item>
    ''' </list>
    ''' <para>
    ''' Both are reduced to byte mask popcounts, which makes the inner loop branch free:
    ''' </para>
    ''' <code>
    ''' either = (a == Unknown) | (b == Unknown)
    ''' match  = ~either &amp; (a == b)
    ''' valid  = ~either
    ''' </code>
    ''' <para>
    ''' The AVX2 path processes four 32 byte tiles per iteration with four independent
    ''' accumulator pairs. A single accumulator would serialize on the
    ''' MoveMask -&gt; PopCount -&gt; add dependency chain, which costs more than the
    ''' vector instructions themselves.
    ''' </para>
    ''' <para>
    ''' The runtime dispatches to AVX2(32 bytes per tile), SSE2(16 bytes) and finally a
    ''' scalar fallback, so the kernel produces exactly the same counts on every
    ''' platform.
    ''' </para>
    ''' </remarks>
    Public Module GwaniKernels

        ''' <summary>
        ''' Popcounts a 32 bit lane mask. The Visual Basic runtime range checks every
        ''' conversion, and the mask is negative whenever the top lane matches, so the
        ''' sign bit is counted separately instead of being converted.
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function PopCount(mask As Integer) As Integer
            Return BitOperations.PopCount(CUInt(mask And &H7FFFFFFF)) + ((mask >> 31) And 1)
        End Function

        ''' <summary>
        ''' Counts the matching and the valid sites of two encoded rows.
        ''' </summary>
        ''' <param name="a">The first encoded row.</param>
        ''' <param name="b">The second encoded row.</param>
        ''' <param name="length">
        ''' The number of bytes to compare. It is normally
        ''' <see cref="NucleotideCodec.PaddedLength"/> of the alignment; any value is
        ''' accepted because a scalar tail handles the remainder.
        ''' </param>
        ''' <param name="matches">Receives the number of equal known sites.</param>
        ''' <param name="valids">Receives the number of sites at which both bases are known.</param>
        Public Sub CountPair(a As Byte(), b As Byte(), length As Integer, ByRef matches As Integer, ByRef valids As Integer)
            Dim m0 As Integer = 0, m1 As Integer = 0, m2 As Integer = 0, m3 As Integer = 0
            Dim v0 As Integer = 0, v1 As Integer = 0, v2 As Integer = 0, v3 As Integer = 0
            Dim p As Integer = 0

            If Avx2.IsSupported Then
                Dim unknown = Vector256.Create(Of Byte)(NucleotideCodec.Unknown)
                Dim ones = Vector256.Create(Of Byte)(255)
                Dim last4 As Integer = length - 128
                Dim last1 As Integer = length - 32

                ' four independent accumulator chains, the latency of
                ' MoveMask -&gt; PopCount -&gt; add is hidden by the other three
                While p <= last4
                    Dim a0 = Vector256.LoadUnsafe(Of Byte)(a(p))
                    Dim b0 = Vector256.LoadUnsafe(Of Byte)(b(p))
                    Dim a1 = Vector256.LoadUnsafe(Of Byte)(a(p + 32))
                    Dim b1 = Vector256.LoadUnsafe(Of Byte)(b(p + 32))
                    Dim a2 = Vector256.LoadUnsafe(Of Byte)(a(p + 64))
                    Dim b2 = Vector256.LoadUnsafe(Of Byte)(b(p + 64))
                    Dim a3 = Vector256.LoadUnsafe(Of Byte)(a(p + 96))
                    Dim b3 = Vector256.LoadUnsafe(Of Byte)(b(p + 96))

                    Dim e0 = Avx2.Or(Avx2.CompareEqual(a0, unknown), Avx2.CompareEqual(b0, unknown))
                    Dim e1 = Avx2.Or(Avx2.CompareEqual(a1, unknown), Avx2.CompareEqual(b1, unknown))
                    Dim e2 = Avx2.Or(Avx2.CompareEqual(a2, unknown), Avx2.CompareEqual(b2, unknown))
                    Dim e3 = Avx2.Or(Avx2.CompareEqual(a3, unknown), Avx2.CompareEqual(b3, unknown))

                    m0 += PopCount(Avx2.MoveMask(Avx2.AndNot(e0, Avx2.CompareEqual(a0, b0))))
                    v0 += PopCount(Avx2.MoveMask(Avx2.AndNot(e0, ones)))
                    m1 += PopCount(Avx2.MoveMask(Avx2.AndNot(e1, Avx2.CompareEqual(a1, b1))))
                    v1 += PopCount(Avx2.MoveMask(Avx2.AndNot(e1, ones)))
                    m2 += PopCount(Avx2.MoveMask(Avx2.AndNot(e2, Avx2.CompareEqual(a2, b2))))
                    v2 += PopCount(Avx2.MoveMask(Avx2.AndNot(e2, ones)))
                    m3 += PopCount(Avx2.MoveMask(Avx2.AndNot(e3, Avx2.CompareEqual(a3, b3))))
                    v3 += PopCount(Avx2.MoveMask(Avx2.AndNot(e3, ones)))

                    p += 128
                End While

                While p <= last1
                    Dim va = Vector256.LoadUnsafe(Of Byte)(a(p))
                    Dim vb = Vector256.LoadUnsafe(Of Byte)(b(p))
                    Dim either = Avx2.Or(Avx2.CompareEqual(va, unknown), Avx2.CompareEqual(vb, unknown))

                    m0 += PopCount(Avx2.MoveMask(Avx2.AndNot(either, Avx2.CompareEqual(va, vb))))
                    v0 += PopCount(Avx2.MoveMask(Avx2.AndNot(either, ones)))

                    p += 32
                End While

                matches = m0 + m1 + m2 + m3
                valids = v0 + v1 + v2 + v3

                Return
            End If

            If Sse2.IsSupported Then
                Dim unknown = Vector128.Create(Of Byte)(NucleotideCodec.Unknown)
                Dim ones = Vector128.Create(Of Byte)(255)
                Dim last2 As Integer = length - 32
                Dim last1 As Integer = length - 16

                While p <= last2
                    Dim a0 = Vector128.LoadUnsafe(Of Byte)(a(p))
                    Dim b0 = Vector128.LoadUnsafe(Of Byte)(b(p))
                    Dim a1 = Vector128.LoadUnsafe(Of Byte)(a(p + 16))
                    Dim b1 = Vector128.LoadUnsafe(Of Byte)(b(p + 16))

                    Dim e0 = Sse2.Or(Sse2.CompareEqual(a0, unknown), Sse2.CompareEqual(b0, unknown))
                    Dim e1 = Sse2.Or(Sse2.CompareEqual(a1, unknown), Sse2.CompareEqual(b1, unknown))

                    m0 += PopCount(Sse2.MoveMask(Sse2.AndNot(e0, Sse2.CompareEqual(a0, b0))))
                    v0 += PopCount(Sse2.MoveMask(Sse2.AndNot(e0, ones)))
                    m1 += PopCount(Sse2.MoveMask(Sse2.AndNot(e1, Sse2.CompareEqual(a1, b1))))
                    v1 += PopCount(Sse2.MoveMask(Sse2.AndNot(e1, ones)))

                    p += 32
                End While

                While p <= last1
                    Dim va = Vector128.LoadUnsafe(Of Byte)(a(p))
                    Dim vb = Vector128.LoadUnsafe(Of Byte)(b(p))
                    Dim either = Sse2.Or(Sse2.CompareEqual(va, unknown), Sse2.CompareEqual(vb, unknown))

                    m0 += PopCount(Sse2.MoveMask(Sse2.AndNot(either, Sse2.CompareEqual(va, vb))))
                    v0 += PopCount(Sse2.MoveMask(Sse2.AndNot(either, ones)))

                    p += 16
                End While

                matches = m0 + m1
                valids = v0 + v1

                Return
            End If

            ' the scalar path on the platforms without SSE2
            While p < length
                Dim ca As Byte = a(p)
                Dim cb As Byte = b(p)

                If ca <> NucleotideCodec.Unknown AndAlso cb <> NucleotideCodec.Unknown Then
                    v0 += 1

                    If ca = cb Then
                        m0 += 1
                    End If
                End If

                p += 1
            End While

            matches = m0
            valids = v0
        End Sub

        ''' <summary>
        ''' The identity percentage of a pair of encoded rows, using exactly the same
        ''' formula as the original panito tool.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <param name="length"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IdentityOf(a As Byte(), b As Byte(), length As Integer) As Double
            Dim m As Integer = 0
            Dim v As Integer = 0

            CountPair(a, b, length, m, v)

            If v > 0 Then
                Return 100.0 * m / v
            Else
                Return 0.0
            End If
        End Function

        ''' <summary>
        ''' Counts the equal slots of two position aligned sketch vectors. The LSH
        ''' banding of the pre-filter never calls this helper, so a plain loop is
        ''' enough here.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function SketchMatches(a As ULong(), b As ULong()) As Integer
            Dim n As Integer = Math.Min(a.Length, b.Length)
            Dim count As Integer = 0

            For p As Integer = 0 To n - 1
                If a(p) = b(p) Then
                    count += 1
                End If
            Next

            Return count
        End Function

    End Module


#End Region
