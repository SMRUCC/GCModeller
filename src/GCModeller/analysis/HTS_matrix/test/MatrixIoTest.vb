Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports BinaryMatrixCodec = Microsoft.VisualBasic.Serialization.BinaryDumping.NetworkByteOrderBuffer.Compression

''' <summary>
''' 超大型矩阵流式 IO 的回归测试：二进制流式写入、惰性读取、随机访问、压缩、
''' CSV ⇄ 二进制转换、格式兼容与损坏检测。
''' </summary>
''' <remarks>
''' <para>
''' 测试结果统一通过 <see cref="DebugAudit"/> 记录，运行结束后输出文本摘要，
''' 并把 JSON 报告写到 <c>report/hts_matrix_io.json</c>；进程退出码等于失败断言的数量。
''' </para>
''' <para>
''' 运行：<c>dotnet run -c Release</c>
''' </para>
''' </remarks>
Friend Module MatrixIoTest

    ''' <summary>
    ''' 已经执行过的用例数量
    ''' </summary>
    Private executedCases As Integer = 0

    ''' <summary>
    ''' 执行全部用例
    ''' </summary>
    ''' <returns>失败的断言数量</returns>
    Friend Function Run() As Integer
        Dim originMute As Boolean = VBDebugger.Mute

        executedCases = 0

        Call DebugAudit.Reset()

        ' 审计失败信息走 VBDebugger 日志通道，这里静默掉以保持控制台输出干净
        VBDebugger.Mute = True

        Dim folder As String = IO.Path.Combine(
            IO.Path.GetTempPath(),
            "hts_matrix_io_test_" & Guid.NewGuid().ToString("N").Substring(0, 8)
        )

        Call Directory.CreateDirectory(folder)

        Try
            Call testNetworkByteOrderCodec()
            Call testV2RoundTrip(folder)
            Call testV1Compatibility(folder)
            Call testCompressed(folder, BinaryMatrixCodec.gzip)
            Call testCompressed(folder, BinaryMatrixCodec.zlib)
            Call testDeferredFeatureIds(folder)
            Call testRandomAccessAndConcurrency(folder)
            Call testNonSeekableStream(folder)
            Call testTruncatedData(folder)
            Call testInvalidMagic(folder)
            Call testViewerParity(folder)
            Call testCsvRoundTrip(folder)
            Call testRollback(folder)
            Call testChunkedScan(folder)
        Finally
            VBDebugger.Mute = originMute

            Try
                Call Directory.Delete(folder, recursive:=True)
            Catch ex As Exception
                ' 临时目录清理失败不影响测试结论
            End Try
        End Try

        Call DebugAudit.Mark("cases", $"{executedCases} case(s) executed, {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

    ''' <summary>
    ''' 输出审计摘要并把 JSON 报告写到 <c>report/hts_matrix_io.json</c>
    ''' </summary>
    ''' <returns>报告文件的绝对路径</returns>
    Friend Function Report() As String
        Console.WriteLine(DebugAudit.Dump())

        Dim path As String = IO.Path.Combine("report", "hts_matrix_io.json")

        Call DebugAudit.SaveReport(path)

        Console.WriteLine($"audit report: {IO.Path.GetFullPath(path)}")
        Console.WriteLine($"failures: {DebugAudit.FailureCount}")

        Return IO.Path.GetFullPath(path)
    End Function

    Private Sub runCase(name As String, body As Action)
        executedCases += 1

        Call DebugAudit.Mark($"case::{name}")

        Try
            Call body()
        Catch ex As Exception
            Call DebugAudit.Test(False, $"[{name}] threw {ex.GetType().Name}: {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' 构造一个带固定随机种子的测试矩阵
    ''' </summary>
    Private Function createMatrix(nGenes As Integer,
                                  nSamples As Integer,
                                  Optional seed As Integer = 0,
                                  Optional withNaN As Boolean = True) As Matrix

        Dim rnd As New Random(seed)
        Dim samples As String() = New String(nSamples - 1) {}
        Dim rows As DataFrameRow() = New DataFrameRow(nGenes - 1) {}

        For i As Integer = 0 To samples.Length - 1
            samples(i) = $"sample_{i + 1}"
        Next

        For i As Integer = 0 To rows.Length - 1
            Dim v As Double() = New Double(nSamples - 1) {}

            For j As Integer = 0 To v.Length - 1
                Select Case (i + j) Mod 13
                    Case 0
                        v(j) = If(withNaN, Double.NaN, 0.0)
                    Case 1
                        v(j) = 0.0
                    Case 2
                        v(j) = -0.0
                    Case Else
                        v(j) = (rnd.NextDouble() - 0.5) * 10000.0
                End Select
            Next

            rows(i) = New DataFrameRow With {
                .geneID = $"gene_{i + 1}",
                .experiments = v
            }
        Next

        Return New Matrix With {
            .tag = "test_matrix",
            .sampleID = samples,
            .expression = rows
        }
    End Function

    ''' <summary>
    ''' 逐值比较视图与内存矩阵，返回不一致的数值个数
    ''' </summary>
    Private Function compareValues(view As MatrixViewer, expected As Matrix, ByRef detail As String) As Integer
        Dim mismatches As Integer = 0

        detail = ""

        If view.Size.nsample <> expected.sample_count Then
            mismatches += 1
            detail &= $"sample count {view.Size.nsample} <> {expected.sample_count}; "
        End If
        If view.Size.nfeature <> expected.size Then
            mismatches += 1
            detail &= $"feature count {view.Size.nfeature} <> {expected.size}; "
        End If

        For i As Integer = 0 To expected.size - 1
            Dim row As DataFrameRow = view.GetRow(i)
            Dim origin As DataFrameRow = expected.expression(i)

            If row.geneID <> origin.geneID Then
                mismatches += 1
                detail &= $"row {i} name '{row.geneID}' <> '{origin.geneID}'; "
            End If

            If row.experiments.Length <> origin.experiments.Length Then
                mismatches += 1
                detail &= $"row {i} size {row.experiments.Length} <> {origin.experiments.Length}; "
                Continue For
            End If

            For j As Integer = 0 To origin.experiments.Length - 1
                Dim a As Double = row.experiments(j)
                Dim b As Double = origin.experiments(j)
                Dim same As Boolean = (Double.IsNaN(a) AndAlso Double.IsNaN(b)) OrElse a = b

                If Not same Then
                    mismatches += 1

                    If detail.Length < 240 Then
                        detail &= $"row {i} col {j}: {a:R} <> {b:R}; "
                    End If
                End If
            Next
        Next

        Return mismatches
    End Function

    Private Sub testNetworkByteOrderCodec()
        Call runCase("network byte order codec", Sub()
            Dim bin As New NetworkByteOrderBuffer()
            Dim values As Double() = {1.5, -1.5, 0.0, -0.0, Double.NaN, Double.Epsilon, 1.0E+300, Double.MaxValue}
            Dim expected As Byte() = bin.encode(values)
            Dim buffer As Byte() = New Byte(expected.Length - 1) {}

            Call bin.EncodeDoubles(values, 0, values.Length, buffer, 0)
            Call DebugAudit.Test(buffer.SequenceEqual(expected), "EncodeDoubles should be byte identical to the legacy encoder")

            Dim decoded As Double() = New Double(values.Length - 1) {}

            Call bin.DecodeDoubles(expected, 0, values.Length, decoded, 0)
            Call DebugAudit.Test(
                decoded.Select(Function(d, i) BitConverter.DoubleToInt64Bits(d) = BitConverter.DoubleToInt64Bits(values(i))).All(Function(ok) ok),
                "DecodeDoubles should reproduce the exact bit pattern of every element"
            )
            Call DebugAudit.AreEqual(
                BitConverter.DoubleToInt64Bits(values(4)),
                BitConverter.DoubleToInt64Bits(bin.DecodeDouble(expected, 32)),
                "DecodeDouble should decode a single element at the given offset"
            )
        End Sub)
    End Sub

    Private Sub testV2RoundTrip(folder As String)
        Call runCase("v2 round trip", Sub()
            Dim mat As Matrix = createMatrix(137, 23, seed:=1)
            Dim file As String = IO.Path.Combine(folder, "v2.bin")

            Call mat.SaveFile(file, version:=2)

            Using view As HTSMatrixReader = Matrix.LoadLazy(file)
                Dim detail As String = ""
                Dim mismatches As Integer = compareValues(view, mat, detail)

                Call DebugAudit.Test(mismatches = 0, $"v2 round trip should reproduce every value ({mismatches} mismatch(es): {detail})")
                Call DebugAudit.AreEqual(2, view.Version, "the file should be written as format v2")
                Call DebugAudit.AreEqual("test_matrix", view.tag, "the matrix tag should be preserved")
                Call DebugAudit.AreEqual(137, view.Size.nfeature, "row count")
                Call DebugAudit.AreEqual(23, view.Size.nsample, "column count")
                Call DebugAudit.Test(view.CanRandomAccess, "an uncompressed file should support row level random access")
            End Using

            ' 全量加载路径（BinaryMatrix.LoadStream）
            Dim loaded As Matrix = Matrix.LoadStreamData(file)
            Dim detail2 As String = ""
            Dim mismatch2 As Integer = compareValues(New HTSMatrixViewer(loaded), mat, detail2)

            Call DebugAudit.Test(mismatch2 = 0, $"LoadStreamData should reproduce every value ({mismatch2} mismatch(es): {detail2})")
        End Sub)
    End Sub

    Private Sub testV1Compatibility(folder As String)
        Call runCase("v1 compatibility", Sub()
            Dim mat As Matrix = createMatrix(64, 9, seed:=2)
            Dim file As String = IO.Path.Combine(folder, "v1.bin")

            Call mat.SaveFile(file, version:=1)

            Using view As HTSMatrixReader = Matrix.LoadLazy(file)
                Dim detail As String = ""
                Dim mismatches As Integer = compareValues(view, mat, detail)

                Call DebugAudit.AreEqual(1, view.Version, "the file should be written as the legacy v1 format")
                Call DebugAudit.Test(mismatches = 0, $"v1 files should still be readable ({mismatches} mismatch(es): {detail})")
            End Using

            ' 旧读取器（BinaryMatrix.MagicHeader）应当仍然匹配 v1 文件的 magic
            Dim raw As Byte() = IO.File.ReadAllBytes(file)
            Call DebugAudit.AreEqual(
                BinaryMatrix.MagicHeader,
                Encoding.ASCII.GetString(raw, 0, BinaryMatrix.MagicHeader.Length),
                "v1 files should keep the legacy magic header"
            )
        End Sub)
    End Sub

    Private Sub testCompressed(folder As String, codec As BinaryMatrixCodec)
        Call runCase($"{codec} compressed round trip", Sub()
            Dim mat As Matrix = createMatrix(300, 31, seed:=3)
            Dim file As String = IO.Path.Combine(folder, $"compressed_{codec}.bin")

            Call mat.SaveFile(file, codec:=codec, blockRows:=64, version:=2)

            Using view As HTSMatrixReader = Matrix.LoadLazy(file)
                Call DebugAudit.AreEqual(codec, view.Codec, "the codec should be recorded in the header")
                Call DebugAudit.Test(view.CanRandomAccess, "a compressed file with a block index should support block level random access")

                ' 顺序枚举
                Dim sequential As DataFrameRow() = view.Rows().ToArray
                Dim detail As String = ""
                Dim mismatches As Integer = 0

                Call DebugAudit.AreEqual(mat.size, sequential.Length, "sequential enumeration row count")

                For i As Integer = 0 To sequential.Length - 1
                    For j As Integer = 0 To mat.sample_count - 1
                        Dim a As Double = sequential(i).experiments(j)
                        Dim b As Double = mat.expression(i).experiments(j)

                        If Not ((Double.IsNaN(a) AndAlso Double.IsNaN(b)) OrElse a = b) Then
                            mismatches += 1
                        End If
                    Next
                Next

                Call DebugAudit.Test(mismatches = 0, $"sequential enumeration should reproduce every value ({mismatches} mismatch(es))")

                ' 块级随机访问（刻意跨块取行）
                Dim probes As Integer() = {0, 63, 64, 128, 200, 299}
                Dim probeMismatches As Integer = 0
                Dim detail2 As String = ""

                For Each i As Integer In probes
                    Dim row As DataFrameRow = view.GetRow(i)

                    For j As Integer = 0 To mat.sample_count - 1
                        Dim a As Double = row.experiments(j)
                        Dim b As Double = mat.expression(i).experiments(j)

                        If Not ((Double.IsNaN(a) AndAlso Double.IsNaN(b)) OrElse a = b) Then
                            probeMismatches += 1

                            If detail2.Length < 200 Then
                                detail2 &= $"row {i} col {j}: {a:R} <> {b:R}; "
                            End If
                        End If
                    Next
                Next

                Call DebugAudit.Test(probeMismatches = 0, $"block level random access should reproduce the probe rows ({probeMismatches} mismatch(es): {detail2})")
            End Using
        End Sub)
    End Sub

    Private Sub testDeferredFeatureIds(folder As String)
        Call runCase("deferred feature ids", Sub()
            Dim mat As Matrix = createMatrix(97, 13, seed:=4)
            Dim file As String = IO.Path.Combine(folder, "deferred.bin")

            ' 写入时不知道总行数：特征名与行数在 Complete() 时回填
            Using writer As HTSMatrixWriter = HTSMatrixWriter.CreateFile(
                path:=file,
                sampleIDs:=mat.sampleID,
                featureIDs:=Nothing,
                tag:="deferred"
            )

                For Each row As DataFrameRow In mat.expression
                    Call writer.WriteRow(row)
                Next

                Call writer.Complete()
            End Using

            Using view As HTSMatrixReader = Matrix.LoadLazy(file)
                Dim detail As String = ""
                Dim mismatches As Integer = compareValues(view, mat, detail)

                Call DebugAudit.AreEqual("deferred", view.tag, "the tag should be preserved")
                Call DebugAudit.AreEqual(97, view.Size.nfeature, "the row count should be patched back into the header")
                Call DebugAudit.AreEqual("gene_97", view.FeatureIDs.Last(), "the feature names should be recovered from the trailer")
                Call DebugAudit.Test(mismatches = 0, $"deferred writing should reproduce every value ({mismatches} mismatch(es): {detail})")
            End Using
        End Sub)
    End Sub

    Private Sub testRandomAccessAndConcurrency(folder As String)
        Call runCase("random access and concurrency", Sub()
            Dim mat As Matrix = createMatrix(211, 17, seed:=5)
            Dim file As String = IO.Path.Combine(folder, "concurrent.bin")

            Call mat.SaveFile(file)

            Using view As HTSMatrixReader = Matrix.LoadLazy(file)
                ' 区间读取
                Dim block As DataFrameRow() = view.GetRows(90, 25)
                Dim detail As String = ""
                Dim mismatches As Integer = 0

                For i As Integer = 0 To block.Length - 1
                    If block(i).geneID <> mat.expression(90 + i).geneID Then
                        mismatches += 1
                    End If
                    For j As Integer = 0 To mat.sample_count - 1
                        Dim a As Double = block(i).experiments(j)
                        Dim b As Double = mat.expression(90 + i).experiments(j)

                        If Not ((Double.IsNaN(a) AndAlso Double.IsNaN(b)) OrElse a = b) Then
                            mismatches += 1
                        End If
                    Next
                Next

                Call DebugAudit.Test(mismatches = 0, $"GetRows should return a contiguous slice ({mismatches} mismatch(es): {detail})")

                ' 样本列读取
                Dim column As Double() = view.GetSample(5)

                Call DebugAudit.AreEqual(mat.size, column.Length, "GetSample length")

                Dim columnMismatches As Integer = 0

                For i As Integer = 0 To column.Length - 1
                    Dim a As Double = column(i)
                    Dim b As Double = mat.expression(i).experiments(5)

                    If Not ((Double.IsNaN(a) AndAlso Double.IsNaN(b)) OrElse a = b) Then
                        columnMismatches += 1
                    End If
                Next

                Call DebugAudit.Test(columnMismatches = 0, $"GetSample should read the whole column ({columnMismatches} mismatch(es))")

                ' 按基因名批量取列值
                Dim genes As String() = {"gene_1", "gene_50", "gene_50", "missing_gene", "gene_211"}
                Dim picked As Double() = view.GetGeneExpression(genes, 3)

                Call DebugAudit.AreEqual(5, picked.Length, "GetGeneExpression length")
                Call DebugAudit.AreEqual(0.0, picked(3), "a missing gene should yield 0")
                Call DebugAudit.Test(
                    (Double.IsNaN(picked(0)) AndAlso Double.IsNaN(mat.expression(0).experiments(3))) OrElse picked(0) = mat.expression(0).experiments(3),
                    "the first picked gene should match"
                )

                ' 并发随机访问：共享同一个读取器实例
                Dim concurrentMismatches As Integer = 0
                Dim local As New ThreadLocal(Of Random)(Function() New Random(Thread.CurrentThread.ManagedThreadId))

                Try
                    Call System.Threading.Tasks.Parallel.For(
                        0,
                        600,
                        Sub(k)
                            Dim i As Integer = local.Value.Next(0, mat.size)
                            Dim j As Integer = local.Value.Next(0, mat.sample_count)
                            Dim row As DataFrameRow = view.GetRow(i)
                            Dim a As Double = row.experiments(j)
                            Dim b As Double = mat.expression(i).experiments(j)

                            If Not ((Double.IsNaN(a) AndAlso Double.IsNaN(b)) OrElse a = b) Then
                                Call Interlocked.Increment(concurrentMismatches)
                            End If
                        End Sub
                    )
                Finally
                    Call local.Dispose()
                End Try

                Call DebugAudit.Test(concurrentMismatches = 0, $"concurrent GetRow should be thread safe ({concurrentMismatches} mismatch(es))")
            End Using
        End Sub)
    End Sub

    Private Sub testNonSeekableStream(folder As String)
        Call runCase("non seekable stream", Sub()
            Dim mat As Matrix = createMatrix(83, 11, seed:=6)
            Dim file As String = IO.Path.Combine(folder, "non_seekable.bin")

            Call mat.SaveFile(file)

            Dim raw As Byte() = IO.File.ReadAllBytes(file)

            Using ms As New MemoryStream(raw)
                Using wrapper As New NonSeekableStream(ms)
                    Dim loaded As Matrix = BinaryMatrix.LoadStream(wrapper)
                    Dim detail As String = ""
                    Dim mismatches As Integer = compareValues(New HTSMatrixViewer(loaded), mat, detail)

                    Call DebugAudit.Test(mismatches = 0, $"a non seekable stream should be read sequentially ({mismatches} mismatch(es): {detail})")
                End Using
            End Using

            Using ms As New MemoryStream(raw)
                Using wrapper As New NonSeekableStream(ms)
                    Using view As New HTSMatrixReader(wrapper)
                        Call DebugAudit.Test(Not view.CanRandomAccess, "a non seekable stream should advertise that random access is unavailable")

                        Dim failed As Boolean = False

                        Try
                            Call view.GetRow(0)
                        Catch ex As NotSupportedException
                            failed = True
                        End Try

                        Call DebugAudit.Test(failed, "GetRow should raise NotSupportedException on a non seekable stream")
                    End Using
                End Using
            End Using
        End Sub)
    End Sub

    Private Sub testTruncatedData(folder As String)
        Call runCase("truncated data detection", Sub()
            Dim mat As Matrix = createMatrix(40, 8, seed:=7)
            Dim file As String = IO.Path.Combine(folder, "truncated_source.bin")
            Dim broken As String = IO.Path.Combine(folder, "truncated.bin")

            Call mat.SaveFile(file)

            Dim raw As Byte() = IO.File.ReadAllBytes(file)
            Dim cut As Byte() = New Byte(raw.Length - 65) {}

            Call Array.ConstrainedCopy(raw, 0, cut, 0, cut.Length)
            Call IO.File.WriteAllBytes(broken, cut)

            Dim detected As Boolean = False

            Try
                Using view As HTSMatrixReader = Matrix.LoadLazy(broken)
                    Dim rows As DataFrameRow() = view.Rows().ToArray
                End Using
            Catch ex As InvalidDataException
                detected = True
            End Try

            Call DebugAudit.Test(detected, "a truncated data area should raise InvalidDataException")
        End Sub)
    End Sub

    Private Sub testInvalidMagic(folder As String)
        Call runCase("invalid magic detection", Sub()
            Dim broken As String = IO.Path.Combine(folder, "bad_magic.bin")

            Call IO.File.WriteAllBytes(broken, Encoding.ASCII.GetBytes(New String("X"c, 128)))

            Dim detected As Boolean = False

            Try
                Using view As HTSMatrixReader = Matrix.LoadLazy(broken)
                    Dim n As Integer = view.Size.nfeature
                End Using
            Catch ex As InvalidDataException
                detected = True
            End Try

            Call DebugAudit.Test(detected, "an unknown magic header should raise InvalidDataException")
        End Sub)
    End Sub

    Private Sub testViewerParity(folder As String)
        Call runCase("memory viewer parity", Sub()
            Dim mat As Matrix = createMatrix(53, 12, seed:=8)
            Dim file As String = IO.Path.Combine(folder, "parity.bin")

            Call mat.SaveFile(file)

            Dim memory As New HTSMatrixViewer(mat)

            Using disk As HTSMatrixReader = Matrix.LoadLazy(file)
                    Call DebugAudit.AreEqual(memory.Size, disk.Size, "both viewers should report the same shape")
                    Call DebugAudit.AreEqual(memory.tag, disk.tag, "both viewers should report the same tag")
                    Call DebugAudit.AreEqual(memory.GetSampleOrdinal("sample_4"), disk.GetSampleOrdinal("sample_4"), "GetSampleOrdinal")

                    Dim memColumn As Double() = memory.GetSample(7)
                    Dim diskColumn As Double() = disk.GetSample(7)

                    Call DebugAudit.AreEqual(memColumn.Length, diskColumn.Length, "GetSample length should agree")

                    Dim mismatches As Integer = 0

                    For i As Integer = 0 To memColumn.Length - 1
                        Dim a As Double = memColumn(i)
                        Dim b As Double = diskColumn(i)

                        If Not ((Double.IsNaN(a) AndAlso Double.IsNaN(b)) OrElse a = b) Then
                            mismatches += 1
                        End If
                    Next

                    Call DebugAudit.Test(mismatches = 0, $"the memory viewer and the disk reader should return identical columns ({mismatches} mismatch(es))")

                    Dim memRow As DataFrameRow = memory.GetRow(20)
                    Dim diskRow As DataFrameRow = disk.GetRow(20)

                    Call DebugAudit.AreEqual(memRow.geneID, diskRow.geneID, "GetRow name")

                    Dim memGene As Double() = memory.GetGeneExpression("gene_33")
                    Dim diskGene As Double() = disk.GetGeneExpression("gene_33")

                    Call DebugAudit.Test(memGene.SequenceEqual(diskGene), "GetGeneExpression should be identical")
            End Using
        End Sub)
    End Sub

    Private Sub testCsvRoundTrip(folder As String)
        Call runCase("csv to binary to csv", Sub()
            Dim mat As Matrix = createMatrix(72, 10, seed:=9, withNaN:=False)
            Dim csv1 As String = IO.Path.Combine(folder, "source.csv")
            Dim bin As String = IO.Path.Combine(folder, "converted.bin")
            Dim csv2 As String = IO.Path.Combine(folder, "converted.csv")

            Call mat.SaveMatrix(csv1)

            Call DebugAudit.Test(csv1.ToBinary(bin), "csv -> binary conversion should succeed")
            Call DebugAudit.Test(bin.ToCsv(csv2), "binary -> csv conversion should succeed")

            Dim origin As Matrix = Document.LoadMatrixDocument(csv1, DirectCast(Nothing, Index(Of String)))
            Dim round As Matrix = Document.LoadMatrixDocument(csv2, DirectCast(Nothing, Index(Of String)))

            Call DebugAudit.AreEqual(origin.size, round.size, "converted row count")
            Call DebugAudit.AreEqual(origin.sample_count, round.sample_count, "converted column count")

            Dim mismatches As Integer = 0
            Dim detail As String = ""

            For i As Integer = 0 To origin.size - 1
                If origin.expression(i).geneID <> round.expression(i).geneID Then
                    mismatches += 1
                End If

                For j As Integer = 0 To origin.sample_count - 1
                    Dim a As Double = origin.expression(i).experiments(j)
                    Dim b As Double = round.expression(i).experiments(j)

                    If Not ((Double.IsNaN(a) AndAlso Double.IsNaN(b)) OrElse a = b) Then
                        mismatches += 1

                        If detail.Length < 200 Then
                            detail &= $"row {i} col {j}: {a:R} <> {b:R}; "
                        End If
                    End If
                Next
            Next

            Call DebugAudit.Test(mismatches = 0, $"csv -> bin -> csv should be value preserving ({mismatches} mismatch(es): {detail})")

            ' 惰性行枚举的行数应当与整表读取一致
            Dim streamed As Integer = csv1.StreamRows().Count()

            Call DebugAudit.AreEqual(origin.size, streamed, "StreamRows row count")
        End Sub)
    End Sub

    Private Sub testRollback(folder As String)
        Call runCase("unfinished writer rollback", Sub()
            Dim mat As Matrix = createMatrix(20, 5, seed:=10)
            Dim target As String = IO.Path.Combine(folder, "rollback.bin")

            Call IO.File.WriteAllText(target, "existing-content")

            Using writer As HTSMatrixWriter = HTSMatrixWriter.CreateFile(target, mat.sampleID)
                Call writer.WriteRow(mat.expression(0))
                ' 故意不调用 Complete()：Dispose 应当回滚
            End Using

            Call DebugAudit.Test(
                IO.File.Exists(target) AndAlso IO.File.ReadAllText(target) = "existing-content",
                "an unfinished writer must not replace the existing target file"
            )

            ' 正常完成的写入应当替换目标文件
            Using writer As HTSMatrixWriter = HTSMatrixWriter.CreateFile(target, mat.sampleID)
                Call writer.WriteRows(mat.expression)
                Call writer.Complete()

                Call DebugAudit.Test(writer.IsCompleted, "the writer should report completion")
            End Using

            Using view As HTSMatrixReader = Matrix.LoadLazy(target)
                Call DebugAudit.AreEqual(mat.size, view.Size.nfeature, "the completed file should contain every row")
            End Using
        End Sub)
    End Sub

    ''' <summary>
    ''' 大矩阵（数据区超过单次读取字节预算）的顺序扫描：确保按字节预算分块之后
    ''' <see cref="HTSMatrixReader.Rows"/> 与 <see cref="HTSMatrixReader.GetSample"/> 仍然正确。
    ''' </summary>
    Private Sub testChunkedScan(folder As String)
        Call runCase("chunked sequential scan", Sub()
            ' 2600 x 220 x 8B ≈ 4.6 MB，超过 MaxReadBytes(4MB)，因此会跨块读取
            Dim mat As Matrix = createMatrix(2600, 220, seed:=11, withNaN:=False)
            Dim file As String = IO.Path.Combine(folder, "chunked.bin")

            Call mat.SaveFile(file)

            Using view As HTSMatrixReader = Matrix.LoadLazy(file)
                Dim rows As DataFrameRow() = view.Rows().ToArray()

                Call DebugAudit.AreEqual(mat.size, rows.Length, "sequential scan row count")

                Dim mismatches As Integer = 0
                Dim detail As String = ""

                For i As Integer = 0 To rows.Length - 1
                    For j As Integer = 0 To mat.sample_count - 1
                        If rows(i).experiments(j) <> mat.expression(i).experiments(j) Then
                            mismatches += 1

                            If detail.Length < 160 Then
                                detail &= $"row {i} col {j}; "
                            End If
                        End If
                    Next
                Next

                Call DebugAudit.Test(mismatches = 0, $"a chunked sequential scan should reproduce every value ({mismatches} mismatch(es): {detail})")

                Dim lastColumn As Double() = view.GetSample(mat.sample_count - 1)
                Dim columnMismatches As Integer = 0

                For i As Integer = 0 To lastColumn.Length - 1
                    If lastColumn(i) <> mat.expression(i).experiments(mat.sample_count - 1) Then
                        columnMismatches += 1
                    End If
                Next

                Call DebugAudit.Test(columnMismatches = 0, $"a chunked column scan should reproduce every value ({columnMismatches} mismatch(es))")
                Call DebugAudit.AreEqual(mat.sample_count, view.Size.nsample, "the column count should survive the round trip")
            End Using
        End Sub)
    End Sub

    ''' <summary>
    ''' 把一个可 seek 的流包装成"只进流"，用于模拟 zip 条目的解压流
    ''' </summary>
    Private Class NonSeekableStream : Inherits Stream

        Private ReadOnly inner As Stream

        Public Sub New(inner As Stream)
            Me.inner = inner
        End Sub

        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property Length As Long
            Get
                Throw New NotSupportedException()
            End Get
        End Property

        Public Overrides Property Position As Long
            Get
                Throw New NotSupportedException()
            End Get
            Set(value As Long)
                Throw New NotSupportedException()
            End Set
        End Property

        Public Overrides Sub Flush()
            Call inner.Flush()
        End Sub

        Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
            Return inner.Read(buffer, offset, count)
        End Function

        Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
            Throw New NotSupportedException()
        End Function

        Public Overrides Sub SetLength(value As Long)
            Throw New NotSupportedException()
        End Sub

        Public Overrides Sub Write(buffer As Byte(), offset As Integer, count As Integer)
            Throw New NotSupportedException()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                Call inner.Dispose()
            End If
        End Sub
    End Class

End Module
