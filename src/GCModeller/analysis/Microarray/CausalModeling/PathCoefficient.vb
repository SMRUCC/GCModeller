''' <summary>
''' 路径系数与显著性检验表
''' 
''' 本表格展示了PLS-PM结构模型（内模型）中相邻层级潜变量之间的直接路径系数及其统计学显著性检验结果。通过该表格，可以了解到不同的潜变量之间存在的直接作用方向与相对强度，以及这些直接调控关系在统计学上是否达到显著水平。
''' </summary>
Public Class PathCoefficient

    ''' <summary>
    ''' fromName表示路径的起始潜变量（自变量）
    ''' </summary>
    ''' <returns></returns>
    Public Property fromName As String
    ''' <summary>
    ''' toName表示路径的目标潜变量（因变量）
    ''' </summary>
    ''' <returns></returns>
    Public Property toName As String
    ''' <summary>
    ''' coef表示路径系数，即直接效应的大小，正负值分别代表正负向调控关系
    ''' </summary>
    ''' <returns></returns>
    Public Property coef As Double
    ''' <summary>
    ''' se表示标准误
    ''' </summary>
    ''' <returns></returns>
    Public Property se As Double
    ''' <summary>
    ''' t表示T检验统计量
    ''' </summary>
    ''' <returns></returns>
    Public Property t As Double
    ''' <summary>
    ''' p表示P值，用于评估路径系数的显著性水平
    ''' </summary>
    ''' <returns></returns>
    Public Property p As Double
    ''' <summary>
    ''' sig表示显著性标记（如***表示极显著）
    ''' </summary>
    ''' <returns></returns>
    Public Property sig As String

    Public Overrides Function ToString() As String
        Return $"{fromName + " -> " + toName,-30}{coef,12:F4}{se,12:F4}{t,10:F3}{p,10:F4}{sig,8}"
    End Function

    Public Shared Iterator Function FromResult(result As PLSPMResult) As IEnumerable(Of PathCoefficient)
        For Each kv In result.PathCoefficients
            Dim fromName = result.LatentNames(kv.Key.Item1)
            Dim toName = result.LatentNames(kv.Key.Item2)
            Dim coef = kv.Value
            Dim se = result.StdErrors(kv.Key)
            Dim t = result.TValues(kv.Key)
            Dim p = result.PValues(kv.Key)
            Dim sig = If(p < 0.001, "***", If(p < 0.01, "**", If(p < 0.05, "*", "ns")))

            Yield New PathCoefficient With {
                .coef = coef,
                .fromName = fromName,
                .p = p,
                .se = se,
                .sig = sig,
                .t = t,
                .toName = toName
            }
        Next
    End Function

    Public Shared Iterator Function FromResult(result As SEMResult) As IEnumerable(Of PathCoefficient)
        For Each kv In result.PathCoefficients
            Dim fromName = result.VarNames(kv.Key.Item1)
            Dim toName = result.VarNames(kv.Key.Item2)
            Dim coef = kv.Value
            Dim se = result.StdErrors(kv.Key)
            Dim t = result.TValues(kv.Key)
            Dim p = result.PValues(kv.Key)
            Dim sig = If(p < 0.001, "***", If(p < 0.01, "**", If(p < 0.05, "*", "ns")))

            Yield New PathCoefficient With {
                .fromName = fromName,
                .coef = coef,
                .p = p,
                .se = se,
                .sig = sig,
                .t = t,
                .toName = toName
            }
        Next
    End Function

End Class

''' <summary>
''' 效应分解总表
''' 
''' 本表格展示了PLS-PM模型中任意起始潜变量对目标潜变量的总效应及其分解结果。通过该表格，可以全面了解一个变量对另一个变量的综合影响，包括直接作用和通过中介网络传导的间接作用，有助于厘清复杂的基因-蛋白-代谢-表型多层级网络中的关键调控主线与次要旁路。
''' </summary>
Public Class EffectDecomposition

    ''' <summary>
    ''' fromName和toName分别表示作用路径的起始潜变量和目标潜变量
    ''' </summary>
    ''' <returns></returns>
    Public Property fromName As String
    ''' <summary>
    ''' fromName和toName分别表示作用路径的起始潜变量和目标潜变量
    ''' </summary>
    ''' <returns></returns>
    Public Property toName As String
    ''' <summary>
    ''' direct表示直接效应系数
    ''' </summary>
    ''' <returns></returns>
    Public Property direct As Double
    ''' <summary>
    ''' indirect表示间接效应系数（通过中介变量）
    ''' </summary>
    ''' <returns></returns>
    Public Property indirect As Double
    ''' <summary>
    ''' total表示总效应系数，即直接效应与间接效应的代数和，反映自变量对因变量的整体影响力。
    ''' </summary>
    ''' <returns></returns>
    Public Property total As Double

    Public Overrides Function ToString() As String
        Return $"{fromName + " -> " + toName,-30}{direct,12:F4}{indirect,12:F4}{total,12:F4}"
    End Function

    Public Shared Iterator Function FromResult(result As PLSPMResult) As IEnumerable(Of EffectDecomposition)
        Dim allPairs As New HashSet(Of (Integer, Integer))
        For Each k In result.DirectEffects.Keys
            allPairs.Add(k)
        Next
        For Each k In result.IndirectEffects.Keys
            allPairs.Add(k)
        Next
        For Each k In result.TotalEffects.Keys
            allPairs.Add(k)
        Next

        For Each k In allPairs
            Dim fromName = result.LatentNames(k.Item1)
            Dim toName = result.LatentNames(k.Item2)
            Dim d = If(result.DirectEffects.ContainsKey(k), result.DirectEffects(k), 0.0)
            Dim i = If(result.IndirectEffects.ContainsKey(k), result.IndirectEffects(k), 0.0)
            Dim t = If(result.TotalEffects.ContainsKey(k), result.TotalEffects(k), 0.0)

            Yield New EffectDecomposition With {
                .fromName = fromName,
                .toName = toName,
                .direct = d,
                .indirect = i,
                .total = t
            }
        Next
    End Function

    Public Shared Iterator Function FromResult(result As SEMResult) As IEnumerable(Of EffectDecomposition)
        Dim allPairs As New HashSet(Of (Integer, Integer))

        For Each k In result.DirectEffects.Keys
            allPairs.Add(k)
        Next
        For Each k In result.IndirectEffects.Keys
            allPairs.Add(k)
        Next
        For Each k In result.TotalEffects.Keys
            allPairs.Add(k)
        Next

        For Each k In allPairs
            Dim fromName = result.VarNames(k.Item1)
            Dim toName = result.VarNames(k.Item2)
            Dim d = If(result.DirectEffects.ContainsKey(k), result.DirectEffects(k), 0.0)
            Dim i = If(result.IndirectEffects.ContainsKey(k), result.IndirectEffects(k), 0.0)
            Dim t = If(result.TotalEffects.ContainsKey(k), result.TotalEffects(k), 0.0)

            Yield New EffectDecomposition With {
                .fromName = fromName,
                .toName = toName,
                .direct = d,
                .indirect = i,
                .total = t
            }
        Next
    End Function

