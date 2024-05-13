#Region "Microsoft.VisualBasic::31dedb578ea039f53ebef6bb1995c070, models\Networks\KEGG\SimpleBuilder.vb"

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

    '   Total Lines: 104
    '    Code Lines: 76
    ' Comment Lines: 11
    '   Blank Lines: 17
    '     File Size: 4.27 KB


    ' Module SimpleBuilder
    ' 
    '     Function: GraphQueryByCompoundList, KOGroupTable
    ' 
    '     Sub: loopCompoundNode, processCompoundLink, processReactionClassLink
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module SimpleBuilder

    Public Const Delimiter$ = " == "

    ''' <summary>
    ''' Using for the group values inforamtion for <see cref="BuildModel"/> function.
    ''' </summary>
    ''' <returns></returns>
    Public Function KOGroupTable() As Dictionary(Of String, String)
        Return PathwayMapping _
            .DefaultKOTable _
            .ToDictionary(Function(KO) KO.Key,
                          Function(KO)
                              Return KO.Value.parent.description
                          End Function)
    End Function

    ''' <summary>
    ''' Build a network based on a given compound id set
    ''' </summary>
    ''' <param name="compoundIds"></param>
    ''' <param name="reactions"></param>
    ''' <param name="maps"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GraphQueryByCompoundList(compoundIds As IEnumerable(Of String), reactions As ReactionClassifier, maps As MapRepository) As NetworkGraph
        Dim g As New NetworkGraph
        Dim usedLinks As New Index(Of String)
        Dim compoundIndex As Index(Of String) = compoundIds.Indexing

        For Each cid As String In compoundIndex.Objects
            g.CreateNode(cid)
        Next

        For Each cid As String In compoundIndex.Objects
            Call g.loopCompoundNode(reactions.QueryByCompoundId(cid), compoundIndex, maps)
        Next

        Return g
    End Function

    <Extension>
    Private Sub loopCompoundNode(g As NetworkGraph, links As ReactionClass(), compoundIndex As Index(Of String), maps As MapRepository)
        For Each link As ReactionClass In links
            Call link.processReactionClassLink(g, compoundIndex, maps)
        Next
    End Sub

    <Extension>
    Private Sub processReactionClassLink(link As ReactionClass, g As NetworkGraph, compoundIndex As Index(Of String), maps As MapRepository)
        For Each transform As ReactionCompoundTransform In link.reactantPairs
            If transform.from Like compoundIndex AndAlso transform.to Like compoundIndex Then
                If Not g.GetEdges(g.GetElementByID(transform.from), g.GetElementByID(transform.to)).Any Then
                    Call transform.processCompoundLink(link, g, maps)
                End If
            End If
        Next
    End Sub

    <Extension>
    Private Sub processCompoundLink(transform As ReactionCompoundTransform, link As ReactionClass, g As NetworkGraph, maps As MapRepository)
        Dim tuple As String() = {transform.from, transform.to}
        Dim linkdata As New EdgeData With {.label = link.definition}

        g.CreateEdge(transform.from, transform.to, data:=linkdata)

        For Each map As MapIndex In maps.QueryMapsByMembers(tuple)
            If map.FilterAll(tuple) Then
                Dim KO As String() = link.orthology _
                    .Where(Function(KOid) map.hasAny(KOid.name)) _
                    .Select(Function(k) k.name) _
                    .ToArray

                If g.GetElementByID(map.EntryId) Is Nothing Then
                    g.CreateNode(map.EntryId)
                End If

                For Each id As String In KO
                    If g.GetElementByID(id) Is Nothing Then
                        g.CreateNode(id)
                    End If

                    If Not g.GetEdges(g.GetElementByID(map.EntryId), g.GetElementByID(id)).Any Then
                        g.CreateEdge(map.EntryId, id)
                    End If
                Next

                If Not g.GetEdges(g.GetElementByID(map.EntryId), g.GetElementByID(transform.from)).Any Then
                    g.CreateEdge(map.EntryId, transform.from)
                End If

                If Not g.GetEdges(g.GetElementByID(map.EntryId), g.GetElementByID(transform.to)).Any Then
                    g.CreateEdge(map.EntryId, transform.to)
                End If
            End If
        Next
    End Sub
End Module
