
Imports Microsoft.VisualBasic.Text

Namespace Assembly.KEGG.DBGET

    ''' <summary>
    ''' KEGG Orthology
    ''' </summary>
    Public Class KOrthology

        Public Property KO_id As String
        Public Property geneNames As String()
        Public Property [function] As String

        Const EC_pattern As String = "\[EC[:]\d+.+\]"

        ' K00001  E1.1.1.1, adh; alcohol dehydrogenase [EC:1.1.1.1]
        ' K00004  BDH, butB; (R,R)-butanediol dehydrogenase / meso-butanediol dehydrogenase / diacetyl reductase [EC:1.1.1.4 1.1.1.- 1.1.1.303]

        Public ReadOnly Property EC_number As String()
            Get
                Return [function].Match(EC_pattern, RegexICSng) _
                    .GetStackValue("[", "]") _
                    .Split(":"c) _
                    .LastOrDefault _
                    .StringSplit("\s+")
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{KO_id}{vbTab}{geneNames.JoinBy(", ")}; {[function]}"
        End Function

        Public Shared Iterator Function ParseText(list_ko As String) As IEnumerable(Of KOrthology)
            For Each line As String In list_ko.LineTokens
                Dim t As String() = line.Split(ASCII.TAB)
                Dim ko_id As String = t(0)

                t = t(1).Split(";"c)

                Yield New KOrthology With {
                    .KO_id = ko_id,
                    .geneNames = t(0).StringSplit("\s*,\s+"),
                    .[function] = t.Skip(1).JoinBy(";")
                }
            Next
        End Function

        ''' <summary>
        ''' request of the KEGG orthology class data via the kegg rest api
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function RequestKEGG() As IEnumerable(Of KOrthology)
            Return ParseText("https://rest.kegg.jp/list/ko".GET)
        End Function

    End Class
End Namespace