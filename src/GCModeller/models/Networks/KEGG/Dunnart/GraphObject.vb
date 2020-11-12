Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace Dunnart

    ''' <summary>
    ''' json model for visualize of the cola network graph
    ''' </summary>
    Public Class GraphObject

        Public Property nodes As Node()
        Public Property links As Link()
        Public Property constraints As Constraint()
        Public Property groups As Group()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    <HideModuleName>
    Public Module Extensions

        <Extension>
        Public Function FromNetwork(network As NetworkGraph,
                                    Optional colorSet As String = "Paired:c12",
                                    Optional groupKey As String = "map") As GraphObject

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

            For Each group In network.vertex.GroupBy(Function(a) a.data(groupKey))
                If group.Key.StringEmpty Then
                    Continue For
                End If

                groups += New Group With {
                    .label = group.Key,
                    .padding = 10,
                    .style = $"fill:{(color = colors.Next).TranslateColor.Lighten.ToHtmlColor};fill-opacity:0.3;stroke:{color};stroke-opacity:1",
                    .leaves = group.Select(Function(a) a.ID).ToArray
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
                                    Optional colorSet As String = "Paired:c12") As GraphObject

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
                    node.data("map") = contains.First.Key
                    node.data.label = maps.KeyItem(node.data!map).compound.KeyItem(node.label).text
                End If
            Next

            Return template.FromNetwork(colorSet, groupKey:="map")
        End Function
    End Module
End Namespace