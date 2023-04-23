#Region "Microsoft.VisualBasic::a7d70797982cab15880a55bd28de9d90, R#\cytoscape_toolkit\kegg.vb"

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

' Module kegg
' 
'     Function: assignPathwayClass, compoundNetwork, topMaps
' 
'     Sub: assignEdgeClass, assignNodeClass
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

''' <summary>
''' The KEGG metabolism pathway network data R# scripting plugin for cytoscape software
''' </summary>
<Package("cytoscape.kegg", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module kegg

    ''' <summary>
    ''' Create kegg metabolism network based on the given metabolite compound data.
    ''' </summary>
    ''' <param name="reactions">The kegg ``br08201`` reaction database.</param>
    ''' <param name="compounds">Kegg compound id list</param>
    ''' <returns></returns>
    <ExportAPI("compounds.network")>
    Public Function compoundNetwork(reactions As ReactionTable(), compounds$(),
                                    Optional enzymes As Dictionary(Of String, String()) = Nothing,
                                    Optional filterByEnzymes As Boolean = False,
                                    Optional extended As Boolean = False,
                                    Optional strictReactionNetwork As Boolean = False,
                                    Optional enzymeBridged As Boolean = True,
                                    Optional random_layout As Boolean = True) As NetworkGraph
        Return compounds _
            .Select(Function(cpd)
                        Return New NamedValue(Of String)(cpd, cpd)
                    End Function) _
            .DoCall(Function(list)
                        Return reactions.BuildModel(
                            compounds:=list,
                            enzymes:=enzymes,
                            filterByEnzymes:=filterByEnzymes,
                            extended:=extended,
                            strictReactionNetwork:=strictReactionNetwork,
                            enzymeBridged:=enzymeBridged,
                            randomLayout:=random_layout
                        )
                    End Function)
    End Function

    ''' <summary>
    ''' assign pathway map id to the nodes in the given network graph
    ''' </summary>
    ''' <param name="graph">
    ''' the node vertex in this network graph object its label value 
    ''' could be one of: glycan, compound, kegg ortholog or reaction id 
    ''' </param>
    ''' <param name="maps"></param>
    ''' <param name="top3"></param>
    ''' <returns></returns>
    <ExportAPI("pathway_class")>
    Public Function assignPathwayClass(graph As NetworkGraph,
                                       maps As Map(),
                                       Optional top3 As Boolean = True,
                                       Optional excludesGlobalAndOverviewMaps As Boolean = True) As NetworkGraph
        ' map011xx
        Dim overviews As Index(Of String) = BriteHEntry.Pathway _
            .GetGlobalAndOverviewMaps _
            .Select(Function(a) a.name.Match("\d+")) _
            .Indexing

        If excludesGlobalAndOverviewMaps Then
            maps = maps _
                .Where(Function(a) Not a.EntryId.Match("\d+") Like overviews) _
                .ToArray
        End If

        Call graph.assignNodeClass(top3, maps)
        Call graph.assignEdgeClass(maps)

        Return graph
    End Function

    <Extension>
    Private Sub assignEdgeClass(graph As NetworkGraph, maps As Map())
        Dim edges = graph.graphEdges _
            .Where(Function(e) e.data.HasProperty("kegg")) _
            .ToArray
        Dim assignments As New Dictionary(Of String, List(Of String))
        Dim edgeIndex As New Dictionary(Of String, List(Of Edge))

        For Each edge As Edge In edges
            For Each id As String In edge.data("kegg").LoadJSON(Of String())
                If Not edgeIndex.ContainsKey(id) Then
                    edgeIndex(id) = New List(Of Edge)
                End If

                Call edgeIndex(id).Add(edge)

                If Not assignments.ContainsKey(id) Then
                    Call assignments.Add(id, New List(Of String))
                End If
            Next
        Next

        For Each map As Map In maps
            For Each id As String In map.GetMembers
                If assignments.ContainsKey(id) Then
                    Call assignments(id).Add(map.EntryId)
                End If
            Next
        Next

        Dim firstMapHits = assignments.topMaps(1000)

        For Each block In firstMapHits
            Dim mapHit As Map = maps.First(Function(a) a.EntryId = block.Key)

            For Each id As String In block.Select(Function(a) a.cid)
                For Each edge In edgeIndex(id)
                    If Not edge.data.HasProperty("map") Then
                        edge.data("map") = mapHit.EntryId
                        edge.data("mapName") = mapHit.name
                    End If
                Next
            Next
        Next
    End Sub

    <Extension>
    Private Function topMaps(assignments As Dictionary(Of String, List(Of String)), n As Integer) As IGrouping(Of String, (cid$, mapId$))()
        Return assignments _
            .Select(Function(a) a.Value.Select(Function(mapId) (cid:=a.Key, mapId))) _
            .IteratesALL _
            .GroupBy(Function(a) a.mapId) _
            .OrderByDescending(Function(m)
                                   Return m.Select(Function(a) a.cid).Distinct.Count
                               End Function) _
            .Take(n) _
            .ToArray
    End Function

    <Extension>
    Private Sub assignNodeClass(graph As NetworkGraph, top3 As Boolean, maps As Map())
        Dim compounds As Node() = graph.vertex _
             .Where(Function(a)
                        Return a.label.IsPattern("[GCKR]\d+") OrElse a.data.HasProperty("kegg")
                    End Function) _
             .ToArray
        Dim assignments As New Dictionary(Of String, List(Of String))
        Dim nodeIndex As New Dictionary(Of String, Node)

        For Each id As Node In compounds
            Call assignments.Add(id.label, New List(Of String))

            If id.data.HasProperty("kegg") AndAlso Not assignments.ContainsKey(id.data("kegg")) Then
                Call assignments.Add(id.data("kegg"), New List(Of String))
            End If

            If Not nodeIndex.ContainsKey(id.label) Then
                Call nodeIndex.Add(id.label, id)
            End If
            If id.data.HasProperty("kegg") AndAlso Not nodeIndex.ContainsKey(id.data("kegg")) Then
                Call nodeIndex.Add(id.data("kegg"), id)
            End If
        Next

        For Each map As Map In maps
            For Each id As String In map.GetMembers
                If assignments.ContainsKey(id) Then
                    assignments(id).Add(map.EntryId)
                End If
            Next
        Next

        If top3 Then
            Dim firstMapHits = assignments.topMaps(3)

            For Each block In firstMapHits
                Dim mapHit As Map = maps.First(Function(a) a.EntryId = block.Key)

                For Each id As String In block.Select(Function(a) a.cid)
                    If Not nodeIndex(id).data.HasProperty("map") Then
                        nodeIndex(id).data("map") = mapHit.EntryId
                        nodeIndex(id).data("mapName") = mapHit.name
                    End If
                Next
            Next
        Else
            For Each node In compounds
                node.data("maps") = assignments(node.label).ToArray.GetJson
            Next
        End If
    End Sub
End Module
