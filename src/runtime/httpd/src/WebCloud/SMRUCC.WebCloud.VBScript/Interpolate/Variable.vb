Namespace Interpolate

    Public Module VariableInterpolate

        Public Sub FillVariables(vbhtml As VBHtml)
            Dim vars As String() = VBHtml.variable _
                .Matches(vbhtml.ToString) _
                .ToArray

            For Each var As String In vars.OrderByDescending(Function(name) name.Length)
                Call FillVariable(vbhtml, var)
            Next
        End Sub

        Private Sub FillVariable(vbhtml As VBHtml, name As String)

        End Sub
    End Module
End Namespace