Imports Microsoft.VisualBasic.Math.Correlations
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame.MultipleOmics
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports GSVAEngine = SMRUCC.genomics.Analysis.HTS.GSVA.GSVA
Imports std = System.Math

Namespace Ablation

    ''' <summary>
    ''' 「拼接矩阵直接做一次单组学 GSVA」的消融对照结果。
    ''' </summary>
    Public Class PooledBaselineResult

        ''' <summary>拼接矩阵上的单组学 GSVA 分数（通路 × 样本）</summary>
        Public Property Scores As Matrix

        ''' <summary>与分组学融合结果逐通路的 Pearson 相关系数</summary>
        Public Property Correlation As Dictionary(Of String, Double) = New Dictionary(Of String, Double)(StringComparer.Ordinal)

        ''' <summary>与融合结果的逐元素平均绝对差</summary>
        Public Property MeanAbsoluteDifference As Double

        ''' <summary>参与比对的通路数</summary>
        Public Property ComparedPathways As Integer

        ''' <summary>对照计算失败时的错误信息</summary>
        Public Property ErrorMessage As String

        Public ReadOnly Property Available As Boolean
            Get
                Return Scores IsNot Nothing
            End Get
        End Property

        Public Overrides Function ToString() As String
            If Available Then
                Return $"pooled baseline: {Scores.size} pathway(s), mean|diff| vs fusion = {MeanAbsoluteDifference:E3}"
            Else
                Return $"pooled baseline: unavailable ({ErrorMessage})"
            End If
        End Function

    End Class

    ''' <summary>
    ''' 消融对照：把拼接矩阵当作单组学直接做 GSVA，用于量化「分组学打分 + 融合」的必要性。
    ''' </summary>
    ''' <remarks>
    ''' 这是 method.md 中与「组学内部估计」直接对立的朴素做法：
    ''' 不同组学的特征被放进同一个特征轴后，KDE 与秩变换会被量纲最大的组学主导。
    ''' 为了让基因集能够匹配到加前缀的拼接特征 id，
    ''' 需要把每条通路的成员按组学层展开成 <c>{omics}:{gene}</c>。
    ''' </remarks>
    Public Module PooledBaseline

        ''' <summary>
        ''' 在拼接矩阵上计算单组学 GSVA 分数。
        ''' </summary>
        Public Function Compute(dataset As Omics.MultiOmicsDataset,
                                geneSets As Background,
                                options As MultiOmicsGsvaOptions) As PooledBaselineResult
            Dim result As New PooledBaselineResult()

            If dataset Is Nothing OrElse geneSets Is Nothing Then
                result.ErrorMessage = "dataset or gene sets is nothing"

                Return result
            End If

            Try
                Dim pooled As Matrix = dataset.Pooled()
                Dim sets As Background = ExpandGeneSets(geneSets, dataset)
                Dim scores As Matrix = GSVAEngine.gsva(
                    pooled,
                    sets,
                    method:=options.ScoreMethod,
                    kcdf:=options.Kcdf,
                    min_sz:=options.MinSize,
                    max_sz:=options.MaxSize,
                    alpha:=options.SsGseaAlpha,
                    normalize:=options.SsGseaNormalize,
                    verbose:=False)

                If scores Is Nothing OrElse scores.size = 0 Then
                    result.ErrorMessage = "the pooled baseline produced no score"
                Else
                    result.Scores = scores
                End If
            Catch ex As Exception
                result.ErrorMessage = $"{ex.GetType.Name}: {ex.Message}"

                If options.Verbose Then
                    Call $"[moGSVA] pooled baseline failed: {result.ErrorMessage}".warning
                End If
            End Try

            Return result
        End Function

        ''' <summary>
        ''' 把拼接基线与融合结果做逐通路相关性诊断。
        ''' </summary>
        ''' <param name="pooled">拼接基线结果</param>
        ''' <param name="fused">分组学融合结果（通路 × 样本）</param>
        ''' <param name="verbose">是否输出诊断摘要</param>
        Public Function Diagnose(pooled As PooledBaselineResult,
                                 fused As Matrix,
                                 Optional verbose As Boolean = True) As PooledBaselineResult

            If pooled Is Nothing OrElse Not pooled.Available OrElse fused Is Nothing Then
                Return pooled
            End If

            Dim index As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To fused.size - 1
                Dim id As String = fused.expression(i).geneID

                If Not String.IsNullOrEmpty(id) AndAlso Not index.ContainsKey(id) Then
                    index(id) = i
                End If
            Next

            Dim sumDiff As Double = 0
            Dim count As Integer = 0

            For Each row As DataFrameRow In pooled.Scores.expression
                Dim i As Integer

                If Not index.TryGetValue(row.geneID, i) Then
                    Continue For
                End If

                Dim theirs As Double() = row.experiments
                Dim ours As Double() = fused.expression(i).experiments

                If theirs.Length <> ours.Length Then
                    Continue For
                End If

                For j As Integer = 0 To theirs.Length - 1
                    If Double.IsNaN(theirs(j)) OrElse Double.IsNaN(ours(j)) Then
                        Continue For
                    End If

                    sumDiff += std.Abs(theirs(j) - ours(j))
                    count += 1
                Next

                Dim r As Double = Correlations.GetPearson(theirs, ours)

                If Not Double.IsNaN(r) Then
                    pooled.Correlation(row.geneID) = r
                End If
            Next

            pooled.ComparedPathways = pooled.Correlation.Count
            pooled.MeanAbsoluteDifference = If(count > 0, sumDiff / count, Double.NaN)

            If verbose Then
                Call $"[moGSVA] {pooled}".info
            End If

            Return pooled
        End Function

        ''' <summary>
        ''' 把基因集成员按组学层展开为拼接矩阵的特征 id。
        ''' </summary>
        Public Function ExpandGeneSets(geneSets As Background, dataset As Omics.MultiOmicsDataset) As Background
            Dim clusters As New List(Of Cluster)
            Dim featureSets As Dictionary(Of String, HashSet(Of String)) = dataset.Layers _
                .ToDictionary(Function(l) l.Name,
                              Function(l) New HashSet(Of String)(l.Genes, StringComparer.Ordinal))

            For Each cluster As Cluster In geneSets.clusters
                If cluster Is Nothing OrElse String.IsNullOrEmpty(cluster.ID) Then
                    Continue For
                End If

                Dim members As New List(Of BackgroundGene)

                For Each layer As OmicsLayer In dataset.Layers
                    Dim features As HashSet(Of String) = featureSets(layer.Name)

                    For Each gene As BackgroundGene In If(cluster.members, New BackgroundGene() {})
                        Dim id As String = If(gene Is Nothing, Nothing, gene.accessionID)

                        If Not String.IsNullOrEmpty(id) AndAlso features.Contains(id) Then
                            members.Add(New BackgroundGene($"{layer.Name}:{id}"))
                        End If
                    Next
                Next

                If members.Count > 0 Then
                    clusters.Add(New Cluster With {
                        .ID = cluster.ID,
                        .names = cluster.names,
                        .description = cluster.description,
                        .members = members.ToArray
                    })
                End If
            Next

            Return New Background With {
                .id = geneSets.id,
                .name = geneSets.name,
                .comments = geneSets.comments,
                .build = geneSets.build,
                .clusters = clusters.ToArray
            }
        End Function
    End Module

End Namespace
