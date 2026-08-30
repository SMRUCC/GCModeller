' ============================================================================
' ProteinFamilyBuilder.vb — 蛋白质家族构建主流程
' ----------------------------------------------------------------------------
''' 编排完整的蛋白质序列聚类 pipeline，分四个阶段：
'
' ┌──────────────────────────────────────────────────────────────────────┐
' │ Phase 1: 索引 + 建库                                                │
' │   流式读取原始 FASTA → 分配整数 ID → 写入重格式化 FASTA             │
' │   → 调用 diamond makedb 构建 DIAMOND 数据库                        │
' │   → 删除重格式化 FASTA（释放磁盘）                                 │
' ├──────────────────────────────────────────────────────────────────────┤
' │ Phase 2: 初始化并查集                                               │
' │   创建 N × 4 字节的内存映射文件，初始化为 -1                        │
' ├──────────────────────────────────────────────────────────────────────┤
' │ Phase 3: 分块比对 + 流式聚类                                       │
' │   再次流式读取 FASTA → 写入 chunk_NNNN.fasta（含 chunk_size 条）   │
' │   → diamond blastp 比对 → 流式解析 TSV → DSU.Union                 │
' │   → 删除 chunk 临时文件 → 读取下一个 chunk                          │
' │   循环直到所有序列处理完毕                                          │
' ├──────────────────────────────────────────────────────────────────────┤
' │ Phase 4: 输出蛋白质家族                                             │
' │   第三次流式读取 FASTA → 按位置分配 ID → DSU.Find → 家族 ID        │
' │   → 写入 families.tsv（序列头 → 家族 ID）                          │
' │   → 写入 family_summary.tsv（家族 ID → 成员数）                     │
' └──────────────────────────────────────────────────────────────────────┘
'
' 内存使用（16GB 物理内存）：
'   - DSU 内存映射文件：OS 按需分页，活跃集 ~1-2GB
'   - 当前 chunk：chunk_size × 平均序列长（~150MB）
'   - DIAMOND 进程：~2-4GB（block-size=0.5）
'   - 程序本身：~500MB
'   - 合计：~4-7GB，16GB 充裕
'
' 磁盘使用（100GB 输入 FASTA）：
'   - 原始 FASTA：100GB（只读）
'   - 重格式化 FASTA：~100GB（临时，Phase 1 后删除）
'   - DIAMOND DB：~50GB（Phase 1-3 期间）
'   - DSU 文件：~8GB（1B 序列 × 4 字节）
'   - chunk 临时文件：~1-5GB/chunk（用完即删）
'   - 输出 TSV：~10-50GB（取决于序列数和头长度）
'   - 峰值：~270GB（建议预留 300GB 磁盘空间）
' ============================================================================

Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Core

    ''' <summary>流程配置参数</summary>
    Public Class PipelineConfig

        ''' <summary>输入 FASTA 文件路径</summary>
        Public Property InputFasta As String

        ''' <summary>输出 families TSV 文件路径</summary>
        Public Property OutputFamilies As String

        ''' <summary>输出 family summary TSV 文件路径</summary>
        Public Property OutputSummary As String

        ''' <summary>最小序列相似性百分比（默认 90.0）</summary>
        Public Property MinIdentity As Double = 90.0

        ''' <summary>最小覆盖度百分比（默认 80.0）</summary>
        Public Property MinCoverage As Double = 80.0

        ''' <summary>每个 chunk 的序列数（默认 500000）</summary>
        Public Property ChunkSize As Integer = 500000

        ''' <summary>DIAMOND 可执行文件路径</summary>
        Public Property DiamondPath As String = "diamond"

        ''' <summary>工作目录（存储中间文件）</summary>
        Public Property WorkDir As String

        ''' <summary>DIAMOND 线程数</summary>
        Public Property Threads As Integer = 4

        ''' <summary>DIAMOND block-size 参数</summary>
        Public Property BlockSize As Double = 0.5

        ''' <summary>是否在完成后删除中间文件</summary>
        Public Property Cleanup As Boolean = True

        ''' <summary>进度报告间隔（序列数）</summary>
        Public Property ProgressInterval As Long = 1000000L
    End Class

    ''' <summary>
    ''' 蛋白质家族构建器：编排完整聚类 pipeline。
    ''' </summary>
    Public Class ProteinFamilyBuilder

        Private ReadOnly _config As PipelineConfig
        Private ReadOnly _diamond As DiamondRunner
        Private ReadOnly _sw As New Stopwatch()

        ' 中间文件路径
        Private ReadOnly _reformattedFasta As String
        Private ReadOnly _diamondDb As String
        Private ReadOnly _dsuPath As String
        Private ReadOnly _tmpDir As String
        Private ReadOnly _progressFile As String

        ''' <summary>总序列数</summary>
        Private _totalSequences As Long = 0

        ''' <summary>
        ''' 构造函数
        ''' </summary>
        Public Sub New(config As PipelineConfig)
            _config = config

            ' 确保工作目录存在
            If Not Directory.Exists(config.WorkDir) Then
                Directory.CreateDirectory(config.WorkDir)
            End If

            _tmpDir = Path.Combine(config.WorkDir, "diamond_tmp")
            _diamond = New DiamondRunner(config.DiamondPath, config.Threads, config.BlockSize, _tmpDir)

            _reformattedFasta = Path.Combine(config.WorkDir, "reformatted.fasta")
            _diamondDb = Path.Combine(config.WorkDir, "protein_db")
            _dsuPath = Path.Combine(config.WorkDir, "dsu.bin")
            _progressFile = Path.Combine(config.WorkDir, "progress.txt")
        End Sub

        ''' <summary>
        ''' 运行完整 pipeline
        ''' </summary>
        Public Sub Run()
            _sw.Start()

            ' 检查 diamond 可用性
            Log("检查 DIAMOND 可用性...")
            If Not _diamond.CheckAvailable() Then
                Throw New Exception(
                    $"无法运行 DIAMOND（路径: {_config.DiamondPath}）。" &
                    "请确认 diamond 已安装并在 PATH 中，或通过 --diamond-path 指定完整路径。")
            End If
            Log("DIAMOND 可用。")

            ' 检查断点续传
            Dim lastPhase = CheckResumePoint()

            If lastPhase < 1 Then
                ' ---- Phase 1: 索引 + 建库 ----
                Log("========== Phase 1: 索引 + 建库 ==========")
                _totalSequences = BuildIndexAndDatabase()
                SaveResumePoint(1, _totalSequences)
            Else
                Log("跳过 Phase 1（已从断点恢复）")
                _totalSequences = ReadResumeTotalSequences()
            End If

            If _totalSequences > Integer.MaxValue Then
                Throw New Exception(
                    $"序列总数 {_totalSequences:N0} 超过 int.MaxValue ({Integer.MaxValue:N0})。" &
                    "请将输入文件拆分后分别处理。")
            End If
            Log($"总序列数: {_totalSequences:N0}")

            If lastPhase < 2 Then
                ' ---- Phase 2: 初始化并查集 ----
                Log("========== Phase 2: 初始化并查集 ==========")
                Log("    创建内存映射 DSU 文件...")
                ' DSU 在 Phase 3 使用，此处只是告知创建完成
                SaveResumePoint(2, _totalSequences)
            Else
                Log("跳过 Phase 2（已从断点恢复）")
            End If

            If lastPhase < 3 Then
                ' ---- Phase 3: 分块比对 + 聚类 ----
                Log("========== Phase 3: 分块比对 + 流式聚类 ==========")
                RunChunkedClustering(CInt(_totalSequences))
                SaveResumePoint(3, _totalSequences)
            Else
                Log("跳过 Phase 3（已从断点恢复）")
            End If

            ' ---- Phase 4: 输出蛋白质家族 ----
            Log("========== Phase 4: 输出蛋白质家族 ==========")
            WriteFamilies(CInt(_totalSequences))

            ' 清理
            If _config.Cleanup Then
                Log("清理中间文件...")
                CleanupIntermediateFiles()
            End If

            _sw.Stop()
            Log($"========== 全部完成！总耗时: {_sw.Elapsed.TotalHours:F1} 小时 ==========")
        End Sub

        ''' <summary>
        ''' Phase 1: 流式读取 FASTA，分配整数 ID，写入重格式化 FASTA，构建 DIAMOND 数据库
        ''' </summary>
        Private Function BuildIndexAndDatabase() As Long
            Log($"    读取输入 FASTA: {_config.InputFasta}")
            Log($"    写入重格式化 FASTA: {_reformattedFasta}")

            Dim count As Long = 0
            Using reader As New StreamIterator(_config.InputFasta),
                  writer As New System.IO.StreamWriter(_reformattedFasta, False, Encoding.ASCII, bufferSize:=1 << 20)

                Dim record As FastaSeq = reader.ReadNext()

                Do While record IsNot Nothing
                    ' 写入重格式化 FASTA：>整数ID\n序列\n
                    writer.Write(">"c)
                    writer.Write(count)
                    writer.Write(ControlChars.Lf)
                    writer.Write(record.SequenceData)
                    writer.Write(ControlChars.Lf)

                    count += 1

                    ' 进度报告
                    If count Mod _config.ProgressInterval = 0 Then
                        Console.Error.Write(
                            $"    已索引 {count:N0} 序列 ({reader.Progress:F1}%)" & ControlChars.Cr)
                    End If

                    record = reader.ReadNext()
                Loop
            End Using
            Console.Error.WriteLine()

            Log($"    索引完成: {count:N0} 序列")
            Log($"    文件大小: {New FileInfo(_reformattedFasta).Length \ (1L << 30):F1} GB")

            ' 构建 DIAMOND 数据库
            Log("    构建 DIAMOND 数据库...")
            Dim result = _diamond.MakeDatabase(_reformattedFasta, _diamondDb)
            If Not result.Success Then
                Throw New Exception($"DIAMOND makedb 失败 (exit={result.ExitCode}): {result.StdErr}")
            End If
            Log($"    DIAMOND 数据库构建完成 (耗时 {result.ElapsedSeconds / 60:F1} 分钟)")

            ' 删除重格式化 FASTA 释放磁盘
            Log("    删除重格式化 FASTA（释放磁盘空间）...")
            File.Delete(_reformattedFasta)

            Return count
        End Function

        ''' <summary>
        ''' Phase 3: 分块比对 + 流式聚类
        ''' </summary>
        Private Sub RunChunkedClustering(totalSeqs As Integer)
            Using dsu As New UnionFind(_dsuPath, totalSeqs),
                  engine As New ClusteringEngine(dsu, _config.MinIdentity, _config.MinCoverage)

                Dim chunkSize = _config.ChunkSize
                Dim totalChunks = CInt(Math.Ceiling(CDbl(totalSeqs) / chunkSize))
                Log($"    chunk 大小: {chunkSize:N0} 序列")
                Log($"    预计 chunk 数: {totalChunks:N0}")
                Log($"    最小相似性: {_config.MinIdentity:F1}%")
                Log($"    最小覆盖度: {_config.MinCoverage:F1}%")
                Log("")

                ' 流式读取原始 FASTA，写 chunk 文件
                Using reader As New StreamIterator(_config.InputFasta)

                    Dim seqIdx As Integer = 0
                    Dim chunkNum As Integer = 0

                    Do While seqIdx < totalSeqs
                        Dim currentChunkSize = Math.Min(chunkSize, totalSeqs - seqIdx)
                        Dim chunkFasta = Path.Combine(_config.WorkDir, $"chunk_{chunkNum:D6}.fasta")
                        Dim chunkTsv = Path.Combine(_config.WorkDir, $"chunk_{chunkNum:D6}.tsv")

                        ' ---- 写 chunk FASTA ----
                        Log($"--- Chunk {chunkNum + 1}/{totalChunks} (序列 {seqIdx:N0}-{seqIdx + currentChunkSize - 1:N0}) ---")

                        Using writer As New System.IO.StreamWriter(chunkFasta, False, Encoding.ASCII, bufferSize:=1 << 20)
                            For i = 0 To currentChunkSize - 1
                                Dim record = reader.ReadNext()
                                If record Is Nothing Then Exit For

                                writer.Write(">"c)
                                writer.Write(seqIdx)
                                writer.Write(ControlChars.Lf)
                                writer.Write(record.SequenceData)
                                writer.Write(ControlChars.Lf)
                                seqIdx += 1
                            Next
                        End Using

                        ' ---- 运行 DIAMOND blastp ----
                        Log($"    运行 DIAMOND blastp...")
                        Dim blastResult = _diamond.BlastP(
                            chunkFasta, _diamondDb, chunkTsv,
                            _config.MinIdentity, _config.MinCoverage)

                        If Not blastResult.Success Then
                            Log($"    [警告] DIAMOND blastp 失败 (exit={blastResult.ExitCode}): {blastResult.StdErr}")
                            Log("    跳过此 chunk，继续下一个...")
                            ' 清理并继续
                            SafeDelete(chunkFasta)
                            SafeDelete(chunkTsv)
                            chunkNum += 1
                            Continue Do
                        End If

                        Log($"    DIAMOND 完成 ({blastResult.ElapsedSeconds / 60:F1} 分钟)")

                        ' ---- 流式解析比对结果 + DSU 聚类 ----
                        Dim tsvSize = If(File.Exists(chunkTsv), New FileInfo(chunkTsv).Length, 0)
                        If tsvSize > 0 Then
                            Using parser As New DiamondResultParser(
                                chunkTsv, _config.MinIdentity, _config.MinCoverage)

                                Dim processed = engine.ProcessAll(parser)
                                engine.MarkChunkComplete()

                                Log($"    解析 {parser.TotalLines:N0} 行比对，通过 {parser.PassedLines:N0} 行")
                            End Using
                        Else
                            Log("    无比对结果（所有序列为单例）")
                            engine.MarkChunkComplete()
                        End If

                        Log($"    累计: {engine.Stats}")

                        ' ---- 删除 chunk 临时文件 ----
                        SafeDelete(chunkFasta)
                        SafeDelete(chunkTsv)

                        ' 删除 DIAMOND 临时文件
                        CleanDiamondTmp()

                        ' 保存进度
                        SaveResumePoint(3, totalSeqs, chunkNum + 1)

                        chunkNum += 1
                    Loop

                    ' 清理 DIAMOND 临时目录
                    CleanDiamondTmp()

                    Log("")
                    Log($"Phase 3 完成！")
                    Log($"    总比对: {engine.Stats.TotalAlignments:N0}")
                    Log($"    通过阈值: {engine.Stats.PassedAlignments:N0}")
                    Log($"    Union 操作: {engine.Stats.UnionsPerformed:N0}")
                    Log($"    DSU Find 操作: {dsu.FindCount:N0}")
                    Log($"    DSU 缓存命中率: {dsu.CacheHitRate:P1}")
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Phase 4: 输出蛋白质家族
        ''' 流式读取原始 FASTA，按位置分配 ID，查找 DSU 根 → 家族 ID
        ''' </summary>
        Private Sub WriteFamilies(totalSeqs As Integer)
            Log("    分配家族 ID 并写入输出文件...")

            ' ---- 第一遍：收集所有根 → 分配顺序家族 ID ----
            Log("    [4a] 扫描根节点，分配顺序家族 ID...")
            Dim rootToFamily As New Dictionary(Of Integer, Integer)(capacity:=totalSeqs \ 10 + 1)
            Dim familySizes As New Dictionary(Of Integer, Integer)(capacity:=totalSeqs \ 10 + 1)
            Dim nextFamilyId As Integer = 0

            Using dsu As New UnionFind(_dsuPath, totalSeqs)
                ' 逐个查找根（不读取 FASTA，直接遍历 DSU）
                ' 注意：对于十亿级序列，这会比较慢，但每个 Find 是 O(α) 均摊
                For i = 0 To totalSeqs - 1
                    Dim root = dsu.Find(i)
                    If Not rootToFamily.ContainsKey(root) Then
                        rootToFamily(root) = nextFamilyId
                        familySizes(nextFamilyId) = 0
                        nextFamilyId += 1
                    End If
                    familySizes(rootToFamily(root)) += 1

                    If (i + 1) Mod _config.ProgressInterval = 0 Then
                        Console.Error.Write(
                            $"    已处理 {i + 1:N0}/{totalSeqs:N0} 序列，" &
                            $"{rootToFamily.Count:N0} 个家族" & ControlChars.Cr)
                    End If
                Next
                Console.Error.WriteLine()
            End Using

            Log($"    家族总数: {rootToFamily.Count:N0}")
            Log($"    单例家族: {familySizes.Values.Where(Function(s) s = 1).Count:N0}")

            ' ---- 第二遍：重新打开 DSU，写入 families.tsv ----
            Log("    [4b] 写入 families.tsv...")
            WriteFamiliesWithDSU(totalSeqs, rootToFamily, familySizes)

            ' ---- 写入 family_summary.tsv ----
            Log("    [4c] 写入 family_summary.tsv...")
            Using writer As New System.IO.StreamWriter(_config.OutputSummary, False, Encoding.ASCII, bufferSize:=1 << 20)
                writer.WriteLine("#family_id" & ControlChars.Tab & "size")
                For Each kv In familySizes.OrderByDescending(Function(k) k.Value)
                    writer.WriteLine($"{kv.Key}{ControlChars.Tab}{kv.Value}")
                Next
            End Using

            Log($"    输出完成: {_config.OutputFamilies}")
            Log($"    家族摘要: {_config.OutputSummary}")
        End Sub

        ''' <summary>
        ''' 使用 DSU 写入 families.tsv
        ''' </summary>
        Private Sub WriteFamiliesWithDSU(totalSeqs As Integer,
                                          rootToFamily As Dictionary(Of Integer, Integer),
                                          familySizes As Dictionary(Of Integer, Integer))
            Using dsu As New UnionFind(_dsuPath, totalSeqs),
                  reader As New StreamIterator(_config.InputFasta),
                  writer As New System.IO.StreamWriter(_config.OutputFamilies, False, Encoding.ASCII, bufferSize:=1 << 20)

                writer.WriteLine("#sequence_header" & ControlChars.Tab & "family_id")

                Dim seqIdx As Integer = 0
                Dim record = reader.ReadNext()
                Do While record IsNot Nothing
                    Dim root = dsu.Find(seqIdx)
                    Dim familyId As Integer = 0
                    If rootToFamily.ContainsKey(root) Then
                        familyId = rootToFamily(root)
                    Else
                        ' 理论上不应发生（所有根已在第一遍收集）
                        familyId = -1
                    End If

                    ' 写入原始头 → 家族 ID
                    writer.Write(record.Title)
                    writer.Write(ControlChars.Tab)
                    writer.WriteLine(familyId)

                    seqIdx += 1

                    If seqIdx Mod _config.ProgressInterval = 0 Then
                        Console.Error.Write(
                            $"    已写入 {seqIdx:N0}/{totalSeqs:N0} 序列" & ControlChars.Cr)
                    End If

                    record = reader.ReadNext()
                Loop
                Console.Error.WriteLine()
            End Using
        End Sub

        ' ---- 辅助方法 ----

        Private Sub Log(message As String)
            Dim ts = DateTime.Now.ToString("HH:mm:ss")
            Dim elapsed = _sw.Elapsed.TotalHours
            Console.Error.WriteLine($"[{ts}] [+{elapsed:F1}h] {message}")
        End Sub

        Private Sub SafeDelete(path As String)
            Try
                If File.Exists(path) Then File.Delete(path)
            Catch ex As Exception
                Log($"    [警告] 无法删除 {path}: {ex.Message}")
            End Try
        End Sub

        Private Sub CleanDiamondTmp()
            Try
                If Directory.Exists(_tmpDir) Then
                    For Each f In Directory.GetFiles(_tmpDir)
                        File.Delete(f)
                    Next
                End If
            Catch
                ' 忽略临时文件清理失败
            End Try
        End Sub

        Private Sub CleanupIntermediateFiles()
            SafeDelete(_reformattedFasta)
            SafeDelete(_diamondDb & ".dmnd")
            SafeDelete(_dsuPath)
            SafeDelete(_progressFile)
            Try
                If Directory.Exists(_tmpDir) Then Directory.Delete(_tmpDir, recursive:=True)
            Catch
            End Try
        End Sub

        ' ---- 断点续传 ----

        Private Function CheckResumePoint() As Integer
            If Not File.Exists(_progressFile) Then Return 0
            Try
                Dim lines = File.ReadAllLines(_progressFile)
                For Each line In lines
                    Dim parts = line.Split("="c)
                    If parts.Length = 2 AndAlso parts(0) = "phase" Then
                        Return Integer.Parse(parts(1))
                    End If
                Next
            Catch
            End Try
            Return 0
        End Function

        Private Function ReadResumeTotalSequences() As Long
            Try
                Dim lines = File.ReadAllLines(_progressFile)
                For Each line In lines
                    Dim parts = line.Split("="c)
                    If parts.Length = 2 AndAlso parts(0) = "total_sequences" Then
                        Return Long.Parse(parts(1))
                    End If
                Next
            Catch
            End Try
            Return 0
        End Function

        Private Sub SaveResumePoint(phase As Integer, totalSequences As Long, Optional chunk As Integer = -1)
            Try
                Dim sb As New StringBuilder()
                sb.AppendLine($"phase={phase}")
                sb.AppendLine($"total_sequences={totalSequences}")
                If chunk >= 0 Then sb.AppendLine($"last_chunk={chunk}")
                sb.AppendLine($"timestamp={DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                File.WriteAllText(_progressFile, sb.ToString())
            Catch
                ' 忽略进度文件写入失败
            End Try
        End Sub

    End Class

End Namespace
