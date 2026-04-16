#Region "Microsoft.VisualBasic::2905a512db83708fa589081521b0a8ab, G:/GCModeller/src/runtime/httpd/src/WebCloud/SMRUCC.WebCloud.VBScript//Interpolate/Includes.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 80
    '    Code Lines: 59
    ' Comment Lines: 5
    '   Blank Lines: 16
    '     File Size: 3.48 KB


    '     Module IncludeInterpolate
    ' 
    '         Function: InterpolateFilePath
    ' 
    '         Sub: FillIncludes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
                Dim rel_path As String = reference.GetStackValue("=", If(reference.EndsWith("/>"), "/", "%")).Trim
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
