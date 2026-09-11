' /********************************************************************************/
'
'  Rockhopper —— Gene 数据模型
'
'  复刻自原始 Rockhopper（Brian Tjaden, 2013）Java 源码中的 Gene.java，
'  并迁移到 GCModeller 现代 VB.NET 框架：
'    * 移除 `Oracle.Java.*` / `StringSplit` / `Output(...)` 等已失效符号，改用 BCL。
'    * lowess / 负二项分布等数学函数改由 RNA-seq.Data 的 Statistics 模块提供。
'    * 算法逻辑（均值/方差/RPKM/负二项 p 值/BH 校正）严格保持与原始实现一致。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Text
Imports SMRUCC.genomics.SequenceModel.RNA_Seq.Statistics

Namespace Core

    ''' <summary>
    ''' A Gene object represents a gene (either protein-coding or RNA).
    ''' A Gene object consists of a variety of data about a gene,
    ''' including its coordinates, strand, name, product, and
    ''' expression information in each replicate (raw counts mapping
    ''' to gene and normalized counts mapping to gene) or in each
    ''' condition (mean, variance, lowess, RPKM) or in a pair of
    ''' conditions (p-value of differential expression).
    ''' </summary>
    Public Class Gene

        ''' <summary>
        ''' 翻译起始位点（Translation start）。
        ''' </summary>
        Public ReadOnly Property Start As Integer
        ''' <summary>
        ''' 翻译终止位点（Translation stop）。
        ''' </summary>
        Public ReadOnly Property [Stop] As Integer
        ''' <summary>
        ''' 基因所在链，'+' / '-' / '?'。
        ''' </summary>
        Public ReadOnly Property Strand As Char
        ''' <summary>
        ''' 基因类型，ORF 或 RNA。
        ''' </summary>
        Public ReadOnly Property Type As String
        ''' <summary>
        ''' 基因 ID（GenBank 中的 PID）。
        ''' </summary>
        Public ReadOnly Property ID As String
        ''' <summary>
        ''' 基因名（GenBank 中的 gene name）。
        ''' </summary>
        Public ReadOnly Property Name As String
        ''' <summary>
        ''' 基因别名 / locus_tag。
        ''' </summary>
        Public ReadOnly Property Synonym As String
        ''' <summary>
        ''' 基因产物描述。
        ''' </summary>
        Public ReadOnly Property Product As String

        ''' <summary>
        ''' 转录起始位点（Transcription start），RNA 基因由注释给出，ORF 由预测给出。
        ''' </summary>
        Public Property StartT As Integer
        ''' <summary>
        ''' 转录终止位点（Transcription stop）。
        ''' </summary>
        Public Property StopT As Integer

        ''' <summary>
        ''' 每个碱基上比对的读段数，[condition][replicate][coordinate]。
        ''' </summary>
        Public rawCounts As List(Of List(Of Long))
        ''' <summary>
        ''' 比对到基因上的读段数（不展开到每个碱基），[condition][replicate]。
        ''' </summary>
        Public rawCounts_reads As List(Of List(Of Long))

        Private ReadOnly normalizedCounts As List(Of List(Of Long))
        Private ReadOnly RPKMs As List(Of Long)
        Private ReadOnly means As List(Of Long)
        Private variances As Long()()
        Private lowess As Long()()
        ''' <summary>
        ''' 差异表达 p 值（按条件对的顺序排列）。
        ''' </summary>
        Public ReadOnly pValues As List(Of Double)
        ''' <summary>
        ''' 差异表达 q 值（BH 校正后）。
        ''' </summary>
        Public ReadOnly qValues As List(Of Double)

        ''' <summary>
        ''' Constructs a new Gene object based on a line from a
        ''' gene file (either *.ptt or *.rnt).
        ''' </summary>
        ''' <param name="line">*.ptt / *.rnt 中的一行（Tab 分隔，至少 9 列）。</param>
        ''' <param name="type">ORF 或 RNA。</param>
        Public Sub New(line As String, type As String)
            Dim parse_line As String() = line.Split(ControlChars.Tab)
            If parse_line.Length < 9 Then
                Call Logging.Output($"Error - expecting 9 columns of gene information but found less than 9:{vbTab}{line}{vbLf}")
                Return
            End If

            Dim parse_coords As String() = parse_line(0).Split("."c)
            Me.Type = type

            ' 原文使用 parse_line(1)(0) 取链方向字符
            Dim strandChar As Char = If(parse_line(1).Length > 0, parse_line(1)(0), "?"c)
            Me.ID = parse_line(3)
            Dim name As String = parse_line(4)
            Me.Synonym = parse_line(5)
            Me.Product = parse_line(8)

            ' 当 gene name 为空（长度 <= 1）时回退到 locus_tag，语义与原文 name() 一致
            Me.Name = If(name.Length > 1, name, Me.Synonym)

            ' Set coordinates based on strand and type
            Dim x As Integer = CInt(Val(parse_coords(0)))
            ' First coord
            Dim y As Integer = CInt(Val(parse_coords(2)))
            ' Second coord

            Select Case type.ToUpperInvariant
                Case "ORF"
                    ' 正链 ORF：x->y；负链 ORF：y->x
                    If strandChar = "-"c Then
                        Me.Start = y
                        Me.[Stop] = x
                    Else
                        Me.Start = x
                        Me.[Stop] = y
                    End If
                    Me.StartT = 0
                    Me.StopT = 0
                Case "RNA"
                    ' RNA 使用转录坐标；负链交换起止
                    Me.Start = 0
                    Me.[Stop] = 0
                    If strandChar = "-"c Then
                        Me.StartT = y
                        Me.StopT = x
                    Else
                        Me.StartT = x
                        Me.StopT = y
                    End If
                Case Else
                    Call Logging.Output($"Error - this case should be unreachable!{vbLf}")
            End Select

            Me.Strand = strandChar
            Me.rawCounts = New List(Of List(Of Long))()
            Me.rawCounts_reads = New List(Of List(Of Long))()
            Me.normalizedCounts = New List(Of List(Of Long))()
            Me.RPKMs = New List(Of Long)()
            Me.means = New List(Of Long)()
            Me.pValues = New List(Of Double)()
            Me.qValues = New List(Of Double)()
        End Sub

        ''' <summary>
        ''' Returns true if this gene is a protein coding gene, false otherwise.
        ''' </summary>
        Public ReadOnly Property ORF As Boolean
            Get
                Return String.Equals(Type, "ORF", System.StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        ''' <summary>
        ''' Returns the first (smallest) coordinate of this Gene.
        ''' If this Gene is an ORF it returns the smallest translation coordinate.
        ''' If this Gene is an RNA it returns the smallest transcription coordinate.
        ''' </summary>
        Public ReadOnly Property First As Integer
            Get
                If ORF Then Return System.Math.Min(Start, [Stop])
                Return System.Math.Min(StartT, StopT)
            End Get
        End Property

        ''' <summary>
        ''' Returns the last (largest) coordinate of this Gene.
        ''' </summary>
        Public ReadOnly Property Last As Integer
            Get
                If ORF Then Return System.Math.Max(Start, [Stop])
                Return System.Math.Max(StartT, StopT)
            End Get
        End Property

        ''' <summary>
        ''' Returns the coordinate (among transcription start/stop coordinates
        ''' and translation start/stop coordinates) with minimum value.
        ''' </summary>
        Public ReadOnly Property MinCoordinate As Integer
            Get
                Dim minCoord As Integer = Integer.MaxValue
                If StartT > 0 Then minCoord = System.Math.Min(minCoord, StartT)
                If Start > 0 Then minCoord = System.Math.Min(minCoord, Start)
                If [Stop] > 0 Then minCoord = System.Math.Min(minCoord, [Stop])
                If StopT > 0 Then minCoord = System.Math.Min(minCoord, StopT)
                Return minCoord
            End Get
        End Property

        ''' <summary>
        ''' Returns the coordinate with maximum value.
        ''' </summary>
        Public ReadOnly Property MaxCoordinate As Integer
            Get
                Dim maxCoord As Integer = -1
                If StartT > 0 Then maxCoord = System.Math.Max(maxCoord, StartT)
                If Start > 0 Then maxCoord = System.Math.Max(maxCoord, Start)
                If [Stop] > 0 Then maxCoord = System.Math.Max(maxCoord, [Stop])
                If StopT > 0 Then maxCoord = System.Math.Max(maxCoord, StopT)
                Return maxCoord
            End Get
        End Property

        ''' <summary>
        ''' Returns the minimum q-value for this Gene.
        ''' </summary>
        Public ReadOnly Property MinQvalue As Double
            Get
                Dim min As Double = 1.0
                For i As Integer = 0 To qValues.Count - 1
                    min = System.Math.Min(min, qValues(i))
                Next
                Return min
            End Get
        End Property

        Public Function HasQvalue(c As Integer) As Boolean
            Return qValues IsNot Nothing AndAlso qValues.Count > c
        End Function

#Region "Expression accessors"

        ''' <summary>
        ''' Return the number of reads mapping to the gene in the specified condition and replicate.
        ''' </summary>
        Public Function GetRawCount(condition As Integer, replicate As Integer) As Long
            If condition < rawCounts.Count Then
                If replicate < rawCounts(condition).Count Then
                    Return rawCounts(condition)(replicate)
                End If
            End If
            Return 0
        End Function

        ''' <summary>
        ''' Return the number of reads (read-level) mapping to the gene.
        ''' </summary>
        Public Function GetRawCount_reads(condition As Integer, replicate As Integer) As Long
            If condition < rawCounts_reads.Count Then
                If replicate < rawCounts_reads(condition).Count Then
                    Return rawCounts_reads(condition)(replicate)
                End If
            End If
            Return 0
        End Function

        ''' <summary>
        ''' Return the normalized number of reads mapping to the gene.
        ''' </summary>
        Public Function GetNormalizedCount(condition As Integer, replicate As Integer) As Long
            If condition < normalizedCounts.Count Then
                If replicate < normalizedCounts(condition).Count Then
                    Return normalizedCounts(condition)(replicate)
                End If
            End If
            Return 0
        End Function

        ''' <summary>
        ''' Return the mean reads mapping to the gene in the specified condition.
        ''' </summary>
        Public Function GetMean(condition As Integer) As Long
            If condition < means.Count Then Return means(condition)
            Return 0
        End Function

        ''' <summary>
        ''' Returns the average expression of the gene (averaged over the
        ''' length of the gene) in the specified condition.
        ''' </summary>
        Public Function GetAvg(condition As Integer) As Long
            Dim avg As Long = 0
            If ORF Then
                If MaxCoordinate >= 0 Then
                    avg = means(condition) \ (System.Math.Max(Start, [Stop]) - System.Math.Min(Start, [Stop]) + 1)
                End If
            Else
                avg = means(condition) \ (System.Math.Max(StartT, StopT) - System.Math.Min(StartT, StopT) + 1)
            End If
            Return avg
        End Function

        ''' <summary>
        ''' Return the RPKM value of this gene in the specified condition.
        ''' </summary>
        Public Function GetRPKM(condition As Integer) As Long
            If condition < RPKMs.Count Then Return RPKMs(condition)
            Return 0
        End Function

        ''' <summary>
        ''' Return the number of replicates in the specified condition.
        ''' </summary>
        Public Function GetNumReplicates(condition As Integer) As Integer
            If condition >= rawCounts.Count Then Return 0
            Return rawCounts(condition).Count
        End Function

        ''' <summary>
        ''' Returns a String representation of a Gene's expression in each condition
        ''' and p-values of differential expression.
        ''' </summary>
        Public Function ExpressionToString() As String
            Dim sb As New StringBuilder()
            For i As Integer = 0 To means.Count - 1
                If Logging.Verbose Then
                    For j As Integer = 0 To rawCounts(i).Count - 1
                        sb.Append(vbTab & rawCounts_reads(i)(j))
                    Next
                    For j As Integer = 0 To normalizedCounts(i).Count - 1
                        sb.Append(vbTab & normalizedCounts(i)(j))
                    Next
                    sb.Append(vbTab & RPKMs(i))
                End If
                sb.Append(vbTab & GetAvg(i))
            Next
            For i As Integer = 0 To qValues.Count - 1
                If Logging.Verbose Then sb.Append(vbTab & pValues(i))
                sb.Append(vbTab & qValues(i))
            Next
            Return sb.ToString()
        End Function

#End Region

#Region "Expression mutators"

        ''' <summary>
        ''' 确保 expression 数据容器已扩展到 [condition][replicate] 维度。
        ''' </summary>
        Private Sub ensureCapacity(condition As Integer, replicate As Integer)
            If rawCounts Is Nothing Then
                ' 构造函数已初始化，此分支仅在反序列化场景下兜底
                Return
            End If
            While rawCounts.Count < condition + 1
                rawCounts.Add(New List(Of Long)())
                rawCounts_reads.Add(New List(Of Long)())
                normalizedCounts.Add(New List(Of Long)())
                RPKMs.Add(0L)
                means.Add(0L)
            End While
            While rawCounts(condition).Count < replicate + 1
                rawCounts(condition).Add(0L)
                rawCounts_reads(condition).Add(0L)
                normalizedCounts(condition).Add(0L)
            End While
        End Sub

        ''' <summary>
        ''' Set the number of reads mapping to this Gene in the specified
        ''' replicate in the specified condition.
        ''' </summary>
        Public Sub SetRawCount(condition As Integer, replicate As Integer, readsForGene As Long)
            Call ensureCapacity(condition, replicate)
            rawCounts(condition)(replicate) = readsForGene
        End Sub

        ''' <summary>
        ''' Set the read-level number of reads mapping to this Gene.
        ''' </summary>
        Public Sub SetRawCount_reads(condition As Integer, replicate As Integer, readsForGene As Long)
            If condition < rawCounts_reads.Count Then
                If replicate < rawCounts_reads(condition).Count Then
                    rawCounts_reads(condition)(replicate) = readsForGene
                End If
            End If
        End Sub

        ''' <summary>
        ''' Set the normalized number of reads mapping to this Gene.
        ''' </summary>
        Public Sub SetNormalizedCount(condition As Integer, replicate As Integer, scalingFactor As Double, upperQuartile As Long)
            If condition < normalizedCounts.Count Then
                If replicate < normalizedCounts(condition).Count Then
                    Dim multiplier As Double = scalingFactor / CDbl(upperQuartile)
                    Dim normalizedCount As Long = CLng(System.Math.Truncate(multiplier * GetRawCount(condition, replicate)))
                    normalizedCounts(condition)(replicate) = normalizedCount
                End If
            End If
        End Sub

        ''' <summary>
        ''' For each condition, across all replicates of the conditions, compute
        ''' the mean and RPKM for this Gene.
        ''' </summary>
        Public Sub ComputeExpression(conditions As List(Of Condition))
            For i As Integer = 0 To means.Count - 1
                Dim sumRawCounts As Long = 0
                Dim totalCounts As Long = 0
                Dim mean As Long = 0
                For j As Integer = 0 To rawCounts(i).Count - 1
                    sumRawCounts += rawCounts(i)(j)
                    totalCounts += conditions(i).GetReplicate(j).TotalReads
                    mean += normalizedCounts(i)(j)
                Next
                sumRawCounts \= rawCounts(i).Count
                totalCounts \= rawCounts(i).Count
                mean \= normalizedCounts(i).Count
                If totalCounts = 0 Then
                    RPKMs(i) = 0
                Else
                    RPKMs(i) = CLng(1000000000L * sumRawCounts \ (totalCounts * (MaxCoordinate - MinCoordinate + 1)))
                End If
                means(i) = mean
            Next
        End Sub

        ''' <summary>
        ''' For each condition, across all replicates of the conditions, compute
        ''' the variance for this Gene.
        ''' </summary>
        Public Sub ComputeVariance(conditions As List(Of Condition))
            Const varianceAdjustmentNoReplicates As Double = 1.1
            Const varianceAdjustmentReplicates As Double = 1.2

            variances = newLongMatrix(conditions.Count, conditions.Count)
            lowess = newLongMatrix(conditions.Count, conditions.Count)

            For x As Integer = 0 To conditions.Count - 1
                For y As Integer = 0 To conditions.Count - 1
                    If x = y Then Continue For

                    If conditions(x).NumReplicates() = 1 Then
                        ' No replicates. Use partner surrogate.
                        Dim partner As Integer = y
                        Dim mean As Long = (means(x) + means(partner)) \ 2
                        Dim variance As Long = ((means(x) - mean) * (means(x) - mean) + (means(partner) - mean) * (means(partner) - mean)) \ 1
                        variances(x)(y) = CLng(System.Math.Truncate(System.Math.Pow(variance, varianceAdjustmentNoReplicates)))
                    Else
                        Dim variance As Long = 0
                        For j As Integer = 0 To conditions(x).NumReplicates() - 1
                            variance += (normalizedCounts(x)(j) - means(x)) * (normalizedCounts(x)(j) - means(x))
                        Next
                        variance \= (conditions(x).NumReplicates() - 1)
                        variances(x)(y) = CLng(System.Math.Truncate(System.Math.Pow(variance, varianceAdjustmentReplicates)))
                    End If
                Next
            Next
        End Sub

        Private Shared Function newLongMatrix(rows As Integer, cols As Integer) As Long()()
            Dim m As Long()() = New Long(rows - 1)() {}
            For i As Integer = 0 To rows - 1
                m(i) = New Long(cols - 1) {}
            Next
            Return m
        End Function

        ''' <summary>
        ''' For each pair of conditions, compute the p-value of differential
        ''' expression for this Gene.
        ''' </summary>
        Public Sub ComputeDifferentialExpression()
            pValues.Clear()
            qValues.Clear()

            For x As Integer = 0 To normalizedCounts.Count - 2
                For y As Integer = x + 1 To normalizedCounts.Count - 1

                    Dim k_A As Double = 0.0
                    Dim k_B As Double = 0.0
                    For j As Integer = 0 To normalizedCounts(x).Count - 1
                        k_A += normalizedCounts(x)(j)
                    Next
                    For j As Integer = 0 To normalizedCounts(y).Count - 1
                        k_B += normalizedCounts(y)(j)
                    Next

                    If normalizedCounts(x).Count < normalizedCounts(y).Count Then
                        k_B *= normalizedCounts(x).Count / CDbl(normalizedCounts(y).Count)
                    ElseIf normalizedCounts(x).Count > normalizedCounts(y).Count Then
                        k_A *= normalizedCounts(y).Count / CDbl(normalizedCounts(x).Count)
                    End If

                    Dim q As Double = k_A + k_B
                    Dim mean_A As Double = q
                    Dim mean_B As Double = q
                    Dim variance_A As Double = lowess(x)(y)
                    Dim variance_B As Double = lowess(y)(x)

                    Dim p_a As Double = mean_A / variance_A
                    Dim p_b As Double = mean_B / variance_B
                    ' r should not be < 1
                    Dim r_a As Double = System.Math.Max(mean_A * mean_A / (variance_A - mean_A), 1.0)
                    Dim r_b As Double = System.Math.Max(mean_B * mean_B / (variance_B - mean_B), 1.0)

                    If p_a < 0.0 OrElse p_b < 0.0 OrElse p_a > 1.0 OrElse p_b > 1.0 OrElse variance_A = 0.0 OrElse variance_B = 0.0 Then
                        pValues.Add(1.0)
                        qValues.Add(1.0)
                        Continue For
                    End If

                    ' Compute p-value of differential expression in two conditions
                    Dim p_ab As Double = NegativeBinomial.PMF(r_a - 1, k_A + r_a - 1, p_a) * NegativeBinomial.PMF(r_b - 1, k_B + r_b - 1, p_b)
                    Dim k_sum As Long = CLng(System.Math.Truncate(k_A + k_B))

                    ' Fast p-value estimation（向上扫描）
                    Dim numerator As Double = 0.0
                    Dim denominator As Double = 0.0
                    Dim mode As Long = CLng(System.Math.Truncate(k_B))

                    Dim a As Long = mode
                    Dim increment As Long = 1
                    Dim alpha As Long = 1000
                    Dim previous_p As Double = 0.0

                    While a <= k_sum
                        Dim b As Long = k_sum - a
                        Dim current_p As Double = NegativeBinomial.PMF(r_a - 1, a + r_a - 1, p_a) * NegativeBinomial.PMF(r_b - 1, b + r_b - 1, p_b)
                        denominator += current_p
                        If current_p <= p_ab Then numerator += current_p
                        If increment > 1 Then
                            Dim average_p As Double = (current_p + previous_p) / 2.0
                            denominator += average_p * (increment - 1)
                            If average_p <= p_ab Then numerator += average_p * (increment - 1)
                        End If
                        previous_p = current_p
                        If a - mode >= alpha Then
                            alpha *= 2
                            increment *= 2
                        End If
                        a += increment
                    End While

                    ' 向下扫描
                    a = mode
                    Dim decrement As Long = 1
                    alpha = 1000
                    previous_p = 0.0
                    While a >= 0
                        Dim b As Long = k_sum - a
                        Dim current_p As Double = NegativeBinomial.PMF(r_a - 1, a + r_a - 1, p_a) * NegativeBinomial.PMF(r_b - 1, b + r_b - 1, p_b)
                        denominator += current_p
                        If current_p <= p_ab Then numerator += current_p
                        If decrement > 1 Then
                            Dim average_p As Double = (previous_p + current_p) / 2.0
                            denominator += average_p * (decrement - 1)
                            If average_p <= p_ab Then numerator += average_p * (decrement - 1)
                        End If
                        previous_p = current_p
                        If mode - a >= alpha Then
                            alpha *= 2
                            decrement *= 2
                        End If
                        a -= decrement
                    End While

                    Dim p_value As Double = 1.0
                    If denominator <> 0.0 Then p_value = numerator / denominator

                    pValues.Add(p_value)
                    qValues.Add(1.0)
                Next
            Next
        End Sub

        ''' <summary>
        ''' Returns true if this Gene is an ORF and if it is differentially
        ''' expressed in at least one pair of conditions at the specified
        ''' significance level.
        ''' </summary>
        Public Function IsDifferentiallyExpressedORF(significance As Double) As Boolean
            If Not ORF Then Return False
            For i As Integer = 0 To qValues.Count - 1
                If qValues(i) <= significance Then Return True
            Next
            Return False
        End Function

#End Region

#Region "Shared statistics"

        ''' <summary>
        ''' Computes the Lowess variance for each Gene in the Genome.
        ''' </summary>
        Public Shared Sub SetLowessVariances(genomes As List(Of Genome), conditions As List(Of Condition))
            For x As Integer = 0 To conditions.Count - 1
                For y As Integer = 0 To conditions.Count - 1
                    If x = y Then Continue For

                    ' Bias correction term
                    Dim b As Double = 0.0
                    For j As Integer = 0 To conditions(x).NumReplicates() - 1
                        b += 100000.0 / CDbl(conditions(x).GetReplicate(j).UpperQuartile)
                    Next
                    b /= conditions(x).NumReplicates()

                    ' Create list of gene expressions and list of gene variances
                    Dim expression As New List(Of Long)()
                    Dim variance As New List(Of Long)()
                    For z As Integer = 0 To genomes.Count - 1
                        Dim genome As Genome = genomes(z)
                        For j As Integer = 0 To genome.NumGenes() - 1
                            expression.Add(genome.GetGene(j).means(x))
                            variance.Add(genome.GetGene(j).variances(x)(y))
                        Next
                    Next

                    ' Perform Lowess computation
                    Dim lowessVariance As Double() = Statistics.Lowess.Fit(
                        expression.Select(Function(v) CDbl(v)),
                        variance.Select(Function(v) CDbl(v)))

                    ' Assign each gene its lowess variances (after subtracting bias correction term)
                    Dim previousGenomeSizes As Integer = 0
                    For z As Integer = 0 To genomes.Count - 1
                        Dim genome As Genome = genomes(z)
                        For j As Integer = 0 To genome.NumGenes() - 1
                            genome.GetGene(j).lowess(x)(y) = CLng(lowessVariance(previousGenomeSizes + j) - (genome.GetGene(j).GetMean(x) * b))
                        Next
                        previousGenomeSizes += genome.NumGenes()
                    Next
                Next
            Next
        End Sub

        ''' <summary>
        ''' Computes q-values for each gene, i.e., corrected p-values,
        ''' using Benjamini Hochberg correction.
        ''' </summary>
        Public Shared Sub CorrectPvalues(genomes As List(Of Genome), conditions As List(Of Condition))
            Dim totalGenes As Integer = 0
            For z As Integer = 0 To genomes.Count - 1
                totalGenes += genomes(z).NumGenes()
            Next

            Dim pValue_index As Integer = 0
            For x As Integer = 0 To genomes(0).GetGene(0).means.Count - 2
                For y As Integer = x + 1 To genomes(0).GetGene(0).means.Count - 1

                    Dim pvalues As Double() = New Double(totalGenes - 1) {}
                    Dim indices As Integer() = New Integer(totalGenes - 1) {}
                    Dim genomeIndices As Integer() = New Integer(totalGenes - 1) {}

                    Dim previousGenomeSizes As Integer = 0
                    For z As Integer = 0 To genomes.Count - 1
                        Dim genome As Genome = genomes(z)
                        For j As Integer = 0 To genome.NumGenes() - 1
                            Dim g As Gene = genome.GetGene(j)
                            pvalues(previousGenomeSizes + j) = g.pValues(pValue_index)
                            indices(previousGenomeSizes + j) = j
                            genomeIndices(previousGenomeSizes + j) = z

                            ' Check if there is too little expression to compute a p-value
                            Dim e1 As Double = g.means(x)
                            Dim e2 As Double = g.means(y)
                            If g.ORF Then
                                Dim len As Integer = System.Math.Max(g.Start, g.[Stop]) - System.Math.Min(g.Start, g.[Stop]) + 1
                                e1 /= len
                                e2 /= len
                            Else
                                Dim len As Integer = System.Math.Max(g.StartT, g.StopT) - System.Math.Min(g.StartT, g.StopT) + 1
                                e1 /= len
                                e2 /= len
                            End If
                            If e1 < conditions(x).MinDiffExpressionLevel AndAlso e2 < conditions(y).MinDiffExpressionLevel Then
                                pvalues(previousGenomeSizes + j) = 1.0
                            End If
                        Next
                        previousGenomeSizes += genome.NumGenes()
                    Next

                    mergesort(pvalues, indices, genomeIndices, 0, totalGenes - 1)

                    Dim previous_BH_value As Double = 0.0
                    For k As Integer = 0 To pvalues.Length - 1
                        Dim BH_value As Double = pvalues(k) * totalGenes / (k + 1)
                        BH_value = System.Math.Min(BH_value, 1.0)
                        BH_value = System.Math.Max(BH_value, previous_BH_value)
                        previous_BH_value = BH_value
                        genomes(genomeIndices(k)).GetGene(indices(k)).qValues(pValue_index) = BH_value
                    Next
                    pValue_index += 1
                Next
            Next
        End Sub

        ''' <summary>
        ''' Mergesort parallel arrays a/b/c based on values in a.
        ''' </summary>
        Private Shared Sub mergesort(a As Double(), b As Integer(), c As Integer(), lo As Integer, hi As Integer)
            If lo < hi Then
                Dim q As Integer = (lo + hi) \ 2
                mergesort(a, b, c, lo, q)
                mergesort(a, b, c, q + 1, hi)
                merge(a, b, c, lo, q, hi)
            End If
        End Sub

        ''' <summary>
        ''' Mergesort helper method.
        ''' </summary>
        Private Shared Sub merge(a As Double(), b As Integer(), c As Integer(), lo As Integer, q As Integer, hi As Integer)
            Dim a1 As Double() = New Double(q - lo) {}
            Dim a2 As Double() = New Double(hi - q - 1) {}
            Dim b1 As Integer() = New Integer(q - lo) {}
            Dim b2 As Integer() = New Integer(hi - q - 1) {}
            Dim c1 As Integer() = New Integer(q - lo) {}
            Dim c2 As Integer() = New Integer(hi - q - 1) {}

            For i As Integer = 0 To a1.Length - 1
                a1(i) = a(lo + i) : b1(i) = b(lo + i) : c1(i) = c(lo + i)
            Next
            For j As Integer = 0 To a2.Length - 1
                a2(j) = a(q + 1 + j) : b2(j) = b(q + 1 + j) : c2(j) = c(q + 1 + j)
            Next

            Dim ii As Integer = 0
            Dim jj As Integer = 0
            For k As Integer = lo To hi
                If ii >= a1.Length Then
                    a(k) = a2(jj) : b(k) = b2(jj) : c(k) = c2(jj) : jj += 1
                ElseIf jj >= a2.Length Then
                    a(k) = a1(ii) : b(k) = b1(ii) : c(k) = c1(ii) : ii += 1
                ElseIf a1(ii) <= a2(jj) Then
                    a(k) = a1(ii) : b(k) = b1(ii) : c(k) = c1(ii) : ii += 1
                Else
                    a(k) = a2(jj) : b(k) = b2(jj) : c(k) = c2(jj) : jj += 1
                End If
            Next
        End Sub

#End Region

        ''' <summary>
        ''' Returns a String representation of a Gene.
        ''' </summary>
        Public Overrides Function ToString() As String
            Try
                Dim tStart As String = If(Me.StartT > 0, Me.StartT.ToString, "")
                Dim start As String = If(Me.Start > 0, Me.Start.ToString, "")
                Dim stopS As String = If(Me.[Stop] > 0, Me.[Stop].ToString, "")
                Dim tStop As String = If(Me.StopT > 0, Me.StopT.ToString, "")
                Return $"{tStart}{vbTab}{start}{vbTab}{stopS}{vbTab}{tStop}{vbTab}{Strand}{vbTab}{Name}{vbTab}{Synonym}{vbTab}{Product}"
            Catch ex As Exception
                Return Synonym
            End Try
        End Function

    End Class

End Namespace
