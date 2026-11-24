Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports Brush = Microsoft.VisualBasic.Imaging.Brush

Namespace Draw3D

    ''' <summary>
    ''' 键圆柱生成器：把一条化学键渲染成分段的多边形柱体。
    ''' </summary>
    ''' <remarks>
    ''' 柱体沿轴向被切成若干段，每段独立成为一个 <see cref="Surface"/>。
    ''' 这一点很重要：画家算法按面片的平均 Z 排序，若整根键只有一个面片，
    ''' 它会整片压在原子球之前或之后，出现明显的穿插错误。
    ''' 两个半边可以分别使用两端原子的颜色，形成常见的"分色键"效果。
    ''' </remarks>
    Public Module BondCylinderBuilder

        ''' <summary>
        ''' 生成一条键的圆柱面片集合。
        ''' </summary>
        ''' <param name="a">一端原子坐标。</param>
        ''' <param name="b">另一端原子坐标。</param>
        ''' <param name="radius">圆柱半径（Å）。</param>
        ''' <param name="colorA">起点端的颜色。</param>
        ''' <param name="colorB">终点端的颜色。</param>
        ''' <param name="segments">沿轴向的分段数。</param>
        ''' <param name="rails">环向分段数。</param>
        Public Function Create(a As Point3D, b As Point3D, radius As Double,
                               colorA As Color, colorB As Color,
                               Optional segments As Integer = 8,
                               Optional rails As Integer = 6) As Surface()

            If segments < 1 Then segments = 1
            If rails < 3 Then rails = 3

            Dim axis As Point3D = (b - a).Normalize()

            If axis.Length < 1.0E-9 Then Return New Surface() {}

            Dim helper As Point3D

            If Math.Abs(axis.Z) < 0.9 Then
                helper = New Point3D(0, 0, 1)
            Else
                helper = New Point3D(0, 1, 0)
            End If

            Dim u As Point3D = Point3D.Cross(axis, helper).Normalize()
            Dim v As Point3D = Point3D.Cross(axis, u).Normalize()

            Dim rings(segments, rails - 1) As Point3D

            For s = 0 To segments
                Dim t As Double = s / segments
                Dim center As Point3D = New Point3D(
                    a.X + (b.X - a.X) * t,
                    a.Y + (b.Y - a.Y) * t,
                    a.Z + (b.Z - a.Z) * t)

                For k = 0 To rails - 1
                    Dim angle As Double = 2 * Math.PI * k / rails
                    Dim offset As Point3D = u.Multiply(Math.Cos(angle)).Add(v.Multiply(Math.Sin(angle)))

                    rings(s, k) = New Point3D(
                        center.X + offset.X * radius,
                        center.Y + offset.Y * radius,
                        center.Z + offset.Z * radius)
                Next
            Next

            Dim brushA As Brush = New SolidBrush(colorA)
            Dim brushB As Brush = New SolidBrush(colorB)
            Dim surfaces(segments * rails - 1) As Surface
            Dim index As Integer = 0

            For s = 0 To segments - 1
                Dim brush As Brush = If(s * 2 < segments, brushA, brushB)

                For k = 0 To rails - 1
                    Dim nextK As Integer = (k + 1) Mod rails

                    surfaces(index) = New Surface(
                        New Point3D() {
                            rings(s, k),
                            rings(s + 1, k),
                            rings(s + 1, nextK),
                            rings(s, nextK)
                        }, brush)

                    index += 1
                Next
            Next

            Return surfaces
        End Function

    End Module

End Namespace
