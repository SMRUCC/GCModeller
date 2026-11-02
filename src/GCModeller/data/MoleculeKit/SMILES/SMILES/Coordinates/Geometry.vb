Namespace Coordinates

    ''' <summary>
    ''' 轻量三维向量。库内只需要很少的向量运算，因此不引入外部线性代数依赖。
    ''' </summary>
    Public Structure Vec3

        ''' <summary>X 分量。</summary>
        Public Property X As Double
        ''' <summary>Y 分量。</summary>
        Public Property Y As Double
        ''' <summary>Z 分量。</summary>
        Public Property Z As Double

        ''' <summary>
        ''' 以三个分量创建一个向量。
        ''' </summary>
        Public Sub New(x As Double, y As Double, z As Double)
            Me.X = x
            Me.Y = y
            Me.Z = z
        End Sub

        ''' <summary>
        ''' 零向量。
        ''' </summary>
        Public Shared ReadOnly Zero As New Vec3(0, 0, 0)

        ''' <summary>
        ''' 向量长度。
        ''' </summary>
        Public ReadOnly Property Length As Double
            Get
                Return Math.Sqrt(X * X + Y * Y + Z * Z)
            End Get
        End Property

        ''' <summary>
        ''' 归一化为单位向量；零向量返回 <see cref="Zero"/>。
        ''' </summary>
        Public Function Normalize() As Vec3
            Dim len As Double = Length

            If len < 1.0E-12 Then Return Zero

            Return New Vec3(X / len, Y / len, Z / len)
        End Function

        ''' <summary>
        ''' 点积。
        ''' </summary>
        Public Shared Operator *(a As Vec3, b As Vec3) As Double
            Return a.X * b.X + a.Y * b.Y + a.Z * b.Z
        End Operator

        ''' <summary>
        ''' 向量加法。
        ''' </summary>
        Public Shared Operator +(a As Vec3, b As Vec3) As Vec3
            Return New Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z)
        End Operator

        ''' <summary>
        ''' 向量减法。
        ''' </summary>
        Public Shared Operator -(a As Vec3, b As Vec3) As Vec3
            Return New Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z)
        End Operator

        ''' <summary>
        ''' 数乘。
        ''' </summary>
        Public Shared Operator *(a As Vec3, k As Double) As Vec3
            Return New Vec3(a.X * k, a.Y * k, a.Z * k)
        End Operator

        ''' <summary>
        ''' 叉积。
        ''' </summary>
        Public Shared Function Cross(a As Vec3, b As Vec3) As Vec3
            Return New Vec3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X)
        End Function

        ''' <summary>
        ''' 两个向量之间的夹角（度）。
        ''' </summary>
        Public Shared Function Angle(a As Vec3, b As Vec3) As Double
            Dim la As Double = a.Length
            Dim lb As Double = b.Length

            If la < 1.0E-12 OrElse lb < 1.0E-12 Then Return 0

            Dim cos As Double = (a * b) / (la * lb)

            If cos > 1 Then cos = 1
            If cos < -1 Then cos = -1

            Return Math.Acos(cos) * 180.0 / Math.PI
        End Function

        ''' <summary>
        ''' 构造一个与给定单位向量垂直的任意单位向量。
        ''' </summary>
        Public Shared Function AnyPerpendicular(unit As Vec3) As Vec3
            Dim axis As Vec3

            If Math.Abs(unit.X) < 0.9 Then
                axis = New Vec3(1, 0, 0)
            Else
                axis = New Vec3(0, 1, 0)
            End If

            Dim result As Vec3 = Cross(unit, axis).Normalize()

            If result.Length < 1.0E-9 Then result = New Vec3(0, 0, 1)

            Return result
        End Function

        ''' <summary>
        ''' 绕任意轴旋转（罗德里格斯公式）。
        ''' </summary>
        ''' <param name="v">待旋转向量。</param>
        ''' <param name="axis">旋转轴（内部归一化）。</param>
        ''' <param name="angle">旋转角度（度）。</param>
        Public Shared Function Rotate(v As Vec3, axis As Vec3, angle As Double) As Vec3
            Dim k As Vec3 = axis.Normalize()

            If k.Length < 1.0E-12 Then Return v

            Dim rad As Double = angle * Math.PI / 180.0
            Dim cos As Double = Math.Cos(rad)
            Dim sin As Double = Math.Sin(rad)

            Return v * cos + Cross(k, v) * sin + k * ((k * v) * (1 - cos))
        End Function

        ''' <summary>
        ''' 三维点之间的距离。
        ''' </summary>
        Public Shared Function Distance(a As Vec3, b As Vec3) As Double
            Return (a - b).Length
        End Function

        ''' <summary>
        ''' 由三个向量组成的标量三重积，符号即四面体的手性。
        ''' </summary>
        Public Shared Function Triple(a As Vec3, b As Vec3, c As Vec3) As Double
            Return a * Cross(b, c)
        End Function

        ''' <summary>
        ''' 调试用的紧凑描述。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"({X:F3}, {Y:F3}, {Z:F3})"
        End Function

    End Structure

    ''' <summary>
    ''' 坐标重建使用的通用几何工具。
    ''' </summary>
    Public Module Geometry

        ''' <summary>
        ''' 读取某原子的三维坐标。
        ''' </summary>
        ''' <param name="graph">分子图。</param>
        ''' <param name="index">原子下标。</param>
        Public Function PointOf(graph As Graph.MoleculeGraph, index As Integer) As Vec3
            Dim atom As Graph.Atom = graph.Atoms(index)

            Return New Vec3(atom.X, atom.Y, atom.Z)
        End Function

        ''' <summary>
        ''' 把三维坐标写回原子。
        ''' </summary>
        ''' <param name="graph">分子图。</param>
        ''' <param name="index">原子下标。</param>
        ''' <param name="point">坐标值。</param>
        Public Sub SetPoint(graph As Graph.MoleculeGraph, index As Integer, point As Vec3)
            With graph.Atoms(index)
                .X = point.X
                .Y = point.Y
                .Z = point.Z
            End With
        End Sub

        ''' <summary>
        ''' 质心。
        ''' </summary>
        ''' <param name="points">点集。</param>
        Public Function Centroid(points As IList(Of Vec3)) As Vec3
            If points.Count = 0 Then Return Vec3.Zero

            Dim sum As Vec3 = Vec3.Zero

            For Each p As Vec3 In points
                sum += p
            Next

            Return sum * (1.0 / points.Count)
        End Function

        ''' <summary>
        ''' 点集到质心的最大距离（包围球半径）。
        ''' </summary>
        ''' <param name="points">点集。</param>
        Public Function BoundingRadius(points As IList(Of Vec3)) As Double
            If points.Count = 0 Then Return 0

            Dim center As Vec3 = Centroid(points)
            Dim radius As Double = 0

            For Each p As Vec3 In points
                Dim d As Double = (p - center).Length

                If d > radius Then radius = d
            Next

            Return radius
        End Function

        ''' <summary>
        ''' 把点集平移到质心位于原点。
        ''' </summary>
        ''' <param name="points">点集（原地修改）。</param>
        Public Sub CenterAtOrigin(points As IList(Of Vec3))
            Dim center As Vec3 = Centroid(points)

            For i = 0 To points.Count - 1
                points(i) = points(i) - center
            Next
        End Sub

    End Module

End Namespace
