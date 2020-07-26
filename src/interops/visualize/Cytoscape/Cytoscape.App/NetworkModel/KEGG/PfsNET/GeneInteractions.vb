#Region "Microsoft.VisualBasic::2d71d31c0bf70373cfd03c22f39c218a, visualize\Cytoscape\Cytoscape.App\NetworkModel\KEGG\PfsNET\GeneInteractions.vb"

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

    '     Module GeneInteractions
    ' 
    '         Function: __exportPathwayGraph, __getRxnRels, __kModNetwork, CreateNetworkObject, (+3 Overloads) ExportPathwayGraph
    '                   ExportPathwayGraphFile, Generate, PfsNETNetwork, PfsNETNetwork_assemble_keggpathways
    '         Class Enzyme
    ' 
    '             Properties: EC
    ' 
    '         Class Interaction
    ' 
    '             Properties: Modules, Pathways, Reactions
    ' 
    '         Class PfsNETNode
    ' 
    '             Properties: Important
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.TabularArchives
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml.Nodes
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports ______NETWORK__ = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic.Network(Of
    Microsoft.VisualBasic.Data.visualize.Network.FileStream.Node,
    Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkEdge)
Imports __KEGG_NETWORK_ = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic.Network(Of
    Global.SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.Enzyme,
    SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.Interaction)

