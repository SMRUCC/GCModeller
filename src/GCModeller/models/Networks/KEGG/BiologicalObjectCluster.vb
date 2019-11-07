Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module BiologicalObjectCluster

    Public Function CompoundsMap(map As Pathway) As NamedCollection(Of String)
        Return New NamedCollection(Of String) With {
            .name = map.EntryId,
            .value = map.compound.Keys
        }
    End Function

    Public Function CompoundsMap(map As Map) As NamedCollection(Of String)
        Return New NamedCollection(Of String) With {
            .name = map.id,
            .value = map.shapes _
                .Select(Function(a) a.IDVector) _
                .IteratesALL _
                .Where(Function(id) id.IsPattern("C\d+")) _
                .Distinct _
                .ToArray
        }
    End Function

    Public Function ReactionMap(map As Map) As NamedCollection(Of String)
        Return New NamedCollection(Of String) With {
            .name = map.id,
            .value = map.shapes _
                .Select(Function(a) a.IDVector) _
                .IteratesALL _
                .Where(Function(id) id.IsPattern("R\d+")) _
                .Distinct _
                .ToArray
        }
    End Function
End Module
