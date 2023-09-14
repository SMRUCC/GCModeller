﻿#Region "Microsoft.VisualBasic::2a33acef2a089ba5728ea2ae75ab2bc7, GCModeller\models\Networks\KEGG\KEGGMap\PathwayMapNetwork.vb"

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

    '   Total Lines: 87
    '    Code Lines: 63
    ' Comment Lines: 12
    '   Blank Lines: 12
    '     File Size: 3.33 KB


    ' Module PathwayMapNetwork
    ' 
    '     Function: BuildModel
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Module PathwayMapNetwork

    Const delimiter$ = "|"

    ''' <summary>
    ''' 这个函数所产生的模型是以代谢途径为主体对象的
    ''' 
    ''' 在这个函数里面产生的也是代谢途径与代谢途径之间的相互作用的概览图
    ''' </summary>
    ''' <param name="br08901">
    ''' <see cref="PathwayMap"/>
    ''' </param>
    ''' <returns></returns>
    Public Function BuildModel(br08901 As String) As NetworkGraph
        Dim g As New NetworkGraph
        Dim node As Node
        Dim nodeData As NodeData, edgeData As EdgeData
        Dim pathwayMap As PathwayMap

        For Each xml As String In ls - l - r - "*.XML" <= br08901
            pathwayMap = xml.LoadXml(Of PathwayMap)
            ' 直接使用name作为键名会和cytoscape网络模型之中的name产生冲突
            ' 所以下面的节点属性中
            ' 使用pathway.name来存储代谢途径的名称
            nodeData = New NodeData With {
                .origID = pathwayMap.EntryId,
                .label = pathwayMap.name,
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, pathwayMap.brite?.class},
                    {"KO", pathwayMap.KEGGOrthology _
                        .Terms _
                        .SafeQuery _
                        .Select(Function(x) x.name) _
                        .JoinBy(PathwayMapNetwork.delimiter)},
                    {"KO.counts", pathwayMap.KEGGOrthology.size}
                }
            }
            node = New Node(pathwayMap.EntryId, nodeData)

            Call g.AddNode(node, assignId:=True)
        Next

        For Each a As Node In g.vertex
            Dim KO As Index(Of String) = Strings.Split(a.data!KO, delimiter).Indexing

            For Each b As Node In g.vertex.Where(Function(vi) vi.ID <> a.ID)
                Dim kb = Strings.Split(b.data!KO, delimiter)
                Dim n = kb.Where(Function(id) KO(id) > -1).AsList
                Dim type$

                If a.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = b.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) Then
                    type = "module internal"
                Else
                    type = "module outbounds"
                End If

                If Not n = 0 Then
                    edgeData = New EdgeData With {
                        .length = n.Count,
                        .Properties = New Dictionary(Of String, String) From {
                            {"intersets", n.JoinBy(delimiter)},
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, type}
                        }
                    }
                    g.CreateEdge(g.GetElementByID(a.label), g.GetElementByID(b.label), n.Count, edgeData)
                End If
            Next
        Next

        Dim edges = g.graphEdges.ToArray
        Dim ranks As Vector = edges _
            .Select(Function(x) x.weight) _
            .RangeTransform(New Double() {0, 100}) _
            .AsVector

        For Each i As Integer In which(ranks < 3)
            Call g.RemoveEdge(edges(i))
        Next

        Return g
    End Function
End Module
