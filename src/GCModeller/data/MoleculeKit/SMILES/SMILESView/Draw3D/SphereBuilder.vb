Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports Brush = Microsoft.VisualBasic.Imaging.Brush

Namespace Draw3D

    ''' <summary>
    ''' 球体面片生成器：把原子渲染成经纬网格构成的多面体。
    ''' </summary>
    ''' <remarks>
    ''' 单位球只有一份顶点表与面索引表（按经纬分段数缓存），每个原子只需要做一次
    ''' 缩放 + 平移，避免 <c>N × stacks × slices</c> 次三角函数运算。
    ''' 面片以四边形为单位交给 <see cref="Surface"/>，从而可以参与画家算法的深度排序与光照。
    ''' </remarks>
    Public Module SphereBuilder

        Private ReadOnly templates As New Dictionary(Of String, (vertices As Point3D(), faces As Integer()()))

        ''' <summary>
        ''' 生成一个球体的面片集合。
        ''' </summary>
        ''' <param name="center">球心。</param>
        ''' <param name="radius">半径（Å）。</param>
        ''' <param name="color">基础颜色。</param>
        ''' <param name="stacks">纬度分段数。</param>
        ''' <param name="slices">经度分段数。</param>
        Public Function Create(center As Point3D, radius As Double, color As Color,
                               Optional stacks As Integer = 8,
                               Optional slices As Integer = 14) As Surface()

            If stacks < 2 Then stacks = 2
            If slices < 3 Then slices = 3

            Dim template = getTemplate(stacks, slices)
            Dim unit As Point3D() = template.vertices
            Dim vertices(unit.Length - 1) As Point3D

            For i = 0 To unit.Length - 1
                vertices(i) = New Point3D(
                    center.X + unit(i).X * radius,
                    center.Y + unit(i).Y * radius,
                    center.Z + unit(i).Z * radius)
            Next

            Dim brush As Brush = New SolidBrush(color)
            Dim faces As Integer()() = template.faces
            Dim surfaces(faces.Length - 1) As Surface

            For f = 0 To faces.Length - 1
                Dim index As Integer() = faces(f)
                Dim polygon(index.Length - 1) As Point3D

                For k = 0 To index.Length - 1
                    polygon(k) = vertices(index(k))
                Next

                surfaces(f) = New Surface(polygon, brush)
            Next

            Return surfaces
        End Function

        Private Function getTemplate(stacks As Integer, slices As Integer) As (vertices As Point3D(), faces As Integer()())
            Dim key As String = $"{stacks}x{slices}"

            SyncLock templates
                If templates.ContainsKey(key) Then Return templates(key)
            End SyncLock

            Dim vertices((stacks + 1) * slices - 1) As Point3D

            For i = 0 To stacks
                Dim theta As Double = Math.PI * i / stacks
                Dim sinTheta As Double = Math.Sin(theta)
                Dim cosTheta As Double = Math.Cos(theta)

                For j = 0 To slices - 1
                    Dim phi As Double = 2 * Math.PI * j / slices
                    Dim x As Double = sinTheta * Math.Cos(phi)
                    Dim z As Double = sinTheta * Math.Sin(phi)

                    vertices(i * slices + j) = New Point3D(x, cosTheta, z)
                Next
            Next

            Dim faces(stacks * slices - 1)() As Integer

            For i = 0 To stacks - 1
                For j = 0 To slices - 1
                    Dim nextJ As Integer = (j + 1) Mod slices

                    faces(i * slices + j) = New Integer() {
                        i * slices + j,
                        (i + 1) * slices + j,
                        (i + 1) * slices + nextJ,
                        i * slices + nextJ
                    }
                Next
            Next

            Dim template As (vertices As Point3D(), faces As Integer()()) = (vertices, faces)

            SyncLock templates
                templates(key) = template
            End SyncLock

            Return template
        End Function

    End Module

End Namespace
