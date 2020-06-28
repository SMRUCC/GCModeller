Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

Public Module ggiBuilder

    ''' <summary>
    ''' apply for LC-MS data analysis
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <param name="reactions"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function ReferenceCompoundNetwork(maps As IEnumerable(Of Map), reactions As IEnumerable(Of ReactionTable)) As IEnumerable(Of GraphEdge)
        Dim fluxIndex As Dictionary(Of String, ReactionTable) = reactions _
            .GroupBy(Function(r) r.entry) _
            .ToDictionary(Function(r) r.Key,
                          Function(r)
                              Return r.First
                          End Function)
        Dim fluxEntries As String()
        Dim compoundEntries As String()
        Dim allId As String()
        Dim mapName As String

        For Each map As Map In maps
            allId = map.GetMembers
            fluxEntries = allId.Where(Function(id) id.IsPattern("R\d+")).ToArray
            mapName = $"{map.id}: {map.Name.Replace(" - Reference pathway", "").Trim}"

            For Each reaction As ReactionTable In fluxEntries _
                .Where(AddressOf fluxIndex.ContainsKey) _
                .Select(Function(id)
                            Return fluxIndex(id)
                        End Function)

                compoundEntries = reaction.substrates.AsList + reaction.products

                For Each a As String In compoundEntries
                    For Each b As String In compoundEntries.Where(Function(id) id <> a)
                        Yield New GraphEdge With {
                            .pathwayID = mapName,
                            .g1 = a,
                            .g2 = b
                        }
                    Next
                Next
            Next
        Next
    End Function
End Module
