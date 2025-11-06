Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.MIME.application.json

' ============================================================================
' 1. 数据结构类
' ============================================================================

''' <summary>
''' 代表一个完整的 Delta 文件，包含文件头和所有比对块。
''' </summary>
Public Class DeltaFile

    Public Property Header As DeltaHeader
    Public Property Alignments As AlignmentBlock()

    Default Public ReadOnly Property Item(i As Integer) As AlignmentBlock
        Get
            Return _Alignments(i)
        End Get
    End Property

    Public Sub PrintAlignment(text As TextWriter, Optional n As Integer = 5)
        Call DebugPrint.BuildString(Me, text, n)
    End Sub

    Public Overrides Function ToString() As String
        Dim sb As New StringBuilder
        Dim writer As New StringWriter(sb)

        Call PrintAlignment(writer)
        Call writer.Flush()

        Return sb.ToString
    End Function

    Public Shared Function LoadDocument(file As String) As DeltaFile
        Return NucmerDeltaParser.Parse(file)
    End Function

End Class

''' <summary>
''' 代表 Delta 文件的头部信息。
''' </summary>
Public Class DeltaHeader
    Public Property ReferenceFilePath As String
    Public Property QueryFilePath As String
    Public Property Tool As String
    Public Property ReferenceId As String
    Public Property QueryId As String
    Public Property ReferenceLength As Long
    Public Property QueryLength As Long
End Class