End Class

''' <summary>
''' Bootstrap重抽样路径系数置信区间表
''' 
''' 本表格展示了基于Bootstrap重抽样方法计算出的路径系数稳健性显著性检验结果。通过该表格，可以了解到经过多次重抽样后，模型中各直接路径系数的置信区间分布情况，以此验证观察到的路径关系是否并非由抽样误差引起，从而确认各上下游层级间直接效应的可靠性。
''' </summary>
Public Class BootstrapSignificanceTest

    ''' <summary>
    ''' fromName和toName分别表示路径的起始潜变量和目标潜变量
    ''' </summary>
    ''' <returns></returns>
    Public Property fromName As String
    ''' <summary>
    ''' fromName和toName分别表示路径的起始潜变量和目标潜变量
    ''' </summary>
    ''' <returns></returns>
    Public Property toName As String
    ''' <summary>
    ''' coef表示基于原始数据计算出的路径系数估计值
    ''' </summary>
    ''' <returns></returns>
    Public Property coef As Double
    ''' <summary>
    ''' bse表示Bootstrap标准误
    ''' </summary>
    ''' <returns></returns>
    Public Property bse As Double
    ''' <summary>
    ''' ci_lb和ci_ub分别表示 Bootstrap 95%置信区间的下限和上限
    ''' </summary>
    ''' <returns></returns>
    Public Property ci_lb As Double
    ''' <summary>
    ''' ci_lb和ci_ub分别表示 Bootstrap 95%置信区间的下限和上限
    ''' </summary>
    ''' <returns></returns>
    Public Property ci_ub As Double
    ''' <summary>
    ''' sig为布尔逻辑值，若为TRUE表示置信区间不跨越零，该路径系数显著。
    ''' </summary>
    ''' <returns></returns>
    Public Property sig As Boolean

    Public Overrides Function ToString() As String
        Return $"{fromName + " -> " + toName,-30}{coef,12:F4}{bse,12:F4}{ci_lb,12:F4}{ci_ub,12:F4}{sig,8}"
    End Function

    Public Shared Iterator Function FromResult(result As PLSPMResult, bootResult As PLSPMBootstrapResult) As IEnumerable(Of BootstrapSignificanceTest)
        For Each kv In result.PathCoefficients
            Dim fromName = result.LatentNames(kv.Key.Item1)
            Dim toName = result.LatentNames(kv.Key.Item2)
            Dim coef = kv.Value
            Dim bse = If(bootResult.PathBootSE.ContainsKey(kv.Key), bootResult.PathBootSE(kv.Key), 0.0)
            Dim ci = If(bootResult.PathBootCI.ContainsKey(kv.Key), bootResult.PathBootCI(kv.Key), (0.0, 0.0))
            Dim sig = (ci.Item1 > 0 AndAlso ci.Item2 > 0) OrElse (ci.Item1 < 0 AndAlso ci.Item2 < 0)

            Yield New BootstrapSignificanceTest With {
                .bse = bse,
                .coef = coef,
                .fromName = fromName,
                .sig = sig,
                .toName = toName,
                .ci_lb = ci.Item1,
                .ci_ub = ci.Item2
            }
        Next
    End Function

    Public Shared Iterator Function FromResult(result As SEMResult, bootResult As BootstrapResult) As IEnumerable(Of BootstrapSignificanceTest)
        For Each kv In result.PathCoefficients
            Dim fromName = result.VarNames(kv.Key.Item1)
            Dim toName = result.VarNames(kv.Key.Item2)
            Dim coef = kv.Value
            Dim bse = If(bootResult.PathBootSE.ContainsKey(kv.Key), bootResult.PathBootSE(kv.Key), 0.0)
            Dim ci = If(bootResult.PathBootCI.ContainsKey(kv.Key), bootResult.PathBootCI(kv.Key), (0.0, 0.0))
            Dim sig = (ci.Item1 > 0 AndAlso ci.Item2 > 0) OrElse (ci.Item1 < 0 AndAlso ci.Item2 < 0)

            Yield New BootstrapSignificanceTest With {
                .bse = bse,
                .coef = coef,
                .fromName = fromName,
                .sig = sig,
                .toName = toName,
                .ci_lb = ci.Item1,
                .ci_ub = ci.Item2
            }
        Next
    End Function

End Class

''' <summary>
''' 间接效应检验表
''' 
''' 本表格展示了PLS-PM模型中潜变量之间通过中介变量传导的间接效应大小及其显著性检验结果。通过该表格，可以了解到上游变量（如蛋白通路）是如何通过中介变量（如黄酮类物质）间接影响下游变量（如生物学表型）的，从而解析多组学数据层级间的跨层调控机制与信号传导路径。
''' </summary>
Public Class IndirectEffectBootstrap

    ''' <summary>
    ''' fromName和toName分别表示间接路径的起始潜变量和最终目标潜变量；
    ''' </summary>
    ''' <returns></returns>
    Public Property fromName As String
    ''' <summary>
    ''' fromName和toName分别表示间接路径的起始潜变量和最终目标潜变量；
    ''' </summary>
    ''' <returns></returns>
    Public Property toName As String
    ''' <summary>
    ''' indirectEffect表示间接效应的大小，即所有中介路径系数乘积之和
    ''' </summary>
    ''' <returns></returns>
    Public Property indirectEffect As Double
    ''' <summary>
    ''' bse表示Bootstrap标准误；
    ''' </summary>
    ''' <returns></returns>
    Public Property bse As Double
    ''' <summary>
    ''' ci_lb和ci_ub分别表示间接效应的Bootstrap 95%置信区间下限和上限；
    ''' </summary>
    ''' <returns></returns>
    Public Property ci_lb As Double
    ''' <summary>
    ''' ci_lb和ci_ub分别表示间接效应的Bootstrap 95%置信区间下限和上限；
    ''' </summary>
    ''' <returns></returns>
    Public Property ci_ub As Double
    ''' <summary>
    ''' sig表示该间接效应是否显著（TRUE表示显著）。
    ''' </summary>
    ''' <returns></returns>
    Public Property sig As Boolean

    Public Overrides Function ToString() As String
        Return $"{fromName + " -> " + toName,-30}{indirectEffect,12:F4}{bse,12:F4}{ci_lb,12:F4}{ci_ub,12:F4}{sig,8}"
    End Function

    Public Shared Iterator Function FromResult(result As PLSPMResult, bootResult As PLSPMBootstrapResult) As IEnumerable(Of IndirectEffectBootstrap)
        For Each kv In result.IndirectEffects
            Dim fromName = result.LatentNames(kv.Key.Item1)
            Dim toName = result.LatentNames(kv.Key.Item2)
            Dim ind = kv.Value
            Dim bse = If(bootResult.IndirectBootSE.ContainsKey(kv.Key), bootResult.IndirectBootSE(kv.Key), 0.0)
            Dim ci = If(bootResult.IndirectBootCI.ContainsKey(kv.Key), bootResult.IndirectBootCI(kv.Key), (0.0, 0.0))
            Dim sig = (ci.Item1 > 0 AndAlso ci.Item2 > 0) OrElse (ci.Item1 < 0 AndAlso ci.Item2 < 0)

            Yield New IndirectEffectBootstrap With {
                .fromName = fromName,
                .indirectEffect = ind,
                .bse = bse,
                .sig = sig,
                .toName = toName,
                .ci_lb = ci.Item1,
                .ci_ub = ci.Item2
            }
        Next
    End Function

    Public Shared Iterator Function FromResult(result As SEMResult, bootResult As BootstrapResult) As IEnumerable(Of IndirectEffectBootstrap)
        For Each kv In result.IndirectEffects
            Dim fromName = result.VarNames(kv.Key.Item1)
            Dim toName = result.VarNames(kv.Key.Item2)
            Dim ind = kv.Value
            Dim bse = If(bootResult.IndirectBootSE.ContainsKey(kv.Key), bootResult.IndirectBootSE(kv.Key), 0.0)
            Dim ci = If(bootResult.IndirectBootCI.ContainsKey(kv.Key), bootResult.IndirectBootCI(kv.Key), (0.0, 0.0))
            Dim sig = (ci.Item1 > 0 AndAlso ci.Item2 > 0) OrElse (ci.Item1 < 0 AndAlso ci.Item2 < 0)

            Yield New IndirectEffectBootstrap With {
                .fromName = fromName,
                .indirectEffect = ind,
                .bse = bse,
                .sig = sig,
                .toName = toName,
                .ci_lb = ci.Item1,
                .ci_ub = ci.Item2
            }
        Next
    End Function
End Class