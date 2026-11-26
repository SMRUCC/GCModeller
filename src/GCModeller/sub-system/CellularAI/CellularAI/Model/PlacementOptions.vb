Namespace Model

    ''' <summary>
    ''' 单条"检测限幅度的规则"：<c>m = amp·(h0 + (1−h0)·z²/(z²+kz))·√(1/μ + φ)</c>，再截到 <c>cap</c>、
    ''' 并保证不小于 <c>cl2·|l2|</c>。
    ''' </summary>
    ''' <remarks>
    ''' 这类规则有两种用法：<see cref="PlacementOptions.Rule"/> 用于"有中等证据"的基因，
    ''' <see cref="PlacementOptions.Frozen"/> 用于"证据很强/很弱"的基因（不再随 z 增长，只保留一个冻结的幅度）。
    ''' </remarks>
    Public Class PlacementBand

        ''' <summary>整体幅度系数。</summary>
        Public Property Amp As Double = 0.6

        ''' <summary>低 z 时的底噪比例（0~1）。</summary>
        Public Property H0 As Double = 0.25

        ''' <summary>z 的饱和尺度。</summary>
        Public Property Kz As Double = 16.0

        ''' <summary>深度过度离散项。</summary>
        Public Property Phi As Double = 0.2

        ''' <summary>幅度上限。</summary>
        Public Property Cap As Double = 1.2

        ''' <summary>幅度相对 |log2FC| 的最小倍数。</summary>
        Public Property Cl2 As Double = 2.2

        ''' <summary>深拷贝。</summary>
        Public Function Clone() As PlacementBand
            Return New PlacementBand With {
                .Amp = Amp,
                .H0 = H0,
                .Kz = Kz,
                .Phi = Phi,
                .Cap = Cap,
                .Cl2 = Cl2
            }
        End Function

    End Class

    ''' <summary>
    ''' 放置（把证据转成目标平均表达）的参数：对应参考配置 <c>model.placement</c> 段。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 核心是"按 |z| 分段选规则"：<see cref="RuleBand"/> 内的基因用 <see cref="Rule"/>（随 |z| 增长），
    ''' 区间外用 <see cref="Frozen"/>（冻结幅度）；<see cref="BoostBand"/> 内再乘 <see cref="Boost"/>，
    ''' <see cref="DampBand"/> 内乘 <see cref="Damp"/>。
    ''' </para>
    ''' <para>
    ''' <see cref="EvidenceZ"/> 是"该扰动是否值得放置"的最低证据门槛；
    ''' <see cref="PlanFloorZ"/> 是计划基因在方向上必须达到的最小幅度所对应的 z。
    ''' </para>
    ''' </remarks>
    Public Class PlacementOptions

        ''' <summary>中等证据区间的规则。</summary>
        Public Property Rule As PlacementBand = New PlacementBand With {
            .Amp = 0.6, .H0 = 0.25, .Kz = 16.0, .Phi = 0.2, .Cap = 1.2, .Cl2 = 2.2
        }

        ''' <summary>强/弱证据区间的冻结规则。</summary>
        Public Property Frozen As PlacementBand = New PlacementBand With {
            .Amp = 0.4, .H0 = 0.25, .Kz = 16.0, .Phi = 0.2, .Cap = 0.8, .Cl2 = 1.5
        }

        ''' <summary>放置所需的最低证据 |z|，默认 1.0。</summary>
        Public Property EvidenceZ As Double = 1.0

        ''' <summary><see cref="Rule"/> 生效的 |z| 区间，默认 [1, 5)。</summary>
        Public Property RuleBand As Double() = {1.0, 5.0}

        ''' <summary>幅度抬升的 |z| 区间，默认 [1, 2)。</summary>
        Public Property BoostBand As Double() = {1.0, 2.0}

        ''' <summary>抬升倍数，默认 1.88。</summary>
        Public Property Boost As Double = 1.88

        ''' <summary>幅度衰减的 |z| 区间，默认 [3, 5)。</summary>
        Public Property DampBand As Double() = {3.0, 5.0}

        ''' <summary>衰减倍数，默认 0.9。</summary>
        Public Property Damp As Double = 0.9

        ''' <summary>计划基因的最小幅度对应的 z，默认 3.0。</summary>
        Public Property PlanFloorZ As Double = 3.0

        ''' <summary>排序键里 |z| 的归一化尺度，默认 0.6。</summary>
        Public Property OrderAz As Double = 0.6

        ''' <summary>排序键里相似度加权 z 的归一化尺度，默认 3.0。</summary>
        Public Property OrderZsim As Double = 3.0

        ''' <summary>相似度加权 z 的裁剪区间，默认 [-6, 12]。</summary>
        Public Property OrderClip As Double() = {-6.0, 12.0}

        ''' <summary>目标文库大小的离散度指数（1 = 与对照同形），默认 0.85。</summary>
        Public Property LibSpread As Double = 0.85

        ''' <summary>判断 |z| 是否落在某个区间 [lo, hi)。</summary>
        Public Shared Function InBand(z As Double, band As Double()) As Boolean
            If band Is Nothing OrElse band.Length < 2 Then
                Return False
            End If

            Return z >= band(0) AndAlso z < band(1)
        End Function

        ''' <summary>深拷贝。</summary>
        Public Function Clone() As PlacementOptions
            Return New PlacementOptions With {
                .Rule = Rule.Clone(),
                .Frozen = Frozen.Clone(),
                .EvidenceZ = EvidenceZ,
                .RuleBand = DirectCast(RuleBand.Clone(), Double()),
                .BoostBand = DirectCast(BoostBand.Clone(), Double()),
                .Boost = Boost,
                .DampBand = DirectCast(DampBand.Clone(), Double()),
                .Damp = Damp,
                .PlanFloorZ = PlanFloorZ,
                .OrderAz = OrderAz,
                .OrderZsim = OrderZsim,
                .OrderClip = DirectCast(OrderClip.Clone(), Double()),
                .LibSpread = LibSpread
            }
        End Function

    End Class
End Namespace
