#Region "Microsoft.VisualBasic::76dad73d0ea63b70502f8acf5ade35ff, G:/GCModeller/src/runtime/httpd/src/WebCloud/SMRUCC.WebCloud.VBScript//Interpolate/Variable.vb"

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

    '   Total Lines: 99
    '    Code Lines: 76
    ' Comment Lines: 5
    '   Blank Lines: 18
    '     File Size: 3.68 KB


    '     Module VariableInterpolate
    ' 
    '         Function: AutoCastJsonValue, GetValueString, GetVariables
    ' 
    '         Sub: (+2 Overloads) FillVariable, FillVariables
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports any = Microsoft.VisualBasic.Scripting

Namespace Interpolate

    Public Module VariableInterpolate

        Friend Function AutoCastJsonValue(json As JsonElement) As Object
            If TypeOf json Is JsonValue Then
                Return DirectCast(json, JsonValue).value
            ElseIf TypeOf json Is JsonArray Then
                Return DirectCast(json, JsonArray).ToArray
            Else
                Return json
            End If
        End Function

        ''' <summary>
        ''' Parse the variable definitions inside the html template file
        ''' </summary>
        ''' <param name="html"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' syntax for define the variable inside the html template file:
        ''' 
        ''' &lt;% @variableName=value %&gt;
        ''' 
        ''' or
        ''' 
        ''' &lt;% @variableName=value /&gt;
        ''' </remarks>
        Public Iterator Function GetVariables(html As String) As IEnumerable(Of NamedValue(Of Object))
            Dim vars As String() = VBHtml.valueExpression.Matches(html).ToArray
            Dim tuple As NamedValue(Of String)
            Dim name As String
            Dim json As JsonElement
            Dim any As Object
            Dim raw_str As String

            For Each value As String In vars
                raw_str = value

                If value.EndsWith("/>") Then
                    value = value.GetStackValue("%", "/").Trim
                Else
                    value = value.GetStackValue("%", "%").Trim
                End If

                tuple = value.GetTagValue("=", trim:=True)
                name = tuple.Name.Trim("@"c)

                Try
                    json = JsonParser.Parse(tuple.Value, strictVectorSyntax:=False)
                    any = AutoCastJsonValue(json)
                Catch ex As Exception
                    ' is not a valid json string
                    ' treated as plain/text string 
                    any = tuple.Value
                End Try

                Yield New NamedValue(Of Object)(name, any, describ:=raw_str)
            Next
        End Function

        Public Sub FillVariables(vbhtml As VBHtml)
            Dim vars As String() = VBHtml.variable _
                .Matches(vbhtml.ToString) _
                .ToArray

            For Each var As String In vars.OrderByDescending(Function(name) name.Length)
                Call FillVariable(vbhtml, var.Trim("@"c))
            Next
        End Sub

        Private Sub FillVariable(<Out> ByRef vbhtml As VBHtml, name As String)
            Dim tokens As String() = name.Split("."c)

            If Not vbhtml.HasSymbol(tokens(Scan0)) Then
                ' skip rendering current symbol due to the reason of missing such
                ' symbol inside the rendering source data list.
                Return
            End If

            If tokens.Length = 1 Then
                ' use tostring
                vbhtml(name) = vbhtml.GetString(tokens(0))
            Else
                FillVariable(vbhtml, name, vbhtml.GetSymbol(tokens(0)))
            End If
        End Sub

        Private Sub FillVariable(vbhtml As VBHtml, name As String, obj As Object)
            vbhtml(name) = GetValueString(obj, name)
        End Sub

        Public Function GetValueString(obj As Object, name As String) As String
            Dim tokens As String() = name.Split("."c)

            For Each item As String In tokens.Skip(1)
                obj = Reflection.GetValue.Read(obj, item)
            Next

            If TypeOf obj Is JsonValue Then
                Return any.ToString(DirectCast(obj, JsonValue).value, "")
            Else
                Return any.ToString(obj, "")
            End If
        End Function
    End Module
End Namespace
