Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 单个节点的解释结果
''' </summary>
''' <remarks>
''' 这里同时保留了两类互补的信息：
'''
''' 1. **重要性** <see cref="RawScore"/> 与 <see cref="AdjustedScore"/>：
'''    跨样本聚合得到的恒正量，衡量"该节点对模型输出结果的影响力大小"；
''' 2. **方向性** <see cref="ActivationDifference"/>：
'''    tanh 激活值在两组样本之间的均值差异，衡量"该节点把样本往哪个方向推"。
'''
''' 二者解耦之后，模型才能同时回答"哪些节点重要"与"它们把样本往哪个方向起作用"这两个问题。
''' </remarks>
Public Class NodeImportance

    ''' <summary>
    ''' 节点所在的网络层下标，0 表示基因层
    ''' </summary>
    ''' <returns>层下标</returns>
    Public Property LayerIndex As Integer

    ''' <summary>
    ''' 节点所在的网络层名称
    ''' </summary>
    ''' <returns>层名称</returns>
    Public Property LayerName As String

    ''' <summary>
    ''' 节点在该层之中的下标
    ''' </summary>
    ''' <returns>节点下标</returns>
    Public Property NodeIndex As Integer

    ''' <summary>
    ''' 节点所对应的生物学实体名称（基因名或者通路名）
    ''' </summary>
    ''' <returns>节点名称</returns>
    Public Property NodeName As String

    ''' <summary>
    ''' 未经偏倚校正的原始重要性分数 <c>C_i,l = Σ_s |C_i,l^s|</c>
    ''' </summary>
    ''' <returns>恒正的重要性分数</returns>
    Public Property RawScore As Double

    ''' <summary>
    ''' 经过节点度归一化校正之后的重要性分数
    ''' </summary>
    ''' <returns>校正后的分数</returns>
    Public Property AdjustedScore As Double

    ''' <summary>
    ''' 节点的度（入度与出度之和）
    ''' </summary>
    ''' <returns>节点度</returns>
    Public Property Degree As Double

    ''' <summary>
    ''' 该节点在阴性样本（原发性）上的平均激活值
    ''' </summary>
    ''' <returns>平均激活值</returns>
    Public Property MeanActivationNegative As Double

    ''' <summary>
    ''' 该节点在阳性样本（转移性 / 耐药）上的平均激活值
    ''' </summary>
    ''' <returns>平均激活值</returns>
    Public Property MeanActivationPositive As Double

    ''' <summary>
    ''' 两组样本之间的激活差异，等于 <see cref="MeanActivationPositive"/> 减去 <see cref="MeanActivationNegative"/>
    ''' </summary>
    ''' <returns>激活差异，正数表示在阳性样本上激活更高</returns>
    Public Property ActivationDifference As Double

    ''' <summary>
    ''' 节点贡献分数在所有样本上的平均值（有符号）
    ''' </summary>
    ''' <returns>平均有符号贡献</returns>
    Public Property MeanSignedContribution As Double

    ''' <summary>
    ''' 生成节点解释结果的单行文本
    ''' </summary>
    ''' <returns>描述文本</returns>
    Public Overrides Function ToString() As String
        Return $"{LayerName}::{NodeName} score={AdjustedScore.ToString("F4")} (raw={RawScore.ToString("F4")}, degree={Degree.ToString("F0")}, Δa={ActivationDifference.ToString("F4")})"
    End Function

End Class

''' <summary>
''' 某一个基因上特定改变类型的重要性
''' </summary>
Public Class GeneAlterationImportance

    ''' <summary>
    ''' 基因名
    ''' </summary>
    ''' <returns>基因名</returns>
    Public Property GeneName As String

    ''' <summary>
    ''' 改变类型，取值为 mutation / amplification / deletion
    ''' </summary>
    ''' <returns>改变类型</returns>
    Public Property AlterationType As String

    ''' <summary>
    ''' 该改变类型在全部样本上的归因分数绝对值之和
    ''' </summary>
    ''' <returns>恒正的重要性分数</returns>
    Public Property Score As Double

    ''' <summary>
    ''' 生成单行描述文本
    ''' </summary>
    ''' <returns>描述文本</returns>
    Public Overrides Function ToString() As String
        Return $"{GeneName} ({AlterationType}) score={Score.ToString("F4")}"
    End Function

End Class

