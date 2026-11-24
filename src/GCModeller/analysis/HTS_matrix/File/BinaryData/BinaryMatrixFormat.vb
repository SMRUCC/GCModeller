Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports BinaryMatrixCodec = Microsoft.VisualBasic.Serialization.BinaryDumping.NetworkByteOrderBuffer.Compression
Imports std = System.Math

''' <summary>
''' <c>HTS_matrix</c> 二进制矩阵文件的头部模型，同时是全项目**唯一**的头部编解码实现。
''' </summary>
''' <remarks>
''' <para>
''' 之所以要把头部收敛到一个类里面：v1 的头部原先分别由 <c>BinaryMatrix.readMatrix</c>、
''' <c>HTSMatrixReader</c> 的构造函数以及 <c>PLAS.NET/SGrid/ZipPersists.readBinaryMatrix</c>
''' 各自手写解析，三处偏移量稍有不同步就会出现"读出来全是垃圾数据"这类难以定位的问题。
''' </para>
''' <para>
''' **格式版本**：v2 使用了另一个 magic 字符串（<see cref="MagicV2"/>），而不是在原有 magic
''' 之后插入一个版本号字段。原因是 v1 的头部紧接着 magic 的是 <see cref="BinaryWriter.Write(String)"/>
''' 的 7-bit 长度前缀，旧读取器会把新的 Int32 版本号当作标签长度继续解析，最终在 JSON
''' 反序列化处抛出难以理解的异常；换 magic 则可以让旧读取器在第一次 21 字节比较时就
''' **立即 fail-fast**。两个 magic 长度相同（21 字节），因此 <see cref="BinaryMatrix.MagicHeader"/>
''' 常量与 <c>Encoding.ASCII.GetBytes(BinaryMatrix.MagicHeader)</c> 的既有用法不受影响。
''' </para>
''' <para>
''' **v2 布局**（定长整数一律小端，与 v1 的 nsamples/nfeatures 保持一致；
''' 矩阵数据区仍然是网络字节序/大端的 double —— 这个"头部小端、负载大端"的约定必须原样保留）：
''' <code>
''' [21]  magic  = "GCModeller/HTS_bin/v2"
''' [i32] version = 2
''' [i32] flags   bit0..3 codec(0=none,1=gzip,2=zlib), bit8 hasBlockIndex, bit9 featureIdsInTrailer
''' [1]   dtype   (0 = double, 预留 float32)
''' [3]   reserved = 0
''' [i64] indexOffset  (trailer 的绝对文件偏移；-1 = 无 trailer)
''' [str] tag
''' [i32] nsamples
''' [i32] nfeatures    (-1 = 写入方尚未落定，即文件未完成写入)
''' [i32] blockRows    (压缩分块行数；0 = 不切块)
''' [str] sampleIdsJson
''' [str] featureIdsJson (flags.bit9 置位时写 "[]"，真实列表在 trailer)
''' [data] 按块排列的行数据
''' [trailer at indexOffset]
'''     [i64] nblocks
'''     [i64] blockOffsets[nblocks + 1]   (相对数据区起点，便于整个文件搬迁)
'''     [str] featureIdsJson              (仅 flags.bit9 置位时存在)
''' </code>
''' </para>
''' <para>
''' 这样设计之后，**无需回填**的内容全部可以确定性算出（数据区起点、块大小、尾部位置），
''' 只有 <see cref="Nfeatures"/> 与 <see cref="IndexOffset"/> 两个定长字段需要在
''' <c>Complete()</c> 时 seek 回填，而这两个字段的位置在写入头部时就已经记录下来了。
''' </para>
''' </remarks>
Public Class BinaryMatrixHeader

    ''' <summary>
    ''' v1 格式的 magic 字符串（21 字节），与 <see cref="BinaryMatrix.MagicHeader"/> 完全一致
    ''' </summary>
    Public Const MagicV1 As String = "GCModeller/HTS_matrix"
    ''' <summary>
    ''' v2 格式的 magic 字符串（同样 21 字节）
    ''' </summary>
    Public Const MagicV2 As String = "GCModeller/HTS_bin/v2"

    Public Const MagicSize As Integer = 21

    ''' <summary>
    ''' flags 之中的 codec 位掩码
    ''' </summary>
    Public Const CodecMask As Integer = &HF
    ''' <summary>
    ''' flags 置位表示 trailer 之中存在块偏移索引
    ''' </summary>
    Public Const FlagHasBlockIndex As Integer = &H100
    ''' <summary>
    ''' flags 置位表示特征名列表写在 trailer 之中（写入时行数未知的场景）
    ''' </summary>
    Public Const FlagFeatureIdsInTrailer As Integer = &H200

    ''' <summary>
    ''' 格式版本：1 = 旧格式，2 = 当前格式
    ''' </summary>
    Public Property Version As Integer = 2
    ''' <summary>
    ''' 头部标志位，见 <see cref="FlagHasBlockIndex"/> 等常量
    ''' </summary>
    Public Property Flags As Integer = 0
    ''' <summary>
    ''' 数据区元素类型：0 = double（当前唯一支持的值）
    ''' </summary>
    Public Property DataType As Byte = 0

    Public Property tag As String
    ''' <summary>
    ''' 列数（样本数）
    ''' </summary>
    Public Property Nsamples As Integer
    ''' <summary>
    ''' 行数（特征数）；<c>-1</c> 表示写入方尚未落定行数，这样的文件不完整
    ''' </summary>
    Public Property Nfeatures As Integer = -1
    ''' <summary>
    ''' 压缩分块的行数；<c>0</c> 或负数表示不切块
    ''' </summary>
    Public Property BlockRows As Integer = 0

    Public Property SampleIds As String()
    Public Property FeatureIds As String()

    ''' <summary>
    ''' trailer（块索引所在位置）的绝对文件偏移；<c>-1</c> 表示没有 trailer
    ''' </summary>
    Public Property IndexOffset As Long = -1

    ''' <summary>
    ''' 数据区起始的绝对文件偏移。由读取方根据自身流位置推算，**不会**写进文件
    ''' </summary>
    Public Property DataStart As Long

    ''' <summary>
    ''' 块偏移表（相对 <see cref="DataStart"/>）；<c>Nothing</c> 表示没有索引
    ''' </summary>
    Public Property BlockOffsets As Long()

    ''' <summary>
    ''' 每一行的字节数
    ''' </summary>
    Public ReadOnly Property BlockSize As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Nsamples * RawStream.DblFloat
        End Get
    End Property

    Public ReadOnly Property Codec As BinaryMatrixCodec
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return CType(Flags And CodecMask, BinaryMatrixCodec)
        End Get
    End Property

    Public ReadOnly Property IsCompressed As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Codec <> BinaryMatrixCodec.none
        End Get
    End Property

    Public ReadOnly Property HasBlockIndex As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return (Flags And FlagHasBlockIndex) <> 0
        End Get
    End Property

    Public ReadOnly Property FeatureIdsInTrailer As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return (Flags And FlagFeatureIdsInTrailer) <> 0
        End Get
    End Property

    ''' <summary>
    ''' 行数是否已经落定？
    ''' </summary>
    Public ReadOnly Property IsFinalized As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Nfeatures >= 0
        End Get
    End Property

    ''' <summary>
    ''' 块数量（<see cref="BlockRows"/> 为 0 时视为单块）
    ''' </summary>
    Public ReadOnly Property Nblocks As Integer
        Get
            Dim rows As Integer = If(BlockRows > 0, BlockRows, std.Max(Nfeatures, 1))
            Dim n As Integer = If(Nfeatures <= 0, 0, CInt(std.Ceiling(Nfeatures / CDbl(rows))))

            Return std.Max(n, 1)
        End Get
    End Property

    ''' <summary>
    ''' 头部之中 <see cref="Nfeatures"/> 字段的绝对位置，用于 <c>Complete()</c> 时回填
    ''' </summary>
    Friend Property NfeaturesFieldPosition As Long = -1
    ''' <summary>
    ''' 头部之中 <see cref="IndexOffset"/> 字段的绝对位置，用于 <c>Complete()</c> 时回填
    ''' </summary>
    Friend Property IndexOffsetFieldPosition As Long = -1

    ''' <summary>
    ''' 从流中读取头部，并自动识别 v1/v2 两种格式
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <returns></returns>
    ''' <exception cref="InvalidDataException">magic 不匹配，或者版本号不受支持</exception>
    Public Shared Function Read(reader As BinaryReader) As BinaryMatrixHeader
        Dim stream As Stream = reader.BaseStream
        ' 只进流（zip 的 deflate 流等）不支持读取当前偏移
        Dim start As Long = If(stream.CanSeek, stream.Position, -1)
        Dim bytes As Byte() = reader.ReadBytes(MagicSize)

        If bytes.Length <> MagicSize Then
            Throw New InvalidDataException(
                $"Truncated binary matrix header: {bytes.Length} byte(s) available but {MagicSize} were expected!")
        End If

        Dim magic As String = Encoding.ASCII.GetString(bytes)

        Select Case magic
            Case MagicV1
                Return readV1(reader, start)
            Case MagicV2
                Return readV2(reader, start)
            Case Else
                Throw New InvalidDataException(
                    $"Invalid magic header string '{magic}': neither the v1 magic '{MagicV1}' nor the v2 magic '{MagicV2}' matches!")
        End Select
    End Function

    Private Shared Function readV1(reader As BinaryReader, start As Long) As BinaryMatrixHeader
        Dim mat As New BinaryMatrixHeader With {
            .Version = 1,
            .Flags = 0,
            .DataType = 0,
            .IndexOffset = -1,
            .BlockRows = 0
        }

        mat.tag = reader.ReadString
        mat.Nsamples = reader.ReadInt32
        mat.Nfeatures = reader.ReadInt32
        mat.SampleIds = readIds(reader.ReadString)
        mat.FeatureIds = readIds(reader.ReadString)
        mat.DataStart = dataStartOf(reader.BaseStream)

        Return mat
    End Function

    Private Shared Function readV2(reader As BinaryReader, start As Long) As BinaryMatrixHeader
        Dim mat As New BinaryMatrixHeader()
        Dim version As Integer = reader.ReadInt32

        If version <> 2 Then
            Throw New InvalidDataException(
                $"Unsupported binary matrix format version {version} at offset {start}; only version 1 and 2 are supported!")
        End If

        mat.Version = version
        mat.Flags = reader.ReadInt32
        mat.DataType = reader.ReadByte

        ' 3 bytes reserved
        Call reader.ReadBytes(3)
        mat.IndexOffset = reader.ReadInt64
        mat.tag = reader.ReadString
        mat.Nsamples = reader.ReadInt32
        mat.Nfeatures = reader.ReadInt32
        mat.BlockRows = reader.ReadInt32
        mat.SampleIds = readIds(reader.ReadString)
        mat.FeatureIds = readIds(reader.ReadString)
        mat.DataStart = dataStartOf(reader.BaseStream)

        Return mat
    End Function

    ''' <summary>
    ''' 数据区起点：只进流无法读取偏移，此时返回 0（该值仅在随机访问时才有意义）
    ''' </summary>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function dataStartOf(stream As Stream) As Long
        If stream.CanSeek Then
            Return stream.Position
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' 把头部写入流中（v1 或 v2，取决于 <see cref="Version"/>）
    ''' </summary>
    ''' <param name="writer"></param>
    ''' <param name="header"></param>
    ''' <exception cref="NotSupportedException">v2 的未知行数模式要求流可 seek</exception>
    Public Shared Sub Write(writer As BinaryWriter, header As BinaryMatrixHeader)
        If header.Version >= 2 Then
            writeV2(writer, header)
        Else
            writeV1(writer, header)
        End If

        Call writer.Flush()
    End Sub

    Private Shared Sub writeV1(writer As BinaryWriter, header As BinaryMatrixHeader)
        Dim stream As Stream = writer.BaseStream

        Call writer.Write(Encoding.ASCII.GetBytes(MagicV1))
        Call writer.Write(If(String.IsNullOrEmpty(header.tag), "NA", header.tag))
        Call writer.Write(header.Nsamples)
        Call writer.Write(header.Nfeatures)
        Call writer.Write(header.SampleIds.GetJson)
        Call writer.Write(header.FeatureIds.GetJson)
        Call writer.Flush()

        header.DataStart = dataStartOf(stream)
        header.NfeaturesFieldPosition = -1
        header.IndexOffsetFieldPosition = -1
    End Sub

    Private Shared Sub writeV2(writer As BinaryWriter, header As BinaryMatrixHeader)
        Dim stream As Stream = writer.BaseStream
        Dim flags As Integer = header.Flags And Not CodecMask

        flags = flags Or (CInt(header.Codec) And CodecMask)

        If header.HasBlockIndex Then
            flags = flags Or FlagHasBlockIndex
        Else
            flags = flags And Not FlagHasBlockIndex
        End If

        If header.FeatureIds.IsNullOrEmpty Then
            flags = flags Or FlagFeatureIdsInTrailer
        Else
            flags = flags And Not FlagFeatureIdsInTrailer
        End If

        header.Flags = flags

        Call writer.Write(Encoding.ASCII.GetBytes(MagicV2))
        Call writer.Write(2)
        Call writer.Write(flags)
        Call writer.Write(header.DataType)
        Call writer.Write(New Byte(2) {})    ' 3 bytes reserved

        header.IndexOffsetFieldPosition = stream.Position
        Call writer.Write(header.IndexOffset)

        Call writer.Write(If(String.IsNullOrEmpty(header.tag), "NA", header.tag))
        Call writer.Write(header.Nsamples)

        header.NfeaturesFieldPosition = stream.Position
        Call writer.Write(header.Nfeatures)

        Call writer.Write(header.BlockRows)
        Call writer.Write(header.SampleIds.GetJson)
        Call writer.Write(If(header.FeatureIds.IsNullOrEmpty, "[]", header.FeatureIds.GetJson))
        Call writer.Flush()

        header.DataStart = dataStartOf(stream)
    End Sub

    ''' <summary>
    ''' 把 <see cref="Nfeatures"/> 与 <see cref="IndexOffset"/> 回填到头部（仅 v2 有效）
    ''' </summary>
    ''' <param name="writer"></param>
    ''' <param name="header"></param>
    ''' <remarks>
    ''' 回填之后流的位置会恢复到调用前的位置，因此可以在写完 trailer 之后安全调用。
    ''' </remarks>
    ''' <exception cref="NotSupportedException">流不可 seek</exception>
    Public Shared Sub Patch(writer As BinaryWriter, header As BinaryMatrixHeader)
        Dim stream As Stream = writer.BaseStream

        If Not stream.CanSeek Then
            Throw New NotSupportedException(
                "Patching the binary matrix header requires a seekable stream: " &
                "please pass the total feature count while creating the writer for non-seekable outputs!")
        End If

        Call writer.Flush()

        Dim pos As Long = stream.Position

        If header.IndexOffsetFieldPosition >= 0 Then
            stream.Position = header.IndexOffsetFieldPosition
            Call writer.Write(header.IndexOffset)
        End If

        If header.NfeaturesFieldPosition >= 0 Then
            stream.Position = header.NfeaturesFieldPosition
            Call writer.Write(header.Nfeatures)
        End If

        Call writer.Flush()

        stream.Position = pos
    End Sub

    ''' <summary>
    ''' 读取 trailer 之中的块索引与（可选的）特征名列表
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <returns>是否成功读取到索引</returns>
    ''' <remarks>
    ''' 只对可 seek 的流有效：非 seek 流（例如 zip 的 deflate 流）读到数据区之后就无法回到
    ''' trailer 的位置了，此时返回 <c>False</c>，调用方应当降级为顺序读取。
    ''' </remarks>
    Public Function LoadTrailer(reader As BinaryReader) As Boolean
        If IndexOffset < 0 Then
            Return False
        End If

        Dim stream As Stream = reader.BaseStream

        If Not stream.CanSeek Then
            Return False
        End If

        Dim pos As Long = stream.Position

        Try
            stream.Position = IndexOffset

            Dim nblocks As Long = reader.ReadInt64

            If nblocks < 0 OrElse nblocks > Integer.MaxValue \ 16 Then
                Throw New InvalidDataException($"Invalid block count {nblocks} in the binary matrix trailer!")
            End If

            If nblocks = 0 Then
                ' trailer 里只有特征名列表（不压缩但行数未知的文件）
                Me.BlockOffsets = Nothing
            Else
                Dim offsets(nblocks) As Long

                For i As Integer = 0 To offsets.Length - 1
                    offsets(i) = reader.ReadInt64
                Next

                Me.BlockOffsets = offsets
            End If

            If FeatureIdsInTrailer Then
                Me.FeatureIds = readIds(reader.ReadString)
            End If

            Return True
        Finally
            stream.Position = pos
        End Try
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function readIds(str As String) As String()
        If String.IsNullOrEmpty(str) Then
            Return New String() {}
        Else
            Return str.LoadJSON(Of String())
        End If
    End Function

    Public Overrides Function ToString() As String
        Return $"HTS_bin/v{Version} [{Nsamples}x{Nfeatures}] codec={Codec} blocks={If(HasBlockIndex, Nblocks.ToString, "-")} tag={tag}"
    End Function

End Class
