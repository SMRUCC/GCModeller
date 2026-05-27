

' ============================================================================
' 第七部分：Spearman + MIC 联合分析模块
' ============================================================================
'
' 联合分析策略:
'   Spearman 相关系数擅长检测单调关联（线性或单调非线性），
'   MIC 擅长检测任意关联（包括非单调非线性）。
'   两者结合可以：
'   1. 交集法（Intersection）: 两个指标同时显著才认为关联显著
'      → 高置信度，低假阳性，但可能遗漏非单调关联
'   2. 并集法（Union）: 任一指标显著即认为关联显著
'      → 高灵敏度，但假阳性较高
'   3. 加权法（Weighted）: 综合得分 = w1*|Spearman| + w2*MIC
'      → 平衡灵敏度和特异度
'   4. Fisher 合并法: 合并两个 p 值得到综合 p 值
'      → 统计上最严谨
' ============================================================================

''' <summary>
''' Spearman + MIC 联合分析结果
''' 包含 Spearman 相关系数、MIC 值、综合得分和显著性判定
''' </summary>
Public Class SpearmanMICResult

    ''' <summary>OTU feature ID 列表（行标签）</summary>
    Public OtuIds As String()
    ''' <summary>代谢物 feature ID 列表（列标签）</summary>
    Public MetaboliteIds As String()
    ''' <summary>相关性系数矩阵 [nOtu, nMet]，取值 [-1, 1]</summary>
    Public CorrelationMatrix As Double(,)
    ''' <summary>p 值矩阵 [nOtu, nMet]</summary>
    Public PValueMatrix As Double(,)
    ''' <summary>计算方法名称</summary>
    Public MethodName As String

    ''' <summary>Spearman 秩相关系数矩阵 [nOtu, nMet]，取值 [-1, 1]</summary>
    Public SpearmanMatrix As Double(,)
    ''' <summary>Spearman p 值矩阵 [nOtu, nMet]</summary>
    Public SpearmanPValueMatrix As Double(,)
    ''' <summary>MIC 值矩阵 [nOtu, nMet]，取值 [0, 1]</summary>
    Public MICMatrix As Double(,)
    ''' <summary>MIC p 值矩阵 [nOtu, nMet]</summary>
    Public MICPValueMatrix As Double(,)
    ''' <summary>综合得分矩阵 [nOtu, nMet]</summary>
    Public CombinedScoreMatrix As Double(,)
    ''' <summary>综合 p 值矩阵 [nOtu, nMet]（Fisher 合并法）</summary>
    Public CombinedPValueMatrix As Double(,)
    ''' <summary>显著性判定矩阵 [nOtu, nMet]</summary>
    Public IsSignificantMatrix As Boolean(,)
    ''' <summary>关联类型标注矩阵 [nOtu, nMet]：Monotonic / NonMonotonic / None</summary>
    Public AssociationTypeMatrix As String(,)
End Class

''' <summary>
''' Spearman + MIC 联合分析模块
''' 提供微生物组-代谢组数据的关联分析完整流程
''' </summary>
Public Module SpearmanMICCombined

    ''' <summary>
    ''' 联合分析方法枚举
    ''' </summary>
    Public Enum CombinationMethod
        ''' <summary>交集法：Spearman 和 MIC 均显著才判定关联</summary>
        Intersection = 0
        ''' <summary>并集法：Spearman 或 MIC 任一显著即判定关联</summary>
        [Union] = 1
        ''' <summary>加权法：综合得分 = w1*|Spearman| + w2*MIC</summary>
        Weighted = 2
        ''' <summary>Fisher 合并法：合并两个 p 值</summary>
        FisherCombined = 3
    End Enum

    ''' <summary>
    ''' Spearman + MIC 联合分析参数配置
    ''' </summary>
    Public Class SpearmanMICConfig
        ''' <summary>MIC 算法参数</summary>
        Public MICConfig As New MICComputation.MICConfig()
        ''' <summary>Spearman 相关系数绝对值阈值（默认 0.3）</summary>
        Public SpearmanThreshold As Double = 0.3
        ''' <summary>MIC 阈值（默认 0.3）</summary>
        Public MICThreshold As Double = 0.3
        ''' <summary>p 值显著性水平（默认 0.05）</summary>
        Public PValueThreshold As Double = 0.05
        ''' <summary>联合分析方法</summary>
        Public Method As CombinationMethod = CombinationMethod.FisherCombined
        ''' <summary>加权法中 Spearman 的权重（默认 0.5）</summary>
        Public WeightSpearman As Double = 0.5
        ''' <summary>加权法中 MIC 的权重（默认 0.5）</summary>
        Public WeightMIC As Double = 0.5
        ''' <summary>
        ''' 判定非单调关联的阈值：
        ''' 当 MIC > MICThreshold 且 |Spearman| < SpearmanThreshold 时，
        ''' 如果 MIC / (|Spearman| + epsilon) > NonMonotonicRatio，则判定为非单调关联
        ''' </summary>
        Public NonMonotonicRatio As Double = 2.0
    End Class

    ''' <summary>
    ''' 计算 Fisher 合并 p 值
    ''' chi2 = -2 * (ln(p1) + ln(p2))
    ''' 对于 2 个 p 值合并，自由度 df = 4
    ''' P(X <= x) = 1 - (1 + x/2) * exp(-x/2)  (chi-squared CDF with df=4)
    ''' </summary>
    Private Function FisherCombinedPValue(p1 As Double, p2 As Double) As Double
        ' 边界保护
        p1 = Max(1.0E-300, Min(1.0, p1))
        p2 = Max(1.0E-300, Min(1.0, p2))

        Dim chi2 As Double = -2.0 * (log(p1) + log(p2))
        If chi2 <= 0 Then Return 1.0

        ' chi-squared CDF with df=4: P(X <= x) = 1 - (1 + x/2) * exp(-x/2)
        Dim cdf As Double = 1.0 - (1.0 + chi2 / 2.0) * Exp(-chi2 / 2.0)
        Dim pCombined As Double = 1.0 - cdf

        Return Max(0.0, Min(1.0, pCombined))
    End Function

    ''' <summary>
    ''' 判定关联类型
    ''' Monotonic: |Spearman| >= threshold 且 MIC 显著
    ''' NonMonotonic: MIC 显著但 |Spearman| 不显著，且 MIC/|Spearman| 比值大
    ''' None: 两者均不显著
    ''' </summary>
    Private Function DetermineAssociationType(
        spearman As Double,
        mic As Double,
        spearmanThreshold As Double,
        micThreshold As Double,
        nonMonotonicRatio As Double) As String

        Dim absSpearman As Double = Abs(spearman)
        Dim spearmanSignificant As Boolean = absSpearman >= spearmanThreshold
        Dim micSignificant As Boolean = mic >= micThreshold

        If spearmanSignificant AndAlso micSignificant Then
            Return "Monotonic"
        ElseIf micSignificant AndAlso Not spearmanSignificant Then
            ' MIC 显著但 Spearman 不显著 → 可能是非单调关联
            If absSpearman < 0.001 Then
                Return "NonMonotonic"
            ElseIf mic / (absSpearman + 0.001) > nonMonotonicRatio Then
                Return "NonMonotonic"
            Else
                Return "Monotonic"  ' 弱单调
            End If
        ElseIf spearmanSignificant AndAlso Not micSignificant Then
            Return "Monotonic"  ' Spearman 显著但 MIC 不显著（少见，可能是样本量小）
        Else
            Return "None"
        End If
    End Function

    ''' <summary>
    ''' 执行 Spearman + MIC 联合分析
    '''
    ''' 完整流程:
    '''   1. 计算 Spearman 秩相关矩阵
    '''   2. 计算 MIC 矩阵
    '''   3. 根据 CombinationMethod 计算综合得分和显著性
    '''   4. 判定每对关联的类型（Monotonic / NonMonotonic / None）
    ''' </summary>
    Public Function ComputeCrossCorrelation(
        otuMatrix As ExpressionMatrix,
        metaboliteMatrix As ExpressionMatrix,
        Optional config As SpearmanMICConfig = Nothing) As SpearmanMICResult

        If config Is Nothing Then config = New SpearmanMICConfig()

        Dim nOtu As Integer = otuMatrix.FeatureCount
        Dim nMet As Integer = metaboliteMatrix.FeatureCount

        ' === Step 1: 计算 Spearman 相关 ===
        Dim spearmanResult As CorrelationResult =
            SpearmanCorrelation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)

        ' === Step 2: 计算 MIC ===
        Dim micResult As CorrelationResult =
            MICComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix, config.MICConfig)

        ' === Step 3: 构建联合分析结果 ===
        Dim result As New SpearmanMICResult()
        result.OtuIds = New String(nOtu - 1) {}
        result.MetaboliteIds = New String(nMet - 1) {}
        result.MethodName = "Spearman+MIC"
        result.SpearmanMatrix = New Double(nOtu - 1, nMet - 1) {}
        result.SpearmanPValueMatrix = New Double(nOtu - 1, nMet - 1) {}
        result.MICMatrix = New Double(nOtu - 1, nMet - 1) {}
        result.MICPValueMatrix = New Double(nOtu - 1, nMet - 1) {}
        result.CorrelationMatrix = New Double(nOtu - 1, nMet - 1) {}
        result.PValueMatrix = New Double(nOtu - 1, nMet - 1) {}
        result.CombinedScoreMatrix = New Double(nOtu - 1, nMet - 1) {}
        result.CombinedPValueMatrix = New Double(nOtu - 1, nMet - 1) {}
        result.IsSignificantMatrix = New Boolean(nOtu - 1, nMet - 1) {}
        result.AssociationTypeMatrix = New String(nOtu - 1, nMet - 1) {}

        For i As Integer = 0 To nOtu - 1
            result.OtuIds(i) = otuMatrix.Expression(i).Id
        Next
        For j As Integer = 0 To nMet - 1
            result.MetaboliteIds(j) = metaboliteMatrix.Expression(j).Id
        Next

        ' === Step 4: 逐对合并分析 ===
        For i As Integer = 0 To nOtu - 1
            For j As Integer = 0 To nMet - 1
                Dim sp As Double = spearmanResult.CorrelationMatrix(i, j)
                Dim spP As Double = spearmanResult.PValueMatrix(i, j)
                Dim mic As Double = micResult.CorrelationMatrix(i, j)
                Dim micP As Double = micResult.PValueMatrix(i, j)

                ' 保存原始值
                result.SpearmanMatrix(i, j) = sp
                result.SpearmanPValueMatrix(i, j) = spP
                result.MICMatrix(i, j) = mic
                result.MICPValueMatrix(i, j) = micP

                ' 判定关联类型
                result.AssociationTypeMatrix(i, j) = DetermineAssociationType(
                    sp, mic,
                    config.SpearmanThreshold,
                    config.MICThreshold,
                    config.NonMonotonicRatio)

                ' 根据方法计算综合得分和显著性
                Select Case config.Method
                    Case CombinationMethod.Intersection
                        ' 交集法：两者均显著
                        result.IsSignificantMatrix(i, j) =
                            (Abs(sp) >= config.SpearmanThreshold AndAlso
                             mic >= config.MICThreshold AndAlso
                             spP < config.PValueThreshold AndAlso
                             micP < config.PValueThreshold)
                        result.CombinedScoreMatrix(i, j) =
                            Min(Abs(sp), mic)
                        result.CombinedPValueMatrix(i, j) =
                            Max(spP, micP)

                    Case CombinationMethod.Union
                        ' 并集法：任一显著
                        result.IsSignificantMatrix(i, j) =
                            (Abs(sp) >= config.SpearmanThreshold AndAlso spP < config.PValueThreshold) OrElse
                            (mic >= config.MICThreshold AndAlso micP < config.PValueThreshold)
                        result.CombinedScoreMatrix(i, j) =
                            Max(Abs(sp), mic)
                        result.CombinedPValueMatrix(i, j) =
                            Min(spP, micP)

                    Case CombinationMethod.Weighted
                        ' 加权法：综合得分 = w1*|Spearman| + w2*MIC
                        Dim score As Double =
                            config.WeightSpearman * Abs(sp) +
                            config.WeightMIC * mic
                        result.CombinedScoreMatrix(i, j) = score
                        result.CombinedPValueMatrix(i, j) =
                            FisherCombinedPValue(spP, micP)
                        result.IsSignificantMatrix(i, j) =
                            (score >= (config.WeightSpearman * config.SpearmanThreshold +
                                       config.WeightMIC * config.MICThreshold) AndAlso
                             result.CombinedPValueMatrix(i, j) < config.PValueThreshold)

                    Case CombinationMethod.FisherCombined
                        ' Fisher 合并法：合并 p 值
                        Dim combinedP As Double = FisherCombinedPValue(spP, micP)
                        result.CombinedPValueMatrix(i, j) = combinedP
                        result.CombinedScoreMatrix(i, j) =
                            config.WeightSpearman * Abs(sp) +
                            config.WeightMIC * mic
                        result.IsSignificantMatrix(i, j) =
                            (combinedP < config.PValueThreshold AndAlso
                             (Abs(sp) >= config.SpearmanThreshold OrElse
                              mic >= config.MICThreshold))
                End Select

                ' CorrelationMatrix 存储综合得分，PValueMatrix 存储综合 p 值
                result.CorrelationMatrix(i, j) = result.CombinedScoreMatrix(i, j)
                result.PValueMatrix(i, j) = result.CombinedPValueMatrix(i, j)
            Next
        Next

        Return result
    End Function

    ''' <summary>
    ''' 从联合分析结果中筛选显著关联对
    ''' 返回 (OTU_ID, Metabolite_ID, Spearman, MIC, CombinedScore, CombinedP, Type) 的列表
    ''' </summary>
    Public Function FilterSignificantPairs(
        result As SpearmanMICResult,
        Optional pValueThreshold As Double = 0.05,
        Optional sortBy As String = "CombinedScore") As List(Of SignificantPair)

        Dim pairs As New List(Of SignificantPair)()
        Dim nOtu As Integer = result.OtuIds.Length
        Dim nMet As Integer = result.MetaboliteIds.Length

        For i As Integer = 0 To nOtu - 1
            For j As Integer = 0 To nMet - 1
                If result.IsSignificantMatrix(i, j) Then
                    Dim pair As New SignificantPair()
                    pair.OtuId = result.OtuIds(i)
                    pair.MetaboliteId = result.MetaboliteIds(j)
                    pair.SpearmanRho = result.SpearmanMatrix(i, j)
                    pair.SpearmanPValue = result.SpearmanPValueMatrix(i, j)
                    pair.MIC = result.MICMatrix(i, j)
                    pair.MICPValue = result.MICPValueMatrix(i, j)
                    pair.CombinedScore = result.CombinedScoreMatrix(i, j)
                    pair.CombinedPValue = result.CombinedPValueMatrix(i, j)
                    pair.AssociationType = result.AssociationTypeMatrix(i, j)
                    pairs.Add(pair)
                End If
            Next
        Next

        ' 排序
        Select Case sortBy
            Case "CombinedScore"
                pairs.Sort(Function(a, b) b.CombinedScore.CompareTo(a.CombinedScore))
            Case "CombinedPValue"
                pairs.Sort(Function(a, b) a.CombinedPValue.CompareTo(b.CombinedPValue))
            Case "MIC"
                pairs.Sort(Function(a, b) b.MIC.CompareTo(a.MIC))
            Case "Spearman"
                pairs.Sort(Function(a, b) Abs(b.SpearmanRho).CompareTo(Abs(a.SpearmanRho)))
        End Select

        Return pairs
    End Function

End Module

''' <summary>
''' 显著关联对数据结构
''' </summary>
Public Class SignificantPair
    ''' <summary>OTU ID</summary>
    Public OtuId As String
    ''' <summary>代谢物 ID</summary>
    Public MetaboliteId As String
    ''' <summary>Spearman 相关系数</summary>
    Public SpearmanRho As Double
    ''' <summary>Spearman p 值</summary>
    Public SpearmanPValue As Double
    ''' <summary>MIC 值</summary>
    Public MIC As Double
    ''' <summary>MIC p 值</summary>
    Public MICPValue As Double
    ''' <summary>综合得分</summary>
    Public CombinedScore As Double
    ''' <summary>综合 p 值</summary>
    Public CombinedPValue As Double
    ''' <summary>关联类型：Monotonic / NonMonotonic / None</summary>
    Public AssociationType As String
End Class