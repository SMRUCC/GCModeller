Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Imaging.SVG
Imports SMRUCC.genomics.Molecule.SMILES.Styling
Imports Mol = SMRUCC.genomics.Molecule.SMILES.Graph

''' <summary>
''' 一站式门面：结构式文本 → 分子图 → 坐标 → 2D/3D 图像。
''' </summary>
''' <remarks>
''' <code>
''' Using viewer = StructureViewer.FromText("c1ccccc1O")
'''     Call viewer.SavePng2D("phenol-2d.png", New Size(640, 480))
'''     Call viewer.SavePng3D("phenol-3d.png", New Size(640, 480), RenderMode.BallAndStick)
''' End Using
''' </code>
''' 输出既支持光栅位图（GDI+）也支持矢量图（SVG），二者共用同一套绘制代码。
''' </remarks>
Public Class StructureViewer

    ''' <summary>
    ''' 被查看的分子图。
    ''' </summary>
    Public Property Graph As Mol.MoleculeGraph

    ''' <summary>
    ''' 绘制主题。
    ''' </summary>
    Public Property Theme As ChemTheme

    ''' <summary>
    ''' 以一个已有的分子图创建查看器。
    ''' </summary>
    ''' <param name="graph">分子图。</param>
    ''' <param name="theme">绘制主题，为空时使用默认主题。</param>
    Public Sub New(graph As Mol.MoleculeGraph, Optional theme As ChemTheme = Nothing)
        If graph Is Nothing Then Throw New ArgumentNullException(NameOf(graph))

        Me.Graph = graph
        Me.Theme = If(theme, ChemTheme.Default)
    End Sub

    ''' <summary>
    ''' 由结构式字符串（SMILES / InChI，自动识别）创建查看器。
    ''' </summary>
    ''' <param name="text">结构式文本。</param>
    ''' <param name="theme">绘制主题。</param>
    Public Shared Function FromText(text As String, Optional theme As ChemTheme = Nothing) As StructureViewer
        Return New StructureViewer(MoleculeIO.Parse(text), theme)
    End Function

    ''' <summary>
    ''' 由结构式文本文件创建查看器。
    ''' </summary>
    ''' <param name="path">文本文件路径。</param>
    ''' <param name="theme">绘制主题。</param>
    Public Shared Function FromFile(path As String, Optional theme As ChemTheme = Nothing) As StructureViewer
        Return New StructureViewer(MoleculeIO.ParseFile(path), theme)
    End Function

#Region "渲染"

    ''' <summary>
    ''' 渲染 2D 结构图（调用方负责释放返回的画布）。
    ''' </summary>
    ''' <param name="size">画布尺寸。</param>
    Public Function Render2D(size As Size) As IGraphics
        Dim canvas As IGraphics = DriverLoad.CreateDefaultRasterGraphics(
            size, fill_color:=ElementColors.ParseHex(Theme.BackgroundColor, Color.White))

        Call New Draw2D.MoleculeDraw2D(Graph, Theme).Draw(canvas)

        Return canvas
    End Function

    ''' <summary>
    ''' 渲染 3D 结构图（调用方负责释放返回的画布）。
    ''' </summary>
    ''' <param name="size">画布尺寸。</param>
    ''' <param name="mode">渲染模式。</param>
    ''' <param name="camera">相机；为空时自动取景。</param>
    Public Function Render3D(size As Size, Optional mode As RenderMode = RenderMode.BallAndStick, Optional camera As Camera = Nothing) As IGraphics
        Dim canvas As IGraphics = DriverLoad.CreateDefaultRasterGraphics(
            size, fill_color:=ElementColors.ParseHex(Theme.BackgroundColor, Color.White))

        Call New Draw3D.MoleculeDraw3D(Graph, Theme).Draw(canvas, mode, camera)

        Return canvas
    End Function

    ''' <summary>
    ''' 把 2D 结构图保存为 PNG。
    ''' </summary>
    ''' <param name="path">输出文件路径。</param>
    ''' <param name="size">画布尺寸。</param>
    Public Function SavePng2D(path As String, Optional size As Size = Nothing) As Boolean
        Return savePng(path, If(size = Nothing, New Size(720, 560), size),
            Sub(canvas) Call New Draw2D.MoleculeDraw2D(Graph, Theme).Draw(canvas))
    End Function

    ''' <summary>
    ''' 把 3D 结构图保存为 PNG。
    ''' </summary>
    ''' <param name="path">输出文件路径。</param>
    ''' <param name="size">画布尺寸。</param>
    ''' <param name="mode">渲染模式。</param>
    ''' <param name="camera">相机；为空时自动取景。</param>
    Public Function SavePng3D(path As String, Optional size As Size = Nothing,
                              Optional mode As RenderMode = RenderMode.BallAndStick,
                              Optional camera As Camera = Nothing) As Boolean
        Return savePng(path, If(size = Nothing, New Size(720, 560), size),
            Sub(canvas) Call New Draw3D.MoleculeDraw3D(Graph, Theme).Draw(canvas, mode, camera))
    End Function

    ''' <summary>
    ''' 把 2D 结构图保存为 SVG 矢量图。
    ''' </summary>
    ''' <param name="path">输出文件路径。</param>
    ''' <param name="size">画布尺寸。</param>
    Public Function SaveSvg2D(path As String, Optional size As Size = Nothing) As Boolean
        Return saveSvg(path, If(size = Nothing, New Size(720, 560), size),
            Sub(canvas) Call New Draw2D.MoleculeDraw2D(Graph, Theme).Draw(canvas))
    End Function

    ''' <summary>
    ''' 把 3D 结构图保存为 SVG 矢量图。
    ''' </summary>
    ''' <param name="path">输出文件路径。</param>
    ''' <param name="size">画布尺寸。</param>
    ''' <param name="mode">渲染模式。</param>
    ''' <param name="camera">相机；为空时自动取景。</param>
    Public Function SaveSvg3D(path As String, Optional size As Size = Nothing,
                              Optional mode As RenderMode = RenderMode.BallAndStick,
                              Optional camera As Camera = Nothing) As Boolean
        Return saveSvg(path, If(size = Nothing, New Size(720, 560), size),
            Sub(canvas) Call New Draw3D.MoleculeDraw3D(Graph, Theme).Draw(canvas, mode, camera))
    End Function

#End Region

    Private Function savePng(path As String, size As Size, render As Action(Of IGraphics)) As Boolean
        Call ensureDirectory(path)

        Using canvas As IGraphics = DriverLoad.CreateDefaultRasterGraphics(
            size, fill_color:=ElementColors.ParseHex(Theme.BackgroundColor, Color.White))

            Call render(canvas)

            Dim raster As GdiRasterGraphics = TryCast(canvas, GdiRasterGraphics)

            If raster Is Nothing Then Return False

            Call raster.ImageResource.SaveAs(path)
        End Using

        Return True
    End Function

    Private Function saveSvg(path As String, size As Size, render As Action(Of IGraphics)) As Boolean
        Call ensureDirectory(path)

        Using canvas As New GraphicsSVG(size, 96, 96)
            Call canvas.Clear(ElementColors.ParseHex(Theme.BackgroundColor, Color.White))
            Call render(canvas)
            Call canvas.WriteSVG(path, size:=$"{size.Width},{size.Height}")
        End Using

        Return True
    End Function

    Private Shared Sub ensureDirectory(path As String)
        Dim folder As String = IO.Path.GetDirectoryName(IO.Path.GetFullPath(path))

        If Not String.IsNullOrEmpty(folder) AndAlso Not IO.Directory.Exists(folder) Then
            Call IO.Directory.CreateDirectory(folder)
        End If
    End Sub

End Class
