Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Mol = SMRUCC.genomics.Molecule.SMILES.Graph
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports FontStyle = System.Drawing.FontStyle

Namespace Draw3D

    ''' <summary>
    ''' 3D 分子结构图渲染器：球棍模型与空间填充模型。
    ''' </summary>
    ''' <remarks>
    ''' 渲染管线完全复用 <c>Microsoft.VisualBasic.Imaging.Drawing3D</c>：
    ''' <see cref="Camera.Rotate(IEnumerable(Of Surface))"/> 完成欧拉角旋转，
    ''' <c>PainterAlgorithm.SurfacePainter</c> 完成透视投影、Lambert 光照与按平均 Z 的画家算法排序。
    ''' 由于走的是 <see cref="IGraphics"/> 重载，同一套代码可以输出 PNG 位图或 SVG 矢量图。
    ''' </remarks>
    Public Class MoleculeDraw3D

        Private ReadOnly graph As Mol.MoleculeGraph
        Private ReadOnly theme As Styling.ChemTheme

        ''' <summary>
        ''' 以一个分子图创建渲染器。
        ''' </summary>
        ''' <param name="graph">目标分子图。</param>
        ''' <param name="theme">绘制主题，为空时使用默认主题。</param>
        Public Sub New(graph As Mol.MoleculeGraph, Optional theme As Styling.ChemTheme = Nothing)
            If graph Is Nothing Then Throw New ArgumentNullException(NameOf(graph))

            Me.graph = graph
            Me.theme = If(theme, Styling.ChemTheme.Default)
        End Sub

        ''' <summary>
        ''' 绘制到指定画布。
        ''' </summary>
        ''' <param name="canvas">目标画布。</param>
        ''' <param name="mode">渲染模式（球棍 / 空间填充）。</param>
        ''' <param name="camera">相机；为空时按场景包围球自动取景。</param>
        Public Sub Draw(canvas As IGraphics, Optional mode As RenderMode = RenderMode.BallAndStick, Optional camera As Camera = Nothing)
            If graph.AtomCount = 0 Then Return

            Dim scene As MoleculeScene = MoleculeScene.Compile(graph, mode, theme)
            Dim view As Camera = If(camera, MoleculeScene.CreateCamera(scene, canvas.Size, theme))
            Dim rotated As IEnumerable(Of Surface) = view.Rotate(scene.Surfaces)

            Call canvas.SurfacePainter(view, rotated, drawPath:=False, illumination:=True)
            Call drawCaption(canvas, mode)
        End Sub

        ''' <summary>
        ''' 编译场景但暂不绘制（供需要复用相机或做二次加工的场景使用）。
        ''' </summary>
        ''' <param name="mode">渲染模式。</param>
        Public Function BuildScene(Optional mode As RenderMode = RenderMode.BallAndStick) As MoleculeScene
            Return MoleculeScene.Compile(graph, mode, theme)
        End Function

        Private Sub drawCaption(canvas As IGraphics, mode As RenderMode)
            Dim caption As String = $"{graph.MolecularFormula()}  -  {If(mode = RenderMode.SpaceFilling, "space filling", "ball and stick")}"
            Dim font As New Font(theme.FontName, theme.TitleFontSize, FontStyle.Regular)
            Dim color As Color = Styling.ElementColors.ParseHex(theme.TitleColor, Color.Gray)

            Call canvas.DrawString(caption, font, New SolidBrush(color),
                CSng(theme.Padding), CSng(canvas.Size.Height - theme.Padding))
        End Sub

    End Class

End Namespace
