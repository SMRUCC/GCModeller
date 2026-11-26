Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.Analysis.VirtualCellular.Data
Imports SMRUCC.genomics.Analysis.VirtualCellular.IO
Imports SMRUCC.genomics.Analysis.VirtualCellular.Model
Imports SMRUCC.genomics.Analysis.VirtualCellular.Perturbation

''' <summary>
''' 鼻咽癌虚拟细胞 Demo：装配模型 →（可选）持久化 → BATF / BATF3 / 联合过表达 → 导出三套转录组。
''' </summary>
''' <remarks>
''' <code>
''' VirtualCellular.Demo --expr E:\hyperomics_24samples\expression.csv ^
'''                      --meta E:\hyperomics_24samples\sampleinfo.csv ^
'''                      --model D:\npc\model.zip --out D:\npc\results --rebuild
''' </code>
''' 可选：<c>--stages III,IVB,IVC,NA</c>（默认全部）、<c>--limit 200</c>（期别内等距抽样，0=全量）、
''' <c>--cells 400</c>（每次扰动的合成细胞数）、<c>--seed 20261126</c>。
''' </remarks>
Public Module Program

    Public Function Main(args As String()) As Integer
        Dim options As Dictionary(Of String, String) = ParseArgs(args)
        Dim expr As String = ValueOf(options, "expr", "E:\hyperomics_24samples\expression.csv")
        Dim meta As String = ValueOf(options, "meta", "E:\hyperomics_24samples\sampleinfo.csv")
        Dim modelPath As String = ValueOf(options, "model", Path.Combine(Environment.CurrentDirectory, "npc-model.zip"))
        Dim outDir As String = ValueOf(options, "out", Path.Combine(Environment.CurrentDirectory, "npc-results"))
        Dim rebuild As Boolean = options.ContainsKey("rebuild") OrElse Not File.Exists(modelPath)
        Dim limit As Integer = CInt(ValueOf(options, "limit", "0"))
        Dim nCells As Integer = CInt(ValueOf(options, "cells", "400"))
        Dim seed As Integer = CInt(ValueOf(options, "seed", "20261126"))
        Dim stages As String() = ValueOf(options, "stages", "").
            Split(","c).Select(Function(s) s.Trim()).Where(Function(s) s.Length > 0).ToArray
        Dim clock As Stopwatch = Stopwatch.StartNew()
        Dim model As VirtualCellModel

        Call Directory.CreateDirectory(outDir)

        If rebuild Then
            Call Console.WriteLine($"[demo] loading {expr} (limit={If(limit = 0, "all", limit.ToString())} per stage)")

            Dim stream As ExpressionStream = ExpressionStream.Load(expr, meta, stages, verbose:=True, limitPerStage:=limit)

            Call Console.WriteLine($"[demo] {stream.Cells.Length} cell x {stream.Genes.Length} gene, " &
                                  $"missing metadata = {stream.MissingMetadata.Length}")
            Call Console.WriteLine($"[demo] normal selector = {stream.Metadata.NormalSelector}")

            Dim assemblyClock As Stopwatch = Stopwatch.StartNew()
            Dim bundle = SummaryAssembly.Build(stream)

            assemblyClock.Stop()
            Call Console.WriteLine($"[demo] assembled {bundle.Catalog.Count} record(s) in {assemblyClock.ElapsedMilliseconds} ms")

            model = New VirtualCellModel With {
                .Options = SMRUCC.genomics.Analysis.Illumina.CellularAI.Model.CellularAiModel.Default(),
                .Bundle = bundle,
                .Controls = stream.Controls,
                .Genes = stream.Genes,
                .Cells = stream.Cells,
                .Metadata = stream.Metadata,
                .Manifest = New ModelManifest With {
                    .Seed = seed,
                    .NormalSelector = stream.Metadata.NormalSelector,
                    .StageFilter = stream.StageFilter
                }
            }

            Call Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(modelPath)))

            Using output As Stream = File.Create(modelPath)
                Call model.Save(output)
            End Using

            Call Console.WriteLine($"[demo] model -> {modelPath} ({New FileInfo(modelPath).Length / 1048576.0:F1} MB)")
        Else
            Call Console.WriteLine($"[demo] loading model {modelPath}")

            Using input As Stream = File.OpenRead(modelPath)
                model = VirtualCellModel.Load(input)
            End Using

            Call Console.WriteLine($"[demo] {model}")
        End If

        Dim engine As New CellularAIPerturbation(model, seed)
        Dim targets As String() = ValueOf(options, "genes", "BATF,BATF3").
            Split(","c).Select(Function(s) s.Trim().ToUpperInvariant()).
            Where(Function(s) s.Length > 0).ToArray
        Dim runs As New List(Of (Name As String, Result As Object))

        If targets.Length = 0 Then
            Call Console.WriteLine("[demo] no target gene given")
            Return 1
        End If

        ' ---- 单基因过表达 ----
        For Each gene As String In targets
            Call Export(engine.OverexpressGene(gene, nCells), gene, outDir, model)
            Call Measure(engine, gene)
        Next

        ' ---- 联合过表达 ----
        If targets.Length > 1 Then
            Dim joint As String = String.Join("+", targets)

            Call Export(engine.OverexpressGenes(targets, nCells), joint, outDir, model)
            Call Measure(engine, joint)
        End If

        clock.Stop()
        Call Console.WriteLine($"[demo] done in {clock.Elapsed.TotalSeconds:F1} s -> {outDir}")

        Return 0
    End Function

#Region "helpers"

    Private Sub Measure(engine As CellularAIPerturbation, label As String)
        Dim cells As Double = 0

        Call engine.Diagnostics.TryGetValue("cells", cells)
        Call Console.WriteLine($"[demo]   {label}: {cells:F0} cell(s) synthesised")
    End Sub

    Private Sub Export(result As SMRUCC.genomics.BioSystem.Intervention.InterventionResult,
                       label As String, outDir As String, model As VirtualCellModel)
        If result Is Nothing OrElse result.Undefined Then
            Call Console.WriteLine($"[demo] {label}: undefined (target not on the gene axis)")
            Return
        End If

        Dim folder As String = Path.Combine(outDir, $"{label}_oe")

        Call Directory.CreateDirectory(folder)
        Call ResultExporter.WriteDe(result, Path.Combine(folder, "de.csv"))
        Call ResultExporter.WriteSummary(result, Path.Combine(folder, "summary.csv"))

        Call Console.WriteLine($"[demo] {label}: {result.NAffected} significant gene(s) -> {folder}")
    End Sub

    Private Function ParseArgs(args As String()) As Dictionary(Of String, String)
        Dim table As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        If args Is Nothing Then
            Return table
        End If

        Dim i As Integer = 0

        While i < args.Length
            Dim key As String = args(i)

            If Not key.StartsWith("--") Then
                i += 1
                Continue While
            End If

            key = key.Substring(2)

            If i + 1 < args.Length AndAlso Not args(i + 1).StartsWith("--") Then
                table(key) = args(i + 1)
                i += 2
            Else
                table(key) = ""
                i += 1
            End If
        End While

        Return table
    End Function

    Private Function ValueOf(table As Dictionary(Of String, String), key As String, fallback As String) As String
        Dim value As String = Nothing

        If table.TryGetValue(key, value) AndAlso Not String.IsNullOrEmpty(value) Then
            Return value
        End If

        Return fallback
    End Function

#End Region

End Module
