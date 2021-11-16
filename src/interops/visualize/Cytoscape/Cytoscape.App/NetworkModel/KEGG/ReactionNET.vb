#Region "Microsoft.VisualBasic::329bbd08728b06aaef7dc5b15cd4952f, visualize\Cytoscape\Cytoscape.App\NetworkModel\KEGG\ReactionNET.vb"

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

    '     Module ReactionNET
    ' 
    '         Function: __buildNET, BuildCompoundHash, (+2 Overloads) BuildNET, LoadObjects, ModelNET
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Cytoscape.GCModeller.FileSystem
Imports SMRUCC.genomics.Visualize.Cytoscape.GCModeller.FileSystem.KEGG.Directories
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml.Nodes
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace NetworkModel.KEGG

    ''' <summary>
    ''' 反应过程对象之间构成网络
    ''' </summary>
    <Package("Cytoscape.Reaction.NET")>
    Public Module ReactionNET

        <ExportAPI("Reaction.Loads")>
        Public Function LoadObjects(Optional source As String = "") As bGetObject.Reaction()
            Dim files = FileIO.FileSystem.GetFiles(TryGetSource(source, AddressOf GetReactions),
                                                   FileIO.SearchOption.SearchAllSubDirectories,
                                                   "*.xml")
            Dim LQuery = (From xml As String
                          In files.AsParallel
                          Let model As bGetObject.Reaction = xml.LoadXml(Of bGetObject.Reaction)
                          Where Not model Is Nothing
                          Select model).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 转换为代谢物和反应的关系
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Hash.Compounds")>
        Public Function BuildCompoundHash(source As IEnumerable(Of bGetObject.Reaction)) As Dictionary(Of String, bGetObject.Reaction())
            Dim preCache = (From x As bGetObject.Reaction
                            In source.AsParallel
                            Let LDM As Equation = x.ReactionModel
                            Select (From cp As CompoundSpecieReference
                                    In LDM.GetMetabolites
                                    Select cp.ID,
                                        rxn = x)).IteratesALL
            Dim hash = (From x In preCache
                        Select x
                        Group x By x.ID Into Group) _
                             .ToDictionary(Function(x) x.ID,
                                           Function(x) x.Group.Select(Function(xx) xx.rxn).ToArray)
            Return hash
        End Function

        ''' <summary>
        ''' 这个函数可能会加载数据库中的全部的资源数据
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("NET.Build")>
        Public Function BuildNET(source As String) As FileStream.NetworkTables
            Return BuildNET(LoadObjects(source))
        End Function

        ''' <summary>
        ''' 这个函数则可以从细菌模型之中选择性的构建网络
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("NET.Build")>
        Public Function BuildNET(source As IEnumerable(Of bGetObject.Reaction)) As FileStream.NetworkTables
            Dim cpHash = BuildCompoundHash(source)
            Dim nodes As New List(Of FileStream.Node)
            Dim nodeTmp As FileStream.Node() = source.Select(Function(x) New FileStream.Node With {.ID = x.ID, .NodeType = "Flux"})
            Call nodes.AddRange(nodeTmp)
            nodeTmp = cpHash.Select(Function(x) New FileStream.Node With {
                                         .ID = x.Key,
                                         .NodeType = "Metabolite",
                                         .Properties = New Dictionary(Of String, String) From {{"associate", x.Value.Length}}})
            Call nodes.AddRange(nodeTmp)

            Dim edges As FileStream.NetworkEdge() = cpHash.Select(Function(x) __buildNET(x.Key, x.Value)).ToVector

            Return New FileStream.NetworkTables With {
                .Edges = edges.ToArray,
                .Nodes = nodes.ToArray
            }
        End Function

        <ExportAPI("NET.Build")>
        Public Function ModelNET(model As XmlModel, Optional sourceDIR As String = "") As FileStream.NetworkTables
            Dim maps As EC_Mapping() = model.EC_Mappings
            Dim source As Dictionary(Of String, bGetObject.Reaction()) = (From x As bGetObject.Reaction
                                                                          In LoadObjects(sourceDIR)
                                                                          Select x
                                                                          Group x By x.ID Into Group) _
                                                                                .ToDictionary(Function(x) x.ID,
                                                                                              Function(x) x.Group.ToArray)
            Dim mapsSource = (From x As String
                              In maps.Select(Function(xx) xx.ECMaps.Select(Function(xxx) xxx.Reactions)).IteratesALL.IteratesALL
                              Where source.ContainsKey(x)
                              Select source(x)).IteratesALL
            Dim rxns = (From x In source.Values.IteratesALL
                        Where x.Enzyme.IsNullOrEmpty
                        Select x).Join(mapsSource).ToArray
            Dim net As FileStream.NetworkTables = BuildNET(rxns)
            Return net
        End Function

        Private Function __buildNET(met As String, rxn As bGetObject.Reaction()) As FileStream.NetworkEdge()
            Dim LQuery = (From x In rxn
                          Let ldm = x.ReactionModel
                          Let s As Double = ldm.GetCoEfficient(met)
                          Let from As String = If(s < 0, met, x.ID)
                          Let toNode As String = If(s < 0, x.ID, met)
                          Let itr As String = If(ldm.Reversible, "Reversible", "Directed")
                          Select New FileStream.NetworkEdge With {
                              .FromNode = from,
                              .ToNode = toNode,
                              .Interaction = itr,
                              .value = s,
                              .Properties = New Dictionary(Of String, String) From {{"def", x.Equation}}}).ToArray
            Return LQuery
        End Function
    End Module
End Namespace
