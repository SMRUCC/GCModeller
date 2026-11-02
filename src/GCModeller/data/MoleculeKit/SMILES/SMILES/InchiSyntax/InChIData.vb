Namespace InchiSyntax

    ''' <summary>
    ''' InChI 中的一个移动氢组（<c>(H,3,4)</c> 这样的记号）：一组原子共享若干个可移动的氢。
    ''' </summary>
    Public Class MobileHydrogen

        ''' <summary>
        ''' 共享的氢原子个数。
        ''' </summary>
        Public Property Count As Integer = 1

        ''' <summary>
        ''' 参与共享的原子下标（0 基）。
        ''' </summary>
        Public Property Atoms As Integer()

        ''' <summary>
        ''' 调试用的紧凑描述。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"({Count}H over [{String.Join(", ", Atoms)}])"
        End Function

    End Class

    ''' <summary>
    ''' InChI 字符串逐层解析之后的中间表示。
    ''' </summary>
    ''' <remarks>
    ''' 只做"拆层与逐项解析"，不做任何化学推断；图构建与键级还原在
    ''' <see cref="InChIBuilder"/> 中完成。
    ''' </remarks>
    Public Class InChIData

        ''' <summary>
        ''' InChI 版本标记（通常为 <c>1S</c>，标准 InChI）。
        ''' </summary>
        Public Property Version As String

        ''' <summary>
        ''' 分子式层的原文（多组分之间以 <c>.</c> 分隔）。
        ''' </summary>
        Public Property Formula As String

        ''' <summary>
        ''' 各组分（分子式层按 <c>.</c> 切分之后的结果）。
        ''' </summary>
        Public Property Components As New List(Of String)

        ''' <summary>
        ''' 重原子元素符号列表，顺序即 InChI 的原子编号顺序（下标 0 对应编号 1）。
        ''' </summary>
        Public Property Elements As New List(Of String)

        ''' <summary>
        ''' 分子式层给出的氢原子总数。
        ''' </summary>
        Public Property HydrogenCount As Integer

        ''' <summary>
        ''' 连接层（<c>/c</c>）原文。
        ''' </summary>
        Public Property Connection As String

        ''' <summary>
        ''' 氢层（<c>/h</c>）原文。
        ''' </summary>
        Public Property Hydrogen As String

        ''' <summary>
        ''' 电荷层（<c>/q</c>）原文。
        ''' </summary>
        Public Property Charge As String

        ''' <summary>
        ''' 质子层（<c>/p</c>）原文。
        ''' </summary>
        Public Property Proton As String

        ''' <summary>
        ''' 同位素层（<c>/i</c>）原文。
        ''' </summary>
        Public Property Isotope As String

        ''' <summary>
        ''' 四面体手性层（<c>/t</c>）原文。
        ''' </summary>
        Public Property Tetrahedral As String

        ''' <summary>
        ''' 立体化学层（<c>/b</c>、<c>/m</c>、<c>/s</c>）原文。
        ''' </summary>
        Public Property StereoLayer As String

        ''' <summary>
        ''' 各层原文的字典（层标记字符 → 内容），便于诊断与扩展。
        ''' </summary>
        Public ReadOnly Property Layers As New Dictionary(Of Char, String)

        ''' <summary>
        ''' 各组分在 <see cref="Elements"/> 中的起始下标。
        ''' </summary>
        Public ReadOnly Property ComponentOffsets As New List(Of (start As Integer, [end] As Integer))

        ''' <summary>
        ''' 各组分的形式电荷。
        ''' </summary>
        Public ReadOnly Property ComponentCharges As New List(Of Integer)

        ''' <summary>
        ''' 各组分额外添加的质子数。
        ''' </summary>
        Public ReadOnly Property ComponentProtons As New List(Of Integer)

        ''' <summary>
        ''' 连接层解析得到的键（全部为单键，键级在构建阶段还原）。
        ''' </summary>
        Public ReadOnly Property Bonds As New List(Of (a As Integer, b As Integer))

        ''' <summary>
        ''' 解析得到的氢分布（原子下标 → 氢个数）。
        ''' </summary>
        Public ReadOnly Property Hydrogens As New Dictionary(Of Integer, Integer)

        ''' <summary>
        ''' 解析得到的移动氢组。
        ''' </summary>
        Public ReadOnly Property MobileHydrogens As New List(Of MobileHydrogen)

        ''' <summary>
        ''' 解析得到的手性中心（原子下标 → 奇偶符号，<c>+1</c> / <c>-1</c>）。
        ''' </summary>
        Public ReadOnly Property StereoCenters As New Dictionary(Of Integer, Integer)

        ''' <summary>
        ''' 重原子总数。
        ''' </summary>
        Public ReadOnly Property HeavyAtomCount As Integer
            Get
                Return Elements.Count
            End Get
        End Property

        ''' <summary>
        ''' 生成用于调试的层清单描述。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"InChI {Version}: {Formula} ({Elements.Count} heavy atom(s))"
        End Function

    End Class

End Namespace
