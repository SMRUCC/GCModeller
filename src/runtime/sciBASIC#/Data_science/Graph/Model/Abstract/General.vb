﻿#Region "Microsoft.VisualBasic::0842b37aea8595c7331f1932da3e0a44, Data_science\Graph\Model\Abstract\General.vb"

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

    ' Class VertexEdge
    ' 
    '     Function: EdgeKey
    ' 
    ' Class Graph
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports TV = Microsoft.VisualBasic.Data.GraphTheory.Vertex

Public Class VertexEdge : Inherits Edge(Of TV)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function EdgeKey(U As TV, V As TV) As String
        Return $"[{U.ID}]{U.label} -> [{V.ID}]{V.label}"
    End Function
End Class

''' <summary>
''' A graph ``G = (V, E)`` consists of a set V of vertices and a set E edges, that is, unordered
''' pairs Of vertices. Unless explicitly stated otherwise, we assume that the graph Is simple,
''' that Is, it has no multiple edges And no self-loops.
''' </summary>
Public Class Graph : Inherits Graph(Of TV, VertexEdge, Graph)

End Class
