Imports System.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace Global.SMRUCC.genomics.Analysis.HTS.GSVA

    ''' <summary>
    ''' 多组学 GSVA 的完整结果。
    ''' </summary>
    ''' <remarks>
    ''' 三个标准输出对齐 method.md 的建议：
    ''' (1) 样本 × 通路评分矩阵 <see cref="Scores"/>；
    ''' (2) 通路 × 组学贡献分解 <see cref="Contributions"/> / <see cref="ContributionShare"/>；
    ''' (3) 通路显著性 p 值表 <see cref="PValues"/> / <see cref="QValues"/>。
    ''' </remarks>
    Public Class MultiOmicsGsvaResult

        ''' <summary>样本轴</summary>
        Public Property SampleID As String()

        ''' <summary>通路顺序</summary>
        Public Property Pathways As String()

        ''' <summary>融合后的通路活性矩阵 ES_multi（通路 × 样本）</summary>
        Public Property Scores As Matrix

        ''' <summary>加权平均分数 ES_raw（未施加一致性奖励）</summary>
        Public Property RawScores As Matrix

        ''' <summary>跨组学符号一致度（通路 × 样本）</summary>
        Public Property Consistency As Matrix

        ''' <summary>逐组学分项 ES_k（omics → 通路 × 样本）</summary>
        Public Property LayerScores As Dictionary(Of String, Matrix)

        ''' <summary>逐通路的组学权重</summary>
        Public Property Weights As Dictionary(Of String, Dictionary(Of String, Double))

        ''' <summary>逐通路的组学贡献 c_k = w_k · ES_k</summary>
        Public Property Contributions As Dictionary(Of String, Dictionary(Of String, Double()))

        ''' <summary>逐通路的组学贡献占比</summary>
        Public Property ContributionShare As Dictionary(Of String, Dictionary(Of String, Double()))

        ''' <summary>每个样本上主导的组学层</summary>
        Public Property DominantOmics As Dictionary(Of String, String())

        ''' <summary>逐通路的组学成员数 |S_k|</summary>
        Public Property Coverage As Dictionary(Of String, Dictionary(Of String, Integer))

        ''' <summary>逐通路被跳过的组学层</summary>
        Public Property SkippedLayers As Dictionary(Of String, String())

        ''' <summary>置换检验的完整结果（未启用时为 Nothing）</summary>
        Public Property Significance As StatTests.PermutationResult

        ''' <summary>逐通路 p 值（未启用置换时为 Nothing）</summary>
        Public Property PValues As Dictionary(Of String, Double)

        ''' <summary>逐通路 BH 校正后的 q 值</summary>
        Public Property QValues As Dictionary(Of String, Double)

        ''' <summary>拼接矩阵消融对照（未计算时为 Nothing）</summary>
        Public Property Pooled As Ablation.PooledBaselineResult

        ''' <summary>参与计算的组学层</summary>
        Public Property OmicsNames As String()

        ''' <summary>本次运行使用的配置</summary>
        Public Property Options As MultiOmicsGsvaOptions

        ''' <summary>总耗时（毫秒）</summary>
        Public Property ElapsedMilliseconds As Long

        Private m_index As Dictionary(Of String, Integer)

        ''' <summary>取某条通路在第 i 个样本上的融合分数</summary>
        Public Function ScoreOf(pathway As String, i As Integer) As Double
            Dim row As Integer = IndexOf(pathway)

            If row < 0 Then
                Return Double.NaN
            End If

            Return Scores.expression(row).experiments(i)
        End Function

        ''' <summary>取某条通路的融合分数向量</summary>
        Public Function ScoreVector(pathway As String) As Double()
            Dim row As Integer = IndexOf(pathway)

            If row < 0 Then
                Return Nothing
            End If

            Return Scores.expression(row).experiments
        End Function

        ''' <summary>取某条通路的 p 值，缺失时返回 NaN</summary>
        Public Function PValueOf(pathway As String) As Double
            Dim p As Double

            If PValues IsNot Nothing AndAlso PValues.TryGetValue(pathway, p) Then
                Return p
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>取某条通路的 q 值，缺失时返回 NaN</summary>
        Public Function QValueOf(pathway As String) As Double
            Dim q As Double

            If QValues IsNot Nothing AndAlso QValues.TryGetValue(pathway, q) Then
                Return q
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>把融合分数转置为「样本 × 通路」视图，便于下游与临床表型拼接</summary>
        Public Function SamplesByPathways() As Matrix
            Dim rows As DataFrameRow() = SampleID _
                .Select(Function(id, j)
                            Return New DataFrameRow With {
                                .geneID = id,
                                .experiments = Pathways.Select(Function(p) ScoreOf(p, j)).ToArray
                            }
                        End Function) _
                .ToArray

            Return New Matrix With {
                .sampleID = Pathways,
                .tag = "moGSVA:samples_by_pathways",
                .expression = rows
            }
        End Function

        Private Function IndexOf(pathway As String) As Integer
            If m_index Is Nothing Then
                m_index = New Dictionary(Of String, Integer)(StringComparer.Ordinal)

                If Scores IsNot Nothing Then
                    For i As Integer = 0 To Scores.size - 1
                        Dim id As String = Scores.expression(i).geneID

                        If Not String.IsNullOrEmpty(id) AndAlso Not m_index.ContainsKey(id) Then
                            m_index(id) = i
                        End If
                    Next
                End If
            End If

            Dim row As Integer

            If m_index.TryGetValue(pathway, row) Then
                Return row
            Else
                Return -1
            End If
        End Function

    End Class

End Namespace
