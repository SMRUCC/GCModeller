﻿#Region "Microsoft.VisualBasic::0bb4d02b6cbf32c093e59e9e811347fc, gr\network-visualization\Network.IO.Extensions\IO\TabularCreator.vb"

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

    '     Module TabularCreator
    ' 
    '         Function: createNodesTable, Tabular
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports names = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic.NamesOf

Namespace FileStream

    Public Module TabularCreator

        ''' <summary>
        ''' 将<see cref="NetworkGraph"/>保存到csv文件之中
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="propertyNames">
        ''' The data property names of nodes and edges.
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function Tabular(g As NetworkGraph,
                                Optional propertyNames$() = Nothing,
                                Optional is2D As Boolean = True,
                                Optional creators As String() = Nothing,
                                Optional title$ = Nothing,
                                Optional description$ = Nothing,
                                Optional keywords$() = Nothing,
                                Optional links$() = Nothing,
                                Optional meta As Dictionary(Of String, String) = Nothing) As NetworkTables

            Dim data As New MetaData With {
                .create_time = Now,
                .creators = creators,
                .description = description,
                .keywords = keywords,
                .links = links,
                .title = title,
                .additionals = meta
            }

            Return g.Tabular(propertyNames, is2D, data)
        End Function

        ''' <summary>
        ''' 将<see cref="NetworkGraph"/>保存到csv文件之中
        ''' </summary>
        ''' <param name="g"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Tabular(g As NetworkGraph) As NetworkTables
            Return g.Tabular(meta:=New MetaData)
        End Function

        ''' <summary>
        ''' 将<see cref="NetworkGraph"/>保存到csv文件之中
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="properties">
        ''' The data property names of nodes and edges.
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function Tabular(g As NetworkGraph,
                                Optional properties$() = Nothing,
                                Optional is2D As Boolean = True,
                                Optional meta As MetaData = Nothing) As NetworkTables

            Dim nodes As Node() = g.createNodesTable(properties, is2D).ToArray
            Dim edges As New List(Of NetworkEdge)

            For Each l As Edge In g.graphEdges
                edges += New NetworkEdge With {
                    .fromNode = l.U.label,
                    .toNode = l.V.label,
                    .interaction = l.data(names.REFLECTION_ID_MAPPING_INTERACTION_TYPE),
                    .value = l.weight,
                    .Properties = New Dictionary(Of String, String) From {
                        {NameOf(EdgeData.label), l.data.label},
                        {names.REFLECTION_ID_MAPPING_EDGE_GUID, l.ID}
                    }
                }

                With edges.Last
                    If Not properties Is Nothing Then
                        For Each key As String In properties.Where(Function(p) l.data.HasProperty(p))
                            .ItemValue(key) = l.data(key)
                        Next
                    End If
                End With
            Next

            Return New NetworkTables With {
                .edges = edges,
                .nodes = nodes,
                .meta = If(meta, New MetaData)
            }
        End Function

        <Extension>
        Private Iterator Function createNodesTable(g As NetworkGraph, properties$(), is2Dlayout As Boolean) As IEnumerable(Of Node)
            Dim data As Dictionary(Of String, String)

            For Each n As Graph.Node In g.vertex
                If n.data Is Nothing Then
                    n.data = New NodeData
                End If

                data = New Dictionary(Of String, String) From {
                    {"weight", n.data.mass}
                }

                If Not n.data.initialPostion Is Nothing Then
                    ' skip coordination information when no layout data.
                    data("x") = n.data.initialPostion.x
                    data("y") = n.data.initialPostion.y

                    If Not is2Dlayout Then
                        data("z") = n.data.initialPostion.z
                    End If
                End If

                If Not n.data.color Is Nothing AndAlso n.data.color.GetType Is GetType(SolidBrush) Then
                    data(names.REFLECTION_ID_MAPPING_NODECOLOR) = DirectCast(n.data.color, SolidBrush).Color.ToHtmlColor
                End If

                If Not properties Is Nothing Then
                    For Each key As String In properties.Where(Function(p) n.data.HasProperty(p))
                        data(key) = n.data(key)
                    Next
                End If

                For Each key As String In {
                    names.REFLECTION_ID_MAPPING_DEGREE,
                    names.REFLECTION_ID_MAPPING_DEGREE_IN,
                    names.REFLECTION_ID_MAPPING_DEGREE_OUT,
                    names.REFLECTION_ID_MAPPING_RELATIVE_DEGREE_CENTRALITY,
                    names.REFLECTION_ID_MAPPING_RELATIVE_OUTDEGREE_CENTRALITY,
                    names.REFLECTION_ID_MAPPING_BETWEENESS_CENTRALITY,
                    names.REFLECTION_ID_MAPPING_RELATIVE_BETWEENESS_CENTRALITY
                }.Where(Function(p) n.data.HasProperty(p))

                    data(key) = n.data(key)
                Next

                ' 20191022
                ' name 会和cytoscape之中的name属性产生冲突
                ' 所以在这里修改为label
                If Not data.ContainsKey("label") Then
                    data.Add("label", n.data.label)
                End If

                Yield New Node With {
                    .ID = n.label,
                    .NodeType = n.data(names.REFLECTION_ID_MAPPING_NODETYPE),
                    .Properties = data
                }
            Next
        End Function
    End Module
End Namespace
