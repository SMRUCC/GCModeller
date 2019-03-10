﻿#Region "Microsoft.VisualBasic::381ff17f065d30b6227ea0954796cf1c, models\Networks\KEGG\KEGGMap\PathwayMapNetwork.vb"

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

' Module PathwayMapNetwork
' 
'     Function: BuildModel
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
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
    ''' </summary>
    ''' <param name="br08901$"></param>
    ''' <returns></returns>
    Public Function BuildModel(br08901$) As NetworkTables
        Dim nodes As New List(Of Node)

        For Each Xml As String In ls - l - r - "*.XML" <= br08901
            Dim pathwayMap As PathwayMap = Xml.LoadXml(Of PathwayMap)

            nodes += New Node(pathwayMap.EntryId) With {
                .NodeType = pathwayMap.brite?.class,
                .Properties = New Dictionary(Of String, String) From {
                    {"KO", pathwayMap.KEGGOrthology _
                        .Terms _
                        .SafeQuery _
                        .Select(Function(x) x.name) _
                        .JoinBy(PathwayMapNetwork.delimiter)
                    },
                    {"KO.counts", pathwayMap.KEGGOrthology.Length},
                    {"pathway.name", pathwayMap.name} ' 直接使用name作为键名会和cytoscape网络模型之中的name产生冲突
                }
            }
        Next

        Dim edges As New List(Of NetworkEdge)

        For Each a As Node In nodes
            Dim KO As Index(Of String) = Strings.Split(a!KO, delimiter).Indexing

            For Each b As Node In nodes.Where(Function(node) node.ID <> a.ID)
                Dim kb = Strings.Split(b!KO, delimiter)
                Dim n = kb.Where(Function(id) KO(id) > -1).AsList
                Dim type$

                If a.NodeType = b.NodeType Then
                    type = "module internal"
                Else
                    type = "module outbounds"
                End If

                If Not n = 0 Then
                    edges += New NetworkEdge With {
                        .Interaction = type,
                        .FromNode = a.ID,
                        .ToNode = b.ID,
                        .value = n.Count,
                        .Properties = New Dictionary(Of String, String) From {
                            {"intersets", n.JoinBy(delimiter)}
                        }
                    }
                End If
            Next
        Next

        Dim ranks As Vector = edges _
            .Select(Function(x) x.value) _
            .RangeTransform("0,100") _
            .AsVector

        edges = edges(Which.IsTrue(ranks >= 3))

        Return New NetworkTables(edges, nodes)
    End Function
End Module
