#If Not DisableCsvTemplate Then
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Text
#End If
Namespace Settings

    Module Templates

#If Not DisableCsvTemplate Then
        Public Sub WriteExcelTemplate(Of T)()
            Dim path$ = App.HOME & $"/Templates/{App.AssemblyName}/{GetType(T).Name}.csv"

            If Not path.FileLength > 0 Then
                Call (New T() {}).SaveTo(path,, Encodings.ASCII.CodePage)
            End If
        End Sub
#End If
    End Module
End Namespace