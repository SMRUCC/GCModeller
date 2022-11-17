Imports System.Xml.Serialization

Namespace Assembly.KEGG.DBGET.bGetObject

    <XmlType("geneName")>
    Public Class GeneName

        <XmlAttribute> Public Property geneId As String
        <XmlAttribute> Public Property geneName As String
        <XmlText> Public Property description As String
        <XmlAttribute> Public Property KO As String
        <XmlAttribute> Public Property EC As String()

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
            text = text.Replace(KO, "")
            EC = text.Replace(EC, "")
            text = text.Trim
            gene.description = text
            gene.KO = KO.Match("K\d+")
            gene.EC = EC.GetStackValue("[", "]") _
                .Split(":"c) _
                .LastOrDefault _
                .StringSplit("\s+")

            Return gene
        End Function

        Public Overrides Function ToString() As String
            Return description
        End Function

    End Class
End Namespace