''' <summary>
''' 跨样本聚合归因分数并对节点度做偏倚校正的解释分析器
''' </summary>
''' <remarks>
''' <para>
''' **样本级 → 总分**：对全部 <c>n_s</c> 个样本求绝对值之和，得到节点的总重要性
''' <c>C_i,l = Σ_s |C_i,l^s|</c>。
''' 注意这是恒正的绝对量（衡量"该节点对结果的影响力大小"），
''' 而节点本身的激活 <c>a_i,l^s</c> 是有符号的（衡量"该节点对某样本是推向耐药还是推向原发"）。
''' </para>
''' <para>
''' **图结构偏倚校正**：一个通路节点若属于过多父通路（枢纽节点、过度注释），
''' 会通过更多路径汇聚归因分数，导致其重要性被系统性高估。
''' 论文给出的校正规则为：定义节点度 <c>d_i,l = fan_in + fan_out</c>，
''' 令 <c>μ</c>、<c>σ</c> 为全体节点度的均值与标准差，则
'''
''' <c>adjusted C_i,l = C_i,l / d_i,l</c> （当 <c>d_i,l &gt; μ + 5σ</c>），否则保持不变。
'''
''' 注意这里只惩罚极端枢纽（度超过均值 5 个标准差的节点，按度等比例缩分），
''' 普通节点不动 —— 这是一个温和的、只修剪分布长尾的校正，
''' 避免把"真正重要且连接多的枢纽"（例如 TP53 通路）误伤。
''' </para>
''' </remarks>
Public Class ImportanceAnalyzer

    ''' <summary>
    ''' 对归因结果做跨样本聚合与偏倚校正
    ''' </summary>
    ''' <param name="result">DeepLIFT 归因结果</param>
    ''' <returns>全部节点的解释结果列表，按照层下标与节点下标排列</returns>
    Public Shared Function Analyze(result As DeepLIFTResult) As List(Of NodeImportance)
        Dim model As PNETModel = result.Model
        Dim hierarchy As PathwayHierarchy = model.Hierarchy
        Dim l As Integer = result.LayerCount
        Dim n As Integer = result.SampleCount
        Dim list As New List(Of NodeImportance)()
        Dim degrees As Double()() = New Double(l - 1)() {}
        Dim allDegrees As New List(Of Double)()

        For i As Integer = 0 To l - 1
            degrees(i) = hierarchy.GetNodeDegrees(i)
            allDegrees.AddRange(degrees(i))
        Next

        Dim mu As Double = TensorOps.Mean(allDegrees.ToArray())
        Dim sigma As Double = TensorOps.StdDev(allDegrees.ToArray())
        Dim threshold As Double = mu + 5.0 * sigma

        For i As Integer = 0 To l - 1
            Dim names As String() = hierarchy.GetNodeNames(i)
            Dim layerName As String = hierarchy.GetLayerName(i)
            Dim size As Integer = names.Length
            Dim contributions As Double()() = result.LayerContributions(i)
            Dim activations As Double()() = result.LayerActivations(i)

            For j As Integer = 0 To size - 1
                Dim raw As Double = 0.0
                Dim signed As Double = 0.0
                Dim negSum As Double = 0.0
                Dim negCount As Integer = 0
                Dim posSum As Double = 0.0
                Dim posCount As Integer = 0

                For s As Integer = 0 To n - 1
                    Dim c As Double = contributions(s)(j)

                    raw += std.Abs(c)
                    signed += c

                    Dim label As Double = If(result.Labels Is Nothing, 0.0, result.Labels(s))

                    If label > 0.5 Then
                        posSum += activations(s)(j)
                        posCount += 1
                    Else
                        negSum += activations(s)(j)
                        negCount += 1
                    End If
                Next

                Dim degree As Double = degrees(i)(j)
                Dim adjusted As Double = If(degree > threshold, raw / degree, raw)
                Dim meanNeg As Double = If(negCount > 0, negSum / negCount, 0.0)
                Dim meanPos As Double = If(posCount > 0, posSum / posCount, 0.0)

                list.Add(New NodeImportance With {
                    .LayerIndex = i,
                    .LayerName = layerName,
                    .NodeIndex = j,
                    .NodeName = names(j),
                    .RawScore = raw,
                    .AdjustedScore = adjusted,
                    .Degree = degree,
                    .MeanActivationNegative = meanNeg,
                    .MeanActivationPositive = meanPos,
                    .ActivationDifference = meanPos - meanNeg,
                    .MeanSignedContribution = If(n > 0, signed / n, 0.0)
                })
            Next
        Next

        Return list
    End Function

    ''' <summary>
    ''' 取得重要性排名最高的若干个基因（即基因层节点）
    ''' </summary>
    ''' <param name="items">由 <see cref="Analyze"/> 得到的节点解释结果</param>
    ''' <param name="topN">返回的数量</param>
    ''' <returns>按照校正后分数降序排列的基因列表</returns>
    Public Shared Function TopGenes(items As IEnumerable(Of NodeImportance), Optional topN As Integer = 10) As List(Of NodeImportance)
        Return items _
            .Where(Function(x) x.LayerIndex = 0) _
            .OrderByDescending(Function(x) x.AdjustedScore) _
            .Take(topN) _
            .ToList()
    End Function

    ''' <summary>
    ''' 取得指定通路上重要性排名最高的若干个节点
    ''' </summary>
    ''' <param name="items">由 <see cref="Analyze"/> 得到的节点解释结果</param>
    ''' <param name="layerIndex">通路层下标，1 表示最精细的通路层</param>
    ''' <param name="topN">返回的数量</param>
    ''' <returns>按照校正后分数降序排列的通路列表</returns>
    Public Shared Function TopPathways(items As IEnumerable(Of NodeImportance), layerIndex As Integer,
                                       Optional topN As Integer = 10) As List(Of NodeImportance)
        Return items _
            .Where(Function(x) x.LayerIndex = layerIndex) _
            .OrderByDescending(Function(x) x.AdjustedScore) _
            .Take(topN) _
            .ToList()
    End Function

    ''' <summary>
    ''' 统计"基因 → 改变类型"的重要性，对应论文图 3 的 Sankey 图分析
    ''' </summary>
    ''' <param name="result">DeepLIFT 归因结果</param>
    ''' <param name="topN">返回的数量</param>
    ''' <returns>按照归因分数绝对值之和降序排列的改变类型列表</returns>
    ''' <remarks>
    ''' 论文通过该分析揭示了改变类型与基因的对应关系：
    ''' AR 主要由扩增驱动、TP53 主要由突变驱动、PTEN 主要由缺失驱动，
    ''' 并且发现拷贝数变异比突变携带的信息量更大。
    ''' </remarks>
    Public Shared Function TopAlterations(result As DeepLIFTResult, Optional topN As Integer = 15) As List(Of GeneAlterationImportance)
        Dim genes As String() = result.Model.Hierarchy.GeneNames
        Dim types As String() = {"mutation", "amplification", "deletion"}
        Dim list As New List(Of GeneAlterationImportance)()
        Dim n As Integer = result.SampleCount

        For g As Integer = 0 To genes.Length - 1
            For t As Integer = 0 To 2
                Dim sum As Double = 0.0

                For s As Integer = 0 To n - 1
                    sum += std.Abs(result.InputContributions(s)(g * 3 + t))
                Next

                list.Add(New GeneAlterationImportance With {
                    .GeneName = genes(g),
                    .AlterationType = types(t),
                    .Score = sum
                })
            Next
        Next

        Return list.OrderByDescending(Function(x) x.Score).Take(topN).ToList()
    End Function

    ''' <summary>
    ''' 生成可打印的解释报告
    ''' </summary>
    ''' <param name="result">DeepLIFT 归因结果</param>
    ''' <param name="topN">每一层输出的节点数量</param>
    ''' <returns>多行报告文本</returns>
    Public Shared Function PrintReport(result As DeepLIFTResult, Optional topN As Integer = 10) As String
        Dim items As List(Of NodeImportance) = Analyze(result)
        Dim sb As New System.Text.StringBuilder()

        Call sb.AppendLine($"DeepLIFT attribution on {result.SampleCount} samples")
        Call sb.AppendLine($"conservation error : {result.ConservationError().ToString("E3")}")
        Call sb.AppendLine()

        For l As Integer = 0 To result.LayerCount - 1
            Dim top As List(Of NodeImportance) = TopPathways(items, l, topN)

            Call sb.AppendLine($"--- layer {l}: {result.Model.Hierarchy.GetLayerName(l)} ---")

            For Each item As NodeImportance In top
                Call sb.AppendLine("  " & item.ToString())
            Next

            Call sb.AppendLine()
        Next

        Return sb.ToString()
    End Function

End Class
