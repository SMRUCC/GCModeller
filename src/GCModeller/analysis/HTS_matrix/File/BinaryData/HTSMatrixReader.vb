Imports System.Buffers
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports std = System.Math

''' <summary>
''' 超大型矩阵二进制文件的**流式惰性读取器**：按需取行/取块/取列，不需要把整张矩阵读进内存。
''' </summary>
''' <remarks>
''' <para>
''' 矩阵形状约定不变：特征是行（row），样本观测是列（column）。
''' </para>
''' <para>
''' **能力矩阵**：
''' <list type="table">
''' <item><term>未压缩 + 可 seek</term><description>行级 O(1) 随机访问，线程安全</description></item>
''' <item><term>压缩 + 有块索引 + 可 seek</term><description>块级随机访问（解压一块后切片），带 LRU 块缓存</description></item>
''' <item><term>不可 seek 的流</term><description>只能通过 <see cref="Rows"/> 顺序读取；随机访问会抛出明确的异常</description></item>
''' </list>
''' </para>
''' <para>
''' **与旧版本相比修复的问题**：
''' <list type="number">
''' <item>旧实现的 <c>Read(buffer, scan0, buffer.Length)</c> 把"数据区起始的绝对文件偏移"当成了
''' 缓冲区偏移，只要 <c>scan0 &gt; 0</c> 就必然越界抛 <see cref="ArgumentException"/>；</item>
''' <item>忽略了 <see cref="Stream.Read"/> 的返回值，遇到部分读会得到脏数据，
''' 现在统一使用 <see cref="Stream.ReadExactly(Byte(), Integer, Integer)"/>；</item>
''' <item>共享的 <see cref="BinaryReader"/> 在多线程下会互相破坏流位置，
''' 现在所有"定位 + 读取"都在 <c>SyncLock</c> 之内完成；</item>
''' <item>按基因名批量取列值时，旧实现每个基因一次 seek + 读 8 字节，
''' 现在先排序去重再读取，并对重复行号做缓存。</item>
''' </list>
''' </para>
''' </remarks>
Public Class HTSMatrixReader : Inherits MatrixViewer
    Implements IDisposable

    ''' <summary>
    ''' 单次读取的字节上限（约 1 MB - 4 MB），避免一次把几百 MB 的行块拉进内存
    ''' </summary>
    Public Const MaxReadBytes As Integer = 4 * 1024 * 1024
    ''' <summary>
    ''' 压缩模式下保留在内存之中的块数量
    ''' </summary>
    Public Const BlockCacheSize As Integer = 2

    ReadOnly bin As New NetworkByteOrderBuffer
    ReadOnly reader As BinaryReader
    ReadOnly stream As Stream
    ReadOnly matrix As BinaryMatrixHeader
    ReadOnly sampleID As Index(Of String)
    ReadOnly geneIDs As Index(Of String)
    ReadOnly sync As New Object()

    ' 压缩模式的块缓存（简单的 FIFO 淘汰，块数量很小所以不需要真正的 LRU）
    ReadOnly blockCache As New Dictionary(Of Integer, Double()())
    ReadOnly blockCacheOrder As New List(Of Integer)

    Dim disposedValue As Boolean

    ''' <summary>
    ''' 当前文件所声明的格式版本（1 或 2）
    ''' </summary>
    Public ReadOnly Property Version As Integer
        Get
            Return matrix.Version
        End Get
    End Property

    ''' <summary>
    ''' 当前文件所使用的压缩方式
    ''' </summary>
    Public ReadOnly Property Codec As BinaryMatrixCodec
        Get
            Return matrix.Codec
        End Get
    End Property

    ''' <summary>
    ''' 文件头部的只读视图（供归档读取等场景直接查询维度与标签）
    ''' </summary>
    Public ReadOnly Property FileHeader As BinaryMatrixHeader
        Get
            Return matrix
        End Get
    End Property

    ''' <summary>
    ''' sample data id in columns
    ''' </summary>
    ''' <returns></returns>
    Public Overrides ReadOnly Property SampleIDs As IEnumerable(Of String)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return sampleID.Objects
        End Get
    End Property

    ''' <summary>
    ''' gene feature ids in rows
    ''' </summary>
    ''' <returns></returns>
    Public Overrides ReadOnly Property FeatureIDs As IEnumerable(Of String)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return geneIDs.Objects
        End Get
    End Property

    ''' <summary>
    ''' the matrix shape dimension is features in rows and 
    ''' the sample observation in columns:
    ''' 
    ''' 1. nsamples: the matrix column width
    ''' 2. nfeature: the matrix row height
    ''' </summary>
    ''' <returns></returns>
    Public Overrides ReadOnly Property Size As (nsample As Integer, nfeature As Integer)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return (sampleID.Count, geneIDs.Count)
        End Get
    End Property

    Public Overrides ReadOnly Property CanRandomAccess As Boolean
        Get
            If Not stream.CanSeek Then
                Return False
            ElseIf matrix.IsCompressed Then
                Return Not matrix.BlockOffsets.IsNullOrEmpty
            Else
                Return True
            End If
        End Get
    End Property

    ''' <summary>
    ''' 打开一个二进制矩阵文件（v1 或 v2 都会被自动识别）
    ''' </summary>
    ''' <param name="file"></param>
    ''' <exception cref="InvalidDataException">magic/版本不正确，或者文件没有写完（行数未落定）</exception>
    Sub New(file As Stream)
        Me.stream = file
        Me.reader = New BinaryReader(file, Encoding.UTF8, leaveOpen:=False)
        Me.matrix = BinaryMatrixHeader.Read(Me.reader)

        Call Me.matrix.LoadTrailer(Me.reader)

        If Not matrix.IsFinalized Then
            Throw New InvalidDataException(
                "The binary matrix file is incomplete: the writer did not finalize the feature count. " &
                "Please re-generate the file with HTSMatrixWriter.Complete().")
        End If

        If matrix.DataType <> 0 Then
            Throw New InvalidDataException($"Unsupported binary matrix data type {matrix.DataType}: only double(0) is supported!")
        End If

        If matrix.IsCompressed AndAlso Not stream.CanSeek Then
            Throw New NotSupportedException(
                "A compressed binary matrix can only be read from a seekable stream: " &
                "copy the archive entry into a MemoryStream first, or re-generate the matrix without compression.")
        End If

        Me.tagString = matrix.tag
        Me.sampleID = New Index(Of String)(normalizeSampleIds())
        Me.geneIDs = New Index(Of String)(normalizeFeatureIds())
    End Sub

    ''' <summary>
    ''' 样本标签数量与头部声明的列数不一致时补上占位名称，避免下游 <see cref="Size"/> 与实际数据错位
    ''' </summary>
    Private Function normalizeSampleIds() As String()
        Dim ids As String() = matrix.SampleIds

        If Not ids.IsNullOrEmpty AndAlso ids.Length = matrix.Nsamples Then
            Return ids
        End If

        Dim names As String() = New String(matrix.Nsamples - 1) {}

        For i As Integer = 0 To names.Length - 1
            names(i) = If(ids.IsNullOrEmpty OrElse i >= ids.Length, "sample_" & (i + 1), ids(i))
        Next

        Return names
    End Function

    Private Function normalizeFeatureIds() As String()
        Dim ids As String() = matrix.FeatureIds

        If Not ids.IsNullOrEmpty AndAlso ids.Length = matrix.Nfeatures Then
            Return ids
        End If

        Dim names As String() = New String(matrix.Nfeatures - 1) {}

        For i As Integer = 0 To names.Length - 1
            names(i) = If(ids.IsNullOrEmpty OrElse i >= ids.Length, "feature_" & (i + 1), ids(i))
        Next

        Return names
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetSampleOrdinal(sampleID As String) As Integer
        Return Me.sampleID.IndexOf(sampleID)
    End Function

    ''' <summary>
    ''' 单次读取的块行数（按字节预算计算）
    ''' </summary>
    Private Function rowsPerChunk() As Integer
        Return std.Max(1, MaxReadBytes \ matrix.BlockSize)
    End Function

    ''' <summary>
    ''' 定位并精确读取（<see cref="Stream.Read"/> 可能返回部分数据，这里必须循环读满）
    ''' </summary>
    Private Sub readAt(offset As Long, buffer As Byte(), count As Integer, rowStart As Integer)
        SyncLock sync
            ' 只进流（zip 的 deflate 流等）无法定位，此时调用方必须是顺序前进的读取
            If stream.CanSeek Then
                stream.Position = offset
            End If

            Call readExactly(buffer, 0, count, rowStart)
        End SyncLock
    End Sub

    Private Sub readExactly(buffer As Byte(), offset As Integer, count As Integer, rowStart As Integer)
        Try
            Call stream.ReadExactly(buffer, offset, count)
        Catch ex As EndOfStreamException
            Throw New InvalidDataException(
                $"The binary matrix data is truncated at row {rowStart + 1}: " &
                $"{count} byte(s) were expected but the stream ended early!", ex)
        End Try
    End Sub

    Private Function featureName(rowIndex As Integer) As String
        SyncLock sync
            If geneIDs.Count = 0 Then
                Return "feature_" & (rowIndex + 1)
            Else
                Return geneIDs.Objects(rowIndex)
            End If
        End SyncLock
    End Function

    Private Function newRow(rowIndex As Integer, values As Double()) As DataFrameRow
        Return New DataFrameRow With {
            .geneID = featureName(rowIndex),
            .experiments = values
        }
    End Function

