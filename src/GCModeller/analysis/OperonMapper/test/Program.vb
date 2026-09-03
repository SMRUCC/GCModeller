' ============================================================================
' Program.vb — OperonPredictor 命令行入口
' ----------------------------------------------------------------------------
' 用法：
'   OperonPredictor predict --gff genome.gff [--fasta genome.fna] [--ptt genome.ptt]
'       [--homology map.tsv] [--reference-gff ref.gff:refId ...]
'       [--functions annot.tsv]
'       [--persistence 0.5] [--threshold 0.5]
'       [--p-barcode-in 0.15 --p-barcode-out 0.45]
'       [--p-conserved-in 0.35 --p-conserved-out 0.05]
'       [--w-distance 1.0 --w-barcode 1.0 --w-conserved 1.2
'        --w-terminator 0.8 --w-promoter 0.4 --w-function 0.3]
'       [--out operons.json] [--pretty]
'   OperonPredictor selftest
'
' 信号模块自动启用：有 --fasta → 序列信号；有 --homology → 比较基因组信号；
' 有 --functions → 功能信号。无外部输入时为纯 UniOP 距离/方向模式。
' ============================================================================

Imports System.Globalization
Imports System.IO
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports SMRUCC.genomics.Model.OperonMapper.OperonPredictor.Core
Imports SMRUCC.genomics.Model.OperonMapper.OperonPredictor.Model

