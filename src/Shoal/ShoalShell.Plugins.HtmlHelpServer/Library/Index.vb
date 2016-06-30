Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Linq.Extensions

''' <summary>
''' 这个模块是用于生成本地帮助文档的工具，这个工具和Shoal脚本引擎之中的自带的帮助文档的生成工具的作用是一样的，
''' 只不过Shoal脚本引擎之中的帮助文档生成引擎由于精简内核的需要，只会生成最基本的视图，而这个模块则是加强版的帮助文档的生成工具
''' 
''' index.html
''' </summary>
Public Module Index

    Public Const Doc As String = "{doc}"
    Public Const Title As String = "{Title}"

    Public Sub GenerateDocument(repository As String)
        Dim index As New StringBuilder(My.Resources.index)
        Dim library = SPM.PackageModuleDb.LoadDefault

        Call $"Library source loaded!".__DEBUG_ECHO
        Call My.Resources.foundation.SaveTo($"{repository}/assets/foundation.css")
        Call $"Write css foundation!".__DEBUG_ECHO
        Call index.Replace("{doc}", __innerHTML(library, repository))
        Call index.Replace(Title, "GCModeller Help Library")
        Call Cite.GenerateDocument(repository, library)
        Call Type.GenerateDocument(repository, library)

        Dim indexPage As String = $"{repository}/index.html"

        Call index.SaveTo(indexPage, Encoding.UTF8)
        Call $"Index page saved at {indexPage.ToFileURL}...".__DEBUG_ECHO
    End Sub

    ''' <summary>
    ''' 生成index.html
    ''' </summary>
    ''' <param name="library"></param>
    ''' <returns></returns>
    Private Function __innerHTML(library As SPM.PackageModuleDb, repository As String) As String
        Dim innerDoc As New StringBuilder(1024)

        Call innerDoc.AppendLine($"<h1>Brief Introductions</h1>
<table>
<tr><td>Publisher: </td><td><a href=""mailto://xie.guigang@gcmodeller.org"">xie.guigang@gcmodeller.org</a></td></tr>
<tr><td>Version: </td><td>{GetType(Runtime.ScriptEngine).ModuleVersion}</td></tr>
<tr><td>License: </td><td>
<a href=""http://www.gnu.org/licenses/gpl-3.0.html"">GPL3</a></td></tr>
<tr><td></td><td>
<p>GCModeller is an open source project of bioinformatics computing platform which is attempting to provide a modern piece of systems biology analysis software for the GNU suite of software. 
Inspired by the Microsoft bioinformatics research project MBF and recently advances in Virtual Cell technology, GCModeller was developed for solving the problem of systemsbiology not only in the discovery of the novel cellular mechanism in molecular biology, but also for more complex situation such as metagenomics and environment co-evolution. 
GCModeller is original written in Microsoft <strong>VisualBasic</strong> language, and some module in GCModeller is hybrids programming with <strong><a href=""https://www.perl.org/"">Perl</a></strong> and <strong><a href=""https://www.r-project.org/"">R</a></strong> languages.<br /> 
ShoalShell program is part of important runtime environment from GCModeller analysis suite which it is distributed under the GPL3 license, you can download the latest source code of shoal shell from SourceForge.net:
<br /><li><a href=""https://github.com/SMRUCC/Shoal"">https://github.com/SMRUCC/Shoal</a></li>
</p>
</td></tr>
</table>")

        Call innerDoc.AppendLine("<h1>Library Document Contents</h1>
<li><a href=""#Modules"">Installed Modules</a></li>
<li><a href=""./contributors.html"">Contributors</a></li>
<li><a href=""./cites.html"">Cites</a></li>
<li><a href=""./cli_man.html"">CLI Tools Manual</a></li>
<li><a href=""./TypeLinks/index.html"">TypeOf GCModeller</a></li>
")
        Dim libraries As SPM.Nodes.Namespace() = (From x As SPM.Nodes.Namespace
                                                  In library.NamespaceCollection
                                                  Select x
                                                  Order By x.Namespace Ascending).ToArray
        Call $"Start indexing library packages....".__DEBUG_ECHO
        Call innerDoc.AppendLine($"<h2><a name=""Modules"">Installed Modules</a></h2>
Currently there are {library.NamespaceCollection.Length} packages
({library.NamespaceCollection.ToArray(Function(x) x.API.ToArray(Function(a) a.OverloadsNumber).Sum, Parallel:=True).Sum} API in totally) and 
{library.HybridEnvironments.Length} <a href=""#Languages"">language hybrids environments</a> have been installed on your ShoalShell system:
<br />
<br />
<p>
<strong>Quick Navigation</strong>
<br />
{HTML.DocRenderer.QuickNavigation(libraries.ToArray(Function(x) x.Namespace))}
<br />
<table>
<tr>
<td><strong>Namespace</strong></td><td><strong>Description</strong></td>
</tr>")

        For Each ns As SPM.Nodes.Namespace In libraries
            Call innerDoc.AppendLine($"<tr>
<td align=""left""><a name=""{ns.Namespace}"" href=""./{ns.Namespace.NormalizePathString(False)}.html"">{ns.Namespace}</a></td>
<td align=""left"">{ns.Description}</td>
</tr>")
        Next

        Call innerDoc.AppendLine("</table></p><a href=""#""><strong><font size=3>[&#8593;]</font></strong></a>")
        Call innerDoc.AppendLine("<h1><a name=""Languages"">Language Hybrids</a></h1>
<p>
Here is the language environment that was installed in your shoal system:
</p>")
        Call innerDoc.AppendLine($"<table>
<tr><td><strong>Language</strong></td><td><strong>Description</strong></td><td><strong>Module</strong></td></tr>
{library.HybridEnvironments.ToArray(Function(x) $"<tr><td><strong>{x.Language}</strong></td><td>{x.Description}</td><td>{FileIO.FileSystem.GetFileInfo(x.Path).Name}</td></tr>").JoinBy(vbCrLf)}
</table>")

        Call [Namespace].GenerateDocument(repository, libraries)
        Call Contributors.GenerateDocument(repository, library)
        Call CLI_MAN.GenerateDocument(repository, library)

        Return innerDoc.ToString
    End Function
End Module
