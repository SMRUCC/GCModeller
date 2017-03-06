Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 从KEGG之上下载疾病的信息
    ''' </summary>
    Public Module DownloadDiseases

        ''' <summary>
        ''' 解析KEGG上面的疾病模型数据的网页文本
        ''' </summary>
        ''' <param name="html$"></param>
        ''' <returns></returns>
        <Extension> Public Function Parse(html As WebForm) As Disease
            Dim dis As New Disease With {
                .Entry = html.GetText("Entry").Split.First,
                .Name = html.GetText("Name"),
                .Description = html.GetText("Description"),
                .Category = html.GetText("Category"),
                .Pathway = PathwayWebParser.__parseHTML_ModuleList(html("Pathway").FirstOrDefault, LIST_TYPES.Pathway),
                .Comment = html.GetText("Comment"),
                .References = html.References,
                .Markers = html("Marker").FirstOrDefault.MarkerList
            }

            Return dis
        End Function

        <Extension>
        Private Function MarkerList(html$) As TripleKeyValuesPair()
            Dim list As New List(Of TripleKeyValuesPair)

            html = html.DivInternals.FirstOrDefault

            Dim lines$() = html _
                .HtmlLines _
                .Select(Function(s) s.StripHTMLTags(stripBlank:=True)) _
                .Where(Function(s) Not s.StringEmpty) _
                .ToArray

            For Each line$ In lines
                Dim tokens$() = line.Split
                list += New TripleKeyValuesPair With {
                    .Key = tokens(0),
                    .Value1 = tokens(1),
                    .Value2 = tokens(2)
                }
            Next

            Return list
        End Function

        ''' <summary>
        ''' 使用疾病编号下载疾病数据模型
        ''' </summary>
        ''' <param name="ID$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Download(ID$) As Disease
            Dim url$ = $"http://www.kegg.jp/dbget-bin/www_bget?ds:{ID}"
            Return DownloadURL(url)
        End Function

        Public Function DownloadURL(url$) As Disease
            Return New WebForm(url).Parse
        End Function
    End Module
End Namespace