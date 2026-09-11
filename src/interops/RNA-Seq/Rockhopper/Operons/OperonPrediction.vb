' /********************************************************************************/
'
'  Rockhopper —— 操纵子预测
'
'  复刻论文与 readme.md 第 6 节：原核生物中相邻且同链的基因可能被共同转录为一个
'  多顺反子 mRNA。Rockhopper 综合两个特征估计相邻基因共转录的概率：
'    * 基因间距离（intergenic distance）—— 越近越可能是同一操纵子；
'    * 表达谱相似度（expression similarity）—— 同一操纵子的基因表达应高度同步。
'  再用贝叶斯框架把两类信息整合为共转录后验概率。
'
'  实现：
'    P(operon | d, s) ∝ P(d | operon) · P(s | operon)
'    其中
'      P(d | operon)   = 1 / (1 + exp((d - D0) / K))          距离 sigmoid（D0=40, K=15）
'      P(d | ¬operon)  = 1 - P(d | operon)
'      P(s | operon)   = max(similarity, ε)
'      P(s | ¬operon)  = 1 - max(similarity, ε)
'    等先验下后验 = (w1·w2) / (w1·w2 + (1-w1)(1-w2))
'  当后验概率 ≥ 阈值时把该基因对并入同一操纵子。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports Core

