#Region "Microsoft.VisualBasic::70c8d7e5c24c7c334b7d95132dcb0e73, engine\Compiler\NetworkViz.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 215
    '    Code Lines: 155 (72.09%)
    ' Comment Lines: 48 (22.33%)
    '    - Xml Docs: 54.17%
    ' 
    '   Blank Lines: 12 (5.58%)
    '     File Size: 9.90 KB


    ' Module NetworkViz
    ' 
    '     Function: CreateGraph, GetPathwayEnzymes, populateReactionLinks
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

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
            Return cell.metabolismStructure _
                .enzymes _
                .Select(Function(enzyme) enzyme.proteinID)
        Else
            With pathways.Indexing

            End With
        End If
    End Function

    <Extension>
    Private Sub CreateGeneNodes(cell As VirtualCell, ByRef g As NetworkGraph)
        For Each genome As replicon In cell.genome.replicons
            For Each gene In genome.GetGeneList
                If g.GetElementByID(gene.locus_tag) Is Nothing Then
                    Call g.CreateNode(gene.locus_tag, New NodeData With {
                        .label = gene.product,
                        .origID = gene.locus_tag,
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "gene"},
                            {"recplicon", genome.genomeName}
                        }
                    })
                End If
            Next
        Next
    End Sub

    <Extension>
    Private Iterator Function CreateReactionNodes(cell As VirtualCell, g As NetworkGraph, pathwayEnzymes As Index(Of String)) As IEnumerable(Of Node)
        Dim rIds As New List(Of String)

        For Each enzyme In cell.metabolismStructure.enzymes
            ' 在这里做代谢途径的酶列表的筛选
            If Not (enzyme.proteinID Like pathwayEnzymes) Then
                Continue For
            End If

            Call rIds.AddRange(enzyme.catalysis.Select(Function(catalysis) catalysis.reaction))
        Next

        For Each rId As String In rIds.Distinct
            If g.GetElementByID(rId) Is Nothing Then
                Call g.CreateNode(rId, New NodeData With {
                    .label = rId,
                    .origID = rId,
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "reaction"}
                    }
                })

                Yield g.GetElementByID(rId)
            End If
        Next
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
    Public Function CreateGraph(cell As VirtualCell, Optional pathways$() = Nothing) As NetworkGraph
        Dim g As New NetworkGraph With {
            .id = cell.cellular_id,
            .name = cell.taxonomy.ToString
        }

        Call cell.CreateGeneNodes(g)
        ' 为了简化模型，在这里仅将存在酶的代谢过程取出来
        Dim pathwayEnzymes As Index(Of String) = cell.GetPathwayEnzymes(pathways).Indexing

        Dim reactionnodes As Node() = cell.CreateReactionNodes(g, pathwayEnzymes).ToArray

        ' 产生酶分子的网络节点
        For Each enzyme In cell.metabolismStructure.enzymes
            ' enzyme的基因肯定存在于所有的基因节点之中
            ' 在这里只需要做属性的替换就行了
            Dim gene = g.GetElementByID(enzyme.proteinID)
            Dim nodeType As String = gene(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)

            nodeType &= "|enzyme"
            gene.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = nodeType
        Next

        ' 产生调控因子的网络节点
        For Each reg In cell.genome.regulations.GroupBy(Function(regl) regl.regulator)
            ' 调控因子只有一个家族
            Dim gene = g.GetElementByID(reg.Key)
            Dim nodeType As String = gene(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)

            nodeType &= "|TF"
            gene.data.Add("family", reg.First.motif.family)
            gene.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = nodeType
        Next

        Call cell.metabolismStructure.enzymes _
            .Where(Function(enzyme)
                       ' 在这里做代谢途径的酶列表的筛选
                       Return enzyme.proteinID Like pathwayEnzymes
                   End Function) _
            .ForEach(Sub(enzyme)
                         For Each catalysis In enzyme.catalysis
                             Call g.CreateEdge(enzyme.proteinID, catalysis.reaction, data:=New EdgeData With {
                                   .Properties = New Dictionary(Of String, String) From {
                                       {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "metabolic_catalysis"}
                                   }
                             })
                         Next
                     End Sub)

        Dim geneNodes As New Dictionary(Of String, Node)

        ' 如果进行代谢途径筛选的话，则删除剩余的gene节点，这些基因节点都不是目标代谢途径的相关的基因
        If Not pathways.IsNullOrEmpty Then
            geneNodes = g.vertex _
                .Where(Function(gene) gene.label Like pathwayEnzymes) _
                .ToDictionary
        End If

        Call cell.genome.regulations _
            .Where(Function(reg)
                       ' 再上面做了所有基因的代谢途径筛选，在这里将剩余的基因的调控关系挑选出来
                       Return reg.centralDogma.Any(Function(id) geneNodes.ContainsKey(id))
                   End Function) _
            .ForEach(Sub(reg)
                         For Each id As String In reg.centralDogma
                             Call g.CreateEdge(reg.regulator, id, data:=New EdgeData With {
                                .Properties = New Dictionary(Of String, String) From {
                                    {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "transcript_regulation"}
                                }
                             })
                         Next
                     End Sub)

        ' 生成代谢网络的上下游链接关系
        Call cell.metabolismStructure _
            .reactions _
            .AsEnumerable _
            .ToDictionary(Function(r) r.ID,
                          Function(r)
                              Return Equation.TryParse(r.equation)
                          End Function) _
            .populateReactionLinks(g, reactionnodes)

        Return g
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="equationTable"></param>
    ''' <param name="reactionnodes">
    ''' 这个列表是已经通过代谢途径筛选的列表，如果不在这个列表之中，则这个函数就不会给出边连接
    ''' </param>
    <Extension>
    Private Sub populateReactionLinks(equationTable As Dictionary(Of String, Equation), g As NetworkGraph, reactionnodes As Node())
        For Each i As Node In reactionnodes
            Dim iEquation As Equation = equationTable(i.ID)

            For Each j As Node In reactionnodes.Where(Function(n) Not n Is i)
                ' 如果有代谢物的交集，则存在一条边
                Dim jEquation = equationTable(j.ID)

                If iEquation.Products.Any(Function(compound) jEquation.Consume(compound)) Then
                    ' j 消耗 i 的产物
                    Call g.CreateEdge(i.ID, j.ID, data:=New EdgeData With {.Properties = New Dictionary(Of String, String) From {{NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "metabolic_link"}}})
                ElseIf jEquation.Products.Any(Function(compound) iEquation.Consume(compound)) Then
                    ' i 消耗 j 的产物
                    Call g.CreateEdge(j.ID, i.ID, data:=New EdgeData With {.Properties = New Dictionary(Of String, String) From {{NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "metabolic_link"}}})
                ElseIf iEquation.GetMetabolites.Keys.Intersect(jEquation.GetMetabolites.Keys).Any Then
                    ' 未知
                    Call g.CreateEdge(i.ID, j.ID, data:=New EdgeData With {.Properties = New Dictionary(Of String, String) From {{NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "metabolic_link"}}})
                Else
                    ' no links
                End If
            Next
        Next
    End Sub
End Module
