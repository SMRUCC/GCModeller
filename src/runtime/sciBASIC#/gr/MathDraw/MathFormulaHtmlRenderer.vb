Imports System.Collections.Concurrent
Imports System.Drawing
Imports Microsoft.VisualBasic.ApplicationServices.Report
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Microsoft.VisualBasic.Net.Http
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports IGraphics = Microsoft.VisualBasic.Imaging.IGraphics
Imports MathImage = Microsoft.VisualBasic.Imaging.Image
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush

Namespace Global.Microsoft.VisualBasic.Drawing.MathDraw

    ''' <summary>
    ''' An <see cref="IFormulaRenderer"/> implementation that converts LaTeX text into
    ''' a transparent PNG and embeds it in the generated html as a data URI.
    ''' </summary>
    ''' <remarks>
    ''' The renderer is self-contained: it owns the LaTeX parser, the drawing device and
    ''' a small cache so that interactive markdown preview does not rasterise the same
    ''' formula on every keystroke. Any parse/draw failure returns <c>Nothing</c> and the
    ''' markdown renderer falls back to the original LaTeX text.
    ''' </remarks>
    Public NotInheritable Class MathFormulaHtmlRenderer : Implements IFormulaRenderer

        Private Shared ReadOnly cache As New ConcurrentDictionary(Of String, String)(StringComparer.Ordinal)
        Private Shared ReadOnly initLock As New Object()
        Private Shared driverReady As Boolean = False

        Public ReadOnly Property Name As String Implements IFormulaRenderer.Name
            Get
                Return "MathDraw"
            End Get
        End Property

        ''' <summary>
        ''' Render one LaTeX formula as an <c>&lt;img&gt;</c> html fragment with an
        ''' embedded PNG data URI.
        ''' </summary>
        ''' <param name="latex">LaTeX source without the surrounding math delimiters.</param>
        ''' <param name="displayMode">block-level formula when <c>True</c>, inline otherwise.</param>
        ''' <returns>the html fragment, or <c>Nothing</c> when rendering is not possible.</returns>
        Public Function RenderFormula(latex As String, displayMode As Boolean) As String Implements IFormulaRenderer.RenderFormula
            If String.IsNullOrWhiteSpace(latex) Then
                Return Nothing
            End If

            Dim source As String = latex.Trim()
            Dim key As String = If(displayMode, "B|", "I|") & source

            Return cache.GetOrAdd(key, Function(ignored) RenderCore(source, displayMode))
        End Function

        Private Shared Function RenderCore(source As String, displayMode As Boolean) As String
            Try
                Call EnsureDriver()

                Dim lambda As LambdaExpression = LaTeXMath.ParseLatex(source)

                If lambda Is Nothing OrElse lambda.lambda Is Nothing Then
                    Return Nothing
                End If

                Dim canvasSize As Size = If(displayMode, New Size(2400, 800), New Size(1600, 600))
                Dim fontSize As Single = If(displayMode, 36F, 28F)

                Using device As IGraphics = DriverLoad.CreateGraphicsDevice(canvasSize,
                                                                            Color.Transparent,
                                                                            dpi:=120,
                                                                            driver:=Drivers.GDI)
                    Dim style As New FormulaRenderStyle(New Font("Cambria Math", fontSize),
                                                        New SolidBrush(Color.Black),
                                                        New Pen(Color.Black, 1.5F))
                    Dim canvas As New Canvas(device, style)
                    Dim used As SizeF = canvas.Draw(lambda, New PointF(16F, 16F))

                    Call canvas.Flush()

                    If used.Width <= 0F OrElse used.Height <= 0F Then
                        Return Nothing
                    End If

                    Dim raw As MathImage = DirectCast(device, GdiRasterGraphics).ImageResource
                    Dim cropped As MathImage = raw.CorpBlank(blankColor:=Color.Transparent, margin:=8)
                    Dim base64 As String = cropped.ToBase64String(ImageFormats.Png)

                    If String.IsNullOrEmpty(base64) Then
                        Return Nothing
                    End If

                    Dim cssClass As String = If(displayMode,
                                                "math-formula math-formula-block",
                                                "math-formula math-formula-inline")
                    Dim cssStyle As String = If(displayMode,
                                                "display:block;margin:1rem auto;max-width:100%;",
                                                "vertical-align:middle;max-width:100%;max-height:2.5em;")

                    Return $"<img class=""{cssClass}"" style=""{cssStyle}"" src=""data:image/png;base64,{base64}"" alt=""{HtmlEncode(source)}"" />"
                End Using
            Catch ex As Exception
                ' A broken formula must not break the document preview.
                Return Nothing
            End Try
        End Function

        Private Shared Sub EnsureDriver()
            If driverReady Then
                Return
            End If

            SyncLock initLock
                If Not driverReady Then
                    Call ImageDriver.Register()
                    driverReady = True
                End If
            End SyncLock
        End Sub

        Private Shared Function HtmlEncode(text As String) As String
            Return text _
                .Replace("&", "&amp;") _
                .Replace("<", "&lt;") _
                .Replace(">", "&gt;") _
                .Replace("""", "&quot;")
        End Function

    End Class
End Namespace
