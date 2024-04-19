Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports any = Microsoft.VisualBasic.Scripting

Namespace Interpolate

    Public Module VariableInterpolate

        Public Iterator Function GetVariables(html As String) As IEnumerable(Of NamedValue(Of Object))
            Dim vars As String() = VBHtml.valueExpression.Matches(html).ToArray
            Dim tuple As NamedValue(Of String)
            Dim name As String
            Dim json As JsonElement

            For Each value As String In vars
                value = value.GetStackValue("%", "%").Trim
                tuple = value.GetTagValue("=", trim:=True)
                json = JsonParser.Parse(tuple.Value)
                name = tuple.Name.Trim("@"c)

                If TypeOf json Is JsonValue Then
                    Yield New NamedValue(Of Object)(name, DirectCast(json, JsonValue).value)
                ElseIf TypeOf json Is JsonArray Then
                    Yield New NamedValue(Of Object)(name, DirectCast(json, JsonArray).ToArray)
                Else
                    Yield New NamedValue(Of Object)(name, json)
                End If
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