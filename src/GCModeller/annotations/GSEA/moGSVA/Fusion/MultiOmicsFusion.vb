Imports System.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace Fusion

    ''' <summary>
    ''' 多组学融合结果：融合分数 + 各组学分项 + 权重 + 贡献分解 + 一致性。
    ''' </summary>
    Public Class MultiOmicsFusionResult

        ''' <summary>样本轴</summary>
        Public Property SampleID As String()

        ''' <summary>通路顺序（与所有分数矩阵的行顺序一致）</summary>
        Public Property Pathways As String()

        ''' <summary>最终融合分数 ES_multi（通路 × 样本）</summary>
        Public Property Scores As Matrix

        ''' <summary>加权平均分数 ES_raw（施加一致性奖励之前，通路 × 样本）</summary>
        Public Property RawScores As Matrix

        ''' <summary>符号一致度 a_j（通路 × 样本）</summary>
        Public Property Consistency As Matrix

        ''' <summary>逐层组学特异分数 ES_k（omics → 通路 × 样本）</summary>
        Public Property LayerScores As Dictionary(Of String, Matrix)

        ''' <summary>逐通路的组学权重（pathway → omics → w，Σw = 1）</summary>
        Public Property Weights As Dictionary(Of String, Dictionary(Of String, Double))

        ''' <summary>逐通路的组学贡献 c_k = w_k · ES_k（pathway → omics → 样本向量）</summary>
        Public Property Contributions As Dictionary(Of String, Dictionary(Of String, Double()))

        ''' <summary>逐通路的组学贡献占比（按 |c_k| 归一化，pathway → omics → 样本向量）</summary>
        Public Property ContributionShare As Dictionary(Of String, Dictionary(Of String, Double()))

        ''' <summary>每个样本上贡献绝对值最大的组学（pathway → 样本向量）</summary>
        Public Property DominantOmics As Dictionary(Of String, String())

        ''' <summary>逐通路的组学成员数 |S_k|（pathway → omics → count）</summary>
        Public Property Coverage As Dictionary(Of String, Dictionary(Of String, Integer))

        ''' <summary>逐通路被跳过的组学层（该层没有这条通路的分数）</summary>
        Public Property SkippedLayers As Dictionary(Of String, String())

        Private m_index As Dictionary(Of String, Integer)

        Public Function HasPathway(pathway As String) As Boolean
            Call EnsureIndex()

            Return m_index.ContainsKey(pathway)
        End Function

        ''' <summary>取某条通路在第 i 个样本上的融合分数</summary>
        Public Function ScoreOf(pathway As String, i As Integer) As Double
            Call EnsureIndex()

            Dim row As Integer

            If Not m_index.TryGetValue(pathway, row) Then
                Return Double.NaN
            End If

            Return Scores.expression(row).experiments(i)
        End Function

        ''' <summary>取某条通路在各样本上的融合分数向量</summary>
        Public Function ScoreVector(pathway As String) As Double()
            Call EnsureIndex()

            Dim row As Integer

            If Not m_index.TryGetValue(pathway, row) Then
                Return Nothing
            End If

            Return Scores.expression(row).experiments
        End Function

        ''' <summary>取某条通路的组学权重，缺失时返回 0</summary>
        Public Function WeightOf(pathway As String, omics As String) As Double
            Dim perPathway As Dictionary(Of String, Double) = Nothing

            If Weights IsNot Nothing AndAlso Weights.TryGetValue(pathway, perPathway) Then
                Dim w As Double

                If perPathway.TryGetValue(omics, w) Then
                    Return w
                End If
            End If

            Return 0
        End Function

        Private Sub EnsureIndex()
            If m_index IsNot Nothing Then
                Return
            End If

            m_index = New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            If Scores Is Nothing Then
                Return
            End If

            For i As Integer = 0 To Scores.size - 1
                Dim id As String = Scores.expression(i).geneID

                If Not String.IsNullOrEmpty(id) AndAlso Not m_index.ContainsKey(id) Then
                    m_index(id) = i
                End If
            Next
        End Sub

    End Class

    ''' <summary>
    ''' 组学加权融合的统一入口（method.md 步骤4）。
    ''' </summary>
    Public Module MultiOmicsFusion

        ''' <summary>
        ''' 把逐组学分数融合为样本级多组学通路活性。
        ''' </summary>
        ''' <param name="scoreSet">逐层打分结果</param>
        ''' <param name="options">运行配置（融合策略、λ、平衡项）</param>
        Public Function Compute(scoreSet As PerOmics.OmicsScoreSet,
                                options As MultiOmicsGsvaOptions) As MultiOmicsFusionResult

            If scoreSet Is Nothing Then
                Throw New ArgumentNullException(NameOf(scoreSet))
            End If
            If options Is Nothing Then
                Throw New ArgumentNullException(NameOf(options))
            End If

            Dim sampleID As String() = scoreSet.SampleID
            Dim nSample As Integer = sampleID.Length
            Dim lambda As Double = std.Max(0, options.ConsistencyLambda)

            Dim scoreRows As New List(Of DataFrameRow)
            Dim rawRows As New List(Of DataFrameRow)
            Dim consRows As New List(Of DataFrameRow)
            Dim weights As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.Ordinal)
            Dim contributions As New Dictionary(Of String, Dictionary(Of String, Double()))(StringComparer.Ordinal)
            Dim shares As New Dictionary(Of String, Dictionary(Of String, Double()))(StringComparer.Ordinal)
            Dim dominant As New Dictionary(Of String, String())(StringComparer.Ordinal)
            Dim coverage As New Dictionary(Of String, Dictionary(Of String, Integer))(StringComparer.Ordinal)
            Dim skipped As New Dictionary(Of String, String())(StringComparer.Ordinal)

            For Each pathway As String In scoreSet.Pathways
                Dim vectors As New Dictionary(Of String, Double())(StringComparer.Ordinal)
                Dim members As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
                Dim absent As New List(Of String)

                For Each table As PerOmics.OmicsScoreTable In scoreSet.Tables
                    Dim v As Double() = table.ScoreVector(pathway)

                    If v Is Nothing Then
                        absent.Add(table.Omics)
                    Else
                        vectors(table.Omics) = v
                        members(table.Omics) = table.MemberCount(pathway)
                    End If
                Next

                If vectors.Count = 0 Then
                    Continue For
                End If

                Dim w As Dictionary(Of String, Double) = WeightCalculators.Compute(vectors, members, options.FusionStrategy, options.BalanceFeatureSize)
                Dim ordered As String() = vectors.Keys.ToArray
                Dim stack As Double()() = New Double(ordered.Length - 1)() {}

                For i As Integer = 0 To ordered.Length - 1
                    stack(i) = vectors(ordered(i))
                Next

                Dim raw As Double() = New Double(nSample - 1) {}
                Dim consensus As Double() = ConsistencyReward.SampleAgreement(stack)

                For j As Integer = 0 To nSample - 1
                    Dim sum As Double = 0

                    For i As Integer = 0 To ordered.Length - 1
                        sum += w(ordered(i)) * stack(i)(j)
                    Next

                    raw(j) = sum
                Next

                Dim fused As Double() = ConsistencyReward.Apply(raw, consensus, lambda)

                ' 贡献分解：c_k = w_k * ES_k，占比按 |c_k| 归一化
                Dim contribution As New Dictionary(Of String, Double())(StringComparer.Ordinal)
                Dim share As New Dictionary(Of String, Double())(StringComparer.Ordinal)
                Dim dominantOmics As String() = New String(nSample - 1) {}

                For Each omics As String In ordered
                    Dim c As Double() = New Double(nSample - 1) {}

                    For j As Integer = 0 To nSample - 1
                        c(j) = w(omics) * vectors(omics)(j)
                    Next

                    contribution(omics) = c
                Next

                For j As Integer = 0 To nSample - 1
                    Dim total As Double = 0
                    Dim bestName As String = Nothing
                    Dim bestValue As Double = -1

                    For Each omics As String In ordered
                        Dim absValue As Double = std.Abs(contribution(omics)(j))

                        total += absValue

                        If absValue > bestValue Then
                            bestValue = absValue
                            bestName = omics
                        End If
                    Next

                    For Each omics As String In ordered
                        Dim s As Double

                        If total > 0 Then
                            s = contribution(omics)(j) / total
                        Else
                            s = 0
                        End If

                        Dim buffer As Double() = Nothing

                        If Not share.TryGetValue(omics, buffer) Then
                            buffer = New Double(nSample - 1) {}
                            share(omics) = buffer
                        End If

                        buffer(j) = s
                    Next

                    dominantOmics(j) = bestName
                Next

                scoreRows.Add(New DataFrameRow With {.geneID = pathway, .experiments = fused})
                rawRows.Add(New DataFrameRow With {.geneID = pathway, .experiments = raw})
                consRows.Add(New DataFrameRow With {.geneID = pathway, .experiments = consensus})
                weights(pathway) = w
                contributions(pathway) = contribution
                shares(pathway) = share
                dominant(pathway) = dominantOmics
                coverage(pathway) = members
                skipped(pathway) = absent.ToArray()
            Next

            Dim layerScores As New Dictionary(Of String, Matrix)(StringComparer.Ordinal)

            For Each table As PerOmics.OmicsScoreTable In scoreSet.Tables
                layerScores(table.Omics) = table.Scores
            Next

            Dim pathways As String() = scoreRows.Select(Function(r) r.geneID).ToArray()

            Return New MultiOmicsFusionResult With {
                .SampleID = sampleID,
                .Pathways = pathways,
                .Scores = New Matrix With {
                    .sampleID = sampleID,
                    .tag = "moGSVA:ES_multi",
                    .expression = scoreRows.ToArray
                },
                .RawScores = New Matrix With {
                    .sampleID = sampleID,
                    .tag = "moGSVA:ES_raw",
                    .expression = rawRows.ToArray
                },
                .Consistency = New Matrix With {
                    .sampleID = sampleID,
                    .tag = "moGSVA:consistency",
                    .expression = consRows.ToArray
                },
                .LayerScores = layerScores,
                .Weights = weights,
                .Contributions = contributions,
                .ContributionShare = shares,
                .DominantOmics = dominant,
                .Coverage = coverage,
                .SkippedLayers = skipped
            }
        End Function

    End Module

End Namespace
