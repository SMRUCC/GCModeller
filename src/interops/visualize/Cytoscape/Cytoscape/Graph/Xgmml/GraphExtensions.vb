﻿#Region "Microsoft.VisualBasic::31a2a421c1c535dc2242fa987c86cf74, visualize\Cytoscape\Cytoscape\Graph\Xgmml\GraphExtensions.vb"

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

    '     Module GraphExtensions
    ' 
    '         Function: __setValue, Distinct, MergeAttributes, MergeEdges
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace CytoscapeGraphView.XGMML

    Public Module GraphExtensions

        <Extension>
        Public Function Distinct(Edges As Edge()) As Edge()
            Dim LQuery = From edge As Edge
                         In Edges
                         Select edge
                         Group edge By edge.__internalUID Into Group
            Dim buf As Edge() = LQuery.Select(Function(x) x.Group) _
                .Select(AddressOf MergeEdges) _
                .WriteAddress.ToArray
            Return buf
        End Function

        <Extension>
        Private Function MergeEdges(source As IEnumerable(Of Edge)) As Edge
            Dim edges As Edge() = source.ToArray

            If edges.Length = 1 Then
                Return edges.First
            End If

            Dim First As Edge = edges.First
            Dim attrs As Attribute() =
                LinqAPI.Exec(Of Attribute) <= edges.Select(Function(x) x.Attributes)

            First.Attributes = MergeAttributes(attrs)

            Return First
        End Function

        Private Function MergeAttributes(attrs As Attribute()) As Attribute()
            Dim LQuery = From attr As Attribute
                         In attrs
                         Select attr
                         Group attr By attr.Name Into Group
            Dim attrsBuffer = From g
                              In LQuery
                              Select g.Group.First,
                                  values = (From x As Attribute
                                            In g.Group
                                            Select x.Value
                                            Distinct).ToArray
            Dim result As Attribute() =
                attrsBuffer.Select(Function(x) x.First.__setValue(x.values))
            Return result
        End Function

        <Extension>
        Private Function __setValue(attr As Attribute, values As String()) As Attribute
            If String.Equals(attr.Type, ATTR_VALUE_TYPE_REAL) Then
                attr.Value = (From s As String In values Select Val(s)).Min
            Else
                attr.Value = String.Join("; ", values)
            End If

            Return attr
        End Function
    End Module
End Namespace
