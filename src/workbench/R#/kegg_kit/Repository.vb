Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

<Package("kegg.repository")>
Public Module Repository

    <ExportAPI("load.maps.index")>
    Public Function LoadMapIndex(repository As String) As Dictionary(Of String, Map)
        Return MapRepository.GetMapsAuto(repository).ToDictionary(Function(map) map.id)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="resource$"></param>
    ''' <returns></returns>
    <ExportAPI("fetch.kegg_organism")>
    Public Function FetchKEGGOrganism(Optional resource$ = "http://www.kegg.jp/kegg/catalog/org_list.html") As Prokaryote()
        Dim result As KEGGOrganism = EntryAPI.FromResource(resource)
        Dim table As List(Of Prokaryote) = result.Prokaryote.AsList + result.Eukaryotes.Select(Function(x) New Prokaryote(x))

        Return table.ToArray
    End Function
End Module
