Imports System.Drawing

Namespace GdiPlus

    ''' <summary>
    ''' 内置 GDI+ 绘图引擎的画布参数。
    ''' 
    ''' 取值优先级为：渲染参数（<see cref="ApplyOverrides"/>） &gt; 
    ''' 文档配置（<see cref="Configurations.Circos.GdiPlusImageSize"/>） &gt; 默认值。
    ''' </summary>
    Public Class GdiRenderOptions

        Public Const DefaultWidth As Integer = 1200
        Public Const DefaultHeight As Integer = 1200
        Public Const DefaultDpi As Integer = 100

        ''' <summary>
        ''' 输出图像的宽度（像素）
        ''' </summary>
        Public Property Width As Integer = DefaultWidth
        ''' <summary>
        ''' 输出图像的高度（像素）
        ''' </summary>
        Public Property Height As Integer = DefaultHeight
        ''' <summary>
        ''' 输出图像的分辨率
        ''' </summary>
        Public Property Dpi As Integer = DefaultDpi
        ''' <summary>
        ''' 画布的背景颜色
        ''' </summary>
        Public Property Background As Color = Color.White
        ''' <summary>
        ''' 画布四边的留白像素（用于避免最外层的标签被画布边缘裁剪，默认不留白）
        ''' </summary>
        Public Property Padding As Integer = 0

        ''' <summary>
        ''' 从 circos 文档对象之中读取画布参数（文档没有配置的时候使用默认值）
        ''' </summary>
        Public Shared Function FromConfig(circos As Configurations.Circos) As GdiRenderOptions
            Dim opts As New GdiRenderOptions

            If circos IsNot Nothing Then
                If circos.GdiPlusImageSize.Width > 0 AndAlso circos.GdiPlusImageSize.Height > 0 Then
                    opts.Width = circos.GdiPlusImageSize.Width
                    opts.Height = circos.GdiPlusImageSize.Height
                End If
                If circos.GdiPlusDpi > 0 Then
                    opts.Dpi = circos.GdiPlusDpi
                End If
            End If

            Return opts
        End Function

        ''' <summary>
        ''' 应用显式指定的渲染参数（仅当参数为有效正数的时候才会覆盖当前值）
        ''' </summary>
        Public Sub ApplyOverrides(spec As GdiRenderOptions)
            If spec Is Nothing Then
                Return
            End If

            If spec.Width > 0 Then
                Width = spec.Width
            End If
            If spec.Height > 0 Then
                Height = spec.Height
            End If
            If spec.Dpi > 0 Then
                Dpi = spec.Dpi
            End If
            If spec.Padding >= 0 Then
                Padding = spec.Padding
            End If

            Background = spec.Background
        End Sub

        ''' <summary>
        ''' 创建一个副本
        ''' </summary>
        Public Function Clone() As GdiRenderOptions
            Return New GdiRenderOptions With {
                .Width = Width,
                .Height = Height,
                .Dpi = Dpi,
                .Background = Background,
                .Padding = Padding
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{Width}x{Height} @{Dpi}dpi"
        End Function
    End Class
End Namespace
