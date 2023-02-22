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

    End Class

    Public Class dgrList

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
            Dim cnodes As Node() = a.Value.vertex _
                .Where(Function(c)
                           If multipleOmics Then
                               Return True
                           Else
                               Return c.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = CompoundNodeTable.KEGGCompoundNodeType
                           End If
                       End Function) _
                .ToArray
            Dim cid = cnodes.Select(Function(c) c.label).ToArray
            Dim dgrVal As Vector = cnodes _
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
    End Class
End Namespace