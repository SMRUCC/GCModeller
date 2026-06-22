Public Class PathCoefficient

    Public Property fromName As String
    Public Property toName As String
    Public Property coef As Double
    Public Property se As Double
    Public Property t As Double
    Public Property p As Double
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

Public Class EffectDecomposition

    Public Property fromName As String
    Public Property toName As String
    Public Property direct As Double
    Public Property indirect As Double
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

Public Class BootstrapSignificanceTest

    Public Property fromName As String
    Public Property toName As String
    Public Property coef As Double
    Public Property bse As Double
    Public Property ci_lb As Double
    Public Property ci_ub As Double
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

Public Class IndirectEffectBootstrap

    Public Property fromName As String
    Public Property toName As String
    Public Property indirectEffect As Double
    Public Property bse As Double
    Public Property ci_lb As Double
    Public Property ci_ub As Double
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