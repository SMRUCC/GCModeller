Namespace Graph

    ''' <summary>
    ''' 原子的四面体手性标记（源自 SMILES 的 <c>@</c> / <c>@@</c> 记号）。
    ''' </summary>
    Public Enum AtomStereo
        ''' <summary>没有手性标记。</summary>
        None = 0
        ''' <summary><c>@@</c>，按 SMILES 规范顺时针（从第一个邻居看过去）。</summary>
        Clockwise = 1
        ''' <summary><c>@</c>，按 SMILES 规范逆时针。</summary>
        CounterClockwise = 2
        ''' <summary>记号存在但当前版本无法解释（如 <c>@AL1</c>/<c>@TB10</c>）。</summary>
        Unsupported = 3
    End Enum

    ''' <summary>
    ''' 化学键的立体标记（源自 SMILES 的 <c>/</c> 与 <c>\</c>，以及从 2D 坐标推导出的楔形/虚线键）。
    ''' </summary>
    Public Enum BondStereo
        ''' <summary>无立体含义。</summary>
        None = 0
        ''' <summary>朝向观察者（实楔形）。</summary>
        Up = 1
        ''' <summary>背离观察者（虚楔形）。</summary>
        Down = 2
        ''' <summary>顺反未知的双键（<c>/?</c> 记号）。</summary>
        Either = 3
    End Enum

    ''' <summary>
    ''' 原子的杂化类型，由成键模式推断，用于 3D 坐标重建时的理想几何选择。
    ''' </summary>
    Public Enum Hybridization
        ''' <summary>无法推断。</summary>
        Unknown = 0
        ''' <summary>只有单键，四面体（109.47°）。</summary>
        SP3 = 1
        ''' <summary>含一个双键或芳香键，平面三角（120°）。</summary>
        SP2 = 2
        ''' <summary>含叁键或累积双键，直线（180°）。</summary>
        SP = 3
    End Enum

    ''' <summary>
    ''' 分子结构式字符串的格式。
    ''' </summary>
    Public Enum MoleculeFormat
        ''' <summary>格式未知 / 尚未解析。</summary>
        Unknown = 0
        ''' <summary><c>SMILES</c> 字符串。</summary>
        Smiles = 1
        ''' <summary><c>InChI</c> 字符串。</summary>
        InChI = 2
    End Enum

    ''' <summary>
    ''' 2D 平面坐标重建的可调参数。
    ''' </summary>
    Public Class Layout2DOptions

        ''' <summary>
        ''' 归一化之后的键长（绘图坐标单位）。默认 <c>1.0</c>。
        ''' </summary>
        Public Property BondLength As Double = 1.0

        ''' <summary>
        ''' 力导向精修的迭代轮数。默认 <c>300</c>。
        ''' </summary>
        Public Property Iterations As Integer = 300

        ''' <summary>
        ''' 键角约束的理想夹角（度）。默认 <c>120</c>。
        ''' </summary>
        Public Property IdealAngle As Double = 120

        ''' <summary>
        ''' 非键原子之间的最小允许距离，以键长为单位。默认 <c>0.85</c>。
        ''' </summary>
        Public Property MinSeparation As Double = 0.85

        ''' <summary>
        ''' 计算过程中的随机数种子，保证同一分子多次布局结果一致。
        ''' </summary>
        Public Property RandomSeed As Integer = 12345

    End Class

    ''' <summary>
    ''' 3D 立体坐标重建的可调参数。
    ''' </summary>
    Public Class Embed3DOptions

        ''' <summary>
        ''' 力场弛豫的迭代轮数。默认 <c>600</c>。
        ''' </summary>
        Public Property Iterations As Integer = 600

        ''' <summary>
        ''' 键长项的力常数。默认 <c>1.0</c>。
        ''' </summary>
        Public Property BondWeight As Double = 1.0

        ''' <summary>
        ''' 键角项的力常数。默认 <c>0.35</c>。
        ''' </summary>
        Public Property AngleWeight As Double = 0.35

        ''' <summary>
        ''' 环平面性项的力常数。默认 <c>0.15</c>。
        ''' </summary>
        Public Property PlanarityWeight As Double = 0.15

        ''' <summary>
        ''' 非键排斥项的力常数。默认 <c>0.08</c>。
        ''' </summary>
        Public Property RepulsionWeight As Double = 0.08

        ''' <summary>
        ''' 非键排斥的截断距离（Å）。默认 <c>3.2</c>。
        ''' </summary>
        Public Property RepulsionCutoff As Double = 3.2

        ''' <summary>
        ''' 是否在重建坐标之前把隐式氢展开为显式氢原子。默认 <c>True</c>。
        ''' </summary>
        Public Property AddHydrogens As Boolean = True

        ''' <summary>
        ''' 是否依据手性标记校正立体构型。默认 <c>True</c>。
        ''' </summary>
        Public Property RespectStereo As Boolean = True

        ''' <summary>
        ''' 计算过程中的随机数种子，保证同一分子多次重建结果一致。
        ''' </summary>
        Public Property RandomSeed As Integer = 20240101

    End Class

End Namespace
