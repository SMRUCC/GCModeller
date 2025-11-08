Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.PubMed

''' <summary>
''' parse the pubmed database file in plaintext format
''' </summary>
Public Module PlainTextParser

    Public Iterator Function LoadArticles(file As String) As IEnumerable(Of PubmedArticle)
        Dim blocks As String()() = file _
            .IterateAllLines _
            .Split(Function(line) line.StringEmpty) _
            .Where(Function(b)
                       Return Not (b.IsNullOrEmpty OrElse
                           b.All(Function(si) si.StringEmpty(, True)))
                   End Function) _
            .ToArray

        For Each block As String() In blocks
            Yield ParseArticle(block)
        Next
    End Function

    Private Function ParseArticle(lines As String()) As PubmedArticle
        Dim terms As Dictionary(Of String, String()) = LoadTerms(lines) _
            .GroupBy(Function(a) a.Name) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.Values
                          End Function)
        Dim article As New Article With {
            .Abstract = New Abstract(terms.TryGetValue("AB")),
            .ArticleTitle = terms.TryGetValue("TI").JoinBy("; "),
            .Journal = New Journal(terms.TryGetValue("JT").JoinBy("; ")),
            .AuthorList = New AuthorList With {
                .Authors = terms.TryGetValue("AU") _
                    .SafeQuery _
                    .Select(Function(name) New Author With {.Initials = name}) _
                    .ToArray
            },
            .ArticleDate = New PubDate(terms.TryGetValue("DP").DefaultFirst)
        }
        Dim cite As New MedlineCitation With {
            .Owner = terms.TryGetValue("OWN").JoinBy("; "),
            .Status = terms.TryGetValue("STAT").JoinBy("; "),
            .PMID = New PMID(terms.TryGetValue("PMID").DefaultFirst),
            .Article = article,
            .KeywordList = New KeywordList With {
                .Keywords = terms _
                    .TryGetValue("OT") _
                    .SafeQuery _
                    .Select(Function(key) New Keyword(key)) _
                    .ToArray
            },
            .MeshHeadingList = terms.TryGetValue("MH") _
                .SafeQuery _
                .Select(Function(key) New MeshHeading(key)) _
                .ToArray
        }
        Dim metadata As New PubmedData With {
            .ArticleIdList = terms.TryGetValue("AID") _
                .SafeQuery _
                .Select(Function(aid)
                            Dim id As String = Nothing
                            Dim type As String = Nothing

                            If TryParseAID(aid, id, type) Then
                                Return New ArticleId(id, type)
                            Else
                                Return Nothing
                            End If
                        End Function) _
                .Where(Function(aid) aid IsNot Nothing) _
                .ToArray
        }

        Return New PubmedArticle With {
            .MedlineCitation = cite,
            .PubmedData = metadata
        }
    End Function

    ''' <summary>
    ''' 解析字符串，提取标识符和类型部分。
    ''' </summary>
    ''' <param name="input">输入的字符串（例如 "ijms24010457 [pii]"）</param>
    ''' <param name="identifier">输出解析后的标识符</param>
    ''' <param name="type">输出解析后的类型</param>
    ''' <returns>解析成功返回 True，失败返回 False</returns>
    Public Function TryParseAID(input As String, ByRef identifier As String, ByRef type As String) As Boolean
        If String.IsNullOrWhiteSpace(input) Then
            Return False
        End If

        ' 正则表达式模式说明：
        ' ^         : 字符串开始
        ' (.*?)     : 非贪婪匹配任意字符（标识符部分），直到遇到后续模式
        ' \s*       : 零个或多个空格
        ' \[        : 匹配左方括号"["
        ' (.*?)     : 非贪婪匹配方括号内的内容（类型部分）
        ' \]        : 匹配右方括号"]"
        ' \s*       : 零个或多个尾部空格
        ' $         : 字符串结束
        Dim pattern As String = "^(.*?)\s*\[(.*?)\]\s*$"
        Dim match As Match = Regex.Match(input, pattern)

        If match.Success AndAlso match.Groups.Count = 3 Then
            ' 使用 Trim() 去除捕获值中可能残留的首尾空格
            identifier = match.Groups(1).Value.Trim()
            type = match.Groups(2).Value.Trim()
            Return True
        End If

        ' 匹配失败时返回 False
        identifier = Nothing
        type = Nothing
        Return False
    End Function

    Private Iterator Function LoadTerms(lines As String()) As IEnumerable(Of NamedValue(Of String))
        Dim data As New List(Of NamedValue(Of String))
        Dim temp As New List(Of String)
        Dim term As String = Nothing

        For Each line As String In lines
            If Not line.StartsWith("    ") Then
                ' start with new term
                If temp.Any Then
                    Yield New NamedValue(Of String)(term, temp.JoinBy(" "))
                    Call temp.Clear()
                End If

                term = line.Substring(0, 4)
                temp.Add(line.Substring(5).Trim)
            Else
                temp.Add(line.Trim)
            End If
        Next

        If temp.Any Then
            Yield New NamedValue(Of String)(term, temp.JoinBy(" "))
        End If
    End Function
End Module
