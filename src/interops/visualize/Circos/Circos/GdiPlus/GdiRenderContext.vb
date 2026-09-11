Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports CircosDoc = SMRUCC.genomics.Visualize.Circos.Configurations.Circos

Namespace GdiPlus

    ''' <summary>
    ''' 内置 GDI+ 绘图引擎的渲染上下文。
    ''' 
    ''' 持有绘图设备、几何布局以及文档对象，并为各个渲染器提供统一的颜色/半径换算入口。
    ''' </summary>
    Public Class GdiRenderContext
        Implements IDisposable

        Public ReadOnly Property Circos As CircosDoc
        Public ReadOnly Property Options As GdiRenderOptions
        Public ReadOnly Property Graphics As IGraphics
        Public ReadOnly Property Canvas As GdiCanvas
        Public ReadOnly Property Layout As CircosLayout

        Public Sub New(circos As CircosDoc, options As GdiRenderOptions)
            Me.Circos = circos
            Me.Options = options
            Me.Graphics = DriverLoad.CreateGraphicsDevice(
                New Size(options.Width, options.Height),
                options.Background,
                options.Dpi)
            Me.Canvas = New GdiCanvas(Graphics, options.Padding)
            Me.Layout = New CircosLayout(circos, Canvas.ImageRadius)
            Me.PixelScale = Canvas.ImageRadius / 1000.0
        End Sub

        ''' <summary>
        ''' ideogram 的内圈半径（像素），由 <see cref="Tracks.IdeogramRenderer"/> 在绘制骨架圈的时候回填
        ''' </summary>
        Public Property IdeogramInnerRadius As Double = 0
        ''' <summary>
        ''' ideogram 的外圈半径（像素），由 <see cref="Tracks.IdeogramRenderer"/> 在绘制骨架圈的时候回填
        ''' </summary>
        Public Property IdeogramOuterRadius As Double = 0
        ''' <summary>
        ''' ideogram 的中心半径（像素），由 <see cref="Tracks.IdeogramRenderer"/> 在绘制骨架圈的时候回填
        ''' </summary>
        Public Property IdeogramCenterRadius As Double = 0

        ''' <summary>
        ''' ``p`` 单位的缩放系数。
        ''' 
        ''' circos 之中的 ``p`` 是相对于参考图像半径（1000px）的像素值，
        ''' 因此实际像素值 = 数值 * (当前图像半径 / 1000)
        ''' </summary>
        Public ReadOnly Property PixelScale As Double

        ''' <summary>
        ''' 将 circos 的长度表达式（``0.85r`` / ``25p`` / ``dims(...)``）换算为像素
        ''' </summary>
        Public Function Radius(expr As String, Optional fallback As Double = 0) As Double
            Return CircosUnits.ParseRadius(expr, Canvas.ImageRadius, fallback, AddressOf resolveDims, PixelScale)
        End Function

        ''' <summary>
        ''' 解析 ``dims(image,radius)`` / ``dims(ideogram,radius)`` / ``dims(ideogram,radius_outer)`` 之类的尺寸引用
        ''' </summary>
        Private Function resolveDims(token As String) As Double
            Dim s$ = If(token, "").ToLowerInvariant()

            If s.Contains("radius_outer") OrElse s.Contains("radius_out") Then
                If IdeogramOuterRadius > 0 Then
                    Return IdeogramOuterRadius
                End If
            ElseIf s.Contains("radius_inner") OrElse s.Contains("radius_in") Then
                If IdeogramInnerRadius > 0 Then
                    Return IdeogramInnerRadius
                End If
            ElseIf s.Contains("ideogram") Then
                If IdeogramCenterRadius > 0 Then
                    Return IdeogramCenterRadius
                End If
            End If

            Return Canvas.ImageRadius
        End Function

        ''' <summary>
        ''' 解析 circos 颜色表达式
        ''' </summary>
        Public Function ColorOf(expr As String, Optional fallback As Color? = Nothing) As Color
            Return CircosColorResolver.Resolve(expr, fallback)
        End Function

        ''' <summary>
        ''' 依据给定的尺寸（像素）创建一个字体
        ''' </summary>
        Public Function CreateFont(size As Double, Optional style As FontStyle = FontStyle.Regular) As Font
            If size < 4 Then size = 12

            Return New Font(FontFace.MicrosoftYaHei, CSng(size), style)
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Call Graphics.Dispose()
        End Sub
    End Class
End Namespace
