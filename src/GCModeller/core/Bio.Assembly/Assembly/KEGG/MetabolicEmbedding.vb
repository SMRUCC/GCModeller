Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Namespace Assembly.KEGG

    Public Class MetabolicEmbedding

        ReadOnly maps As BriteTerm()

        Sub New()
            maps = BriteHText.Load_ko00001.Deflate("\d+").ToArray
        End Sub

        Public Function MakeVector(ec As IEnumerable(Of String)) As Dictionary(Of String, Double)
            Dim vec As New Dictionary(Of String, Double)

            Return vec
        End Function

    End Class
End Namespace