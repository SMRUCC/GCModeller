Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Xsl
Imports Microsoft.VisualBasic.ApplicationServices.Debugging

''' <summary>
''' 把 <see cref="VirtualCell"/> 模型 XML 通过 ``VirtualCell.xsl`` 离线转换为一个
''' 自包含的 HTML 可视化报告，避免浏览器直接加载数十 MB 的原始模型文件。
'''
''' 命令行：
'''   test.exe --report model.xml [--xsl VirtualCell.xsl] [--out model.report.html]
'''           [--limit 50] [--audit outputDirectory]
'''
''' 转换过程中的断言、耗时、输入输出路径与产物大小统一由 <see cref="DebugAudit"/>
''' 记录，并输出 ``xsl_report_audit.txt`` / ``xsl_report_audit.json`` 两份测试报告。
''' </summary>
''' <remarks>
''' 断言统一使用 ``DebugAudit.Test`` 而不是 ``DebugAudit.Check``：后者带
''' ``Conditional("DEBUG")`` 特性，在 Release 构建中会被编译器消除，导致测试报告为空。
''' </remarks>
Public Module XslReport

    Private Const DefaultLimit As Integer = 50
    Private Const StylesheetName As String = "VirtualCell.xsl"

    ''' 已执行的断言次数。DebugAudit 只把失败的断言写入事件缓冲区，
    ''' 因此单独计数才能在测试报告中给出完整的检查项数量。
    Private checkCount As Integer = 0

    ''' <summary>
    ''' 记录一条断言：成功时静默通过，失败时写入 DebugAudit 事件缓冲区。
    ''' </summary>
    Private Sub Assert(condition As Boolean, message As String)
        checkCount += 1
        Call DebugAudit.Test(condition, message)
    End Sub

    Private Class Options
        Public Property Model As String
        Public Property Xsl As String
        Public Property Out As String
        Public Property Audit As String
        Public Property Limit As Integer = DefaultLimit
        Public Property ShowHelp As Boolean = False
    End Class

    ''' <summary>
    ''' 由 ``Program.Main`` 的 ``--report`` 分支调用；返回进程退出码。
    ''' </summary>
    Public Function Run(args As String()) As Integer
        Call DebugAudit.Reset()
        checkCount = 0

        Dim opts As Options = Parse(args)

        If opts Is Nothing Then
            Call PrintUsage()
            Call Assert(False, "the xsl report command line is invalid")
            Call WriteAudit(Environment.CurrentDirectory, Nothing, Nothing, TimeSpan.Zero)
            Return 2
        End If

        If opts.ShowHelp Then
            Call PrintUsage()
            Call DebugAudit.Mark("help requested")
            Return 0
        End If

        Dim stopwatch As Stopwatch = Stopwatch.StartNew()
        Dim modelPath As String = Path.GetFullPath(opts.Model)
        Dim xslPath As String = ResolveStylesheet(opts.Xsl)
        Dim outPath As String = If(opts.Out, Path.ChangeExtension(modelPath, ".report.html"))
        Dim auditDir As String = If(opts.Audit, Path.GetDirectoryName(outPath))

        outPath = Path.GetFullPath(outPath)
        If String.IsNullOrEmpty(auditDir) Then
            auditDir = Path.GetDirectoryName(outPath)
        End If
        If String.IsNullOrEmpty(auditDir) Then
            auditDir = Environment.CurrentDirectory
        End If

        Call Console.WriteLine($"[xsl-report] model:     {modelPath}")
        Call Console.WriteLine($"[xsl-report] stylesheet:{If(xslPath, "<not found>")}")
        Call Console.WriteLine($"[xsl-report] output:    {outPath}")
        Call Console.WriteLine($"[xsl-report] limit:     {opts.Limit}")

        Call DebugAudit.Mark("model file", modelPath)
        Call DebugAudit.Mark("stylesheet", If(xslPath, "<not found>"))
        Call DebugAudit.Mark("output file", outPath)
        Call DebugAudit.Mark("detail limit", opts.Limit)

        Call Assert(File.Exists(modelPath), "the virtual cell model xml file exists")
        Call Assert(Not xslPath Is Nothing AndAlso File.Exists(xslPath), "the VirtualCell.xsl stylesheet exists")
        Call Assert(opts.Limit > 0 AndAlso opts.Limit <= 1000, "the detail limit is inside the supported range")

        If Not File.Exists(modelPath) OrElse xslPath Is Nothing OrElse Not File.Exists(xslPath) Then
            stopwatch.Stop()
            Call WriteAudit(auditDir, modelPath, Nothing, stopwatch.Elapsed)
            Return 1
        End If

        Dim outFolder As String = Path.GetDirectoryName(outPath)
        If Not String.IsNullOrEmpty(outFolder) Then
            Call Directory.CreateDirectory(outFolder)
        End If

        ' ------------------------------------------------------------------
        ' 1. compile stylesheet and transform in a single streaming pass
        ' ------------------------------------------------------------------
        Dim transformError As String = Nothing

        Try
            Dim xslt As New XslCompiledTransform()
            Call xslt.Load(xslPath)

            Call Assert(True, "the xsl stylesheet has been compiled by XslCompiledTransform")

            Dim readerSettings As New XmlReaderSettings With {
                .DtdProcessing = DtdProcessing.Prohibit,
                .IgnoreComments = True,
                .IgnoreWhitespace = True,
                .XmlResolver = Nothing
            }
            Dim xslArgs As New XsltArgumentList()

            Call xslArgs.AddParam("limit", "", CDbl(opts.Limit))
            Call xslArgs.AddParam("reportTitle", "", "GCModeller Virtual Cell Report")
            Call xslArgs.AddParam("generatedAt", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))

            Using reader As XmlReader = XmlReader.Create(modelPath, readerSettings)
                Using writer As New StreamWriter(outPath, False, New UTF8Encoding(False))
                    Call xslt.Transform(reader, xslArgs, writer)
                    Call writer.Flush()
                End Using
            End Using
        Catch ex As Exception
            transformError = $"{ex.GetType().Name}: {ex.Message}"
            Call Console.Error.WriteLine($"[xsl-report] {transformError}")
            Call Assert(False, $"xsl transformation failed: {ex.Message}")
        End Try

        stopwatch.Stop()

        Call DebugAudit.Mark("elapsed", stopwatch.Elapsed.ToString())
        Call Assert(transformError Is Nothing, "the xsl transformation completed without exceptions")

        ' ------------------------------------------------------------------
        ' 2. structural validation of the generated single-file report
        ' ------------------------------------------------------------------
        If File.Exists(outPath) Then
            Dim size As Long = New FileInfo(outPath).Length
            Dim html As String = File.ReadAllText(outPath)

            Call DebugAudit.Mark("html bytes", size)
            Call Assert(size > 10000, "the generated html report is not empty")
            Call Assert(html.Contains("<!DOCTYPE html"), "the report declares an html5 doctype")
            Call Assert(html.Contains("<html"), "the report contains the html root element")
            Call Assert(html.Contains("id=""overview"""), "the overview dashboard section exists")
            Call Assert(html.Contains("id=""graphics"""), "the graphics section exists")
            Call Assert(html.Contains("id=""details"""), "the detail tables section exists")
            Call Assert(html.Contains("<svg"), "the inline svg graphics have been rendered")
            Call Assert(html.Contains("details class=""panel"""), "the collapsible detail panels exist")
            Call Assert(html.Contains("--bg:#0B1220"), "the inline css has been emitted without escaping")
            Call Assert(html.Contains("function filterRows"), "the inline filter script has been emitted")
            Call Assert(html.Contains("已显示"), "each detail panel reports the shown record count")
            Call Assert(html.Contains("共 "), "each detail panel reports the total record count")
        Else
            Call Assert(False, "the html report file has been created")
        End If

        ' ------------------------------------------------------------------
        ' 3. audit / test report
        ' ------------------------------------------------------------------
        Call WriteAudit(auditDir, modelPath, outPath, stopwatch.Elapsed)

        Call Console.WriteLine($"[xsl-report] elapsed:   {stopwatch.Elapsed}")
        Call Console.WriteLine($"[xsl-report] checks:    {checkCount}, failures: {DebugAudit.FailureCount}, audit events: {DebugAudit.Records.Length}")

        Return If(DebugAudit.FailureCount = 0, 0, 1)
    End Function

    ''' <summary>
    ''' 解析 ``--report`` 之后的参数；第一个非选项参数视为模型 XML 路径。
    ''' </summary>
    Private Function Parse(args As String()) As Options
        Dim o As New Options()

        If args Is Nothing Then
            Return Nothing
        End If

        Dim i As Integer = 1

        While i < args.Length
            Dim arg As String = args(i)
            Dim key As String = arg.ToLowerInvariant()

            Select Case key
                Case "--help", "-h", "/?"
                    o.ShowHelp = True
                    Return o

                Case "--model", "--xsl", "--out", "--limit", "--audit"
                    If i + 1 >= args.Length Then
                        Call Console.Error.WriteLine($"[xsl-report] missing value for option {arg}")
                        Return Nothing
                    End If

                    Dim value As String = args(i + 1)

                    Select Case key
                        Case "--model"
                            o.Model = value
                        Case "--xsl"
                            o.Xsl = value
                        Case "--out"
                            o.Out = value
                        Case "--audit"
                            o.Audit = value
                        Case "--limit"
                            Dim n As Integer = 0
                            If Not Integer.TryParse(value, n) Then
                                Call Console.Error.WriteLine($"[xsl-report] limit must be an integer: {value}")
                                Return Nothing
                            End If
                            o.Limit = n
                    End Select

                    i += 2

                Case Else
                    If arg.StartsWith("-") Then
                        Call Console.Error.WriteLine($"[xsl-report] unknown option: {arg}")
                        Return Nothing
                    End If
                    If String.IsNullOrEmpty(o.Model) Then
                        o.Model = arg
                        i += 1
                    Else
                        Call Console.Error.WriteLine($"[xsl-report] unexpected argument: {arg}")
                        Return Nothing
                    End If
            End Select
        End While

        If String.IsNullOrEmpty(o.Model) Then
            Call Console.Error.WriteLine("[xsl-report] the model xml file is required")
            Return Nothing
        End If

        Return o
    End Function

    ''' <summary>
    ''' 优先使用显式路径，其次使用随程序输出的 XSL，最后从可执行文件目录向上
    ''' 回溯到源码树中的 ``IO/GCMarkupLanguage/v2/Xml/VirtualCell.xsl``。
    ''' </summary>
    Private Function ResolveStylesheet(explicitPath As String) As String
        If Not String.IsNullOrEmpty(explicitPath) Then
            Return Path.GetFullPath(explicitPath)
        End If

        Dim direct As String = Path.Combine(AppContext.BaseDirectory, StylesheetName)
        If File.Exists(direct) Then
            Return direct
        End If

        Dim folder As DirectoryInfo = New DirectoryInfo(AppContext.BaseDirectory)

        For level As Integer = 0 To 8
            If folder Is Nothing Then
                Exit For
            End If

            Dim candidate As String = Path.Combine(folder.FullName, "IO", "GCMarkupLanguage", "v2", "Xml", StylesheetName)
            If File.Exists(candidate) Then
                Return candidate
            End If

            folder = folder.Parent
        Next

        Dim cwd As String = Path.Combine(Environment.CurrentDirectory, StylesheetName)
        If File.Exists(cwd) Then
            Return cwd
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' 输出 DebugAudit 文本表格与 JSON 两份测试报告，不覆盖编译演示的既有报告文件。
    ''' </summary>
    Private Sub WriteAudit(auditDir As String, modelPath As String, outPath As String, elapsed As TimeSpan)
        Try
            If String.IsNullOrEmpty(auditDir) Then
                auditDir = Environment.CurrentDirectory
            End If
            Call Directory.CreateDirectory(auditDir)

            Dim sb As New StringBuilder()

            Call sb.AppendLine("GCModeller :: VirtualCell XSL visual report converter")
            Call sb.AppendLine($"generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
            Call sb.AppendLine($"model:     {If(modelPath, "<none>")}")
            Call sb.AppendLine($"output:    {If(outPath, "<none>")}")
            Call sb.AppendLine($"elapsed:   {elapsed}")
            Call sb.AppendLine($"checks executed: {checkCount}")
            Call sb.AppendLine($"failures: {DebugAudit.FailureCount}")
            Call sb.AppendLine($"audit events: {DebugAudit.Records.Length} (DebugAudit 仅记录失败断言与标记点)")
            Call sb.AppendLine()
            Call sb.AppendLine(DebugAudit.Dump(includeRecords:=True))

            Call File.WriteAllText(Path.Combine(auditDir, "xsl_report_audit.txt"), sb.ToString(), Encoding.UTF8)
            Call File.WriteAllText(Path.Combine(auditDir, "xsl_report_audit.json"), DebugAudit.ToJson(indent:=True), Encoding.UTF8)

            Call Console.WriteLine($"[xsl-report] audit:     {Path.Combine(auditDir, "xsl_report_audit.txt")}")
        Catch ex As Exception
            Call Console.Error.WriteLine($"[xsl-report] unable to write the audit report: {ex.Message}")
        End Try
    End Sub

    Private Sub PrintUsage()
        Call Console.WriteLine("GCModeller VirtualCell XSL report converter")
        Call Console.WriteLine()
        Call Console.WriteLine("  test.exe --report <model.xml> [--xsl <VirtualCell.xsl>] [--out <report.html>]")
        Call Console.WriteLine("           [--limit 50] [--audit <directory>]")
        Call Console.WriteLine()
        Call Console.WriteLine("  model   the GCMarkup 3.0 VirtualCell model xml file")
        Call Console.WriteLine("  xsl     stylesheet path; defaults to VirtualCell.xsl next to the executable")
        Call Console.WriteLine("  out     output html file; defaults to <model>.report.html")
        Call Console.WriteLine("  limit   maximum rows rendered per detail panel (1..1000, default 50)")
        Call Console.WriteLine("  audit   directory for xsl_report_audit.txt / xsl_report_audit.json")
    End Sub

End Module
