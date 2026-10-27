Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace KOBAS

    ''' <summary>
    ''' 富集结果的整理与输出（KOBAS <c>src/kobas/output.py</c> 之中纯算法部分的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' KOBAS 的 <c>identify</c> 输出包含 Web 超链接（<c>http://www.genome.jp/...</c>），
    ''' 属于网络服务相关的内容，这里不予翻译；本模块只保留：
    ''' <list type="bullet">
    ''' <item>按「通路名称的实义词集合」对结果做去重分组（重复项加上 <c>@@</c> 前缀）；</item>
    ''' <item>结果表的排序与制表符分隔导出。</item>
    ''' </list>
    ''' </remarks>
    Public Module IdentifyOutput

        ''' <summary>
        ''' 通路名称去重时所忽略的停用词表（与 KOBAS <c>output.STOPLIST</c> 一致）。
        ''' </summary>
        Public ReadOnly Property StopList As HashSet(Of String) = New HashSet(Of String)(
            {
                "of", "a", "an", "to", "on", "and", "with", "or", "not", "syndrome",
                "by", "&", "in", "progression", "the", "recurrent", "-", "disease", "secondary",
                "and/or", "due", "as", "syndrome", "traits", "chronic", "familial", "type", "1",
                "2", "3", "4", "5", "6", "7", "8", "primary", "disorder"
            }, StringComparer.Ordinal)

        ''' <summary>
        ''' 对结果行按通路名称的实义词集合去重分组。
        ''' </summary>
        ''' <param name="rows">富集结果行。</param>
        ''' <returns>
        ''' 去重之后的显示用结果行；与前面某一行实义词集合完全相同的行，
        ''' 其 <see cref="EnrichmentRow.Term"/> 会被追加 <c>@@</c> 前缀并紧跟在该行之后。
        ''' </returns>
        <Extension>
        Public Function Group(rows As IEnumerable(Of EnrichmentRow)) As EnrichmentRow()
            Dim keys As New List(Of HashSet(Of String))
            Dim display As New List(Of EnrichmentRow)

            For Each row As EnrichmentRow In rows
                Dim name As New HashSet(Of String)(
                    If(row.Term, "") _
                        .ToLowerInvariant() _
                        .Split(New Char() {" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries) _
                        .Where(Function(word) Not StopList.Contains(word)),
                    StringComparer.Ordinal)
                Dim index As Integer = IndexOfSet(keys, name)

                If index < 0 Then
                    keys.Add(name)
                    display.Add(row)
                Else
                    keys.Insert(index, New HashSet(Of String)(StringComparer.Ordinal))
                    display.Insert(index + 1, CloneWithTerm(row, "@@" & row.Term))
                End If
            Next

            Return display.ToArray
        End Function

        ''' <summary>
        ''' 在集合列表之中查找与给定集合内容相等的第一个位置。
        ''' </summary>
        ''' <param name="keys">集合列表。</param>
        ''' <param name="name">待查找的集合。</param>
        ''' <returns>下标，未找到时返回 -1。</returns>
        Private Function IndexOfSet(keys As List(Of HashSet(Of String)), name As HashSet(Of String)) As Integer
            For i As Integer = 0 To keys.Count - 1
                If keys(i).SetEquals(name) Then
                    Return i
                End If
            Next

            Return -1
        End Function

        ''' <summary>
        ''' 复制一行结果并替换词条名称。
        ''' </summary>
        Private Function CloneWithTerm(row As EnrichmentRow, term As String) As EnrichmentRow
            Return New EnrichmentRow With {
                .Term = term,
                .Database = row.Database,
                .ID = row.ID,
                .InputNumber = row.InputNumber,
                .BackgroundNumber = row.BackgroundNumber,
                .PValue = row.PValue,
                .CorrectedPValue = row.CorrectedPValue,
                .Input = row.Input
            }
        End Function

        ''' <summary>
        ''' 把整理后的结果表写出为制表符分隔文件。
        ''' </summary>
        ''' <param name="rows">富集结果行。</param>
        ''' <param name="outputFile">输出文件路径。</param>
        ''' <param name="method">FDR 校正方法；为 <c>Nothing</c> 时跳过校正。</param>
        ''' <returns>写出的文本内容。</returns>
        Public Function Write(rows As IEnumerable(Of EnrichmentRow),
                              outputFile As String,
                              Optional method As FdrMethod? = FdrMethod.BH) As String

            Dim table As New EnrichmentTable(rows)

            If method IsNot Nothing Then
                Call table.AddFdr(method.Value)
            End If

            Call table.SortByPValue()

            Dim text As String = table.ToTsv

            Call File.WriteAllText(outputFile, text)

            Return text
        End Function
    End Module
End Namespace
