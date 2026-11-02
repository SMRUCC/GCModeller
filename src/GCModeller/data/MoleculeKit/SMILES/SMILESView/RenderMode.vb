''' <summary>
''' 3D 分子结构图的渲染模式。
''' </summary>
Public Enum RenderMode

    ''' <summary>
    ''' 球棍模型：原子画成小球（共价半径缩放），键画成圆柱。
    ''' </summary>
    BallAndStick = 0

    ''' <summary>
    ''' 空间填充模型：原子按范德华半径画成实心球，不绘制键。
    ''' </summary>
    SpaceFilling = 1

End Enum
