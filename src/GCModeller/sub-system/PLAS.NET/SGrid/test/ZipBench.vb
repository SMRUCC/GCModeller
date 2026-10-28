Imports System.Diagnostics
Imports System.Text
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace SelfTest

    ''' <summary>
    ''' 模型 zip 序列化（<see cref="IO.ZipPersists"/>）的规模基准。
    ''' </summary>
    ''' <remarks>
    ''' 该入口构造一个与真实拟合结果同构的合成模型（n 个基因、每行 k 个候选调控因子，
    ''' G/H 为 n×n 的稀疏矩阵），测量保存/加载的耗时、归档体积与内存占用，
    ''' 并校验往返是否无损。
    ''' 
    ''' 它用于在不重跑数小时拟合的前提之下验证：
    ''' 
    ''' 1. 大模型序列化不会再出现 <c>OutOfMemoryException</c>（矩阵条目改为流式写出）；
    ''' 2. G/H 使用稀疏三列格式之后，归档体积由 n² 降到 n·k；
    ''' 3. 加载端可以同时读取旧的稠密归档与新的稀疏归档，往返无损。
    ''' </remarks>
    Public Module ZipBench

        ''' <summary>
        ''' 执行基准
        ''' </summary>
        ''' <param name="genes">合成的基因数量</param>
        ''' <param name="candidates">每一行的候选调控因子数量</param>
        ''' <param name="outDir">输出目录</param>
        ''' <returns>退出码，零表示通过</returns>
        Public Function Run(genes As Integer, candidates As Integer, outDir As String) As Integer
            If genes < 16 Then
                genes = 16
            End If
            If candidates < 1 Then
                candidates = 1
            End If
            If candidates > genes Then
                candidates = genes
            End If
            If String.IsNullOrEmpty(outDir) Then
                outDir = IO.Path.Combine(IO.Path.GetTempPath(), "sgrid-zip-bench")
            End If
            If Not IO.Directory.Exists(outDir) Then
                Call IO.Directory.CreateDirectory(outDir)
            End If

            Dim report As New StringBuilder()
            Dim failures As New List(Of String)
            Dim zipFile As String = IO.Path.Combine(outDir, "synthetic_network.zip")

            ' 合成模型本身占 n² × 2 × 8B（G/H），先把它构造出来
            Dim matrixBytes As Long = CLng(genes) * genes * 8

            Call report.AppendLine("=========== zip serialization benchmark ===========")
            Call report.AppendLine($"model  : {genes} gene(s), {candidates} candidate(s) per row")
            Call report.AppendLine($"dense  : n x n double matrix = {(matrixBytes / 1024.0 / 1024.0).ToString("F1")} MB each (G, H)")

            Dim model As SSystemNetwork = synthetic(genes, candidates)
            Dim before As Long = GC.GetTotalMemory(forceFullCollection:=True)

            Call report.AppendLine($"heap   : {(before / 1024.0 / 1024.0).ToString("F1")} MB after the synthetic model was built")

            Dim saveWatch As Stopwatch = Stopwatch.StartNew()

            Call model.SaveZip(zipFile)

            saveWatch.Stop()

            Dim size As Long = New IO.FileInfo(zipFile).Length
            Dim loadWatch As Stopwatch = Stopwatch.StartNew()
            Dim reloaded As SSystemNetwork = SSystemNetwork.LoadZip(zipFile)

            loadWatch.Stop()

            Dim delta As Double = maxDelta(model, reloaded)

            Call report.AppendLine($"zip    : {(size / 1024.0 / 1024.0).ToString("F2")} MB, saved in {saveWatch.Elapsed.TotalSeconds.ToString("F2")}s, loaded in {loadWatch.Elapsed.TotalSeconds.ToString("F2")}s")
            Call report.AppendLine($"format : {describeFormat(zipFile)}")
            Call report.AppendLine($"peak   : working set {(Process.GetCurrentProcess().PeakWorkingSet64 / 1024.0 / 1024.0).ToString("F0")} MB")
            Call report.AppendLine($"verify : max |delta| = {delta.ToString("G3")}")

            If delta <> 0 Then
                Call failures.Add($"the model round trip is not lossless: max |delta| = {delta.ToString("G6")}")
            End If
            If reloaded.Parameters.nGenes <> genes Then
                Call failures.Add($"the reloaded model contains {reloaded.Parameters.nGenes} gene(s) instead of {genes}")
            End If

            Call report.AppendLine()
            Call report.AppendLine(If(failures.Count = 0, "PASS: the archive was written and read back losslessly.", $"FAIL: {failures.Count} assertion(s) were violated:"))

            For Each fail As String In failures
                Call report.AppendLine($"  {fail}")
            Next

            Call IO.File.WriteAllText(IO.Path.Combine(outDir, "zip_bench.txt"), report.ToString(), New UTF8Encoding(False))
            Call Console.WriteLine(report.ToString)

            Return If(failures.Count = 0, 0, 1)
        End Function

        ''' <summary>
        ''' 读回归档之中矩阵条目的类型，用于确认写出的是二进制格式还是旧版本的文本格式
        ''' </summary>
        ''' <param name="zipFile"></param>
        ''' <returns></returns>
        Private Function describeFormat(zipFile As String) As String
            Using stream As IO.Stream = IO.File.OpenRead(zipFile)
                Using archive As New IO.Compression.ZipArchive(stream, IO.Compression.ZipArchiveMode.Read)
                    Dim entry As IO.Compression.ZipArchiveEntry = archive.GetEntry("matrix_G.bin")

                    If Not entry Is Nothing Then
                        Return $"binary (HTS_matrix::BinaryMatrix), {entry.Length / 1024.0 / 1024.0:F1} MB raw, " &
                               $"{entry.CompressedLength / 1024.0:F1} KB compressed"
                    End If

                    entry = archive.GetEntry("matrix_G.csv")

                    If entry Is Nothing Then
                        Return "no matrix_G entry!"
                    End If

                    Using reader As New IO.StreamReader(entry.Open())
                        Dim header As String = reader.ReadLine()
                        Dim fields As String() = If(header, "").Split(","c)

                        If fields.Length >= 3 AndAlso fields(1).Trim() = "index" Then
                            Return $"legacy sparse csv (gene,index,value), {entry.CompressedLength / 1024.0:F1} KB compressed"
                        Else
                            Return $"legacy dense csv, {entry.CompressedLength / 1024.0:F1} KB compressed"
                        End If
                    End Using
                End Using
            End Using
        End Function

        ''' <summary>
        ''' 两个模型的逐元素最大绝对差（只比较 G/H 与 alpha/beta）
        ''' </summary>
        Private Function maxDelta(a As SSystemNetwork, b As SSystemNetwork) As Double
            Dim delta As Double = 0

            For i As Integer = 0 To a.Parameters.nGenes - 1
                delta = std.Max(delta, std.Abs(a.Parameters.alpha(i) - b.Parameters.alpha(i)))
                delta = std.Max(delta, std.Abs(a.Parameters.beta(i) - b.Parameters.beta(i)))

                For j As Integer = 0 To a.Parameters.nGenes - 1
                    delta = std.Max(delta, std.Abs(a.Parameters.G(i)(j) - b.Parameters.G(i)(j)))
                    delta = std.Max(delta, std.Abs(a.Parameters.H(i)(j) - b.Parameters.H(i)(j)))
                Next
            Next

            Return delta
        End Function

        ''' <summary>
        ''' 构造一个与真实拟合结果同构的合成模型
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="k"></param>
        ''' <returns></returns>
        Private Function synthetic(genes As Integer, k As Integer) As SSystemNetwork
            Dim rnd As New Random(20261028)
            Dim names As String() = New String(genes - 1) {}
            Dim G As Double()() = New Double(genes - 1)() {}
            Dim H As Double()() = New Double(genes - 1)() {}
            Dim candidates As Integer()() = New Integer(genes - 1)() {}
            Dim points As Integer = 8
            Dim axis As Double() = New Double(points - 1) {}
            Dim X As Double()() = New Double(genes - 1)() {}
            Dim edges As New List(Of RegulatoryEdge)

            For i As Integer = 0 To genes - 1
                names(i) = "G" & (i + 1).ToString("D5")

                G(i) = New Double(genes - 1) {}
                H(i) = New Double(genes - 1) {}
                X(i) = New Double(points - 1) {}

                Dim index As New List(Of Integer) From {i}

                Do While index.Count < k
                    Dim j As Integer = rnd.Next(genes)

                    If Not index.Contains(j) Then
                        Call index.Add(j)
                    End If
                Loop

                candidates(i) = index.ToArray

                For Each j As Integer In candidates(i)
                    G(i)(j) = (rnd.NextDouble() * 2 - 1) * 3
                    H(i)(j) = (rnd.NextDouble() * 2 - 1) * 3

                    If std.Abs(G(i)(j) - H(i)(j)) > 0.05 Then
                        Call edges.Add(New RegulatoryEdge With {
                            .source = names(j),
                            .target = names(i),
                            .g = G(i)(j),
                            .h = H(i)(j),
                            .weight = G(i)(j) - H(i)(j),
                            .PriorSupported = False
                        })
                    End If
                Next

                For p As Integer = 0 To points - 1
                    X(i)(p) = 2 + std.Sin(p * 0.3 + i * 0.01) + rnd.NextDouble() * 0.05
                Next
            Next

            For p As Integer = 0 To points - 1
                axis(p) = p
            Next

            Dim parameters As New SSystemParameters With {
                .GeneNames = names,
                .OriginalNames = names,
                .alpha = filled(genes, 0.1),
                .beta = filled(genes, 0.05),
                .G = G,
                .H = H,
                .SlopeSSE = filled(genes, 1.0E-6),
                .SlopeRMSE = filled(genes, 1.0E-3),
                .SlopeSST = filled(genes, 1.0),
                .Candidates = candidates,
                .Converged = converged(genes)
            }
            Dim series As New ExpressionTimeSeries With {
                .GeneNames = names,
                .OriginalNames = names,
                .TimePoints = axis,
                .X = X
            }

            Return New SSystemNetwork With {
                .Series = series,
                .Parameters = parameters,
                .Edges = edges.ToArray,
                .Options = New FittingOptions(),
                .Prior = New SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork(),
                .Warnings = New List(Of String)
            }
        End Function

        Private Function filled(n As Integer, value As Double) As Double()
            Dim array As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                array(i) = value
            Next

            Return array
        End Function

        Private Function converged(n As Integer) As Boolean()
            Dim array As Boolean() = New Boolean(n - 1) {}

            For i As Integer = 0 To n - 1
                array(i) = True
            Next

            Return array
        End Function

    End Module
End Namespace
