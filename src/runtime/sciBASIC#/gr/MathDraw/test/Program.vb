Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Drawing.MathDraw
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports IGraphics = Microsoft.VisualBasic.Imaging.IGraphics
Imports MathImage = Microsoft.VisualBasic.Imaging.Image
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush

Module Program

    Sub New()
        Call ImageDriver.Register()
    End Sub

    Function Main(args As String()) As Integer
        Dim baseDir As String = AppContext.BaseDirectory
        Dim mathDir As String = System.IO.Path.Combine(baseDir, "math")
        Dim outputDir As String = System.IO.Path.Combine(baseDir, "output")
        Dim reportPath As String = System.IO.Path.Combine(outputDir, "debug-audit.json")
        Dim cases As String() = {"formula-quadratic.xml", "formula-fraction.xml"}

        Call DebugAudit.Reset()

        Try
            Call System.IO.Directory.CreateDirectory(outputDir)

            For Each file As String In cases
                Using scope = DebugAudit.Watch(file)
                    Call DebugAudit.Mark("case", file)

                    Dim mathFile As String = System.IO.Path.Combine(mathDir, file)
                    Dim lambda As LambdaExpression = LambdaExpression.FromMathML(System.IO.File.ReadAllText(mathFile))
                    Dim parsed As Boolean = lambda IsNot Nothing AndAlso lambda.lambda IsNot Nothing

                    Call DebugAudit.Test(parsed, "MathML parsed into expression tree")

                    If Not parsed Then
                        Continue For
                    End If

                    Call DebugAudit.Capture(CStr($"{file}.expression"), CObj(lambda.ToString()))

                    Using g As IGraphics = DriverLoad.CreateGraphicsDevice(New Size(1600, 900),
                                                                          Color.Transparent,
                                                                          dpi:=120,
                                                                          driver:=Drivers.GDI)
                        Dim style As New FormulaRenderStyle(New Font("Cambria Math", 56),
                                                            New SolidBrush(Color.Black),
                                                            New Pen(Color.Black, 2F))
                        Dim canvas As New Canvas(g, style)
                        Dim used As SizeF = canvas.Draw(lambda, New PointF(40F, 40F))

                        Call canvas.Flush()
                        Call DebugAudit.Test(used.Width > 0F AndAlso used.Height > 0F, "formula layout has non-empty bounds")
                        Call DebugAudit.Capture(CStr($"{file}.layout"), CObj(used.ToString()))

                        Dim image As MathImage = DirectCast(g, GdiRasterGraphics).ImageResource

                        Using buffer = image.GetMemoryBitmap()
                            Dim alpha As Integer = If(buffer Is Nothing, -1, CInt(buffer.GetPixel(0, 0).A))
                            Call DebugAudit.Test(alpha = 0, "canvas background is transparent")
                            Call DebugAudit.Capture($"{file}.canvasAlpha", alpha)
                        End Using

                        Dim cropped As MathImage = image.CorpBlank(blankColor:=Color.Transparent, margin:=20)

                        Using croppedBuffer = cropped.GetMemoryBitmap()
                            Dim croppedAlpha As Integer = If(croppedBuffer Is Nothing, -1, CInt(croppedBuffer.GetPixel(0, 0).A))
                            Call DebugAudit.Test(croppedAlpha = 0, "cropped PNG margin is transparent")
                        End Using

                        Dim png As String = System.IO.Path.Combine(outputDir, System.IO.Path.GetFileNameWithoutExtension(file) & ".png")

                        If Not cropped.SaveAs(png, ImageFormats.Png) Then
                            Call DebugAudit.Test(False, $"save PNG: {png}")
                            Throw New IOException($"Failed to save PNG: {png}")
                        End If

                        Dim bytes As Byte() = System.IO.File.ReadAllBytes(png)
                        Dim signatureOk As Boolean = bytes.Length >= 26 AndAlso
                                                     bytes(0) = &H89 AndAlso bytes(1) = &H50 AndAlso
                                                     bytes(2) = &H4E AndAlso bytes(3) = &H47 AndAlso
                                                     bytes(4) = &HD AndAlso bytes(5) = &HA AndAlso
                                                     bytes(6) = &H1A AndAlso bytes(7) = &HA AndAlso
                                                     bytes(12) = &H49 AndAlso bytes(13) = &H48 AndAlso
                                                     bytes(14) = &H44 AndAlso bytes(15) = &H52

                        Call DebugAudit.Test(signatureOk, "PNG signature and IHDR chunk are valid")
                        Call DebugAudit.Test(bytes(25) = 6, "PNG color type is RGBA")
                        Call DebugAudit.Test(System.IO.File.Exists(png), "PNG file exists on disk")
                        Call DebugAudit.Capture(CStr($"{file}.png"), CObj(png))
                        Call DebugAudit.Capture(CStr($"{file}.bytes"), CObj(bytes.Length))
                        Call DebugAudit.Mark("output", png)

                        Console.WriteLine($"{file} -> {png} ({used.Width.ToString("0.#")}x{used.Height.ToString("0.#")})")
                    End Using
                End Using
            Next
        Catch ex As Exception
            Call DebugAudit.Test(False, $"unhandled exception: {ex.Message}")
            Call DebugAudit.Mark("error", ex.ToString())
            Call Console.WriteLine(ex)
        Finally
            Call DebugAudit.SaveReport(reportPath)
            Call Console.WriteLine(DebugAudit.Dump(includeRecords:=False))
        End Try

        Return If(DebugAudit.FailureCount = 0, 0, 1)
    End Function
End Module