Namespace NetworkModel.PfsNET

    ''' <summary>
    ''' 绘制KEGG数据为主的基因互作网络
    ''' </summary>
    ''' <remarks>
    ''' 1. 对于有代谢反应的网络，表现为酶分子与酶分子之间所组成的网络
    ''' </remarks>
    ''' 
    <Package("KEGG_Pathway.Network",
                      Description:="Network data visualization for the pfsNET result",
                      Category:=APICategories.ResearchTools,
                      Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module GeneInteractions

        Private Function __getRxnRels(rxnMaps As ReactionMaps(), locus As String) As NetworkEdge()
            Dim rels As NetworkEdge() = (From rId As ReactionMaps In rxnMaps
                                         Let reactions As NetworkEdge() = rId.Reactions.Select(
                                             Function(sId) New NetworkEdge With {
                                             .fromNode = locus,
                                             .toNode = sId,
                                             .interaction = "EnzymeRelated"
                                         }).ToArray
                                         Select reactions).ToVector
            Return rels
        End Function

        ''' <summary>
        ''' 对外的接口
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Network.Creates")>
        Public Function CreateNetworkObject(<Parameter("From.Pathway")> fromPathway As XmlModel) As ______NETWORK__
            Dim reactions As Dictionary(Of String, bGetObject.Reaction) = fromPathway.Metabolome.ToDictionary(Function(r) r.ID)
            Dim rxnGeneRels = (From ezMap As EC_Mapping In fromPathway.EC_Mappings
                               Let rels As NetworkEdge() = __getRxnRels(ezMap.ECMaps, ezMap.locusId)
                               Select rels).ToVector
            Dim nodes As FileStream.Node() = (From gene As NetworkEdge
                                              In rxnGeneRels
                                              Let gNode = New FileStream.Node With {
                                                  .ID = gene.fromNode,
                                                  .NodeType = "Gene"
                                              }
                                              Select gNode).ToArray
            Dim rxnNodes = (From gene As NetworkEdge
                            In rxnGeneRels
                            Let rNode = New FileStream.Node With {
                                .ID = gene.toNode,
                                .NodeType = "MetabolismFlux"
                            }
                            Select rNode).ToArray
            Call nodes.Add(rxnNodes)

            Dim rxnRels = (From rxn As bGetObject.Reaction In fromPathway.Metabolome
                           Let nextRxn As bGetObject.Reaction() = (From r As bGetObject.Reaction
                                                                   In fromPathway.Metabolome
                                                                   Where rxn.IsConnectWith(r)
                                                                   Select r).ToArray
                           Let flux As NetworkEdge() = (From r As bGetObject.Reaction
                                                        In nextRxn
                                                        Select New NetworkEdge With {
                                                            .fromNode = rxn.ID,
                                                            .toNode = r.ID,
                                                            .interaction = "Flux"}).ToArray
                           Select flux).ToVector
            Return New ______NETWORK__ With {
                .nodes = nodes,
                .edges = rxnGeneRels.Join(rxnRels).ToArray
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="FromPathway"></param>
        ''' <param name="saveDIR"></param>
        ''' <param name="Trim"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Export.Network.KEGG_Modules",
                   Info:="Function create the network file for each module seperatly. Trim will remove the dulplicated edges and self loop nodes")>
        Public Function ExportPathwayGraph(<Parameter("From.Pathway")> FromPathway As XmlModel, saveDIR As String, Optional Trim As Boolean = True) As Boolean
            For Each [mod] As KeyValuePair(Of String, ______NETWORK__) In ExportPathwayGraph(FromPathway, Trim)
                Dim DIR As String = $"{saveDIR}/{[mod].Key}/"
                Call [mod].Value.Save(DIR, Encodings.UTF8.CodePage)
                Call CytoscapeGraphView.Serialization.Export([mod].Value).Save($"{DIR}/Cytoscape.xml")
            Next

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="saveDIR"></param>
        ''' <param name="Trim"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Export.Network.KEGG_Modules",
                   Info:="Function create the network file for each module seperatly. Trim will remove the dulplicated edges and self loop nodes")>
        Public Function ExportPathwayGraph(XmlModel As String, saveDIR As String, Optional Trim As Boolean = True) As Boolean
            Dim Model = XmlModel.LoadXml(Of XmlModel)
            Return ExportPathwayGraph(Model, saveDIR, Trim)
        End Function

        ''' <summary>
        ''' 这个方法所导出的网络模型可以用于PfsNEt的文件3 
        ''' </summary>
        ''' <param name="FromPathway"></param>
        ''' <param name="Trim"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Export.KEGG.CytoscapeModel")>
        Public Function __exportPathwayGraph(<Parameter("From.Pathway")> FromPathway As XmlModel, Trim As Boolean) As __KEGG_NETWORK_
            Dim ChunkBuffer = ExportPathwayGraph(FromPathway, Trim)
            Dim Nodes As List(Of Enzyme) = New List(Of Enzyme)
            Dim Edges As List(Of Interaction) = New List(Of Interaction)
            Dim rxnhash = (From x As bGetObject.Reaction
                           In FromPathway.Metabolome
                           Select x
                           Group x By x.ID Into Group) _
                                .ToDictionary(Function(x) x.ID,
                                              Function(x) x.Group.First)

            Dim ECMappings = (From item In FromPathway.EC_Mappings.AsParallel
                              Let reactions = (From id As ReactionMaps In item.ECMaps
                                               Select (From sId As String
                                                       In id.Reactions
                                                       Where rxnhash.ContainsKey(sId)
                                                       Select rxnhash(sId)).ToArray).Unlist.Distinct.ToArray
                              Select item.locusId,
                                  item.ECMaps,
                                  reactions).ToDictionary(Function(item) item.locusId)
            Dim Pathways = (From item In FromPathway.Pathways Let value = item.Pathways Select value).ToArray.ToVector

            For Each Line As KeyValuePair(Of String, ______NETWORK__) In ChunkBuffer
                Call Nodes.AddRange((From item In Line.Value.nodes Select New Enzyme With {
                                                                       .ID = item.ID,
                                                                       .NodeType = item.NodeType,
                                                                       .EC = If(ECMappings.ContainsKey(item.ID), ECMappings(item.ID).ECMaps.Select(Function(x) x.EC), Nothing)}))
                Call Edges.AddRange((From item In Line.Value.edges Select New Interaction With {
                                                                       .fromNode = item.fromNode,
                                                                       .toNode = item.toNode,
                                                                       .interaction = item.interaction,
                                                                       .Modules = {Line.Key}}).ToArray)
            Next

            '重新整理节点和互作
            Nodes = (From item In (From item In Nodes Select item Group item By item.ID Into Group).ToArray.AsParallel
                     Let node_collection = item.Group.ToArray
                     Let ec = (From node In node_collection Select node.EC).ToArray.ToVector.Distinct.ToArray
                     Let type = (From node In node_collection Select node.NodeType Distinct).ToArray
                     Let nodeEnzyme = New Enzyme With {.ID = item.ID, .EC = ec, .NodeType = String.Join("; ", type)}
                     Select nodeEnzyme
                     Order By nodeEnzyme.ID Ascending).AsList
            Edges = (From item In (From item In Edges Select Guid = item.GetNullDirectedGuid, item Group By Guid Into Group).AsParallel
                     Let edge_collection = item.Group.ToArray
                     Let instance = edge_collection.First
                     Let modules = (From node In edge_collection Select node.item.Modules).ToVector.Distinct.ToArray
                     Let pathwaycollection = (From pathway In Pathways Where Not (From mid As String In modules Where pathway.IsContainsModule(mid) Select 1).ToArray.IsNullOrEmpty Select pathway.EntryId).ToArray
                     Let coeffectReactions = {(From nn In ECMappings(instance.item.fromNode).reactions Select nn.ID).ToArray, (From nn In ECMappings(instance.item.toNode).reactions Select nn.ID).ToArray}.Intersection
                     Select New Interaction With {
                         .fromNode = instance.item.fromNode,
                         .toNode = instance.item.toNode,
                         .Pathways = pathwaycollection,
                         .Modules = modules,
                         .interaction = instance.item.interaction,
                         .Reactions = coeffectReactions}).AsList

            Dim Network As __KEGG_NETWORK_ = New __KEGG_NETWORK_ With {
                .edges = Edges.ToArray,
                .nodes = Nodes.ToArray
            }
            Return Network
        End Function

        ''' <summary>
        ''' 创建KEGG数据库所编译的cytoscape网络，并导出为文件
        ''' </summary>
        ''' <param name="FromPathway"></param>
        ''' <param name="saveDIR"></param>
        ''' <param name="Trim"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Export.KEGG_Module.NetworkFile",
                   Info:="Function create the network file for each module seperatly. Trim will remove the dulplicated edges and self loop nodes")>
        Public Function ExportPathwayGraphFile(<Parameter("From.Pathway")> FromPathway As XmlModel,
                                               saveDIR As String,
                                               Optional Trim As Boolean = True) As Boolean
            Dim Network = __exportPathwayGraph(FromPathway, Trim)
            Return Network.Save(saveDIR, Encodings.UTF8.CodePage)
        End Function

        Public Class Enzyme : Inherits FileStream.Node
            Public Property EC As String()
        End Class

        Public Class Interaction : Inherits NetworkEdge
            Public Property Pathways As String()
            ''' <summary>
            ''' 仅对于两个基因共同催化一个过程的时候有用途
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Reactions As String()
            Public Property Modules As String()
        End Class

        ''' <summary>
        ''' 函数导出kegg——module的信息，组成pathway还需要进行拼接
        ''' </summary>
        ''' <param name="FromPathway"></param>
        ''' <param name="Trim"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Export.Graph.Pathway")>
        Public Function ExportPathwayGraph(<Parameter("From.Pathway")> FromPathway As XmlModel, Optional Trim As Boolean = True) As Dictionary(Of String, ______NETWORK__)
            Dim reactions = FromPathway.Metabolome.ToDictionary(Function(r) r.ID)
            Dim rxnGeneRels As NetworkEdge() = (From ezMap As EC_Mapping
                                                In FromPathway.EC_Mappings.AsParallel
                                                Select (From r As ReactionMaps
                                                        In ezMap.ECMaps
                                                        Select r.Reactions.Select(Function(x) New NetworkEdge With {
                                                            .fromNode = ezMap.locusId,
                                                            .toNode = x,
                                                            .interaction = "EnzymeRelated"}))).Unlist.ToVector
            Dim rxnRels As NetworkEdge() = (From rxn As bGetObject.Reaction
                                            In FromPathway.Metabolome.AsParallel
                                            Let NextRxn = (From r In FromPathway.Metabolome Where rxn.IsConnectWith(r) Select r).ToArray
                                            Select (From r As bGetObject.Reaction
                                                    In NextRxn
                                                    Select New NetworkEdge With {
                                                        .fromNode = rxn.ID,
                                                        .toNode = r.ID,
                                                        .interaction = "Flux"}).ToArray).ToArray.ToVector

            Dim List As Dictionary(Of String, ______NETWORK__) =
                (From kNod As bGetObject.Module In FromPathway.Modules.AsParallel
                 Let net As KeyValuePair(Of String, ______NETWORK__) = __kModNetwork(kNod, rxnGeneRels, rxnRels, Trim)
                 Where Not net.Value Is Nothing
                 Select net).ToDictionary(Function(x) x.Key,
                                          Function(x) x.Value)
            Return List
        End Function

        Private Function __kModNetwork([mod] As bGetObject.Module, rxnGeneRels As NetworkEdge(), rxnRels As NetworkEdge(), trim As Boolean) As KeyValuePair(Of String, ______NETWORK__)
            If [mod].pathwayGenes.IsNullOrEmpty OrElse [mod].reaction.IsNullOrEmpty Then
                Call $"""{[mod].EntryId}"" is not a network!".__DEBUG_ECHO
                Return Nothing
            End If

            Dim ReactionList = (From item In [mod].reaction Select item.name).ToArray
            Dim GeneIdList = [mod].GetPathwayGenes

            Dim Nodes = (From id As String
                         In GeneIdList
                         Select New FileStream.Node With {
                             .ID = id,
                             .NodeType = "Gene"}).ToArray '创建酶分子的节点

            Dim bufSource = (From item In rxnRels
                             Where Array.IndexOf(ReactionList, item.fromNode) > -1 OrElse Array.IndexOf(ReactionList, item.toNode) > -1
                             Let [from] = (From n As NetworkEdge
                                           In rxnGeneRels
                                           Where Array.IndexOf(GeneIdList, n.fromNode) > -1 AndAlso String.Equals(item.fromNode, n.toNode)
                                           Select n.fromNode
                                           Distinct
                                           Order By fromNode Ascending).ToArray
                             Let [to] = (From n As NetworkEdge
                                         In rxnGeneRels
                                         Where Array.IndexOf(GeneIdList, n.fromNode) > -1 AndAlso String.Equals(item.toNode, n.toNode)
                                         Select n.fromNode
                                         Distinct
                                         Order By fromNode Ascending).ToArray
                             Select [from], [to]).ToArray
            Dim Edges = (From item In bufSource
                         Where Not (item.from.IsNullOrEmpty OrElse item.to.IsNullOrEmpty)
                         Select Generate(item.from, item.to)).ToArray.ToVector
            Dim net As ______NETWORK__ = New ______NETWORK__ With {
                .nodes = Nodes,
                .edges = Edges
            }
            If trim Then
                Call net.RemoveDuplicated()
                Call net.RemoveSelfLoop()
            End If

            Return New KeyValuePair(Of String, ______NETWORK__)([mod].EntryId, net)
        End Function

        Private Function Generate(from As String(), [to] As String()) As NetworkEdge()
            Dim ListDDD As List(Of NetworkEdge) = New List(Of NetworkEdge)

            If from.Length > 1 Then
                Call ListDDD.AddRange((From id As String In from
                                       Select (From id2 As String In from
                                               Select New NetworkEdge With {
                                                   .fromNode = id,
                                                   .toNode = id2,
                                                   .interaction = "Coeffect"}).ToArray).ToArray.ToVector)
            End If
            If [to].Length > 1 Then
                Call ListDDD.AddRange((From id As String In [to]
                                       Select (From id2 As String In [to]
                                               Select New NetworkEdge With {
                                                   .fromNode = id,
                                                   .toNode = id2,
                                                   .interaction = "Coeffect"}).ToArray).ToArray.ToVector)
            End If

            Call ListDDD.AddRange((From id As String
                                   In from
                                   Select (From id2 As String
                                           In [to]
                                           Select New NetworkEdge With {
                                               .fromNode = id,
                                               .toNode = id2,
                                               .interaction = "Flux"}).ToArray).ToVector)

            Return ListDDD.ToArray
        End Function


        ''' <summary>
        ''' 从模型之中导出网络数据并按照模块编号分别保存到文件系统之中
        ''' </summary>
        ''' <param name="FromPathway"></param>
        ''' <param name="PfsNet"></param>
        ''' <param name="ExportDIR"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Export.PfsNet.Cytoscape.KEGG_Modules",
                   Info:="visualize kegg module pfsnet calculation data, this method only export the kegg_module information.")>
        Public Function PfsNETNetwork(<Parameter("From.Pathway")>
                                      FromPathway As XmlModel,
                                      PfsNet As IEnumerable(Of SubNetTable),
                                      ExportDIR As String) As Boolean

            Dim net = ExportPathwayGraph(FromPathway, True).ToDictionary(Function(t) t.Key)

            For Each item In PfsNet
                Dim [mod] = net(item.UniqueId)
                Dim nodes = (From n In item.SignificantGeneObjects Select New PfsNETNode With {
                                                                       .ID = n,
                                                                       .NodeType = "Gene",
                                                                       .Important = {"True"}
                                                                       }).ToArray

                If nodes.IsNullOrEmpty Then
                    Continue For
                End If

                Call [mod].Value.Save(String.Format("{0}/{1}/", ExportDIR, [mod].Key), Encodings.UTF8.CodePage)
                Call nodes.SaveTo(String.Format("{0}/{1}/Nodes__{2}.csv", ExportDIR, [mod].Key, item.PhenotypePair))
            Next

            Return True
        End Function

        ''' <summary>
        ''' 按照代谢途径来可视化pfsnet的计算数据
        ''' </summary>
        ''' <param name="FromPathway"></param>
        ''' <param name="PfsNet"></param>
        ''' <param name="ExportDIR"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Export.PfsNet.Cytoscape")>
        Public Function PfsNETNetwork_assemble_keggpathways(<Parameter("From.Pathway")> FromPathway As XmlModel,
                                                            PfsNet As IEnumerable(Of SubNetTable),
                                                            ExportDIR As String) As Boolean

            Dim net = ExportPathwayGraph(FromPathway, True).ToDictionary(Function(t) t.Key)

            For Each pathway In FromPathway.Pathways
                For Each obj In pathway.Pathways

                    If obj.modules.IsNullOrEmpty Then
                        Call $"pathway ""{obj.EntryId}"" is not a valid visualization source!".__DEBUG_ECHO
                        Continue For
                    End If

                    Dim modIdlist = (From item In obj.modules Select item.name).ToArray
                    Dim modules = (From item In net Where Array.IndexOf(modIdlist, item.Key) > -1 Select item).ToArray
                    Dim network As New ______NETWORK__ With {
                        .edges = (From item In modules Select item.Value.Value.edges).ToVector,
                        .nodes = (From item In modules Select item.Value.Value.nodes).ToVector
                    }

                    Call network.RemoveDuplicated()
                    Call network.RemoveSelfLoop()

                    Dim pfsnetList = (From item In PfsNet Where String.Equals(item.UniqueId, obj.EntryId) Select item).ToArray

                    If pfsnetList.IsNullOrEmpty Then
                        Continue For
                    End If

                    Call network.Save(String.Format("{0}/{1}/", ExportDIR, obj.EntryId), Encodings.UTF8.CodePage)

                    Dim NodeTable = (From item In network.nodes Select item.CopyTo(Of PfsNETNode)()).ToArray

                    For Each Node In NodeTable
                        Node.Important = (From item As SubNetTable
                                          In pfsnetList.AsParallel
                                          Where Array.IndexOf(item.SignificantGeneObjects, Node.ID) > -1
                                          Select item.PhenotypePair
                                          Distinct
                                          Order By PhenotypePair Ascending).ToArray
                    Next

                    Call NodeTable.SaveTo(String.Format("{0}/{1}/Nodes.pfsnet.csv", ExportDIR, obj.EntryId), False)
                Next
            Next

            Return True
        End Function

        Public Class PfsNETNode : Inherits FileStream.Node

            ''' <summary>
            ''' 当前的<see cref="PfsNETNode.ID">基因节点</see>受这个列表之中的调控因子的影响比较大
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Important As String()
        End Class
    End Module
End Namespace
