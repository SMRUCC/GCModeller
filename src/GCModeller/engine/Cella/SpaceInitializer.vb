' ============================================================
' SpaceInitializer.vb - 特定形状的培养空间初始化
' ============================================================
' 提供四种形状：
'   1. 培养皿（扁平圆柱）
'   2. 发酵罐（圆柱）
'   3. 摇瓶（锥形瓶 = 圆锥过渡区 + 直筒颈部）
'   4. 发酵池（长方体）
'
' 形状之外的格点为 Nothing，Environment.Tick 会跳过它们。
' 每个格点持有一份**独立拷贝**的培养基，因此不同位置的细胞会看到不同的
' 营养浓度，摄取消耗也是局部的。
' ============================================================

Imports Microsoft.VisualBasic.Imaging

Public Module SpaceInitializer

    ''' <summary>
    ''' 1. 初始化培养皿（扁平圆形空间）
    ''' </summary>
    ''' <param name="radius">培养皿半径</param>
    ''' <param name="height">培养皿高度（通常较小，如2-5层）</param>
    Public Function CreatePetriDishSpace(radius As Integer, height As Integer,
                                         Optional medium As Dictionary(Of String, Double) = Nothing,
                                         Optional timeStep As Double = 1.0) As Environment
        ' 培养皿本质上是高度较小的圆柱体，直接复用圆柱体生成逻辑
        Return CreateCylinderSpace(radius, height, medium, timeStep)
    End Function

    ''' <summary>
    ''' 2. 初始化发酵罐（圆柱型空间）
    ''' </summary>
    ''' <param name="radius">圆柱半径</param>
    ''' <param name="height">圆柱总高度</param>
    Public Function CreateCylinderSpace(radius As Integer, height As Integer,
                                        Optional medium As Dictionary(Of String, Double) = Nothing,
                                        Optional timeStep As Double = 1.0) As Environment
        Dim env As Environment = NewEnvironment(medium, timeStep)
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
                        env.Space(x)(y)(z) = CreateSpot(x, y, z, medium)
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
    Public Function CreateFlaskSpace(bottomRadius As Integer, neckRadius As Integer,
                                     coneHeight As Integer, neckHeight As Integer,
                                     Optional medium As Dictionary(Of String, Double) = Nothing,
                                     Optional timeStep As Double = 1.0) As Environment
        Dim env As Environment = NewEnvironment(medium, timeStep)
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
                        env.Space(x)(y)(z) = CreateSpot(x, y, z, medium)
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
    Public Function CreateCuboidSpace(width As Integer, depth As Integer, height As Integer,
                                      Optional medium As Dictionary(Of String, Double) = Nothing,
                                      Optional timeStep As Double = 1.0) As Environment
        Dim env As Environment = NewEnvironment(medium, timeStep)

        env.Space = New Spot(width - 1)()() {}

        For x As Integer = 0 To width - 1
            env.Space(x) = New Spot(depth - 1)() {}
            For y As Integer = 0 To depth - 1
                env.Space(x)(y) = New Spot(height - 1) {}
                For z As Integer = 0 To height - 1
                    ' 长方体所有点都在范围内
                    env.Space(x)(y)(z) = CreateSpot(x, y, z, medium)
                Next
            Next
        Next

        Return env
    End Function

    ''' <summary>
    ''' 5. 初始化球形类器官空间（spheroid）
    ''' </summary>
    ''' <param name="radius">类器官半径（格点单位）</param>
    ''' <param name="shellRadius">
    ''' 归一化半径阈值：大于等于该值的格点视为「表层」，直接浸润在培养基中；
    ''' 内部格点的初始培养基乘以 <paramref name="coreRetention"/>，
    ''' 营养必须靠扩散从表层渗入 —— 这正是类器官出现营养/氧梯度与
    ''' 中心坏死核的物理来源
    ''' </param>
    ''' <param name="coreRetention">核心区初始培养基的保留比例（0 = 初始为空）</param>
    Public Function CreateSpheroidSpace(radius As Integer,
                                        Optional medium As Dictionary(Of String, Double) = Nothing,
                                        Optional timeStep As Double = 1.0,
                                        Optional shellRadius As Double = 0.72,
                                        Optional coreRetention As Double = 0.0) As Environment
        Return CreateEllipsoidSpace(radius, radius, radius, medium, timeStep, shellRadius, coreRetention)
    End Function

    ''' <summary>
    ''' 6. 初始化椭球类器官空间（半径各轴可不同）
    ''' </summary>
    Public Function CreateEllipsoidSpace(radiusX As Integer, radiusY As Integer, radiusZ As Integer,
                                         Optional medium As Dictionary(Of String, Double) = Nothing,
                                         Optional timeStep As Double = 1.0,
                                         Optional shellRadius As Double = 0.72,
                                         Optional coreRetention As Double = 0.0) As Environment
        Dim env As Environment = NewEnvironment(medium, timeStep)

        Dim sizeX As Integer = 2 * radiusX + 1
        Dim sizeY As Integer = 2 * radiusY + 1
        Dim sizeZ As Integer = 2 * radiusZ + 1

        env.Space = New Spot(sizeX - 1)()() {}

        For x As Integer = 0 To sizeX - 1
            env.Space(x) = New Spot(sizeY - 1)() {}
            For y As Integer = 0 To sizeY - 1
                env.Space(x)(y) = New Spot(sizeZ - 1) {}
                For z As Integer = 0 To sizeZ - 1
                    Dim dx As Double = (x - radiusX) / CDbl(System.Math.Max(1, radiusX))
                    Dim dy As Double = (y - radiusY) / CDbl(System.Math.Max(1, radiusY))
                    Dim dz As Double = (z - radiusZ) / CDbl(System.Math.Max(1, radiusZ))
                    Dim r As Double = System.Math.Sqrt(dx * dx + dy * dy + dz * dz)

                    If r <= 1.0 Then
                        ' 表层直接接触培养基；内部按 coreRetention 缩放
                        Dim scale As Double = If(r >= shellRadius, 1.0, coreRetention)

                        env.Space(x)(y)(z) = CreateSpot(x, y, z, medium, r, scale)
                    Else
                        env.Space(x)(y)(z) = Nothing
                    End If
                Next
            Next
        Next

        Return env
    End Function

    ''' <summary>
    ''' 构造一份培养基（每个格点会拿到一份独立拷贝）
    ''' </summary>
    Public Function CreateMedium(ParamArray components As (id As String, level As Double)()) As Dictionary(Of String, Double)
        Dim medium As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        For Each c In components
            medium(c.id) = c.level
        Next

        Return medium
    End Function

    Private Function NewEnvironment(medium As Dictionary(Of String, Double), timeStep As Double) As Environment
        Dim env As New Environment() With {
            .TimeStep = If(timeStep > 0, timeStep, 1.0)
        }

        If medium IsNot Nothing Then
            ' 记录培养基模板：贴壁补料按此模板回补
            env.MediumTemplate = New Dictionary(Of String, Double)(medium, StringComparer.OrdinalIgnoreCase)
        End If

        Return env
    End Function

    ''' <summary>
    ''' 辅助方法：用于实例化一个Spot对象并赋予空间索引与培养基
    ''' </summary>
    ''' <param name="radius">归一化径向位置（0 = 球心，1 = 最外层）；非球形空间传 -1</param>
    ''' <param name="scale">初始培养基的缩放系数（类器官核心区可设为 0）</param>
    Private Function CreateSpot(x As Integer, y As Integer, z As Integer,
                                medium As Dictionary(Of String, Double),
                                Optional radius As Double = -1.0,
                                Optional scale As Double = 1.0) As Spot

        Dim local As Dictionary(Of String, Double) = Nothing

        If medium IsNot Nothing Then
            ' 每个格点一份独立拷贝，摄取消耗才是局部的
            local = New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            For Each item In medium
                local(item.Key) = item.Value * System.Math.Max(0.0, scale)
            Next
        End If

        Return New Spot() With {
            .index = New SpatialIndex3D() With {
                .X = x,
                .Y = y,
                .Z = z
            },
            .cells = New List(Of VirtualCella)(),
            .Medium = local,
            .NormalizedRadius = radius
        }
    End Function

End Module
