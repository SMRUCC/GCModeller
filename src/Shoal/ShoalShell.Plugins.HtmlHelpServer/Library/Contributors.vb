Imports System.Text
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.ShoalShell

Module Contributors

    Public Function GenerateDocument(repository As String, SPMgr As SPM.PackageModuleDb) As Boolean
        Dim HtmlBuilder As StringBuilder = New StringBuilder(My.Resources.index)
        Call HtmlBuilder.Replace(Title, "Contributors")
        Call HtmlBuilder.Replace(Doc, __contributors(SPMgr))

        Return HtmlBuilder.SaveTo(repository & "/contributors.html", System.Text.Encoding.UTF8)
    End Function

    Private Function __contributors(SPMgr As SPM.PackageModuleDb) As String
        Dim doc As New StringBuilder(
"<a href=""./index.html"">Document Library</a> > <strong>Contributors</strong><br /><br />
<br /><h1>About GCModeller</h1>
GCModeller is a project which is attempting to provide a modern piece of systems biology analysis software for the GNU suite of software.
<br />
<p>GCModeller has three type of user interface(CLI commandline, Workbench GUI, ShoalShell Scripting), and the ShoalShell scripting language project is the one of the user interface for biologist applied GCModeller in their researches.
As a most important part in the GCModeller, the current ShoalShell is the result of a collaborative effort with contributions from all over the world.
<br />

<h2>Authors of ShoalShell</h2>
<p>As the project leader of GCModeller, master xie developed the entired ShoalShell runtime core library and the most part of the GCModeller ShoalShell library packages, he is begins written GCModeller since mid-2013 and start writing ShoalShell from a idea of develop a much commandline like debugging tools for the GCModeller virtual cell system since 2014 August.</p>

Here is the currently developer members on developing GCModeller:<br /><p>
<li>Master xie (<a href=""mailto://xie.guigang@gcmodeller.org"">xie.guigang@gcmodeller.org</a>, <a href=""mailto://xie.guigang@xanthomonas.wiki"">xie.guigang@xanthomonas.wiki</a>)</li>
<li>and his research assistant Miss asuka (<a href=""mailto://amethyst.asuka@gcmodeller.org"">amethyst.asuka@gcmodeller.org</a>)</li>
<li>Mr Huahao Jiang (<a href=""mailto://jhh1725@gcmodeller.org"">jhh1725@gcmodeller.org</a>)</li></p><br />

<h2>Contributors to GCModeller Library</h2>
<p>
Here are the peoples who is contributes to the algorithm or source code of GCModeller:
</p>
<br />")
        Call doc.AppendLine("<table>")
        Call doc.AppendLine("<tr>                            
    <th>Author/Contributors</th><th>Modules</th>
  </tr>")
        Dim getAuthors = (From authorMod
                          In (From obj In SPMgr.NamespaceCollection Select (From [mod] In obj.PartialModules Select [mod].Publisher, [mod]).ToArray).ToArray.MatrixToList
                          Select authorMod,
                              author = InputHandler.ToString(authorMod.Publisher).ToLower.Trim
                          Group authorMod By author Into Group).ToArray

        For Each author In getAuthors
            Dim modList As String = String.Join(", ", (From [mod] In author.Group
                                                       Let typeRef As System.Type = [mod].mod.Assembly.GetType
                                                       Where Not typeRef Is Nothing  '可能文件被删掉了或者命名空间被修改了，找不到类型的定义入口数据
                                                       Let [namespace] = typeRef.NamespaceEntry.Namespace
                                                       Select $"<a href=""./{[namespace].NormalizePathString(False)}.html"">{[namespace]}</a>").ToArray)
            Dim authorName As String = author.author
            If String.IsNullOrEmpty(authorName) Then
                authorName = "Thanks to these anonymous developers"
            Else
                authorName = author.Group.First.mod.Publisher
            End If

            Call doc.AppendLine($" <tr><td>{authorName}</td><td>{modList}</td> </tr>")
        Next

        Call doc.AppendLine("</table>")
        Call doc.AppendLine("<br />
<p> </p>
<h2>Special thanks</h2>
<p>Many thanks to my teacher and <strong>doctor niu(<a href=""mailto://niuxiangna@gmail.com"">niuxiangna@gmail.com</a>), 
professor Jiang(<a href=""mailto://weijiang@gxu.edu.cn"">weijiang@gxu.edu.cn</a>) and 
professor He(<a href=""mailto://yqhe@gxu.edu.cn"">yqhe@gxu.edu.cn</a>)</strong> from <a href=""http://sklcusa.gxu.edu.cn/"">SKLCUSA Laboratory</a> in Guangxi University, 
for giving good advices on my research and encouraging me devoted myself into the GCModeller project and bringing out this fantastic language project to you.
<br /><br /><p>")
        Call doc.AppendLine($"<h2>License</h2>
<p>
<pre>{Scripting.ShoalShell.License}</pre>
</p>")
        Call doc.AppendLine("<p><br /><br /></p><h2>Project Links</h2>
<p>GCModeller was benefits from two special project on the RNA-seq high-performance analysis, these two project which are:
<br />
<table>
<tr><th>Bioinformatics Project</th><th>HOME</th></tr>
<tr><td><li>Microsoft .NET Bio</li></td><td><a href=""https://github.com/dotnetbio/bio"">https://github.com/dotnetbio/bio</a></td></tr>
<tr><td><li>BOW(Bioinformatics on Windows)</li></td><td><a href=""http://bow.codeplex.com/"">http://bow.codeplex.com/</a></td></tr>
</table>
<h3>Project Folked Branches</h3>
<p>Shoal Shell not just working in systems biology area but also works in financial economics area: 
Another Shoal commercial extension project ""<strong>MiMaster</strong> (pre-alpha state)"" was applied on the financial online trading system from <a href=""http://mipaimai.com"">MiPaiMai.com</a>, power this very first online instant auction platform.
<p>
<table>
<tr><th>Shoal Project</th><th>HOME</th><th>Science</th><th>License</th></tr>
<tr><td><li>GCModeller</li></td><td><a href=""http://GCModeller.org"">http://GCModeller.org</a></td><td>Systems Biology/Bioinformatics</td><td>GPL3 Open Source</td></tr>
<tr><td><li>MiMaster</li></td><td><a href=""http://mipaimai.com/"">http://mipaimai.com/</a></td><td>Financial Online Trading System</td><td>Commercial Licensed($32k/Product System)</td></tr>
</table>
</p> </p>

</p>")

        Return doc.ToString
    End Function
End Module
