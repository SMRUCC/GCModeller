Imports System.Text.RegularExpressions
Imports System.Xml.Serialization

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 参考文献
    ''' </summary>
    Public Class Reference

        <XmlElement> Public Property Authors As String()
        <XmlText> Public Property Title As String
        <XmlAttribute> Public Property Journal As String
        <XmlAttribute> Public Property PMID As String

        Public Shared Function References(data As String()) As Reference()
            Dim LQuery = (From str As String In data Select Reference(str)).ToArray
            Return LQuery
        End Function

        Const REF_ITEM As String = "<td .+?</div></td></tr>"

        Public Shared Function Reference(str As String) As Reference
            Dim Tokens As String() = (From m As Match In Regex.Matches(str, REF_ITEM) Select m.Value).ToArray
            Tokens = (From s As String In Tokens Select Regex.Match(s, "<div .+?>.+?</div>").Value).ToArray

            Dim p As Integer
            Dim PMID As String = Tokens.Get(p.MoveNext).GetValue
            Dim Authors As String = Tokens.Get(p.MoveNext).GetValue
            Dim Title As String = Tokens.Get(p.MoveNext).GetValue
            Dim Journal As String = Tokens.Get(p.MoveNext).GetValue

            If Regex.Match(PMID, "PMID[:]<a").Success Then
                PMID = PMID.GetValue
            End If

            Return New Reference With {
                .Authors = Strings.Split(Authors, ", "),
                .Title = Title,
                .Journal = Journal,
                .PMID = PMID
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{ String.Join(", ", Authors) }. {Title}. {Journal}.  PMID:{PMID}"
        End Function
    End Class
End Namespace