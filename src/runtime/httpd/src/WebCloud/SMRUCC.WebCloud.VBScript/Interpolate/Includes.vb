
Namespace Interpolate

    Module IncludeInterpolate

        Public Sub FillIncludes(vbhtml As VBHtml)
            Dim includes As String() = VBHtml.partialIncludes _
                .Matches(vbhtml.ToString) _
                .ToArray

            ' each includes is also a new vbhtml template file
            ' do render and get the html text as the value
            For Each reference As String In includes

            Next
        End Sub
    End Module
End Namespace