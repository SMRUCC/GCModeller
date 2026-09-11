Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSAR.Models
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSAR.Statistics
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci

''' <summary>
''' *T*ranscription *S*tart *S*ites *A*nnotation *R*egime（TSSAR）：
''' 基于 dRNA-seq 数据的转录起始位点注释算法。
''' </summary>
''' <remarks>
''' 本实现为论文（Amman et al., BMC Bioinformatics 2014）与原始
''' ``TSSAR.pl``/R 工作流的纯 VB.NET 重写，核心步骤为：
''' 
''' 1. 从 [+] / [-] 两个 SAM 文库统计双链逐位置 read 起始覆盖度；
''' 2. 在滑动窗口（默认 1000 nt）内用零膨胀 Poisson 回归估计局部背景，
'''    分离"结构零（不转录）"与"采样零（转录但未采到）"；
''' 3. 对 [+] 与 [-] 的逐位置计数差使用 Skellam 分布做显著性检验；
''' 4. 多个窗口的 p 值取几何平均，并以双阈值（p 值 + 噪声阈值）判定 TSS；
''' 5. 可选多重检验校正、连续 TSS 聚类、基因上下文分类与 5'UTR / TEX 评估。
''' </remarks>
<Package("TSSAR",
         Publisher:="SMRUCC genomics institute",
         Description:="dRNA-seq transcription start site annotation based on a Skellam distribution with zero-inflated-Poisson regression.")>
Public Module TSSAR

    ''' <summary>
    ''' 执行一次完整的 TSS 注释分析。
    ''' </summary>
    ''' <param name="options">运行参数。</param>
    ''' <returns>分析结果。</returns>
    Public Function Annotate(options As TssarOptions) As TssarResult
        If options Is Nothing Then
            Throw New ArgumentNullException(NameOf(options))
        End If
        If String.IsNullOrEmpty(options.PlusSam) OrElse Not File.Exists(options.PlusSam) Then
            Throw New FileNotFoundException($"The [+] library SAM file could not be found: {options.PlusSam}")
        End If
        If String.IsNullOrEmpty(options.MinusSam) OrElse Not File.Exists(options.MinusSam) Then
            Throw New FileNotFoundException($"The [-] library SAM file could not be found: {options.MinusSam}")
        End If

        Dim chromosome As String = options.Chromosome
        Dim genomeSize As Integer = options.GenomeSize

        If Not String.IsNullOrEmpty(options.Fasta) AndAlso File.Exists(options.Fasta) Then
            genomeSize = ReadsCoverage.GetGenomeSize(options.Fasta, chromosome)
        ElseIf String.IsNullOrEmpty(chromosome) Then
            chromosome = "chr"
        End If

        If genomeSize <= 0 Then
            Throw New ArgumentException("Either a valid fasta file or a positive genome size (--g_size) must be specified.")
        End If

        If options.Verbose Then
            Console.WriteLine($"[TSSAR] genome size = {genomeSize}, chromosome = '{chromosome}'")
            Console.WriteLine($"[TSSAR] window = {options.WindowSize} nt, minPeak = {options.MinPeakSize}, p-value cutoff = {options.PValueCutoff:G3}")
        End If

        ' 1. 覆盖度统计
        Dim coverage As CoverageMap = ReadsCoverage.FromSam(options.PlusSam, options.MinusSam, genomeSize, options.Prorata)

        If coverage.MaxPosition > genomeSize Then
            Throw New InvalidOperationException(
                $"The used genome size ({genomeSize}) is smaller than the largest mapped read position ({coverage.MaxPosition}). " &
                "It seems that a different reference genome was used for mapping than for TSS annotation.")
        End If

        Dim normalizePlus As Double = coverage.NormalizePlus
        Dim normalizeMinus As Double = coverage.NormalizeMinus

        If options.Verbose Then
            Console.WriteLine($"[TSSAR] [+] library read starts = {coverage.SumPlus:G4}, [-] library read starts = {coverage.SumMinus:G4}")
            Console.WriteLine($"[TSSAR] normalization: [+] x {normalizePlus:G6}, [-] x {normalizeMinus:G6}")
        End If

        ' 2. 滑动窗口分析（链间彼此独立）
        Dim plusResult As StrandPValueResult = SlidingWindow.Run(
            coverage.PlusPlus, coverage.MinusPlus, normalizePlus, normalizeMinus,
            genomeSize, options.WindowSize, options.MinPeakSize, options.Seed)

        Dim minusResult As StrandPValueResult = SlidingWindow.Run(
            coverage.PlusMinus, coverage.MinusMinus, normalizePlus, normalizeMinus,
            genomeSize, options.WindowSize, options.MinPeakSize, options.Seed)

        ' 3. 可选多重检验校正
        Dim plusPValue As Double() = plusResult.PValue
        Dim minusPValue As Double() = minusResult.PValue

        If Not String.IsNullOrWhiteSpace(options.MultipleTesting) Then
            plusPValue = MultipleTesting.Adjust(plusPValue, options.MultipleTesting)
            minusPValue = MultipleTesting.Adjust(minusPValue, options.MultipleTesting)
        End If

        ' 4. 双阈值判定
        Dim significant As List(Of TssSite) = SelectSignificant(
            plusPValue, coverage.PlusPlus, coverage.MinusPlus, normalizePlus, normalizeMinus,
            Strands.Forward, chromosome, genomeSize, options)

        significant.AddRange(SelectSignificant(
            minusPValue, coverage.PlusMinus, coverage.MinusMinus, normalizePlus, normalizeMinus,
            Strands.Reverse, chromosome, genomeSize, options))

        ' 判定顺序与 TSSAR.pl 一致：按位置升序，同一位置先正链后负链
        Dim ordered As TssSite() = significant _
            .OrderBy(Function(site) site.Position) _
            .ThenBy(Function(site) If(site.Strand = Strands.Forward, 0, 1)) _
            .ToArray()

        Dim individualCount As Integer = ordered.Length

        ' 5. 聚类
        Dim output As TssSite()

        If options.Clustering Then
            output = TssClustering.Cluster(ordered, options.ClusterRange, options.ScoreMode)
        Else
            output = ordered
        End If

        ' 6. TEX 效率估计
        For Each site As TssSite In output
            Dim total As Double = site.PlusCoverage + site.MinusCoverage

            site.TexEfficiency = If(total > 0, site.PlusCoverage / total, 0.0)
        Next

        ' 7. 基因上下文分类
        Dim utrLengths As Integer() = New Integer() {}

        If Not String.IsNullOrEmpty(options.Ptt) AndAlso File.Exists(options.Ptt) Then
            Dim ptt As PTT = PTT.Load(options.Ptt, fillBlank:=True)

            TssClassification.Classify(output, ptt, options.PrimaryUpstream, options.AntisenseDownstream)

            utrLengths = output _
                .Where(Function(site) site.Type = TssTypes.Primary) _
                .Select(Function(site) site.UtrLength) _
                .ToArray()
        End If

        ' 8. 未建模区域
        Dim dumpRegions As DumpRegion() = BuildDumpRegions(plusResult.SeenRegion, minusResult.SeenRegion, chromosome, genomeSize)
        Dim dumpLength As Long = 0

        For Each region As DumpRegion In dumpRegions
            dumpLength += region.Length
        Next

        Dim meanTex As Double = If(output.Length > 0, output.Average(Function(site) site.TexEfficiency), 0.0)

        If options.Verbose Then
            Console.WriteLine($"[TSSAR] {individualCount} individual TSS were annotated; {output.Length} remained after clustering.")
            Console.WriteLine($"[TSSAR] {dumpRegions.Length} region(s) with a total length of {dumpLength} nt could not be modeled.")
        End If

        Return New TssarResult With {
            .Chromosome = chromosome,
            .GenomeSize = genomeSize,
            .Tss = output,
            .UnmodeledRegions = dumpRegions,
            .TotalIndividualTss = individualCount,
            .TotalClusteredTss = output.Length,
            .UnmodeledLength = dumpLength,
            .PrimaryUtrLengths = utrLengths,
            .MeanTexEfficiency = meanTex
        }
    End Function

    ''' <summary>
    ''' 执行一次完整的 TSS 注释分析，并把 TSS 与未建模区域写出为 BED 文件。
    ''' </summary>
    ''' <param name="plusSam">[+] 文库的 SAM 文件路径。</param>
    ''' <param name="minusSam">[-] 文库的 SAM 文件路径。</param>
    ''' <param name="genomeSize">基因组长度（当提供 <paramref name="fasta"/> 时可省略）。</param>
    ''' <param name="fasta">参考基因组 fasta 文件路径。</param>
    ''' <param name="out">输出的 TSS BED 文件路径。</param>
    ''' <param name="dump">输出的未建模区域 BED 文件路径。</param>
    ''' <param name="ptt">基因注释（PTT）文件路径。</param>
    ''' <param name="windowSize">滑动窗口大小。</param>
    ''' <param name="minPeak">噪声阈值。</param>
    ''' <param name="pvalue">p 值阈值。</param>
    ''' <param name="score">评分模式（``p`` 或 ``d``）。</param>
    ''' <param name="range">聚类合并的最大间距。</param>
    ''' <param name="nocluster">是否禁止聚类（``True`` 时输出全部显著位点）。</param>
    ''' <param name="mtc">多重检验校正方法；为空表示不校正。</param>
    ''' <param name="prorata">是否按 ``NH`` 标签按比例计数。</param>
    ''' <param name="chrName">输出 BED 中使用的参考序列名称。</param>
    ''' <returns>被注释的 TSS 数量。</returns>
    ''' <remarks>
    ''' 用法示例：
    ''' 
    ''' ```
    ''' TSSAR.Annotate --libP plus.sam --libM minus.sam --g_size 5000000 --minPeak 3 --pval 1e-4 --winSize 1000 --score d --mtc fdr --ptt genome.ptt --out tss.bed --dump Dump.bed
    ''' ```
    ''' </remarks>
    <ExportAPI("TSSAR.Annotate")>
    Public Function AnnotateCommand(plusSam As String,
                                    minusSam As String,
                                    Optional genomeSize As Integer = 0,
                                    Optional fasta As String = "",
                                    Optional out As String = "",
                                    Optional dump As String = "",
                                    Optional ptt As String = "",
                                    Optional windowSize As Integer = 1000,
                                    Optional minPeak As Integer = 3,
                                    Optional pvalue As Double = 0.0001,
                                    Optional score As String = "d",
                                    Optional range As Integer = 3,
                                    Optional nocluster As Boolean = False,
                                    Optional mtc As String = "",
                                    Optional prorata As Boolean = False,
                                    Optional chrName As String = "chr") As Integer

        Dim options As New TssarOptions With {
            .PlusSam = plusSam,
            .MinusSam = minusSam,
            .GenomeSize = genomeSize,
            .Fasta = fasta,
            .Chromosome = chrName,
            .WindowSize = windowSize,
            .MinPeakSize = minPeak,
            .PValueCutoff = pvalue,
            .ScoreMode = If(String.Equals(score, "p", StringComparison.OrdinalIgnoreCase), ScoreModes.PValue, ScoreModes.PeakDifference),
            .ClusterRange = range,
            .Clustering = Not nocluster,
            .MultipleTesting = mtc,
            .Prorata = prorata,
            .Ptt = ptt
        }

        Dim result As TssarResult = Annotate(options)

        If Not String.IsNullOrEmpty(out) Then
            BedWriter.WriteTss(result.Tss, out, options.ScoreMode)
        End If
        If Not String.IsNullOrEmpty(dump) Then
            BedWriter.WriteDump(result.UnmodeledRegions, dump)
        End If

        Return result.Tss.Length
    End Function

    Private Function SelectSignificant(pvalues As Double(),
                                       plus As Double(),
                                       minus As Double(),
                                       normalizePlus As Double,
                                       normalizeMinus As Double,
                                       strand As Strands,
                                       chromosome As String,
                                       genomeSize As Integer,
                                       options As TssarOptions) As List(Of TssSite)

        Dim result As New List(Of TssSite)

        For position As Integer = 1 To genomeSize
            Dim pvalue As Double = pvalues(position)

            If Double.IsNaN(pvalue) OrElse pvalue > options.PValueCutoff Then
                Continue For
            End If

            Dim plusCoverage As Double = plus(position)

            If plusCoverage < options.MinPeakSize Then
                Continue For
            End If

            Dim difference As Double = plusCoverage * normalizePlus - minus(position) * normalizeMinus

            If difference <= 0.0 Then
                Continue For
            End If

            result.Add(New TssSite With {
                .Chromosome = chromosome,
                .Position = position,
                .Strand = strand,
                .PValue = pvalue,
                .PeakDifference = difference,
                .PlusCoverage = plusCoverage,
                .MinusCoverage = minus(position)
            })
        Next

        Return result
    End Function

    Private Function BuildDumpRegions(seenPlus As Boolean(),
                                      seenMinus As Boolean(),
                                      chromosome As String,
                                      genomeSize As Integer) As DumpRegion()

        Dim regions As New List(Of DumpRegion)

        regions.AddRange(BuildDumpRegions(seenPlus, chromosome, Strands.Forward, genomeSize))
        regions.AddRange(BuildDumpRegions(seenMinus, chromosome, Strands.Reverse, genomeSize))

        Return regions.ToArray()
    End Function

    Private Function BuildDumpRegions(seen As Boolean(),
                                      chromosome As String,
                                      strand As Strands,
                                      genomeSize As Integer) As IEnumerable(Of DumpRegion)

        Dim regions As New List(Of DumpRegion)
        Dim start As Integer = -1

        For position As Integer = 1 To genomeSize
            If Not seen(position) Then
                If start < 0 Then
                    start = position
                End If
            ElseIf start >= 0 Then
                regions.Add(New DumpRegion With {
                    .Chromosome = chromosome,
                    .Start = start,
                    .Ends = position - 1,
                    .Strand = strand
                })
                start = -1
            End If
        Next

        If start >= 0 Then
            regions.Add(New DumpRegion With {
                .Chromosome = chromosome,
                .Start = start,
                .Ends = genomeSize,
                .Strand = strand
            })
        End If

        Return regions
    End Function
End Module
