Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

Namespace Metabolism.Metpa

    Public Class rbc

        Public Property data As Double()
        Public Property kegg_id As String()
        Public Property network_id As String

        Public Function GetImpacts() As Dictionary(Of String, Double)
            Dim impact As New Dictionary(Of String, Double)

            For i As Integer = 0 To kegg_id.Length - 1
                impact.Add(kegg_id(i), data(i))
            Next

            Return impact
        End Function

    End Class

    Public Class rbcList : Implements TopologyScoreProvider

        Public Property list As Dictionary(Of String, rbc)

        Public Shared Function calcRbc(graphs As NamedValue(Of NetworkGraph)(), multipleOmics As Boolean) As Dictionary(Of String, rbc)
            Return graphs _
                .Select(Function(g) rbcList.calcRbc(g, multipleOmics)) _
                .ToDictionary(Function(map) map.Name,
                              Function(map)
                                  Return map.Value
                              End Function)
        End Function

        Public Shared Function calcRbc(a As NamedValue(Of NetworkGraph), multipleOmics As Boolean) As NamedValue(Of rbc)
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
            Dim rbcVal As Vector = vlist _
                .Select(Function(c)
                            Return c.data(NamesOf.REFLECTION_ID_MAPPING_RELATIVE_BETWEENESS_CENTRALITY)
                        End Function) _
                .Select(AddressOf Val) _
                .ToArray
            Dim rbc As New rbc With {
                .data = rbcVal / rbcVal.Sum,
                .kegg_id = cid,
                .network_id = a.Name
            }

            Return New NamedValue(Of rbc)(a.Name, rbc)
        End Function

        Public Function GetScoreImpacts(mapid As String) As Dictionary(Of String, Double) Implements TopologyScoreProvider.GetScoreImpacts
            Return list(mapid).GetImpacts
        End Function
    End Class
End Namespace