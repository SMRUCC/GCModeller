﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

''' <summary>
''' Export network visualize model for cytoscape software.
''' </summary>
Public Module NetworkViz

    ''' <summary>
    ''' 这个函数如果没有指定编号列表，则导出所有的pathway的数据
    ''' </summary>
    ''' <param name="cell"></param>
    ''' <param name="pathways$">KO pathway编号列表，应该为格式：``koxxxxxx``</param>
    ''' <returns></returns>
    <Extension>
    Public Function GetPathwayEnzymes(cell As VirtualCell, Optional pathways$() = Nothing) As IEnumerable(Of String)
        If pathways.IsNullOrEmpty Then
            ' 因为有些enzyme可能会没有被归类到某一个代谢途径中
            ' 所以不做pathway筛选的时候就直接从所有的酶分子列表
            ' 之中导出来吧
            Return cell.MetabolismStructure _
                .Enzymes _
                .Select(Function(enzyme) enzyme.geneID)
        Else
            With pathways.Indexing
                Return cell.MetabolismStructure _
                    .maps _
                    .Select(Function(map) map.pathways) _
                    .IteratesALL _
                    .Where(predicate:=Function(pathway)
                                          Return pathway.ID.IsOneOfA(.ByRef)
                                      End Function) _
                    .Select(Function(pathway) pathway.enzymes) _
                    .IteratesALL _
                    .Select(Function(enzyme) enzyme.Comment) _
                    .Distinct
            End With
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cell"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 因为<see cref="CellularModule"/>仅包含有和计算相关的数据，会缺失很多细节信息
    ''' 所以直接使用<see cref="CellularModule"/>作为网络模型的数据源不太合适
    ''' 在这里使用Xml模型来作为统一的虚拟细胞的网络数据模型的来源，而table模型则可以在
    ''' 转换为Xml格式对象之后再调用这个方法产生网络可视化的模型
    ''' </remarks>
    <Extension>
    Public Function CreateGraph(cell As VirtualCell, Optional pathways$() = Nothing) As NetworkTables
        Dim geneNodes As Dictionary(Of Node) = cell.genome _
            .replicons _
            .Select(Function(genome)
                        Return genome.genes _
                            .Select(Function(gene)
                                        ' 因为还会包含有转录调控因子，所以不在这里进行基因的pathway筛选
                                        Return New Node With {
                                            .ID = gene.locus_tag,
                                            .NodeType = "gene",
                                            .Properties = New Dictionary(Of String, String) From {
                                                {"replicon", genome.genomeName}
                                            }
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToDictionary()
        ' 为了简化模型，在这里仅将存在酶的代谢过程取出来
        Dim pathwayEnzymes = cell.GetPathwayEnzymes(pathways).Indexing
        Dim reactionNodes = cell.MetabolismStructure _
            .Enzymes _
            .Where(Function(enzyme)
                       ' 在这里做代谢途径的酶列表的筛选
                       Return enzyme.geneID.IsOneOfA(pathwayEnzymes)
                   End Function) _
            .Select(Function(enzyme)
                        Return enzyme _
                            .catalysis _
                            .Select(Function(catalysis) catalysis.reaction)
                    End Function) _
            .IteratesALL _
            .Distinct _
            .Select(Function(rId)
                        Return New Node With {
                            .ID = rId,
                            .NodeType = "reaction"
                        }
                    End Function) _
            .ToArray
        ' 产生酶分子的网络节点
        Call cell.MetabolismStructure _
            .Enzymes _
            .ForEach(Sub(enzyme, i)
                         ' enzyme的基因肯定存在于所有的基因节点之中
                         ' 在这里只需要做属性的替换就行了
                         geneNodes(enzyme.geneID).NodeType &= "|enzyme"
                     End Sub)
        ' 产生调控因子的网络节点
        Call cell.genome _
            .regulations _
            .GroupBy(Function(reg) reg.regulator) _
            .ForEach(Sub(reg, i)
                         ' 调控因子只有一个家族
                         geneNodes(reg.Key).NodeType &= "|TF"
                         geneNodes(reg.Key).Properties.Add("family", reg.First.motif.family)
                     End Sub)

        Dim enzymeCatalysisEdges = cell.MetabolismStructure _
            .Enzymes _
            .Where(Function(enzyme)
                       ' 在这里做代谢途径的酶列表的筛选
                       Return enzyme.geneID.IsOneOfA(pathwayEnzymes)
                   End Function) _
            .Select(Function(enzyme)
                        Return enzyme.catalysis _
                            .Select(Function(catalysis)
                                        Return New NetworkEdge With {
                                            .FromNode = enzyme.geneID,
                                            .ToNode = catalysis.reaction,
                                            .Interaction = "metabolic_catalysis"
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .AsList

        ' 如果进行代谢途径筛选的话，则删除剩余的gene节点，这些基因节点都不是目标代谢途径的相关的基因
        If Not pathways.IsNullOrEmpty Then
            geneNodes = geneNodes.Values _
                .Where(Function(gene) gene.ID.IsOneOfA(pathwayEnzymes)) _
                .ToDictionary
        End If

        Dim transcriptRegulationEdges = cell.genome _
            .regulations _
            .Where(Function(reg)
                       ' 再上面做了所有基因的代谢途径筛选，在这里将剩余的基因的调控关系挑选出来
                       Return geneNodes.ContainsKey(reg.target)
                   End Function) _
            .Select(Function(reg)
                        Return New NetworkEdge With {
                            .FromNode = reg.regulator,
                            .ToNode = reg.target,
                            .Interaction = "transcript_regulation"
                        }
                    End Function) _
            .ToArray

        ' 生成代谢网络的上下游链接关系
        Dim reactionLinks = cell.MetabolismStructure _
            .Reactions _
            .ToDictionary(Function(r) r.ID,
                          Function(r)
                              Return Equation.TryParse(r.Equation)
                          End Function) _
            .populateReactionLinks(reactionNodes) _
            .ToArray

        Return New NetworkTables With {
            .Nodes = geneNodes.Values.AsList + reactionNodes,
            .Edges = enzymeCatalysisEdges +
                transcriptRegulationEdges +
                reactionLinks
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="equationTable"></param>
    ''' <param name="reactionnodes">
    ''' 这个列表是已经通过代谢途径筛选的列表，如果不在这个列表之中，则这个函数就不会给出边连接
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function populateReactionLinks(equationTable As Dictionary(Of String, Equation), reactionnodes As Node()) As IEnumerable(Of NetworkEdge)
        For Each i As Node In reactionnodes
            Dim iEquation As Equation = equationTable(i.ID)

            For Each j As Node In reactionnodes.Where(Function(n) Not n Is i)
                ' 如果有代谢物的交集，则存在一条边
                Dim jEquation = equationTable(j.ID)

                If iEquation.Products.Any(Function(compound) jEquation.Consume(compound)) Then
                    ' j 消耗 i 的产物
                    Yield New NetworkEdge With {
                        .FromNode = i.ID,
                        .Interaction = "metabolic_link",
                        .ToNode = j.ID
                    }
                ElseIf jEquation.Products.Any(Function(compound) iEquation.Consume(compound)) Then
                    ' i 消耗 j 的产物
                    Yield New NetworkEdge With {
                        .FromNode = j.ID,
                        .Interaction = "metabolic_link",
                        .ToNode = i.ID
                    }
                ElseIf iEquation.GetMetabolites.Keys.Intersect(jEquation.GetMetabolites.Keys).Any Then
                    ' 未知
                    Yield New NetworkEdge With {
                        .FromNode = i.ID,
                        .ToNode = j.ID,
                        .Interaction = "metabolic_link"
                    }
                Else
                    ' no links
                End If
            Next
        Next
    End Function
End Module
