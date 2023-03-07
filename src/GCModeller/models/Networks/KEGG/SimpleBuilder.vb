#Region "Microsoft.VisualBasic::6760f72e8961361015d0db7e48d377d0, GCModeller\models\Networks\KEGG\SimpleBuilder.vb"

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

    '   Total Lines: 66
    '    Code Lines: 53
    ' Comment Lines: 0
    '   Blank Lines: 13
    '     File Size: 3.18 KB


    ' Module SimpleBuilder
    ' 
    '     Function: GraphQueryByCompoundList
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data

Public Module SimpleBuilder

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
            For Each transform In link.reactantPairs
                If transform.from Like compoundIndex AndAlso transform.to Like compoundIndex Then
                    If Not g.GetEdges(g.GetElementByID(transform.from), g.GetElementByID(transform.to)).Any Then
                        Dim tuple As String() = {transform.from, transform.to}

                        g.CreateEdge(transform.from, transform.to, data:=New EdgeData With {.label = link.definition})

                        For Each map As MapIndex In maps.QueryMapsByMembers(tuple)
                            If map.FilterAll(tuple) Then
                                Dim KO = link.orthology.Where(Function(KOid) map.hasAny(KOid.name)).Select(Function(k) k.name).ToArray

                                If g.GetElementByID(map.id) Is Nothing Then
                                    g.CreateNode(map.id)
                                End If

                                For Each id As String In KO
                                    If g.GetElementByID(id) Is Nothing Then
                                        g.CreateNode(id)
                                    End If

                                    If Not g.GetEdges(g.GetElementByID(map.id), g.GetElementByID(id)).Any Then
                                        g.CreateEdge(map.id, id)
                                    End If
                                Next

                                If Not g.GetEdges(g.GetElementByID(map.id), g.GetElementByID(transform.from)).Any Then
                                    g.CreateEdge(map.id, transform.from)
                                End If

                                If Not g.GetEdges(g.GetElementByID(map.id), g.GetElementByID(transform.to)).Any Then
                                    g.CreateEdge(map.id, transform.to)
                                End If
                            End If
                        Next
                    End If
                End If
            Next
        Next
    End Sub
End Module
