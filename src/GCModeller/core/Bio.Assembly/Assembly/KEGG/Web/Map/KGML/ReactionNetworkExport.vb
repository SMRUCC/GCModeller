Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.WebServices.KGML

    Public Class ReactionNetworkExport

        Public Property substrateId As String()
        Public Property substrateName As String()
        Public Property productId As String()
        Public Property productName As String()
        Public Property reaction As String
        Public Property ko As String()
        Public Property koName As String

        Public Property mapId As String
        Public Property mapName As String

        Public Shared Iterator Function ExtractFromKGML(kgml As pathway) As IEnumerable(Of ReactionNetworkExport)
            Dim koIndex = kgml.entries _
                .Where(Function(a) a.type = "ortholog") _
                .Select(Function(k)
                            Return k.reaction.StringSplit(" ").Select(Function(rid) (rid, k))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(r) r.rid) _
                .ToDictionary(Function(r) r.Key,
                              Function(r)
                                  Return r.Select(Function(a) a.k).ToArray
                              End Function)

            If kgml.reactions Is Nothing Then
                Return
            End If

            For Each rxn As reaction In kgml.reactions
                Dim subs = rxn.substrates.Select(Function(si) si.name.GetTagValue(":").Value).ToArray
                Dim prod = rxn.products.Select(Function(si) si.name.GetTagValue(":").Value).ToArray
                Dim ko = koIndex.TryGetValue(rxn.name).SafeQuery.Select(Function(kid) kid.name).IteratesALL.Distinct.ToArray
                Dim koNames = ko.Select(Function(kid) kid.GetTagValue(":").Value).Select(Function(kid) GeneNetworkExport.koNames(kid)).ToArray

                Yield New ReactionNetworkExport With {
                    .ko = ko,
                    .koName = koNames.JoinBy("; "),
                    .reaction = rxn.name,
                    .substrateId = subs,
                    .substrateName = subs.Select(Function(cid) GeneNetworkExport.cpdNames(cid)).ToArray,
                    .productId = prod,
                    .productName = prod.Select(Function(cid) GeneNetworkExport.cpdNames(cid)).ToArray,
                    .mapId = kgml.name,
                    .mapName = kgml.title
                }
            Next
        End Function

    End Class
End Namespace