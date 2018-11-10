Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Linq
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
            .ForEach(Sub(reg, i)
                         geneNodes(reg.regulator).NodeType &= "|TF"
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

        Return New NetworkTables With {
            .Nodes = geneNodes.Values.AsList + reactionNodes,
            .Edges = enzymeCatalysisEdges + transcriptRegulationEdges
        }
    End Function
End Module
