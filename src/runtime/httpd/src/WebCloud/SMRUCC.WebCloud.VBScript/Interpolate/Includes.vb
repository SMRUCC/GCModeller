Imports Microsoft.VisualBasic.Text

Namespace Interpolate

    Module IncludeInterpolate

        Public Sub FillIncludes(vbhtml As VBHtml)
            Dim includes As String() = VBHtml.partialIncludes _
                .Matches(vbhtml.ToString) _
                .ToArray

            ' each includes is also a new vbhtml template file
            ' do render and get the html text as the value
            For Each reference As String In includes
                Dim rel_path As String = reference.GetStackValue("=", "%").Trim
                Dim full_path As String = (vbhtml.filepath.ParentPath & "/" & rel_path).GetFullPath
                Dim html As String = VBHtml.ReadHTML(full_path,
                    wwwroot:=vbhtml.wwwroot,
                    encoding:=TextEncodings.GetEncodings(vbhtml.encoding))

                Call vbhtml.Replace(reference, html)
            Next
        End Sub
    End Module
End Namespace