Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

Namespace Metabolism.Metpa

    Public Class rbc

        Public Property data As Double()
        Public Property kegg_id As String()
        Public Property network_id As String

    End Class

    Public Class rbcList

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
            Dim rbcVal As Double() = vlist _
                .Select(Function(c)
                            Return c.data(NamesOf.REFLECTION_ID_MAPPING_RELATIVE_BETWEENESS_CENTRALITY)
                        End Function) _
                .Select(AddressOf Val) _
                .ToArray
            Dim rbc As New rbc With {
                .data = rbcVal,
                .kegg_id = cid,
                .network_id = a.Name
            }

            Return New NamedValue(Of rbc)(a.Name, rbc)
        End Function
    End Class
End Namespace