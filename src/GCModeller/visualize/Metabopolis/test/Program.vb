#Region "Metabopolis end to end demo"

' ============================================================================
' Metabopolis 端到端测试
' ----------------------------------------------------------------------------
' 分别跑通两条数据链路：
'     1) BioCyc PGDB（F:\ecoli\29.0\data）  —— 类别 = 通路 (IN-PATHWAY)
'     2) SBML 文件（metabolic-reactions.xml）—— 类别 = compartment
' 每条链路执行：加载 -> 转换器 -> 布局流水线 -> 输出 PNG 与 SVG，
' 并打印网络规模、各阶段耗时与输出文件路径。
'
' 命令行参数：
'     --biocyc <dir>     BioCyc PGDB 目录（默认 F:\ecoli\29.0\data）
'     --sbml   <file>    SBML 文件路径（默认 F:\ecoli\29.0\data\metabolic-reactions.xml）
'     --out    <dir>     输出目录（默认 ./metabopolis_out）
'     --limit  <n>       每个数据源最多转换的反应数（默认 0 = 全部）
'     --canvas <WxH>     画布尺寸（默认 2400x1800）
'     --iter   <n>       模拟退火迭代次数（默认 6000）
'     --only   <biocyc|sbml>   只跑指定链路
' ============================================================================

Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports Metabopolis
Imports Metabopolis.Floorplan
Imports Metabopolis.Model
Imports Metabopolis.Rendering
Imports MetabopolisAdapter

