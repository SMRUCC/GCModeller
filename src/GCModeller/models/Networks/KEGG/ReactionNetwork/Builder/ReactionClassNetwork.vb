Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ReactionNetwork

    Public Class ReactionClassNetwork : Inherits BuilderBase

        ReadOnly classIndex As Index(Of String)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="br08901">代谢反应数据</param>
        ''' <param name="compounds">KEGG化合物编号，``{kegg_id => compound name}``</param>
        Sub New(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String)), reactionClass As IEnumerable(Of ReactionClassTable))
            Call MyBase.New(br08901, compounds, blue)

            classIndex = ReactionClassTable.IndexTable(reactionClass)
        End Sub

        ''' <summary>
        ''' 两个代谢物必须要存在reaction class信息才会添加一条边
        ''' </summary>
        ''' <param name="commonsReactionId"></param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        Protected Overrides Sub createEdges(commonsReactionId() As String, a As Node, b As Node)
            Dim key As String = ReactionClassTable.CreateIndexKey(a.label, b.label)

            ' 没有边连接
            If Not key Like classIndex Then
                Return
            End If

            Dim rid = commonsReactionId.Select(Function(id) networkBase(id)).GetGeneSymbols

            Call New Edge With {
               .U = a,
               .V = b,
               .data = New EdgeData With {
                   .Properties = New Dictionary(Of String, String) From {
                       {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, rid.EC.Distinct.JoinBy(", ")},
                       {"kegg", commonsReactionId.GetJson}
                   }
               },
               .weight = rid.geneSymbols.TryCount
           }.DoCall(AddressOf addNewEdge)
        End Sub
    End Class
End Namespace