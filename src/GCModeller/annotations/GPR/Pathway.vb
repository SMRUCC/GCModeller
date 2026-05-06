Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.MetabolicModel

Public Class Pathway : Inherits MetabolicPathway

    ''' <summary>
    ''' 反应网络
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ReactionNetwork As NetworkGraph

    Sub New(network As IReadOnlyCollection(Of MetabolicReaction))
        Dim g As New NetworkGraph

        For Each u As MetabolicReaction In network
            Dim right = u.right.ToDictionary(Function(specie) specie.ID)
            Dim uNode As Node = g.GetElementByID(u.id)

            If uNode Is Nothing Then
                uNode = g.CreateNode(u.id)
            End If

            For Each v As MetabolicReaction In network
                If u Is v Then
                    Continue For
                End If

                Dim vNode As Node = g.GetElementByID(v.id)

                If vNode Is Nothing Then
                    vNode = g.CreateNode(v.id)
                End If

                If v.left.Any(Function(specie) right.ContainsKey(specie.ID)) Then
                    Call g.CreateEdge(uNode, vNode)
                End If
            Next
        Next

        metabolicNetwork = network.ToArray
        ReactionNetwork = g
    End Sub

    Public Shared Iterator Function FromKEGGPathways(pathways As IEnumerable(Of Map), reactions As IEnumerable(Of Reaction)) As IEnumerable(Of Pathway)
        Dim reactionIndex As Dictionary(Of String, MetabolicReaction) = reactions _
            .Where(Function(r)
                       ' 20260506 filter out the possible empty equation
                       Return r IsNot Nothing AndAlso r.ID <> "" AndAlso r.Equation <> ""
                   End Function) _
            .GroupBy(Function(r) r.ID) _
            .ToDictionary(Function(r) r.Key,
                          Function(r)
                              Return KEGGConvertor.ConvertReaction(r.First)
                          End Function)
        Dim bar As ProgressBar = Nothing

        Call "processing on build reference pathway map from kegg database...".info

        For Each map As Map In TqdmWrapper.WrapIterator(pathways, bar:=bar)
            Dim rxnIDs As String() = map.GetMembers.Where(Function(id) reactionIndex.ContainsKey(id)).ToArray
            Dim network As MetabolicReaction() = rxnIDs.Select(Function(id) reactionIndex(id)).ToArray

            Call bar.SetLabel(map.name)

            Yield New Pathway(network) With {
                .ID = map.EntryId,
                .metabolicNetwork = network,
                .metabolites = map _
                    .GetCompoundSet _
                    .Select(Function(c)
                                Return New MetabolicCompound With {.id = c.Name, .name = c.Value}
                            End Function) _
                    .ToArray,
                .name = map.name
            }
        Next
    End Function

End Class
