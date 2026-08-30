' ============================================================================
' FastaReader.vb — 流式 FASTA 读取器
' ----------------------------------------------------------------------------
' 逐条读取 FASTA 文件，不在内存中保留全部序列。
' 支持多行序列、空行跳过、超大文件流式处理。
'
' 核心设计：
'   - 使用 StreamReader 逐行读取，每次只保留一条序列在内存中
'   - 预读下一行判断是否为新的序列头（以 '>' 开头）
'   - 支持 IDisposable，确保文件句柄正确释放
'
' 适用场景：100GB 级 FASTA 文件，十亿条序列
' ============================================================================

Imports System.IO
Imports System.Text

Namespace Core

    ''' <summary>单条 FASTA 记录</summary>
    Public Class FastaRecord

        ''' <summary>序列头（不含 '>' 字符）</summary>
        Public Property Header As String

        ''' <summary>氨基酸序列（已去除换行）</summary>
        Public Property Sequence As String

        Public Overrides Function ToString() As String
            Dim headerPreview As String = If(Header?.Length > 30, Header.Substring(0, 30) & "...", Header)
            Return $">{headerPreview} [{If(Sequence?.Length, 0)} aa]"
        End Function
    End Class

    ''' <summary>
    ''' 流式 FASTA 读取器：逐条读取，内存占用恒定（仅当前序列）。
    ''' </summary>
    Public Class FastaReader
        Implements IDisposable

        Private ReadOnly _reader As StreamReader
        Private ReadOnly _filePath As String
        Private _nextHeaderLine As String
        Private _disposed As Boolean = False

        ''' <summary>已读取的字节数（用于进度报告）</summary>
        Private _bytesRead As Long = 0

        ''' <summary>已读取的序列数</summary>
        Private _recordCount As Long = 0

        ''' <summary>文件总大小（字节）</summary>
        Public ReadOnly Property FileSize As Long

        ''' <summary>已读取的字节数</summary>
        Public ReadOnly Property BytesRead As Long
            Get
                Return _bytesRead
            End Get
        End Property

        ''' <summary>已读取的序列数</summary>
        Public ReadOnly Property RecordCount As Long
            Get
                Return _recordCount
            End Get
        End Property

        ''' <summary>
        ''' 构造函数
        ''' </summary>
        ''' <param name="filePath">FASTA 文件路径</param>
        Public Sub New(filePath As String)
            _filePath = filePath
            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException($"FASTA 文件不存在: {filePath}")
            End If

            Dim fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize:=1 << 20)
            _reader = New StreamReader(fs, Encoding.ASCII)
            _nextHeaderLine = Nothing

            ' 获取文件大小用于进度计算
            FileSize = New FileInfo(filePath).Length
        End Sub

        ''' <summary>
        ''' 读取下一条 FASTA 记录。
        ''' </summary>
        ''' <returns>FastaRecord 或 Nothing（文件结束）</returns>
        Public Function ReadNext() As FastaRecord
            Dim header As String = Nothing
            Dim sb As New StringBuilder(8192)

            ' ---- 1. 获取序列头 ----
            If _nextHeaderLine IsNot Nothing Then
                ' 使用上次预读的头行
                header = _nextHeaderLine
                _nextHeaderLine = Nothing
            Else
                ' 从流中读取
                Dim line = _reader.ReadLine()
                Do While line IsNot Nothing
                    _bytesRead += line.Length + 1 ' +1 for newline
                    If line.Length = 0 Then
                        ' 跳过空行
                        line = _reader.ReadLine()
                        Continue Do
                    End If
                    If line.StartsWith(">"c) Then
                        header = line.Substring(1) ' 去掉 '>'
                        Exit Do
                    Else
                        ' 文件开头不是 '>'，跳过无效行
                        line = _reader.ReadLine()
                    End If
                Loop

                If header Is Nothing Then Return Nothing ' 文件结束
            End If

            ' ---- 2. 读取序列行，直到遇到下一个 '>' 或文件结束 ----
            Dim nextLine = _reader.ReadLine()
            Do While nextLine IsNot Nothing
                _bytesRead += nextLine.Length + 1
                If nextLine.StartsWith(">"c) Then
                    ' 预读到了下一条序列的头行，保存供下次使用
                    _nextHeaderLine = nextLine.Substring(1) ' 去掉 '>'
                    Exit Do
                End If
                If nextLine.Length > 0 Then
                    sb.Append(nextLine)
                End If
                nextLine = _reader.ReadLine()
            Loop

            _recordCount += 1
            Return New FastaRecord With {
                .Header = header,
                .Sequence = sb.ToString()
            }
        End Function

        ''' <summary>读取进度百分比 (0~100)</summary>
        Public ReadOnly Property Progress As Double
            Get
                If FileSize = 0 Then Return 0.0
                Return Math.Min(100.0, _bytesRead * 100.0 / FileSize)
            End Get
        End Property

        ''' <summary>释放资源</summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then Return
            _reader?.Dispose()
            _disposed = True
        End Sub

    End Class

End Namespace
