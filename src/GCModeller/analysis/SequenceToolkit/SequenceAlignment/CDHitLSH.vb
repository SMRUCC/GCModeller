Imports System.Diagnostics
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Math.HashMaps.MinHash

''' <summary>
''' CD-HIT 的并行 LSH 分桶索引
''' </summary>
''' <remarks>
''' 这里不再像 <see cref="LSH.FindSimilarItems"/> 那样把全部的相似序列对都物化出来，
''' 而是只建立「波段 -> 桶索引」的数据结构，然后在贪婪聚类的时候按需查询：
''' 
''' + 原实现会生成 Σ C(桶大小, 2) 个相似序列对。在泛基因组这类高度冗余的数据集上面
'''   （同一个基因在上百个菌株之中都存在），相似对的数量可以达到数十亿条，
'''   需要消耗数十 GB 的内存并且产生大量的锁竞争；
''' + 而实际上贪婪聚类只会用到「代表序列的、且还没有被归簇的邻居」，
'''   所以完全可以推迟到聚类的时候再按需计算，中间不需要保存任何相似序列对。
''' 
''' 另外，分桶索引只占用 3 个与序列数量成正比的数组（每个波段一套），
''' 相比原来「桶字典 + 每个桶一个 List 对象」的内存占用要低很多。
''' </remarks>
Friend NotInheritable Class CDHitLSH

    Public Const DefaultNumBands As Integer = 20
    Public Const DefaultRowsPerBand As Integer = 5

    Private Sub New()
    End Sub

    ''' <summary>
    ''' 并行计算每一个 LSH 波段上面的分桶索引
    ''' </summary>
    ''' <param name="minHash">序列集合的 min-hash 签名（下标即为序列的下标）</param>
    ''' <param name="workers">并行的工作线程数量</param>
    ''' <param name="Num_Bands">LSH波段数</param>
    ''' <param name="Rows_Per_Band">每个波段的行数</param>
    ''' <returns></returns>
    Public Shared Function BuildBuckets(minHash As SequenceItem(),
                                        workers As Integer,
                                        Optional Num_Bands As Integer = DefaultNumBands,
                                        Optional Rows_Per_Band As Integer = DefaultRowsPerBand) As CDHitLSHBuckets

        Dim n As Integer = minHash.Length
        Dim sw As Stopwatch = Stopwatch.StartNew

        Call validateSignatures(minHash, Num_Bands * Rows_Per_Band)

        Dim bandKeys As UInteger()() = New UInteger(Num_Bands - 1)() {}
        Dim sequences As Integer()() = New Integer(Num_Bands - 1)() {}
        Dim positions As Integer()() = New Integer(Num_Bands - 1)() {}
        Dim options As New ParallelOptions With {.MaxDegreeOfParallelism = If(workers < 1, 1, workers)}

        Call Parallel.For(
            fromInclusive:=0,
            toExclusive:=Num_Bands,
            parallelOptions:=options,
            body:=Sub(band As Integer)
                      Dim keys As UInteger() = New UInteger(n - 1) {}
                      Dim ids As Integer() = New Integer(n - 1) {}
                      Dim pos As Integer() = New Integer(n - 1) {}
                      Dim buffer As Byte() = New Byte(Rows_Per_Band * 4 - 1) {}
                      Dim offset As Integer = band * Rows_Per_Band

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

                      ' 2. 排序之后具有相同桶编号的序列在数组之上是连续分布的，
                      '    这样子就可以直接通过二分/线性扫描来枚举一个桶里面的全部序列
                      Call Array.Sort(keys, ids)

                      ' 3. 记录每一条序列在排序之后的数组之中的位置
                      For p As Integer = 0 To n - 1
                          pos(ids(p)) = p
                      Next

                      bandKeys(band) = keys
                      sequences(band) = ids
                      positions(band) = pos

                      Call $"[cdhit] LSH band {band}: {n} sequences bucketed, elapsed {sw.ElapsedMilliseconds} ms".debug
                  End Sub)

        Call $"[cdhit] LSH bucketing done, elapsed {sw.ElapsedMilliseconds} ms".debug

        Return New CDHitLSHBuckets(bandKeys, sequences, positions)
    End Function

    ''' <summary>
    ''' 计算两个 min-hash 签名之间的相似度
    ''' </summary>
    ''' <param name="sig1"></param>
    ''' <param name="sig2"></param>
    ''' <returns></returns>
    ''' <remarks>与 <c>LSH.CalculateSimilarity</c> 的算法完全一致</remarks>
    Friend Shared Function SignatureSimilarity(sig1 As UInteger(), sig2 As UInteger()) As Double
        Dim matches As Integer = 0

        For i As Integer = 0 To sig1.Length - 1
            If sig1(i) = sig2(i) Then
                matches += 1
            End If
        Next

        Return matches / sig1.Length
    End Function

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
''' LSH 分桶索引：可以按照 (序列下标, 波段) 来查询该序列所属的那个桶里面的全部序列
''' </summary>
''' <remarks>
''' 这个索引是只读的，可以安全地在多个线程之间共享。
''' </remarks>
Friend NotInheritable Class CDHitLSHBuckets

    ReadOnly bandKeys As UInteger()()
    ReadOnly sequences As Integer()()
    ReadOnly positions As Integer()()

    Friend Sub New(bandKeys As UInteger()(), sequences As Integer()(), positions As Integer()())
        Me.bandKeys = bandKeys
        Me.sequences = sequences
        Me.positions = positions
    End Sub

    ''' <summary>
    ''' LSH 波段的数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property NumBands As Integer
        Get
            Return bandKeys.Length
        End Get
    End Property

    ''' <summary>
    ''' 枚举 <paramref name="index"/> 这条序列在 <paramref name="band"/> 波段上面所属的那个桶里面的全部序列
    ''' </summary>
    ''' <param name="index"></param>
    ''' <param name="band"></param>
    ''' <returns>桶内的其它序列的下标（不包含 <paramref name="index"/> 自己）</returns>
    Public Iterator Function BucketMembers(index As Integer, band As Integer) As IEnumerable(Of Integer)
        Dim keys As UInteger() = bandKeys(band)
        Dim ids As Integer() = sequences(band)
        Dim p As Integer = positions(band)(index)
        Dim key As UInteger = keys(p)
        Dim i As Integer = p - 1

        ' 向前扫描
        While i >= 0 AndAlso keys(i) = key
            If ids(i) <> index Then
                Yield ids(i)
            End If

            i -= 1
        End While

        ' 向后扫描
        i = p + 1

        While i < ids.Length AndAlso keys(i) = key
            If ids(i) <> index Then
                Yield ids(i)
            End If

            i += 1
        End While
    End Function
End Class
