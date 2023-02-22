Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace Metabolism.Metpa

    Public Class dgr

        Public Property kegg_id As String()
        Public Property dgr As Double()

    End Class

    Public Class dgrList

        Public Property pathways As Dictionary(Of String, dgr)

        Public Shared Function calcDgr(graphs As NamedValue(Of NetworkGraph)()) As Dictionary(Of String, dgr)
            Return graphs _
                .Select(AddressOf dgrList.calcDgr) _
                .ToDictionary(Function(map) map.Name,
                              Function(map)
                                  Return map.Value
                              End Function)
        End Function

        Public Shared Function calcDgr(a As NamedValue(Of NetworkGraph)) As NamedValue(Of dgr)
            Dim cnodes As Node() = a.Value.vertex _
                .Where(Function(c)
                           Return c.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "KEGG Compound"
                       End Function) _
                .ToArray
            Dim cid = cnodes.Select(Function(c) c.label).ToArray
            Dim rbcVal As Double() = cnodes _
                .Select(Function(c)
                            Return c.data(NamesOf.REFLECTION_ID_MAPPING_RELATIVE_DEGREE_CENTRALITY)
                        End Function) _
                .Select(AddressOf Val) _
                .ToArray
            Dim dgr As New dgr With {
                .dgr = rbcVal,
                .kegg_id = cid
            }

            Return New NamedValue(Of dgr)(a.Name, dgr)
        End Function
    End Class
End Namespace