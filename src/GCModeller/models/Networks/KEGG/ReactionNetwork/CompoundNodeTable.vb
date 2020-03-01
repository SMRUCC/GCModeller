Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace ReactionNetwork

    Public Class CompoundNodeTable

        ReadOnly nodes As Dictionary(Of Node)
        ReadOnly g As NetworkGraph

        Public ReadOnly Property values As IEnumerable(Of Node)
            Get
                Return nodes.Values
            End Get
        End Property

        Default Public ReadOnly Property getNode(id As String) As Node
            Get
                Return nodes(id)
            End Get
        End Property

        Sub New(compounds As IEnumerable(Of NamedValue(Of String)), cpdGroups As Dictionary(Of String, String()), g As NetworkGraph, color As Brush)
            nodes = compounds _
                .Where(Function(cpd) Not cpd.Name Like ReactionNetworkBuilder.commonIgnores) _
                .Select(Function(cpd As NamedValue(Of String))
                            Return createCompoundNode(cpd, cpdGroups, color)
                        End Function) _
                .ToDictionary
            nodes.Values _
                .DoEach(AddressOf g.AddNode)

            Me.g = g
        End Sub

        Public Function containsKey(nodeLabelId As String) As Boolean
            Return nodes.ContainsKey(nodeLabelId)
        End Function

        Private Shared Function createCompoundNode(cpd As NamedValue(Of String), cpdGroups As Dictionary(Of String, String()), color As Brush) As Node
            Dim type$ = "n/a"

            If cpdGroups.ContainsKey(cpd.Name) Then
                type = cpdGroups(cpd.Name).JoinBy("|")
            End If

            Return New Node With {
                .label = cpd.Name,
                .data = New NodeData With {
                    .label = cpd.Name,
                    .origID = cpd.Value,
                    .color = color,
                    .Properties = New Dictionary(Of String, String) From {
                        {"common_name", cpd.Value},
                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "KEGG Compound"},
                        {"related", type}
                    }
                }
            }
        End Function

        Public Function add(compoundNode As Node) As Node
            Call nodes.Add(compoundNode)
            Call g.AddNode(compoundNode)

            Return compoundNode
        End Function
    End Class
End Namespace