#Region "Uncompressed sequential access"

    Private Function readUncompressed(rowStart As Integer, nRows As Integer) As DataFrameRow()
        Dim size As Integer = matrix.BlockSize
        Dim buffer As Byte() = ArrayPool(Of Byte).Shared.Rent(std.Max(nRows * size, 8))
        Dim rows As DataFrameRow() = New DataFrameRow(nRows - 1) {}

        Try
            Call readAt(matrix.DataStart + CLng(rowStart) * size, buffer, nRows * size, rowStart)

            For i As Integer = 0 To nRows - 1
                Dim v As Double() = New Double(matrix.Nsamples - 1) {}

                Call bin.DecodeDoubles(buffer, i * size, matrix.Nsamples, v)

                rows(i) = newRow(rowStart + i, v)
            Next
        Finally
            Call ArrayPool(Of Byte).Shared.Return(buffer)
        End Try

        Return rows
    End Function

#End Region

#Region "Compressed block access"

    Private Function blockRows() As Integer
        If matrix.BlockRows > 0 Then
            Return matrix.BlockRows
        Else
            Return std.Max(matrix.Nfeatures, 1)
        End If
    End Function

    Private Function ensureBlock(blockIndex As Integer) As Double()()
        Dim cached As Double()() = Nothing

        SyncLock sync
            If blockCache.TryGetValue(blockIndex, cached) Then
                Return cached
            End If

            cached = decompressBlock(blockIndex)
            Call storeBlock(blockIndex, cached)

            Return cached
        End SyncLock
    End Function

    Private Sub storeBlock(index As Integer, data As Double()())
        If blockCache.ContainsKey(index) Then
            Return
        End If

        Do While blockCacheOrder.Count >= BlockCacheSize
            Dim evict As Integer = blockCacheOrder(0)

            Call blockCacheOrder.RemoveAt(0)
            Call blockCache.Remove(evict)
        Loop

        blockCache(index) = data
        Call blockCacheOrder.Add(index)
    End Sub

    Private Function decompressBlock(blockIndex As Integer) As Double()()
        If matrix.BlockOffsets.IsNullOrEmpty Then
            Throw New NotSupportedException(
                "This compressed binary matrix has no block index, so random access is not available: " &
                "please enumerate the rows sequentially via Rows() instead!")
        End If
        If blockIndex + 1 >= matrix.BlockOffsets.Length Then
            Throw New InvalidDataException(
                $"Invalid block index {blockIndex}: the block offset table only contains {matrix.BlockOffsets.Length} entries!")
        End If

        Dim rows As Integer = blockRows()
        Dim rowStart As Integer = blockIndex * rows
        Dim blockN As Integer = std.Min(rows, matrix.Nfeatures - rowStart)
        Dim start As Long = matrix.BlockOffsets(blockIndex)
        Dim [end] As Long = matrix.BlockOffsets(blockIndex + 1)
        Dim compressed As Integer = CInt([end] - start)

        If compressed <= 0 Then
            Throw New InvalidDataException($"Invalid compressed block size {compressed} at block {blockIndex}!")
        End If

        Dim size As Integer = matrix.BlockSize
        Dim rawLen As Integer = blockN * size
        Dim comp As Byte() = ArrayPool(Of Byte).Shared.Rent(compressed)
        Dim raw As Byte() = ArrayPool(Of Byte).Shared.Rent(rawLen)

        Try
            Call readAt(matrix.DataStart + start, comp, compressed, rowStart)

            Using ms As New MemoryStream(comp, 0, compressed, writable:=False)
                Using inflate As Stream = openInflate(ms)
                    Call inflate.ReadExactly(raw, 0, rawLen)
                End Using
            End Using

            Dim data As Double()() = New Double(blockN - 1)() {}

            For i As Integer = 0 To blockN - 1
                Dim v As Double() = New Double(matrix.Nsamples - 1) {}

                Call bin.DecodeDoubles(raw, i * size, matrix.Nsamples, v)

                data(i) = v
            Next

            Return data
        Finally
            Call ArrayPool(Of Byte).Shared.Return(comp)
            Call ArrayPool(Of Byte).Shared.Return(raw)
        End Try
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function openInflate(source As Stream) As Stream
        If matrix.Codec = BinaryMatrixCodec.gzip Then
            Return New GZipStream(source, CompressionMode.Decompress, leaveOpen:=True)
        Else
            Return New ZLibStream(source, CompressionMode.Decompress, leaveOpen:=True)
        End If
    End Function

    Private Function readCompressedRow(rowIndex As Integer) As Double()
        Dim rows As Integer = blockRows()
        Dim blockIndex As Integer = rowIndex \ rows
        Dim data As Double()() = ensureBlock(blockIndex)

        Return data(rowIndex - blockIndex * rows)
    End Function

