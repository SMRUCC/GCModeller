Imports System.Text

Namespace Assembly.MetaCyc.File

    Public Class RecordLine

        Public Property Data As String()

        Sub New()
        End Sub

        Sub New(line As String)
            Data = Strings.Split(line, vbTab)
        End Sub

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder =
                    New StringBuilder(128)

            For i As Integer = 0 To Data.Length - 1
                If String.Equals(Data(i), String.Empty) Then
                    sBuilder.Append("NULL, ")
                Else
                    sBuilder.AppendFormat("{0}, ", Data(i))
                End If
            Next

            Return sBuilder.ToString
        End Function
    End Class
End Namespace