Namespace Operons

    ''' <summary>
    ''' 操纵子预测参数。
    ''' </summary>
    Public Class OperonPredictionOptions

        ''' <summary>距离 sigmoid 的中点（bp）。</summary>
        Public Property DistanceMidpoint As Double = 40.0
        ''' <summary>距离 sigmoid 的斜率尺度。</summary>
        Public Property DistanceScale As Double = 15.0
        ''' <summary>判定为同一操纵子的后验概率阈值。</summary>
        Public Property ProbabilityThreshold As Double = 0.5
        ''' <summary>参与比较的基因的最小表达量（RPKM），低于该值的基因不参与相似度计算。</summary>
        Public Property MinExpression As Double = 0.0

    End Class

    ''' <summary>
    ''' 操纵子预测器。
    ''' </summary>
    Public Module OperonPrediction

        ''' <summary>
        ''' 预测单个基因组上的操纵子。
        ''' </summary>
        ''' <param name="genome">基因组（基因表达量应已完成定量）。</param>
        ''' <param name="conditionCount">实验条件数量（表达向量维度）。</param>
        ''' <param name="options">预测参数。</param>
        Public Function Predict(genome As Genome, conditionCount As Integer,
                                Optional options As OperonPredictionOptions = Nothing) As List(Of Operon)
            If options Is Nothing Then options = New OperonPredictionOptions()

            Dim operons As New List(Of Operon)()
            Dim genes As List(Of Gene) = genome.Genes
            If genes.Count = 0 Then Return operons

            ' 计算相邻同链基因对的共转录概率
            Dim pairs As New List(Of (index As Integer, pair As OperonGenePair))()
            For i As Integer = 0 To genes.Count - 2
                Dim a As Gene = genes(i)
                Dim b As Gene = genes(i + 1)
                If a.Strand <> b.Strand Then Continue For

                Dim distance As Integer = System.Math.Max(0, b.First - a.Last - 1)
                Dim similarity As Double = expressionSimilarity(a, b, conditionCount)

                Dim pDistance As Double = distanceProbability(distance, options)
                Dim pSimilarity As Double = System.Math.Max(similarity, 0.0001)
                Dim w1 As Double = pDistance
                Dim w2 As Double = pSimilarity
                Dim denom As Double = w1 * w2 + (1.0 - w1) * (1.0 - w2)
                Dim posterior As Double = If(denom <= 0.0, 0.0, (w1 * w2) / denom)

                pairs.Add((i, New OperonGenePair With {
                    .Gene1 = a.Synonym,
                    .Gene2 = b.Synonym,
                    .Distance = distance,
                    .ExpressionSimilarity = similarity,
                    .Probability = posterior,
                    .IsOperon = posterior >= options.ProbabilityThreshold
                }))
            Next

            ' 合并连续的共转录基因对为一个操纵子
            Dim current As List(Of Integer) = Nothing
            For Each item In pairs
                If item.pair.IsOperon Then
                    If current Is Nothing Then
                        current = New List(Of Integer) From {item.index, item.index + 1}
                    ElseIf current.Last() = item.index Then
                        current.Add(item.index + 1)
                    Else
                        operons.Add(buildOperon(genes, current))
                        current = New List(Of Integer) From {item.index, item.index + 1}
                    End If
                Else
                    If current IsNot Nothing Then
                        operons.Add(buildOperon(genes, current))
                        current = Nothing
                    End If
                End If
            Next
            If current IsNot Nothing Then operons.Add(buildOperon(genes, current))

            ' 编号（DOOR 风格）
            For i As Integer = 0 To operons.Count - 1
                If String.IsNullOrEmpty(operons(i).OperonID) Then
                    operons(i).OperonID = $"OP{i + 1:0000}"
                End If
            Next

            Return operons
        End Function

        ''' <summary>
        ''' 输出全部相邻基因对的共转录证据（对应 operonGenePairs.txt）。
        ''' </summary>
        Public Function GenePairs(genome As Genome, conditionCount As Integer,
                                  Optional options As OperonPredictionOptions = Nothing) As List(Of OperonGenePair)
            If options Is Nothing Then options = New OperonPredictionOptions()
            Dim genes As List(Of Gene) = genome.Genes
            Dim result As New List(Of OperonGenePair)()

            For i As Integer = 0 To genes.Count - 2
                Dim a As Gene = genes(i)
                Dim b As Gene = genes(i + 1)
                If a.Strand <> b.Strand Then Continue For

                Dim distance As Integer = System.Math.Max(0, b.First - a.Last - 1)
                Dim similarity As Double = expressionSimilarity(a, b, conditionCount)
                Dim pDistance As Double = distanceProbability(distance, options)
                Dim w1 As Double = pDistance
                Dim w2 As Double = System.Math.Max(similarity, 0.0001)
                Dim denom As Double = w1 * w2 + (1.0 - w1) * (1.0 - w2)
                Dim posterior As Double = If(denom <= 0.0, 0.0, (w1 * w2) / denom)

                result.Add(New OperonGenePair With {
                    .Gene1 = a.Synonym,
                    .Gene2 = b.Synonym,
                    .Distance = distance,
                    .ExpressionSimilarity = similarity,
                    .Probability = posterior,
                    .IsOperon = posterior >= options.ProbabilityThreshold
                })
            Next

            Return result
        End Function

        ''' <summary>距离 sigmoid：距离越近，共转录先验越高。</summary>
        Public Function distanceProbability(distance As Integer, options As OperonPredictionOptions) As Double
            Return 1.0 / (1.0 + System.Math.Exp((distance - options.DistanceMidpoint) / options.DistanceScale))
        End Function

        ''' <summary>
        ''' 跨条件的表达谱相似度（皮尔逊相关系数）。
        ''' 当任一基因在所有条件下表达均近似恒定（方差为 0）时，退化为相对表达水平的相似度。
        ''' </summary>
        Public Function expressionSimilarity(a As Gene, b As Gene, conditionCount As Integer) As Double
            If conditionCount <= 0 Then Return 0.0

            Dim x As New List(Of Double)()
            Dim y As New List(Of Double)()
            For c As Integer = 0 To conditionCount - 1
                x.Add(CDbl(a.GetRPKM(c)))
                y.Add(CDbl(b.GetRPKM(c)))
            Next

            If x.Count < 2 Then Return 0.0

            Dim meanX As Double = x.Average()
            Dim meanY As Double = y.Average()
            Dim cov As Double = 0.0
            Dim varX As Double = 0.0
            Dim varY As Double = 0.0
            For i As Integer = 0 To x.Count - 1
                Dim dx As Double = x(i) - meanX
                Dim dy As Double = y(i) - meanY
                cov += dx * dy
                varX += dx * dx
                varY += dy * dy
            Next

            If varX <= 0.0 OrElse varY <= 0.0 Then
                ' 表达恒定：以水平接近程度作为相似度
                Dim maxV As Double = System.Math.Max(meanX, meanY)
                If maxV <= 0.0 Then Return 0.0
                Return 1.0 - System.Math.Abs(meanX - meanY) / maxV
            End If

            Return cov / System.Math.Sqrt(varX * varY)
        End Function

        Private Function buildOperon(genes As List(Of Gene), indices As List(Of Integer)) As Operon
            Dim members As Gene() = indices.Select(Function(i) genes(i)).ToArray()
            Dim first As Gene = members.First
            Dim lastGene As Gene = members.Last

            Return New Operon With {
                .Start = System.Math.Min(first.First, lastGene.Last),
                .[Stop] = System.Math.Max(first.First, lastGene.Last),
                .Strand = If(first.Strand = "-"c, "-", "+"),
                .Genes = members.Select(Function(g) g.Synonym).ToArray()
            }
        End Function

    End Module

End Namespace
