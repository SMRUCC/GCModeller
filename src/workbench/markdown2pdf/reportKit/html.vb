
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.ReportBuilder.HTML

<Package("html")>
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
End Module
