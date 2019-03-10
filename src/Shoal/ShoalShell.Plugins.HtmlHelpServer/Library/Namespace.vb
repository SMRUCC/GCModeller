Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.DocumentFormat.HTML
Imports Microsoft.VisualBasic.Linq.Extensions

''' <summary>
''' 生成API模块的帮助文档
''' </summary>
Module [Namespace]

    Public Sub GenerateDocument(repository As String, library As SPM.Nodes.Namespace())
        Dim IndexFiles = (From obj In (From x In library Select (From xx In x.PartialModules Select xx.Assembly.Path, xx).ToArray).ToArray.MatrixToList
                          Select obj.Path, obj.xx
                          Group By Path Into Group) _
                             .ToDictionary(Function(x) x.Path, elementSelector:=Function(x) x.Group.ToArray(Function(xx) xx.xx))

        For Each ns As SPM.Nodes.Namespace In library
            Dim HTMLBuilder As New StringBuilder(My.Resources.index)
            Dim HTML As String = HTMLDoc(repository, ns, IndexFiles)
            Dim path As String = $"{repository}/{ns.Namespace.NormalizePathString(False)}.html"

            Call HTMLBuilder.Replace(Title, ns.Namespace)
            Call HTMLBuilder.Replace(Doc, HTML)
            Call HTMLBuilder.SaveTo(path, System.Text.Encoding.UTF8)

            Call $"   ===> {ns.Namespace}...".__DEBUG_ECHO
        Next
    End Sub

    Public Function HTMLDoc(repository As String, ns As SPM.Nodes.Namespace, related As Dictionary(Of String, SPM.Nodes.PartialModule())) As String
        Dim HTMLBuilder As StringBuilder = New StringBuilder(1024)
        Call HTMLBuilder.AppendLine($"<a href=""./index.html"">Document Library</a> > {ns.Namespace}")
        Call HTMLBuilder.AppendLine("<br />")
        Call HTMLBuilder.AppendLine("<h2>Related Namespace</h2>")
        For Each entry In ns.PartialModules
            Dim Name As String = FileIO.FileSystem.GetFileInfo(entry.Assembly.Path).Name
            Dim path As String = $"{repository}/{Name}/{ns.Namespace.NormalizePathString(False)}.html"
            Call HTMLBuilder.AppendLine($"<li><a href=""./{Name}/{ns.Namespace.NormalizePathString(False)}.html"">{Name}</a></li>")
            Dim relates = related(entry.Assembly.Path)
            Call HTMLBuilder.AppendLine("<table class=""API"" width=""100%"">
<tr><td width=""200px""><strong>Namespace</strong></td><td><strong>Description</strong></td></tr>")
            For Each nss In relates
                Dim href As String = $"./{FileIO.FileSystem.GetFileInfo(nss.Assembly.Path).Name}/{nss.Namespace}.html"
                Call HTMLBuilder.AppendLine($"<tr><td><a href=""{href}"">{nss.Namespace}</a></td><td>{nss.Description}</td></tr>")
            Next
            Call HTMLBuilder.AppendLine("</table>")

            Dim innerDoc As New StringBuilder(My.Resources.index)
            Call innerDoc.RestoreCSS
            Call innerDoc.Replace(Doc, HTMLDoc(entry, ns.Namespace))
            Call innerDoc.Replace(Title, ns.Namespace)
            Call innerDoc.SaveTo(path, System.Text.Encoding.UTF8)
        Next
        Return HTMLBuilder.ToString
    End Function

    Public Function HTMLDoc(ns As SPM.Nodes.PartialModule, name As String) As String
        Dim HTMLBuilder As StringBuilder = New StringBuilder(1024)
        Call HTMLBuilder.AppendLine($"<a href=""../index.html"">Document Library</a> > <a href=""../{name}.html"">{name}</a>")
        Call HTMLBuilder.AppendLine($"<h1>{name}</h1>")
        Call HTMLBuilder.AppendLine($"<table class=""API"" width=""100%"">
<tr><td width=""100px"">Publishers</td><td>{ns.Publisher}</td></tr>
<tr><td>Description</td><td>{ns.Description}</td></tr>
<tr><td>Revision</td><td>{ns.Revision}</td></tr>
<tr><td>Cites</td><td>{ns.GetCites}</td></tr>
<tr><td>Category</td><td>{ns.Category.ToString} ({ns.Category.Description})</td></tr>
<tr><td>URL</td><td><a href=""{ns.Url}"">{ns.Url}</a></td></tr>
<tr><td>Library</td><td>{ns.Assembly.TypeId}</td></tr>
</table>")

        If ns.EntryPoints.IsNullOrEmpty Then
            GoTo _EXIT
        End If

        Dim entryPoints = Scripting.ShoalShell.SPM.Nodes.AssemblyParser.Imports(ns.Assembly.GetType)

        Call HTMLBuilder.AppendLine("<h4>API List</h4>")

        For Each api In entryPoints
            Call HTMLBuilder.AppendLine($"<li><a href=""#{api.Name}"">{api.Name}</a></li>")
        Next

        Call HTMLBuilder.AppendLine("<hr><br /><br />")

        For Each api As Interpreter.Linker.APIHandler.APIEntryPoint In entryPoints

            Call HTMLBuilder.AppendLine($"<h4><a name=""{api.Name}"" href=""#"">{api.Name}{If(api.IsOverloaded, $"(+{api.OverloadsNumber} Overloads)", "")}</a></h4>")
            Call HTMLBuilder.AppendLine("<p>")

            For Each overloadsAPI In api.OverloadsAPI

                Dim rtvl = overloadsAPI.EntryPoint.GetAttribute(Of FunctionReturns)

                Call HTMLBuilder.AppendLine($"<h5><strong>{overloadsAPI.Name}</strong></h5>")
                Call HTMLBuilder.AppendLine($"<table class=""API"" width=""100%"">
<tr><td width=""100px"">Description</td><td>{overloadsAPI.Info.InvokeReplace("<", "&lt;")}</td></tr>
<tr><td>Return</td><td>{If(rtvl Is Nothing, "", rtvl.Description.InvokeReplace("<", "&lt;"))}</td></tr>
<tr><td>Usage</td><td>{overloadsAPI.Usage.InvokeReplace("<", "&lt;")}</td></tr>
</table>")
                Call HTMLBuilder.AppendLine("API Details")
                Call HTMLBuilder.AppendLine($"<table class=""API"" width=""100%"">
<tr><td width=""100px"">Prototype</td><td>{overloadsAPI.EntryPointFullName(True)}</td></tr>
<tr><td>Returns</td><td>{overloadsAPI.EntryPoint.ReturnType.FullName}</td></tr>
</table>")

                If ns.Category = APICategories.CLI_MAN Then
                    Call __buildCLIArguments(overloadsAPI, HTMLBuilder)
                Else
                    Call __buildAPIParameters(overloadsAPI, HTMLBuilder)
                End If
            Next

            Call HTMLBuilder.AppendLine("</p>")
        Next

_EXIT:  Return HTMLBuilder.ToString
    End Function

    Private Sub __buildAPIParameters(overloadsAPI As CommandLine.Reflection.EntryPoints.APIEntryPoint, ByRef HTMLBuilder As StringBuilder)
        Dim lstParams = overloadsAPI.EntryPoint.GetParameters

        If lstParams.IsNullOrEmpty Then
            Call HTMLBuilder.AppendLine("Not Required of parameters.")
            Return
        End If

        Call HTMLBuilder.AppendLine("Parameters")
        Call HTMLBuilder.AppendLine("<table class=""API"" width=""100%"">
<tr><td width=""100px"">Name</td><td>Description</td><td>Type</td></tr>")

        For Each param As Reflection.ParameterInfo In lstParams
            Dim describ = param.GetCustomAttributes(attributeType:=GetType(Scripting.MetaData.Parameter), inherit:=True)
            Dim attr = If(describ.IsNullOrEmpty, Nothing, DirectCast(describ(Scan0), Parameter))
            Call HTMLBuilder.AppendLine($"<tr>
<td>{If(attr Is Nothing, param.Name, attr.Alias.InvokeReplace("<", "&lt;"))}</td>
<td>{If(attr Is Nothing, "", attr.Description.InvokeReplace("<", "&lt;"))}</td>
<td>{param.ParameterType.FullName}</td>
</tr>")
        Next

        Call HTMLBuilder.AppendLine("</table>")
    End Sub

    Private Sub __buildCLIArguments(overloadsAPI As CommandLine.Reflection.EntryPoints.APIEntryPoint, ByRef HTMLBuilder As StringBuilder)
        Dim args As CommandLine.Reflection.ParameterInfo() =
            overloadsAPI.EntryPoint.GetCustomAttributes(GetType(CommandLine.Reflection.ParameterInfo), inherit:=True)
        If args.IsNullOrEmpty Then
            Call HTMLBuilder.AppendLine("Probably Not Required of parameters, please read <strong><i>Usage</i></strong> for help.")
            Return
        End If

        Call HTMLBuilder.AppendLine("Parameters")
        Call HTMLBuilder.AppendLine("<table class=""API"" width=""100%"">
<tr><td width=""90px"">Name</td><td width=""80px"">Optional</td><td>Description</td></tr>")

        For Each param As CommandLine.Reflection.ParameterInfo In args
            Call HTMLBuilder.AppendLine($"<tr>
<td>{param.Name.InvokeReplace("<", "&lt;")}</td>
<td>{param.Optional.ToString }</td>
<td>{DocFormatter.HighlightLinks(param.Description).InvokeReplace("<", "&lt;")}</td>
</tr>")
        Next

        Call HTMLBuilder.AppendLine("</table>")
    End Sub

    <Extension> Public Function InvokeReplace(source As String, old As String, _new As String) As String
        If String.IsNullOrEmpty(source) Then
            Return ""
        Else
            Return source.Replace(old, _new)
        End If
    End Function
End Module
