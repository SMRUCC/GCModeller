Imports System.Diagnostics
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Math.HashMaps.MinHash

''' <summary>
''' CD-HIT 的并行 LSH 扫描实现：构建序列之间的相似度邻接表
''' </summary>
''' <remarks>
''' 与共享的 <see cref="LSH.FindSimilarItems"/> 相比，这里的实现做了下面这些事情：
''' 
''' 1. 按照 LSH band 完全并行（原实现完全单线程）；
''' 2. 每一个 band 使用「键值数组 + 排序分组」来替代「桶字典 + 每个桶一个 List 对象」，
'''    在数百万条序列的数据集上面可以显著降低内存占用；
''' 3. 候选对去重使用分片哈希表，避免单点锁竞争；
''' 4. 邻接表按照序列下标分片写入：序列 x 的全部相似关系都只会落在 x 所在的分片里面，
'''    因此各个分片之间不会冲突，最后也不需要做内层字典的合并；
''' 5. 不再为序列下标建立额外的反查字典（SequenceItem.ID 就是数组下标）。
''' 
''' 注意：按照 band 独立分桶之后，不同 band 之间偶然发生的 32 位哈希碰撞不会再被合并到同一个桶里面，
''' 这与原来共享的 LSH 实现之间存在极其微小的行为差异。
''' </remarks>
Friend NotInheritable Class CDHitLSH

    ''' <summary>
    ''' 分片的数量，必须是2的幂
    ''' </summary>
    Friend Const Shards As Integer = 256

    Private Const ShardMask As Integer = Shards - 1

    Public Const DefaultNumBands As Integer = 20
    Public Const DefaultRowsPerBand As Integer = 5

    Private Sub New()
    End Sub

    ''' <summary>
    ''' 并行构建序列相似度邻接表
    ''' </summary>
    ''' <param name="minHash">序列集合的 min-hash 签名（下标即为序列的下标）</param>
    ''' <param name="jaccardThreshold">min-hash 相似度的阈值</param>
    ''' <param name="workers">并行的工作线程数量</param>
    ''' <param name="Num_Bands">LSH波段数</param>
    ''' <param name="Rows_Per_Band">每个波段的行数</param>
    ''' <returns></returns>
    Public Shared Function BuildSimilarityGraph(minHash As SequenceItem(),
                                                jaccardThreshold As Double,
                                                workers As Integer,
                                                Optional Num_Bands As Integer = DefaultNumBands,
                                                Optional Rows_Per_Band As Integer = DefaultRowsPerBand) As CDHitSimilarityGraph

        Dim n As Integer = minHash.Length
        Dim sw As Stopwatch = Stopwatch.StartNew

        Call validateSignatures(minHash, Num_Bands * Rows_Per_Band)

        Dim adjacency(Shards - 1) As Dictionary(Of Integer, Dictionary(Of Integer, Double))
        Dim adjacencyLocks(Shards - 1) As Object
        Dim seen(Shards - 1) As HashSet(Of Long)
        Dim seenLocks(Shards - 1) As Object

        For i As Integer = 0 To Shards - 1
            adjacency(i) = New Dictionary(Of Integer, Dictionary(Of Integer, Double))()
            adjacencyLocks(i) = New Object()
            seen(i) = New HashSet(Of Long)()
            seenLocks(i) = New Object()
        Next

        Dim candidates As Long = 0
        Dim matches As Long = 0
        Dim options As New ParallelOptions With {.MaxDegreeOfParallelism = If(workers < 1, 1, workers)}

        Call Parallel.For(
            fromInclusive:=0,
            toExclusive:=Num_Bands,
            parallelOptions:=options,
            body:=Sub(band As Integer)
                      Dim keys As UInteger() = New UInteger(n - 1) {}
                      Dim ids As Integer() = New Integer(n - 1) {}
                      Dim buffer As Byte() = New Byte(Rows_Per_Band * 4 - 1) {}
                      Dim offset As Integer = band * Rows_Per_Band
                      Dim localCandidates As Long = 0
                      Dim localMatches As Long = 0

                      ' 1. 计算当前波段上面每一条序列的桶编号
                      For i As Integer = 0 To n - 1
                          Dim signature As UInteger() = minHash(i).Signature

                          For r As Integer = 0 To Rows_Per_Band - 1
                              Dim val As UInteger = signature(offset + r)
                              Dim p As Integer = r * 4

                              ' 注意：这里使用掩码之后再做CByte转换，因为CByte在默认的整数溢出检查之下
                              ' 对于数值大于255的输入是会抛出异常的
                              buffer(p) = CByte(val And &HFFUI)
                              buffer(p + 1) = CByte((val >> 8) And &HFFUI)
                              buffer(p + 2) = CByte((val >> 16) And &HFFUI)
                              buffer(p + 3) = CByte(val >> 24)
                          Next

                          keys(i) = MurmurHash.MurmurHashCode3_x86_32(buffer, CUInt(band))
                          ids(i) = i
                      Next

                      ' 2. 排序之后，具有相同桶编号的序列在数组之上是连续分布的
                      Call Array.Sort(keys, ids)

                      Dim pos As Integer = 0

                      While pos < n
                          Dim [stop] As Integer = pos + 1

                          While [stop] < n AndAlso keys([stop]) = keys(pos)
                              [stop] += 1
                          End While

                          ' 3. 桶内序列两两组合
                          If [stop] - pos > 1 Then
                              For a As Integer = pos To [stop] - 2
                                  Dim u As Integer = ids(a)

                                  For b As Integer = a + 1 To [stop] - 1
                                      Dim v As Integer = ids(b)
                                      Dim lo As Integer
                                      Dim hi As Integer

                                      If u < v Then
                                          lo = u
                                          hi = v
                                      Else
                                          lo = v
                                          hi = u
                                      End If

                                      localCandidates += 1

                                      ' 同一个候选对可能出现在多个波段里面，这里做全局去重，
                                      ' 避免对同一个序列重复计算相似度
                                      If markCandidate(seen, seenLocks, lo, hi) Then
                                          Dim similarity As Double = CalculateSimilarity(minHash(lo).Signature, minHash(hi).Signature)

                                          If similarity >= jaccardThreshold Then
                                              localMatches += 1

                                              Call addEdge(adjacency, adjacencyLocks, lo, hi, similarity)
                                          End If
                                      End If
                                  Next
                              Next
                          End If

                          pos = [stop]
                      End While

                      Call Interlocked.Add(candidates, localCandidates)
                      Call Interlocked.Add(matches, localMatches)

                      Call $"[cdhit] LSH band {band}: candidates +{localCandidates}, matches +{localMatches}, elapsed {sw.ElapsedMilliseconds} ms".debug
                  End Sub)

        Call $"[cdhit] LSH scan done: {candidates} candidate pairs, {matches} similar pairs, elapsed {sw.ElapsedMilliseconds} ms".debug

        Return New CDHitSimilarityGraph(adjacency)
    End Function

    ''' <summary>
    ''' 计算两个 min-hash 签名之间的相似度
    ''' </summary>
    ''' <param name="sig1"></param>
    ''' <param name="sig2"></param>
    ''' <returns></returns>
    ''' <remarks>与 <c>LSH.CalculateSimilarity</c> 的算法完全一致</remarks>
    Private Shared Function CalculateSimilarity(sig1 As UInteger(), sig2 As UInteger()) As Double
        Dim matchesCount As Integer = 0

        For i As Integer = 0 To sig1.Length - 1
            If sig1(i) = sig2(i) Then
                matchesCount += 1
            End If
        Next

        Return matchesCount / sig1.Length
    End Function

    ''' <summary>
    ''' 标记一个候选对：返回True表示这个候选对之前没有出现过
    ''' </summary>
    Private Shared Function markCandidate(seen As HashSet(Of Long)(),
                                          locks As Object(),
                                          u As Integer,
                                          v As Integer) As Boolean

        Dim key As Long = (CLng(CUInt(u)) << 32) Or CLng(CUInt(v))
        Dim shard As Integer = CInt(key And CLng(ShardMask))
        Dim result As Boolean

        SyncLock locks(shard)
            result = seen(shard).Add(key)
        End SyncLock

        Return result
    End Function

    ''' <summary>
    ''' 写入一条无向边（两个方向都会写入邻接表）
    ''' </summary>
    ''' <remarks>
    ''' 序列 x 的邻接表只会存放在 x 所在的分片之中，所以这里需要同时锁定两个分片；
    ''' 为了不产生死锁，两个分片总是按照下标从小到大的顺序来加锁。
    ''' </remarks>
    Private Shared Sub addEdge(adjacency As Dictionary(Of Integer, Dictionary(Of Integer, Double))(),
                              locks As Object(),
                              lo As Integer,
                              hi As Integer,
                              similarity As Double)

        Dim sLo As Integer = lo And ShardMask
        Dim sHi As Integer = hi And ShardMask

        If sLo = sHi Then
            SyncLock locks(sLo)
                Call addDirected(adjacency(sLo), lo, hi, similarity)
                Call addDirected(adjacency(sLo), hi, lo, similarity)
            End SyncLock
        ElseIf sLo < sHi Then
            SyncLock locks(sLo)
                SyncLock locks(sHi)
                    Call addDirected(adjacency(sLo), lo, hi, similarity)
                    Call addDirected(adjacency(sHi), hi, lo, similarity)
                End SyncLock
            End SyncLock
        Else
            SyncLock locks(sHi)
                SyncLock locks(sLo)
                    Call addDirected(adjacency(sHi), hi, lo, similarity)
                    Call addDirected(adjacency(sLo), lo, hi, similarity)
                End SyncLock
            End SyncLock
        End If
    End Sub

    Private Shared Sub addDirected(shard As Dictionary(Of Integer, Dictionary(Of Integer, Double)),
                                   u As Integer,
                                   v As Integer,
                                   similarity As Double)

        Dim edges As Dictionary(Of Integer, Double) = Nothing

        If Not shard.TryGetValue(u, edges) Then
            edges = New Dictionary(Of Integer, Double)()
            Call shard.Add(u, edges)
        End If

        ' 跨波段重复写入同一个边是幂等的
        edges(v) = similarity
    End Sub

    ''' <summary>
    ''' 检查所有的签名数据都是完整的（长度足够、非空）
    ''' </summary>
    Private Shared Sub validateSignatures(minHash As SequenceItem(), signatureSize As Integer)
        For i As Integer = 0 To minHash.Length - 1
            Dim item As SequenceItem = minHash(i)

            If item Is Nothing OrElse item.Signature Is Nothing Then
                Throw New InvalidOperationException($"the min-hash signature of the sequence {i} is nothing!")
            ElseIf item.Signature.Length < signatureSize Then
                Throw New InvalidOperationException($"the min-hash signature of the sequence {i} is too short: {item.Signature.Length} < {signatureSize}!")
            End If
        Next
    End Sub
End Class

''' <summary>
''' 序列相似度邻接表（分片存储）
''' </summary>
''' <remarks>
''' 序列 x 的全部相似序列都存放在 <c>shards(x And (Shards - 1))(x)</c> 之中，
''' 因此各个分片之间相互独立，可以在并行计算之中直接写入而不需要做后续的合并。
''' </remarks>
Friend NotInheritable Class CDHitSimilarityGraph

    Friend ReadOnly shards As Dictionary(Of Integer, Dictionary(Of Integer, Double))()

    Friend Sub New(shards As Dictionary(Of Integer, Dictionary(Of Integer, Double))())
        Me.shards = shards
    End Sub

    ''' <summary>
    ''' 获取与序列 <paramref name="index"/> 相似的全部序列
    ''' </summary>
    ''' <param name="index"></param>
    ''' <returns>
    ''' Key为相似序列的下标，Value为相似度；如果没有找到任何相似的序列则返回Nothing
    ''' </returns>
    Public Function Neighbors(index As Integer) As Dictionary(Of Integer, Double)
        Dim shard As Dictionary(Of Integer, Dictionary(Of Integer, Double)) = shards(index And (CDHitLSH.Shards - 1))
        Dim edges As Dictionary(Of Integer, Double) = Nothing

        Call shard.TryGetValue(index, edges)

        Return edges
    End Function

    ''' <summary>
    ''' 已经建立了相似关系的序列的数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Size As Integer
        Get
            Dim n As Integer = 0

            For i As Integer = 0 To shards.Length - 1
                n += shards(i).Count
            Next

            Return n
        End Get
    End Property
End Class
