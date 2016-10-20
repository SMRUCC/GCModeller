Imports System.Text

Namespace HTML

    Public Module TypeLinks

        Public Function PageName(Type As Type) As String
            Dim arrayType As Type

            Try
                arrayType = Type.Collection2GenericIEnumerable(False)
            Catch ex As Exception
                Call Type.FullName.__DEBUG_ECHO
                Throw
            End Try

            If Type.Equals(arrayType) Then
                Dim Name As String

                If String.Equals(Type.Name, GetType(Generic.KeyValuePair(Of  ,)).Name) Then
                    Dim Generics = Type.GetGenericArguments
                    Name = $"System.Generic.`2+{Generics(Scan0).Name}, {Generics(1).Name}"
                Else
                    Name = Type.FullName  '不是集合类型
                End If

                If Len(Name) > 100 Then
                    Name = SecurityString.MD5Hash.GetMd5Hash(Name.ToLower)
                End If

                Return Name
            Else
                Return $"{NameOf(Collections.IEnumerable)}+{PageName(arrayType.GetGenericArguments(Scan0))}"
            End If
        End Function

        Public Function NamespaceRequestHtml(Type As Type) As String
            Return $"./{NameOf(TypeLinks)}/{PageName(Type)}.html"
        End Function

        Private Function __innerReference(Type As Type) As String
            Return $"./{PageName(Type)}.html"
        End Function

        Public Function Indexing(SPMDb As SPM.PackageModuleDb) As Boolean
            Dim TypeGroups = GetSource(SPMDb)
            Dim __getPages = (From data In TypeGroups
                              Select tdef = TypeLinks.__createPage(data)
                              Order By tdef.FullName Ascending).ToArray

            Dim HTMLBuilder As New StringBuilder(My.Resources.index)
            Dim doc As String = String.Join(vbCrLf, (From type In __getPages Select $"<li><a href=""./{PageName(type)}.html"">{type.FullName}</a></li>").ToArray)

            Call HTMLBuilder.Replace("%Version%", My.Application.Info.Version.ToString)
            Call HTMLBuilder.Replace("%SDK_HELP%", doc)
            Call HTMLBuilder.Replace("%etc%", "<p><h2><font size=5><span><a href=""../index.html"">< Back To Index</a></span></font></h2></p>")
            Call HTMLBuilder.ToString.SaveTo($"{HTML.DocRenderer.wwwroot}/{NameOf(TypeLinks)}/index.html")

            Return Not __getPages.IsNullOrEmpty
        End Function

        Public Function GetSource(SPMDb As SPM.PackageModuleDb) As KeyValuePair(Of SPM.Nodes.Namespace, KeyValuePair(Of Interpreter.Linker.APIHandler.APIEntryPoint, Interpreter.Linker.APIHandler.SignedFuncEntryPoint))()()
            Dim AllAPIs = (From obj As SPM.Nodes.Namespace
                        In SPMDb.NamespaceCollection.AsParallel
                           Select (From api As Interpreter.Linker.APIHandler.APIEntryPoint
                                   In obj.API
                                   Select ns = obj,
                                       api).ToArray).ToArray.Unlist
            Dim AllFunctions = (From obj In AllAPIs.AsParallel Select (From func As Interpreter.Linker.APIHandler.SignedFuncEntryPoint
                                                                       In obj.api.ToArray
                                                                       Select func,
                                                                           obj.api,
                                                                           obj.ns).ToArray).ToArray.Unlist
            Dim TypeGroups = (From obj In AllFunctions.AsParallel
                              Select obj
                              Group obj By Name = PageName(obj.func.EntryPoint.EntryPoint.ReturnType) Into Group).ToArray
            Dim __getPages = (From type In TypeGroups.AsParallel
                              Let data = (From obj In type.Group
                                          Select New KeyValuePair(Of SPM.Nodes.Namespace,
                                              KeyValuePair(Of Interpreter.Linker.APIHandler.APIEntryPoint,
                                              Interpreter.Linker.APIHandler.SignedFuncEntryPoint))(
                                              obj.ns,
                                              New KeyValuePair(Of Interpreter.Linker.APIHandler.APIEntryPoint,
                                              Interpreter.Linker.APIHandler.SignedFuncEntryPoint)(obj.api, obj.func))).ToArray
                              Select data).ToArray
            Return __getPages
        End Function

        Private Function __createPage(data As KeyValuePair(Of SPM.Nodes.Namespace,
                                      KeyValuePair(Of Interpreter.Linker.APIHandler.APIEntryPoint,
                                      Interpreter.Linker.APIHandler.SignedFuncEntryPoint))()) As Type
            Dim template As String = My.Resources.typeLinks_doc
            Dim typeRef As KeyValuePair(Of String, Type) = CreatePage(template, data)
            Dim path As String = $"{HTML.DocRenderer.wwwroot}/{NameOf(TypeLinks)}/{PageName(typeRef.Value)}.html"
            Call typeRef.Key.SaveTo(path)

            Return typeRef.Value
        End Function

        Public Function CreatePage(htmlTemplate As String,
                                   data As KeyValuePair(Of
                                   SPM.Nodes.Namespace,
                                   KeyValuePair(Of Interpreter.Linker.APIHandler.APIEntryPoint,
                                   Interpreter.Linker.APIHandler.SignedFuncEntryPoint))()) As KeyValuePair(Of String, Type)
            Dim Type = data.First.Value.Value.EntryPoint.EntryPoint.ReturnType
            Dim HtmlBuilder As New StringBuilder(htmlTemplate)
            Dim doc As New StringBuilder

            Call doc.AppendLine("<table class=""API"" width=""100%"">")
            Call doc.AppendLine("    <tr>
                            <th width=""250px"">API</th><th>EntryPoint</th>
    <th>Description</th>
  </tr>")
            For Each func In (From obj In data Select obj Order By obj.Key.Namespace Ascending).ToArray
                Call doc.AppendLine($"    <tr>
                            <td><a href=""../{func.Key.Namespace.NormalizePathString}.html#"">{func.Key.Namespace}</a>::<a href=""../{func.Key.Namespace.NormalizePathString}.html#{func.Value.Key.Name}"">{func.Value.Key.Name}</a></td>
                            <td>{func.Value.Value.EntryPoint.EntryPoint.ToString}</td>
    <td>{func.Value.Value.EntryPoint.Info}</td>
  </tr>")
            Next

            Call doc.AppendLine("</table>")

            Call HtmlBuilder.Replace("%Publisher%", String.Join("<br />", (From obj In Type.Assembly.CustomAttributes
                                                                           Let str = obj.ToString
                                                                           Where InStr(str, "System.Reflection.Assembly") > 0
                                                                           Let final = str.Replace("System.Reflection.Assembly", "").Replace("Attribute(", "(")
                                                                           Select If(Len(final) > 128, Mid(final, 1, 128) & "...", final)).ToArray))
            Call HtmlBuilder.Replace("%SDK_HELP%", doc.ToString)
            Call HtmlBuilder.Replace("%Description%", Type.Description)
            Call HtmlBuilder.Replace("%DefineFile%", ProgramPathSearchTool.RelativePath(Type.Assembly.Location))
            Call HtmlBuilder.Replace("%Type%", Type.FullName)

            Return New KeyValuePair(Of String, Type)(HtmlBuilder.ToString, Type)
        End Function
    End Module
End Namespace