Module Program

    Private Const DefaultBioCyc As String = "F:\ecoli\29.0\data"
    Private Const DefaultSbml As String = "F:\ecoli\29.0\data\metabolic-reactions.xml"

    Sub Main(args As String())
        Console.OutputEncoding = Encoding.UTF8

        Dim biocycDir As String = ReadOption(args, "--biocyc", DefaultBioCyc)
        Dim sbmlFile As String = ReadOption(args, "--sbml", DefaultSbml)
        Dim outDir As String = ReadOption(args, "--out", Path.Combine(AppContext.BaseDirectory, "metabopolis_out"))
        Dim limit As Integer = CInt(ReadOption(args, "--limit", "0"))
        Dim canvas As String = ReadOption(args, "--canvas", "2400x1800")
        Dim iterations As Integer = CInt(ReadOption(args, "--iter", "6000"))
        Dim only As String = ReadOption(args, "--only", "")

        Dim parts As String() = canvas.ToLowerInvariant().Split("x"c)
        Dim width As Integer = 2400
        Dim height As Integer = 1800

        If parts.Length = 2 Then
            width = CInt(parts(0))
            height = CInt(parts(1))
        End If

        Directory.CreateDirectory(outDir)

        Console.WriteLine("================================================================")
        Console.WriteLine("Metabopolis end to end demo")
        Console.WriteLine("================================================================")
        Console.WriteLine($"biocyc : {biocycDir}")
        Console.WriteLine($"sbml   : {sbmlFile}")
        Console.WriteLine($"output : {outDir}")
        Console.WriteLine($"canvas : {width} x {height}")
        Console.WriteLine("")

        If String.IsNullOrEmpty(only) OrElse only.Equals("biocyc", StringComparison.OrdinalIgnoreCase) Then
            Call RunBioCyc(biocycDir, outDir, limit, width, height, iterations)
        End If

        If String.IsNullOrEmpty(only) OrElse only.Equals("sbml", StringComparison.OrdinalIgnoreCase) Then
            Call RunSBML(sbmlFile, outDir, limit, width, height, iterations)
        End If

        Console.WriteLine("")
        Console.WriteLine("done.")
    End Sub

    Private Sub RunBioCyc(directory As String, outDir As String, limit As Integer, width As Integer, height As Integer, iterations As Integer)
        Console.WriteLine("----------------------------------------------------------------")
        Console.WriteLine("[1/2] BioCyc -> Metabopolis")
        Console.WriteLine("----------------------------------------------------------------")

        If Not Directory.Exists(directory) Then
            Console.Error.WriteLine($"skip BioCyc: directory not found: {directory}")
            Return
        End If

        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            Dim network As MetabolicNetwork = BioCycDataAdapter.Load(directory, New BioCycAdapterOptions With {
                .MaxReactions = limit,
                .SkipUnassigned = True
            })

            watch.Stop()

            Console.WriteLine($"loaded in {watch.ElapsedMilliseconds} ms")
            Console.WriteLine(network.Statistics())
            ReportProblems(network)

            Dim layout As NetworkLayout = RunLayout(network, "BioCyc E. coli metabolic map", width, height, iterations,
                                                    Path.Combine(outDir, "biocyc"))

            Console.WriteLine(layout.Statistics())
        Catch ex As Exception
            Console.Error.WriteLine($"[biocyc] failed: {ex.Message}")
            Console.Error.WriteLine(ex.StackTrace)
        End Try
    End Sub

    Private Sub RunSBML(sbmlFile As String, outDir As String, limit As Integer, width As Integer, height As Integer, iterations As Integer)
        Console.WriteLine("----------------------------------------------------------------")
        Console.WriteLine("[2/2] SBML -> Metabopolis")
        Console.WriteLine("----------------------------------------------------------------")

        If Not File.Exists(sbmlFile) Then
            Console.Error.WriteLine($"skip SBML: file not found: {sbmlFile}")
            Return
        End If

        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            Dim network As MetabolicNetwork = SBMLDataAdapter.Load(sbmlFile, New SBMLAdapterOptions With {
                .MaxReactions = limit
            })

            watch.Stop()

            Console.WriteLine($"loaded in {watch.ElapsedMilliseconds} ms")
            Console.WriteLine(network.Statistics())
            ReportProblems(network)

            Dim layout As NetworkLayout = RunLayout(network, "SBML E. coli metabolic map (by compartment)", width, height, iterations,
                                                    Path.Combine(outDir, "sbml"))

            Console.WriteLine(layout.Statistics())
        Catch ex As Exception
            Console.Error.WriteLine($"[sbml] failed: {ex.Message}")
            Console.Error.WriteLine(ex.StackTrace)
        End Try
    End Sub

    Private Function RunLayout(network As MetabolicNetwork, title As String, width As Integer, height As Integer, iterations As Integer, outputPrefix As String) As NetworkLayout
        Dim options As New MetabopolisOptions With {
            .CanvasWidth = width,
            .CanvasHeight = height,
            .SnapshotPath = outputPrefix & ".layout.json"
        }

        options.Floorplan = New FloorplanOptions With {
            .AnnealIterations = iterations
        }
        options.Skeleton.MaxDegree = 4

        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim layout As NetworkLayout = MetabopolisLayout.Run(network, options)
        watch.Stop()

        Console.WriteLine($"layout done in {watch.ElapsedMilliseconds} ms")

        Dim renderer As New MetabopolisRenderer(MapTheme.CreateDefault(), New RenderOptions With {
            .Title = title,
            .DrawJunctionLabels = False
        })

        watch.Restart()
        Dim png As String = outputPrefix & ".png"
        Dim svg As String = outputPrefix & ".svg"

        renderer.SavePng(layout, png)
        renderer.SaveSvg(layout, svg)
        watch.Stop()

        Console.WriteLine($"render done in {watch.ElapsedMilliseconds} ms")
        Console.WriteLine($"png : {png}")
        Console.WriteLine($"svg : {svg}")

        Return layout
    End Function

    Private Sub ReportProblems(network As MetabolicNetwork)
        Dim problems As String() = network.Verify()

        If problems Is Nothing OrElse problems.Length = 0 Then
            Return
        End If

        Console.WriteLine($"verify: {problems.Length} problem(s)")

        For i As Integer = 0 To Math.Min(problems.Length, 10) - 1
            Console.WriteLine($"  - {problems(i)}")
        Next

        If problems.Length > 10 Then
            Console.WriteLine($"  ... {problems.Length - 10} more")
        End If
    End Sub

    Private Function ReadOption(args As String(), name As String, [default] As String) As String
        For i As Integer = 0 To args.Length - 1
            If String.Equals(args(i), name, StringComparison.OrdinalIgnoreCase) AndAlso i + 1 < args.Length Then
                Return args(i + 1)
            End If
        Next

        Return [default]
    End Function

End Module

#End Region
