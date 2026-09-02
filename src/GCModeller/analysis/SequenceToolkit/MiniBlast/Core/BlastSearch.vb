' ============================================================================
' BlastSearch.vb — 搜索编排（CLI 与自检共用的同一条代码路径）
' ----------------------------------------------------------------------------
' 职责：读 FASTA → 建库（编码 + DUST/SEG 掩码）→ 逐查询 RunQuery
'       → 组装 BlastReport（含 parameters 段的 λ/K/H 与数据库统计）。
'
' 原先这段编排写在 Program.RunSearch（Private）里，自检无法调用，
' 导致测试走的是另一条路径、测不到真实调用链。抽出后 CLI 与自检同源。
' ============================================================================

Imports MiniBlast.Model
Imports MiniBlast.Options
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Core

    Public Module BlastSearch

        Public Const VersionString As String = "1.0.0"

        ''' <summary>从文件路径执行一次完整搜索</summary>
        Public Function Run(queryPath As String, dbPath As String, opts As BlastOptions) As BlastReport
            Dim queries = FastaFile.Read(queryPath)
            Dim dbSeqs = FastaFile.Read(dbPath)
            If queries Is Nothing OrElse queries.Count = 0 Then
                Throw New ArgumentException($"查询文件为空或无法解析: {queryPath}")
            End If
            If dbSeqs Is Nothing OrElse dbSeqs.Count = 0 Then
                Throw New ArgumentException($"数据库文件为空或无法解析: {dbPath}")
            End If
            Return RunQueries(queries, dbSeqs, opts)
        End Function

        ''' <summary>对内存中已加载的序列执行一次完整搜索（供自检/库使用者构造用例）</summary>
        Public Function RunQueries(queries As IEnumerable(Of FastaSeq),
                                   dbSeqs As IEnumerable(Of FastaSeq),
                                   opts As BlastOptions) As BlastReport
            Dim qList = queries.ToList()
            Dim dbList = dbSeqs.ToList()
            If qList.Count = 0 Then Throw New ArgumentException("查询集为空")
            If dbList.Count = 0 Then Throw New ArgumentException("数据库为空")

            Dim dbp = BlastDb.BuildDatabase(dbList, opts)

            Dim qrs As New List(Of QueryResult)()
            For Each q As FastaSeq In qList
                qrs.Add(BlastEngine.RunQuery(q, dbp.Item1, dbp.Item2, opts))
            Next

            Return New BlastReport With {
                .Program = opts.Program,
                .Task = opts.Task,
                .Version = VersionString,
                .Parameters = BuildParameters(opts, dbp.Item2),
                .Queries = qrs
            }
        End Function

        ''' <summary>该打分系统对应的 Karlin-Altschul 参数（自检校验 E/BitScore 时用同一套）</summary>
        Public Function StatsFor(opts As BlastOptions) As KaParams
            If opts.Program = "blastn" Then
                Return KarlinAltschul.NtParams(opts.Reward, opts.Penalty)
            End If
            Return KarlinAltschul.ProteinParams(opts.Matrix)
        End Function

        ''' <summary>组装报告级 parameters 段（λ/K/H 与数据库统计）</summary>
        Public Function BuildParameters(opts As BlastOptions, dbStats As DbStatistics) As BlastParameters
            Dim ka = StatsFor(opts)

            Dim hist As SortedDictionary(Of Integer, Double)
            If opts.Program = "blastn" Then
                hist = KarlinAltschul.BuildNtHist(opts.Reward, opts.Penalty)
            Else
                hist = KarlinAltschul.BuildAaHist(New AaScorer(opts.Matrix))
            End If
            Dim hInfo = KarlinAltschul.SolveH(hist, ka.Lambda)

            Return New BlastParameters With {
                .WordSize = If(opts.Program = "blastn" AndAlso opts.Task = "dc-megablast", 11, opts.WordSize),
                .Matrix = If(opts.Program = "blastp", opts.Matrix, Nothing),
                .Reward = If(opts.Program = "blastn", opts.Reward, 0),
                .Penalty = If(opts.Program = "blastn", opts.Penalty, 0),
                .Threshold = If(opts.Program = "blastp", opts.Threshold, 0),
                .GapOpen = opts.GapOpen,
                .GapExtend = opts.GapExtend,
                .EvalueCutoff = opts.EvalueCutoff,
                .TwoHitWindow = opts.WindowTwoHit,
                .Dust = opts.Dust,
                .Seg = opts.Seg,
                .CompBasedStats = opts.CompBasedStats,
                .Lambda = Math.Round(ka.Lambda, 6),
                .K = Math.Round(ka.K, 6),
                .H = Math.Round(hInfo, 6),
                .DbSequences = dbStats.Sequences,
                .DbResidues = dbStats.Residues
            }
        End Function

    End Module

End Namespace
