Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

''' <summary>
''' Export network visualize model for cytoscape software.
''' </summary>
Public Module NetworkViz

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
    Public Function CreateGraph(cell As VirtualCell) As NetworkTables
        Dim geneNodes = cell.Genome _
            .genes _
            .Select(Function(gene)
                        Return New Node With {
                            .ID = gene.locus_tag,
                            .NodeType = "gene"
                        }
                    End Function) _
            .ToDictionary()
        ' 为了简化模型，在这里仅将存在酶的代谢过程取出来
        Dim reactionNodes = cell.MetabolismStructure _
            .Enzymes _
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
        Call cell.Genome _
            .regulations _
            .GroupBy(Function(reg) reg.regulator) _
            .ForEach(Sub(reg, i)
                         ' 调控因子只有一个家族
                         geneNodes(reg.Key).NodeType &= "|TF"
                         geneNodes(reg.Key).Properties.Add("family", reg.First.motif.family)
                     End Sub)

        Dim enzymeCatalysisEdges = cell.MetabolismStructure _
            .Enzymes _
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
        Dim transcriptRegulationEdges = cell.Genome _
            .regulations _
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
            .Edges = enzymeCatalysisEdges + transcriptRegulationEdges + reactionLinks
        }
    End Function

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
