Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Text

Namespace Settings

    Module Templates

        Public Sub WriteExcelTemplate(Of T)()
            Dim path$ = App.HOME & $"/Templates/{App.AssemblyName}/{GetType(T).Name}.csv"

            If Not path.FileLength > 0 Then
                Call (New T() {}).SaveTo(path,, Encodings.ASCII.CodePage)
            End If
        End Sub
    End Module
End Namespace