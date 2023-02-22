Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace Metabolism.Metpa

    Public Class rbc

        Public Property data As Double()
        Public Property kegg_id As String()

    End Class

    Public Class rbcList

        Public Property list As Dictionary(Of String, rbc)

        Public Shared Function calcRbc(a As NamedValue(Of NetworkGraph)) As NamedValue(Of rbc)
            Dim cnodes As Node() = a.Value.vertex _
                .Where(Function(c)
                           Return c.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "KEGG Compound"
                       End Function) _
                .ToArray
            Dim cid = cnodes.Select(Function(c) c.label).ToArray
            Dim rbcVal As Double() = cnodes _
                .Select(Function(c)
                            Return c.data(NamesOf.REFLECTION_ID_MAPPING_RELATIVE_BETWEENESS_CENTRALITY)
                        End Function) _
                .Select(AddressOf Val) _
                .ToArray
            Dim rbc As New rbc With {
                .data = rbcVal,
                .kegg_id = cid
            }

            Return New NamedValue(Of rbc)(a.Name, rbc)
        End Function
    End Class
End Namespace