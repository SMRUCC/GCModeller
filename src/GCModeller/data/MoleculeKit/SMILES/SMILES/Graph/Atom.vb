Imports Imaging = Microsoft.VisualBasic.Imaging

Namespace Graph

    ''' <summary>
    ''' 分子图中的一个原子。同时携带 2D 与 3D 坐标，坐标由
    ''' <see cref="MoleculeGraph.Layout2D"/> / <see cref="MoleculeGraph.Embed3D"/> 写入。
    ''' </summary>
    ''' <remarks>
    ''' 实现 <see cref="Microsoft.VisualBasic.Imaging.PointF3D"/> 的目的是让视图层可以直接把原子
    ''' 集合交给 <c>Drawing3D</c> 的点云/相机 API（<c>Extensions.LoadVector3D</c>），
    ''' 避免在渲染层再写一遍坐标搬运代码。
    ''' </remarks>
    Public Class Atom
        Implements Imaging.PointF3D

        ''' <summary>
        ''' 原子在分子图中的 0 基下标，与 <see cref="MoleculeGraph.Atoms"/> 的位置一致。
        ''' </summary>
        Public Property Index As Integer

        ''' <summary>
        ''' 元素符号（标准大小写，如 <c>C</c>、<c>Cl</c>、<c>Na</c>）。
        ''' </summary>
        Public Property Element As String = "C"

        ''' <summary>
        ''' 同位素质量数；<c>0</c> 表示按天然丰度处理。
        ''' </summary>
        Public Property Isotope As Integer

        ''' <summary>
        ''' 形式电荷（电子数差值，如 <c>+1</c>、<c>-2</c>）。
        ''' </summary>
        Public Property Charge As Integer

        ''' <summary>
        ''' 结构式记号中显式声明的氢原子个数（SMILES 方括号原子、InChI 的氢层）。
        ''' 注意这与"作为独立原子出现在图中的氢"是两种不同的表示，统计总数请使用
        ''' <see cref="MoleculeGraph.TotalH"/>。
        ''' </summary>
        Public Property ExplicitH As Integer

        ''' <summary>
        ''' 该原子是否来自"氢数被显式固定"的记号（SMILES 的 <c>[...]</c>、InChI 的全部原子）。
        ''' 为 <c>True</c> 时不再按价态补隐式氢。
        ''' </summary>
        Public Property NoImplicitH As Boolean

        ''' <summary>
        ''' 该原子是否参与芳香体系（在 SMILES 中以小写字母书写，或由 InChI 推断）。
        ''' </summary>
        Public Property IsAromatic As Boolean

        ''' <summary>
        ''' SMILES 中的原子类别（<c>:n</c> 记号），<c>0</c> 表示未指定。
        ''' </summary>
        Public Property AtomClass As Integer

        ''' <summary>
        ''' 四面体手性标记。
        ''' </summary>
        Public Property Stereo As AtomStereo

        ''' <summary>
        ''' 手性记号的原始文本（如 <c>@@</c>、<c>@TH1</c>），便于原样回写与诊断。
        ''' </summary>
        Public Property StereoText As String

        ''' <summary>
        ''' 结构式中出现的邻接原子顺序（用于按 SMILES 约定还原四面体构型）。
        ''' 缺少第 4 个取代基时以 <c>-1</c> 表示隐式氢。
        ''' </summary>
        Public Property StereoNeighbors As Integer()

        ''' <summary>
        ''' 该原子是否由 <see cref="MoleculeGraph.AddHydrogens"/> 生成的氢原子。
        ''' 绘图时可以据此决定是否隐藏。
        ''' </summary>
        Public Property IsHydrogen As Boolean

        ''' <summary>
        ''' 2D 平面坐标的 X 分量（布局完成后键长被归一化为 1）。
        ''' </summary>
        Public Property X As Double Implements Imaging.Layout2D.X

        ''' <summary>
        ''' 2D 平面坐标的 Y 分量（化学坐标系，Y 轴向上；渲染时再翻转为屏幕坐标）。
        ''' </summary>
        Public Property Y As Double Implements Imaging.Layout2D.Y

        ''' <summary>
        ''' 3D 坐标的 Z 分量，单位为埃（Å）。
        ''' </summary>
        Public Property Z As Double Implements Imaging.PointF3D.Z

        ''' <summary>
        ''' 创建一个未指定元素的空原子。
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' 以元素符号与形式电荷创建一个原子。
        ''' </summary>
        ''' <param name="element">元素符号。</param>
        ''' <param name="charge">形式电荷。</param>
        Public Sub New(element As String, Optional charge As Integer = 0)
            Me.Element = If(String.IsNullOrEmpty(element), "C", element)
            Me.Charge = charge
        End Sub

        ''' <summary>
        ''' 是否为氢（或氘/氚）。
        ''' </summary>
        Public ReadOnly Property IsH As Boolean
            Get
                Return Element = "H" OrElse Element = "D" OrElse Element = "T"
            End Get
        End Property

        ''' <summary>
        ''' 该原子的元素符号在显示时是否需要连同氢数/电荷一起标注（即非碳原子）。
        ''' </summary>
        Public ReadOnly Property NeedsLabel As Boolean
            Get
                Return Not IsH
            End Get
        End Property

        ''' <summary>
        ''' 生成用于调试的紧凑描述，例如 <c>#3 N(+1)</c>。
        ''' </summary>
        Public Overrides Function ToString() As String
            Dim text As String = $"#{Index} {Element}"

            If Charge <> 0 Then text &= $"({If(Charge > 0, "+", "")}{Charge})"
            If ExplicitH > 0 Then text &= $"H{ExplicitH}"

            Return text
        End Function

    End Class

End Namespace
