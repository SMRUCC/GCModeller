Imports System.Globalization
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' 表达数据的类型，对应 KOBAS <c>snbea.get_exset</c> 的 <c>-t/--datype</c> 参数
    ''' （<c>ma</c> 微阵列、<c>rseq</c> 转录组测序）。
    ''' </summary>
    Public Enum ExpressionDataType
        Microarray
        RnaSeq
    End Enum

    ''' <summary>
    ''' 基因调控网络（GRN）的一条有向边。
    ''' </summary>
    Public Class GeneRelation

        ''' <summary>
        ''' 起始基因。
        ''' </summary>
        ''' <returns></returns>
        Public Property FromGene As String

        ''' <summary>
        ''' 目标基因。
        ''' </summary>
        ''' <returns></returns>
        Public Property ToGene As String

        ''' <summary>
        ''' 关系类型（激活 / 抑制 等）。
        ''' </summary>
        ''' <returns></returns>
        Public Property RelationType As String

        Public Overrides Function ToString() As String
            Return $"{FromGene} -[{RelationType}]-> {ToGene}"
        End Function
    End Class

    ''' <summary>
    ''' SNBEA 分析所需要的数据准备工具（KOBAS <c>src/kobas/snbea.py</c> 之中
    ''' 纯数据准备部分的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' 与 KOBAS 原实现的差异：
    ''' <list type="bullet">
    ''' <item>KOBAS 通过 rpy2 调用 <c>edgeR::cpm</c> 与 <c>EnrichmentBrowser::normalize</c>，
    ''' 这里分别用本地的对数 CPM 与分位数归一化实现替代。</item>
    ''' <item>基因集尺寸过滤统一为闭区间（KOBAS 为严格不等号）。</item>
    ''' </list>
    ''' </remarks>
    Public Module SnbeaData

        ''' <summary>
        ''' 读取基因调控网络文件（制表符分隔，列依次为 FROM / TO / TYPE）。
        ''' </summary>
        ''' <param name="grnFile">GRN 文件路径。</param>
        ''' <returns>网络边集合。</returns>
        ''' <exception cref="FormatException">出现列数不足的行时抛出。</exception>
        Public Function ReadGrn(grnFile As String) As GeneRelation()
            Dim edges As New List(Of GeneRelation)

            For Each line As String In File.ReadLines(grnFile)
                Dim text As String = If(line, "").TrimEnd()

                If text.Length = 0 OrElse text.StartsWith("#") Then
                    Continue For
                End If

                Dim words As String() = text.Split(ControlChars.Tab)

                If words.Length < 3 Then
                    Throw New FormatException($"Bad GRN format at line: {text}. Expect 3 tab separated columns.")
                End If

                edges.Add(New GeneRelation With {
                    .FromGene = words(0).Trim(),
                    .ToGene = words(1).Trim(),
                    .RelationType = words(2).Trim()
                })
            Next

            Return edges.ToArray
        End Function

        ''' <summary>
        ''' 拆分表型分组的下标。
        ''' </summary>
        ''' <param name="pheno">表型分组。</param>
        ''' <returns>
        ''' 元组：<c>Group0</c> 为标签 0 的样本下标，<c>Group1</c> 为其余样本的下标。
        ''' </returns>
        <Extension>
        Public Function Groups(pheno As phenotype) As (Group0 As Integer(), Group1 As Integer())
            Dim labels As Integer() = pheno.labels
            Dim group0 As Integer() = Enumerable.Range(0, labels.Length).Where(Function(i) labels(i) = 0).ToArray
            Dim group1 As Integer() = Enumerable.Range(0, labels.Length).Where(Function(i) labels(i) <> 0).ToArray

            Return (group0, group1)
        End Function

        ''' <summary>
        ''' 按数据类型做表达矩阵预变换（KOBAS <c>get_exset</c> 的等价实现）。
        ''' </summary>
        ''' <param name="expr">原始表达矩阵。</param>
        ''' <param name="dataType">数据类型。</param>
        ''' <param name="normalize">
        ''' 是否执行归一化：微阵列数据做分位数归一化，转录组数据做带先验计数的对数 CPM；
        ''' 为 <c>False</c> 时转录组数据只做 <c>log2(x + 1)</c>。
        ''' </param>
        ''' <param name="priorCount">对数 CPM 的先验计数，默认 3（与 KOBAS 一致）。</param>
        ''' <returns>变换之后的表达矩阵。</returns>
        Public Function Prepare(expr As Matrix,
                                dataType As ExpressionDataType,
                                Optional normalize As Boolean = False,
                                Optional priorCount As Double = 3) As Matrix

            If dataType = ExpressionDataType.RnaSeq Then
                Return If(normalize,
                          CpmLog(expr, priorCount),
                          Log2(expr, 1))
            End If

            Return If(normalize, QuantileNormalize(expr), CopyOf(expr))
        End Function

        ''' <summary>
        ''' 逐元素执行 <c>log2(x + offset)</c>。
        ''' </summary>
        ''' <param name="expr">表达矩阵。</param>
        ''' <param name="offset">对数变换之前的平移量，默认 1。</param>
        ''' <returns>新的表达矩阵。</returns>
        Public Function Log2(expr As Matrix, Optional offset As Double = 1) As Matrix
            Dim rows As DataFrameRow() = expr.expression _
                .Select(Function(row)
                            Return New DataFrameRow With {
                                .geneID = row.geneID,
                                .experiments = row.experiments _
                                    .Select(Function(x) std.Log(x + offset, 2)) _
                                    .ToArray
                            }
                        End Function) _
                .ToArray

            Return New Matrix With {
                .tag = expr.tag,
                .sampleID = expr.sampleID,
                .expression = rows
            }
        End Function

        ''' <summary>
        ''' 计算带先验计数的对数 CPM（对应 <c>edgeR::cpm(log = TRUE, prior.count = 3)</c>）。
        ''' </summary>
        ''' <param name="expr">原始计数矩阵。</param>
        ''' <param name="priorCount">先验计数，默认 3。</param>
        ''' <returns>新的表达矩阵。</returns>
        Public Function CpmLog(expr As Matrix, Optional priorCount As Double = 3) As Matrix
            Dim samples As Integer = expr.sampleID.Length
            Dim genes As Integer = expr.size
            Dim librarySize As Double() = New Double(samples - 1) {}

            For s As Integer = 0 To samples - 1
                Dim total As Double = 0

                For g As Integer = 0 To genes - 1
                    total += expr.expression(g).experiments(s)
                Next

                librarySize(s) = total
            Next

            Dim meanLibrarySize As Double = 0

            For s As Integer = 0 To samples - 1
                meanLibrarySize += librarySize(s)
            Next

            meanLibrarySize = If(samples = 0, 0, meanLibrarySize / samples)

            Dim rows As New List(Of DataFrameRow)

            For g As Integer = 0 To genes - 1
                Dim row As DataFrameRow = expr.expression(g)
                Dim values As Double() = New Double(samples - 1) {}

                For s As Integer = 0 To samples - 1
                    If librarySize(s) <= 0 OrElse meanLibrarySize <= 0 Then
                        values(s) = 0
                    Else
                        Dim prior As Double = priorCount * librarySize(s) / meanLibrarySize

                        values(s) = std.Log((row.experiments(s) + prior) / librarySize(s) * 1000000.0, 2)
                    End If
                Next

                rows.Add(New DataFrameRow With {
                    .geneID = row.geneID,
                    .experiments = values
                })
            Next

            Return New Matrix With {
                .tag = expr.tag,
                .sampleID = expr.sampleID,
                .expression = rows.ToArray
            }
        End Function

        ''' <summary>
        ''' 分位数归一化（对应 <c>EnrichmentBrowser::normalize</c> 的分位数方法）。
        ''' </summary>
        ''' <param name="expr">表达矩阵。</param>
        ''' <returns>新的表达矩阵。</returns>
        Public Function QuantileNormalize(expr As Matrix) As Matrix
            Dim samples As Integer = expr.sampleID.Length
            Dim genes As Integer = expr.size

            If genes = 0 OrElse samples = 0 Then
                Return CopyOf(expr)
            End If

            Dim values As Double()() = expr.expression _
                .Select(Function(row) DirectCast(row.experiments.Clone(), Double())) _
                .ToArray
            Dim orders As Integer()() = New Integer(samples - 1)() {}

            For s As Integer = 0 To samples - 1
                Dim column As Integer = s

                orders(s) = Enumerable.Range(0, genes) _
                    .OrderBy(Function(g) values(g)(column)) _
                    .ToArray
            Next

            Dim means As Double() = New Double(genes - 1) {}

            For rank As Integer = 0 To genes - 1
                Dim sum As Double = 0

                For s As Integer = 0 To samples - 1
                    sum += values(orders(s)(rank))(s)
                Next

                means(rank) = sum / samples
            Next

            For s As Integer = 0 To samples - 1
                For rank As Integer = 0 To genes - 1
                    values(orders(s)(rank))(s) = means(rank)
                Next
            Next

            Dim rows As DataFrameRow() = expr.expression _
                .Select(Function(row, g)
                            Return New DataFrameRow With {
                                .geneID = row.geneID,
                                .experiments = values(g)
                            }
                        End Function) _
                .ToArray

            Return New Matrix With {
                .tag = expr.tag,
                .sampleID = expr.sampleID,
                .expression = rows
            }
        End Function

        ''' <summary>
        ''' 复制一个表达矩阵（只复制数值，避免后续变换污染原始数据）。
        ''' </summary>
        ''' <param name="expr">表达矩阵。</param>
        ''' <returns></returns>
        Public Function CopyOf(expr As Matrix) As Matrix
            Return New Matrix With {
                .tag = expr.tag,
                .sampleID = expr.sampleID,
                .expression = expr.expression _
                    .Select(Function(row)
                                Return New DataFrameRow With {
                                    .geneID = row.geneID,
                                    .experiments = DirectCast(row.experiments.Clone(), Double())
                                }
                            End Function) _
                    .ToArray
            }
        End Function

        ''' <summary>
        ''' 把表达矩阵的基因名映射到基因集成员之上，返回「基因集 → 行下标」的映射。
        ''' </summary>
        ''' <param name="expr">表达矩阵。</param>
        ''' <param name="background">基因集背景模型。</param>
        ''' <param name="minSize">基因集可用成员数下限（闭区间），默认 5。</param>
        ''' <param name="maxSize">基因集可用成员数上限（闭区间），默认 500。</param>
        ''' <returns>基因集编号 → 行下标数组（按矩阵行序升序）。</returns>
        <Extension>
        Public Function MapGeneSets(expr As Matrix,
                                    background As Background,
                                    Optional minSize As Integer = 5,
                                    Optional maxSize As Integer = 500) As Dictionary(Of String, Integer())

            Dim index As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To expr.expression.Length - 1
                Dim id As String = expr.expression(i).geneID

                If Not String.IsNullOrEmpty(id) AndAlso Not index.ContainsKey(id) Then
                    index(id) = i
                End If
            Next

            Dim result As New Dictionary(Of String, Integer())(StringComparer.Ordinal)

            For Each cluster As Cluster In background.clusters
                Dim rows As New List(Of Integer)

                For Each gene As BackgroundGene In If(cluster.members, New BackgroundGene() {})
                    Dim position As Integer

                    If Not String.IsNullOrEmpty(gene.accessionID) AndAlso index.TryGetValue(gene.accessionID, position) Then
                        rows.Add(position)
                    End If
                Next

                Dim distinct As Integer() = rows.Distinct().OrderBy(Function(i) i).ToArray

                If distinct.Length >= minSize AndAlso distinct.Length <= maxSize Then
                    result(cluster.ID) = distinct
                End If
            Next

            Return result
        End Function
    End Module
End Namespace