''' <summary>
''' 代表一个单独的比对块。
''' </summary>
Public Class AlignmentBlock
    ''' <summary>
    ''' 参考序列上的起始位置 (1-based inclusive)。
    ''' </summary>
    Public Property RStart As Long

    ''' <summary>
    ''' 参考序列上的结束位置 (1-based inclusive)。
    ''' </summary>
    Public Property REnd As Long

    ''' <summary>
    ''' 查询序列上的起始位置 (1-based inclusive)。
    ''' </summary>
    Public Property QStart As Long

    ''' <summary>
    ''' 查询序列上的结束位置 (1-based inclusive)。
    ''' </summary>
    Public Property QEnd As Long

    ''' <summary>
    ''' 比对区域内的错误总数 (错配 + 插入缺失)。
    ''' </summary>
    Public Property Errors As Long

    ''' <summary>
    ''' 比对区域内相似碱基的数量。
    ''' </summary>
    Public Property Similarity As Long

    ''' <summary>
    ''' 终止符，通常为 0。
    ''' </summary>
    Public Property [Stop] As Long

    ''' <summary>
    ''' 描述该比对区域内具体差异的整数列表。
    ''' </summary>
    Public Property Deltas As Long()

    ''' <summary>
    ''' 获取一个值，指示该比对是否为反向互补比对。
    ''' 如果 QStart > QEnd，则为反向互补。
    ''' </summary>
    Public ReadOnly Property IsReverseComplement As Boolean
        Get
            Return QStart > QEnd
        End Get
    End Property

    ''' <summary>
    ''' 获取参考序列上比对区域的长度。
    ''' </summary>
    Public ReadOnly Property ReferenceAlignmentLength As Long
        Get
            Return Math.Abs(REnd - RStart) + 1
        End Get
    End Property

    ''' <summary>
    ''' 获取查询序列上比对区域的长度。
    ''' </summary>
    Public ReadOnly Property QueryAlignmentLength As Long
        Get
            Return Math.Abs(QEnd - QStart) + 1
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Deltas.GetJson
    End Function
End Class


' ============================================================================
' 2. 解析器类
' ============================================================================

''' <summary>
''' 用于解析 Nucmer Delta 文件的工具类。
''' </summary>
Public Class NucmerDeltaParser

    ''' <summary>
    ''' 解析指定路径的 Delta 文件。
    ''' </summary>
    ''' <param name="filePath">Delta 文件的完整路径。</param>
    ''' <returns>一个包含解析后数据的 DeltaFile 对象，如果解析失败则返回 Nothing。</returns>
    Public Shared Function Parse(filePath As String) As DeltaFile
        Dim lines = filePath.ReadAllLines

        If lines.Length < 3 Then
            Console.Error.WriteLine("错误: 文件格式不正确，行数不足。")
            Return Nothing
        End If

        Dim result As New DeltaFile()

        ' --- 1. 解析文件头 ---
        Try
            result.Header = ParseHeader(lines)
            result.Alignments = ParseAlignment(lines).ToArray
        Catch ex As Exception
            Console.Error.WriteLine($"错误: 解析文件头失败. {ex.Message}")
            Return Nothing
        End Try

        Return result
    End Function

    Private Shared Iterator Function ParseAlignment(lines As String()) As IEnumerable(Of AlignmentBlock)
        ' --- 2. 解析比对块 ---
        Dim i As Integer = 3 ' 从第4行开始 (索引为3)
        While i < lines.Length
            Dim line As String = lines(i).Trim()

            ' 跳过空行
            If String.IsNullOrWhiteSpace(line) Then
                i += 1
                Continue While
            End If

            ' 检查是否是坐标行 (包含7个由空格分隔的整数)
            Dim parts As String() = line.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
            If parts.Length = 7 AndAlso parts.All(Function(p) Long.TryParse(p, Nothing)) Then
                Dim alignment As AlignmentBlock = ParseAlignmentBlock(lines, i)

                If alignment IsNot Nothing Then
                    Yield alignment
                End If
            Else
                ' 如果不是坐标行，继续向下搜索
                i += 1
            End If
        End While
    End Function

    ''' <summary>
    ''' 从文件行列表中解析文件头。
    ''' </summary>
    Private Shared Function ParseHeader(ByRef lines As String()) As DeltaHeader
        Dim header As New DeltaHeader()

        ' 第1行: 文件路径
        Dim pathParts = lines(0).Split({" "c}, 2)
        header.ReferenceFilePath = pathParts(0)
        header.QueryFilePath = If(pathParts.Length > 1, pathParts(1), "")

        ' 第2行: 工具名称
        header.Tool = lines(1).Trim()

        ' 第3行: ID 和长度
        Dim idParts = lines(2).Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
        If idParts.Length < 4 Then
            Throw New FormatException("Header ID line is malformed.")
        End If
        header.ReferenceId = idParts(0).Substring(1) ' 移除开头的 '>'
        header.QueryId = idParts(1)
        header.ReferenceLength = Long.Parse(idParts(2))
        header.QueryLength = Long.Parse(idParts(3))

        Return header
    End Function

    ''' <summary>
    ''' 从指定行开始解析一个比对块。
    ''' </summary>
    Private Shared Function ParseAlignmentBlock(ByRef lines As String(), ByRef startIndex As Integer) As AlignmentBlock
        Dim block As New AlignmentBlock()
        Dim deltas As New List(Of Long)()

        ' 解析坐标行
        Dim coordParts = lines(startIndex).Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
        block.RStart = Long.Parse(coordParts(0))
        block.REnd = Long.Parse(coordParts(1))
        block.QStart = Long.Parse(coordParts(2))
        block.QEnd = Long.Parse(coordParts(3))
        block.Errors = Long.Parse(coordParts(4))
        block.Similarity = Long.Parse(coordParts(5))
        block.Stop = Long.Parse(coordParts(6))

        ' 解析后续的差异值行
        Dim i As Integer = startIndex + 1
        While i < lines.Length
            Dim deltaLineParts = lines(i).Trim().Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

            If deltaLineParts.Length = 1 Then
                deltas.Add(Long.Parse(deltaLineParts(0)))
                i += 1
            Else
                Exit While
            End If
        End While

        block.Deltas = deltas.ToArray
        startIndex = i

        Return block
    End Function
End Class

Module DebugPrint

    Public Sub BuildString(deltaFile As DeltaFile, ByRef text As TextWriter, n As Integer)
        ' --- 打印文件头信息 ---
        Call text.WriteLine("文件头信息:")
        Call text.WriteLine($"  参考序列: {deltaFile.Header.ReferenceId} (长度: {deltaFile.Header.ReferenceLength:N0} bp)")
        Call text.WriteLine($"  查询序列: {deltaFile.Header.QueryId} (长度: {deltaFile.Header.QueryLength:N0} bp)")
        Call text.WriteLine($"  使用工具: {deltaFile.Header.Tool}")
        Call text.WriteLine()

        ' --- 打印比对块摘要 ---
        Call text.WriteLine($"共找到 {deltaFile.Alignments.Count} 个比对块。")
        Call text.WriteLine("-------------------------------------")

        ' 只打印前5个比对块的详细信息作为示例
        For i As Integer = 0 To Math.Min(n - 1, deltaFile.Alignments.Count - 1)
            Dim alignment As AlignmentBlock = deltaFile(i)

            Call text.WriteLine($"比对块 #{i + 1}:")
            Call text.WriteLine($"  参考坐标: {alignment.RStart:N0} - {alignment.REnd:N0} (长度: {alignment.ReferenceAlignmentLength:N0})")
            Call text.WriteLine($"  查询坐标: {alignment.QStart:N0} - {alignment.QEnd:N0} (长度: {alignment.QueryAlignmentLength:N0})")
            Call text.WriteLine($"  方向: {If(alignment.IsReverseComplement, "反向互补", "正向")}")
            Call text.WriteLine($"  错误/相似: {alignment.Errors} / {alignment.Similarity}")

            ' 分析差异值
            Dim insertions = alignment.Deltas.Where(Function(d) d > 0).Sum()
            Dim deletions = Math.Abs(alignment.Deltas.Where(Function(d) d < 0).Sum())
            Dim mismatches = alignment.Deltas.Where(Function(d) d = 0).Count()

            Call text.WriteLine($"  差异摘要: 插入={insertions} bp, 缺失={deletions} bp, 错配={mismatches} 个")
            Call text.WriteLine()
        Next

        If deltaFile.Alignments.Count > n Then
            Call text.WriteLine("... (其余比对块已省略) ...")
        End If
    End Sub
End Module