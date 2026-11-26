Namespace Model

    ''' <summary>
    ''' 随机效应池化的参数：对应参考配置 <c>model.pooling</c> 段。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 六个 <c>*_row / *_col / *_mat</c> 字符串是<b>因子表达式</b>：以 <c>*</c> 连接的因子名，
    ''' 例如 <c>"inv:mad:dl"</c>（逆方差权重）、<c>"sef*kd"</c>（尺度误差 × 敲低系数）、
    ''' <c>"auxiliary*sim*pair"</c>（辅助行 × 相似度 × 配对降权）。
    ''' 表达式的<b>写法与相乘顺序都是模型定义的一部分</b>（浮点乘法不可交换），因此这里保留为字符串，
    ''' 由 <c>FactorExpression</c> 解析、由池化内核按序求值。
    ''' </para>
    ''' <para>
    ''' <see cref="Views"/> 决定参与池化的训练视图（按顺序），<see cref="AuxiliaryWeight"/>
    ''' 是对 <c>role = auxiliary</c> 的整行加权倍数。
    ''' </para>
    ''' </remarks>
    Public Class PoolingOptions

        ''' <summary>log2FC 的行（基因）方向因子表达式，默认 <c>inv:mad:dl</c>。</summary>
        Public Property L2Row As String = "inv:mad:dl"

        ''' <summary>log2FC 的列（扰动）方向因子表达式，默认 <c>sef*kd</c>。</summary>
        Public Property L2Col As String = "sef*kd"

        ''' <summary>log2FC 的整行因子表达式，默认 <c>auxiliary*sim*pair</c>。</summary>
        Public Property L2Mat As String = "auxiliary*sim*pair"

        ''' <summary>z 通道的行方向因子表达式，默认 <c>nr</c>（噪声比）。</summary>
        Public Property ZRow As String = "nr"

        ''' <summary>z 通道的列方向因子表达式，默认 <c>sef*kd</c>。</summary>
        Public Property ZCol As String = "sef*kd"

        ''' <summary>z 通道的整行因子表达式，默认 <c>auxiliary*sim*pair</c>。</summary>
        Public Property ZMat As String = "auxiliary*sim*pair"

        ''' <summary>噪声比因子的幂次，默认 1.0。</summary>
        Public Property Noise As Double = 1.0

        ''' <summary>尺度误差因子的幂次，默认 1.0。</summary>
        Public Property Se As Double = 1.0

        ''' <summary>尺度误差比值的裁剪幅度，默认 4.0。</summary>
        Public Property SeClip As Double = 4.0

        ''' <summary>敲低系数的参考值（归一化分母），默认 0.6。</summary>
        Public Property KdRef As Double = 0.6

        ''' <summary>敲低系数的下限，默认 0（允许完全敲低）。</summary>
        Public Property KdFloor As Double = 0.0

        ''' <summary>相似度权重的指数系数 β，默认 0.25。</summary>
        Public Property Sim As Double = 0.25

        ''' <summary>计算相似度时使用的高变基因数，默认 2000。</summary>
        Public Property SimHvg As Integer = 2000

        ''' <summary>池化后的修正方式（保留上游字段，默认 <c>LZ</c>）。</summary>
        Public Property Correction As String = "LZ"

        ''' <summary>缺失值填充方式（保留上游字段，默认 <c>zf</c>）。</summary>
        Public Property Fill As String = "zf"

        ''' <summary>是否使用单条证据流（保留上游字段，默认 True）。</summary>
        Public Property OneStream As Boolean = True

        ''' <summary>参与池化的训练视图，按顺序。</summary>
        Public Property Views As String() = {"view_0", "view_1"}

        ''' <summary>辅助行的整行加权倍数，默认 2。</summary>
        Public Property AuxiliaryWeight As Double = 2.0

        ''' <summary>配对/交替记录的降权倍数，默认 0.5。</summary>
        Public Property PairWeight As Double = 0.5

        ''' <summary>按通道名取表达式：<c>l2_row</c> / <c>l2_col</c> / <c>l2_mat</c> / <c>z_row</c> / <c>z_col</c> / <c>z_mat</c>。</summary>
        Public Function Expression(stream As String, axis As String) As String
            Select Case $"{stream}_{axis}".ToLowerInvariant()
                Case "l2_row"
                    Return L2Row
                Case "l2_col"
                    Return L2Col
                Case "l2_mat"
                    Return L2Mat
                Case "z_row"
                    Return ZRow
                Case "z_col"
                    Return ZCol
                Case "z_mat"
                    Return ZMat
                Case Else
                    Throw New ArgumentException($"unknown pooling channel '{stream}_{axis}'", NameOf(stream))
            End Select
        End Function

    End Class
End Namespace
