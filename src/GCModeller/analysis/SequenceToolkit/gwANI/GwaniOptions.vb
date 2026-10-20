#Region "Microsoft.VisualBasic::7c1e5b0a9d3f4a2b8c6d1e0f3a5b7c9d, analysis\SequenceToolkit\gwANI\GwaniOptions.vb"

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

Imports System.IO


    ''' <summary>
    ''' Runtime options of the gwANI calculation engine.
    '''
    ''' All of the properties are optional, the default value of a freshly created
    ''' instance is a reasonable configuration for a workstation class machine.
    ''' </summary>
    Public NotInheritable Class GwaniOptions

        ''' <summary>
        ''' Maximum degree of parallelism; a value that is less than or equal to zero
        ''' means <see cref="Environment.ProcessorCount"/>.
        ''' </summary>
        Public Property MaxDegreeOfParallelism As Integer = -1

        ''' <summary>
        ''' Memory budget of the encoded sequence matrix, in bytes. Once the accumulated
        ''' encoded rows exceed this budget, the store spills to a memory mapped scratch
        ''' file so that the physical memory usage stays bounded.
        ''' </summary>
        Public Property MemoryBudgetBytes As Long = 32L * 1024 * 1024 * 1024

        ''' <summary>
        ''' Scratch directory that is used by the memory mapped backend; nothing means
        ''' the system temporary directory.
        ''' </summary>
        Public Property ScratchDirectory As String = Nothing

        ''' <summary>
        ''' Collapse exactly identical sequences into a single representative row before
        ''' the pairwise kernel runs. Enabled by default: it costs a single linear hashing
        ''' pass and can reduce the quadratic work by a large factor when the alignment
        ''' contains duplicated strains.
        ''' </summary>
        Public Property EnableDeduplication As Boolean = True

        ''' <summary>
        ''' Enable the experimental sketch(LSH) pre-filter which skips the pair whose
        ''' estimated k-mer Jaccard upper bound is below the bound that is implied by
        ''' <see cref="MinIdentityThreshold"/>. Disabled by default because the filter is
        ''' a heuristic: a skipped pair is reported as <see cref="GwaniValues.Skipped"/>.
        ''' </summary>
        Public Property EnableSketchPreFilter As Boolean = False

        ''' <summary> k-mer length that is used by the sketch pre-filter. </summary>
        Public Property SketchKmerLength As Integer = 8

        ''' <summary> Number of sampled k-mer positions per sequence. </summary>
        Public Property SketchSampleCount As Integer = 128

        ''' <summary>
        ''' Number of LSH bands. With one sketch row per band a single shared sampled
        ''' k-mer already makes a pair a candidate, which keeps the miss probability of
        ''' a pair above the threshold at about (1 - t^k)^samples, i.e. essentially zero
        ''' for the default k-mer length and sample count.
        ''' </summary>
        Public Property SketchBandCount As Integer = 128

        ''' <summary> Number of sketch rows per LSH band. </summary>
        Public Property SketchBandRows As Integer = 1

        ''' <summary> Skip a LSH bucket once it holds more than this many sequences. </summary>
        Public Property SketchMaxBucketSize As Integer = 1024

        ''' <summary> Safety margin of the sketch filter, in standard deviations. </summary>
        Public Property SketchSafetySigma As Double = 3.0

        ''' <summary> ANI threshold that is used by the sketch pre-filter. </summary>
        Public Property MinIdentityThreshold As Double = 0.8

        ''' <summary>
        ''' Optional progress callback. The callback may be invoked from worker threads,
        ''' so the consumer has to be thread safe.
        ''' </summary>
        Public Property Progress As Action(Of String) = Nothing

        ''' <summary>
        ''' Resolves the effective scratch directory.
        ''' </summary>
        Public Function ResolveScratchDirectory() As String
            If Not String.IsNullOrEmpty(ScratchDirectory) Then
                Return ScratchDirectory
            Else
                Return Path.GetTempPath()
            End If
        End Function

        ''' <summary>
        ''' Resolves the effective degree of parallelism.
        ''' </summary>
        Public Function ResolveParallelism() As Integer
            If MaxDegreeOfParallelism > 0 Then
                Return MaxDegreeOfParallelism
            Else
                Return Math.Max(1, Environment.ProcessorCount)
            End If
        End Function

        ''' <summary>
        ''' A copy of the current options with the deduplication turned off, which is
        ''' what the compatibility API needs in order to keep returning the classic
        ''' input ordered matrix.
        ''' </summary>
        ''' <returns></returns>
        Public Function CloneWithoutDeduplication() As GwaniOptions
            Return New GwaniOptions With {
                .MaxDegreeOfParallelism = MaxDegreeOfParallelism,
                .MemoryBudgetBytes = MemoryBudgetBytes,
                .ScratchDirectory = ScratchDirectory,
                .EnableDeduplication = False,
                .EnableSketchPreFilter = False,
                .Progress = Progress
            }
        End Function

    End Class


#End Region
