Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.Drawing

''' <summary>
''' 进化树绘制门面：创建 GDI+ 画布 → 计算布局（<see cref="TreeLayoutEngine"/>）→
''' 渲染（<see cref="TreeRenderer"/>）→ 返回图像。
''' </summary>
''' <remarks>
''' <para>
''' 本模块仅依赖 <c>Microsoft.VisualBasic.Core</c> 中已有的 GDI+ 抽象
''' （<see cref="IGraphics"/> / <see cref="DriverLoad"/> / <see cref="GdiRasterGraphics"/>），
''' 不引入额外的成像程序集依赖。
''' </para>
''' <para>
''' **注意**：调用前宿主必须注册光栅图形驱动，否则无法创建画布。请在程序启动时调用：
''' <c>Microsoft.VisualBasic.Imaging.Driver.ImageDriver.Register()</c>
''' （该方法位于 <c>Microsoft.VisualBasic.Imaging</c> 程序集）。
''' </para>
''' <para>
''' 支持的布局样式见 <see cref="TreePlotMode"/>（共 8 种），完整配置项见 <see cref="TreeDrawingOptions"/>。
''' </para>
''' </remarks>
Public Module TreeDrawing

    ''' <summary>
    ''' 使用默认样式（矩形扇形图 / phylogram）绘制进化树。
    ''' </summary>
    ''' <param name="Tree">待绘制的进化树</param>
    ''' <returns>GDI+ 位图；调用方负责释放</returns>
    Public Function InvokeDrawing(Tree As Evolview.PhyloTree) As Image
        Return InvokeDrawing(Tree, TreePlotMode.RECT_PHYLOGRAM)
    End Function

    ''' <summary>
    ''' 使用指定布局样式与默认画布尺寸绘制进化树。
    ''' </summary>
    Public Function InvokeDrawing(Tree As Evolview.PhyloTree, Mode As TreePlotMode) As Image
        Return InvokeDrawing(Tree, TreeDrawingOptions.Defaults(Mode))
    End Function

    ''' <summary>
    ''' 使用完整配置绘制进化树。
    ''' </summary>
    Public Function InvokeDrawing(Tree As Evolview.PhyloTree, options As TreeDrawingOptions) As Image
        Return GetImage(Tree, options)
    End Function

    ''' <summary>
    ''' 按给定配置把进化树绘制为一张新的位图。
    ''' </summary>
    ''' <param name="Tree">待绘制的进化树</param>
    ''' <param name="options">绘制配置；为空时使用默认配置</param>
    Public Function GetImage(Tree As Evolview.PhyloTree, Optional options As TreeDrawingOptions = Nothing) As Image
        If Tree Is Nothing Then
            Throw New ArgumentNullException(NameOf(Tree))
        End If

        If options Is Nothing Then
            options = TreeDrawingOptions.Defaults()
        End If

        Call EnsureDriver()

        Dim canvas As IGraphics = DriverLoad.CreateDefaultRasterGraphics(options.CanvasSize, options.Background, options.Dpi)

        ' 注意：这里刻意不释放 canvas —— 返回的 Image 是对底层位图的包装，
        ' 释放画布会使返回的图像失效（与既有调用约定保持一致）。
        Call RenderTo(canvas, Tree, options)

        Return DirectCast(canvas, GdiRasterGraphics).ImageResource
    End Function

    ''' <summary>
    ''' 把进化树绘制到已有的（光栅或矢量）画布上。
    ''' </summary>
    ''' <param name="g">目标画布，由调用方负责创建与释放</param>
    ''' <param name="Tree">待绘制的进化树</param>
    ''' <param name="options">绘制配置；为空时使用默认配置</param>
    Public Sub RenderTo(g As IGraphics, Tree As Evolview.PhyloTree, Optional options As TreeDrawingOptions = Nothing)
        If g Is Nothing Then
            Throw New ArgumentNullException(NameOf(g))
        End If
        If Tree Is Nothing Then
            Throw New ArgumentNullException(NameOf(Tree))
        End If

        If options Is Nothing Then
            options = TreeDrawingOptions.Defaults()
        End If

        Dim layout As TreeLayoutResult = TreeLayoutEngine.Compute(Tree, options, g)

        Using renderer As New TreeRenderer(Tree, options)
            Call renderer.Render(g, layout)
        End Using
    End Sub

    ''' <summary>
    ''' 只计算布局（不绘制），便于内容度量、命中测试或单元测试。
    ''' </summary>
    Public Function Layout(Tree As Evolview.PhyloTree,
                           g As IGraphics,
                           Optional options As TreeDrawingOptions = Nothing) As TreeLayoutResult

        If options Is Nothing Then
            options = TreeDrawingOptions.Defaults()
        End If

        Return TreeLayoutEngine.Compute(Tree, options, g)
    End Function

    Private Sub EnsureDriver()
        If Not DriverLoad.CheckRasterImageLoader Then
            Throw New InvalidOperationException(
                "尚未注册光栅图形驱动，无法绘制进化树。请在程序启动时调用：" & vbCrLf &
                "    Microsoft.VisualBasic.Imaging.Driver.ImageDriver.Register()" & vbCrLf &
                "该方法位于 Microsoft.VisualBasic.Imaging 程序集。")
        End If
    End Sub

End Module
