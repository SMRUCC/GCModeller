Namespace Model

    ''' <summary>
    ''' 总量设计（每个扰动每个基因的整数总量）的参数：对应参考配置 <c>model.totals</c> 段。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>信用（credit）</b>：用对照细胞的留一法矩（delete-one jackknife）抽样，
    ''' 得到"n 个细胞能承载多大的表达离散度"，作为设计目标的上界；
    ''' <see cref="CreditDraws"/> 是抽样次数，<see cref="CreditSeed"/> 是固定种子。
    ''' </para>
    ''' <para>
    ''' <see cref="AmpShare"/> 决定初始幅度设计取信用的多少；随后 <see cref="ProfileSpread"/>
    ''' 把设计边距解到"合成后的离散度命中信用目标"，最多迭代 <see cref="FixedSteps"/> 轮。
    ''' </para>
    ''' <para>
    ''' <see cref="ResidualRank"/> / <see cref="ResidualRidge"/> / <see cref="ResidualMinMeasured"/> /
    ''' <see cref="ResidualGlobal"/> 是残差校正层的参数（低秩基的秩、岭惩罚、最少实测记录数、全局主成分数）。
    ''' </para>
    ''' </remarks>
    Public Class TotalsOptions

        ''' <summary>信用抽样的次数，默认 64。</summary>
        Public Property CreditDraws As Integer = 64

        ''' <summary>信用抽样的随机种子，默认 7。</summary>
        Public Property CreditSeed As Integer = 7

        ''' <summary>初始幅度设计占信用的比例，默认 0.9。</summary>
        Public Property AmpShare As Double = 0.9

        ''' <summary>形状指数（<c>|l2|^gamma</c>），默认 0.8。</summary>
        Public Property ShapeGamma As Double = 0.8

        ''' <summary>形状的 z 饱和尺度，默认 16.0。</summary>
        Public Property ShapeKz As Double = 16.0

        ''' <summary>总量形状与剖面效应形状的混合比例，默认 0.5。</summary>
        Public Property Mix As Double = 0.5

        ''' <summary>控制程序平滑的作用门槛 |z|，默认 5.0。</summary>
        Public Property GateZ As Double = 5.0

        ''' <summary>控制 PCA 的抽样细胞数，默认 6000。</summary>
        Public Property PcsCells As Integer = 6000

        ''' <summary>控制 PCA 的抽样种子，默认 0。</summary>
        Public Property PcsSeed As Integer = 0

        ''' <summary>控制程序平滑使用的起始主成分下标，默认 10。</summary>
        Public Property CpJ0 As Integer = 10

        ''' <summary>控制程序平滑使用的主成分个数，默认 50。</summary>
        Public Property CpK As Integer = 50

        ''' <summary>控制程序平滑的强度 λ，默认 1.0。</summary>
        Public Property CpLam As Double = 1.0

        ''' <summary>参与控制程序平滑的扰动比例（按强度取前若干），默认 0.33。</summary>
        Public Property CpTop As Double = 0.33

        ''' <summary>程序加权的特征值指数，默认 0.25。</summary>
        Public Property PwGamma As Double = 0.25

        ''' <summary>程序加权使用的主成分个数，默认 100。</summary>
        Public Property PwK As Integer = 100

        ''' <summary>程序加权的强度 λ，默认 1.0。</summary>
        Public Property PwLam As Double = 1.0

        ''' <summary>参与程序加权的扰动比例，默认 0.25。</summary>
        Public Property PwTop As Double = 0.25

        ''' <summary>残差响应基的秩，默认 160。</summary>
        Public Property ResidualRank As Integer = 160

        ''' <summary>逐基因岭回归的惩罚 λ，默认 10.0。</summary>
        Public Property ResidualRidge As Double = 10.0

        ''' <summary>一个基因参与拟合所需的最少实测记录数，默认 32。</summary>
        Public Property ResidualMinMeasured As Integer = 32

        ''' <summary>残差修正的收缩系数 α，默认 0.25。</summary>
        Public Property ResidualAlpha As Double = 0.25

        ''' <summary>残差基拟合使用的最大扰动列数，默认 1024。</summary>
        Public Property ResidualPerts As Integer = 1024

        ''' <summary>控制表达标准化的尺度下限，默认 0.1。</summary>
        Public Property ResidualScaleFloor As Double = 0.1

        ''' <summary>全局控制主成分的个数，默认 5（因此特征数 f = 6）。</summary>
        Public Property ResidualGlobal As Integer = 5

        ''' <summary>残差坐标投影的岭系数，默认 0.1。</summary>
        Public Property ResidualProjRidge As Double = 0.1

        ''' <summary>控制特征在预测时的裁剪幅度，默认 3.0。</summary>
        Public Property ResidualClip As Double = 3.0

        ''' <summary>固定点迭代的最大轮数，默认 3。</summary>
        Public Property FixedSteps As Integer = 3

        ''' <summary>固定点迭代的相对收敛阈值，默认 0.001。</summary>
        Public Property FixedRtol As Double = 0.001

        ''' <summary>深拷贝。</summary>
        Public Function Clone() As TotalsOptions
            Return New TotalsOptions With {
                .CreditDraws = CreditDraws,
                .CreditSeed = CreditSeed,
                .AmpShare = AmpShare,
                .ShapeGamma = ShapeGamma,
                .ShapeKz = ShapeKz,
                .Mix = Mix,
                .GateZ = GateZ,
                .PcsCells = PcsCells,
                .PcsSeed = PcsSeed,
                .CpJ0 = CpJ0,
                .CpK = CpK,
                .CpLam = CpLam,
                .CpTop = CpTop,
                .PwGamma = PwGamma,
                .PwK = PwK,
                .PwLam = PwLam,
                .PwTop = PwTop,
                .ResidualRank = ResidualRank,
                .ResidualRidge = ResidualRidge,
                .ResidualMinMeasured = ResidualMinMeasured,
                .ResidualAlpha = ResidualAlpha,
                .ResidualPerts = ResidualPerts,
                .ResidualScaleFloor = ResidualScaleFloor,
                .ResidualGlobal = ResidualGlobal,
                .ResidualProjRidge = ResidualProjRidge,
                .ResidualClip = ResidualClip,
                .FixedSteps = FixedSteps,
                .FixedRtol = FixedRtol
            }
        End Function

    End Class
End Namespace
