#Region "Microsoft.VisualBasic::e6515685c3c13a560eaa944eee6b277d, GCModeller\models\Networks\KEGG\Dunnart\Builder.vb"

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

    '   Total Lines: 204
    '    Code Lines: 165
    ' Comment Lines: 9
    '   Blank Lines: 30
    '     File Size: 8.19 KB


    '     Module Extensions
    ' 
    '         Function: CreateModel, FromNetwork, OptmizeGraph
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace Dunnart

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' convert the <see cref="NetworkGraph"/> object to Dunnart graph
        ''' </summary>
        ''' <param name="network"></param>
        ''' <param name="colorSet"></param>
        ''' <param name="groupKey"></param>
        ''' <param name="fillOpacity"></param>
        ''' <param name="lighten"></param>
        ''' <returns></returns>
        <Extension>
        Public Function FromNetwork(network As NetworkGraph,
                                    Optional colorSet As String = "Paired:c12",
                                    Optional groupKey As String = "map",
                                    Optional fillOpacity As Double = 0.5,
                                    Optional lighten As Double = 0.1) As GraphObject

            Dim nodes As New Dictionary(Of String, Node)
            Dim pos As AbstractVector
            Dim links As New List(Of Link)
            Dim constraints As New List(Of Constraint)
            Dim groups As New List(Of Group)

            For Each node In network.vertex
                pos = node.data.initialPostion
                nodes(node.ID) = New Node With {
                    .label = node.data.label,
                    .dunnartid = node.ID,
                    .index = node.ID,
                    .width = 60,
                    .height = 40,
                    .rx = 9,
                    .ry = 9,
                    .x = pos.x,
                    .y = pos.y
                }
            Next

            For Each link As Edge In network.graphEdges
                links += New Link With {.source = link.U.ID, .target = link.V.ID}
            Next

            Dim colors As LoopArray(Of String) = Designer _
                .GetColors(colorSet) _
                .Select(Function(c) c.ToHtmlColor) _
                .ToArray
            Dim color As Value(Of String) = ""
            Dim style As String

            For Each group In network.vertex.GroupBy(Function(a) a.data(groupKey))
                Dim mapName As String = group.Key
                Dim mapNodes = group.ToArray

                mapNodes = mapNodes _
                    .Where(Function(a)
                               Return a.EnumerateAdjacencies.Any(Function(n) mapNodes.Any(Function(o) o Is n))
                           End Function) _
                    .ToArray

                If mapName.StringEmpty OrElse mapNodes.Length <= 1 Then
                    Continue For
                End If

                style = $"fill:{(color = colors.Next).TranslateColor.Lighten(lighten).ToHtmlColor};
                          fill-opacity:{fillOpacity};
                          stroke:{color};
                          stroke-opacity:1;
                          "
                groups += New Group With {
                    .label = group.Key,
                    .padding = 10,
                    .style = style.TrimNewLine(""),
                    .leaves = mapNodes _
                        .Select(Function(a) a.ID) _
                        .ToArray
                }
            Next

            Return New GraphObject With {
                .constraints = constraints,
                .links = links,
                .nodes = nodes.Values.ToArray,
                .groups = groups
            }
        End Function

        <Extension>
        Public Function CreateModel(template As NetworkGraph, maps As Pathway(),
                                    Optional desc As Boolean = False,
                                    Optional colorSet As String = "Paired:c12",
                                    Optional fillOpacity As Double = 0.5,
                                    Optional lighten As Double = 0.1,
                                    Optional isConnected As Boolean = True) As GraphObject

            Dim mapHits As New Dictionary(Of String, Integer)
            Dim mapCompounds As New Dictionary(Of String, Index(Of String))

            template = template.Copy

            For Each map In maps
                mapHits.Add(map.EntryId, 0)
                mapCompounds(map.EntryId) = map.compound _
                    .SafeQuery _
                    .Select(Function(c) c.name) _
                    .Indexing
            Next

            For Each node In template.vertex
                For Each map In maps
                    If node.label Like mapCompounds(map.EntryId) Then
                        mapHits(map.EntryId) += 1
                    End If
                Next
            Next

            For Each node In template.vertex
                Dim contains = mapCompounds _
                    .Where(Function(map)
                               Return node.label Like map.Value
                           End Function) _
                    .Sort(Function(a) mapHits(a.Key), desc) _
                    .ToArray

                If contains.Length > 0 Then
                    Dim map = maps.KeyItem(contains.First.Key)

                    node.data("map") = map.name
                    node.data.label = map.compound.KeyItem(node.label).text
                End If
            Next

            Return If(Not isConnected, template, template.GetConnectedGraph) _
                .FromNetwork(
                    colorSet:=colorSet,
                    groupKey:="map",
                    fillOpacity:=fillOpacity,
                    lighten:=lighten
                )
        End Function

        <Extension>
        Public Function OptmizeGraph(template As NetworkGraph,
                                     Optional optmizeIterations As Integer = 100,
                                     Optional lowerDegrees As Integer = 3,
                                     Optional lowerAdjcents As Integer = 2) As NetworkGraph

            For i As Integer = 0 To optmizeIterations
                Dim top As KeyValuePair(Of String, Integer)() = template _
                    .ConnectedDegrees _
                    .OrderByDescending(Function(a) a.Value) _
                    .Take(3) _
                    .Where(Function(a) a.Value >= lowerDegrees) _
                    .ToArray

                If top.Length = 0 Then
                    Exit For
                End If

                Dim centers = template.ComputeBetweennessCentrality

                For Each topNode In top
                    Dim target = template.GetElementByID(topNode.Key)

                    If target Is Nothing Then
                        Continue For
                    End If

                    Dim adjcents = target _
                        .EnumerateAdjacencies _
                        .Where(Function(node)
                                   Return centers.ContainsKey(node.label) AndAlso
                                       node.EnumerateAdjacencies _
                                           .Where(Function(n) centers.ContainsKey(n.label)) _
                                           .Count > lowerAdjcents
                               End Function) _
                        .OrderBy(Function(node) centers(node.label)) _
                        .ToArray

                    If adjcents.Length > 0 Then
                        Call template.RemoveNode(adjcents.First)
                    End If
                Next
            Next

            Return template
        End Function
    End Module
End Namespace
