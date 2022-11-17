Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class GeneName

        Public Property geneId As String
        Public Property geneName As String
        Public Property description As String
        Public Property KO As String
        Public Property EC As String()

        Friend Shared Function Parse(text As String) As GeneName
            Dim data = text.GetTagValue(" ", trim:=True, failureNoName:=False)
            Dim geneId As String = data.Name
            Dim gene As New GeneName With {.geneId = geneId}
            Dim KO As String
            Dim EC As String

            data = data.Value.GetTagValue(";", trim:=True, failureNoName:=False)
            gene.geneName = data.Name
            text = data.Value
            KO = text.Match("\[KO[:](K\d+\s*)+\]")
            EC = text.Match("\[EC[:]([\d.\s]+)\]")

            Return gene
        End Function

    End Class
End Namespace