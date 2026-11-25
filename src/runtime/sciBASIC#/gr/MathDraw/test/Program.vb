Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ApplicationServices.Report
Imports Microsoft.VisualBasic.Drawing.MathDraw
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Microsoft.VisualBasic.MIME.text.markdown
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports MarkdownHtmlRender = Microsoft.VisualBasic.MIME.text.markdown.HtmlRender
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
        Dim cases As String() = {"formula-quadratic.xml", "formula-fraction.xml", "formula-complex.xml"}

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

                    Dim canvasSize As Size = If(file = "formula-complex.xml",
                                                New Size(5200, 1400),
                                                New Size(1600, 900))

                    Using g As IGraphics = DriverLoad.CreateGraphicsDevice(canvasSize,
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

            Using scope = DebugAudit.Watch("latex-roundtrip")
                Call DebugAudit.Mark("case", "latex-roundtrip")

                Dim latex As String = "\Phi=\lim_{n\to\infty}\sum_{i=1}^{n}\frac{i^2}{x^i}+\frac{d^2}{dx^2}F+\int_0^x e^{-t^2}dt+\frac{\partial u}{\partial y}+\begin{matrix}a^2&1/n\\b_1&\pi^2\end{matrix}"
                Dim lambda As LambdaExpression = LaTeXMath.ParseLatex(latex)
                Dim parsed As Boolean = lambda IsNot Nothing AndAlso lambda.lambda IsNot Nothing

                Call DebugAudit.Test(parsed, "LaTeX parsed into MathML expression object")
                Call DebugAudit.Capture(CStr("latex.source"), CObj(latex))

                If parsed Then
                    Dim canonical As String = LaTeXMath.ToLatex(lambda)
                    Call DebugAudit.Capture(CStr("latex.canonical"), CObj(canonical))
                    Dim reparsed As LambdaExpression = LaTeXMath.ParseLatex(canonical)
                    Dim canonicalAgain As String = LaTeXMath.ToLatex(reparsed)
                    Dim hasFeatures As Boolean = canonical.Contains("\lim") AndAlso
                                                 canonical.Contains("\sum") AndAlso
                                                 canonical.Contains("\int") AndAlso
                                                 canonical.Contains("\begin{matrix}")

                    Call DebugAudit.Test(hasFeatures, "canonical LaTeX retains complex operators")
                    Call DebugAudit.Test(canonical = canonicalAgain, "LaTeX round-trip is stable")
                    Call DebugAudit.Capture(CStr("latex.canonical"), CObj(canonical))
                    Call DebugAudit.Capture(CStr("latex.canonicalAgain"), CObj(canonicalAgain))

                    Using g As IGraphics = DriverLoad.CreateGraphicsDevice(New Size(5200, 1400),
                                                                          Color.Transparent,
                                                                          dpi:=120,
                                                                          driver:=Drivers.GDI)
                        Dim style As New FormulaRenderStyle(New Font("Cambria Math", 56),
                                                            New SolidBrush(Color.Black),
                                                            New Pen(Color.Black, 2F))
                        Dim canvas As New Canvas(g, style)
                        Dim used As SizeF = canvas.Draw(lambda, New PointF(40F, 40F))

                        Call canvas.Flush()
                        Call DebugAudit.Test(used.Width > 0F AndAlso used.Height > 0F, "LaTeX formula layout has non-empty bounds")
                        Call DebugAudit.Capture(CStr("latex.layout"), CObj(used.ToString()))

                        Dim image As MathImage = DirectCast(g, GdiRasterGraphics).ImageResource

                        Using buffer = image.GetMemoryBitmap()
                            Dim alpha As Integer = If(buffer Is Nothing, -1, CInt(buffer.GetPixel(0, 0).A))
                            Call DebugAudit.Test(alpha = 0, "LaTeX formula canvas is transparent")
                            Call DebugAudit.Capture(CStr("latex.canvasAlpha"), CObj(alpha))
                        End Using

                        Dim cropped As MathImage = image.CorpBlank(blankColor:=Color.Transparent, margin:=20)
                        Dim png As String = System.IO.Path.Combine(outputDir, "latex-roundtrip.png")

                        If Not cropped.SaveAs(png, ImageFormats.Png) Then
                            Call DebugAudit.Test(False, $"save LaTeX PNG: {png}")
                            Throw New IOException($"Failed to save LaTeX PNG: {png}")
                        End If

                        Dim bytes As Byte() = System.IO.File.ReadAllBytes(png)
                        Dim signatureOk As Boolean = bytes.Length >= 26 AndAlso
                                                     bytes(0) = &H89 AndAlso bytes(1) = &H50 AndAlso
                                                     bytes(2) = &H4E AndAlso bytes(3) = &H47 AndAlso
                                                     bytes(4) = &HD AndAlso bytes(5) = &HA AndAlso
                                                     bytes(6) = &H1A AndAlso bytes(7) = &HA AndAlso
                                                     bytes(12) = &H49 AndAlso bytes(13) = &H48 AndAlso
                                                     bytes(14) = &H44 AndAlso bytes(15) = &H52

                        Call DebugAudit.Test(signatureOk, "LaTeX PNG signature and IHDR chunk are valid")
                        Call DebugAudit.Test(bytes(25) = 6, "LaTeX PNG color type is RGBA")
                        Call DebugAudit.Capture(CStr("latex.png"), CObj(png))
                        Call DebugAudit.Capture(CStr("latex.bytes"), CObj(bytes.Length))
                        Call DebugAudit.Mark("output", png)

                        Console.WriteLine($"latex-roundtrip -> {png} ({used.Width.ToString("0.#")}x{used.Height.ToString("0.#")})")
                    End Using
                End If
            End Using

            Using scope = DebugAudit.Watch("formula-html-plugin")
                Call DebugAudit.Mark("case", "formula-html-plugin")

                Dim plugin As IFormulaRenderer = New MathFormulaHtmlRenderer()
                Dim latex As String = "\frac{a}{b}=c"
                Dim fragment As String = plugin.RenderFormula(latex, True)
                Dim valid As Boolean = Not String.IsNullOrEmpty(fragment) AndAlso
                                       fragment.Contains("data:image/png;base64,") AndAlso
                                       fragment.Contains("math-formula-block")

                Call DebugAudit.Test(valid, "MathDraw plugin produces an embedded PNG formula fragment")
                Call DebugAudit.Capture("plugin.name", CObj(plugin.Name))
                Call DebugAudit.Capture("plugin.fragmentLength", CObj(If(fragment, "").Length))

                Dim source As String = "intro" & vbLf & vbLf &
                                       "$$" & vbLf & "\frac{a}{b}=c" & vbLf & "$$" & vbLf & vbLf &
                                       "```tex" & vbLf & "E=mc^2" & vbLf & "```" & vbLf & vbLf &
                                       "$$x^2+y^2=z^2$$" & vbLf & vbLf &
                                       "inline $x^2$ here"
                Dim withPlugin As New MarkdownRender(New MarkdownHtmlRender(MarkdownStyle.GitHub), plugin)
                Dim html As String = withPlugin.Transform(source)
                Dim formulaImages As Integer = CountOccurrences(html, "math-formula")

                Call DebugAudit.Test(formulaImages >= 8, "block, single-line, fenced and inline formulas are rendered as images")
                Call DebugAudit.Capture("markdown.formulaImages", CObj(formulaImages))

                Dim withoutPlugin As New MarkdownRender(New MarkdownHtmlRender(MarkdownStyle.GitHub))
                Dim plain As String = withoutPlugin.Transform(source)

                Call DebugAudit.Test(Not plain.Contains("math-formula"), "without a plugin formula syntax is untouched")
                Call DebugAudit.Test(plain.Contains("\frac{a}{b}=c") AndAlso plain.Contains("$x^2$"),
                                     "without a plugin raw LaTeX remains in the output")
                Call DebugAudit.Capture("markdown.plainLength", CObj(plain.Length))
            End Using

            Using scope = DebugAudit.Watch("readme-formula-regressions")
                Call DebugAudit.Mark("case", "readme-formula-regressions")

                Dim formulas As String() = {
                    "\text{Neuro-Fuzzy} = \text{Fuzzy Logic（知识表达 + 推理）} \oplus \text{Neural Network（学习 + 参数优化）}",
                    "w_r = \prod_{i=1}^{N} \mu_{A_{r,i}}(x_i)"
                }
                Dim readmePlugin As IFormulaRenderer = New MathFormulaHtmlRenderer()

                For idx As Integer = 0 To formulas.Length - 1
                    Dim formula As String = formulas(idx)
                    Dim parsed As LambdaExpression = Nothing
                    Dim failure As String = ""

                    Try
                        parsed = LaTeXMath.ParseLatex(formula)
                    Catch ex As Exception
                        failure = ex.GetType.Name & ": " & ex.Message
                    End Try

                    Dim rendered As String = readmePlugin.RenderFormula(formula, True)

                    Call DebugAudit.Capture($"formula.{idx}", CObj(formula))
                    Call DebugAudit.Capture($"failure.{idx}", CObj(failure))
                    Call DebugAudit.Capture($"canonical.{idx}", CObj(If(parsed Is Nothing, "", LaTeXMath.ToLatex(parsed))))
                    Call DebugAudit.Test(parsed IsNot Nothing AndAlso parsed.lambda IsNot Nothing,
                                         "readme formula parses: " & Left(formula, 24))
                    Call DebugAudit.Test(Not String.IsNullOrEmpty(rendered) AndAlso rendered.Contains("data:image/png;base64,"),
                                         "readme formula renders: " & Left(formula, 24))

                    If Not String.IsNullOrEmpty(rendered) Then
                        Dim marker As String = "base64,"
                        Dim pos As Integer = rendered.IndexOf(marker, StringComparison.Ordinal)

                        If pos >= 0 Then
                            Dim base64 As String = rendered.Substring(pos + marker.Length)
                            Dim quote As Integer = base64.IndexOf(""""c)

                            If quote > 0 Then
                                base64 = base64.Substring(0, quote)
                            End If

                            Dim pngPath As String = IO.Path.Combine(outputDir, $"readme-formula-{idx}.png")
                            Call IO.File.WriteAllBytes(pngPath, Convert.FromBase64String(base64))
                            Call DebugAudit.Capture($"png.{idx}", CObj(pngPath))
                        End If
                    End If
                Next
            End Using

            Using scope = DebugAudit.Watch("readme-markdown-preview")
                Call DebugAudit.Mark("case", "readme-markdown-preview")

                Dim readmePath As String = IO.Path.GetFullPath(IO.Path.Combine(
                    AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..",
                    "Data_science", "MachineLearning", "ANFIS", "readme.md"))

                If IO.File.Exists(readmePath) Then
                    Dim markdown As String = IO.File.ReadAllText(readmePath)
                    Dim reader As New MarkdownRender(New MarkdownHtmlRender(MarkdownStyle.GitHub),
                                                     New MathFormulaHtmlRenderer())
                    Dim html As String = reader.Transform(markdown)

                    Call IO.File.WriteAllText(IO.Path.Combine(outputDir, "readme-preview.html"), html)
                    Call DebugAudit.Test(html.Contains("math-formula"), "ANFIS readme preview contains formula images")
                    Call DebugAudit.Test(html.Contains("alt=""" & "\text{Neuro-Fuzzy}"),
                                         "Neuro-Fuzzy formula is rendered as an image")
                    Call DebugAudit.Test(Not html.Contains(">" & "\text{Neuro-Fuzzy}"),
                                         "Neuro-Fuzzy formula is no longer raw text in preview")
                    Call DebugAudit.Test(html.Contains("w_r = \prod_{i=1}^{N}"),
                                         "product formula is rendered as an image")
                    Call DebugAudit.Test(Not html.Contains(">" & "\prod_{i=1}^{N}"),
                                         "product formula is no longer raw text in preview")
                    Call DebugAudit.Capture("readme.formulaImages", CObj(CountOccurrences(html, "math-formula")))
                Else
                    Call DebugAudit.Test(False, "ANFIS readme not found: " & readmePath)
                End If
            End Using
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

    Private Function CountOccurrences(text As String, token As String) As Integer
        If String.IsNullOrEmpty(text) OrElse String.IsNullOrEmpty(token) Then
            Return 0
        End If

        Dim count As Integer = 0
        Dim pos As Integer = 0

        Do
            Dim found As Integer = text.IndexOf(token, pos, StringComparison.Ordinal)

            If found < 0 Then
                Exit Do
            End If

            count += 1
            pos = found + token.Length
        Loop

        Return count
    End Function
End Module
