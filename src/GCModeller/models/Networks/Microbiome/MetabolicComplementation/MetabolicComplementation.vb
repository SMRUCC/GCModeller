#Region "Microsoft.VisualBasic::bc563f4b57d2ff246d22a76042470ede, models\Networks\Microbiome\MetabolicComplementation\MetabolicComplementation.vb"

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

    '   Total Lines: 203
    '    Code Lines: 137 (67.49%)
    ' Comment Lines: 43 (21.18%)
    '    - Xml Docs: 53.49%
    ' 
    '   Blank Lines: 23 (11.33%)
    '     File Size: 9.23 KB


    ' Module MetabolicComplementation
    ' 
    '     Function: BuildInternalNetwork, BuildMicrobiomeMetabolicNetwork
    ' 
    '     Sub: FetchModels, link, linkNodes, RenderColors
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Data.GraphTheory.Network.Extensions
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' 微生物组营养互补网络，在这个模块之中节点为微生物，网络的边为互补或者竞争的营养物
''' 
''' 计算原理如下:
''' 
''' 1. 对于一个微生物的基因组,可以通过注释得到KO列表,通过KO列表可以从KEGG数据库中查询出
'''    相关联的代谢反应过程信息
''' 2. 则可以通过所查询得到的代谢反应过程信息构建出一个在该目标微生物细胞内可能存在的代谢反应网络
''' 3. 对这个所构建的代谢网络进行网络端点分析
''' 4. 通过端点分析得到的输入端,物质是该微生物所不能够自行合成的;
'''    通过端点分析得到的输出端,该物质是该微生物可能分泌的次级代谢物
''' 5. 根据端点分析结果即可构建出微生物之间的营养物质竞争与互补关系
''' 6. 假若两个微生物在某一个端点代谢物上,都是输入端,则可能在该代谢物上存在竞争关系
''' 7. 假若两个微生物在某一个端点代谢物中,是上下游关系,则可能存在该代谢物上的互补关系
''' </summary>
Public Module MetabolicComplementation

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function BuildInternalNetwork(taxon As TaxonomyRef, reactions As ReactionRepository) As NetworkGraph
        Return taxon.genome _
            .Terms _
            .Select(Function(t) t.name) _
            .ToArray _
            .BuildInternalNetwork(reactions)
    End Function

    ''' <summary>
    ''' 构建出所给定的微生物组的代谢物互补与营养竞争网络
    ''' </summary>
    ''' <param name="metagenome">从Uniprot上批量下载的基因组蛋白注释数据</param>
    ''' <param name="reactions">KEGG参考代谢反应模型库</param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildMicrobiomeMetabolicNetwork(metagenome As IEnumerable(Of TaxonomyRef), reactions As ReactionRepository) As NetworkGraph
        Dim graph As New NetworkGraph

        ' 在微生物组的营养互补竞争网络之中
        ' 节点为微生物的基因组编号
        ' 链接的边为营养物关系
        Call "Fetch UniProt reference genome model data...".debug
        Call graph.FetchModels(metagenome, reactions)
        Call graph.RenderColors

        ' 在构建完了所有的基因组的代谢网络的输入和输出端点之后
        ' 开始装配营养互补和竞争网络
        Call "Link microbiome metabolic network...".debug
        Call graph.linkNodes

        Return graph
    End Function

    ''' <summary>
    ''' Set category colors
    ''' </summary>
    ''' <param name="graph"></param>
    <Extension> Private Sub RenderColors(graph As NetworkGraph)
        Dim families$() = graph.vertex _
            .Select(Function(n) n.data!Family) _
            .Distinct _
            .ToArray
        Dim colors$() = Designer _
            .GetColors(
                term:="scibasic.category31()",
                n:=families.Length
            ) _
            .Select(Function(c) c.ToHtmlColor) _
            .ToArray
        Dim colorRender As Dictionary(Of String, String) = families _
            .SeqIterator _
            .ToDictionary(Function(family) family.value, Function(i) colors(i))

        For Each node As Node In graph.vertex
            With node.data
                !Color = colorRender(!Family)
            End With
        Next
    End Sub

    <Extension>
    Private Sub FetchModels(graph As NetworkGraph, metagenome As IEnumerable(Of TaxonomyRef), reactions As ReactionRepository)
        For Each genome As TaxonomyRef In metagenome
            Dim metabolicNetwork As NetworkGraph = genome.BuildInternalNetwork(reactions)
            ' 添加该基因组的节点
            Dim node As Node = graph.CreateNode(genome.organism.scientificName)
            Dim endPoints = metabolicNetwork.EndPoints
            Dim family$ = genome.TaxonomyString.BIOMTaxonomyString(TaxonomyRanks.Family)

            ' 将该微生物的代谢网络端点写入缓存之中
            With node.data
                !Family = family
                .ItemValue(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = family

                ' 当前的这个基因组所必须的营养物，无法进行自身的合成
                !Essential_nutrients = endPoints _
                    .input _
                    .Select(Function(n) n.data.label) _
                    .ToArray _
                    .GetJson

                ' 当前的这个基因组所能够合成的次生代谢物网络终点
                !Secondary_metabolite = endPoints _
                    .output _
                    .Select(Function(n) n.data.label) _
                    .ToArray _
                    .GetJson
            End With

            Call genome.ToString.info
        Next
    End Sub

    <Extension> Private Sub link(genome As Node, graph As NetworkGraph)
        Dim Ainput$() = genome _
            .data _
            !Essential_nutrients _
            .LoadJSON(Of String())
        Dim Aoutput$() = genome _
            .data _
            !Secondary_metabolite _
            .LoadJSON(Of String())

        For Each member As Node In graph.vertex _
            .Where(Function(n)
                       ' 忽略掉自身对自身的边连接，无意义
                       Return Not n Is genome
                   End Function)

            Dim B = member.data

            ' 通过查看A和B的输入输出端点是否有重合来了解二者是否存在营养互补的关系
            ' A input vs B output
            Dim Boutput = B!Secondary_metabolite.LoadJSON(Of String())

            With Ainput.Intersect(Boutput).ToArray
                If Not .IsNullOrEmpty Then
                    ' 输入与输出有重叠部分，则可能存在营养互补
                    Dim complementary = graph.CreateEdge(member, genome)
                    complementary.data!compounds = .GetJson
                    complementary.isDirected = True
                    complementary.weight = .Length
                    complementary.data.label = $"{member.Label} => {genome.Label}"
                End If
            End With

            ' A output vs B input
            Dim Binput = B!Essential_nutrients.LoadJSON(Of String())

            With Binput.Intersect(Aoutput).ToArray
                If Not .IsNullOrEmpty Then
                    ' 输入与输出有重叠部分，则可能存在营养互补
                    Dim complementary = graph.CreateEdge(genome, member)
                    complementary.data!compounds = .GetJson
                    complementary.isDirected = True
                    complementary.weight = .Length
                    complementary.data.label = $"{genome.Label} => {member.Label}"
                    complementary.data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE) = NameOf(complementary)
                End If
            End With

            ' 通过查看A和B的输入输入端点是否有重合来了解二者是否存在营养竞争的关系
            ' A input vs B input
            With Ainput.Intersect(Binput).ToArray
                If Not .IsNullOrEmpty Then
                    ' 两个基因组的代谢网络输入端点存在重叠的部分，则可能存在营养竞争关系
                    Dim competition = graph.CreateEdge(genome, member)
                    competition.data!compounds = .GetJson
                    competition.isDirected = False
                    competition.weight = .Length
                    competition.data.label = $"{genome.Label} vs {member.Label}"
                    competition.data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE) = NameOf(competition)
                End If
            End With
        Next
    End Sub

    <Extension> Private Sub linkNodes(graph As NetworkGraph)
        Using progress As New ProgressBar("Link networks...", 1, CLS:=True)
            Dim ticks As New ProgressProvider(progress, graph.vertex.Count)
            Dim msg$

            For Each genome As Node In graph.vertex
                genome.link(graph)
                msg$ = $"ETA={ticks.ETA().FormatTime}  // {genome.data.label}"
                progress.SetProgress(ticks.StepProgress, msg)
            Next
        End Using
    End Sub
End Module
