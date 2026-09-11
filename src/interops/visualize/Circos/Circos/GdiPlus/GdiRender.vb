Imports System.IO
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.GdiPlus.Tracks
Imports CircosDoc = SMRUCC.genomics.Visualize.Circos.Configurations.Circos

Namespace GdiPlus

    ''' <summary>
    ''' 内置 GDI+ 绘图引擎的渲染入口。
    ''' 
    ''' 直接读取已经构建完成的 circos 文档对象（<see cref="Configurations.Circos"/>），在 .NET 环境
    ''' 之中通过 GDI+ 绘制出圆环图并输出 PNG 图像，整个过程不需要依赖外部安装的 circos 程序。
    ''' </summary>
    Public Module GdiRender

        ''' <summary>
        ''' 将 circos 文档对象渲染为一张 PNG 图像
        ''' </summary>
        ''' <param name="circos">circos 文档对象</param>
        ''' <param name="outputFile">输出图像的文件路径（``*.png``）</param>
        ''' <param name="options">画布参数，为空的时候使用文档配置或者默认值</param>
        ''' <returns></returns>
        Public Function Render(circos As CircosDoc,
                               outputFile As String,
                               Optional options As GdiRenderOptions = Nothing) As CircosRenderResult

            Dim result As New CircosRenderResult With {
                .ConfFile = Nothing,
                .PngPath = Nothing
            }

            If circos Is Nothing Then
                result.Message = "The input circos document object is null!"
                Return result
            End If
            If String.IsNullOrWhiteSpace(outputFile) Then
                result.Message = "The output image file path is not specified!"
                Return result
            End If

            Try
                Call ensureGraphicsDriver()

                Dim opts As GdiRenderOptions = GdiRenderOptions.FromConfig(circos)

                Call opts.ApplyOverrides(options)

                outputFile = Path.GetFullPath(outputFile)

                Dim parent$ = Path.GetDirectoryName(outputFile)

                If Not String.IsNullOrEmpty(parent) Then
                    Call Directory.CreateDirectory(parent)
                End If

                Using ctx As New GdiRenderContext(circos, opts)
                    Call ctx.Graphics.Clear(opts.Background)

                    Dim ideogram = IdeogramRenderer.Render(ctx)

                    Call IdeogramRenderer.RenderTicks(ctx, ideogram.Outer)
                    Call PlotsRenderer.RenderPlots(ctx)
                    Call LinkRenderer.Render(ctx)
                    Call PlotsRenderer.RenderHighlights(ctx)

                    Dim device As DeviceInterop = DriverLoad.UseGraphicsDevice(Drivers.GDI)
                    Dim data = device.GetData(ctx.Graphics, {0, 0, 0, 0})
                    Dim imageData = TryCast(data, ImageData)

                    If imageData Is Nothing Then
                        result.Message = "The raster graphics driver did not return a bitmap image data!"
                        Return result
                    End If

                    If imageData.Save(outputFile) Then
                        result.Success = True
                        result.PngPath = outputFile
                    Else
                        result.Message = $"Failed to save the rendered image to '{outputFile}'!"
                    End If
                End Using
            Catch ex As Exception
                result.Success = False
                result.Message = $"{ex.GetType.Name}: {ex.Message}" & vbCrLf & ex.StackTrace
            End Try

            Return result
        End Function

        ''' <summary>
        ''' 确保 GDI+ 的光栅绘图驱动已经被注册到框架之中
        ''' </summary>
        Private Sub ensureGraphicsDriver()
            Try
                Call DriverLoad.UseGraphicsDevice(Drivers.GDI)
                Return
            Catch ex As Exception
                ' 尚未注册，尝试注册一个默认的驱动
            End Try

#If WINDOWS Then
            Call ImageDriver.Register()
#End If
        End Sub

    End Module
End Namespace
