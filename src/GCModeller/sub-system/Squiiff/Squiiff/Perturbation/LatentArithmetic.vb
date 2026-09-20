Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.Evaluation
Imports SMRUCC.genomics.Analysis.Squiiff.NN
Imports std = System.Math

Namespace Perturbation

    ''' <summary>
    ''' 语义隐空间中的向量算术（SquiDiff 扰动预测的核心操作）。
    '''
    ''' 两种操作方式（README 第六节）：
    ''' <list type="number">
    ''' <item><b>向量加法</b>：<c>Δz_sem = z_sem^pert − z_sem^ctrl</c>，
    '''       代表扰动在隐空间中的"方向向量"；把它加到新的对照细胞隐变量上再条件解码，
    '''       即得该细胞受同一扰动后的预测转录组。适用于基因敲除、药物处理等离散条件。</item>
    ''' <item><b>线性插值</b>：<c>Lerp(z_A, z_B, t) = (1−t)·z_A + t·z_B</c>，
    '''       用于生成两个状态之间的连续过渡态（如分化过程中的中间态）。</item>
    ''' </list>
    ''' 注意 <c>Δz_sem</c> 代表的是一段扰动/分化的**平均方向**，而非精确的瞬时动态。
    ''' </summary>
    Public Module LatentArithmetic

        ''' <summary>把一组隐变量按样本维求平均，得到 <c>[1,D]</c> 的组中心。</summary>
        Public Function MeanVector(z As Tensor) As Tensor
            Dim batch = z.Shape(0)
            If batch <= 0 Then Throw New ArgumentException("隐变量批量为空")

            Return TensorUtil.Scale(z.Sum(axis:=0), 1.0 / batch)
        End Function

        ''' <summary>按指定行下标求平均，得到 <c>[1,D]</c> 的组中心。</summary>
        Public Function MeanRows(z As Tensor, rowIndices As Integer()) As Tensor
            If rowIndices Is Nothing OrElse rowIndices.Length = 0 Then
                Throw New ArgumentException("行下标集合为空", NameOf(rowIndices))
            End If

            Dim columns = z.Shape(1)
            Dim data = z.Data
            Dim mean(columns - 1) As Double

            For Each row In rowIndices
                Dim offset = row * columns
                For j As Integer = 0 To columns - 1
                    mean(j) += data(offset + j)
                Next
            Next

            For j As Integer = 0 To columns - 1
                mean(j) /= rowIndices.Length
            Next

            Return TensorUtil.ColumnVector(mean).Transpose()     ' [D,1] -> [1,D]
        End Function

        ''' <summary>
        ''' 估计扰动方向向量：<c>Δz_sem = mean(z_sem^perturbed) − mean(z_sem^control)</c>。
        ''' </summary>
        Public Function EstimateDelta(zPerturbed As Tensor, zControl As Tensor) As Tensor
            Return MeanVector(zPerturbed) - MeanVector(zControl)
        End Function

        ''' <summary>
        ''' 向量加法：把扰动方向叠加到对照细胞的语义隐变量上，得到 <c>z_sem^new</c>。
        ''' </summary>
        Public Function ApplyDelta(zControl As Tensor, delta As Tensor) As Tensor
            Return TensorUtil.BroadcastAdd(zControl, delta)
        End Function

        ''' <summary>线性插值 <c>(1−α)·z_A + α·z_B</c>（逐样本）。</summary>
        Public Function Lerp(zA As Tensor, zB As Tensor, alpha As Double) As Tensor
            Dim a = CSng(1.0 - alpha)
            Dim b = CSng(alpha)

            Return zA * a + zB * b
        End Function

        ''' <summary>
        ''' 生成插值轨迹：<paramref name="steps"/> 个等间隔的过渡态（含两端点）。
        ''' </summary>
        Public Function LerpTrajectory(zA As Tensor, zB As Tensor, steps As Integer) As List(Of Tensor)
            If steps < 2 Then Throw New ArgumentOutOfRangeException(NameOf(steps), "插值步数至少为 2")

            Dim trajectory As New List(Of Tensor)(steps)
            For i As Integer = 0 To steps - 1
                trajectory.Add(Lerp(zA, zB, CDbl(i) / (steps - 1)))
            Next

            Return trajectory
        End Function

        ''' <summary>方向向量的 L2 范数（用于比较不同扰动的"强度"）。</summary>
        Public Function Norm(delta As Tensor) As Double
            Return std.Sqrt(Tensor.computeKernel.SumAll(delta.ElementwiseMultiply(delta)))
        End Function

        ''' <summary>两个 <c>[1,D]</c> 方向向量的余弦相似度。</summary>
        Public Function Cosine(a As Tensor, b As Tensor) As Double
            Dim dot = Tensor.computeKernel.SumAll(a.ElementwiseMultiply(b))
            Dim normA = Norm(a)
            Dim normB = Norm(b)

            If normA <= 1.0E-300 OrElse normB <= 1.0E-300 Then Return Double.NaN
            Return dot / (normA * normB)
        End Function

        ''' <summary>
        ''' 隐空间方向一致性：同一扰动在两个细胞类型上的 <c>Δz</c> 余弦相似度。
        ''' 见 <see cref="PerturbationMetrics.DirectionConsistency"/>。
        ''' </summary>
        Public Function DirectionConsistency(deltaA As Tensor, deltaB As Tensor) As Double
            Return PerturbationMetrics.DirectionConsistency(deltaA, deltaB)
        End Function

        ''' <summary>把 <c>[1,D]</c> 的方向向量转为可读的数值摘要。</summary>
        Public Function Describe(delta As Tensor, geneLabels As String(), Optional topN As Integer = 6) As String
            Dim values = delta.Data
            Dim order = Enumerable.Range(0, values.Length).ToArray()
            Array.Sort(order, Function(x, y) std.Abs(values(y)).CompareTo(std.Abs(values(x))))

            Dim items As New List(Of String)
            For i As Integer = 0 To std.Min(topN, order.Length) - 1
                Dim index = order(i)
                Dim label = If(geneLabels IsNot Nothing AndAlso index < geneLabels.Length, geneLabels(index), $"z{index + 1}")
                items.Add($"{label}={values(index):F3}")
            Next

            Return $"‖Δz‖={Norm(delta):F3} 主导分量: {String.Join(", ", items)}"
        End Function
    End Module
End Namespace
