﻿#Region "Microsoft.VisualBasic::c72eec99828ba9d8d72306dad4cb8354, models\Networks\KEGG\KEGGMap\PathwayNetwork.vb"

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

    ' Module PathwayNetwork
    ' 
    '     Function: BuildNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module PathwayNetwork

    <Extension>
    Public Function BuildNetwork(ref As IEnumerable(Of Map), nodeValue As Action(Of Node)) As NetworkGraph
        Dim graph As New NetworkGraph
        Dim maps As Map() = ref.ToArray

        For Each map As Map In maps
            Call nodeValue(graph.CreateNode(map.ID))
        Next

        For Each A As Map In maps
            Dim compoundsA = A _
                .GetMembers _
                .Where(Function(id) id.IsPattern("C\d+")) _
                .ToArray

            For Each B In maps.Where(Function(bb) Not A Is bb)
                With B.GetMembers _
                    .Where(Function(id) id.IsPattern("C\d+")) _
                    .Intersect(compoundsA) _
                    .ToArray

                    If Not .IsNullOrEmpty Then
                        Dim edge As Edge = graph.CreateEdge(A.ID, B.ID)
                        edge.Weight = .Length
                    End If
                End With
            Next
        Next

        Return graph
    End Function
End Module