Namespace OperonPredictor

    Public Module Program

        Private Const VersionString As String = "1.0.0"

        Public Function Main(args As String()) As Integer
            If args.Length = 0 OrElse args(0) = "--help" OrElse args(0) = "-h" Then
                PrintUsage()
                Return 0
            End If
            Dim cmd = args(0).ToLowerInvariant()
            If cmd = "selftest" Then Return SelfTest.RunAll()

            Try
                If cmd = "predict" Then
                    Return RunPredict(args)
                Else
                    Console.Error.WriteLine($"未知子命令: {cmd}")
                    Return 2
                End If
            Catch ex As Exception
                Console.Error.WriteLine($"错误: {ex.Message}")
                Return 1
            End Try
        End Function

        Private Function FlagValue(args As String(), name As String) As String
            For i = 0 To args.Length - 2
                If args(i).ToLowerInvariant() = name Then Return args(i + 1)
            Next
            Return Nothing
        End Function

        Private Function HasFlag(args As String(), name As String) As Boolean
            For i = 0 To args.Length - 1
                If args(i).ToLowerInvariant() = name Then Return True
            Next
            Return False
        End Function

        Private Function DblArg(args As String(), name As String, defVal As Double) As Double
            Dim v = FlagValue(args, name)
            If v Is Nothing Then Return defVal
            Return Double.Parse(v, CultureInfo.InvariantCulture)
        End Function

        Private Function RunPredict(args As String()) As Integer
            Dim gffPath = FlagValue(args, "--gff")
            Dim pttPath = FlagValue(args, "--ptt")
            If gffPath Is Nothing AndAlso pttPath Is Nothing Then
                Console.Error.WriteLine("必须提供 --gff 或 --ptt 基因注释")
                Return 2
            End If
            Dim outPath = FlagValue(args, "--out")

            ' ---- 输入 ----
            Dim genes As New List(Of Gene)()
            Dim contigNames As New List(Of String)()
            If gffPath IsNot Nothing Then
                genes = AnnotationIO.ReadGff(gffPath, "contig_1")
                contigNames = genes.Select(Function(g) g.Contig).Distinct().ToList()
            Else
                Dim contig = If(FlagValue(args, "--contig"), "contig_1")
                genes = AnnotationIO.ReadPtt(pttPath, contig)
                contigNames.Add(contig)
            End If
            If genes.Count < 2 Then
                Console.Error.WriteLine("基因数 < 2，无法预测")
                Return 2
            End If

            Dim fasta As Dictionary(Of String, String) = Nothing
            Dim fastaPath = FlagValue(args, "--fasta")
            If fastaPath IsNot Nothing Then
                fasta = AnnotationIO.ReadFasta(fastaPath)
            End If

            Dim homologyMap As Dictionary(Of String, Dictionary(Of String, Tuple(Of String, Double))) = Nothing
            Dim homologyPath = FlagValue(args, "--homology")
            If homologyPath IsNot Nothing Then
                homologyMap = AnnotationIO.ReadHomology(homologyPath)
            End If
            Dim refGffs As Dictionary(Of String, List(Of Gene)) = Nothing
            Dim refSpecs As New List(Of Tuple(Of String, String))()
            For i = 0 To args.Length - 2
                If args(i).ToLowerInvariant() = "--reference-gff" Then
                    Dim spec = args(i + 1)
                    Dim ci = spec.LastIndexOf(":"c)
                    If ci > 0 Then
                        refSpecs.Add(Tuple.Create(spec.Substring(0, ci), spec.Substring(ci + 1)))
                    Else
                        refSpecs.Add(Tuple.Create(spec, IO.Path.GetFileNameWithoutExtension(spec)))
                    End If
                End If
            Next
            If refSpecs.Count > 0 Then
                refGffs = AnnotationIO.ReadReferenceGffs(refSpecs)
            End If

            Dim functions As Dictionary(Of String, String) = Nothing
            Dim funcPath = FlagValue(args, "--functions")
            If funcPath IsNot Nothing Then
                functions = AnnotationIO.ReadFunctions(funcPath)
            End If

            ' ---- 选项 ----
            Dim intOpts As New IntegrationOptions With {
                .WPrior = DblArg(args, "--w-distance", 1.0),
                .WBarcode = DblArg(args, "--w-barcode", 1.0),
                .WConserved = DblArg(args, "--w-conserved", 1.2),
                .WTerminator = DblArg(args, "--w-terminator", 0.8),
                .WPromoter = DblArg(args, "--w-promoter", 0.4),
                .WFunction = DblArg(args, "--w-function", 0.3),
                .Persistence = DblArg(args, "--persistence", 0.5),
                .PBarcodeIn = DblArg(args, "--p-barcode-in", 0.15),
                .PBarcodeOut = DblArg(args, "--p-barcode-out", 0.45),
                .PConservedIn = DblArg(args, "--p-conserved-in", 0.35),
                .PConservedOut = DblArg(args, "--p-conserved-out", 0.05)}
            Dim threshold = DblArg(args, "--threshold", 0.5)

            Dim engOpts As New EngineOptions With {
                .Integration = intOpts,
                .UseSequenceSignals = fasta IsNot Nothing,
                .UseComparative = homologyMap IsNot Nothing AndAlso refGffs IsNot Nothing,
                .UseFunction = functions IsNot Nothing}
            If fasta IsNot Nothing Then
                ' 仅对出现的 contig 提供序列
                engOpts.UseSequenceSignals = contigNames.Any(Function(c) fasta.ContainsKey(c))
            End If

            ' ---- 预测 ----
            Dim homologySignals As HomologySignals = Nothing
            If engOpts.UseComparative Then homologySignals = New HomologySignals(homologyMap, refGffs)

            Dim sw = System.Diagnostics.Stopwatch.StartNew()
            Dim result = Engine.Predict(genes, fasta, homologySignals, functions, engOpts)
            Dim pairs = result.Item1
            Dim signals = result.Item2
            Dim qPrior = result.Item3
            sw.Stop()

            ' ---- 操纵子装配（阈值可覆盖 Viterbi：--threshold 只影响展示边界判断一致性）----
            ' Viterbi 为全局一致解；threshold 用于 combined_posterior 的独立参考判断
            Dim operons = Engine.AssembleOperons(pairs, signals)

            ' ---- 报告 ----
            Dim geneDtos = genes.OrderBy(Function(g) g.Contig).ThenBy(Function(g) g.StartMin).
                Select(Function(g) New GeneDto With {
                    .Id = g.Id, .Contig = g.Contig, .Start = g.StartMin,
                    .End = g.EndMax, .Strand = g.Strand.ToString()}).ToList()

            Dim pairDtos As New List(Of PairDto)()
            For i = 0 To pairs.Count - 1
                Dim pr = pairs(i)
                Dim sg = signals(i)
                Dim pattern As String
                Select Case pr.Relation
                    Case StrandRelation.Same : pattern = "same"
                    Case StrandRelation.Convergent : pattern = "convergent"
                    Case Else : pattern = "divergent"
                End Select
                pairDtos.Add(New PairDto With {
                    .GeneA = pr.A.Id, .GeneB = pr.B.Id,
                    .StrandPattern = pattern, .Igd = pr.Igd,
                    .Scores = New ScoreDto With {
                        .DistancePosterior = Math.Round(Math.Max(0.0, sg.UniopPosterior), 5),
                        .LlrDistance = Math.Round(sg.LlrDistance, 4),
                        .BarcodeHamming = sg.BarcodeHamming,
                        .BarcodeRefs = sg.BarcodeRefs,
                        .LlrBarcode = Math.Round(sg.BarcodeLlr, 4),
                        .ConservedCount = Math.Max(0, sg.ConservedCount),
                        .LlrConserved = Math.Round(sg.ConservedLlr, 4),
                        .PcbbhCount = Math.Max(0, sg.PcbbhCount),
                        .TerminatorStrength = Math.Round(Math.Max(0.0, sg.TerminatorStrength), 4),
                        .LlrTerminator = Math.Round(sg.LlrTerminator, 4),
                        .PromoterStrength = Math.Round(Math.Max(0.0, sg.PromoterStrength), 4),
                        .LlrPromoter = Math.Round(sg.LlrPromoter, 4),
                        .FunctionalMatch = If(sg.FunctionalMatch.HasValue,
                                              If(sg.FunctionalMatch.Value, "true", "false"), "na"),
                        .LlrFunction = Math.Round(sg.LlrFunction, 4),
                        .CombinedLlr = Math.Round(sg.CombinedLlr, 4),
                        .CombinedPosterior = Math.Round(sg.CombinedPosterior, 5),
                        .HmmPosterior = Math.Round(sg.HmmPosterior, 5)},
                    .SameOperon = sg.ViterbiState AndAlso pr.IsSameStrand})
            Next

            Dim operonDtos = operons.Select(Function(o) New OperonDto With {
                .OperonId = o.OperonId, .Contig = o.Contig, .Strand = o.Strand,
                .Start = o.Start, .End = o.[End], .NumGenes = o.NumGenes,
                .Genes = o.Genes, .GeneStarts = o.GeneStarts, .GeneEnds = o.GeneEnds,
                .MeanPairPosterior = Math.Round(o.MeanPairPosterior, 5)}).ToList()

            Dim report As New OperonReport With {
                .Program = "OperonPredictor",
                .Version = VersionString,
                .Parameters = New PredictionParameters With {
                    .NumContigs = contigNames.Count,
                    .Weights = New Dictionary(Of String, Double) From {
                        {"distance", intOpts.WPrior}, {"barcode", intOpts.WBarcode},
                        {"conserved", intOpts.WConserved}, {"terminator", intOpts.WTerminator},
                        {"promoter", intOpts.WPromoter}, {"function", intOpts.WFunction}},
                    .Persistence = intOpts.Persistence,
                    .PBarcodeIn = intOpts.PBarcodeIn,
                    .PBarcodeOut = intOpts.PBarcodeOut,
                    .PConservedIn = intOpts.PConservedIn,
                    .PConservedOut = intOpts.PConservedOut,
                    .SequenceSignals = engOpts.UseSequenceSignals,
                    .ComparativeSignals = engOpts.UseComparative,
                    .NumReferenceGenomes = If(refGffs IsNot Nothing, refGffs.Count, 0)},
                .Summary = New PredictionSummary With {
                    .NumGenes = genes.Count,
                    .NumPairs = pairs.Count,
                    .NumSameStrandPairs = pairs.Where(Function(p) p.IsSameStrand).Count,
                    .NumOppositePairs = pairs.Count - pairs.Where(Function(p) p.IsSameStrand).Count,
                    .UniopPriorQ = Math.Round(qPrior, 5),
                    .NumOperons = operons.Count,
                    .NumMultiGeneOperons = operons.Where(Function(o) o.NumGenes >= 2).Count,
                    .MeanOperonSize = If(operons.Count > 0, Math.Round(operons.Average(Function(o) CDbl(o.NumGenes)), 3), 0)},
                .Genes = geneDtos,
                .Pairs = pairDtos,
                .Operons = operonDtos}

            Dim jsonOpts As New JsonSerializerOptions With {
                .WriteIndented = HasFlag(args, "--pretty"),
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull}
            Dim json = JsonSerializer.Serialize(report, jsonOpts)
            If outPath IsNot Nothing Then
                File.WriteAllText(outPath, json)
                Console.Error.WriteLine($"结果已写入 {outPath}")
            Else
                Console.Out.WriteLine(json)
            End If
            Console.Error.WriteLine($"OperonPredictor {VersionString}: {genes.Count} 基因, " &
                                    $"{pairs.Count} 相邻对, q={qPrior:F3}, " &
                                    $"{operons.Count} 操纵子（多基因 {report.Summary.NumMultiGeneOperons}）" &
                                    $"（{sw.Elapsed.TotalSeconds:F1}s）")
            Return 0
        End Function

        Private Sub PrintUsage()
            Console.WriteLine("OperonPredictor — 细菌基因组操纵子预测（多信号概率整合，纯 BCL）")
            Console.WriteLine()
            Console.WriteLine("用法:")
            Console.WriteLine("  OperonPredictor predict --gff genome.gff [--fasta genome.fna]")
            Console.WriteLine("      [--homology map.tsv] [--reference-gff ref.gff:refId ...]")
            Console.WriteLine("      [--functions annot.tsv] [--out operons.json] [--pretty]")
            Console.WriteLine("  OperonPredictor selftest")
            Console.WriteLine()
            Console.WriteLine("信号模块: 距离(UniOP 无监督) + 链向(硬规则) 总是启用；")
            Console.WriteLine("  终止子/启动子扫描需 --fasta；发育条形码/保守对/PCBBH 需 --homology + --reference-gff；")
            Console.WriteLine("  功能相关性需 --functions。同源映射可由 MiniBlast blastp 预生成。")
        End Sub

    End Module

End Namespace
