Imports System.Net.Http
Imports System.Text.RegularExpressions

Namespace SwissTCS

    ''' <summary>
    ''' SwissTCS 网页的下载与解析工具。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 旧实现直接调用 <c>url.GET</c> 并且用正则对整页做贪婪匹配，一旦网络抖动或者页面结构变化
    ''' 就会静默返回空结果。本模块把这些操作收敛到一处，并且补上：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>带超时与重试的下载(可配置)</description></item>
    ''' <item><description>HTML 标签剥离与实体解码</description></item>
    ''' <item><description>表格单元与 <c>&lt;option&gt;</c> 列表的结构化解析</description></item>
    ''' </list>
    ''' <para>
    ''' 所有方法都是纯函数(除了 <see cref="Download"/>)，因此解析部分可以直接用固定的 HTML
    ''' 片段做单元测试。
    ''' </para>
    ''' </remarks>
    Public Module WebParser

        ''' <summary>
        ''' 单次请求的超时时间(毫秒)
        ''' </summary>
        Public Timeout As Integer = 15000
        ''' <summary>
        ''' 失败之后的重试次数
        ''' </summary>
        Public Retries As Integer = 2

        Private ReadOnly Client As HttpClient = CreateClient()

        Private Function CreateClient() As HttpClient
            Dim client As New HttpClient()
            client.DefaultRequestHeaders.Add("User-Agent", "GCModeller/ProteinTools.Interactions")
            Return client
        End Function

        ''' <summary>
        ''' 下载一个网页。失败的时候重试 <see cref="Retries"/> 次，仍然失败则返回空字符串。
        ''' </summary>
        Public Function Download(url As String) As String
            If String.IsNullOrEmpty(url) Then
                Return ""
            End If

            For attempt As Integer = 0 To Retries
                Try
                    Dim source As Threading.Tasks.Task(Of String) = Client.GetStringAsync(url)

                    If source.Wait(Timeout) Then
                        Return source.Result
                    End If
                Catch ex As Exception
                    If attempt >= Retries Then
                        Call Console.WriteLine($"Download failed for '{url}': {ex.Message}")
                    End If
                End Try
            Next

            Return ""
        End Function

        ''' <summary>
        ''' 从 <c>&lt;select&gt;</c> 块之中解析出全部 <c>&lt;option&gt;</c> 的取值
        ''' </summary>
        Public Function ParseOptions(html As String) As String()
            If String.IsNullOrEmpty(html) Then
                Return New String() {}
            End If

            Dim matches As MatchCollection = Regex.Matches(html, "<option\s+value=""([^""]*)""", RegexOptions.IgnoreCase)

            Return matches.Cast(Of Match)() _
                .Select(Function(m) m.Groups(1).Value.Trim()) _
                .Where(Function(s) Not String.IsNullOrEmpty(s)) _
                .Distinct() _
                .ToArray()
        End Function

        ''' <summary>
        ''' 解析全部 <c>&lt;optgroup&gt;</c> 块，每个块返回其中的选项列表
        ''' </summary>
        Public Function ParseOptionGroups(html As String) As String()()
            If String.IsNullOrEmpty(html) Then
                Return New String()() {}
            End If

            Dim groups As MatchCollection = Regex.Matches(html, "<optgroup[^>]*>(.*?)</optgroup>", RegexOptions.Singleline + RegexOptions.IgnoreCase)
            Dim result As New List(Of String())

            For Each group As Match In groups
                Call result.Add(ParseOptions(group.Groups(1).Value))
            Next

            Return result.ToArray()
        End Function

        ''' <summary>
        ''' 解析表格单元格(保留内部的原始 HTML)
        ''' </summary>
        Public Function ParseTableCells(html As String) As String()
            If String.IsNullOrEmpty(html) Then
                Return New String() {}
            End If

            Dim matches As MatchCollection = Regex.Matches(html, "<td[^>]*>(.*?)</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase)

            Return matches.Cast(Of Match)() _
                .Select(Function(m) m.Groups(1).Value) _
                .ToArray()
        End Function

        ''' <summary>
        ''' 剥离 HTML 标签，并把常见实体替换为字符
        ''' </summary>
        Public Function StripHtml(html As String) As String
            If String.IsNullOrEmpty(html) Then
                Return ""
            End If

            Dim text As String = Regex.Replace(html, "<[^>]+>", " ")
            text = text.Replace("&nbsp;", " ") _
                       .Replace("&amp;", "&") _
                       .Replace("&lt;", "<") _
                       .Replace("&gt;", ">") _
                       .Replace("&quot;", """") _
                       .Replace("&#39;", "'")

            Return Regex.Replace(text, "\s+", " ").Trim()
        End Function

        ''' <summary>
        ''' 从一个表格单元之中取出 <c>&lt;font&gt;</c> 标签之间的文本
        ''' </summary>
        Public Function ParseFontText(cell As String) As String
            If String.IsNullOrEmpty(cell) Then
                Return ""
            End If

            Dim match As Match = Regex.Match(cell, "<font[^>]*>(.*?)</font>", RegexOptions.Singleline Or RegexOptions.IgnoreCase)

            If match.Success Then
                Return StripHtml(match.Groups(1).Value)
            Else
                Return StripHtml(cell)
            End If
        End Function

        ''' <summary>
        ''' 从形如 <c>&lt;form method="POST" action="get_scores2.pl"&gt;</c> 的位置之后截取正文
        ''' </summary>
        ''' <param name="html">完整页面</param>
        ''' <param name="marker">起始标记，为 <c>Nothing</c> 的时候使用默认的评分表单标记</param>
        Public Function SliceAfter(html As String, Optional marker As String = Nothing) As String
            If String.IsNullOrEmpty(html) Then
                Return ""
            End If

            Dim anchor As String = If(marker, "<form method=""POST"" action=""get_scores2.pl"">")
            Dim p As Integer = html.IndexOf(anchor, StringComparison.OrdinalIgnoreCase)

            If p < 0 Then
                Return html
            End If

            Return html.Substring(p)
        End Function
    End Module
End Namespace
