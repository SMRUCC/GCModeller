#Region "Microsoft.VisualBasic::7cddfbe2ad66c43094840dac7e755db0, analysis\RNA-Seq\TSSsTools\Transcriptome.UTRs\Replicate.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class Replicate
    ' 
    '         Properties: avgLengthReads, avgReads, minExpression, minExpressionRNA, minExpressionUTR
    '                     name, totalReads, upperQuartile
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: __strandCoordinates, __transformation, getBackgroundProb, getMeanOfRange, getReads
    '                   getReadsInRange, getStdevOfRange, ToString
    ' 
    '         Sub: (+2 Overloads) __readInAlignmentFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.ComponentModel.Loci
Imports stdNum = System.Math

Namespace Transcriptome.UTRs

    ''' <summary>
    ''' A Replicate object represents information about a single RNA-seq experiment, including information about all reads from the experiment.
    ''' </summary>
    Public Class Replicate

        ''' <summary>
        ''' Path to sequencing reads file
        ''' </summary>
        Dim fileName As String
        ''' <summary>
        ''' Reads on the plus strand for each genome
        ''' </summary>
        Dim plusReads As Integer()
        ''' <summary>
        ''' Reads on the minus strand for each genome
        ''' </summary>
        Dim minusReads As Integer()
        ''' <summary>
        ''' Number of nucleotides with few or no reads
        ''' </summary>
        Dim background As List(Of Integer)
        ''' <summary>
        ''' Parameter of geometric distribution of background reads
        ''' </summary>
        Dim backgroundParameter As Double

        ''' <summary>
        ''' Constructs a new Replicate object based on compressed sequencing reads files.
        ''' </summary>
        Sub New(genomeSize As Long, unstranded As Boolean, fileName As String)
            Call Me.New(genomeSize, unstranded, data:=ReadsCount.LoadDb(fileName))
        End Sub

        Sub New(genomeSize As Long, unstranded As Boolean, data As IEnumerable(Of ReadsCount))
            Me._totalReads = 0
            Dim backgroundLength As Integer = 2
            Me.background = New List(Of Integer)(backgroundLength)
            For i As Integer = 0 To backgroundLength - 1
                background.Add(0)
            Next
            __readInAlignmentFile(data, unstranded, genomeSize)
            For i As Integer = 0 To backgroundLength - 2
                backgroundParameter += background(i)
            Next
            backgroundParameter /= (3.0 * 2.0 * genomeSize)
            If unstranded Then
                backgroundParameter /= (3.0 * 1.0 * genomeSize)
            End If
            _avgReads = _totalReads / (2.0 * genomeSize)
            If unstranded Then
                _avgReads = _totalReads / (1.0 * genomeSize)
            End If

            If totalReads = 0 Then
                Call $"[WARN] {NameOf(totalReads)} is equals to ZERO!".__DEBUG_ECHO
            End If

            minExpression = 0.5  ' Rockhopper里面的默认参数是0.5
        End Sub

#Region "**********   PUBLIC INSTANCE METHODS   **********"

        ''' <summary>
        ''' Return the name of this Replicate.
        ''' </summary>
        Public ReadOnly Property name() As String
        ''' <summary>
        ''' Return total number of reads in this Replicate.
        ''' </summary>
        Public ReadOnly Property totalReads() As Long
        ''' <summary>
        ''' Return average number of reads in this Replicate.
        ''' </summary>
        Public ReadOnly Property avgReads() As Double

        ''' <summary>
        ''' Return the number of reads mapping to the specified coordinate
        ''' on the specified strand for the specified genome at index z.
        ''' </summary>
        Public Function getReads(coord As Integer, strand As Char) As Integer
            If strand = "+"c Then
                Return plusReads(coord)
            ElseIf strand = "-"c Then
                Return minusReads(coord)
            Else
                Return plusReads(coord) + minusReads(coord)
            End If
        End Function

        ''' <summary>
        ''' Return the number of reads on the given strand mapping
        ''' to the given range of genomic coordinates for the specified
        ''' genome at index z.
        ''' </summary>
        Public Function getReadsInRange(start As Integer, [stop] As Integer, strand As Char) As Long
            If [stop] < start Then
                ' Swap
                Dim temp As Integer = start
                start = [stop]
                [stop] = temp
            End If

            Dim sum As Long = 0
            For i As Integer = Math.Max(start, 1) To stdNum.Min([stop] + 1, plusReads.Length) - 1
                If strand = "+"c Then
                    ' Plus strand
                    sum += Me.plusReads(i)
                ElseIf strand = "-"c Then
                    ' Minus strand
                    sum += Me.minusReads(i)
                Else
                    ' Ambiguous strand
                    sum += Me.plusReads(i) + Me.minusReads(i)
                End If
            Next
            Return sum
        End Function

        ''' <summary>
        ''' Return the upper quartile for this Replicate.
        ''' </summary>
        Public Property upperQuartile() As Long

        ''' <summary>
        ''' Sets the minimum level of expression (for a UTR region and
        ''' ncRNA to be considered expressed) in this Replicate based on 
        ''' the average number of reads per nucleotide in this Replicate
        ''' and the specified transcript sensitivity between 0.0 and 1.0, 
        ''' inclusive.
        ''' </summary>
        Public WriteOnly Property minExpression() As Double
            Set
                _minExpressionUTR = __transformation(Math.Pow(Value, 3.3))
                _minExpressionRNA = __transformation(Math.Pow(Value, 0.4))
            End Set
        End Property

        ''' <summary>
        ''' Return the minimum level of expression for a UTR region to be
        ''' considered expressed in this Replicate.
        ''' </summary>
        Public ReadOnly Property minExpressionUTR() As Double

        ''' <summary>
        ''' Return the minimum level of expression for a ncRNA to be
        ''' considered expressed in this Replicate.
        ''' </summary>
        Public ReadOnly Property minExpressionRNA() As Double

        ''' <summary>
        ''' Return average length of sequencing reads in this Replicate.
        ''' </summary>
        Public ReadOnly Property avgLengthReads() As Long

        ''' <summary>
        ''' Returns the probability that the given number of reads
        ''' at some nucleotide corresponds to the background, i.e.,
        ''' a non-transcript.
        ''' </summary>
        Public Function getBackgroundProb(numReads As Integer) As Double
            ' Based on geometric distribution
            Return Math.Pow(1.0 - Me.backgroundParameter, numReads) * Me.backgroundParameter
        End Function

        ''' <summary>
        ''' Return the mean number of reads on the given strand mapping
        ''' to the given range of genomic coordinates in the specified
        ''' genome at index z.
        ''' </summary>
        Public Function getMeanOfRange(start As Integer, [stop] As Integer, strand As Char) As Double
            Return getReadsInRange(start, [stop], strand) / CDbl(Math.Abs([stop] - start) + 1)
        End Function

        ''' <summary>
        ''' Return the standard deviation of reads on the given strand mapping
        ''' to the given range of genomic coordinates in the specified genome
        ''' at index z.
        ''' </summary>
        Public Function getStdevOfRange(start As Integer, [stop] As Integer, strand As Char, mean As Double) As Double
            Dim stdev As Double = 0.0

            If [stop] < start Then Call start.Swap([stop])    ' Swap

            For i As Integer = Math.Max(start, 1) To stdNum.Min([stop] + 1, plusReads.Length) - 1
                If strand = "+"c Then '      ' Plus strand
                    stdev += Math.Pow(plusReads(i) - mean, 2.0)
                ElseIf strand = "-"c Then '      ' Minus strand
                    stdev += Math.Pow(minusReads(i) - mean, 2.0)
                Else '     ' Ambiguous strand
                    stdev += Math.Pow(plusReads(i) + minusReads(i) - mean, 2.0)
                End If
            Next
            Return stdev / Math.Sqrt(stdNum.Min([stop], plusReads.Length - 1) - Math.Max(start, 1) + 1)
        End Function

        ''' <summary>
        ''' Returns a String representation of this object.
        ''' </summary>
        Public Overloads Function ToString() As String
            Return _name
        End Function
#End Region

        Private Sub __readInAlignmentFile(data As IEnumerable(Of ReadsCount), unstranded As Boolean, genomeSizes As Long)
            Me._avgLengthReads = 90 '(From obj In data.AsParallel Select obj.Length).Sum / data.Count
            Dim coordinates_plus As Integer() = __strandCoordinates(data, Strands.Forward, genomeSizes)
            Dim coordinates_minus As Integer() = __strandCoordinates(data, Strands.Reverse, genomeSizes)
            Me.plusReads = New Integer(genomeSizes - 1) {}
            Me.minusReads = New Integer(genomeSizes - 1) {}

            For i As Integer = 0 To genomeSizes - 1
                Dim reads_plus As Integer = coordinates_plus(i)
                Dim reads_minus As Integer = coordinates_minus(i)

                plusReads(i) = reads_plus
                minusReads(i) = reads_minus
                _totalReads += reads_plus + reads_minus

                If Not unstranded Then     ' Strand specific
                    If reads_plus < background.Count Then
                        background(reads_plus) = background(reads_plus) + 1
                    Else
                        background(background.Count - 1) = background(background.Count - 1) + 1
                    End If
                    If reads_minus < background.Count Then
                        background(reads_minus) = background(reads_minus) + 1
                    Else
                        background(background.Count - 1) = background(background.Count - 1) + 1
                    End If
                Else        ' Strand ambiguous
                    If reads_plus + reads_minus < background.Count Then
                        background(reads_plus + reads_minus) = background(reads_plus + reads_minus) + 1
                    Else
                        background(background.Count - 1) = background(background.Count - 1) + 1
                    End If
                End If
            Next
        End Sub

        ''' <summary>
        ''' Reads in a compressed alignment file for a genome with the specified index z. Stores the 
        ''' RNA-seq data in two lists, one for the plus strand 
        ''' and one for the minus strand. Also computes the 
        ''' total reads in the file.
        ''' </summary>
        Private Sub __readInAlignmentFile(inFile As String, unstranded As Boolean, genomeSizes As Long)
            Dim data = inFile.LoadCsv(Of DocumentFormat.Transcript)(False)
            Call __readInAlignmentFile(data, unstranded, genomeSizes)
        End Sub

        Private Function __strandCoordinates(data As IEnumerable(Of ReadsCount),
                                             Strand As Strands,
                                             genomeSize As Long) As Integer()

            Dim coordinates As Integer() = New Integer(genomeSize - 1) {}

            If Strand = Strands.Forward Then
                For Each nt As ReadsCount In data
                    coordinates(nt.Index - 1) += nt.ReadsPlus  ' 由于序列数据是从1开始的，而数组是从0开始的，所以在这里需要减1，否则会越界
                Next

            Else
                For Each nt As ReadsCount In data
                    coordinates(nt.Index - 1) += nt.ReadsMinus
                Next
            End If

            'For Each obj In strandData
            '    Dim Ends = Math.Max(obj.MappingLocation.Left, obj.MappingLocation.Right)
            '    For idx = sys.Min(obj.MappingLocation.Left, obj.MappingLocation.Right) To Ends - 1
            '        coordinates(idx) = coordinates(idx) + obj.TSSsShared
            '    Next
            'Next

            Return coordinates
        End Function

        ''' <summary>
        ''' Helper method for setting the minimum expression level for UTRs and ncRNAs.
        ''' </summary>
        Private Function __transformation(transcriptSensitivity As Double) As Double
            If transcriptSensitivity <= 0.5 Then
                ' less than or equal to 0.5
                Return Math.Pow(_avgReads / 2, Math.Pow(transcriptSensitivity / 0.5, 0.25))
            Else
                ' greater than 0.5
                Return Math.Pow(_avgReads / 2, 1 + Math.Pow(2.0 * (transcriptSensitivity - 0.5), 2))
            End If
        End Function
    End Class
End Namespace
