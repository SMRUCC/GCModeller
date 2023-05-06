Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

Namespace Metabolism.Metpa

    Public Class dgr

        Public Property kegg_id As String()
        Public Property dgr As Double()
        Public Property network_id As String

        Public Function GetImpacts() As Dictionary(Of String, Double)
            Dim impact As New Dictionary(Of String, Double)

            For i As Integer = 0 To kegg_id.Length - 1
                impact.Add(kegg_id(i), dgr(i))
            Next

            Return impact
        End Function

    End Class

    Public Class dgrList : Implements TopologyScoreProvider

        Public Property pathways As Dictionary(Of String, dgr)

        Public Shared Function calcDgr(graphs As NamedValue(Of NetworkGraph)(), multipleOmics As Boolean) As Dictionary(Of String, dgr)
            Return graphs _
                .Select(Function(g) dgrList.calcDgr(g, multipleOmics)) _
                .ToDictionary(Function(map) map.Name,
                              Function(map)
                                  Return map.Value
                              End Function)
        End Function

        Public Shared Function calcDgr(a As NamedValue(Of NetworkGraph), multipleOmics As Boolean) As NamedValue(Of dgr)
            Dim vlist As Node() = a.Value.vertex _
                .Where(Function(c)
                           If multipleOmics Then
                               Return True
                           Else
                               Return c.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = CompoundNodeTable.KEGGCompoundNodeType
                           End If
                       End Function) _
                .ToArray
            Dim cid = vlist.Select(Function(c) c.label).ToArray
            Dim dgrVal As Vector = vlist _
                .Select(Function(c)
                            Return c.data(NamesOf.REFLECTION_ID_MAPPING_RELATIVE_DEGREE_CENTRALITY)
                        End Function) _
                .Select(AddressOf Val) _
                .ToArray
            Dim dgr As New dgr With {
                .dgr = dgrVal / dgrVal.Sum,
                .kegg_id = cid,
                .network_id = a.Name
            }

            Return New NamedValue(Of dgr)(a.Name, dgr)
        End Function

        Public Function GetScoreImpacts(mapid As String) As Dictionary(Of String, Double) Implements TopologyScoreProvider.GetScoreImpacts
            Return pathways(mapid).GetImpacts
        End Function
    End Class
End Namespace