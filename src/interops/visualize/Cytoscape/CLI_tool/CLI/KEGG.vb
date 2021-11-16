#Region "Microsoft.VisualBasic::3bb6a36d4b7c90c506047480d0306f37, visualize\Cytoscape\CLI_tool\CLI\KEGG.vb"

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

    ' Module CLI
    ' 
    '     Function: __classNetwork, __mergeCommon, __pathwayNetwork, __typeNetwork, BuildKOLinks
    '               KEGGPathwayMapNetwork, KEGGReferenceMapModel, ModsNET, ModuleRegulations, NodeInformationTable
    '               ReactionNET, RenamesKEGGNode, RenderReferenceMapNetwork, WriteKEGGCompoundsSummary, WriteReactionTable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis.Model
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml.Nodes
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Model.Network.KEGG
Imports SMRUCC.genomics.Model.Network.KEGG.PathwayMaps
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.KEGG
Imports SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.KEGG.ReactionNET
Imports SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET
Imports xCytoscape.GCModeller.FileSystem
Imports xCytoscape.GCModeller.FileSystem.KEGG.Directories
Imports ______NETWORK__ = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic.Network(Of
    Microsoft.VisualBasic.Data.visualize.Network.FileStream.Node,
    Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkEdge)

Partial Module CLI

    <ExportAPI("--mod.regulations",
               Usage:="--mod.regulations /model <KEGG.xml> /footprints <footprints.csv> /out <outDIR> [/pathway /class /type]")>
    <ArgumentAttribute("/class", True, Description:="This parameter can not be co-exists with ``/type`` parameter")>
    <ArgumentAttribute("/type", True, Description:="This parameter can not be co-exists with ``/class`` parameter")>
    <Group(CLIGrouping.KEGGTools)>
    Public Function ModuleRegulations(args As CommandLine) As Integer
        Dim Model = args("/model").LoadXml(Of XmlModel)
        Dim Footprints = (From x
                          In (args <= "/footprints").LoadCsv(Of PredictedRegulationFootprint)
                          Where Not String.IsNullOrEmpty(x.Regulator)
                          Select x).ToArray

        Dim Networks = GeneInteractions.ExportPathwayGraph(Model)
        Dim regulators = Footprints.Select(Function(x) x.Regulator).Distinct.Select(
            Function(x) New FileStream.Node With {
                .ID = x,
                .NodeType = "TF"
            }).ToArray
        Dim regulations = (From x In Footprints
                           Let regulation = New FileStream.NetworkEdge With {
                               .value = x.Pcc,
                               .fromNode = x.Regulator,
                               .toNode = x.ORF,
                               .interaction = "Regulation"
                           }
                           Select regulation
                           Group regulation By regulation.toNode Into Group) _
                               .ToDictionary(Function(x) x.toNode,
                                             Function(x) x.Group.ToArray)
        Dim outDIR As String = FileIO.FileSystem.GetDirectoryInfo(args("/out")).FullName

        If args.GetBoolean("/pathway") Then
            Networks = __pathwayNetwork(Model, Networks)
        End If

        If args.GetBoolean("/class") Then
            Networks = __classNetwork(Model, Networks)
        ElseIf args.GetBoolean("/type") Then
            Networks = __typeNetwork(Model, Networks)
        End If

        For Each kMod In Networks
            Dim edges = kMod.Value _
                .nodes _
                .Select(Function(x) regulations.TryGetValue(x.ID)) _
                .Unlist
            Dim Path As String = $"{outDIR}/{kMod.Key}/"

            If edges.IsNullOrEmpty Then
                Continue For
            End If

            Call kMod.Value.nodes.Add(regulators)
            Call kMod.Value.edges.Add(edges)
            Call kMod.Value.Save(Path, Encoding.UTF8)
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 基因表达调控网络细胞表型大分类
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="networks"></param>
    ''' <returns></returns>
    Private Function __typeNetwork(model As XmlModel, networks As Dictionary(Of String, ______NETWORK__)) As Dictionary(Of String, ______NETWORK__)
        Call $"Merge {networks.Count} network by type....".__DEBUG_ECHO

        Dim classes = (From x As PwyBriteFunc
                       In model.Pathways
                       Select x
                       Group x By x.Class Into Group) _
                            .ToDictionary(Function(x) x.Class,
                                          Function(x) x.Group.Select(
                                          Function(xx) xx.Pathways.Select(
                                          Function(xxx) networks.TryGetValue(xxx.EntryId))).Unlist)
        Dim dict As Dictionary(Of String, ______NETWORK__) = classes.ToDictionary(Function(x) x.Key,
                                                                                  Function(x) __mergeCommon(x.Value))
        Return dict
    End Function

    ''' <summary>
    ''' 基因表达调控网络按照细胞表型小分类聚合
    ''' </summary>
    ''' <param name="model">KEGG细胞表型分类</param>
    ''' <param name="networks"></param>
    ''' <returns></returns>
    Private Function __classNetwork(model As XmlModel, networks As Dictionary(Of String, ______NETWORK__)) As Dictionary(Of String, ______NETWORK__)
        Call $"Merge {networks.Count} network by class category....".__DEBUG_ECHO

        Dim classes = (From x As PwyBriteFunc
                       In model.Pathways
                       Select x
                       Group x By x.Category Into Group) _
                            .ToDictionary(Function(x) x.Category, elementSelector:=
                                          Function(x) x.Group.Select(
                                          Function(xx) xx.Pathways.Select(
                                          Function(xxx) networks.TryGetValue(xxx.EntryId))).Unlist)
        Dim dict = classes.ToDictionary(Function(x) x.Key,
                                        Function(x) __mergeCommon(x.Value))
        Return dict
    End Function

    Private Function __mergeCommon(source As IEnumerable(Of ______NETWORK__)) As ______NETWORK__
        Dim Nods = source.Where(Function(x) Not x Is Nothing).Select(Function(x) x.nodes).Unlist
        Dim Edges As List(Of FileStream.NetworkEdge) =
            source.Where(Function(x) Not x Is Nothing).Select(Function(x) x.edges).Unlist

        Dim __nodes = LinqAPI.Exec(Of FileStream.Node) <=
            From node
            In (From node As FileStream.Node
                In Nods
                Select node
                Group node By node.ID Into Group)
            Select New FileStream.Node With {
                .ID = node.ID,
                .NodeType = node.Group _
                    .Select(Function(x) x.NodeType) _
                    .Distinct _
                    .ToArray _
                    .JoinBy("; ")
            }
        Dim __edges = (From edge As FileStream.NetworkEdge
                       In Edges
                       Select edge,
                           id = edge.GetDirectedGuid
                       Group By id Into Group).Select(Function(x) x.Group.First.edge)
        Dim net As ______NETWORK__ = New ______NETWORK__ With {
            .edges = __edges,
            .nodes = __nodes
        }
        Return net
    End Function

    ''' <summary>
    ''' 将Module视图转换为Pathway视图
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="networks"></param>
    ''' <returns></returns>
    Private Function __pathwayNetwork(model As XmlModel, networks As Dictionary(Of String, ______NETWORK__)) As Dictionary(Of String, ______NETWORK__)
        Dim dict As New Dictionary(Of String, ______NETWORK__)

        For Each ph As Pathway In model.GetAllPathways
            If ph.modules.IsNullOrEmpty Then
                Continue For
            End If

            Dim LQuery = (From m In ph.modules
                          Let km = networks.TryGetValue(m.name)
                          Where Not km Is Nothing
                          Select km).ToArray
            Dim net = __mergeCommon(LQuery)

            Call dict.Add(ph.EntryId, net)
        Next

        Return dict
    End Function

    <ExportAPI("/reaction.NET", Usage:="/reaction.NET [/model <xmlModel.xml> /source <rxn.DIR> /out <outDIR>]")>
    <Group(CLIGrouping.KEGGTools)>
    Public Function ReactionNET(args As CommandLine) As Integer
        Dim source As String = TryGetSource(args("/source"), AddressOf GetReactions)
        Dim model As String = args("/model")
        Dim out As String
        If Not String.IsNullOrEmpty(model) Then
            out = model.TrimSuffix & ".ReactionNET/"
            Dim bMods As XmlModel = model.LoadXml(Of XmlModel)
            Dim net As FileStream.NetworkTables = ModelNET(bMods, source)
            Return net.Save(out, Encodings.ASCII.CodePage).CLICode
        Else
            out = args.GetValue("/out", source & ".ReactionNET/")
            Dim net As FileStream.NetworkTables = BuildNET(source)
            Return net.Save(out, Encodings.ASCII.CodePage).CLICode
        End If
    End Function

    ''' <summary>
    ''' 基因和模块之间的从属关系，附加调控信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KEGG.Mods.NET",
               Usage:="/KEGG.Mods.NET /in <mods.xml.DIR> [/out <outDIR> /pathway /footprints <footprints.Csv> /brief /cut 0 /pcc 0]")>
    <ArgumentAttribute("/brief", True,
                   Description:="If this parameter is represented, then the program just outs the modules, all of the non-pathway genes wil be removes.")>
    <Group(CLIGrouping.KEGGTools)>
    Public Function ModsNET(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim isPathway As Boolean = args.GetBoolean("/pathway")
        Dim net = If(isPathway,
            LoadPathways(inDIR).BuildNET,
            LoadModules(inDIR).BuildNET)
        Dim out As String = args.GetValue("/out", inDIR & ".modsNET/")
        Dim footprint As String = args("/footprints")
        Dim cut As Double = args.GetValue("/cut", 0.0R)
        Dim nulls As FileStream.NetworkTables = Nothing

        If footprint.FileExists Then
            Dim brief As Boolean = args.GetBoolean("/brief")
            Dim footprints As IEnumerable(Of RegulatesFootprints) =
                footprint.LoadCsv(Of RegulatesFootprints)

            Dim pcc As Double = args.GetValue("/pcc", 0R)

            If pcc <> 0R Then
                footprints = (From x In footprints Where Math.Abs(x.Pcc) >= pcc Select x).ToArray
            End If

            Call net.AddFootprints(footprints, brief)
            If brief Then
                Dim LQuery = (From x As FileStream.NetworkEdge
                              In net.edges
                              Where String.Equals(x.interaction, PathwayGene)
                              Select x
                              Group x By x.fromNode Into Group)  ' 代谢途径基因按照模块分组
                Dim rhaves As String() = footprints.Select(Function(x) x.ORF).Distinct.ToArray
                Dim Trim = (From m In LQuery
                            Where (From x As FileStream.NetworkEdge In m.Group
                                   Where Array.IndexOf(rhaves, x.toNode) > -1
                                   Select x).FirstOrDefault Is Nothing
                            Select m).ToArray
                nulls = New FileStream.NetworkTables + Trim.Select(Function(x) x.Group).IteratesALL ' 添加新的网络节点
                net -= nulls.edges  ' 删除旧的网络节点
                nulls += net <= nulls.edges.Select(Function(x) {x.fromNode, x.toNode}).IteratesALL
                net -= nulls.nodes
            End If
        End If

        If cut <> 0R Then  ' 按照阈值筛选
            net.edges =
                LinqAPI.Exec(Of NetworkEdge) <= From x As NetworkEdge
                                                In net.edges
                                                Where Math.Abs(x.value) >= cut
                                                Select x
            out = out & "." & cut
        End If

        If Not nulls Is Nothing Then
            Call nulls.Save(out & "/no-regs/", Encoding.ASCII)
        End If
        Return net.Save(out, Encoding.ASCII).CLICode
    End Function

    <ExportAPI("/KEGG.pathwayMap.Network")>
    <Usage("/KEGG.pathwayMap.Network /in <br08901.DIR> [/node <nodes.data.csv> /out <out.DIR>]")>
    <Group(CLIGrouping.KEGGTools)>
    Public Function KEGGPathwayMapNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim node$ = args <= "/node"
        Dim out$ = args.GetValue("/out", [in].TrimDIR & ".network/")
        Dim graph As NetworkTables = PathwayMapNetwork.BuildModel([in])

        If node.FileExists(True) Then
            Dim data = EntityObject.LoadDataSet(node)
            Dim nodes As New Dictionary(Of FileStream.Node)(graph.nodes)

            For Each n As EntityObject In data
                If nodes.ContainsKey(n.ID) Then
                    With nodes(n.ID).Properties
                        For Each p In n.Properties
                            Call .Add(p.Key, p.Value)
                        Next
                    End With
                End If
            Next
        End If

        Return graph.Save(out, Encodings.ASCII.CodePage).CLICode
    End Function

    <ExportAPI("/KEGG.referenceMap.Model")>
    <Usage("/KEGG.referenceMap.Model /repository <[reference/organism]kegg_maps.directory> /reactions <kegg_reactions.directory> [/top.priority <map.name.list> /category.level2 /reaction_class <repository> /organism <name> /coverage.cutoff <[0,1], default=0> /delete.unmapped /delete.tupleEdges /split /ignores <compoind idlist> /out <result_network.directory>]")>
    <Description("Create network model of KEGG reference pathway map for cytoscape data visualization.")>
    <ArgumentAttribute("/repository", False, CLITypes.File,
              AcceptTypes:={GetType(Map), GetType(Pathway)},
              Extensions:="*.Xml",
              Description:="This parameter accept two kind of parameters: The kegg reference map data or organism specific pathway map model data.")>
    <ArgumentAttribute("/top.priority", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="The map names in the argument value will be forced populate in top priority and ignores of their map coverage value is top or not. 
              Use comma symbol as the map id terms' delimiter.")>
    <ArgumentAttribute("/reactions", False, CLITypes.File,
              AcceptTypes:={GetType(Reaction)},
              Extensions:="*.Xml",
              Description:="The KEGG reference reaction data models.")>
    <ArgumentAttribute("/organism", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="The organism name or code, if this argument presents in the cli command input, then it means 
              the ``/repository`` parameter data model is the organism specific pathway map data.")>
    <ArgumentAttribute("/out", True, CLITypes.File,
              AcceptTypes:={GetType(NetworkTables)},
              Extensions:="*.csv",
              Description:="The network file data output directory that used for cytoscape network visualization.")>
    <ArgumentAttribute("/reaction_class", True, CLITypes.File,
              AcceptTypes:={GetType(ReactionClass)},
              Extensions:="*.Xml",
              Description:="Apply reaction class filter for reduce network size.")>
    <ArgumentAttribute("/coverage.cutoff", True, CLITypes.Double,
              AcceptTypes:={GetType(Double)},
              Description:="The coverage cutoff of the pathway map, cutoff value in range [0,1]. Default value is zero means no cutoff.")>
    <ArgumentAttribute("/ignores", True, CLITypes.File,
              AcceptTypes:={GetType(String())},
              Description:="A list of kegg compound id list that will be ignores in the generated pathway map model, this optional
              value could be a id list which use the comma symbol as delimiter or an id list file with format of one id per line.")>
    <Group(CLIGrouping.KEGGPathwayMapTools)>
    Public Function KEGGReferenceMapModel(args As CommandLine) As Integer
        Dim in$ = args <= "/repository"
        Dim organismName$ = args("/organism")
        Dim out$
        Dim reactions = ReactionTable.Load(args <= "/reactions")
        Dim model As NetworkTables
        Dim reactionClass As ReactionClassifier = ReactionClassifier.FromRepository(args <= "/reaction_class")
        Dim doRemoveUnmapped As Boolean = args("/delete.unmapped")
        Dim coverageCutoff As Double = args("/coverage.cutoff") Or 0.0
        Dim splitNetwork As Boolean = args("/split")
        Dim deleteTupleEdges As Boolean = args("/delete.tupleEdges")
        Dim categoryLevel2 As Boolean = args("/category.level2")
        Dim topMaps As String() = args("/top.priority").Split(",")
        Dim ignores As String() = Nothing

        If args.ContainsParameter("/ignores") Then
            With args <= "/ignores"
                If .FileExists Then
                    ignores = .ReadAllLines
                Else
                    ignores = .Trim.Split(","c).Select(AddressOf Strings.Trim).ToArray
                End If
            End With
        End If

        If ReactionClassifier.IsNullOrEmpty(reactionClass) Then
            reactionClass = Nothing
        Else
            Call $"Try to reduce network size with {reactionClass.Count} reaction class data.".__INFO_ECHO
        End If

        If organismName.StringEmpty Then
            Dim maps As Map()

            If [in].FileExists Then
                out = args("/out") Or $"{[in].TrimSuffix}/"
                maps = {[in].LoadXml(Of Map)}
            Else
                out = args("/out") Or $"{[in].TrimDIR}.referenceMap/"
                maps = MapRepository _
                    .ScanMaps(directory:=[in]) _
                    .ToArray
            End If

            model = PathwayMaps.BuildNetworkModel(
                maps:=maps,
                reactions:=reactions,
                classFilter:=False,
                reactionClass:=reactionClass,
                doRemoveUnmmaped:=doRemoveUnmapped,
                coverageCutoff:=coverageCutoff,
                categoryLevel2:=categoryLevel2,
                topMaps:=topMaps,
                ignores:=ignores
            )
        Else
            out = args("/out") Or $"{[in].TrimDIR}.{organismName}.referenceMap/"
            model = PathwayMaps.BuildNetworkModel(
                maps:=OrganismModel.EnumerateModules(handle:=[in]),
                reactions:=reactions,
                reactionClass:=reactionClass,
                doRemoveUnmmaped:=doRemoveUnmapped,
                coverageCutoff:=coverageCutoff,
                categoryLevel2:=categoryLevel2,
                topMaps:=topMaps
            )
        End If

        Dim groupSelects = model.nodes.GroupBy(Function(n) n("group.category")).ToArray

        For Each group In groupSelects
            Call group.Keys.FlushAllLines($"{out}/selects/{group.Key.NormalizePathString}.txt")
        Next

        If splitNetwork Then
            Dim bridgeEdges As New List(Of NetworkEdge)
            Dim topMapName = topMaps.DefaultFirst("")

            For Each group In groupSelects
                Dim nodeIndex = group.Select(Function(n) n.ID).Indexing
                Dim edges = model.edges _
                    .Where(Function(e)
                               ' The first top map will not be trimmed!
                               If topMapName = group.Key Then
                                   Return True
                               Else
                                   Return e.fromNode Like nodeIndex AndAlso e.toNode Like nodeIndex
                               End If
                           End Function) _
                    .ToArray
                Dim subNetwork As New NetworkTables(group, edges)

                ' The first top map will not be trimmed!
                If deleteTupleEdges AndAlso topMapName <> group.Key Then
                    Dim index = New GraphIndex(Of FileStream.Node, NetworkEdge)().nodes(subNetwork.nodes).edges(subNetwork.edges)
                    Dim nonTuples = subNetwork.edges.Where(Function(e) Not e.isTupleEdge(index)).ToArray

                    subNetwork.edges = nonTuples
                End If

                Call subNetwork.Save($"{out}/subset/{group.Key.NormalizePathString}/")
                Call model.edges _
                    .Where(Function(e)
                               Return (e.fromNode Like nodeIndex AndAlso Not e.toNode Like nodeIndex) OrElse (Not e.fromNode Like nodeIndex AndAlso e.toNode Like nodeIndex)
                           End Function) _
                    .DoCall(AddressOf bridgeEdges.AddRange)
            Next

            Dim bridgeNodex As Index(Of String) = bridgeEdges _
                .Select(Function(e) {e.fromNode, e.toNode}) _
                .IteratesALL _
                .Distinct _
                .Indexing

            Dim bridgeNetwork As New NetworkTables(bridgeEdges, model.nodes.Where(Function(n) n.ID Like bridgeNodex))

            Call bridgeNetwork.RemoveDuplicated()
            Call bridgeNetwork.Save($"{out}/subset/bridge/")
        End If

        Return model.Save(out).CLICode
    End Function

    <ExportAPI("/renames.kegg.node")>
    <Description("Update the KEGG compound id and KEGG reaction id as the metabolite common name and enzyme gene name.")>
    <Usage("/renames.kegg.node /network <tables.csv.directory> /compounds <names.json> /KO <reactionKOMapping.json> [/out <output_renames.directory>]")>
    Public Function RenamesKEGGNode(args As CommandLine) As Integer
        Dim in$ = args <= "/network"
        Dim compounds = args("/compounds").LoadJson(Of Dictionary(Of String, String))
        Dim KOnames = args("/KO").LoadJson(Of Dictionary(Of String, String()))
        Dim network As NetworkTables = NetworkFileIO.Load([in])
        Dim out$ = args("/out") Or [in]
        Dim nodeIndex As Dictionary(Of String, FileStream.Node) = network.nodes.ToDictionary(Function(n) n.ID)
        Dim getLabel = ReferenceMapRender.GetNodeLabel(compounds, KOnames)

        For Each node As FileStream.Node In nodeIndex.Values
            node!label_text = getLabel(node)
        Next

        Dim getNodeLabel As Func(Of String, String) =
            Function(nodeId As String) As String
                If nodeIndex.ContainsKey(nodeId) Then
                    Return nodeIndex(nodeId)!label_text
                Else
                    Return nodeId
                End If
            End Function

        For Each edge As NetworkEdge In network.edges
            edge.Add("from.label_text", getNodeLabel(edge.fromNode))
            edge.Add("connTo.label_text", getNodeLabel(edge.toNode))
        Next

        Return network.Save(out).CLICode
    End Function

    <ExportAPI("/KEGG.referenceMap.info")>
    <Usage("/KEGG.referenceMap.info /model <network.xgmml> /compounds <names.json> /KO <reactionKOMapping.json> [/out <table.csv>]")>
    Public Function NodeInformationTable(args As CommandLine) As Integer
        Dim in$ = args <= "/model"
        Dim compounds = (args <= "/compounds") _
            .ReadAllText _
            .LoadJSON(Of Dictionary(Of String, String))
        Dim reactionKOMappingJson = (args <= "/KO") _
            .ReadAllText _
            .LoadJSON(Of Dictionary(Of String, String()))
        Dim model = XGMML.RDFXml.Load([in])
        Dim out As String = args("/out") Or $"{[in].TrimSuffix}.information/"
        Dim info = model.GetIdProperties(reactionKOMappingJson, compounds)

        Call info.nodes.SaveTo($"{out}/nodes.csv")
        Call info.edges.SaveTo($"{out}/edges.csv")

        Return 0
    End Function

    <ExportAPI("/KEGG.referenceMap.render")>
    <Usage("/KEGG.referenceMap.render /model <network.xgmml/directory> [/edge.bends /compounds <names.json> /KO <reactionKOMapping.json> /convexHull <category.txt> /style2 /size <10(A0)> /out <viz.png>]")>
    <Description("Render pathway map as image after cytoscape layout progress.")>
    <Group(CLIGrouping.KEGGPathwayMapTools)>
    <ArgumentAttribute("/compounds", True, CLITypes.File,
              AcceptTypes:={GetType(Dictionary(Of String, String))},
              Extensions:="*.json",
              Description:="The kegg compound id to its command names mapping table file. 
              Content in this table file should be ``Cid -> name``, which could be created 
              by using ``/compound.names`` command from ``kegg_tools``.")>
    Public Function RenderReferenceMapNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/model"
        Dim out$
        Dim size$ = args("/size") Or "10(A0)"
        Dim result As GraphicsData
        Dim convexHull As String() = args("/convexHull").ReadAllLines
        Dim compounds$ = args <= "/compounds"
        Dim edgeBends As Boolean = args("/edge.bends")
        Dim altStyle As Boolean = args("/style2")
        Dim reactionKOMappingJson$ = args("/KO")

        If [in].FileExists AndAlso [in].ExtensionSuffix.TextEquals("xgmml") Then
            out = args("/out") Or ([in].TrimSuffix & $".render.{g.DriverExtensionName}")
            result = ReferenceMapRender.Render(
                model:=XGMML.RDFXml.Load([in]),
                canvasSize:=size,
                convexHull:=convexHull,
                compoundNamesJson:=compounds,
                edgeBends:=edgeBends,
                altStyle:=altStyle,
                reactionKOMappingJson:=reactionKOMappingJson,
                wordWrapWidth:=If(altStyle, -1, 14)
            )
        Else
            Dim table As NetworkTables = NetworkFileIO.Load([in])
            Dim graph As NetworkGraph = table.CreateGraph
            Dim compoundNames = compounds _
                .ReadAllText _
                .LoadJSON(Of Dictionary(Of String, String))

            out = args("/out") Or ([in] & $"/render.{g.DriverExtensionName}")
            result = ReferenceMapRender.Render(
                graph:=graph,
                canvasSize:=size,
                edgeBends:=edgeBends,
                compoundNames:=compoundNames
            )
        End If

        Return result.Save(out).CLICode
    End Function

    <ExportAPI("/Write.Reaction.Table")>
    <Usage("/Write.Reaction.Table /in <br08201.DIR> [/out <out.csv>]")>
    <Group(CLIGrouping.KEGGTools)>
    Public Function WriteReactionTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or ([in].TrimDIR & ".table.csv")
        Dim table As ReactionTable() = ReactionTable.Load(br08201:=[in]).ToArray
        Dim mapcache As MapCache = MapCache.CreateFromTable(table)

        Call mapcache.Text.SaveTo(out.TrimSuffix & ".cacheIndex.txt")

        Return table.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 主要是用于生成网络之中的代谢物节点的信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Write.Compounds.Table")>
    <Usage("/Write.Compounds.Table /in <kegg_compounds.DIR> [/out <out.csv>]")>
    <Group(CLIGrouping.KEGGTools)>
    Public Function WriteKEGGCompoundsSummary(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.table.csv"
        Dim result As New List(Of EntityObject)

        For Each compound As Compound In CompoundRepository.ScanRepository([in], False)
            result += New EntityObject With {
                .ID = compound.entry,
                .Properties = New Dictionary(Of String, String) From {
                    {"name", compound.commonNames.SafeQuery.FirstOrDefault Or compound.entry.AsDefault},
                    {"reaction", compound.reactionId.JoinBy("|")},
                    {"image", compound.Image}
                }
            }
        Next

        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/KO.link")>
    <Usage("/KO.link /in <ko00001.DIR> [/out <out.XML>]")>
    <Group(CLIGrouping.KEGGTools)>
    Public Function BuildKOLinks(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimDIR & ".links.XML")
        Dim data As KOLinks() = KOLinks.Build(ko00001:=[in]).ToArray
        Return data.GetXml.SaveTo(out).CLICode
    End Function
End Module
