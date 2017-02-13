Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    Module PathwayWebParser

        Const PATHWAY_SPLIT As String = "<a href=""/kegg-bin/show_pathway.+?"">.+?"
        Const MODULE_SPLIT As String = "<a href=""/kegg-bin/show_module.+?"">.+?"
        Const GENE_SPLIT As String = "<a href=""/dbget-bin/www_bget\?{0}:.+?"">.+?</a>"
        Const COMPOUND_SPLIT As String = "\<a href\=""/dbget-bin/www_bget\?cpd:.+?""\>.+?\</a\>.+?"

        ''' <summary>
        ''' 从某一个页面url或者文件路径所指向的网页文件之中解析出模型数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        <Extension> Public Function PageParser(url$) As Pathway
            Dim WebForm As New WebForm(url)

            If WebForm.Count = 0 Then
                Return Nothing
            End If

            Dim Pathway As New Pathway With {
                .Organism = WebForm.__organism,
                .EntryId = WebForm.__entryID,
                .KOpathway = WebForm.__koPathways,
                .Name = WebForm.__name,
                .Disease = __parseHTML_ModuleList(WebForm.GetValue("Disease").FirstOrDefault, LIST_TYPES.Disease),
                .PathwayMap = __parseHTML_ModuleList(WebForm.GetValue("Pathway map").FirstOrDefault, LIST_TYPES.Pathway).FirstOrDefault,
                .Description = WebForm.__description,
                .Modules = __parseHTML_ModuleList(WebForm.GetValue("Module").FirstOrDefault, LIST_TYPES.Module),
                .Genes = WebForm.parseList(WebForm.GetValue("Gene").FirstOrDefault, String.Format(GENE_SPLIT, .Organism.Key)),
                .Compound = WebForm.parseList(WebForm.GetValue("Compound").FirstOrDefault, COMPOUND_SPLIT),
                .References = WebForm.References
            }

            Return Pathway
        End Function

        <Extension>
        Private Function __koPathways(webForm As WebForm) As KeyValuePair()
            Dim KOpathway = webForm.GetValue("KO pathway") _
                .FirstOrDefault _
                .GetTablesHTML _
                .LastOrDefault _
                .GetRowsHTML _
                .Select(Function(row$)
                            Dim cols As String() = row.GetColumnsHTML
                            Return New KeyValuePair With {
                                .Key = cols(0).StripHTMLTags.StripBlank,
                                .Value = cols(1).StripHTMLTags.StripBlank
                            }
                        End Function).ToArray
            Return KOpathway
        End Function

        <Extension> Private Function __organism(webForm As WebForm) As KeyValuePair
            Dim html As String = webForm.GetValue("Organism").FirstOrDefault
            Dim speciesCode = Regex.Match(html, "\[GN:<a href="".+?"">.+?</a>]").Value.GetValue

            html = html.StripHTMLTags(True)
            html = html.GetTagValue(vbLf).Value

            Return New KeyValuePair With {
                .Key = speciesCode,
                .Value = html
            }
        End Function

        <Extension> Private Function __description(webForm As WebForm) As String
            Dim html$ = webForm.GetValue("Description").FirstOrDefault
            html = html.StripHTMLTags(True)
            html = html.GetTagValue(vbLf).Value
            Return html
        End Function

        <Extension> Private Function __name(webForm As WebForm) As String
            Dim html$ = webForm.GetValue("Name").FirstOrDefault
            html = html.StripHTMLTags(True).GetTagValue(vbLf, True).Value
            Return html
        End Function

        <Extension> Private Function __entryID(webForm As WebForm) As String
            Dim html$ = webForm.GetValue("Entry").FirstOrDefault
            html = html.StripHTMLTags.StripBlank
            Return Regex.Match(html, "[a-z]+\d+", RegexOptions.IgnoreCase).Value
        End Function

        ''' <summary>
        ''' Pathway和Module的格式都是一样的，所以在这里通过<paramref name="type"/>参数来控制对象的类型
        ''' </summary>
        ''' <param name="s_Value"></param>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Public Function __parseHTML_ModuleList(s_Value As String, type As LIST_TYPES) As KeyValuePair()
            If String.IsNullOrEmpty(s_Value) Then
                Return Nothing
            End If

            Dim SplitRegex As String = ""

            Select Case type
                Case LIST_TYPES.Disease
                    SplitRegex = "<a href=""/dbget-bin/www_bget\?ds:H.+?"">.+?"
                Case LIST_TYPES.Module
                    SplitRegex = MODULE_SPLIT
                Case LIST_TYPES.Pathway
                    SplitRegex = PATHWAY_SPLIT
            End Select

            Dim sbuf As String() = Regex.Matches(s_Value, SplitRegex).ToArray.Distinct.ToArray
            Dim ModuleList As New List(Of KeyValuePair)

            Select Case type
                Case LIST_TYPES.Disease
                    SplitRegex = "<a href=""/dbget-bin/www_bget\?ds:H.+?"">.+?</a>"
                Case LIST_TYPES.Module
                    SplitRegex = "<a href=""/kegg-bin/show_module.+?"">.+?</a>"
                Case LIST_TYPES.Pathway
                    SplitRegex = "<a href=""/kegg-bin/show_pathway.+?"">.+?</a>"
            End Select

            For i As Integer = 0 To sbuf.Count - 2
                Dim p1 As Integer = InStr(s_Value, sbuf(i))
                Dim p2 As Integer = InStr(s_Value, sbuf(i + 1))
                Dim len As Integer = p2 - p1
                Dim strTemp As String = Mid(s_Value, p1, len)

                Dim ModuleEntry As String = Regex.Match(strTemp, SplitRegex).Value
                Dim ModuleFunction As String = strTemp.Replace(ModuleEntry, "").Trim

                ModuleEntry = ModuleEntry.GetValue
                ModuleFunction = WebForm.RemoveHrefLink(ModuleFunction)

                ModuleList += New KeyValuePair With {
                    .Key = ModuleEntry,
                    .Value = ModuleFunction
                }
            Next

            Dim p As Integer = InStr(s_Value, sbuf.Last)
            s_Value = Mid(s_Value, p)
            Dim LastEntry As New KeyValuePair With {
                .Key = Regex.Match(s_Value, SplitRegex).Value,
                .Value = WebForm.RemoveHrefLink(s_Value.Replace(.Key, "").Trim)
            }
            LastEntry.Key = LastEntry.Key.GetValue

            Call ModuleList.Add(LastEntry)

            For Each x In ModuleList
                x.Key = x.Key.StripHTMLTags.StripBlank
                x.Value = x.Value.StripHTMLTags.StripBlank
            Next

            Return ModuleList.ToArray
        End Function

        Enum LIST_TYPES As Integer
            [Module]
            [Pathway]
            [Disease]
        End Enum
    End Module
End Namespace