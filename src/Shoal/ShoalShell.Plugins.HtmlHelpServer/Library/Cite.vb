Imports System.Text
Imports Microsoft.VisualBasic.Linq.Extensions

''' <summary>
''' 生成每一个工具的引用文献的工具
''' </summary>
Module Cite

    Public Sub GenerateDocument(repository As String, library As Scripting.ShoalShell.SPM.PackageModuleDb)
        Dim index As New StringBuilder(My.Resources.index)
        Call index.Replace(Title, "Cites")
        Call index.Replace(Doc, __innerHTML(library))
        Call index.SaveTo($"{repository}/cites.html", System.Text.Encoding.UTF8)
    End Sub

    Private Function __innerHTML(library As Scripting.ShoalShell.SPM.PackageModuleDb) As String
        Dim html As New StringBuilder("<a href=""./index.html"">Document Library</a> > <strong>Module Cites</strong><br /><br />
<p>
Here is a list of articles that cite for build the modules in the GCModeller, many thanks to these researches:</p>")
        Dim LQuery = (From ns In library.NamespaceCollection
                      Select (From x In ns.PartialModules
                              Let cites As String = x.GetCites
                              Where Not String.IsNullOrEmpty(cites)
                              Select cites).ToArray).MatrixToList
        Call html.AppendLine("<table><tr><td>References</td></tr>")
        Call html.AppendLine(LQuery.ToArray(Function(x, i) $"<tr><td>{x}</td></tr>").JoinBy(vbCrLf))
        Call html.AppendLine("</table>")

        Return html.ToString
    End Function
End Module
