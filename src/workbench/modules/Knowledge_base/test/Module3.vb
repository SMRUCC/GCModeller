Imports System.Text.RegularExpressions
Imports System.Collections.Generic

' 用于存放提取结果的简单结构
Public Structure GeneMatch
    Public Property Identifier As String
    Public Property Type As String ' "GeneID" 或 "GeneName"
End Structure

''' <summary>
''' 提供从文本中提取基因ID和基因名称的功能。
''' </summary>
Public Class GeneIdentifierExtractor
    ' 定义用于匹配基因ID和名称的正则表达式模式
    ' 模式解释：
    ' \b             : 单词边界，确保匹配的是独立的单词，而不是其他单词的一部分（如 "someBRCA1word" 中的 "BRCA1" 不会被匹配）。
    ' (              : 开始一个捕获组。
    '   [A-Z]{2,}\d+ : 匹配基因ID，例如 BRCA1, ATDC5。至少两个大写字母后跟一个或多个数字。
    '   |             : 或
    '   [A-Za-z]{1,2}[tTgG]\d+ : 匹配模式生物的基因ID，例如 AT1G01010 (拟南芥), Os03g0123456 (水稻)。1-2个字母 + t/g + 数字。
    '   |             : 或
    '   [A-Za-z]+[0-9]+ : 匹配基因名称，例如 p53, Oct4, Sox2。字母和数字的组合。
    ' )              : 结束捕获组。
    ' \b             : 单词边界。
    Private Const GenePattern As String = "\b([A-Z]{2,}\d+|[A-Za-z]{1,2}[tTgG]\d+|[A-Za-z]+[0-9]+)\b"

    ' 使用 RegexOptions.IgnoreCase 使匹配不区分大小写，以便匹配 "p53" 和 "P53"
    Private Shared ReadOnly RegexOptions As RegexOptions = RegexOptions.IgnoreCase

    ''' <summary>
    ''' 从输入文本中提取所有潜在的基因ID和基因名称。
    ''' </summary>
    ''' <param name="text">要分析的英文文献文本。</param>
    ''' <returns>一个包含所有匹配到的基因标识符的列表。</returns>
    Public Function ExtractGenes(text As String) As List(Of String)
        If String.IsNullOrWhiteSpace(text) Then
            Return New List(Of String)()
        End If

        ' 使用 HashSet 来自动处理重复项
        Dim foundGenes As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Try
            Dim matches As MatchCollection = Regex.Matches(text, GenePattern, RegexOptions)

            For Each match As Match In matches
                ' match.Groups(1).Value 是我们捕获组里的内容
                foundGenes.Add(match.Groups(1).Value)
            Next
        Catch ex As ArgumentException
            ' 处理正则表达式模式错误（虽然这里模式是固定的，但这是个好习惯）
            Console.WriteLine($"正则表达式模式错误: {ex.Message}")
            Return New List(Of String)()
        End Try

        Return foundGenes.ToList()
    End Function

    ''' <summary>
    ''' 从输入文本中提取基因标识符，并尝试区分其类型。
    ''' </summary>
    ''' <param name="text">要分析的英文文献文本。</param>
    ''' <returns>一个包含 GeneMatch 结构的列表，其中包含标识符和其可能的类型。</returns>
    Public Function ExtractGenesWithTypes(text As String) As List(Of GeneMatch)
        If String.IsNullOrWhiteSpace(text) Then
            Return New List(Of GeneMatch)()
        End If

        Dim results As New List(Of GeneMatch)
        Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Try
            Dim matches As MatchCollection = Regex.Matches(text, GenePattern, RegexOptions)

            For Each match As Match In matches
                Dim identifier As String = match.Groups(1).Value

                If seen.Contains(identifier) Then
                    Continue For
                End If
                seen.Add(identifier)

                Dim type As String = "GeneName" ' 默认为 GeneName

                ' 根据匹配到的内容推断类型
                If Regex.IsMatch(identifier, "^[A-Z]{2,}\d+$", RegexOptions.IgnoreCase) OrElse
                   Regex.IsMatch(identifier, "^[A-Za-z]{1,2}[tTgG]\d+$", RegexOptions.IgnoreCase) Then
                    type = "GeneID"
                End If

                results.Add(New GeneMatch With {.Identifier = identifier, .Type = type})
            Next
        Catch ex As ArgumentException
            Console.WriteLine($"正则表达式模式错误: {ex.Message}")
            Return New List(Of GeneMatch)()
        End Try

        Return results
    End Function
End Class

' --- 演示如何使用的模块 ---
Module Module1GeneMatchTest
    Sub Main()
        ' 模拟一段包含基因信息的文献文本
        Dim sampleText As String = "
            The tumor suppressor gene p53 plays a crucial role in cellular response to DNA damage.
            Studies on BRCA1 and BRCA2 have linked mutations to increased breast cancer risk.
            In Arabidopsis thaliana, the flowering time gene AT1G01010 and another gene AT5G12340 are well-characterized.
            The expression of Oct4 and Sox2 is critical for maintaining pluripotency in stem cells.
            Rice gene Os03g0123456 was found to be upregulated under drought conditions.
            Note that common words like 'B2B' marketing or 'C3PO' from Star Wars might be incorrectly identified.
            The gene ATDC5 is often studied in chondrogenesis.
        "

        Console.WriteLine("--- 基因提取工具演示 ---")
        Console.WriteLine("输入文本:")
        Console.WriteLine(sampleText)
        Console.WriteLine(vbCrLf & "========================================" & vbCrLf)

        ' 1. 创建提取器实例
        Dim extractor As New GeneIdentifierExtractor()

        ' 2. 调用方法提取基因（简单列表）
        Console.WriteLine("--- 方法1: 提取所有基因标识符（简单列表）---")
        Dim geneList As List(Of String) = extractor.ExtractGenes(sampleText)

        If geneList.Any() Then
            Console.WriteLine($"找到 {geneList.Count} 个潜在的基因标识符:")
            For Each gene As String In geneList
                Console.WriteLine($" - {gene}")
            Next
        Else
            Console.WriteLine("未找到任何基因标识符。")
        End If

        Console.WriteLine(vbCrLf & "========================================" & vbCrLf)

        ' 3. 调用方法提取基因（带类型区分）
        Console.WriteLine("--- 方法2: 提取基因标识符并尝试区分类型 ---")
        Dim geneListWithTypes As List(Of GeneMatch) = extractor.ExtractGenesWithTypes(sampleText)

        If geneListWithTypes.Any() Then
            Console.WriteLine($"找到 {geneListWithTypes.Count} 个潜在的基因标识符:")
            For Each item As GeneMatch In geneListWithTypes
                Console.WriteLine($" - {item.Identifier,-15} (类型: {item.Type})")
            Next
        Else
            Console.WriteLine("未找到任何基因标识符。")
        End If

        Console.WriteLine(vbCrLf & "--- 演示结束 ---")
        Console.ReadKey()
    End Sub
End Module