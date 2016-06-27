Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    Public Class DBLINK : Inherits KeyWord

        Public Property Db As String
        Public Property Id As String

        Public Overrides Function ToString() As String
            Return $"DBLINK      {Db}: {Id}"
        End Function

        Friend Shared Function Parser(sList As String()) As DBLINK
            Dim s As String = If(sList.IsNullOrEmpty, "", sList.FirstOrDefault)

            If String.IsNullOrEmpty(s) Then
                Return New DBLINK With {.Db = "unknown"}
            End If

            Dim tokens As String() = Mid(s, 13).Trim.Split(":"c)
            Return New DBLINK With {
                .Db = tokens.Get(Scan0).Trim,
                .Id = tokens.Get(1).Trim
            }
        End Function
    End Class
End Namespace