Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell.HTML
Imports Microsoft.VisualBasic.Linq.Extensions

''' <summary>
''' 生成数据类型的帮助文档
''' </summary>
Module Type

    Public Sub GenerateDocument(repository As String, library As Scripting.ShoalShell.SPM.PackageModuleDb)
        Dim index As New StringBuilder(My.Resources.index)

        Call index.Replace(Title, "TypeOf GCModeller")
        Call index.Replace(Doc, __innerHTML(library, repository))
        Call index.RestoreCSS
        Call index.SaveTo($"{repository}/TypeLinks/index.html", System.Text.Encoding.UTF8)
    End Sub

    <Extension> Public Sub RestoreCSS(ByRef sbr As StringBuilder)
        Call sbr.Replace("=""assets", "=""../assets")
    End Sub

    Private Function __innerHTML(library As Scripting.ShoalShell.SPM.PackageModuleDb, repository As String) As String
        Dim html As StringBuilder = New StringBuilder
        Dim pages = __pages(library, repository)

        Call html.AppendLine("<a href=""../index.html"">Document Library</a> > TypeOf GCModeller")
        Call html.AppendLine("<br /><br /><p>
Here are the list of all data types in the GCModeller for facility of your programming with GCModeller API:
</p>
")
        Call html.AppendLine("<p> <font size=""2"">
<table class=""API"" width=""100%"">
<tr><td><strong>TypeOf List GCModeller Programming Manual</strong></td></tr>")
        Call html.AppendLine(String.Join(vbCrLf, (From type As System.Type
                                                  In pages
                                                  Where Not String.IsNullOrEmpty(type.FullName)
                                                  Select $"<tr><td><a href=""./{PageName(type)}.html"">{type.FullName}</a></td></tr>").ToArray))
        Call html.AppendLine("</table>
</font>")
        Call html.AppendLine("</p>")

        Return html.ToString
    End Function

    Private Function __pages(library As Scripting.ShoalShell.SPM.PackageModuleDb, repository As String) As System.Type()
        Dim source = Scripting.ShoalShell.HTML.TypeLinks.GetSource(library)
        Dim __getPages = (From data In source
                          Let template As String = pageTemplate
                          Let tdefValue = TypeLinks.CreatePage(template, data)
                          Select tdef = tdefValue.Value,
                              html = tdefValue.Key
                          Order By tdef.FullName Ascending).ToArray

        For Each page In __getPages
            Dim path As String = $"{repository}/{NameOf(TypeLinks)}/{PageName(page.tdef)}.html"
            Dim html As StringBuilder = New StringBuilder(My.Resources.index)
            Dim innerDoc As New StringBuilder
            Call innerDoc.AppendLine("<a href=""../index.html"">Document Library</a> > <a href=""./index.html"">TypeOf GCModeller</a><br />")
            Call innerDoc.AppendLine(page.html)
            Call html.Replace(Title, page.tdef.ToString)
            Call html.Replace(Doc, innerDoc.ToString)
            Call html.RestoreCSS
            Call html.SaveTo(path, System.Text.Encoding.UTF8)
        Next

        Return __getPages.ToArray(Function(x) x.tdef)
    End Function

    Const pageTemplate As String = "<table class=""API"" width=""100%"">
<tr><td width=""120px""></td><td>%Publisher%</td></tr>
<tr><td>Description</td><td>%Description%</td></tr>
<tr><td></td><td>%DefineFile%</td></tr>
<tr><td>TypeOf</td><td>%Type%</td></tr>
</table>


<p>%SDK_HELP%</p>"
End Module
