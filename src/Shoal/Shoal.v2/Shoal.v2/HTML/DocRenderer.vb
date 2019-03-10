Imports System.Text
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.SPM.Nodes

Namespace HTML

    ''' <summary>
    ''' 命名空间的html帮助页面的生成工具
    ''' </summary>
    Public Module DocRenderer

        Public ReadOnly Property wwwroot As String = $"{App.HOME}/html"

        ''' <summary>
        ''' 值返回路径
        ''' </summary>
        ''' <param name="[Namespace]"></param>
        ''' <returns></returns>
        Public Function RequestHtml([Namespace] As SPM.Nodes.Namespace) As String
            Return $"{wwwroot}/{[Namespace].Namespace.NormalizePathString(False)}.html"
        End Function

        Public Function RequestHtml(Name As String) As String
            Return $"{wwwroot}/{Name.NormalizePathString(False)}.html"
        End Function

        Public Sub GenerateHtmlDoc([Namespace] As SPM.Nodes.Namespace)
            Dim html As String = HtmlRender([Namespace])
            Call html.SaveTo(RequestHtml([Namespace]))
            Call Console.Write(".")
        End Sub

        ''' <summary>
        ''' 已经排过序了的
        ''' </summary>
        ''' <param name="nsList"></param>
        ''' <returns></returns>
        Public Function QuickNavigation(nsList As String()) As String
            Dim sbr As StringBuilder = New StringBuilder("<strong>", 4096)
            Dim i As Integer = 0
            Dim d As Integer

            On Error Resume Next

            For Each preChar As Char In INDEX
                d = 0

                Do While __getPrefix(nsList(i + d)) = preChar
                    d += 1
                    If i + d >= nsList.Length - 1 Then
                        Exit Do
                    End If
                Loop

                i = i + d

                If d = 0 Then
                    Call sbr.AppendLine($"{preChar} ")
                Else
                    Call sbr.AppendLine($"<a href=""#{nsList(i - d)}"">{preChar}</a> ")
                End If
            Next

            Call sbr.AppendLine("</strong>")

            Return sbr.ToString
        End Function

        Const INDEX As String = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ"

        Private Function __getPrefix(ns As String) As Char
            If String.IsNullOrEmpty(ns) Then
                Return "#"c
            Else
                Dim ch As Char = ns.ToUpper.First
                Dim asc As Integer = AscW(ch)

                If asc >= AscW("A"c) AndAlso asc <= AscW("Z"c) Then
                    Return ch
                Else
                    Return "#"c
                End If
            End If
        End Function

        Public Function Indexing(SPMgrDb As SPM.PackageModuleDb) As Boolean
            Dim Html As New StringBuilder(My.Resources.index)
            Dim doc As New StringBuilder
            Dim Namespaces = (From nsValue As SPM.Nodes.Namespace
                              In SPMgrDb.NamespaceCollection
                              Select nsValue
                              Order By nsValue.Namespace Ascending).ToArray

            doc.AppendLine($"  <p><strong>
                <li>Type reference manual can be found at here: <a href=""./{NameOf(TypeLinks)}/index.html"">[^] TypeLinks Library</a></li>
                           <li>GCModeller is a collaborative project with many contributors. Goto contributor page for more information: <a href=""./contributors.html"">[^] Contributors</a></li></strong>
                </p>
                <p></p>  
            <p>   <br />  
                <br /></p>

                <h2>Installed Modules</h2>
                <p>Currently there are {SPMgrDb.NamespaceCollection.Length} packages({(From ns In SPMgrDb.NamespaceCollection
                                                                                       Select (From api In ns.API Select api.OverloadsNumber).Sum).Sum} API in totally) have been installed on your ShoalShell system:
                </p><p><br /><br />   {QuickNavigation(Namespaces.Select(Function(ns) ns.Namespace).ToArray)}<br /><br /><br />
             <table>
                           <tr>
                            <th></th>
    <th>Package Namespace</th><td> </td><td> </td><td> </td>
    <th>Description</th>
  </tr>")
            For Each ns As SPM.Nodes.Namespace In Namespaces
                Call doc.AppendLine($"<tr>
                        <td><a href=""./{ns.Namespace.NormalizePathString(False)}.html""><strong>[...]</strong></a></td>
    <th align=""left""><a name=""{ns.Namespace}"">{ns.Namespace}</a></th><td> </td><td> </td><td> </td>
    <td align=""left"">{ns.Description}</td>
  </tr>     ")
            Next

            Call doc.AppendLine("</table></p>")
            Call doc.AppendLine("</p>")
            Call doc.AppendLine("</p><a href=""#""><strong><font size=3>[&#8593;]</font></strong></a><hr>")
            Call doc.AppendLine("Here is the language environment that was installed in your shoal system:")

            For Each language As HybridEnvir In SPMgrDb.HybridEnvironments
                Call doc.AppendLine($"<p><li><strong>{language.Language}</strong></li>
                    <br />
                                    {If(Not String.IsNullOrEmpty(language.Description), $"{language.Description}<br />", "")} 
                                    {language.Path}!{language.TypeId}
                    </p>")
            Next

            Call Html.Replace("%Version%", My.Application.Info.Version.ToString)
            Call Html.Replace("%SDK_HELP%", doc.ToString)
            Call Html.Replace("%etc%", "")
            Call ShoalShell.HTML.TypeLinks.Indexing(SPMgrDb)
            Call ShoalShell.HTML.DocRenderer.IndexAuthorsPage(SPMgrDb)

            Return Html.ToString.SaveTo(RequestHtml("index"))
        End Function

        Public Function IndexAuthorsPage(SPMgr As SPM.PackageModuleDb) As Boolean
            Dim HtmlBuilder As StringBuilder = New StringBuilder(My.Resources.index)
            Call HtmlBuilder.Replace("%etc%", "<p><h2><font size=5><span><a href=""./index.html"">< Back To Index</a></span></font></h2></p>")
            Call HtmlBuilder.Replace("%Version%", My.Application.Info.Version.ToString)
            Call HtmlBuilder.Replace("%SDK_HELP%", __contributors(SPMgr))

            Return HtmlBuilder.SaveTo(RequestHtml("contributors"))
        End Function

        Private Function __contributors(SPMgr As SPM.PackageModuleDb) As String
            Dim doc As New StringBuilder(
"GCModeller is a project which is attempting to provide a modern piece of systems biology analysis software for the GNU suite of software.
<br />
<p>GCModeller has three type of user interface(CLI commandline, Workbench GUI, ShoalShell Scripting), and the ShoalShell scripting language project is the one of the user interface for biologist applied GCModeller in their researches.
As a most important part in the GCModeller, the current ShoalShell is the result of a collaborative effort with contributions from all over the world.
<br />

<h2>Authors of ShoalShell</h2>
<p>As the project leader of GCModeller, master xie developed the entired ShoalShell runtime core library and the most part of the GCModeller ShoalShell library packages, he is begins written GCModeller since mid-2013 and start writing ShoalShell from a idea of develop a much commandline like debugging tools for the GCModeller virtual cell system since 2014 August.</p>

Here is the currently developer members on developing GCModeller:<br /><p>
<li>Master xie (<a href=""mailto://xie.guigang@gcmodeller.org"">xie.guigang@gcmodeller.org</a> or just contact him via <a href=""http://xanthomonas.wiki"">Xanthomonas Wikipedia Public Library Program</a>: <a href=""mailto://xie.guigang@xanthomonas.wiki"">xie.guigang@xanthomonas.wiki</a>)</li>
<li>and his research assistant Miss asuka (<a href=""mailto://amethyst.asuka@gcmodeller.org"">amethyst.asuka@gcmodeller.org</a>)</li>
<li>Mr Huahao Jiang (<a href=""mailto://jhh1725@gcmodeller.org"">jhh1725@gcmodeller.org</a>)</li></p><br />

<h2>Contributors to GCModeller Library</h2>
<p>
Here are the peoples who is contributes to the algorithm or source code of GCModeller:
</p>
<br />")
            Call doc.AppendLine("<table>")
            Call doc.AppendLine("<tr>                            
    <th>Author/Contributors</th><th> </th><th>Modules</th>
  </tr>")
            Dim getAuthors = (From authorMod
                                  In (From obj In SPMgr.NamespaceCollection Select (From [mod] In obj.PartialModules Select [mod].Publisher, [mod]).ToArray).ToArray.Unlist
                              Select authorMod, author = InputHandler.ToString(authorMod.Publisher).ToLower.Trim
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

                Call doc.AppendLine($" <tr><td>{authorName}</td> <td> </td><td>{modList}</td> </tr>")
            Next

            Call doc.AppendLine("</table>")
            Call doc.AppendLine("<br /><br />
<p> </p>
<h2>Special thanks</h2>
<p>Many thanks to my teacher and <strong>doctor niu(<a href=""mailto://niuxiangna@gmail.com"">niuxiangna@gmail.com</a>), 
professor Jiang(<a href=""mailto://weijiang@gxu.edu.cn"">weijiang@gxu.edu.cn</a>) and 
professor He(<a href=""mailto://yqhe@gxu.edu.cn"">yqhe@gxu.edu.cn</a>)</strong> from <a href=""http://sklcusa.gxu.edu.cn/"">SKLCUSA Laboratory</a> in Guangxi University, 
for giving good advices on my research and encouraging me devoted myself into the GCModeller project and bringing out this fantastic language project to you.
<br /><br /><p>")
            Call doc.AppendLine($"<h2>License</h2>
<p>
<pre>{My.Resources.license}</pre>
</p>")
            Call doc.AppendLine("<p><br /><br /><br /></p><h2>Project Links</h2>
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
<p><br />
<table>
<tr><th>Shoal Project</th><th>HOME</th><th>Science</th><th>License</th></tr>
<tr><td><li>GCModeller</li></td><td><a href=""http://GCModeller.org"">http://GCModeller.org</a></td><td>Systems Biology/Bioinformatics</td><td>GPL3 Open Source</td></tr>
<tr><td><li>MiMaster</li></td><td><a href=""http://mipaimai.com/"">http://mipaimai.com/</a></td><td>Financial Online Trading System</td><td>Commercial Licensed($32k/Product System)</td></tr>
</table>
</p> </p>

</p>")

            Return doc.ToString
        End Function

        Public Function HtmlRender([Namespace] As SPM.Nodes.Namespace) As String
            Dim htmlTemplate As StringBuilder = New StringBuilder(My.Resources.sdk_doc)
            Dim htmlDoc As New StringBuilder()
            Dim Navigation As New StringBuilder

            Navigation.AppendLine("<table>")

            For Each API As Interpreter.Linker.APIHandler.APIEntryPoint In [Namespace].API
                Call __documents(htmlDoc, API)
                Call Navigation.AppendLine($"  <tr>
    <th align=""left"">
                                           <a href=""#{API.Name}"">{API.Name}</a> </th><td> </td><td> </td><td> </td>")
                Call Navigation.AppendLine($"<th align=""left"">{ String.Join("<br />", (From overloadsAPI In API Select overloadsAPI.EntryPoint.Info).ToArray)}</th>")
                Call Navigation.AppendLine("</tr>")
            Next

            Navigation.AppendLine("</table>")

            Dim Relates = __getRelatedNamespaces([Namespace])

            Call htmlTemplate.Replace("%Related%", String.Join(vbCrLf, (From ns As String In Relates Select $"<li><a href=""./{ns.NormalizePathString(False)}.html"">{ns}</a></li>").ToArray))
            Call htmlTemplate.Replace("%Contents%", Navigation.ToString)
            Call htmlTemplate.Replace("%SDK_HELP%", htmlDoc.ToString)
            Call htmlTemplate.Replace("%Namespace%", [Namespace].Namespace)
            Call htmlTemplate.Replace("%Url%", [Namespace].Url)
            Call htmlTemplate.Replace("%Publisher%", [Namespace].Publisher)
            Call htmlTemplate.Replace("%Description%", [Namespace].Description)
            Call htmlTemplate.Replace("%Cites%", String.Join("<br /><p>", If([Namespace].Cites Is Nothing, New String() {}, [Namespace].Cites)))

            Return htmlTemplate.ToString
        End Function

        Private Function __getRelatedNamespaces([Namespace] As SPM.Nodes.Namespace) As String()
            Dim LQuery = (From pm In [Namespace].PartialModules.AsParallel Let assm = pm.Assembly.LoadAssembly Let types = assm.GetTypes Select types).ToArray.Unlist
            Dim GetNames = (From type In LQuery.AsParallel
                            Let Name = type.NamespaceEntry
                            Where Not Name Is Nothing AndAlso Not String.IsNullOrEmpty(Name.Namespace) AndAlso Not Name.AutoExtract
                            Select nsTag = Name.Namespace.ToLower, Name
                            Group By nsTag Into Group).ToArray
            Return (From obj In GetNames Select str = obj.Group.ToArray()(Scan0).Name.Namespace Order By str Ascending).ToArray
        End Function

        Private Sub __documents(ByRef htmlDoc As StringBuilder, API As Interpreter.Linker.APIHandler.APIEntryPoint)

            If API.IsOverloaded Then
                Call htmlDoc.AppendLine($"<strong>{API.Name}</strong><br />")
                Call htmlDoc.AppendLine($"<strong>+ {API.OverloadsNumber} Overloads</strong><p>")

                For Each cmdEntryPoint In API.OverloadsAPI
                    Call htmlDoc.AppendLine(cmdEntryPoint.EntryPointFullName(True))
                    Call __documents(htmlDoc, cmdEntryPoint)
                Next

            Else
                Call __documents(htmlDoc, API.OverloadsAPI(Scan0))
            End If
        End Sub

        Private Sub __documents(ByRef htmlDoc As StringBuilder, API As CommandLine.Reflection.EntryPoints.APIEntryPoint)
            Dim doc As String = CommandLine.Reflection.ExportAPIAttribute.GenerateHtmlDoc(API, NameOf(API), __functionDetails(API))
            Call htmlDoc.AppendLine($"<li>
    {doc}")
            Call htmlDoc.AppendLine("<br /><p>")
        End Sub

        Private Function __functionDetails(API As CommandLine.Reflection.EntryPoints.APIEntryPoint) As String
            Dim parameters = API.EntryPoint.GetParameters
            Dim doc As New StringBuilder(1024)

            Call doc.AppendLine("<br />")
            Call doc.AppendLine($"<strong>API Prototype</strong>:  {API.EntryPoint.DeclaringType.FullName}::{API.EntryPoint.Name}")
            Call doc.AppendLine("<br />Function Returns: " & MetaData.FunctionReturns.GetDescription(API.EntryPoint))

            If Not parameters.IsNullOrEmpty Then
                Call doc.AppendLine($"<br /><br />Parameters: <br />
<table>{ String.Join("", (From parm In parameters Let aliass = Scripting.MetaData.Parameter.GetParameterNameAlias(parm, True)
                          Select $" 
 <tr>
                              <td align=""left"">{If(aliass.ParameterInfo.IsOptional, "[Optional]", "")}</td>
    <td align=""left""><strong>{aliass.Alias}</strong></td>
    <td align=""left""><a href=""{HTML.TypeLinks.NamespaceRequestHtml(aliass.ParameterInfo.ParameterType)}"">{aliass.ParameterInfo.ParameterType.FullName}</a></td>
<td align=""left"">{aliass.Description}</td>
<td align=""left"">{If(aliass.ParameterInfo.IsOptional, $" = {InputHandler.ToString(aliass.ParameterInfo.DefaultValue)}", "")}</td>
  </tr>").ToArray) }     
</table></p>")
            End If

            Call doc.AppendLine($"<p><strong>Returns</strong>: <a href=""{HTML.TypeLinks.NamespaceRequestHtml(API.EntryPoint.ReturnType)}"">{API.EntryPoint.ReturnType.FullName}</a></p>")

            Return doc.ToString
        End Function
    End Module
End Namespace