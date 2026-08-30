' ============================================================================
' Program.vb — CLI 入口程序
' ----------------------------------------------------------------------------
''' 命令行参数：
'''
''' 必需参数：
'''   --input <path>        输入 FASTA 文件路径
'''   --output <path>      输出 families TSV 文件路径
'''
''' 可选参数：
'''   --summary <path>      输出 family summary TSV 路径（默认: <output>.summary.tsv）
'''   --min-identity <f>    最小序列相似性百分比（默认: 90.0）
'''   --min-coverage <f>    最小覆盖度百分比（默认: 80.0）
'''   --chunk-size <n>      每个 chunk 的序列数（默认: 500000）
'''   --diamond-path <p>    diamond 可执行文件路径（默认: diamond）
'''   --workdir <path>      工作目录，存储中间文件（默认: ./protein_clustering_work）
'''   --threads <n>         DIAMOND 线程数（默认: 4）
'''   --block-size <f>      DIAMOND block-size 参数（默认: 0.5，≈2GB 内存）
'''   --no-cleanup          完成后保留中间文件
'''   --progress-interval <n>  进度报告间隔（序列数，默认: 1000000）
'''   --help                显示帮助
'''
''' 用法示例：
'''   dotnet ProteinClustering.dll \
'''     --input huge_proteins.fasta \
'''     --output families.tsv \
'''     --min-identity 90 \
'''     --min-coverage 80 \
'''     --chunk-size 500000 \
'''     --diamond-path /usr/bin/diamond \
'''     --workdir /tmp/protein_clustering \
'''     --threads 8 \
'''     --block-size 0.5
' ============================================================================

Imports ProteinClustering.Core

