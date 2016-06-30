Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell

Module CLI_MAN

    Public Sub GenerateDocument(repository As String, SPMgr As SPM.PackageModuleDb)
        Dim html As New StringBuilder(My.Resources.index)
        Call html.Replace(Title, "GCModeller CLI Manual")
        Call html.Replace(Doc, __innerHTML(SPMgr))
        Call html.SaveTo(repository & "/cli_man.html", System.Text.Encoding.UTF8)
    End Sub

    Private Function __innerHTML(SPMgr As SPM.PackageModuleDb) As String
        Dim LQuery = (From x In SPMgr.NamespaceCollection
                      Select (From xx In x.PartialModules
                              Where xx.Category = Scripting.MetaData.APICategories.CLI_MAN
                              Select xx).ToArray).MatrixToList
        Dim html As New StringBuilder("<a href=""./index.html"">Document Library</a> > <strong>CLI Manuals</strong>
<h1>GCModeller CLI Manual</h1>")
        Call html.AppendLine("<strong>How to get help from the program tools in the GCModeller</strong><br /> 
If you are not sure how to use the tools which is provided from the GCModeller, then you can just using the command line help system in the utility tools, 
<li>using <strong>?</strong> command to listing all of the available commands that exists in the tools</li>
<li>and using <strong>? &lt;name></strong> command to gets the details help information of which is the detail description of the command including usage and example.</li>
<li>and at last <strong>man</strong> command is available for all tools to print theirs command line user manual.</li>
<br /><br />
Here is a list of program tools that including in GCModeller for the systems biology analysis.
<br />")
        Call html.AppendLine("<table class=""API"" width=""100%"">
<tr>
<td width=""150px""><strong>Program</strong></td>
<td width=""250px""><strong>Name</strong></td>
<td><strong>Description</strong></td>
</tr>")
        For Each tool In LQuery
            Dim Name As String = FileIO.FileSystem.GetFileInfo(tool.Assembly.Path).Name
            Call html.AppendLine($"<tr>
<td><strong><a href=""./{Name}/{tool.Namespace}.html"">{IO.Path.GetFileNameWithoutExtension(Name)}</a></strong></td>
<td>{tool.Namespace}</td>
<td>{tool.Description}</td>
</tr>")
        Next
        Call html.AppendLine("</table>")

        Return html.ToString
    End Function
End Module
