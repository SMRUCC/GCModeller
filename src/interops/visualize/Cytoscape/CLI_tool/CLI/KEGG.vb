#Region "Microsoft.VisualBasic::60000ba89862ce898f43ea71446775e8, ..\interops\visualize\Cytoscape\CLI_tool\CLI\KEGG.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml.Nodes
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
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
    <Argument("/class", True, Description:="This parameter can not be co-exists with ``/type`` parameter")>
    <Argument("/type", True, Description:="This parameter can not be co-exists with ``/class`` parameter")>
    <Group(CLIGrouping.KEGGTools)>
    Public Function ModuleRegulations(args As CommandLine) As Integer
        Dim Model = args("/model").LoadXml(Of XmlModel)
        Dim Footprints = (From x
                          In args("/footprints").LoadCsv(Of PredictedRegulationFootprint)
                          Where Not String.IsNullOrEmpty(x.Regulator)
                          Select x).ToArray

        Dim Networks = GeneInteractions.ExportPathwayGraph(Model)
        Dim regulators = Footprints.ToArray(Function(x) x.Regulator).Distinct.ToArray(
            Function(x) New FileStream.Node With {
                .ID = x,
                .NodeType = "TF"
            })
        Dim regulations = (From x In Footprints
                           Let regulation = New FileStream.NetworkEdge With {
                               .value = x.Pcc,
                               .FromNode = x.Regulator,
                               .ToNode = x.ORF,
                               .Interaction = "Regulation"
                           }
                           Select regulation
                           Group regulation By regulation.ToNode Into Group) _
                               .ToDictionary(Function(x) x.ToNode,
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
                .Nodes _
                .ToArray(Function(x) regulations.TryGetValue(x.ID)) _
                .Unlist
            Dim Path As String = $"{outDIR}/{kMod.Key}/"

            If edges.IsNullOrEmpty Then
                Continue For
            End If

            Call kMod.Value.Nodes.Add(regulators)
            Call kMod.Value.Edges.Add(edges)
            Call kMod.Value.Save(Path, Encodings.UTF8)
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
                                          Function(x) x.Group.ToArray(
                                          Function(xx) xx.Pathways.ToArray(
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
                                          Function(x) x.Group.ToArray(
                                          Function(xx) xx.Pathways.ToArray(
                                          Function(xxx) networks.TryGetValue(xxx.EntryId))).Unlist)
        Dim dict = classes.ToDictionary(Function(x) x.Key,
                                        Function(x) __mergeCommon(x.Value))
        Return dict
    End Function

    Private Function __mergeCommon(source As IEnumerable(Of ______NETWORK__)) As ______NETWORK__
        Dim Nods = source.ToArray(Function(x) x.Nodes, where:=Function(x) Not x Is Nothing).Unlist
        Dim Edges As List(Of FileStream.NetworkEdge) =
            source.ToArray(Function(x) x.Edges, where:=Function(x) Not x Is Nothing).Unlist

        Dim __nodes = LinqAPI.Exec(Of Node) <=
            From node
            In (From node As FileStream.Node
                In Nods
                Select node
                Group node By node.ID Into Group)
            Select New FileStream.Node With {
                .ID = node.ID,
                .NodeType = node.Group _
                    .ToArray(Function(x) x.NodeType) _
                    .Distinct _
                    .ToArray _
                    .JoinBy("; ")
            }
        Dim __edges = (From edge As FileStream.NetworkEdge
                       In Edges
                       Select edge,
                           id = edge.GetDirectedGuid
                       Group By id Into Group).ToArray(Function(x) x.Group.First.edge)
        Dim net As ______NETWORK__ = New ______NETWORK__ With {
            .Edges = __edges,
            .Nodes = __nodes
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

        For Each ph As bGetObject.Pathway In model.GetAllPathways
            If ph.Modules.IsNullOrEmpty Then
                Continue For
            End If

            Dim LQuery = (From m In ph.Modules
                          Let km = networks.TryGetValue(m.Key)
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
    <Argument("/brief", True,
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
                              In net.Edges
                              Where String.Equals(x.Interaction, PathwayGene)
                              Select x
                              Group x By x.FromNode Into Group)  ' 代谢途径基因按照模块分组
                Dim rhaves As String() = footprints.ToArray(Function(x) x.ORF).Distinct.ToArray
                Dim Trim = (From m In LQuery
                            Where (From x As FileStream.NetworkEdge In m.Group
                                   Where Array.IndexOf(rhaves, x.ToNode) > -1
                                   Select x).FirstOrDefault Is Nothing
                            Select m).ToArray
                nulls = New FileStream.NetworkTables + Trim.ToArray(Function(x) x.Group).IteratesALL ' 添加新的网络节点
                net -= nulls.Edges  ' 删除旧的网络节点
                nulls += net <= nulls.Edges.ToArray(Function(x) {x.FromNode, x.ToNode}).IteratesALL
                net -= nulls.Nodes
            End If
        End If

        If cut <> 0R Then  ' 按照阈值筛选
            net.Edges =
                LinqAPI.Exec(Of NetworkEdge) <= From x As NetworkEdge
                                                In net.Edges
                                                Where Math.Abs(x.value) >= cut
                                                Select x
            out = out & "." & cut
        End If

        If Not nulls Is Nothing Then
            Call nulls.Save(out & "/no-regs/", Encodings.ASCII)
        End If
        Return net.Save(out, Encodings.ASCII).CLICode
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
            Dim nodes As New Dictionary(Of Node)(graph.Nodes)

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

        Return graph.Save(out, Encodings.ASCII).CLICode
    End Function
End Module
