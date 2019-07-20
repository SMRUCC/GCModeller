#Region "Microsoft.VisualBasic::f189f3fdbdbc851171938d5f6afd9857, models\Networks\Microbiome\MetabolicComplementation.vb"

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

' Module MetabolicComplementation
' 
'     Function: (+2 Overloads) BuildInternalNetwork, BuildMicrobiomeMetabolicNetwork
' 
'     Sub: FetchModels, link, linkNodes, RenderColors
' 
' /********************************************************************************/

#End Region

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

''' <summary>
''' 微生物组营养互补网络，在这个模块之中节点为微生物，网络的边为互补或者竞争的营养物
''' </summary>
Public Module MetabolicComplementation

    ''' <summary>
    ''' 通过细菌的基因组内的KO编号列表查询出相对应的代谢反应过程模型，然后将这些代谢反应过程通过代谢物交点组装出代谢网络
    ''' </summary>
    ''' <param name="KO$">某一个细菌物种的基因组内的KO编号列表可以批量的从Uniprot数据库获取得到</param>
    ''' <param name="reactions">KEGG数据库之中的参考代谢反应列表</param>
    ''' <returns></returns>
    <Extension> Public Function BuildInternalNetwork(KO$(), reactions As ReactionRepository) As NetworkGraph
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
                    With graph.GetEdges(r, p)
                        If .IsNullOrEmpty Then
                            Call graph.CreateEdge(r, p)
                        End If
                    End With
                Next
            Next
        Next

        Return graph
    End Function

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
        Call "Fetch UniProt reference genome model data...".__DEBUG_ECHO
        Call graph.FetchModels(metagenome, reactions)
        Call graph.RenderColors

        ' 在构建完了所有的基因组的代谢网络的输入和输出端点之后
        ' 开始装配营养互补和竞争网络
        Call "Link microbiome metabolic network...".__DEBUG_ECHO
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

            Call genome.ToString.__INFO_ECHO
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
            Dim ticks As New ProgressProvider(graph.vertex.Count)
            Dim msg$

            For Each genome As Node In graph.vertex
                genome.link(graph)
                msg$ = $"ETA={ticks.ETA(progress.ElapsedMilliseconds).FormatTime}  // {genome.data.label}"
                progress.SetProgress(ticks.StepProgress, msg)
            Next
        End Using
    End Sub
End Module
