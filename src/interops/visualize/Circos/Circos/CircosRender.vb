Imports System.IO
Imports System.Text

Namespace Configurations

    ''' <summary>
    ''' The result package of a circos rendering invocation.
    ''' </summary>
    Public Class CircosRenderResult

        ''' <summary>
        ''' 是否成功生成了图形？
        ''' </summary>
        ''' <returns></returns>
        Public Property Success As Boolean
        ''' <summary>
        ''' circos 程序的命令行参数是否被正确执行（不为0的时候表示执行出错）
        ''' </summary>
        ''' <returns></returns>
        Public Property ExitCode As Integer
        ''' <summary>
        ''' circos 程序执行失败时的错误摘要，成功的时候为空字符串
        ''' </summary>
        ''' <returns></returns>
        Public Property Message As String
        ''' <summary>
        ''' 被渲染的配置文件<see cref="ConfFile"/>的文件路径
        ''' </summary>
        Public Property ConfFile As String
        ''' <summary>
        ''' 渲染结果 png 图片的文件路径，渲染失败的时候为空
        ''' </summary>
        ''' <returns></returns>
        Public Property PngPath As String
        ''' <summary>
        ''' 渲染结果 svg 图片的文件路径，渲染失败的时候为空
        ''' </summary>
        ''' <returns></returns>
        Public Property SvgPath As String
        ''' <summary>
        ''' circos 程序的标准输出流
        ''' </summary>
        ''' <returns></returns>
        Public Property StdOut As String
        ''' <summary>
        ''' circos 程序的错误输出流
        ''' </summary>
        ''' <returns></returns>
        Public Property StdErr As String

        Public Overrides Function ToString() As String
            If Success Then
                Return $"[{ExitCode}] OK  --> {PngPath}"
            Else
                Return $"[{ExitCode}] ERROR  {Message}"
            End If
        End Function
    End Class

    ''' <summary>
    ''' Invoke the circos command line tools for drawing the circos plot.
    ''' </summary>
    ''' <remarks>
    ''' ### 为什么直接调用 circos.exe 而不是 perl
    ''' 
    ''' circos 是一个 perl 程序，但是由于 windows 环境之中通常并没有安装 perl 解释器，
    ''' 所以官方的 windows 发行版提供了一个通过 PAR 打包的 ``bin\circos.exe``，
    ''' 这个可执行文件内部已经自带了 perl 运行时（``v 0.69-10 | 25 August 2025 | Perl 5.014002``）。
    ''' 因此在这里直接执行这个可执行文件即可，不需要再额外寻找 perl 解释器。
    ''' </remarks>
    Public Module CircosRender

        ''' <summary>
        ''' The default location of the circos windows distributed executable file.
        ''' (如果你的发行版不在这个位置的话，则可以通过参数覆盖掉这个默认的文件路径)
        ''' </summary>
        Public Const DefaultCircosExe As String = "G:\circos-0.69-10\bin\circos.exe"

        ''' <summary>
        ''' 尝试自动化的查找 circos 命令行程序的安装位置
        ''' </summary>
        ''' <returns>找不到的时候会返回 Nothing</returns>
        Public Function SearchCircosExe() As String
            Dim candidates As String() = {
                DefaultCircosExe,
                Environment.GetEnvironmentVariable("CIRCOS_HOME"),
                "%CIRCOS_HOME%\bin\circos.exe"
            }

            For Each path As String In candidates
                If Not String.IsNullOrEmpty(path) Then
                    Dim exe As String = Environment.ExpandEnvironmentVariables(path)

                    If File.Exists(exe) Then
                        Return exe
                    ElseIf Directory.Exists(exe) Then
                        Dim bin As String = $"{exe}/bin/circos.exe"

                        If File.Exists(bin) Then
                            Return bin
                        End If
                    End If
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' Invoke the circos command line tools for drawing the circos plot from a
        ''' given ``circos.conf`` template file.
        ''' </summary>
        ''' <param name="confFile">The file path of the ``circos.conf`` file.</param>
        ''' <param name="circosExe">
        ''' The circos executable file location, by default is <see cref="DefaultCircosExe"/>,
        ''' if this location is invalid, then this function will try to search via
        ''' <see cref="SearchCircosExe"/> automatically.
        ''' </param>
        ''' <param name="outputFile">
        ''' The output image file name, by default is ``circos.png``.
        ''' </param>
        ''' <param name="outputDir">
        ''' 图片文件的输出文件夹，默认为配置文件所在的文件夹
        ''' </param>
        ''' <param name="timeoutMs"></param>
        ''' <returns></returns>
        Public Function Render(confFile As String,
                               Optional circosExe As String = Nothing,
                               Optional outputFile As String = "circos.png",
                               Optional outputDir As String = Nothing,
                               Optional timeoutMs As Integer = 300000) As CircosRenderResult

            Dim result As New CircosRenderResult With {
                .ConfFile = confFile
            }

            If String.IsNullOrEmpty(confFile) OrElse Not File.Exists(confFile) Then
                result.Message = $"The given circos configuration file '{confFile}' is not exists on your filesystem!"
                Return result
            End If

            Dim exe As String = circosExe

            If String.IsNullOrEmpty(exe) OrElse Not File.Exists(exe) Then
                exe = SearchCircosExe()
            End If

            If String.IsNullOrEmpty(exe) OrElse Not File.Exists(exe) Then
                result.Message = $"Could not find the circos executable program, please install the circos distribution first, or specific it via the function parameter. (default location is '{DefaultCircosExe}')"
                Return result
            End If

            confFile = Path.GetFullPath(confFile)

            If String.IsNullOrEmpty(outputDir) Then
                outputDir = Path.GetDirectoryName(confFile)
            Else
                outputDir = Path.GetFullPath(outputDir)

                If Not Directory.Exists(outputDir) Then
                    Call Directory.CreateDirectory(outputDir)
                End If
            End If

            ' 注意：必须使用 ArgumentList 而不是拼接好的 Arguments 字符串，
            ' 否则当路径以反斜杠结尾的时候会出现引号被转义的问题
            ' (例如 ``-outputdir "Z:\test\"`` 会被 windows 的命令行解析器误读)
            Dim argvs As New Specialized.StringCollection

            Call argvs.Add("-conf")
            Call argvs.Add(confFile)
            Call argvs.Add("-outputdir")
            Call argvs.Add(outputDir.TrimEnd("\"c, "/"c))
            Call argvs.Add("-outputfile")
            Call argvs.Add(outputFile)

            ' circos 使用相对路径来查找数据文件，所以必须将工作目录设定为配置文件所在的文件夹
            Dim psi As New ProcessStartInfo With {
                .FileName = exe,
                .WorkingDirectory = Path.GetDirectoryName(confFile),
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True,
                .StandardOutputEncoding = Encoding.UTF8,
                .StandardErrorEncoding = Encoding.UTF8
            }

            Using proc As New Process With {.StartInfo = psi}
                For Each arg As String In argvs
                    Call proc.StartInfo.ArgumentList.Add(arg)
                Next

                ' 必须同时异步读取 stdout 与 stderr，否则缓冲区写满之后子进程会被卡死
                Dim stdout As New StringBuilder
                Dim stderr As New StringBuilder

                AddHandler proc.OutputDataReceived, Sub(sender, e)
                                                        If Not e.Data Is Nothing Then
                                                            Call stdout.AppendLine(e.Data)
                                                        End If
                                                    End Sub
                AddHandler proc.ErrorDataReceived, Sub(sender, e)
                                                       If Not e.Data Is Nothing Then
                                                           Call stderr.AppendLine(e.Data)
                                                       End If
                                                   End Sub

                Call proc.Start()
                Call proc.BeginOutputReadLine()
                Call proc.BeginErrorReadLine()

                If Not proc.WaitForExit(timeoutMs) Then
                    Try
                        Call proc.Kill()
                    Catch ex As Exception
                    End Try

                    result.Message = $"Rendering timeout({timeoutMs}ms)! The circos job was killed."
                    result.StdOut = stdout.ToString
                    result.StdErr = stderr.ToString
                    Return result
                End If

                result.ExitCode = proc.ExitCode
                result.StdOut = stdout.ToString
                result.StdErr = stderr.ToString
            End Using

            Dim base As String = Path.GetFileNameWithoutExtension(outputFile)
            Dim png As String = $"{outputDir}/{base}.png"
            Dim svg As String = $"{outputDir}/{base}.svg"

            If File.Exists(png) Then
                result.PngPath = png
            End If
            If File.Exists(svg) Then
                result.SvgPath = svg
            End If

            result.Success = (result.ExitCode = 0) AndAlso (Not String.IsNullOrEmpty(result.PngPath))

            If Not result.Success AndAlso String.IsNullOrEmpty(result.Message) Then
                result.Message = If(
                    String.IsNullOrWhiteSpace(result.StdErr),
                    "Unknown error, please check the StdOut of this result for more details.",
                    tailLines(result.StdErr, 12))
            End If

            Return result
        End Function

        ''' <summary>
        ''' 取出很多行文本之中的末尾几行，用于渲染失败的时候的错误摘要显示
        ''' </summary>
        Private Function tailLines(text As String, n As Integer) As String
            Dim lines As String() = text.Replace(vbCrLf, vbLf).Split(CChar(vbLf))

            If lines.Length > n Then
                lines = lines.Skip(lines.Length - n).ToArray
            End If

            Return lines _
                .Select(Function(s) s.Trim) _
                .Where(Function(s) Not String.IsNullOrEmpty(s)) _
                .JoinBy(vbLf)
        End Function

        ''' <summary>
        ''' Save the circos document model into a specific directory and then drawing
        ''' the circos plot from the generated configuration file.
        ''' </summary>
        ''' <param name="circos"></param>
        ''' <param name="directory"></param>
        ''' <param name="circosExe"></param>
        ''' <param name="outputFile"></param>
        ''' <param name="timeoutMs"></param>
        ''' <returns></returns>
        Public Function Render(circos As Circos,
                               directory As String,
                               Optional circosExe As String = Nothing,
                               Optional outputFile As String = "circos.png",
                               Optional timeoutMs As Integer = 300000) As CircosRenderResult

            Dim base As String = Circos.NormalizeDirectory(directory)

            Call circos.Save(base)

            Return Render(
                confFile:=$"{base}/{Circos.FileName}",
                circosExe:=circosExe,
                outputFile:=outputFile,
                outputDir:=base,
                timeoutMs:=timeoutMs)
        End Function
    End Module
End Namespace
