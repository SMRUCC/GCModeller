Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
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
    ''' <param name="type">
    ''' 0. all
    ''' 1. prokaryote
    ''' 2. eukaryotes
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("fetch.kegg_organism")>
    Public Function FetchKEGGOrganism(Optional resource$ = "http://www.kegg.jp/kegg/catalog/org_list.html", Optional type As Integer = 0) As Prokaryote()
        Dim result As KEGGOrganism = EntryAPI.FromResource(resource)
        Dim eukaryotes As List(Of Prokaryote) = result.Eukaryotes _
            .Select(Function(x)
                        Return New Prokaryote(x)
                    End Function) _
            .AsList

        If type = 0 Then
            Return result.Prokaryote + eukaryotes
        ElseIf type = 1 Then
            Return result.Prokaryote
        ElseIf type = 2 Then
            Return eukaryotes
        Else
            Return {}
        End If
    End Function

    <ExportAPI("save.kegg_organism")>
    Public Function SaveKEGGOrganism(organism As Prokaryote(), file$) As Boolean
        Return organism.SaveTo(file)
    End Function

    <ExportAPI("read.kegg_organism")>
    Public Function ReadKEGGOrganism(file As String) As Prokaryote()
        Return file.LoadCsv(Of Prokaryote)
    End Function
End Module
