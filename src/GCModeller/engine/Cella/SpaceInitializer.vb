Imports Microsoft.VisualBasic.Imaging

Public Module SpaceInitializer

    ''' <summary>
    ''' 1. 初始化培养皿（扁平圆形空间）
    ''' </summary>
    ''' <param name="radius">培养皿半径</param>
    ''' <param name="height">培养皿高度（通常较小，如2-5层）</param>
    Public Function CreatePetriDishSpace(radius As Integer, height As Integer) As Environment
        ' 培养皿本质上是高度较小的圆柱体，直接复用圆柱体生成逻辑
        Return CreateCylinderSpace(radius, height)
    End Function

    ''' <summary>
    ''' 2. 初始化发酵罐（圆柱型空间）
    ''' </summary>
    ''' <param name="radius">圆柱半径</param>
    ''' <param name="height">圆柱总高度</param>
    Public Function CreateCylinderSpace(radius As Integer, height As Integer) As Environment
        Dim env As New Environment()
        Dim sizeXY As Integer = 2 * radius + 1 ' 确保圆心在正中心，奇数边长

        ' 初始化3D交错数组边界框
        env.Space = New Spot(sizeXY - 1)()() {}

        For x As Integer = 0 To sizeXY - 1
            env.Space(x) = New Spot(sizeXY - 1)() {}
            For y As Integer = 0 To sizeXY - 1
                env.Space(x)(y) = New Spot(height - 1) {}
                For z As Integer = 0 To height - 1
                    ' 计算当前点到圆心的距离的平方
                    Dim dx As Integer = x - radius
                    Dim dy As Integer = y - radius
                    If dx * dx + dy * dy <= radius * radius Then
                        ' 在圆柱体内，生成Spot
                        env.Space(x)(y)(z) = CreateSpot(x, y, z)
                    Else
                        ' 在圆柱体外，设为Nothing
                        env.Space(x)(y)(z) = Nothing
                    End If
                Next
            Next
        Next

        Return env
    End Function

    ''' <summary>
    ''' 3. 初始化摇瓶（锥形瓶空间）
    ''' </summary>
    ''' <param name="bottomRadius">底部半径</param>
    ''' <param name="neckRadius">颈部半径</param>
    ''' <param name="coneHeight">锥形过渡区高度</param>
    ''' <param name="neckHeight">直筒颈部高度</param>
    Public Function CreateFlaskSpace(bottomRadius As Integer, neckRadius As Integer, coneHeight As Integer, neckHeight As Integer) As Environment
        Dim env As New Environment()
        Dim totalHeight As Integer = coneHeight + neckHeight
        Dim sizeXY As Integer = 2 * bottomRadius + 1

        env.Space = New Spot(sizeXY - 1)()() {}

        For x As Integer = 0 To sizeXY - 1
            env.Space(x) = New Spot(sizeXY - 1)() {}
            For y As Integer = 0 To sizeXY - 1
                env.Space(x)(y) = New Spot(totalHeight - 1) {}
                For z As Integer = 0 To totalHeight - 1
                    Dim dx As Integer = x - bottomRadius
                    Dim dy As Integer = y - bottomRadius
                    Dim distSq As Integer = dx * dx + dy * dy

                    Dim currentRadius As Double
                    If z < coneHeight Then
                        ' 锥形部分：半径随高度线性递减
                        Dim ratio As Double = z / coneHeight
                        currentRadius = bottomRadius - (bottomRadius - neckRadius) * ratio
                    Else
                        ' 颈部直筒部分：半径固定
                        currentRadius = neckRadius
                    End If

                    If distSq <= currentRadius * currentRadius Then
                        env.Space(x)(y)(z) = CreateSpot(x, y, z)
                    Else
                        env.Space(x)(y)(z) = Nothing
                    End If
                Next
            Next
        Next

        Return env
    End Function

    ''' <summary>
    ''' 4. 初始化大型发酵池（长方体空间）
    ''' </summary>
    ''' <param name="width">X轴长度</param>
    ''' <param name="depth">Y轴深度</param>
    ''' <param name="height">Z轴高度</param>
    Public Function CreateCuboidSpace(width As Integer, depth As Integer, height As Integer) As Environment
        Dim env As New Environment()

        env.Space = New Spot(width - 1)()() {}

        For x As Integer = 0 To width - 1
            env.Space(x) = New Spot(depth - 1)() {}
            For y As Integer = 0 To depth - 1
                env.Space(x)(y) = New Spot(height - 1) {}
                For z As Integer = 0 To height - 1
                    ' 长方体所有点都在范围内
                    env.Space(x)(y)(z) = CreateSpot(x, y, z)
                Next
            Next
        Next

        Return env
    End Function

    ''' <summary>
    ''' 辅助方法：用于实例化一个Spot对象并赋予空间索引
    ''' </summary>
    Private Function CreateSpot(x As Integer, y As Integer, z As Integer) As Spot
        Return New Spot() With {
            .index = New SpatialIndex3D() With {
                .X = x,
                .Y = y,
                .Z = z
            },
            .cells = New List(Of VirtualCella)(),
            .external = Nothing ' 外部引用在生成时默认为空，后续业务逻辑中可自行绑定
        }
    End Function

End Module