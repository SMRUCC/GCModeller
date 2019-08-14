Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory.Network.Extensions
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Metagenomics

Public Module EndPointAnalysis

    ''' <summary>
    ''' 通过细菌的基因组内的KO编号列表查询出相对应的代谢反应过程模型，然后将这些代谢反应过程通过代谢物交点组装出代谢网络
    ''' </summary>
    ''' <param name="KO$">某一个细菌物种的基因组内的KO编号列表可以批量的从Uniprot数据库获取得到</param>
    ''' <param name="reactions">KEGG数据库之中的参考代谢反应列表</param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildInternalNetwork(KO$(), reactions As ReactionRepository) As NetworkGraph
        Dim graph As New NetworkGraph
        Dim getNodes =
            Function(compounds As IEnumerable(Of CompoundSpecieReference))
                Dim nodes As New List(Of Node)

                For Each compound As CompoundSpecieReference In compounds
                    With graph.GetNode(compound.ID)
                        If .IsNothing Then
                            nodes += graph.CreateNode(compound.ID)
                        Else
                            nodes += .ByRef
                        End If
                    End With
                Next

                Return nodes
            End Function

        ' 对当前的这个细菌的基因组内的代谢网络进行装配
        ' 使用已经注释出来的KO编号列表，从参考反应模型库之中查询出相应的模型数据
        For Each link As Reaction In reactions.GetByKOMatch(KO)
            Dim flux = link.ReactionModel

            ' 节点为kegg compounds
            ' 链接的边为kegg reaction
            Dim reactants = getNodes(compounds:=flux.Reactants)
            Dim products = getNodes(compounds:=flux.Products)

            ' 对代谢物之间创建代谢反应连接边
            For Each r In reactants
                For Each p In products
                    With graph.GetEdges(r, p).SafeQuery.ToArray
                        If .IsNullOrEmpty Then
                            Call graph.CreateEdge(r, p)
                        End If
                    End With
                Next
            Next
        Next

        Return graph
    End Function

    <Extension>
    Public Function DoMetabolicEndPointsAnalysis(taxon As TaxonomyRef, reactions As ReactionRepository) As MetabolicEndPoints
        Dim metabolicGraph As NetworkGraph = taxon.genome _
            .Terms _
            .Select(Function(t) t.name) _
            .ToArray _
            .BuildInternalNetwork(reactions)
        Dim endPoints = metabolicGraph.EndPoints

        Return New MetabolicEndPoints With {
            .secrete = endPoints.output.Keys,
            .uptakes = endPoints.input.Keys,
            .taxonomy = taxon.TaxonomyString.ToString
        }
    End Function
End Module
