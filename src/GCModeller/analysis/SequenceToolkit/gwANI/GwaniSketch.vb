#Region "Microsoft.VisualBasic::6f8a0b2c4d6e8f0a2b4c6d8e0f2a4b6c, analysis\SequenceToolkit\gwANI\GwaniSketch.vb"

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

Imports System
Imports System.Collections.Generic


    ''' <summary>
    ''' The optional sketch based pre-filter of the gwANI engine.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Because the sequences of an alignment share the same coordinate system, a
    ''' sketch is simply the hash of the k-mer that starts at a fixed stride: every
    ''' sequence samples the very same positions, so comparing two sketches reduces to
    ''' counting the equal slots, and the k-mer Jaccard is estimated as
    ''' <c>m / (2n - m)</c> where <c>m</c> is the number of matching slots.
    ''' </para>
    ''' <para>
    ''' A pair whose per site identity is <c>t</c> keeps a k-mer with probability
    ''' <c>t^k</c>, hence its k-mer Jaccard is about <c>t^k / (2 - t^k)</c>. A pair is
    ''' skipped when the estimated Jaccard plus <see cref="GwaniOptions.SketchSafetySigma"/>
    ''' standard deviations still stays below that bound, which keeps the false
    ''' negative rate small for the pairs that are near the threshold.
    ''' </para>
    ''' <para>
    ''' The candidate pairs are produced by LSH banding, so the filter itself costs
    ''' O(sequences x bands) instead of O(sequences^2).
    ''' </para>
    ''' </remarks>
    Public NotInheritable Class GwaniSketch

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Builds the position aligned sketch of every stored row.
        ''' </summary>
        ''' <param name="store"></param>
        ''' <param name="alignmentLength">The unpadded alignment length.</param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Shared Function Build(store As GwaniMatrixStore, alignmentLength As Integer, options As GwaniOptions) As ULong()()
            Dim u As Integer = store.SequenceCount
            Dim result As ULong()() = New ULong(u - 1)() {}
            Dim mapped As Boolean = store.Backend = GwaniStoreBackend.MemoryMapped
            Dim buffer As Byte() = If(mapped, New Byte(store.PaddedLength - 1) {}, Nothing)
            Dim row As Byte() = Nothing

            For r As Integer = 0 To u - 1
                If mapped Then
                    store.ReadRowInto(r, buffer)
                    row = buffer
                Else
                    row = store.GetRow(r)
                End If

                result(r) = BuildRow(row, alignmentLength, options)
            Next

            Return result
        End Function

        ''' <summary>
        ''' Estimates the k-mer Jaccard of two position aligned sketches.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Shared Function JaccardEstimate(a As ULong(), b As ULong()) As Double
            Dim n As Integer = Math.Min(a.Length, b.Length)

            If n <= 0 Then
                Return 0.0
            End If

            Dim m As Integer = GwaniKernels.SketchMatches(a, b)

            Return m / (2.0 * n - m)
        End Function

        ''' <summary>
        ''' Tests whether a pair is worth the exact computation.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Shared Function Candidate(a As ULong(), b As ULong(), options As GwaniOptions) As Boolean
            Dim n As Integer = Math.Min(a.Length, b.Length)

            If n <= 0 Then
                Return True
            End If

            Dim m As Integer = GwaniKernels.SketchMatches(a, b)
            Dim jac As Double = m / (2.0 * n - m)
            Dim target As Double = JaccardBound(options.MinIdentityThreshold, options.SketchKmerLength)
            Dim sigma As Double = Math.Sqrt(jac * (1.0 - jac) / n)

            Return jac + options.SketchSafetySigma * sigma >= target
        End Function

        ''' <summary>
        ''' The k-mer Jaccard that corresponds to an identity threshold.
        ''' </summary>
        ''' <param name="identity"></param>
        ''' <param name="kmerLength"></param>
        ''' <returns></returns>
        Public Shared Function JaccardBound(identity As Double, kmerLength As Integer) As Double
            Dim q As Double = Math.Pow(Math.Min(1.0, Math.Max(0.0, identity)), kmerLength)

            Return q / (2.0 - q)
        End Function

        ''' <summary>
        ''' Produces the sorted candidate pair list via LSH banding. Each element is
        ''' encoded as <c>first * u + second</c> with <c>first &lt; second</c>.
        ''' </summary>
        ''' <param name="sketches"></param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Shared Function CandidatePairs(sketches As ULong()(), options As GwaniOptions) As Long()
            Dim u As Integer = sketches.Length
            Dim samples As Integer = If(u > 0, sketches(0).Length, 0)
            Dim bands As Integer = options.SketchBandCount
            Dim rows As Integer = options.SketchBandRows

            If u < 2 OrElse samples < bands * rows Then
                ' the sketch is too short for banding, fall back to the direct filter
                Return DirectCandidates(sketches, options)
            End If

            Dim buckets As New Dictionary(Of ULong, List(Of Integer))()
            Dim found As New List(Of Long)()
            Dim partners As New HashSet(Of Integer)()

            For r As Integer = 0 To u - 1
                Dim sketch As ULong() = sketches(r)

                ' a pair is found once per shared band, so the partners of one
                ' sequence are collected in a set before they are emitted
                partners.Clear()

                For band As Integer = 0 To bands - 1
                    Dim key As ULong = BandKey(sketch, band * rows, rows, band)
                    Dim bucket As List(Of Integer) = Nothing

                    If buckets.TryGetValue(key, bucket) Then
                        If bucket.Count <= options.SketchMaxBucketSize Then
                            For Each other As Integer In bucket
                                partners.Add(other)
                            Next
                        End If

                        bucket.Add(r)
                    Else
                        buckets(key) = New List(Of Integer)({r})
                    End If
                Next

                ' the pair is encoded as smaller * u + larger, so that the list stays
                ' sorted by the smaller index and the block mask can binary search it
                For Each other As Integer In partners
                    found.Add(CLng(other) * u + CLng(r))
                Next
            Next

            found.Sort()

            Return Distinct(found)
        End Function

        ''' <summary>
        ''' Builds the per target candidate bitmask of one reference block. Bit
        ''' <c>b</c> of <c>masks(j - targetStart)</c> is set when the pair
        ''' <c>(blockStart + b, j)</c> is a candidate.
        ''' </summary>
        ''' <remarks>
        ''' A candidate pair is encoded as <c>smaller * u + larger</c> with
        ''' <c>smaller &lt; larger</c>. The smaller index is the block row and the larger
        ''' index is the target, which matches the upper triangle that the engine
        ''' computes.
        ''' </remarks>
        ''' <param name="pairs">The sorted candidate pair list.</param>
        ''' <param name="u">The number of representative sequences.</param>
        ''' <param name="blockStart"></param>
        ''' <param name="blockCount">Must not be greater than 32.</param>
        ''' <param name="targetStart"></param>
        ''' <param name="targetCount"></param>
        ''' <returns></returns>
        Public Shared Function BlockMasks(pairs As Long(), u As Integer, blockStart As Integer, blockCount As Integer, targetStart As Integer, targetCount As Integer) As UInteger()
            Dim masks As UInteger() = New UInteger(targetCount - 1) {}

            If pairs Is Nothing OrElse pairs.Length = 0 OrElse u <= 0 Then
                Return masks
            End If

            ' lower bound of the first pair whose smaller index is >= blockStart
            Dim lo As Integer = 0
            Dim hi As Integer = pairs.Length

            While lo < hi
                Dim mid As Integer = (lo + hi) \ 2

                If CInt(pairs(mid) \ u) < blockStart Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Dim blockEnd As Integer = blockStart + blockCount
            Dim targetEnd As Integer = targetStart + targetCount
            Dim i As Integer = lo

            While i < pairs.Length
                Dim pair As Long = pairs(i)
                Dim rowIndex As Integer = CInt(pair \ u)

                If rowIndex >= blockEnd Then
                    Exit While
                End If

                Dim targetIndex As Integer = CInt(pair Mod u)

                If targetIndex >= targetStart AndAlso targetIndex < targetEnd Then
                    masks(targetIndex - targetStart) =
                        masks(targetIndex - targetStart) Or (1UI << (rowIndex - blockStart))
                End If

                i += 1
            End While

            Return masks
        End Function

        ''' <summary>
        ''' Removes the duplicated entries of a sorted list.
        ''' </summary>
        ''' <param name="sorted"></param>
        ''' <returns></returns>
        Private Shared Function Distinct(sorted As List(Of Long)) As Long()
            If sorted.Count = 0 Then
                Return Array.Empty(Of Long)()
            End If

            Dim result As New List(Of Long)(sorted.Count)
            Dim last As Long = sorted(0)

            result.Add(last)

            For i As Integer = 1 To sorted.Count - 1
                If sorted(i) <> last Then
                    last = sorted(i)
                    result.Add(last)
                End If
            Next

            Return result.ToArray()
        End Function

        ''' <summary>
        ''' The direct O(n^2) candidate filter, used when the sketch is too short for banding.
        ''' </summary>
        Private Shared Function DirectCandidates(sketches As ULong()(), options As GwaniOptions) As Long()
            Dim u As Integer = sketches.Length
            Dim found As New List(Of Long)()

            For r As Integer = 0 To u - 1
                For j As Integer = r + 1 To u - 1
                    If Candidate(sketches(r), sketches(j), options) Then
                        found.Add(CLng(r) * u + j)
                    End If
                Next
            Next

            found.Sort()

            Return Distinct(found)
        End Function

        Private Shared Function BuildRow(row As Byte(), alignmentLength As Integer, options As GwaniOptions) As ULong()
            Dim k As Integer = options.SketchKmerLength
            Dim span As Integer = alignmentLength - k + 1

            If span <= 0 Then
                Return Array.Empty(Of ULong)()
            End If

            Dim stride As Integer = Math.Max(1, span \ options.SketchSampleCount)
            Dim samples As New List(Of ULong)()
            Dim pos As Integer = 0

            Do While pos < span AndAlso samples.Count < options.SketchSampleCount
                samples.Add(HashWindow(row, pos, k))
                pos += stride
            Loop

            Return samples.ToArray()
        End Function

        Private Shared Function HashWindow(row As Byte(), offset As Integer, k As Integer) As ULong
            Dim h As UInteger = &H9E3779B9UI

            For i As Integer = 0 To k - 1
                h = Mix32(h Xor CUInt(row(offset + i)))
            Next

            Return CULng(h)
        End Function

        ''' <summary>
        ''' Hashes one LSH band. The band index is part of the key, otherwise two
        ''' sequences that share a k-mer at two different sampled positions would end
        ''' up in the same bucket and become a false candidate.
        ''' </summary>
        Private Shared Function BandKey(sketch As ULong(), offset As Integer, rows As Integer, band As Integer) As ULong
            Dim h As UInteger = &H85EBCA6BUI Xor CUInt(band)

            For i As Integer = 0 To rows - 1
                h = Mix32(h Xor CUInt(sketch(offset + i) And &HFFFFFFFFUL))
                h = Mix32(h Xor CUInt(sketch(offset + i) >> 32))
            Next

            Return CULng(h)
        End Function

        ''' <summary>
        ''' A 32 bit mixer. The result is kept unsigned on purpose: mixing a signed and
        ''' an unsigned operand inside a bitwise operation makes the Visual Basic
        ''' runtime convert the signed operand to unsigned, which throws on a negative
        ''' value.
        ''' </summary>
        Private Shared Function Mix32(x As UInteger) As UInteger
            Dim hc As New HashCode()

            hc.Add(x)

            Dim bits As Long = hc.ToHashCode()

            If bits < 0 Then
                bits += 4294967296L
            End If

            Return CUInt(bits)
        End Function

    End Class


#End Region
