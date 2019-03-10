﻿#Region "Microsoft.VisualBasic::d50580861295f0352d2ba1f141f556e7, visualize\Cytoscape\Cytoscape\Graph\Serialization\ExportToFile.vb"

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

    '     Module ExportToFile
    ' 
    '         Function: __createTypeMapping, __exportEdge, __exportEdges, __exportNode, __exportNodes
    '                   __getMap, __mapInterface, __mapNodes, __mapping, (+4 Overloads) Export
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.ComponentModels
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection.Reflector
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic.NameOf
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkEdge
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Node
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML

Namespace CytoscapeGraphView.Serialization

    ''' <summary>
    ''' 将网络模型的数据导出至Cytoscape的网络模型文件之中
    ''' </summary>
    ''' <remarks></remarks>
    Public Module ExportToFile

        Public Function Export(Of Edge As INetworkEdge)(
                                 nodes As IEnumerable(Of FileStream.Node),
                                 edges As IEnumerable(Of Edge),
                                 Optional title$ = "NULL") As Graph

            Return Export(Of FileStream.Node, Edge)(nodes.ToArray, edges.ToArray, title)
        End Function

        ''' <summary>
        ''' 对于所有的属性值，Cytoscape之中的数据类型会根据属性值的类型自动映射
        ''' </summary>
        ''' <typeparam name="Node"></typeparam>
        ''' <typeparam name="Edge"></typeparam>
        ''' <param name="nodeList"></param>
        ''' <param name="edges"></param>
        ''' <param name="title"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Export(Of Node As INode, Edge As INetworkEdge)(nodeList As Node(), edges As Edge(), Optional title$ = "NULL") As Graph
            Dim Model As Graph = New Graph With {
                    .Label = "0",
                    .ID = "1",
                    .Directed = "1",
                    .Graphics = Graphics.DefaultValue
            }
            Dim ModelAttributes = New GraphAttribute() {
                New GraphAttribute With {
                    .Name = ATTR_SHARED_NAME,
                    .Value = title,
                    .Type = ATTR_VALUE_TYPE_STRING
                },
                New GraphAttribute With {
                    .Name = ATTR_NAME,
                    .Value = title,
                    .Type = ATTR_VALUE_TYPE_STRING
                }
            }
            Dim EdgeSchema = SchemaProvider.CreateObject(GetType(Edge), False)
            Dim interMaps = __mapInterface(EdgeSchema)

            VBDebugger.Mute = False

            Model.Nodes = __exportNodes(nodeList, GetType(Node).GetDataFrameworkTypeSchema(False))
            Model.Edges = __exportEdges(Of Edge)(edges,
                                                 Nodes:=Model.Nodes.ToDictionary(Function(item) item.label),
                                                 EdgeTypeMapping:=GetType(Edge).GetDataFrameworkTypeSchema(False),
                                                 schema:=interMaps)
            Model.Attributes = ModelAttributes
            Model.NetworkMetaData = New XGMML.NetworkMetadata With {
                .Title = "GCModeller Exports: " & title,
                .Description = "http://code.google.com/p/genome-in-code/cytoscape"
            }

            VBDebugger.Mute = True

            Return Model
        End Function

        Public Function Export(Of Node As FileStream.Node,
                                  Edge As FileStream.NetworkEdge)(
                               network As Network(Of Node, Edge),
                               Optional title$ = "NULL") As Graph

            Return Export(network.Nodes, network.Edges, title)
        End Function

        ''' <summary>
        ''' 属性类型可以进行用户的自定义映射
        ''' </summary>
        ''' <typeparam name="Node"></typeparam>
        ''' <typeparam name="Edge"></typeparam>
        ''' <param name="NodeList"></param>
        ''' <param name="Edges"></param>
        ''' <param name="NodeTypeMapping"></param>
        ''' <param name="EdgeTypeMapping"></param>
        ''' <param name="Title"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Export(Of Node As FileStream.Node,
                                  Edge As FileStream.NetworkEdge)(
                              NodeList As Node(),
                                 Edges As Edge(),
                       NodeTypeMapping As Dictionary(Of String, Type),
                       EdgeTypeMapping As Dictionary(Of String, Type),
                              Optional Title$ = "NULL") As Graph

            Dim Model As New Graph With {
                .Label = "0",
                .ID = "1",
                .Directed = "1",
                .Graphics = Graphics.DefaultValue
            }
            Dim ModelAttributes = New XGMML.Attribute() {
                New XGMML.Attribute With {
                    .Name = ATTR_SHARED_NAME,
                    .Value = Title,
                    .Type = ATTR_VALUE_TYPE_STRING
                },
                New XGMML.Attribute With {
                    .Name = ATTR_SHARED_NAME,
                    .Value = Title,
                    .Type = ATTR_VALUE_TYPE_STRING
                }
            }
            Dim EdgeSchema = SchemaProvider.CreateObject(GetType(Edge), False)
            Dim interMaps = __mapInterface(EdgeSchema)
            Dim hash As Dictionary(Of String, XGMML.Node) =
                Model.Nodes.ToDictionary(Function(x) x.label)

            VBDebugger.Mute = True

            Model.Nodes = __exportNodes(NodeList, NodeTypeMapping)
            Model.Edges = __exportEdges(Edges, hash, EdgeTypeMapping, interMaps)
            Model.Attributes = ModelAttributes

            VBDebugger.Mute = False

            Return Model
        End Function

        Const propGET As String = "get_"

        Private Function __mapInterface(schema As SchemaProvider) As Dictionary(Of String, String)
            Dim mapEdge = schema.DeclaringType.GetInterfaceMap(GetType(INetworkEdge))
            Dim mapNodes = schema.DeclaringType.GetInterfaceMap(GetType(IInteraction))
            Dim maps As New Dictionary(Of String, String)

            Dim edgeMaps = (From i As SeqValue(Of MethodInfo)
                            In mapEdge.TargetMethods.SeqIterator
                            Let [interface] = mapEdge.InterfaceMethods(i)
                            Where InStr([interface].Name, propGET) = 1
                            Select ([interface]:=[interface],
                                mMethod:=(+i))) _
                                .ToDictionary(Function(x) x.interface.Name.Replace(propGET, ""))
            Dim nodeMaps = (From i As SeqValue(Of MethodInfo)
                            In mapNodes.TargetMethods.SeqIterator
                            Let [interface] = mapNodes.InterfaceMethods(i)
                            Where InStr([interface].Name, propGET) = 1
                            Select ([interface]:=[interface],
                                mMethod:=(+i))) _
                                .ToDictionary(Function(x) x.interface.Name.Replace(propGET, ""))

            Dim map As New Value(Of ([interface] As MethodInfo, mMethod As MethodInfo))

            Call maps.Add(
                REFLECTION_ID_MAPPING_FROM_NODE,
                __getMap((map = nodeMaps(NameOf(IInteraction.source))).interface, (+map).mMethod, schema))

            Call maps.Add(
                REFLECTION_ID_MAPPING_TO_NODE,
                __getMap((map = nodeMaps(NameOf(IInteraction.target))).interface, (+map).mMethod, schema))

            Call maps.Add(
                REFLECTION_ID_MAPPING_CONFIDENCE,
                __getMap((map = edgeMaps(NameOf(INetworkEdge.value))).interface, (+map).mMethod, schema))

            Call maps.Add(
                REFLECTION_ID_MAPPING_INTERACTION_TYPE,
                __getMap((map = edgeMaps(NameOf(INetworkEdge.Interaction))).interface, (+map).mMethod, schema))

            Return maps
        End Function

        Private Function __getMap([interface] As MethodInfo, mMethod As MethodInfo, schema As SchemaProvider) As String
            Dim mapName As String = mMethod.Name.Replace(propGET, "")
            Dim mapFiled As StorageProvider = schema.GetField(mapName)

            If mapFiled Is Nothing Then
                Return mapName
            Else
                mapName = mapFiled.Name
                Return mapName
            End If
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <returns>输入属性名，然后返回属性的值类型的映射</returns>
        ''' <remarks></remarks>
        Private Function __createTypeMapping(typeMapping As Dictionary(Of String, Type)) As Func(Of String, String)
            If typeMapping.IsNullOrEmpty Then
                Return Function(null) ATTR_VALUE_TYPE_STRING
            End If

            Dim CytoscapeMapping As Dictionary(Of Type, String) = XGMML.Attribute.TypeMapping
            Dim Mapping As Func(Of String, String) =
                Function(attrKey) __mapping(attrKey, typeMapping, CytoscapeMapping)
            Return Mapping
        End Function

        Private Function __mapping(attrKey$,
                                   typeMapping As Dictionary(Of String, Type),
                                   cytoscapeMapping As Dictionary(Of Type, String)) As String
            Dim type As Type = typeMapping.TryGetValue(attrKey)

            If Not type Is Nothing AndAlso
                cytoscapeMapping.ContainsKey(type) Then

                Return cytoscapeMapping(type)
            Else
                Return ATTR_VALUE_TYPE_STRING
            End If
        End Function

        Private Function __exportNodes(Of Node As INode)(nodes As Node(), nodeTypeMapping As Dictionary(Of String, Type)) As XGMML.Node()
            Dim buf As List(Of Dictionary(Of String, String)) =
                nodes.ExportAsPropertyAttributes(False)
            Dim typeMapping As Func(Of String, String) =
                __createTypeMapping(nodeTypeMapping)
            Dim LQuery = From x As Dictionary(Of String, String)
                         In buf.AsParallel
                         Let node_obj = __exportNode(x, __getType:=typeMapping)
                         Select node_obj
                         Group node_obj By node_obj.label Into Group
                         Order By label Ascending ' Linq查询在这里会被执行两次，不清楚是什么原因
            Return LQuery _
                .Select(Function(x) x.Group) _
                .Select(AddressOf DefaultFirst) _
                .WriteAddress  ' 生成节点数据并去除重复
        End Function

        Private Function __exportNode(dict As Dictionary(Of String, String), __getType As Func(Of String, String)) As XGMML.Node
            Dim ID As String = dict(REFLECTION_ID_MAPPING_IDENTIFIER)
            Dim attrs As New List(Of XGMML.Attribute)

            attrs += New XGMML.Attribute With {
                .Name = ATTR_SHARED_NAME,
                .Value = ID,
                .Type = ATTR_VALUE_TYPE_STRING
            }
            attrs += New XGMML.Attribute With {
                .Name = ATTR_NAME,
                .Value = ID,
                .Type = ATTR_VALUE_TYPE_STRING
            }
            Call dict.Remove(REFLECTION_ID_MAPPING_IDENTIFIER)

            attrs += From item As KeyValuePair(Of String, String)
                     In dict
                     Select New XGMML.Attribute With {
                         .Name = item.Key,
                         .Value = item.Value,
                         .Type = __getType(item.Key)
                     }

            Dim node As New XGMML.Node With {
                .label = ID,
                .Attributes = attrs.ToArray,
                .Graphics = New NodeGraphics
            }

            Return node
        End Function

        Private Function __exportEdges(Of Edge As INetworkEdge)(
                                         Edges As Edge(),
                                         Nodes As Dictionary(Of String, XGMML.Node),
                               EdgeTypeMapping As Dictionary(Of String, Type),
                                        schema As Dictionary(Of String, String)) As XGMML.Edge()

            Dim buf = __mapNodes(Edges.ExportAsPropertyAttributes(False), schema)
            Dim typeMapping As Func(Of String, String) =
                __createTypeMapping(EdgeTypeMapping)
            Dim LQuery As XGMML.Edge() =
                buf.Select(Function(x) x.__exportEdge(Nodes, typeMapping)) _
                .WriteAddress(offset:=Nodes.Count)
            Return LQuery
        End Function

        Private Function __mapNodes(ByRef buffer As List(Of Dictionary(Of String, String)), Schema As Dictionary(Of String, String)) As List(Of Dictionary(Of String, String))
            For Each dict As Dictionary(Of String, String) In buffer
                For Each map As KeyValuePair(Of String, String) In Schema
                    If Not dict.ContainsKey(map.Key) Then
                        If dict.ContainsKey(map.Value) Then
                            Dim value As String = dict(map.Value)
                            Call dict.Add(map.Key, value)
                        End If
                    End If
                Next
            Next

            Return buffer
        End Function

        <Extension>
        Private Function __exportEdge(dict As Dictionary(Of String, String), Nodes As Dictionary(Of String, XGMML.Node), __getType As Func(Of String, String)) As XGMML.Edge
            Dim nodeName As String = dict(REFLECTION_ID_MAPPING_FROM_NODE)
            Dim fromNode As XGMML.Node = Nodes.TryGetValue(nodeName)

            If fromNode Is Nothing Then
                Call $"fromNode '{nodeName}' could not be found in the node list!".__DEBUG_ECHO
                fromNode = New XGMML.Node With {
                    .label = nodeName,
                    .id = Nodes.Count
                }
                Call Nodes.Add(nodeName, fromNode)
                Call $"INSERT this absence node into network...".__DEBUG_ECHO
            Else
                nodeName = dict(REFLECTION_ID_MAPPING_TO_NODE)
            End If

            Dim toNode As XGMML.Node = Nodes.TryGetValue(nodeName)
            If toNode Is Nothing Then
                Call $"toNode '{nodeName}' could not be found in the node list!".__DEBUG_ECHO
                toNode = New XGMML.Node With {
                    .label = nodeName,
                    .id = Nodes.Count
                }
                Call Nodes.Add(nodeName, toNode)
                Call $"INSERT this absence node into network...".__DEBUG_ECHO
            End If

            Dim InteractionType As String = dict.TryGetValue(REFLECTION_ID_MAPPING_INTERACTION_TYPE)
            InteractionType = If(String.IsNullOrEmpty(InteractionType), "interact", InteractionType)

            Dim label As String = String.Format("{0} ({1}) {2}", fromNode.label, InteractionType, toNode.label)
            Dim attrs As List(Of XGMML.Attribute) =
                New List(Of XGMML.Attribute)
            attrs += New XGMML.Attribute With {
                .Name = ATTR_SHARED_NAME,
                .Value = label,
                .Type = ATTR_VALUE_TYPE_STRING
            }
            attrs += New XGMML.Attribute With {
                .Name = ATTR_NAME,
                .Value = label,
                .Type = ATTR_VALUE_TYPE_STRING
            }
            attrs += New XGMML.Attribute With {
                .Name = ATTR_SHARED_INTERACTION,
                .Value = InteractionType,
                .Type = ATTR_VALUE_TYPE_STRING
            }
            Call dict.Remove(REFLECTION_ID_MAPPING_FROM_NODE)
            Call dict.Remove(REFLECTION_ID_MAPPING_TO_NODE)
            Call dict.Remove(REFLECTION_ID_MAPPING_INTERACTION_TYPE)

            attrs += From item As KeyValuePair(Of String, String)
                     In dict
                     Select New XGMML.Attribute With {
                         .Name = item.Key,
                         .Value = item.Value,
                         .Type = __getType(item.Key)
                     }

            Dim Node As New XGMML.Edge With {
                    .Label = label,
                    .source = fromNode.id,
                    .target = toNode.id,
                    .Attributes = attrs,
                    .Graphics = New EdgeGraphics
            }
            Return Node
        End Function
    End Module
End Namespace