Public Module Program

    Public Function Main(args As String()) As Integer
        Console.WriteLine("="c, 72)
        Console.WriteLine("  ProteinClustering: 蛋白质序列无监督聚类构建蛋白质家族")
        Console.WriteLine("  分块流式处理 | DIAMOND BLASTP | 内存映射并查集")
        Console.WriteLine("="c, 72)
        Console.WriteLine()

        ' 解析命令行参数
        Dim config As PipelineConfig
        Try
            config = ParseArgs(args)
        Catch ex As ArgumentException
            Console.Error.WriteLine($"参数错误: {ex.Message}")
            Console.Error.WriteLine()
            PrintHelp()
            Return 1
        End Try

        ' 显示配置摘要
        PrintConfig(config)
        Console.WriteLine()

        ' 运行 pipeline
        Try
            Dim builder As New ProteinFamilyBuilder(config)
            builder.Run()
            Return 0
        Catch ex As Exception
            Console.Error.WriteLine()
            Console.Error.WriteLine($"[错误] {ex.Message}")
            Console.Error.WriteLine(ex.StackTrace)
            Return 2
        End Try
    End Function

    ''' <summary>解析命令行参数</summary>
    Private Function ParseArgs(args As String()) As PipelineConfig
        Dim config As New PipelineConfig()

        Dim i = 0
        Do While i < args.Length
            Dim arg = args(i)
            Select Case arg.ToLower()
                Case "--help", "-h"
                    PrintHelp()
                    Environment.Exit(0)

                Case "--input"
                    i += 1
                    config.InputFasta = RequireValue(args, i, "--input")

                Case "--output"
                    i += 1
                    config.OutputFamilies = RequireValue(args, i, "--output")

                Case "--summary"
                    i += 1
                    config.OutputSummary = RequireValue(args, i, "--summary")

                Case "--min-identity"
                    i += 1
                    config.MinIdentity = Double.Parse(RequireValue(args, i, "--min-identity"))

                Case "--min-coverage"
                    i += 1
                    config.MinCoverage = Double.Parse(RequireValue(args, i, "--min-coverage"))

                Case "--chunk-size"
                    i += 1
                    config.ChunkSize = Integer.Parse(RequireValue(args, i, "--chunk-size"))

                Case "--diamond-path"
                    i += 1
                    config.DiamondPath = RequireValue(args, i, "--diamond-path")

                Case "--workdir"
                    i += 1
                    config.WorkDir = RequireValue(args, i, "--workdir")

                Case "--threads"
                    i += 1
                    config.Threads = Integer.Parse(RequireValue(args, i, "--threads"))

                Case "--block-size"
                    i += 1
                    config.BlockSize = Double.Parse(RequireValue(args, i, "--block-size"))

                Case "--no-cleanup"
                    config.Cleanup = False

                Case "--progress-interval"
                    i += 1
                    config.ProgressInterval = Long.Parse(RequireValue(args, i, "--progress-interval"))

                Case Else
                    Throw New ArgumentException($"未知参数: {arg}")
            End Select
            i += 1
        Loop

        ' 验证必需参数
        If String.IsNullOrEmpty(config.InputFasta) Then
            Throw New ArgumentException("缺少必需参数: --input")
        End If
        If String.IsNullOrEmpty(config.OutputFamilies) Then
            Throw New ArgumentException("缺少必需参数: --output")
        End If
        If Not IO.File.Exists(config.InputFasta) Then
            Throw New ArgumentException($"输入文件不存在: {config.InputFasta}")
        End If

        ' 默认值
        If String.IsNullOrEmpty(config.WorkDir) Then
            config.WorkDir = IO.Path.Combine(IO.Path.GetDirectoryName(config.InputFasta), "protein_clustering_work")
        End If
        If String.IsNullOrEmpty(config.OutputSummary) Then
            config.OutputSummary = config.OutputFamilies & ".summary.tsv"
        End If

        Return config
    End Function

    Private Function RequireValue(args As String(), index As Integer, paramName As String) As String
        If index >= args.Length Then
            Throw New ArgumentException($"参数 {paramName} 缺少值")
        End If
        Return args(index)
    End Function

    ''' <summary>打印配置摘要</summary>
    Private Sub PrintConfig(config As PipelineConfig)
        Console.WriteLine("配置参数:")
        Console.WriteLine($"  输入 FASTA:       {config.InputFasta}")
        Console.WriteLine($"  输出 families:    {config.OutputFamilies}")
        Console.WriteLine($"  输出 summary:     {config.OutputSummary}")
        Console.WriteLine($"  最小相似性:       {config.MinIdentity:F1}%")
        Console.WriteLine($"  最小覆盖度:       {config.MinCoverage:F1}%")
        Console.WriteLine($"  chunk 大小:       {config.ChunkSize:N0} 序列")
        Console.WriteLine($"  DIAMOND 路径:     {config.DiamondPath}")
        Console.WriteLine($"  工作目录:         {config.WorkDir}")
        Console.WriteLine($"  线程数:           {config.Threads}")
        Console.WriteLine($"  block-size:       {config.BlockSize:F1}")
        Console.WriteLine($"  完成后清理:       {If(config.Cleanup, "是", "否")}")
    End Sub

    ''' <summary>打印帮助信息</summary>
    Private Sub PrintHelp()
        Console.WriteLine("ProteinClustering — 蛋白质序列无监督聚类构建蛋白质家族")
        Console.WriteLine()
        Console.WriteLine("用法:")
        Console.WriteLine("  dotnet ProteinClustering.dll --input <fasta> --output <tsv> [options]")
        Console.WriteLine()
        Console.WriteLine("必需参数:")
        Console.WriteLine("  --input <path>        输入 FASTA 文件路径")
        Console.WriteLine("  --output <path>       输出 families TSV 文件路径")
        Console.WriteLine()
        Console.WriteLine("可选参数:")
        Console.WriteLine("  --summary <path>      输出 family summary TSV 路径（默认: <output>.summary.tsv）")
        Console.WriteLine("  --min-identity <f>    最小序列相似性百分比（默认: 90.0）")
        Console.WriteLine("  --min-coverage <f>    最小覆盖度百分比（默认: 80.0）")
        Console.WriteLine("  --chunk-size <n>      每个 chunk 的序列数（默认: 500000）")
        Console.WriteLine("  --diamond-path <p>    diamond 可执行文件路径（默认: diamond）")
        Console.WriteLine("  --workdir <path>      工作目录（默认: <input_dir>/protein_clustering_work）")
        Console.WriteLine("  --threads <n>         DIAMOND 线程数（默认: 4）")
        Console.WriteLine("  --block-size <f>      DIAMOND block-size（默认: 0.5, ≈2GB 内存）")
        Console.WriteLine("  --no-cleanup          完成后保留中间文件")
        Console.WriteLine("  --progress-interval <n>  进度报告间隔（默认: 1000000）")
        Console.WriteLine("  --help                显示此帮助")
        Console.WriteLine()
        Console.WriteLine("示例:")
        Console.WriteLine("  dotnet ProteinClustering.dll \")
        Console.WriteLine("    --input huge_proteins.fasta \")
        Console.WriteLine("    --output families.tsv \")
        Console.WriteLine("    --min-identity 90 --min-coverage 80 \")
        Console.WriteLine("    --chunk-size 500000 \")
        Console.WriteLine("    --diamond-path /usr/bin/diamond \")
        Console.WriteLine("    --threads 8 --block-size 0.5")
    End Sub

End Module