#End Region

    ''' <summary>
    ''' 按行号取一行（O(1) 定位；压缩模式下为"解压一块 + 切片"）
    ''' </summary>
    ''' <param name="index">行号，从 0 开始</param>
    Public Overrides Function GetRow(index As Integer) As DataFrameRow
        If index < 0 OrElse index >= matrix.Nfeatures Then
            Throw New ArgumentOutOfRangeException(
                NameOf(index), index, $"The row index must be in [0, {matrix.Nfeatures - 1}]!")
        End If

        If matrix.IsCompressed Then
            Return newRow(index, readCompressedRow(index))
        ElseIf stream.CanSeek Then
            Return readUncompressed(index, 1)(0)
        Else
            Throw new NotSupportedException(noRandomAccessMessage)
        End If
    End Function

    ''' <summary>
    ''' 按行号区间批量取行
    ''' </summary>
    ''' <param name="start">起始行号</param>
    ''' <param name="count">行数</param>
    Public Overrides Function GetRows(start As Integer, count As Integer) As DataFrameRow()
        If count <= 0 Then
            Return New DataFrameRow() {}
        End If
        If start < 0 OrElse start + count > matrix.Nfeatures Then
            Throw New ArgumentOutOfRangeException(
                NameOf(start), $"The row range [{start}, {start + count}) is out of the matrix bounds [0, {matrix.Nfeatures})!")
        End If

        Dim rows As DataFrameRow() = New DataFrameRow(count - 1) {}

        If matrix.IsCompressed OrElse Not stream.CanSeek Then
            For i As Integer = 0 To count - 1
                rows(i) = GetRow(start + i)
            Next
        Else
            Dim perChunk As Integer = rowsPerChunk()
            Dim offset As Integer = 0

            While offset < count
                Dim n As Integer = std.Min(perChunk, count - offset)
                Dim chunk As DataFrameRow() = readUncompressed(start + offset, n)

                Call Array.ConstrainedCopy(chunk, 0, rows, offset, n)

                offset += n
            End While
        End If

        Return rows
    End Function

    ''' <summary>
    ''' 顺序惰性枚举所有行：内存占用只随"块"增长
    ''' </summary>
    ''' <remarks>
    ''' 该迭代器持有流的位置，因此**不要**与 <see cref="GetRow"/> 等随机访问方法并发调用。
    ''' </remarks>
    Public Overrides Iterator Function Rows() As IEnumerable(Of DataFrameRow)
        If matrix.Nfeatures <= 0 Then
            Return
        End If

        If matrix.IsCompressed Then
            For i As Integer = 0 To matrix.Nfeatures - 1
                Yield GetRow(i)
            Next
        Else
            Dim perChunk As Integer = rowsPerChunk()
            Dim row As Integer = 0
            Dim chunk As DataFrameRow()

            While row < matrix.Nfeatures
                Dim n As Integer = std.Min(perChunk, matrix.Nfeatures - row)

                chunk = readUncompressed(row, n)

                For Each r As DataFrameRow In chunk
                    Yield r
                Next

                row += n
            End While
        End If
    End Function

    ''' <summary>
    ''' 取一整列样本数据
    ''' </summary>
    ''' <param name="sampleOrdinal">样本列号，从 0 开始</param>
    ''' <remarks>
    ''' 这里按"连续的行块"读取，再从块中抽取该列，避免逐行 seek 造成的 n 次系统调用。
    ''' </remarks>
    Public Overrides Function GetSample(sampleOrdinal As Integer) As Double()
        If sampleOrdinal < 0 OrElse sampleOrdinal >= matrix.Nsamples Then
            Throw New ArgumentOutOfRangeException(
                NameOf(sampleOrdinal), sampleOrdinal, $"The sample ordinal must be in [0, {matrix.Nsamples - 1}]!")
        End If

        Dim v As Double() = New Double(matrix.Nfeatures - 1) {}

        If matrix.IsCompressed OrElse Not stream.CanSeek Then
            Dim i As Integer = 0

            For Each row As DataFrameRow In Rows()
                v(i) = row.experiments(sampleOrdinal)
                i += 1
            Next
        Else
            Dim size As Integer = matrix.BlockSize
            Dim perChunk As Integer = rowsPerChunk()
            Dim buffer As Byte() = ArrayPool(Of Byte).Shared.Rent(perChunk * size)

            Try
                Dim row As Integer = 0

                While row < matrix.Nfeatures
                    Dim n As Integer = std.Min(perChunk, matrix.Nfeatures - row)

                    Call readAt(matrix.DataStart + CLng(row) * size, buffer, n * size, row)

                    For i As Integer = 0 To n - 1
                        v(row + i) = bin.DecodeDouble(buffer, i * size + sampleOrdinal * RawStream.DblFloat)
                    Next

                    row += n
                End While
            Finally
                Call ArrayPool(Of Byte).Shared.Return(buffer)
            End Try
        End If

        Return v
    End Function

    Private ReadOnly Property noRandomAccessMessage As String
        Get
            Return "Random access is not available on this binary matrix stream: " &
                   "the stream is not seekable, please enumerate the rows sequentially via Rows() instead!"
        End Get
    End Property

    ''' <summary>
    ''' 按基因名取整行表达数据
    ''' </summary>
    ''' <param name="geneID"></param>
    Public Overrides Function GetGeneExpression(geneID As String) As Double()
        Dim i As Integer

        SyncLock sync
            If geneID Like geneIDs Then
                i = geneIDs.IndexOf(geneID)
            Else
                Return New Double(matrix.Nsamples - 1) {}
            End If
        End SyncLock

        Return GetRow(i).experiments
    End Function

    ''' <summary>
    ''' 取一组基因在某个样本列上的表达值
    ''' </summary>
    ''' <param name="geneID">目标基因集合</param>
    ''' <param name="sampleOrdinal">样本列号</param>
    ''' <returns>与 <paramref name="geneID"/> 顺序对齐的表达值</returns>
    ''' <remarks>
    ''' 稀疏取值时按行号排序去重之后逐个读取（每次只读 8 字节），
    ''' 重复出现的基因名只会产生一次 IO。
    ''' </remarks>
    Public Overrides Function GetGeneExpression(geneID() As String, sampleOrdinal As Integer) As Double()
        Dim v As Double() = New Double(geneID.Length - 1) {}

        If sampleOrdinal < 0 Then
            Return v
        End If

        Dim index As Integer() = New Integer(geneID.Length - 1) {}

        SyncLock sync
            For i As Integer = 0 To geneID.Length - 1
                index(i) = If(geneID(i) Like geneIDs, geneIDs.IndexOf(geneID(i)), -1)
            Next
        End SyncLock

        If matrix.IsCompressed Then
            For i As Integer = 0 To index.Length - 1
                If index(i) >= 0 Then
                    v(i) = readCompressedRow(index(i))(sampleOrdinal)
                End If
            Next
        ElseIf Not stream.CanSeek Then
            For i As Integer = 0 To index.Length - 1
                If index(i) >= 0 Then
                    v(i) = GetRow(index(i)).experiments(sampleOrdinal)
                End If
            Next
        Else
            Dim cache As New Dictionary(Of Integer, Double)
            Dim buffer As Byte() = ArrayPool(Of Byte).Shared.Rent(RawStream.DblFloat)

            Try
                For Each rowIndex As Integer In index _
                    .Where(Function(x) x >= 0) _
                    .Distinct _
                    .OrderBy(Function(x) x)

                    Call readAt(
                        matrix.DataStart + CLng(rowIndex) * matrix.BlockSize + CLng(sampleOrdinal) * RawStream.DblFloat,
                        buffer,
                        RawStream.DblFloat,
                        rowIndex
                    )

                    cache(rowIndex) = bin.DecodeDouble(buffer, 0)
                Next
            Finally
                Call ArrayPool(Of Byte).Shared.Return(buffer)
            End Try

            For i As Integer = 0 To index.Length - 1
                If index(i) >= 0 Then
                    v(i) = cache(index(i))
                End If
            Next
        End If

        Return v
    End Function

    Public Overrides Sub SetNewSampleIDs(sampleIDs() As String)
        SyncLock sync
            Call Me.sampleID.Clear()
            Call Me.sampleID.Add(sampleIDs).ToArray
        End SyncLock
    End Sub

    ''' <summary>
    ''' just updates of the gene id index
    ''' </summary>
    ''' <param name="geneIDs"></param>
    Public Overrides Sub SetNewGeneIDs(geneIDs() As String)
        SyncLock sync
            Call Me.geneIDs.Clear()
            Call Me.geneIDs.Add(geneIDs).ToArray
        End SyncLock
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Call blockCache.Clear()
                Call blockCacheOrder.Clear()
                Call reader.Dispose()
            End If

            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Call Dispose(disposing:=True)
        Call GC.SuppressFinalize(Me)
    End Sub
End Class
