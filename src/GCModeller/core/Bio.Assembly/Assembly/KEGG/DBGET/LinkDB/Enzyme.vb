Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.LinkDB

    Public Module Enzyme

        Public Function PageUrl(RefSeq As String) As String
            Return $"https://www.genome.jp/dbget-bin/get_linkdb?-t+enzyme+rs:{RefSeq}"
        End Function

        Public Function DoGetEnzymeList(refSeq$, Optional cache$ = "./cache/") As NamedValue()
            Return GenericParser.LinkDbEntries(PageUrl(refSeq), cache)
        End Function
    End Module
End Namespace