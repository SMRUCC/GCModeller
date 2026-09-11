Imports System.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSAR
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSAR.Models

Module Program

    Const GenomeSize As Integer = 3000
    Const ReadLength As Integer = 50

    Sub Main()
        Dim dir As String = IO.Path.Combine(IO.Path.GetTempPath(), "tsar_e2e")
        Call IO.Directory.CreateDirectory(dir)

        Dim plusSam As String = IO.Path.Combine(dir, "plus.sam")
        Dim minusSam As String = IO.Path.Combine(dir, "minus.sam")

        ' [+] library: strong TSS at 500 (+) and 2000 (-)
        IO.File.WriteAllText(plusSam, BuildSam(40, 30))
        ' [-] library: only background
        IO.File.WriteAllText(minusSam, BuildSam(1, 1))

        Dim pttFile As String = IO.Path.Combine(dir, "test.ptt")
        IO.File.WriteAllText(pttFile, BuildPtt())

        Dim options As New TssarOptions With {
            .PlusSam = plusSam,
            .MinusSam = minusSam,
            .GenomeSize = GenomeSize,
            .Chromosome = "test",
            .WindowSize = 1000,
            .MinPeakSize = 3,
            .PValueCutoff = 0.0001,
            .Clustering = False,
            .Ptt = pttFile,
            .Verbose = True
        }

        Dim result As TssarResult = TSSAR.Annotate(options)

        Console.WriteLine()
        Console.WriteLine($"=== individual TSS: {result.TotalIndividualTss} ===")
        For Each site As TssSite In result.Tss
            Console.WriteLine($"  pos={site.Position,-6} strand={site.Strand,-8} p={site.PValue:E3} diff={site.PeakDifference:F2} plus={site.PlusCoverage} minus={site.MinusCoverage}")
        Next

        Console.WriteLine($"=== unmodeled regions: {result.UnmodeledRegions.Length}, length={result.UnmodeledLength} ===")

        ' clustering test
        options.Clustering = True
        options.ClusterRange = 3
        Dim clustered As TssarResult = TSSAR.Annotate(options)
        Console.WriteLine($"=== clustered TSS: {clustered.TotalClusteredTss} ===")
        For Each site As TssSite In clustered.Tss
            Console.WriteLine($"  pos={site.Position,-6} strand={site.Strand,-8} type={site.Type,-10} gene={site.Gene} utr={site.UtrLength} tex={site.TexEfficiency:F3}")
        Next
        Console.WriteLine($"Primary UTR lengths: {String.Join(", ", clustered.PrimaryUtrLengths)}")
        Console.WriteLine($"Mean TEX efficiency: {clustered.MeanTexEfficiency:F4}")

        ' BED output test
        Dim bed As String = IO.Path.Combine(dir, "tss.bed")
        BedWriter.WriteTss(clustered.Tss, bed, ScoreModes.PeakDifference)
        Console.WriteLine("=== BED (score d) ===")
        Console.WriteLine(IO.File.ReadAllText(bed))

        Dim bedP As String = IO.Path.Combine(dir, "tss_p.bed")
        BedWriter.WriteTss(clustered.Tss, bedP, ScoreModes.PValue)
        Console.WriteLine("=== BED (score p) ===")
        Console.WriteLine(IO.File.ReadAllText(bedP))

        ' multiple testing correction
        options.MultipleTesting = "fdr"
        Dim mtcResult As TssarResult = TSSAR.Annotate(options)
        Console.WriteLine($"=== mtc=fdr clustered TSS: {mtcResult.TotalClusteredTss} ===")
        Console.WriteLine($"  dump regions: {mtcResult.UnmodeledRegions.Length}, unmodeled rate={mtcResult.UnmodeledRate:P2}")
    End Sub

    ''' <summary>
    ''' 生成 SAM：+ strand 背景位置 400..700（每位置 1 条），位置 500 额外 spike 条；
    ''' - strand 背景位置 1900..2100，位置 2000 额外 revSpike 条。
    ''' </summary>
    Private Function BuildSam(spike As Integer, revSpike As Integer) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("@HD	VN:1.0	SO:unsorted")
        sb.AppendLine($"@SQ	SN:test	LN:{GenomeSize}")

        Dim index As Integer = 0

        ' 正链 TSS：500(Primary)、1450(Internal)
        index = AppendForward(sb, index, 400, 700, 500, spike)
        index = AppendForward(sb, index, 1300, 1600, 1450, spike)

        ' 负链 TSS：2000(Primary)、2500(AntisenseInternal)
        index = AppendReverse(sb, index, 1900, 2100, 2000, revSpike)
        index = AppendReverse(sb, index, 2400, 2650, 2500, revSpike)

        Return sb.ToString()
    End Function

    Private Function AppendForward(sb As StringBuilder, index As Integer, fromPos As Integer, toPos As Integer, spikePos As Integer, spike As Integer) As Integer
        For pos As Integer = fromPos To toPos
            Dim count As Integer = If(pos = spikePos, spike, 1)
            For k As Integer = 1 To count
                sb.AppendLine(SamLine($"f{index}", 0, pos))
                index += 1
            Next
        Next
        Return index
    End Function

    Private Function AppendReverse(sb As StringBuilder, index As Integer, fromPos As Integer, toPos As Integer, spikePos As Integer, spike As Integer) As Integer
        For pos As Integer = fromPos To toPos
            Dim count As Integer = If(pos = spikePos, spike, 1)
            For k As Integer = 1 To count
                ' 反向链：read 的 5' 起始（start）位于 POS + span - 1
                sb.AppendLine(SamLine($"r{index}", 16, pos - ReadLength + 1))
                index += 1
            Next
        Next
        Return index
    End Function

    Private Function SamLine(qname As String, flag As Integer, pos As Integer) As String
        Dim seq As String = New String("A"c, ReadLength)
        Dim qual As String = New String("I"c, ReadLength)
        Return String.Join(ControlChars.Tab, qname, flag, "test", pos, 60, $"{ReadLength}M", "*", 0, 0, seq, qual)
    End Function

    ''' <summary>
    ''' 构造 PTT 注释：gene1 覆盖 600..900（正链），gene2 覆盖 1700..1950（负链）。
    ''' 因此 TSS 500（正链）与 2000（负链）都应当被分类为 Primary。
    ''' </summary>
    Private Function BuildPtt() As String
        Dim sb As New StringBuilder()
        sb.AppendLine("test genome - 1..3000")
        sb.AppendLine()
        sb.AppendLine("Location	Strand	Length	PID	Gene	Synonym	Code	COG	Product")
        sb.AppendLine("600..900	+	301	-	geneA	TAG_0001	-	-	product A")
        sb.AppendLine("1400..1500	+	101	-	geneC	TAG_0003	-	-	product C")
        sb.AppendLine("2400..2600	+	201	-	geneD	TAG_0004	-	-	product D")
        sb.AppendLine("1700..1950	-	251	-	geneB	TAG_0002	-	-	product B")
        Return sb.ToString()
    End Function
End Module
