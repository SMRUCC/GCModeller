Namespace Styling

    ''' <summary>
    ''' 分子结构图的绘制主题：配色、字体、线宽与 3D 渲染参数。
    ''' </summary>
    ''' <remarks>
    ''' 颜色一律以 <c>RRGGBB</c> 十六进制字符串保存，再由视图层转换为
    ''' <see cref="System.Drawing.Color"/>，这样主题对象本身不依赖具体的绘图后端。
    ''' </remarks>
    Public Class ChemTheme

        ''' <summary>画布背景色。</summary>
        Public Property BackgroundColor As String = "FFFFFF"
        ''' <summary>化学键的主色。</summary>
        Public Property BondColor As String = "222222"
        ''' <summary>化学键的线宽（像素）。</summary>
        Public Property BondWidth As Single = 2.0
        ''' <summary>双键/叁键平行线之间的间距（像素）。</summary>
        Public Property BondSeparation As Single = 4.0
        ''' <summary>原子标签的字体名。</summary>
        Public Property FontName As String = "Arial"
        ''' <summary>原子标签的字号（像素）。</summary>
        Public Property AtomFontSize As Single = 16
        ''' <summary>标题字号（像素）。</summary>
        Public Property TitleFontSize As Single = 15
        ''' <summary>画布四周的留白（像素）。</summary>
        Public Property Padding As Integer = 28
        ''' <summary>是否在图像顶部绘制结构式原文。</summary>
        Public Property ShowTitle As Boolean = True
        ''' <summary>标题文本的颜色。</summary>
        Public Property TitleColor As String = "808080"
        ''' <summary>原子标签底衬的透明程度（0 完全透明，255 完全不透明）。</summary>
        Public Property LabelHaloAlpha As Integer = 235
        ''' <summary>是否隐藏碳原子标签（键线式画法）。</summary>
        Public Property HideCarbon As Boolean = True

        ''' <summary>球棍模型中原子球的半径与共价半径之比。</summary>
        Public Property BallScale As Double = 0.38
        ''' <summary>空间填充模型中原子球的半径与范德华半径之比。</summary>
        Public Property SpaceFillScale As Double = 1.0
        ''' <summary>球棍模型中键圆柱的半径（Å）。</summary>
        Public Property BondRadius As Double = 0.11
        ''' <summary>3D 相机的旋转角（欧拉角，度）。</summary>
        Public Property CameraAngleX As Single = 18
        ''' <summary>3D 相机的旋转角（欧拉角，度）。</summary>
        Public Property CameraAngleY As Single = -32
        ''' <summary>3D 相机的旋转角（欧拉角，度）。</summary>
        Public Property CameraAngleZ As Single = 0
        ''' <summary>3D 场景的环境光强度。</summary>
        Public Property AmbientStrength As Double = 0.35
        ''' <summary>3D 场景的透视视场（Drawing3D 的 fov 参数）。</summary>
        Public Property FieldOfView As Single = 320
        ''' <summary>3D 球体的纬度分段数。</summary>
        Public Property SphereStacks As Integer = 8
        ''' <summary>3D 球体的经度分段数。</summary>
        Public Property SphereSlices As Integer = 14
        ''' <summary>3D 键圆柱沿轴向的分段数（影响画家算法的遮挡正确性）。</summary>
        Public Property CylinderSegments As Integer = 8
        ''' <summary>3D 键圆柱的环向分段数。</summary>
        Public Property CylinderRails As Integer = 6

        ''' <summary>
        ''' 创建一个使用默认参数的主题。
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' 当主题未显式给出时提供一个共享的默认实例。
        ''' </summary>
        Public Shared ReadOnly Property [Default] As ChemTheme
            Get
                Return _default
            End Get
        End Property

        Private Shared ReadOnly _default As New ChemTheme

    End Class

End Namespace
