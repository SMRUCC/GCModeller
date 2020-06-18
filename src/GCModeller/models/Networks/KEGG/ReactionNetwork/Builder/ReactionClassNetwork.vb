Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace ReactionNetwork

    Public Class ReactionClassNetwork : Inherits BuilderBase

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="br08901">代谢反应数据</param>
        ''' <param name="compounds">KEGG化合物编号，``{kegg_id => compound name}``</param>
        Sub New(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String)))
            Call MyBase.New(br08901, compounds, blue)
        End Sub

        ''' <summary>
        ''' 两个代谢物必须要存在reaction class信息才会添加一条边
        ''' </summary>
        ''' <param name="commonsReactionId"></param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        Protected Overrides Sub createEdges(commonsReactionId() As String, a As Node, b As Node)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace