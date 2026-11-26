Namespace Model

    ''' <summary>
    ''' 单细胞计数合成的参数：对应参考配置 <c>model.synthesis</c> 段。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="ZPlan"/> 是计划基因的目标秩和 z 区间（按优先级从低到高线性插值），
    ''' <see cref="ZNudge"/> 是非计划基因在微调强度 1 时的目标 z。
    ''' </para>
    ''' <para>
    ''' <see cref="Rmax"/> / <see cref="NBis"/> 控制 copula 相关系数的搜索范围与二分次数；
    ''' <see cref="Ztol"/> / <see cref="ZtolPlan"/> 是"z 可以偏离目标多少"的容差（后者用于计划基因，更严）；
    ''' <see cref="Ltol"/> 是"平均每细胞 CPM 可以偏离放置目标多少"（对数尺度）。
    ''' </para>
    ''' <para>
    ''' <see cref="PlannedZ"/> / <see cref="OvershootZ"/> / <see cref="RepairR"/> 决定"过冲修复"的触发条件与候选相关；
    ''' <see cref="SolveNtop"/> / <see cref="SolveTol"/> / <see cref="SolveIters"/> 控制平移二分；
    ''' <see cref="ShiftBand"/> 是平移的搜索区间；<see cref="SparsifyStep"/> 是稀疏化扫描步长。
    ''' </para>
    ''' </remarks>
    Public Class SynthesisOptions

        ''' <summary>计划基因的目标 z 区间（低优先级 → 高优先级），默认 (5.5, 9.0)。</summary>
        Public Property ZPlan As Double() = {5.5, 9.0}

        ''' <summary>非计划基因在满强度下的目标 |z|，默认 2.0。</summary>
        Public Property ZNudge As Double = 2.0

        ''' <summary>copula 相关系数的搜索上界，默认 0.99。</summary>
        Public Property Rmax As Double = 0.99

        ''' <summary>相关系数二分的次数，默认 5。</summary>
        Public Property NBis As Integer = 5

        ''' <summary>非计划基因的 z 容差，默认 1.0。</summary>
        Public Property Ztol As Double = 1.0

        ''' <summary>计划基因的 z 容差，默认 0.2。</summary>
        Public Property ZtolPlan As Double = 0.2

        ''' <summary>平均 CPM 的对数容差，默认 0.01。</summary>
        Public Property Ltol As Double = 0.01

        ''' <summary>非表达基因突发散布的每块大小，默认 4。</summary>
        Public Property NeBurst As Integer = 4

        ''' <summary>"计划基因"的 |z| 判定门槛，默认 5.0。</summary>
        Public Property PlannedZ As Double = 5.0

        ''' <summary>过冲修复的触发幅度，默认 0.4。</summary>
        Public Property OvershootZ As Double = 0.4

        ''' <summary>过冲修复的候选相关系数，默认 (0.25, 0.5)（另加其负值与 0、±rmax）。</summary>
        Public Property RepairR As Double() = {0.25, 0.5}

        ''' <summary>质量守恒补足时使用的降序名次个数，默认 40。</summary>
        Public Property SolveNtop As Integer = 40

        ''' <summary>平移二分的 z 容差，默认 0.05。</summary>
        Public Property SolveTol As Double = 0.05

        ''' <summary>平移二分的最大迭代次数，默认 40。</summary>
        Public Property SolveIters As Integer = 40

        ''' <summary>平移量的搜索区间，默认 [-0.6, 1.6]。</summary>
        Public Property ShiftBand As Double() = {-0.6, 1.6}

        ''' <summary>稀疏化扫描的步长，默认 0.025。</summary>
        Public Property SparsifyStep As Double = 0.025

        ''' <summary>深拷贝。</summary>
        Public Function Clone() As SynthesisOptions
            Return New SynthesisOptions With {
                .ZPlan = DirectCast(ZPlan.Clone(), Double()),
                .ZNudge = ZNudge,
                .Rmax = Rmax,
                .NBis = NBis,
                .Ztol = Ztol,
                .ZtolPlan = ZtolPlan,
                .Ltol = Ltol,
                .NeBurst = NeBurst,
                .PlannedZ = PlannedZ,
                .OvershootZ = OvershootZ,
                .RepairR = DirectCast(RepairR.Clone(), Double()),
                .SolveNtop = SolveNtop,
                .SolveTol = SolveTol,
                .SolveIters = SolveIters,
                .ShiftBand = DirectCast(ShiftBand.Clone(), Double()),
                .SparsifyStep = SparsifyStep
            }
        End Function

    End Class
End Namespace
