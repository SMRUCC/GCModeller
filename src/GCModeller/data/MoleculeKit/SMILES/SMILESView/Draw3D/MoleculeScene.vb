Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Mol = SMRUCC.genomics.Molecule.SMILES.Graph

Namespace Draw3D

    ''' <summary>
    ''' 把分子图编译成一个可以直接交给相机投影的 3D 场景。
    ''' </summary>
    ''' <remarks>
    ''' 场景的所有面片都以分子质心为原点（<see cref="Camera"/> 的投影公式假定模型位于原点附近），
    ''' 并记录包围球半径以便自动取景。
    ''' </remarks>
    Public Class MoleculeScene

        ''' <summary>场景中的全部面片。</summary>
        Public Property Surfaces As Surface() = New Surface() {}

        ''' <summary>场景的包围球半径（Å）。</summary>
        Public Property Radius As Double = 1

        ''' <summary>渲染所使用的模式。</summary>
        Public Property Mode As RenderMode

        ''' <summary>
        ''' 依据分子图与渲染模式编译场景。
        ''' </summary>
        ''' <param name="graph">目标分子图（应已完成 3D 坐标重建）。</param>
        ''' <param name="mode">渲染模式。</param>
        ''' <param name="theme">绘制主题。</param>
        Public Shared Function Compile(graph As Mol.MoleculeGraph, mode As RenderMode, theme As Styling.ChemTheme) As MoleculeScene
            Dim scene As New MoleculeScene With {.Mode = mode}

            If graph.AtomCount = 0 Then Return scene

            If Not hasCoordinates(graph) Then Call graph.Embed3D()

            Dim cx As Double = 0
            Dim cy As Double = 0
            Dim cz As Double = 0

            For Each atom As Mol.Atom In graph.Atoms
                cx += atom.X
                cy += atom.Y
                cz += atom.Z
            Next

            cx /= graph.AtomCount
            cy /= graph.AtomCount
            cz /= graph.AtomCount

            Dim centers(graph.AtomCount - 1) As Point3D
            Dim radii(graph.AtomCount - 1) As Double
            Dim surfaces As New List(Of Surface)

            For i = 0 To graph.AtomCount - 1
                Dim atom As Mol.Atom = graph.Atoms(i)

                centers(i) = New Point3D(atom.X - cx, atom.Y - cy, atom.Z - cz)
                radii(i) = If(mode = RenderMode.SpaceFilling,
                    Mol.ElementTable.VdwRadius(atom.Element) * theme.SpaceFillScale,
                    Math.Max(0.22, Mol.ElementTable.CovalentRadius(atom.Element) * theme.BallScale))
            Next

            If mode = RenderMode.BallAndStick Then
                For Each bond As Mol.Bond In graph.Bonds
                    Dim colorA As Color = Styling.ElementColors.ColorOf(graph.Atoms(bond.a).Element)
                    Dim colorB As Color = Styling.ElementColors.ColorOf(graph.Atoms(bond.b).Element)

                    surfaces.AddRange(BondCylinderBuilder.Create(
                        centers(bond.a), centers(bond.b), theme.BondRadius,
                        colorA, colorB, theme.CylinderSegments, theme.CylinderRails))
                Next
            End If

            For i = 0 To graph.AtomCount - 1
                Dim color As Color = Styling.ElementColors.ColorOf(graph.Atoms(i).Element)

                surfaces.AddRange(SphereBuilder.Create(
                    centers(i), radii(i), color, theme.SphereStacks, theme.SphereSlices))
            Next

            scene.Surfaces = surfaces.ToArray()

            Dim radius As Double = 0

            For i = 0 To centers.Length - 1
                Dim distance As Double = centers(i).Length() + radii(i)

                If distance > radius Then radius = distance
            Next

            scene.Radius = Math.Max(radius, 0.5)

            Return scene
        End Function

        ''' <summary>
        ''' 依据场景的包围球与画布尺寸自动生成相机。
        ''' </summary>
        ''' <param name="scene">已编译的场景。</param>
        ''' <param name="size">画布尺寸。</param>
        ''' <param name="theme">绘制主题（提供旋转角与视场参数）。</param>
        Public Shared Function CreateCamera(scene As MoleculeScene, size As Size, theme As Styling.ChemTheme) As Camera
            Dim extent As Double = Math.Max(0.42 * Math.Min(size.Width, size.Height), 1)
            Dim distance As Double = scene.Radius * theme.FieldOfView / extent

            Dim camera As New Camera(New Point3D(theme.CameraAngleX, theme.CameraAngleY, theme.CameraAngleZ)) With {
                .Screen = size,
                .FieldOfView = theme.FieldOfView,
                .ViewDistance = CSng(Math.Max(distance, scene.Radius * 3)),
                .Offset = New PointF(0, 0),
                .AmbientStrength = theme.AmbientStrength
            }

            Return camera
        End Function

        Private Shared Function hasCoordinates(graph As Mol.MoleculeGraph) As Boolean
            Dim extent As Double = 0

            For Each atom As Mol.Atom In graph.Atoms
                extent += Math.Abs(atom.X) + Math.Abs(atom.Y) + Math.Abs(atom.Z)
            Next

            Return extent > 1.0E-6
        End Function

    End Class

End Namespace
