Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' compound filter algorithm of the pathway maps
''' </summary>
Public Module UniqueRank

    ''' <summary>
    ''' the more pathway of one compound occurs in, the less unique rank of the compound it have
    ''' </summary>
    ''' <param name="pathwayProfile"></param>
    ''' <returns></returns>
    Public Function EvaluateUniqueRank(pathwayProfile As IEnumerable(Of Pathway)) As DataSet()
        Dim maps As Pathway() = pathwayProfile.ToArray
        Dim allCompounds As String() = maps _
            .Select(Function(a) a.compound.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim occurs As Dictionary(Of String, String()) = allCompounds _
            .ToDictionary(Function(a) a,
                          Function(a)
                              Return maps _
                                  .Where(Function(p)
                                             Return p.IsContainsCompound(a)
                                         End Function) _
                                  .Select(Function(p) p.EntryId) _
                                  .ToArray
                          End Function)


    End Function
End Module
