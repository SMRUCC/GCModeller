Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports any = Microsoft.VisualBasic.Scripting

Namespace Interpolate

    Public Module VariableInterpolate

        Public Iterator Function GetVariables(html As String) As IEnumerable(Of NamedValue(Of Object))
            Dim vars As String() = VBHtml.valueExpression.Matches(html).ToArray

            For Each value As String In vars

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
            Dim tokens As String() = name.Split("."c)

            For Each item As String In tokens.Skip(1)
                obj = Reflection.GetValue.Read(obj, item)
            Next

            vbhtml(name) = any.ToString(obj, "")
        End Sub
    End Module
End Namespace