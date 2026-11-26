' ------------------------------------------------------------------------
' 三维加权池化的审计
'
' 判据：
'   * 与自己的 CPU 参考实现逐元素对拍（S0/S1/S2/SQ 相对误差 <= 1e-12，
'     Measured 逐元素相等）
'   * 与 Math.Statistics 的元分析原语交叉验证：
'     - 单格 l2 应当等于 EvidenceCombination.WeightedMean(missingAsZero:=True)
'     - 单格 |z| 应当等于 |EvidenceCombination.Stouffer(...)|
'     这条用例把"我们的内核语义"钉在库里已有的口径上，
'     避免实现自洽但语义偏掉
'   * 退化情形：单行、全缺失、恒等因子
' ------------------------------------------------------------------------

Imports Microsoft.VisualBasic.Computing.ILCuda.Runtime
Imports Microsoft.VisualBasic.Computing.ILCuda.GPUTensor
Imports Microsoft.VisualBasic.Math.Statistics.MetaAnalysis

Namespace Audit

    ''' <summary>CellPooling 的验收用例</summary>
    Public Module PoolingAudit

        Public Function Run() As Integer
            Call Console.WriteLine()
            Call Console.WriteLine("== 三维加权池化 ==")

            MatchesReferenceWithMissing()
            MatchesLibrarySemantics()
            SingleRowIsIdentity()
            AllMissing()
            LargeVolume()
            ProfileStandardization()

            Return AuditRunner.FailureCount
        End Function

        ' ------------------------------------------------------------------
        ' 用例
        ' ------------------------------------------------------------------

        Private Sub MatchesReferenceWithMissing()
            Dim t As Integer = 8
            Dim g As Integer = 64
            Dim p As Integer = 32
            Dim effect As Double() = RandomEffect(t, g, p, 307, missingRate:=0.25)
            Dim z As Double() = RandomEffect(t, g, p, 311, missingRate:=0.25)
            Dim l2Factor As PoolFactors = RandomFactors(t, g, p, 313)
            Dim zFactor As PoolFactors = RandomFactors(t, g, p, 317)

            Dim actual As PooledStats = CellPooling.Pool(AuditRunner.Engine, effect, z, l2Factor, zFactor, t, g, p)
            Dim expected As PooledStats = CellPooling.PoolCpu(effect, z, l2Factor, zFactor, t, g, p)

            Dim s0 As Double = AuditRunner.MaxRelError(actual.S0, expected.S0)
            Dim s1 As Double = AuditRunner.MaxRelError(actual.S1, expected.S1)
            Dim s2 As Double = AuditRunner.MaxRelError(actual.S2, expected.S2)
            Dim sq As Double = AuditRunner.MaxRelError(actual.SQ, expected.SQ)
            Dim l2 As Double = AuditRunner.MaxRelError(actual.L2, expected.L2)
            Dim zErr As Double = AuditRunner.MaxRelError(actual.Z, expected.Z)
            Dim measured As Boolean = AuditRunner.SameIntegers(actual.Measured, expected.Measured)

            Call AuditRunner.Capture("pool.s0.maxRelError", s0)
            Call AuditRunner.Capture("pool.s1.maxRelError", s1)
            Call AuditRunner.Capture("pool.s2.maxRelError", s2)
            Call AuditRunner.Capture("pool.sq.maxRelError", sq)
            Call AuditRunner.Capture("pool.l2.maxRelError", l2)
            Call AuditRunner.Capture("pool.z.maxRelError", zErr)

            Dim ok As Boolean = s0 <= 1.0E-12 AndAlso s1 <= 1.0E-12 AndAlso s2 <= 1.0E-12 AndAlso
                                sq <= 1.0E-12 AndAlso l2 <= 1.0E-12 AndAlso zErr <= 1.0E-12 AndAlso measured

            Call AuditRunner.Record("充分统计量与 CPU 参考一致（含 25% 缺失）", ok,
                                    $"max(s0,s1,s2,sq,l2,z)={Max6(s0, s1, s2, sq, l2, zErr):G6}")
            Call AuditRunner.Record("实测行数逐元素相等", measured)
        End Sub

        Private Sub MatchesLibrarySemantics()
            Dim t As Integer = 8
            Dim g As Integer = 16
            Dim p As Integer = 16
            Dim effect As Double() = RandomEffect(t, g, p, 331, missingRate:=0.2)
            Dim z As Double() = RandomEffect(t, g, p, 337, missingRate:=0.2)
            Dim l2Factor As PoolFactors = RandomFactors(t, g, p, 347)

            Dim stats As PooledStats = CellPooling.PoolCpu(effect, z, l2Factor, l2Factor, t, g, p)

            ' 取一格，用库里的元分析原语重算
            Dim gi As Integer = 5
            Dim pi As Integer = 7
            Dim index As Integer = gi * p + pi
            Dim weights(t - 1) As Double
            Dim effects(t - 1) As Double
            Dim scores(t - 1) As Double

            For ti As Integer = 0 To t - 1
                weights(ti) = l2Factor.Row(ti * g + gi) * l2Factor.Col(ti * p + pi) * l2Factor.Mat(ti)
                effects(ti) = effect((ti * g + gi) * p + pi)
                scores(ti) = z((ti * g + gi) * p + pi)
            Next

            Dim wantL2 As Double = EvidenceCombination.WeightedMean(effects, weights, Nothing, missingAsZero:=True)
            Dim wantZ As Double = System.Math.Abs(EvidenceCombination.Stouffer(scores, weights))
            Dim l2Error As Double = System.Math.Abs(stats.L2(index) - wantL2)
            Dim zError As Double = System.Math.Abs(stats.Z(index) - wantZ)

            Call AuditRunner.Capture("pool.library.l2.absError", l2Error)
            Call AuditRunner.Capture("pool.library.z.absError", zError)
            Call AuditRunner.Record("单格 log2FC 与 EvidenceCombination.WeightedMean 一致", l2Error <= 1.0E-12, $"absError={l2Error:G6}")
            Call AuditRunner.Record("单格 |z| 与 EvidenceCombination.Stouffer 一致", zError <= 1.0E-12, $"absError={zError:G6}")
        End Sub

        Private Sub SingleRowIsIdentity()
            Dim t As Integer = 1
            Dim g As Integer = 32
            Dim p As Integer = 16
            Dim effect As Double() = RandomEffect(t, g, p, 349, missingRate:=0.0)
            Dim z As Double() = RandomEffect(t, g, p, 353, missingRate:=0.0)
            Dim factors As PoolFactors = PoolFactors.Identity(t, g, p)
            Dim stats As PooledStats = CellPooling.Pool(AuditRunner.Engine, effect, z, factors, factors, t, g, p)
            Dim err As Double = AuditRunner.MaxRelError(stats.L2, effect)

            Call AuditRunner.Record("单行池化等于原始效应", err <= 1.0E-12, $"maxRelError={err:G6}")

            ' 权重诊断：单行时有效行数必然是 1
            Dim neff As Double() = stats.EffectiveRows()
            Dim ok As Boolean = True

            For i As Integer = 0 To neff.Length - 1
                If System.Math.Abs(neff(i) - 1.0) > 1.0E-12 Then ok = False
            Next

            Call AuditRunner.Record("单行池化的有效行数为 1", ok)
        End Sub

        Private Sub AllMissing()
            Dim t As Integer = 4
            Dim g As Integer = 16
            Dim p As Integer = 8
            Dim effect(t * g * p - 1) As Double
            Dim z(t * g * p - 1) As Double

            For i As Integer = 0 To effect.Length - 1
                effect(i) = Double.NaN
                z(i) = Double.NaN
            Next

            Dim factors As PoolFactors = PoolFactors.Identity(t, g, p)
            Dim stats As PooledStats = CellPooling.Pool(AuditRunner.Engine, effect, z, factors, factors, t, g, p)
            Dim zero As Boolean = True
            Dim empty As Boolean = True

            For i As Integer = 0 To stats.L2.Length - 1
                If stats.L2(i) <> 0.0 Then zero = False
                If stats.Measured(i) <> 0 Then empty = False
            Next

            ' 缺失按"值为 0、权重照算"计入分母，因此 S0 仍然 > 0
            Call AuditRunner.Record("全缺失时 log2FC 合并为 0", zero)
            Call AuditRunner.Record("全缺失时实测行数为 0", empty)
            Call AuditRunner.Record("全缺失时权重和仍为正（missingAsZero 语义）", stats.S0(0) > 0.0, $"S0={stats.S0(0):G6}")
        End Sub

        Private Sub LargeVolume()
            ' 真实规模的一半：T=20, G=512, P=128（131 万格 x 20 行）
            Dim t As Integer = 20
            Dim g As Integer = 512
            Dim p As Integer = 128
            Dim effect As Double() = RandomEffect(t, g, p, 359, missingRate:=0.1)
            Dim z As Double() = RandomEffect(t, g, p, 367, missingRate:=0.1)
            Dim l2Factor As PoolFactors = RandomFactors(t, g, p, 373)
            Dim zFactor As PoolFactors = RandomFactors(t, g, p, 379)

            Dim actual As PooledStats = CellPooling.Pool(AuditRunner.Engine, effect, z, l2Factor, zFactor, t, g, p)
            Dim expected As PooledStats = CellPooling.PoolCpu(effect, z, l2Factor, zFactor, t, g, p)
            Dim err As Double = AuditRunner.MaxRelError(actual.L2, expected.L2)
            Dim zErr As Double = AuditRunner.MaxRelError(actual.Z, expected.Z)

            Call AuditRunner.Capture("pool.large.l2.maxRelError", err)
            Call AuditRunner.Record("大规模池化（20 x 512 x 128）与参考一致",
                                    err <= 1.0E-12 AndAlso zErr <= 1.0E-12,
                                    $"l2={err:G6}, z={zErr:G6}")
        End Sub

        Private Sub ProfileStandardization()
            Dim n As Integer = 100000
            Dim input As Double() = RandomVector(n, 383)
            Dim center As Double() = RandomVector(n, 389)
            Dim scale(n - 1) As Double

            For i As Integer = 0 To n - 1
                scale(i) = System.Math.Abs(input(i)) * 0.5 + 0.01
            Next

            Dim actual As Double() = CellPooling.Profile(AuditRunner.Engine, input, center, scale)
            Dim expected As Double() = CellPooling.ProfileCpu(input, center, scale)
            Dim err As Double = AuditRunner.MaxRelError(actual, expected)

            Call AuditRunner.Record("剖面标准化与 CPU 参考一致", err <= 1.0E-12, $"maxRelError={err:G6}")
        End Sub

        ' ------------------------------------------------------------------
        ' 工具
        ' ------------------------------------------------------------------

        Private Function RandomEffect(t As Integer, g As Integer, p As Integer,
                                      seed As Integer, missingRate As Double) As Double()
            Dim rng As New Random(seed)
            Dim result(t * g * p - 1) As Double

            For i As Integer = 0 To result.Length - 1
                If rng.NextDouble() < missingRate Then
                    result(i) = Double.NaN
                Else
                    result(i) = rng.NextDouble() * 2.0 - 1.0
                End If
            Next

            Return result
        End Function

        Private Function RandomFactors(t As Integer, g As Integer, p As Integer, seed As Integer) As PoolFactors
            Dim rng As New Random(seed)
            Dim row(t * g - 1) As Double
            Dim col(t * p - 1) As Double
            Dim mat(t - 1) As Double

            For i As Integer = 0 To row.Length - 1
                row(i) = 0.5 + rng.NextDouble()
            Next

            For i As Integer = 0 To col.Length - 1
                col(i) = 0.5 + rng.NextDouble()
            Next

            For i As Integer = 0 To mat.Length - 1
                mat(i) = 0.5 + rng.NextDouble()
            Next

            Return New PoolFactors With {.Row = row, .Col = col, .Mat = mat}
        End Function

        Private Function RandomVector(count As Integer, seed As Integer) As Double()
            Dim rng As New Random(seed)
            Dim result(count - 1) As Double

            For i As Integer = 0 To count - 1
                result(i) = rng.NextDouble() * 2.0 - 1.0
            Next

            Return result
        End Function

        Private Function Max6(a As Double, b As Double, c As Double,
                              d As Double, e As Double, f As Double) As Double
            Dim m As Double = a

            If b > m Then m = b
            If c > m Then m = c
            If d > m Then m = d
            If e > m Then m = e
            If f > m Then m = f

            Return m
        End Function
    End Module
End Namespace
