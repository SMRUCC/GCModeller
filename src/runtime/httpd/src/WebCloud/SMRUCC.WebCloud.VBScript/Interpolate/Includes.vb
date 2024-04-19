Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text

Namespace Interpolate

    Module IncludeInterpolate

        Public Sub FillIncludes(vbhtml As VBHtml)
            Dim includes As String() = VBHtml.partialIncludes _
                .Matches(vbhtml.ToString) _
                .ToArray
            Dim wwwroot As String = vbhtml.filepath.ParentPath
            Dim codepage As Encodings = TextEncodings.GetEncodings(vbhtml.encoding)
            Dim html As String

            ' each includes is also a new vbhtml template file
            ' do render and get the html text as the value
            For Each reference As String In includes
                Dim rel_path As String = reference.GetStackValue("=", "%").Trim
                Dim full_path As String = (wwwroot & "/" & InterpolateFilePath(vbhtml, rel_path)).GetFullPath

                Select Case full_path.ExtensionSuffix
                    Case "vbhtml"
                        html = VBHtml.ReadHTML(full_path, vbhtml.variables, encoding:=codepage)
                    Case "html", "txt", "svg"
                        ' just read and replace
                        html = full_path.ReadAllText
                    Case "png", "jpg", "jpeg", "bitmap", "gif"
                        ' read as base64 encoded data uri string
                        html = New DataURI(full_path).ToString
                    Case "json"
                        ' read and load into variables
                        Dim json_str As String = full_path.ReadAllText
                        Dim json As JsonElement = JsonParser.Parse(json_str, strictVectorSyntax:=False)

                        If Not TypeOf json Is JsonObject Then
                            Throw New InvalidProgramException($"the resource data for interpolate in json file should be in json object key-value tuple format!")
                        End If

                        Dim vars As JsonObject = json

                        For Each name As String In vars.ObjectKeys
                            Call vbhtml.AddSymbol(name, VariableInterpolate.AutoCastJsonValue(vars(name)))
                        Next

                        html = ""
                    Case Else
                        Throw New NotImplementedException($"read file for interpolate of file '{full_path}' has not been implemeneted!")
                End Select

                Call vbhtml.Replace(reference, html)
            Next
        End Sub

        Private Function InterpolateFilePath(vbhtml As VBHtml, path As String) As String
            Dim variables As String() = VBHtml.variable.Matches(path).ToArray

            For Each var As String In variables
                Dim key As String = var.Trim("@"c)

                If Not vbhtml.HasSymbol(key) Then
                    Continue For
                End If

                Dim value As String = vbhtml.GetString(key)
                Dim wrap As String = $"{{@{key}}}"

                If path.IndexOf(wrap) > -1 Then
                    path = path.Replace(wrap, value)
                Else
                    path = path.Replace(var, value)
                End If
            Next

            Return path
        End Function
    End Module
End Namespace