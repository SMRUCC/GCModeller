' /********************************************************************************/
'
'  Rockhopper —— Replicate 数据模型
'
'  复刻自原始 Rockhopper（Brian Tjaden, 2013）Java 源码中的 Replicate.java。
'  原实现通过 FileOps 读取压缩比对文件；这里改为直接消费 IO 层产出的
'  AlignmentCoverage，从而移除对 Oracle.Java 文件层与全局 genomeSize 的依赖。
'
' /********************************************************************************/

Imports System.Collections.Generic

Namespace Core

    ''' <summary>
    ''' A Replicate object represents information about a single RNA-seq experiment,
    ''' including information about all reads from the experiment.
    ''' </summary>
    Public Class Replicate

        ''' <summary>原始读段文件路径。</summary>
        Private fileName As String
        ''' <summary>每个复制子的压缩文件名。</summary>
        Private ReadOnly compressedFileNames As List(Of String)
        ''' <summary>正链读段数，[genome][coordinate]。</summary>
        Private ReadOnly plusReads As Integer()()
        ''' <summary>负链读段数，[genome][coordinate]。</summary>
        Private ReadOnly minusReads As Integer()()
        Private _totalReads As Long
        ''' <summary>平均读长。</summary>
        Private _avgLengthReads As Long
        ''' <summary>每个复制子的长度。</summary>
        Private ReadOnly genomeSizes As Integer()
        ''' <summary>全部复制子长度之和。</summary>
        Private ReadOnly genomeSize As Integer

        ''' <summary>Number of nucleotides with few or no reads.</summary>
        Private ReadOnly background As List(Of Integer)
        ''' <summary>Parameter of geometric distribution of background reads.</summary>
        Private ReadOnly backgroundParameter As Double
        ''' <summary>Average number of reads mapping to nucleotides throughout genome.</summary>
        Private ReadOnly _avgReads As Double
        ''' <summary>Minimum expression for a UTR region to be deemed expressed.</summary>
        Private _minExpressionUTR As Double
        ''' <summary>Minimum expression for a ncRNA to be deemed expressed.</summary>
        Private _minExpressionRNA As Double

        ''' <summary>
        ''' 实验名称（由读段文件名推断）。
        ''' </summary>
        Private _name As String
        Public ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        ''' <summary>
        ''' Constructs a new Replicate object based on compressed sequencing reads.
        ''' </summary>
        ''' <param name="coverages">每个复制子对应的覆盖度数据。</param>
        ''' <param name="unstranded">是否为链非特异（模糊链）数据。</param>
        Public Sub New(coverages As AlignmentCoverage(), unstranded As Boolean)
            Me.compressedFileNames = New List(Of String)()
            Me.genomeSizes = New Integer(coverages.Length - 1) {}
            Me.plusReads = New Integer(coverages.Length - 1)() {}
            Me.minusReads = New Integer(coverages.Length - 1)() {}
            Me._totalReads = 0
            Me._avgLengthReads = 0

            Const backgroundLength As Integer = 2
            Me.background = New List(Of Integer)(backgroundLength)
            For i As Integer = 0 To backgroundLength - 1
                background.Add(0)
            Next

            For z As Integer = 0 To coverages.Length - 1
                Dim coverage As AlignmentCoverage = coverages(z)
                If coverage Is Nothing Then Continue For

                Me.compressedFileNames.Add(coverage.ReadFileName)
                Me.fileName = coverage.ReadFileName
                If Not String.IsNullOrEmpty(coverage.Name) Then
                    Me._name = coverage.Name
                ElseIf Not String.IsNullOrEmpty(coverage.ReadFileName) Then
                    Me._name = IO.Path.GetFileNameWithoutExtension(coverage.ReadFileName)
                End If
                Me._avgLengthReads = coverage.AvgLengthReads

                Dim size As Integer = If(coverage.PlusReads IsNot Nothing, coverage.PlusReads.Length - 1, 0)
                Me.genomeSizes(z) = size
                Me.genomeSize += size

                Me.plusReads(z) = New Integer(size) {}
                Me.minusReads(z) = New Integer(size) {}
                For i As Integer = 1 To size
                    Dim reads_plus As Integer = coverage.PlusReads(i)
                    Dim reads_minus As Integer = If(coverage.MinusReads IsNot Nothing, coverage.MinusReads(i), 0)
                    plusReads(z)(i) = reads_plus
                    minusReads(z)(i) = reads_minus
                    _totalReads += reads_plus + reads_minus
                    Call accumulateBackground(reads_plus, reads_minus, unstranded)
                Next
            Next

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
        End Sub

        Private Sub accumulateBackground(reads_plus As Integer, reads_minus As Integer, unstranded As Boolean)
            If Not unstranded Then
                ' Strand specific
                If reads_plus < background.Count Then
                    background(reads_plus) += 1
                Else
                    background(background.Count - 1) += 1
                End If
                If reads_minus < background.Count Then
                    background(reads_minus) += 1
                Else
                    background(background.Count - 1) += 1
                End If
            Else
                ' Strand ambiguous
                Dim sum As Integer = reads_plus + reads_minus
                If sum < background.Count Then
                    background(sum) += 1
                Else
                    background(background.Count - 1) += 1
                End If
            End If
        End Sub

        ''' <summary>
        ''' Return the name of the compressed alignment file for this Replicate
        ''' for the specified genome at index z.
        ''' </summary>
        Public Function GetCompressedFileName(z As Integer) As String
            If z >= 0 AndAlso z < compressedFileNames.Count Then
                Return compressedFileNames(z)
            End If
            Return Nothing
        End Function

        ''' <summary>Return total number of reads in this Replicate.</summary>
        Public ReadOnly Property TotalReads As Long
            Get
                Return _totalReads
            End Get
        End Property

        ''' <summary>Return average number of reads per nucleotide in this Replicate.</summary>
        Public ReadOnly Property AvgReads As Double
            Get
                Return _avgReads
            End Get
        End Property

        ''' <summary>
        ''' Return average length of sequencing reads in this Replicate.
        ''' </summary>
        Public ReadOnly Property AvgLengthReads As Long
            Get
                Return _avgLengthReads
            End Get
        End Property

        ''' <summary>
        ''' Return the number of reads mapping to the specified coordinate
        ''' on the specified strand for the specified genome at index z.
        ''' </summary>
        Public Function GetReads(z As Integer, coord As Integer, strand As Char) As Integer
            If z < 0 OrElse z >= plusReads.Length Then Return 0
            If coord < 0 OrElse coord >= plusReads(z).Length Then Return 0

            If strand = "+"c Then
                Return plusReads(z)(coord)
            ElseIf strand = "-"c Then
                Return minusReads(z)(coord)
            Else
                Return plusReads(z)(coord) + minusReads(z)(coord)
            End If
        End Function

        ''' <summary>
        ''' Return the number of reads on the given strand mapping to the given
        ''' range of genomic coordinates for the specified genome at index z.
        ''' </summary>
        Public Function GetReadsInRange(z As Integer, start As Integer, [stop] As Integer, strand As Char) As Long
            If z < 0 OrElse z >= plusReads.Length Then Return 0
            If [stop] < start Then
                Dim temp As Integer = start
                start = [stop]
                [stop] = temp
            End If

            Dim sum As Long = 0
            Dim upper As Integer = System.Math.Min([stop] + 1, plusReads(z).Length) - 1
            For i As Integer = System.Math.Max(start, 1) To upper
                If strand = "+"c Then
                    sum += plusReads(z)(i)
                ElseIf strand = "-"c Then
                    sum += minusReads(z)(i)
                Else
                    sum += plusReads(z)(i) + minusReads(z)(i)
                End If
            Next
            Return sum
        End Function

        ''' <summary>Return the upper quartile for this Replicate.</summary>
        Public Property UpperQuartile As Long

        ''' <summary>
        ''' Sets the minimum level of expression (for a UTR region and ncRNA to be
        ''' considered expressed) based on the average number of reads per nucleotide
        ''' and the specified transcript sensitivity in [0.0, 1.0].
        ''' </summary>
        Public WriteOnly Property MinExpression As Double
            Set(value As Double)
                _minExpressionUTR = transformation(System.Math.Pow(value, 3.3))
                _minExpressionRNA = transformation(System.Math.Pow(value, 0.4))
            End Set
        End Property

        ''' <summary>Return the minimum expression for a UTR region.</summary>
        Public ReadOnly Property MinExpressionUTR As Double
            Get
                Return _minExpressionUTR
            End Get
        End Property

        ''' <summary>Return the minimum expression for a ncRNA.</summary>
        Public ReadOnly Property MinExpressionRNA As Double
            Get
                Return _minExpressionRNA
            End Get
        End Property

        ''' <summary>
        ''' Returns the probability that the given number of reads at some
        ''' nucleotide corresponds to the background, i.e., a non-transcript.
        ''' </summary>
        Public Function GetBackgroundProb(numReads As Integer) As Double
            ' Based on geometric distribution
            Return System.Math.Pow(1.0 - backgroundParameter, numReads) * backgroundParameter
        End Function

        ''' <summary>
        ''' Return the mean number of reads on the given strand mapping to the
        ''' given range of genomic coordinates in the specified genome at index z.
        ''' </summary>
        Public Function GetMeanOfRange(z As Integer, start As Integer, [stop] As Integer, strand As Char) As Double
            Return GetReadsInRange(z, start, [stop], strand) / CDbl(System.Math.Abs([stop] - start) + 1)
        End Function

        ''' <summary>
        ''' Return the standard deviation of reads on the given strand mapping to
        ''' the given range of genomic coordinates in the specified genome at index z.
        ''' </summary>
        Public Function GetStdevOfRange(z As Integer, start As Integer, [stop] As Integer, strand As Char, mean As Double) As Double
            If z < 0 OrElse z >= plusReads.Length Then Return 0
            If [stop] < start Then
                Dim temp As Integer = start
                start = [stop]
                [stop] = temp
            End If

            Dim stdev As Double = 0.0
            Dim upper As Integer = System.Math.Min([stop] + 1, plusReads(z).Length) - 1
            For i As Integer = System.Math.Max(start, 1) To upper
                If strand = "+"c Then
                    stdev += System.Math.Pow(plusReads(z)(i) - mean, 2.0)
                ElseIf strand = "-"c Then
                    stdev += System.Math.Pow(minusReads(z)(i) - mean, 2.0)
                Else
                    stdev += System.Math.Pow(plusReads(z)(i) + minusReads(z)(i) - mean, 2.0)
                End If
            Next
            Return stdev / System.Math.Sqrt(System.Math.Min([stop], plusReads(z).Length - 1) - System.Math.Max(start, 1) + 1)
        End Function

        ''' <summary>
        ''' Helper method for setting the minimum expression level for UTRs and ncRNAs.
        ''' </summary>
        Private Function transformation(transcriptSensitivity As Double) As Double
            If transcriptSensitivity <= 0.5 Then
                Return System.Math.Pow(_avgReads / 2, System.Math.Pow(transcriptSensitivity / 0.5, 0.25))
            Else
                Return System.Math.Pow(_avgReads / 2, 1 + System.Math.Pow(2.0 * (transcriptSensitivity - 0.5), 2))
            End If
        End Function

        ''' <summary>Returns a String representation of this object.</summary>
        Public Overrides Function ToString() As String
            Return Name
        End Function

    End Class

End Namespace
