
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.MIME.text.markdown
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.ReportBuilder.HTML

<Package("html", Category:=APICategories.UtilityTools)>
Public Module html

    ''' <summary>
    ''' Create a html template model from the given template file
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    <ExportAPI("template")>
    Public Function template(url As String) As TemplateHandler
        Return New TemplateHandler(file:=url)
    End Function

    ''' <summary>
    ''' Render markdown to html text
    ''' </summary>
    ''' <param name="markdown"></param>
    ''' <returns></returns>
    <ExportAPI("markdown.html")>
    Public Function markdownToHtml(markdown As String) As String
        Static render As New MarkdownHTML
        Return render.Transform(markdown)
    End Function
End Module
