Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports BinaryMatrixCodec = Microsoft.VisualBasic.Serialization.BinaryDumping.NetworkByteOrderBuffer.Compression

''' <summary>
''' data encoded in network byte order
''' </summary>
''' <remarks>
''' <para>
''' 这一层是二进制矩阵的门面（facade）：<see cref="Save"/> / <see cref="LoadStream"/>
''' 面向"整表已经在内存里"的调用方，内部转交给流式的 <see cref="HTSMatrixWriter"/>
''' 与 <see cref="HTSMatrixReader"/>；需要按需读取超大型文件时请直接使用
''' <see cref="Open(Stream)"/> 拿到惰性读取器。
''' </para>
''' <para>
''' 格式本身（v1/v2 头部）的唯一实现是 <see cref="BinaryMatrixHeader"/>，
''' 本模块只保留 <see cref="MagicHeader"/> 常量供既有代码（例如 PLAS.NET 的归档读写）引用。
''' </para>
''' </remarks>
Public Module BinaryMatrix

    Friend ReadOnly magic As Byte()
    ReadOnly bin As New NetworkByteOrderBuffer

    Public Const MagicHeader As String = "GCModeller/HTS_matrix"

    Sub New()
        magic = Encoding.ASCII.GetBytes(MagicHeader)
    End Sub

    ''' <summary>
    ''' 把整张二进制矩阵读进内存（v1 与 v2 都会被自动识别）
    ''' </summary>
    ''' <param name="file">矩阵文件流；本方法结束时会关闭它</param>
    ''' <remarks>
    ''' 超大型矩阵请改用 <see cref="Open(Stream)"/> 返回的 <see cref="HTSMatrixReader"/>，
    ''' 否则这一步本身就是一次全量内存分配。
    ''' </remarks>
    Public Function LoadStream(file As Stream) As Matrix
        Using reader As New HTSMatrixReader(file)
            Return reader.AsMatrix()
        End Using
    End Function

    ''' <summary>
    ''' 打开一个惰性读取器（不加载数据，只读取头部与块索引）
    ''' </summary>
    ''' <param name="file">矩阵文件流；读取器释放时该流也会被关闭</param>
    Public Function Open(file As Stream) As HTSMatrixReader
        Return New HTSMatrixReader(file)
    End Function

    ''' <summary>
    ''' 打开一个惰性读取器（随机访问模式）
    ''' </summary>
    ''' <param name="path">矩阵文件路径</param>
    Public Function Open(path As String) As HTSMatrixReader
        Dim fs As New FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize:=64 * 1024,
            options:=FileOptions.RandomAccess
        )

        Return New HTSMatrixReader(fs)
    End Function

    ''' <summary>
    ''' 保存 HTS 矩阵对象为二进制文件（默认写出 v2 格式）
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="file">输出流；默认在结束时关闭它（保持旧版本语义）</param>
    ''' <param name="codec">压缩方式</param>
    ''' <param name="blockRows">压缩分块的行数，0 表示使用默认值</param>
    ''' <param name="version">格式版本：1 = 旧格式，2 = 当前格式</param>
    ''' <param name="leaveOpen">结束时是否保持流可用</param>
    ''' <returns></returns>
    <Extension>
    Public Function Save(mat As Matrix,
                         file As Stream,
                         Optional codec As BinaryMatrixCodec = BinaryMatrixCodec.none,
                         Optional blockRows As Integer = 0,
                         Optional version As Integer = 2,
                         Optional leaveOpen As Boolean = False) As Boolean

        Using writer As HTSMatrixWriter = HTSMatrixWriter.Create(
            stream:=file,
            sampleIDs:=mat.sampleID,
            featureIDs:=mat.rownames,
            tag:=mat.tag,
            codec:=codec,
            blockRows:=blockRows,
            nfeatures:=mat.size,
            version:=version,
            leaveOpen:=leaveOpen
        )

            Call writer.WriteRows(mat.expression)
            Call writer.Complete()
        End Using

        Return True
    End Function

    ''' <summary>
    ''' 保存 HTS 矩阵对象为二进制文件（先写临时文件，成功后原子替换）
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="path"></param>
    ''' <param name="codec"></param>
    ''' <param name="blockRows"></param>
    ''' <param name="version"></param>
    <Extension>
    Public Function SaveFile(mat As Matrix,
                             path As String,
                             Optional codec As BinaryMatrixCodec = BinaryMatrixCodec.none,
                             Optional blockRows As Integer = 0,
                             Optional version As Integer = 2) As Boolean

        Using writer As HTSMatrixWriter = HTSMatrixWriter.CreateFile(
            path:=path,
            sampleIDs:=mat.sampleID,
            featureIDs:=mat.rownames,
            tag:=mat.tag,
            codec:=codec,
            blockRows:=blockRows,
            nfeatures:=mat.size,
            version:=version
        )

            Call writer.WriteRows(mat.expression)
            Call writer.Complete()
        End Using

        Return True
    End Function

End Module
