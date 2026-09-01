Imports SMRUCC.genomics.Analysis.BNLearn.Intervention

Namespace ModularNetwork

    Public Class BlockPropagate

        Public Property Model As BlockNetwork
        ' ---- 全局扰动参数 ----

        ''' <summary>最大传播步数（雅可比收敛上限 / 级联采样时间步数）</summary>
        Public Property MaxSteps As Integer = 50

        ''' <summary>雅可比收敛阈值：||e_{t+1}|| / ||e_t|| 小于该值即停止</summary>
        Public Property Tolerance As Double = 0.000001
        ''' <summary>参数学习与采样所用样本数</summary>
        Public Property NSamples As Integer = 10000

        ''' <summary>随机种子</summary>
        Public Property RandomSeed As Integer = 42

        ' ============================================================
        ' 7. 传播方法
        ' ============================================================

        ''' <summary>雅可比矩阵多步线性传播</summary>
        Public Function PropagateJacobian(sourceIdx As Integer, mode As InterventionMode) As GlobalPerturbationResult
            Dim n As Integer = Model._genes.Length
            Dim delta = New Double(n - 1) {}
            delta(sourceIdx) = InterventionValue(sourceIdx, mode)

            Dim current = CType(delta.Clone(), Double())
            Dim total = New Double(n - 1) {}
            Dim result As New GlobalPerturbationResult() With {
                .SourceGene = Model._genes(sourceIdx),
                .Method = PropagationMethod.Jacobian,
                .mode = mode,
                .GeneNames = Model._genes
            }
            result.StepEffects.Add(CType(delta.Clone(), Double()))

            Dim steps As Integer = 0
            For t = 1 To MaxSteps
                Dim [next] = MatrixVectorMul(Model._A, current)
                For i = 0 To n - 1
                    total(i) += [next](i)
                Next
                result.StepEffects.Add([next])
                steps = t

                Dim normCur = Norm(current)
                Dim normNxt = Norm([next])
                If normCur < 0.000000001 Then Exit For
                If normNxt / normCur < Tolerance Then Exit For
                current = [next]
            Next

            result.Effects = total
            result.Steps = steps
            Return result
        End Function

        ''' <summary>级联采样：在全局聚合网络上做多步 do-演算传播</summary>
        Public Function PropagateCascade(sourceIdx As Integer, mode As InterventionMode) As GlobalPerturbationResult
            Dim spec As New InterventionSpec() With {
                .GeneName = Model._genes(sourceIdx),
                .GeneIndex = sourceIdx,
                .Mode = mode
            }

            Dim analyzer As New BnInterventionAnalyzer(Model._globalNet, Model._exprStd)
            Dim res = analyzer.DynamicIntervention(spec, MaxSteps, NSamples, RandomSeed)

            Dim result As New GlobalPerturbationResult() With {
                .SourceGene = Model._genes(sourceIdx),
                .Method = PropagationMethod.CascadeSampling,
                .mode = mode,
                .GeneNames = Model._genes,
                .Effects = CType(res.FoldChanges.Clone(), Double()),
                .Steps = MaxSteps
            }
            result.StepEffects.Add(CType(res.FoldChanges.Clone(), Double()))
            Return result
        End Function

        Private Function InterventionValue(sourceIdx As Integer, mode As InterventionMode) As Double
            ' 雅可比传播需要的是「相对野生型的扰动增量 Δx0」，而非绝对干预值。
            ' 标准化数据野生型均值≈0、SD≈1；Knockout 下调 1 个 SD、Overexpression 上调 3 倍、
            ' Knockdown 下调 2 倍（与 BnInterventionAnalyzer 中采样所用的偏离尺度一致）。
            ' 注意：不能用 GetInterventionValue(0,1) —— Knockout 返回绝对干预值 0，
            ' 在标准化数据（野生型均值已是 0）下扰动增量为 0，导致传播全 0。
            Select Case mode
                Case Intervention.InterventionMode.Knockout
                    Return -1.0
                Case Intervention.InterventionMode.Overexpression
                    Return 3.0
                Case Intervention.InterventionMode.Knockdown
                    Return -2.0
                Case Else
                    Return 0.0
            End Select
        End Function

        ' ---- 线性代数辅助 ----
        Private Function MatrixVectorMul(A As Double(,), v As Double()) As Double()
            Dim n = v.Length
            Dim out = New Double(n - 1) {}
            For i = 0 To n - 1
                Dim s As Double = 0
                For j = 0 To n - 1
                    s += A(i, j) * v(j)
                Next
                out(i) = s
            Next
            Return out
        End Function

        Private Function Norm(v As Double()) As Double
            Dim s As Double = 0
            For i = 0 To v.Length - 1
                s += v(i) * v(i)
            Next
            Return Math.Sqrt(s)
        End Function
    End Class
End Namespace