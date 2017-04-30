Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module ModuleDBGet

        ''' <summary>
        ''' 这个函数下载的是物种特定的模块信息，不是参考模块数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Public Function Download(url As String) As [Module]
            Dim WebForm As New WebForm(url)

            If WebForm.Count = 0 Then
                Return Nothing
            End If

            Dim sp As String = WebForm.GetValue("Organism").FirstOrDefault
            sp = Regex.Match(sp, "\[GN:<a href="".+?"">.+?</a>]").Value.GetValue

            Dim [Module] As New [Module] With {
                .EntryId = Regex.Match(WebForm.GetValue("Entry").FirstOrDefault, "[a-z]+_M\d+").Value,
                .Name = WebForm.GetValue("Name").FirstOrDefault,
                .Description = WebForm.GetValue("Definition").FirstOrDefault,
                .Pathway = WebForm.parseList(WebForm.GetValue("Pathway").FirstOrDefault, "<a href=""/kegg-bin/show_pathway\?.+?"">.+?</a>"),
                .PathwayGenes = WebForm.parseList(WebForm.GetValue("Gene").FirstOrDefault, String.Format("<a href=""/dbget-bin/www_bget\?{0}:.+?"">.+?</a>", sp)),
                .Compound = WebForm.parseList(WebForm.GetValue("Compound").FirstOrDefault, "<a href=""/dbget-bin/www_bget\?cpd:.+?"">.+?</a>"),
                .Reaction = WebForm.parseList(WebForm.GetValue("Reaction").FirstOrDefault, "<a href=""/dbget-bin/www_bget\?rn:.+?"">.+?</a>")
            }

            Return [Module]
        End Function
    End Module
End Namespace