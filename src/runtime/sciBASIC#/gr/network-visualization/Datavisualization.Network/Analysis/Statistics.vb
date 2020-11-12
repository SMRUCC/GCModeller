﻿#Region "Microsoft.VisualBasic::efc0df45ea6e93309eb19968e5e14b92, gr\network-visualization\Datavisualization.Network\Analysis\Statistics.vb"

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

    '     Module Statistics
    ' 
    '         Function: BetweennessCentrality, ComputeBetweennessCentrality, ComputeDegreeData, ComputeNodeDegrees, ConnectedDegrees
    '                   Sum
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis
Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.Dijkstra
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
Imports Microsoft.VisualBasic.Linq
Imports GraphNetwork = Microsoft.VisualBasic.Data.GraphTheory.Network
Imports names = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic.NamesOf
Imports Node = Microsoft.VisualBasic.Data.visualize.Network.Graph.Node

Namespace Analysis

    Public Module Statistics

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ComputeDegreeData(Of T As IInteraction)(edges As IEnumerable(Of T)) As ([in] As Dictionary(Of String, Integer), out As Dictionary(Of String, Integer))
            Return GraphNetwork.ComputeDegreeData(edges, Function(l) l.source, Function(l) l.target)
        End Function

        <Extension>
        Public Function Sum(degrees As ([in] As Dictionary(Of String, Integer), out As Dictionary(Of String, Integer))) As Dictionary(Of String, Integer)
            Dim degreeValue As New Dictionary(Of String, Integer)(degrees.in)

            For Each node As KeyValuePair(Of String, Integer) In degrees.out
                degreeValue(node.Key) += degreeValue(node.Key) + node.Value
            Next

            Return degreeValue
        End Function

        <Extension>
        Public Function BetweennessCentrality(graph As NetworkGraph, Optional undirected As Boolean = False) As Dictionary(Of String, Integer)
            Return DijkstraRouter.FromNetwork(graph, undirected).BetweennessCentrality
        End Function

        ''' <summary>
        ''' compute and write data of <see cref="names.REFLECTION_ID_MAPPING_BETWEENESS_CENTRALITY"/>, <see cref="names.REFLECTION_ID_MAPPING_RELATIVE_BETWEENESS_CENTRALITY"/>
        ''' </summary>
        ''' <param name="graph"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ComputeBetweennessCentrality(ByRef graph As NetworkGraph) As Dictionary(Of String, Integer)
            Dim data As Dictionary(Of String, Integer) = graph.BetweennessCentrality
            ' convert to double for avoid the integer upbound overflow
            ' when deal with the network graph in ultra large size
            Dim sumAll As Double = data.Values.Select(Function(i) CDbl(i)).Sum

            For Each node As Graph.Node In graph.vertex
                node.data.betweennessCentrality = data(node.label)
                node.data(names.REFLECTION_ID_MAPPING_BETWEENESS_CENTRALITY) = data(node.label)
                node.data(names.REFLECTION_ID_MAPPING_RELATIVE_BETWEENESS_CENTRALITY) = data(node.label) / sumAll
            Next

            Return data
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <returns>``[<see cref="Node.label"/> => degree]``</returns>
        <Extension>
        Public Function ConnectedDegrees(g As NetworkGraph) As Dictionary(Of String, Integer)
            Return g.graphEdges _
                .Select(Function(link) {link.U.label, link.V.label}) _
                .IteratesALL _
                .GroupBy(Function(id) id) _
                .ToDictionary(Function(ID) ID.Key,
                              Function(list)
                                  Return list.Count
                              End Function)
        End Function

        ''' <summary>
        ''' 这个函数计算网络的节点的degree，然后将degree数据写入节点的同时，通过字典返回给用户
        ''' </summary>
        ''' <param name="g"></param>
        ''' <returns>
        ''' ``[<see cref="Node.label"/> => degree]``
        ''' </returns>
        <Extension>
        Public Function ComputeNodeDegrees(ByRef g As NetworkGraph) As Dictionary(Of String, Integer)
            Dim connectNodes As Dictionary(Of String, Integer) = g.ConnectedDegrees
            Dim d%
            Dim dt As (Integer, Integer)
            Dim degreeList = g.graphEdges.ComputeDegreeData
            Dim sumAllOut As Double = degreeList.out.Values.Sum
            Dim sumAllDegree As Double = connectNodes.Values.Sum

            For Each node As Graph.Node In g.vertex
                If Not connectNodes.ContainsKey(node.label) Then
                    ' 这个节点是孤立的节点，度为零
                    node.data.SetValue(names.REFLECTION_ID_MAPPING_DEGREE, 0)
                    node.data.SetValue(names.REFLECTION_ID_MAPPING_DEGREE_IN, 0)
                    node.data.SetValue(names.REFLECTION_ID_MAPPING_DEGREE_OUT, 0)
                    node.data.SetValue(names.REFLECTION_ID_MAPPING_RELATIVE_DEGREE_CENTRALITY, 0)
                    node.data.SetValue(names.REFLECTION_ID_MAPPING_RELATIVE_OUTDEGREE_CENTRALITY, 0)
                Else
                    d = connectNodes(node.label)
                    dt = (0, 0)
                    node.data.SetValue(names.REFLECTION_ID_MAPPING_DEGREE, d)
                    node.data.SetValue(names.REFLECTION_ID_MAPPING_RELATIVE_DEGREE_CENTRALITY, d / sumAllDegree)

                    If degreeList.in.ContainsKey(node.label) Then
                        d = degreeList.in(node.label)
                        node.data.SetValue(names.REFLECTION_ID_MAPPING_DEGREE_IN, d)
                        dt = (d, 0)
                    End If
                    If degreeList.out.ContainsKey(node.label) Then
                        d = degreeList.out(node.label)
                        node.data.SetValue(names.REFLECTION_ID_MAPPING_DEGREE_OUT, d)
                        node.data.SetValue(names.REFLECTION_ID_MAPPING_RELATIVE_OUTDEGREE_CENTRALITY, d / sumAllOut)
                        dt = (dt.Item1, d)
                    End If

                    node.degree = dt
                End If
            Next

            Return connectNodes
        End Function
    End Module
End Namespace
