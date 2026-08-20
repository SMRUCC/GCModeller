' Linclust 阶段一 & 二:mN 行 k-mer 索引表、排序分桶、选中心
'
' 阶段一:对每条序列取哈希值最小的 m 个 k-mer,生成 16 字节记录行:
'   k-mer 索引(8 字节 Long) + 序列 ID(4 字节 Integer) + 序列长度(2 字节 UShort) + 位置(2 字节 UShort)
' 整个表共 mN 行,即 Linclust 的内存足迹。
'
' 阶段二:按 k-mer 索引排序,相同 k-mer 的行聚成"k-mer 组",每组选最长序列为中心。

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm

Namespace Linclust

    ''' <summary>
    ''' 16 字节 k-mer 记录行
    ''' </summary>
    Public Structure KmerEntry
        ''' <summary>k-mer 哈希索引(16 位哈希扩展到 8 字节存储)</summary>
        Public KmerIndex As Long
        ''' <summary>序列 ID(整数索引)</summary>
        Public SeqId As Integer
        ''' <summary>序列长度</summary>
        Public SeqLen As UShort
        ''' <summary>k-mer 在序列中的位置</summary>
        Public Position As UShort
    End Structure

    ''' <summary>
    ''' 一个 k-mer 组(共享同一 k-mer 的全部序列)
    ''' </summary>
    Public Class KmerGroup
        Public Property KmerIndex As Long
        Public Property Members As List(Of KmerEntry)
    End Class

    Public Class KmerTable

        ''' <summary>
        ''' 由 <see cref="RollingHash.GetMinHashes"/> 结果构造完整 mN 行表。
        ''' </summary>
        ''' <param name="encodedSeqs">缩减字母表编码后的序列数组(下标即 SeqId)</param>
        ''' <param name="k">k-mer 长度</param>
        ''' <param name="m">每序列保留的 k-mer 数</param>
        Public Shared Function Build(encodedSeqs As String(), k As Integer, m As Integer) As KmerEntry()
            Dim rows As New List(Of KmerEntry)

            For Each seqId As Integer In TqdmWrapper.Range(0, encodedSeqs.Length)
                Dim seq = encodedSeqs(seqId)

                If seq Is Nothing OrElse seq.Length < k Then
                    Continue For
                End If

                Dim hashes = RollingHash.GetMinHashes(seq, k, m)
                Dim seqLen = CUShort(Math.Min(seq.Length, UShort.MaxValue))

                For Each h As RollingHash.KmerHash In hashes
                    rows.Add(New KmerEntry With {
                        .KmerIndex = CLng(h.Hash),
                        .SeqId = seqId,
                        .SeqLen = seqLen,
                        .Position = CUShort(Math.Min(h.Position, UShort.MaxValue))
                    })
                Next
            Next

            Return rows.ToArray
        End Function

        ''' <summary>
        ''' 阶段二:按 k-mer 索引排序后分桶,每个 k-mer 组选最长的序列作为中心。
        ''' </summary>
        ''' <returns>每个 k-mer 对应的中心序列 ID</returns>
        Public Shared Function SelectCenters(rows As KmerEntry()) As Dictionary(Of Long, Integer)
            ' 按 k-mer 索引排序(稳定)
            Array.Sort(rows, Function(a, b)
                                 If a.KmerIndex <> b.KmerIndex Then
                                     Return a.KmerIndex.CompareTo(b.KmerIndex)
                                 End If
                                 ' 同 k-mer 内按长度降序,便于扫描时第一个即为最长
                                 Return b.SeqLen.CompareTo(a.SeqLen)
                             End Function)

            Dim centers As New Dictionary(Of Long, Integer)
            Dim tqdm As New ProgressBar
            Dim i As Integer = 0
            While i < rows.Length
                Dim kmer = rows(i).KmerIndex
                ' 同组第一条(已按长度降序)即最长序列 -> 中心
                centers(kmer) = rows(i).SeqId

                ' 跳过同 k-mer 的其余行
                i += 1
                While i < rows.Length AndAlso rows(i).KmerIndex = kmer
                    i += 1
                End While

                Call tqdm.Progress(i, rows.Length)
            End While

            Call tqdm.Finish()

            Return centers
        End Function

        ''' <summary>
        ''' 合并共享同一中心的 k-mer 组,得到以中心为键、成员列表为值的映射。
        ''' 即:所有把同一条序列选为"中心"的 k-mer 组,其成员被并入该中心的候选集。
        ''' </summary>
        Public Shared Function MergeByCenter(rows As KmerEntry(), centers As Dictionary(Of Long, Integer)) As Dictionary(Of Integer, HashSet(Of Integer))
            Dim byCenter As New Dictionary(Of Integer, HashSet(Of Integer))

            Dim i As Integer = 0
            While i < rows.Length
                Dim kmer = rows(i).KmerIndex
                Dim center = centers(kmer)

                If Not byCenter.ContainsKey(center) Then
                    byCenter(center) = New HashSet(Of Integer)
                    byCenter(center).Add(center)  ' 中心自身也在组内
                End If

                ' 把同 k-mer 组内所有成员加入该中心候选集
                While i < rows.Length AndAlso rows(i).KmerIndex = kmer
                    byCenter(center).Add(rows(i).SeqId)
                    i += 1
                End While
            End While

            Return byCenter
        End Function
    End Class
End Namespace
