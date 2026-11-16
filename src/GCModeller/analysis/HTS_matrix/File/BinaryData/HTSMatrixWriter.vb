Imports System.Buffers
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports Microsoft.VisualBasic.Serialization.JSON
Imports BinaryMatrixCodec = Microsoft.VisualBasic.Serialization.BinaryDumping.NetworkByteOrderBuffer.Compression
Imports std = System.Math

''' <summary>
''' 超大型矩阵二进制文件的**流式写入器**：边计算边写盘，不要求整表驻留内存。
''' </summary>
''' <remarks>
''' <para>
''' 旧的写入路径只有 <see cref="BinaryMatrix.Save"/>, 它要求调用方先把整张矩阵物化在
''' 内存里才能落盘；对于 GB 级的表达矩阵（几万个基因 × 几万个样本），这本身就是一次
''' <see cref="OutOfMemoryException"/> 的高危操作。本类把写入拆成头 + 若干行 + 尾三个阶段：
''' </para>
''' <para>
''' <code>
''' Using writer = HTSMatrixWriter.CreateFile("./matrix.bin", sampleIDs)
'''     For Each row In rows          ' 可以是 CSV 惰性枚举、也可以是计算结果
'''         Call writer.WriteRow(row)
'''     Next
'''     Call writer.Complete()        ' 回填行数与索引
''' End Using
''' </code>
''' </para>
''' <para>
''' **内存占用**：一个块缓冲（默认上限 4 MB，可用 <c>blockRows</c> 调整）。矩阵数据以网络
''' 字节序（大端）逐块编码，编码过程复用同一段 <see cref="ArrayPool(Of Byte)"/> 缓冲区，
''' 不产生任何中间数组与字符串。
''' </para>
''' <para>
''' **可靠性**：<see cref="Complete"/> 之前 <see cref="Dispose"/> 被视为写入失败 ——
''' 文件模式会删除临时文件（不覆盖既有的可用归档），流模式会尝试把流截断回写入起点，
''' 从而避免"半成品文件被下游当成合法矩阵读入"。
''' </para>
''' </remarks>
Public Class HTSMatrixWriter : Implements IDisposable

    ''' <summary>
    ''' 默认每个压缩块/写缓冲的行数
    ''' </summary>
    Public Const DefaultBlockRows As Integer = 512
    ''' <summary>
    ''' 单次缓冲的字节上限，避免 blockRows × blockSize 直接把大对象堆打爆
    ''' </summary>
    Public Const MaxBlockBytes As Integer = 4 * 1024 * 1024

    ReadOnly bin As New NetworkByteOrderBuffer
    ReadOnly writer As BinaryWriter
    ReadOnly stream As Stream
    ReadOnly header As BinaryMatrixHeader
    ReadOnly codec As BinaryMatrixCodec
    ReadOnly chunkRows As Integer
    ReadOnly blockSize As Integer
    ReadOnly leaveOpen As Boolean
    ReadOnly path As String
    ReadOnly tempPath As String
    ReadOnly startPosition As Long

    ''' <summary>
    ''' 行数未知时在这里累积特征名，<see cref="Complete"/> 时写进 trailer
    ''' </summary>
    ReadOnly pendingFeatures As List(Of String)
    ''' <summary>
    ''' 构造时头部给出的行数（<c>-1</c> 表示需要回填）
    ''' </summary>
    ReadOnly initialNfeatures As Integer

    ReadOnly blockOffsets As New List(Of Long)
    Dim buffer As Byte()
    Dim filled As Integer
    Dim nRows As Integer
    Dim completed As Boolean
    Dim disposedValue As Boolean

    ''' <summary>
    ''' 是否校验每一行的特征名与头部给出的顺序一致？默认开启，用于尽早发现行列错位
    ''' </summary>
    Public Property StrictFeatureOrder As Boolean = True

    ''' <summary>
    ''' 已经写入的行数
    ''' </summary>
    Public ReadOnly Property RowCount As Integer
        Get
            Return nRows
        End Get
    End Property

    Public ReadOnly Property IsCompleted As Boolean
        Get
            Return completed
        End Get
    End Property

    Public ReadOnly Property Size As (nsample As Integer, nfeature As Integer)
        Get
            Return (header.Nsamples, nRows)
        End Get
    End Property

    Private Sub New(stream As Stream,
                    header As BinaryMatrixHeader,
                    codec As BinaryMatrixCodec,
                    blockRows As Integer,
                    leaveOpen As Boolean,
                    path As String,
                    tempPath As String)

        Me.stream = stream
        Me.header = header
        Me.codec = codec
        Me.leaveOpen = leaveOpen
        Me.path = path
        Me.tempPath = tempPath
        Me.blockSize = header.Nsamples * RawStream.DblFloat
        Me.startPosition = If(stream.CanSeek, stream.Position, 0)
        Me.initialNfeatures = header.Nfeatures

        If header.Nsamples <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(header), "The sample count of a binary matrix must be a positive number!")
        End If

        Dim requested As Integer = If(blockRows > 0, blockRows, DefaultBlockRows)
        Dim maxRows As Integer = std.Max(1, MaxBlockBytes \ blockSize)

        Me.chunkRows = std.Min(requested, maxRows)
        Me.writer = New BinaryWriter(stream, Encoding.UTF8, leaveOpen)

        If header.FeatureIds.IsNullOrEmpty Then
            Me.pendingFeatures = New List(Of String)()
        End If

        ' codec 由 flags 的低 4 位承载，必须在写头部之前落到 flags 上
        header.Flags = (header.Flags And Not BinaryMatrixHeader.CodecMask) Or (CInt(codec) And BinaryMatrixHeader.CodecMask)

        If codec <> BinaryMatrixCodec.none Then
            ' 压缩块之间需要记录偏移，因此必须有可 seek 的输出流
            If Not stream.CanSeek Then
                Throw New NotSupportedException(
                    "Compressed binary matrix writing requires a seekable output stream: " &
                    "use BinaryMatrixCodec.none for non-seekable outputs (for example a zip entry stream).")
            End If

            header.Flags = header.Flags Or BinaryMatrixHeader.FlagHasBlockIndex
            header.BlockRows = Me.chunkRows
        Else
            header.BlockRows = 0
        End If

        If Not stream.CanSeek Then
            ' 非 seek 流（例如 zip 条目流）：所有需要回填的内容必须在创建时就已知
            If header.FeatureIds.IsNullOrEmpty Then
                Throw New NotSupportedException(
                    "Writing a binary matrix with an unknown feature list requires a seekable output stream: " &
                    "please pass the featureIDs (and the total feature count) while creating the writer.")
            End If
            If header.Nfeatures < 0 Then
                Throw New NotSupportedException(
                    "Writing a binary matrix with an unknown feature count requires a seekable output stream: " &
                    "please pass the 'nfeatures' parameter while creating the writer.")
            End If
        End If

        Call BinaryMatrixHeader.Write(writer, header)

        Me.buffer = ArrayPool(Of Byte).Shared.Rent(std.Max(blockSize, chunkRows * blockSize))
    End Sub

    ''' <summary>
    ''' 在一个流之上创建流式写入器
    ''' </summary>
    ''' <param name="stream">输出流</param>
    ''' <param name="sampleIDs">列标签（样本名）</param>
    ''' <param name="featureIDs">
    ''' 行标签（特征名）。为 <c>Nothing</c> 时表示行数在写入过程中才能确定，
    ''' 此时特征名会被累积到 trailer 里，且要求 <paramref name="stream"/> 可 seek
    ''' </param>
    ''' <param name="tag">矩阵标签</param>
    ''' <param name="codec">压缩方式</param>
    ''' <param name="blockRows">每个压缩块/写缓冲的行数；0 或负数使用默认值</param>
    ''' <param name="nfeatures">
    ''' 总行数。已知时给出可以省掉一次头部回填（非 seek 流必须给出）；
    ''' <c>-1</c> 表示由 <see cref="Complete"/> 回填
    ''' </param>
    ''' <param name="version">格式版本：1 = 旧格式，2 = 当前格式（默认）</param>
    ''' <param name="leaveOpen">完成/释放之后是否保持底层流可用</param>
    Public Shared Function Create(stream As Stream,
                                  sampleIDs As String(),
                                  Optional featureIDs As String() = Nothing,
                                  Optional tag As String = Nothing,
                                  Optional codec As BinaryMatrixCodec = BinaryMatrixCodec.none,
                                  Optional blockRows As Integer = 0,
                                  Optional nfeatures As Integer = -1,
                                  Optional version As Integer = 2,
                                  Optional leaveOpen As Boolean = False) As HTSMatrixWriter

        Dim header As New BinaryMatrixHeader With {
            .Version = version,
            .DataType = 0,
            .tag = If(String.IsNullOrEmpty(tag), "NA", tag),
            .Nsamples = sampleIDs.Length,
            .Nfeatures = If(featureIDs.IsNullOrEmpty, If(nfeatures >= 0, nfeatures, -1), featureIDs.Length),
            .SampleIds = sampleIDs,
            .FeatureIds = featureIDs
        }

        Return New HTSMatrixWriter(stream, header, codec, blockRows, leaveOpen, Nothing, Nothing)
    End Function

    ''' <summary>
    ''' 创建一个文件写入器（先写临时文件，<see cref="Complete"/> 后原子替换目标文件）
    ''' </summary>
    ''' <param name="path">目标文件路径</param>
    ''' <param name="sampleIDs">列标签（样本名）</param>
    ''' <param name="featureIDs">行标签（特征名）；<c>Nothing</c> 表示行数未知</param>
    ''' <param name="tag">矩阵标签</param>
    ''' <param name="codec">压缩方式</param>
    ''' <param name="blockRows">每个压缩块/写缓冲的行数</param>
    ''' <param name="nfeatures">总行数，<c>-1</c> 表示由 <see cref="Complete"/> 回填</param>
    ''' <param name="version">格式版本</param>
    Public Shared Function CreateFile(path As String,
                                      sampleIDs As String(),
                                      Optional featureIDs As String() = Nothing,
                                      Optional tag As String = Nothing,
                                      Optional codec As BinaryMatrixCodec = BinaryMatrixCodec.none,
                                      Optional blockRows As Integer = 0,
                                      Optional nfeatures As Integer = -1,
                                      Optional version As Integer = 2) As HTSMatrixWriter

        Dim dir As String = IO.Path.GetDirectoryName(IO.Path.GetFullPath(path))

        If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
            Call Directory.CreateDirectory(dir)
        End If

        Dim temp As String = path & ".tmp-" & Guid.NewGuid().ToString("N").Substring(0, 8)
        Dim fs As New FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None, 64 * 1024)
        Dim header As New BinaryMatrixHeader With {
            .Version = version,
            .DataType = 0,
            .tag = If(String.IsNullOrEmpty(tag), "NA", tag),
            .Nsamples = sampleIDs.Length,
            .Nfeatures = If(featureIDs.IsNullOrEmpty, If(nfeatures >= 0, nfeatures, -1), featureIDs.Length),
            .SampleIds = sampleIDs,
            .FeatureIds = featureIDs
        }

        Return New HTSMatrixWriter(fs, header, codec, blockRows, leaveOpen:=False, path:=path, tempPath:=temp)
    End Function

    ''' <summary>
    ''' 写入一行
    ''' </summary>
    ''' <param name="featureID">行标签</param>
    ''' <param name="values">该行的表达值，长度必须等于样本数</param>
    Public Sub WriteRow(featureID As String, values As Double())
        If completed Then
            Throw New InvalidOperationException("The binary matrix writer has already been completed!")
        End If
        If values Is Nothing OrElse values.Length <> header.Nsamples Then
            Throw New ArgumentException(
                $"Invalid row data size at row {nRows + 1}('{featureID}'): {If(values Is Nothing, "nothing", values.Length.ToString)} value(s) " &
                $"but {header.Nsamples} sample(s) were expected!", NameOf(values))
        End If

        If pendingFeatures IsNot Nothing Then
            Call pendingFeatures.Add(featureID)
        ElseIf StrictFeatureOrder AndAlso featureID <> header.FeatureIds(nRows) Then
            Throw New InvalidOperationException(
                $"Feature order mismatch at row {nRows + 1}: '{featureID}' was written but '{header.FeatureIds(nRows)}' was expected!")
        End If

        Call bin.EncodeDoubles(values, 0, values.Length, buffer, filled * blockSize)

        filled += 1
        nRows += 1

        If filled >= chunkRows Then
            Call flushBlock()
        End If
    End Sub

    ''' <summary>
    ''' 写入一行（直接接受 <see cref="DataFrameRow"/>）
    ''' </summary>
    ''' <param name="row"></param>
    Public Sub WriteRow(row As DataFrameRow)
        If row Is Nothing Then
            Throw New ArgumentNullException(NameOf(row))
        End If

        Call WriteRow(row.geneID, row.experiments)
    End Sub

    ''' <summary>
    ''' 批量写入
    ''' </summary>
    ''' <param name="rows"></param>
    Public Sub WriteRows(rows As IEnumerable(Of DataFrameRow))
        If rows Is Nothing Then
            Return
        End If

        For Each row As DataFrameRow In rows
            Call WriteRow(row)
        Next
    End Sub

    ''' <summary>
    ''' 批量写入数组的一个区间
    ''' </summary>
    Public Sub WriteBlock(rows As DataFrameRow(), offset As Integer, count As Integer)
        If rows Is Nothing Then
            Throw New ArgumentNullException(NameOf(rows))
        End If

        For i As Integer = offset To offset + count - 1
            Call WriteRow(rows(i))
        Next
    End Sub

    ''' <summary>
    ''' 把缓冲区之中尚未落盘的行写出去（不结束文件）
    ''' </summary>
    Public Sub Flush()
        If filled > 0 Then
            Call flushBlock()
        End If

        Call writer.Flush()
    End Sub

    Private Sub flushBlock()
        If filled <= 0 Then
            Return
        End If

        Dim bytes As Integer = filled * blockSize

        If codec = BinaryMatrixCodec.none Then
            Call writer.Write(buffer, 0, bytes)
        Else
            Dim offset As Long = stream.Position - header.DataStart

            Using deflate As Stream = openDeflate()
                Call deflate.Write(buffer, 0, bytes)
            End Using

            Call blockOffsets.Add(offset)
        End If

        filled = 0
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function openDeflate() As Stream
        If codec = BinaryMatrixCodec.gzip Then
            Return New GZipStream(stream, CompressionLevel.Fastest, leaveOpen:=True)
        Else
            Return New ZLibStream(stream, CompressionLevel.Fastest, leaveOpen:=True)
        End If
    End Function

    ''' <summary>
    ''' 结束写入：落定行数、块索引与特征名，并回填头部
    ''' </summary>
    Public Sub Complete()
        If completed Then
            Return
        End If

        Call Flush()

        If pendingFeatures IsNot Nothing Then
            header.FeatureIds = pendingFeatures.ToArray
            header.Flags = header.Flags Or BinaryMatrixHeader.FlagFeatureIdsInTrailer
        End If

        header.Nfeatures = nRows

        Dim needTrailer As Boolean = codec <> BinaryMatrixCodec.none OrElse pendingFeatures IsNot Nothing

        If needTrailer Then
            Call writer.Flush()

            header.IndexOffset = stream.Position

            If codec <> BinaryMatrixCodec.none Then
                ' nblocks + 1 个相对偏移：第 i 块起点 + 数据区末尾
                Call blockOffsets.Add(stream.Position - header.DataStart)

                writer.Write(CLng(blockOffsets.Count - 1))

                For i As Integer = 0 To blockOffsets.Count - 1
                    writer.Write(blockOffsets(i))
                Next
            Else
                writer.Write(0L)   ' 没有块索引
            End If

            If pendingFeatures IsNot Nothing Then
                writer.Write(header.FeatureIds.GetJson)
            End If
        End If

        Call writer.Flush()

        ' 只有"行数未知"或者"存在 trailer"这两种情况才需要回填头部
        If header.Version >= 2 AndAlso (needTrailer OrElse initialNfeatures < 0) Then
            Call BinaryMatrixHeader.Patch(writer, header)
        End If

        completed = True

        If path IsNot Nothing Then
            ' 文件模式：关闭临时文件后原子替换目标文件
            Call writer.Dispose()
            Call File.Move(tempPath, path, overwrite:=True)
        ElseIf Not leaveOpen Then
            Call writer.Dispose()
        End If
    End Sub

    Private Sub rollback()
        Try
            If path IsNot Nothing Then
                Call writer.Dispose()

                If File.Exists(tempPath) Then
                    Call File.Delete(tempPath)
                End If
            ElseIf stream.CanSeek Then
                Call writer.Flush()
                stream.SetLength(startPosition)
                stream.Position = startPosition

                If Not leaveOpen Then
                    Call writer.Dispose()
                End If
            ElseIf Not leaveOpen Then
                Call writer.Dispose()
            End If
        Catch ex As Exception
            ' 回滚失败不应该掩盖原始的写入异常
        End Try
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If disposedValue Then
            Return
        End If

        If Not completed Then
            Call rollback()
        Else
            If Not leaveOpen Then
                Call writer.Dispose()
            End If
        End If

        If buffer IsNot Nothing Then
            Call ArrayPool(Of Byte).Shared.Return(buffer)
            buffer = Nothing
        End If

        disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Call Dispose(disposing:=True)
        Call GC.SuppressFinalize(Me)
    End Sub

    Public Overrides Function ToString() As String
        Return $"HTSMatrixWriter [{header.Nsamples}x{nRows}] codec={codec} completed={completed}"
    End Function

End Class
