Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' compound filter algorithm of the pathway maps
''' </summary>
Public Module UniqueRank

    Public Function EvaluateUniqueRank(pathwayProfile As IEnumerable(Of Pathway)) As DataSet()
        Dim maps As Pathway() = pathwayProfile.ToArray
        Dim allCompounds As String() = maps.Select(Function(a) a)

    End Function
End Module
