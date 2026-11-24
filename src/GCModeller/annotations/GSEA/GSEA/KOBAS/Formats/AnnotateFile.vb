Imports System.Globalization
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace KOBAS

    ''' <summary>
    ''' KOBAS annotate 输出文件之中的一行注释记录。
    ''' </summary>
    ''' <remarks>
    ''' 对应 KOBAS 3.0 <c>src/kobas/annot.py</c> 的 <c>Annotation</c> 类型：
    ''' <c>query</c> 为输入基因编号，<c>links</c> 为该基因被注释到的数据库条目编号。
    ''' </remarks>
    Public Class AnnotationEntry

        ''' <summary>
        ''' 输入文件之中待注释的基因编号。
        ''' </summary>
        ''' <returns></returns>
        Public Property Query As String

        ''' <summary>
        ''' 注释到的数据库条目编号（KEGG 基因编号、KO 编号、GO 编号等）。
        ''' </summary>
        ''' <returns></returns>
        Public Property Links As String()

        ''' <summary>
        ''' 当前基因是否被注释到了至少一个数据库条目。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property HasLinks As Boolean
            Get
                Return Links IsNot Nothing AndAlso Links.Length > 0
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Query} -> {Links.JoinBy(", ")}"
        End Function
    End Class

    ''' <summary>
    ''' KOBAS annotate 输出文件的头部元信息。
    ''' </summary>
    Public Class AnnotateHeader

        ''' <summary>
        ''' 物种缩写，例如 <c>hsa</c>、<c>ko</c>。
        ''' </summary>
        ''' <returns></returns>
        Public Property Species As String

        ''' <summary>
        ''' 注释方法（BLAST 参数或者 ID mapping 类型）。
        ''' </summary>
        ''' <returns></returns>
        Public Property Method As String

        ''' <summary>
        ''' 注释时所使用的 ID 类型，默认 <c>entrez_gene_id</c>。
        ''' </summary>
        ''' <returns></returns>
        Public Property IdMapping As String = "entrez_gene_id"

        ''' <summary>
        ''' 注释成功的基因数量（来自 <c>##Summary</c> 行）。
        ''' </summary>
        ''' <returns></returns>
        Public Property AnnotatedGenes As Integer
    End Class

    ''' <summary>
    ''' KOBAS annotate 输出文件的解析结果。
    ''' </summary>
    Public Class AnnotateDocument

        ''' <summary>
        ''' 头部元信息。
        ''' </summary>
        ''' <returns></returns>
        Public Property Header As AnnotateHeader

        ''' <summary>
        ''' 注释记录集合。
        ''' </summary>
        ''' <returns></returns>
        Public Property Entries As AnnotationEntry()
    End Class

    ''' <summary>
    ''' KOBAS annotate 输出文件的解析器。
    ''' </summary>
    ''' <remarks>
    ''' 本模块是 KOBAS 3.0 <c>src/kobas/annot.py</c> 之中 <c>get_args</c>、
    ''' <c>parse_annot</c> 与 <c>Iterator</c> 的 VB.NET 移植。
    ''' 文件布局如下：
    ''' <code>
    ''' ##hsa	Homo sapiens
    ''' ##Method: ID mapping (id:ncbigene)
    ''' ##Summary:	1234 succeed, 10 fail
    ''' #Query	Gene ID|Gene name|Hyperlink
    ''' TP53	7157|P53|http://...
    ''' ...
    ''' --------------------
    ''' </code>
    ''' 与 KOBAS 原实现的差异：不解析超链接列，仅保留条目编号；
    ''' 注释记录的迭代在遇到连续 20 个 <c>-</c> 的分隔行时结束。
    ''' </remarks>
    Public Module AnnotateFile

        ''' <summary>
        ''' 连续 20 个 <c>-</c> 构成的分段分隔行。
        ''' </summary>
        Public ReadOnly Property Separator As String = New String("-"c, 20)

        ''' <summary>
        ''' 读取并解析 KOBAS annotate 输出文件。
        ''' </summary>
        ''' <param name="annotateFile">annotate 输出文件路径。</param>
        ''' <returns>解析结果（头部信息与注释记录）。</returns>
        ''' <exception cref="FormatException">注释行的列数不足时抛出。</exception>
        Public Function Load(annotateFile As String) As AnnotateDocument
            Return Parse(File.ReadLines(annotateFile))
        End Function

        ''' <summary>
        ''' 解析 KOBAS annotate 输出的文本行。
        ''' </summary>
        ''' <param name="lines">annotate 输出的文本行。</param>
        ''' <returns>解析结果（头部信息与注释记录）。</returns>
        Public Function Parse(lines As IEnumerable(Of String)) As AnnotateDocument
            Dim buffer As String() = lines.ToArray
            Dim header As AnnotateHeader = ReadHeader(buffer)
            Dim entries As New List(Of AnnotationEntry)

            ' 跳过前三行的头部信息
            For i As Integer = CInt(Math.Min(3, buffer.Length)) To buffer.Length - 1
                Dim text As String = buffer(i)

                If text Is Nothing Then
                    Continue For
                End If

                If text.StartsWith(Separator) Then
                    Exit For
                End If

                Dim trimmed As String = text.Trim()

                If trimmed.Length = 0 OrElse trimmed.StartsWith("#") Then
                    Continue For
                End If

                entries.Add(ParseEntry(trimmed))
            Next

            Return New AnnotateDocument With {
                .Header = header,
                .Entries = entries.ToArray
            }
        End Function

        ''' <summary>
        ''' 读取 annotate 文件的头部元信息。
        ''' </summary>
        ''' <param name="lines">annotate 输出的文本行（至少前三行）。</param>
        ''' <returns>头部元信息。</returns>
        Public Function ReadHeader(lines As String()) As AnnotateHeader
            Dim header As New AnnotateHeader

            If lines.Length > 0 Then
                Dim words As String() = lines(0).Trim().Split(ControlChars.Tab, " "c) _
                    .Where(Function(w) Not String.IsNullOrEmpty(w)) _
                    .ToArray

                If words.Length > 0 AndAlso words(0).StartsWith("##") Then
                    header.Species = words(0).Substring(2)
                End If
            End If

            If lines.Length > 1 Then
                Dim second As String = lines(1).Trim()

                If second.StartsWith("##Method:") Then
                    header.Method = second.Substring("##Method:".Length).Trim()

                    If second.Contains("ID mapping") Then
                        ' ##Method: ID mapping (id:ncbigene)
                        Dim open As Integer = second.LastIndexOf("("c)
                        Dim close As Integer = second.LastIndexOf(")"c)

                        If open >= 0 AndAlso close > open Then
                            header.IdMapping = second.Substring(open + 1, close - open - 1)
                        End If
                    End If
                End If
            End If

            If lines.Length > 2 Then
                Dim third As String = lines(2).Trim()

                If third.StartsWith("##Summary") Then
                    Dim parts As String() = third.Split(ControlChars.Tab)

                    If parts.Length > 1 Then
                        Dim number As String = parts(1).Trim().Split(" "c)(0)
                        Dim value As Integer

                        If Integer.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, value) Then
                            header.AnnotatedGenes = value
                        End If
                    End If
                End If
            End If

            Return header
        End Function

        ''' <summary>
        ''' 解析一行注释记录：<c>query \t link1|name1|url!link2|name2|url</c> 或者 <c>query \t None</c>。
        ''' </summary>
        ''' <param name="line">注释记录行。</param>
        ''' <returns>解析得到的注释记录。</returns>
        ''' <exception cref="FormatException">该行不是两列的制表符分隔格式时抛出。</exception>
        Private Function ParseEntry(line As String) As AnnotationEntry
            Dim words As String() = line.Split(ControlChars.Tab)

            If words.Length < 2 Then
                Throw New FormatException(
                    "Bad format of the input file. You may do annotation first or check your input file. " &
                    "The input file should be the output of annotate." &
                    $"{vbCrLf}invalid line: {line}")
            End If

            Dim query As String = words(0).Trim()
            Dim links As String()

            If words(1).Trim() = "None" Then
                links = New String() {}
            Else
                links = words(1) _
                    .Split("!"c) _
                    .Select(Function(item) item.Split("|"c)(0).Trim()) _
                    .Where(Function(id) Not String.IsNullOrEmpty(id)) _
                    .ToArray
            End If

            Return New AnnotationEntry With {
                .Query = query,
                .Links = links
            }
        End Function

        ''' <summary>
        ''' 将注释记录投影成「基因编号 → 注释条目编号集合」的字典。
        ''' </summary>
        ''' <param name="entries">注释记录集合。</param>
        ''' <returns>只包含 <see cref="AnnotationEntry.HasLinks"/> 为真的记录。</returns>
        <Extension>
        Public Function ToLinksTable(entries As IEnumerable(Of AnnotationEntry)) As Dictionary(Of String, String())
            Return entries _
                .Where(Function(entry) entry.HasLinks) _
                .GroupBy(Function(entry) entry.Query) _
                .ToDictionary(Function(g) g.Key,
                              Function(g) g.SelectMany(Function(entry) entry.Links).Distinct().ToArray)
        End Function
    End Module
End Namespace
