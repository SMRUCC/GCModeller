Namespace MetaAnalysis

    ''' <summary>
    ''' 一个基因（或一个效应）在随机效应模型下的合并结果与诊断量。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="Converged"/> 为 <c>False</c> 表示"证据不足以完成合并"
    ''' （有效研究数不足、全部方差缺失、或效应全为缺失），此时
    ''' <see cref="Effect"/> / <see cref="StandardError"/> / <see cref="Z"/> 为 <c>NaN</c>，
    ''' <see cref="Diagnostics"/> 给出原因。调用方应把它当作"未合并"，而不是"合并得到 0"。
    ''' </para>
    ''' <para>
    ''' <see cref="NEffective"/>（<c>(Σw)²/Σw²</c>）与 <see cref="MaxWeightShare"/>（<c>max(w)/Σw</c>）
    ''' 是权重集中度的两个互补视角：前者回答"等效于几项独立研究"，
    ''' 后者回答"是否被单一研究劫持"。
    ''' </para>
    ''' </remarks>
    Public Class PooledEvidence

        ''' <summary>
        ''' 合并后的效应量（加权均值）
        ''' </summary>
        Public Property Effect As Double = Double.NaN

        ''' <summary>
        ''' 合并效应的标准误 <c>1/√Σw</c>
        ''' </summary>
        Public Property StandardError As Double = Double.NaN

        ''' <summary>
        ''' 合并效应的 z 值
        ''' </summary>
        Public Property Z As Double = Double.NaN

        ''' <summary>
        ''' 合并效应的双侧 P 值
        ''' </summary>
        Public Property PValue As Double = Double.NaN

        ''' <summary>
        ''' 研究间方差 τ²（DerSimonian-Laird）
        ''' </summary>
        Public Property Tau2 As Double = 0.0

        ''' <summary>
        ''' Cochran Q 异质性统计量
        ''' </summary>
        Public Property Q As Double = 0.0

        ''' <summary>
        ''' 异质性自由度 <c>k - 1</c>
        ''' </summary>
        Public Property Df As Integer = 0

        ''' <summary>
        ''' 异质性比例 <c>I² = max(0, (Q - df)/Q)</c>
        ''' </summary>
        Public Property I2 As Double = 0.0

        ''' <summary>
        ''' 参与合并的研究数
        ''' </summary>
        Public Property Studies As Integer = 0

        ''' <summary>
        ''' 有效研究数 <c>(Σw)²/Σw²</c>
        ''' </summary>
        Public Property NEffective As Double = 0.0

        ''' <summary>
        ''' 最大单项权重占比 <c>max(w)/Σw</c>
        ''' </summary>
        Public Property MaxWeightShare As Double = 0.0

        ''' <summary>
        ''' 是否成功完成合并
        ''' </summary>
        Public Property Converged As Boolean = False

        ''' <summary>
        ''' 未收敛（或退化）的原因
        ''' </summary>
        Public Property Diagnostics As String

        Public Overrides Function ToString() As String
            If Not Converged Then Return $"not pooled: {Diagnostics}"

            Return $"effect={Effect:G6}, se={StandardError:G6}, z={Z:G6}, p={PValue:G6}, tau2={Tau2:G4}, I2={I2:G3}, k={Studies}"
        End Function

    End Class
End Namespace
