#Region "Microsoft.VisualBasic::a6922b2804d5692afa558fa78854d980, ..\interops\visualize\Cytoscape\Cytoscape\Tabular\NodeTable.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Mathematical

Namespace Tables

    Public Class Node : Implements sIdEnumerable

        Public Property SUID As Integer
        Public Property AverageShortestPathLength As Double
        Public Property BetweennessCentrality As Double
        Public Property ClosenessCentrality As Double
        Public Property ClusteringCoefficient As Double
        Public Property Degree As Integer
        <Column("degree.layout")> Public Property DegreeLayout As Integer
        Public Property Eccentricity As Integer
        Public Property IsSingleNode As Boolean
        Public Property name As String
        Public Property NeighborhoodConnectivity As Double
        <Collection("NodeType", ";")> Public Property NodeType As String()
        Public Property NumberOfDirectedEdges As Integer
        Public Property NumberOfUndirectedEdges As Integer
        Public Property PartnerOfMultiEdgedNodePairs As Integer
        Public Property Radiality As Double
        Public Property SelfLoops As Integer
        <Column("shared name")> Public Property SharedName As String Implements sIdEnumerable.Identifier
        Public Property Stress As Integer
        Public Property TopologicalCoefficient As Double

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", SUID, SharedName)
        End Function

        Public Function Dominate(b As Node) As Double
            Dim ScoreList As List(Of Double) = New List(Of Double)

            Call ScoreList.Add(If(Degree > b.Degree, 1, 2))
            Call ScoreList.Add(If(BetweennessCentrality > b.BetweennessCentrality, 1, 2))
            Call ScoreList.Add(If(ClosenessCentrality > b.ClosenessCentrality, 1, 2))
            Call ScoreList.Add(If(NeighborhoodConnectivity > b.NeighborhoodConnectivity, 1, 2))

            If Array.IndexOf(NodeType, "Regulator") > -1 Then

                If Array.IndexOf(b.NodeType, "Regulator") > -1 Then
                    Call ScoreList.Add(1.5)
                Else
                    Call ScoreList.Add(1)
                End If

            Else

                If Array.IndexOf(b.NodeType, "Regulator") > -1 Then
                    Call ScoreList.Add(2)
                Else
                    Call ScoreList.Add(1)
                End If

            End If

            Dim value As Double = ScoreList.EuclideanDistance
            Return value
        End Function

        Public Overloads Shared Operator <(a As Node, b As Node) As Boolean
            Dim sa As Double = a.Dominate(b)
            Dim sb As Double = b.Dominate(a)

            Return sa < sb
        End Operator

        Public Overloads Shared Operator >(a As Node, b As Node) As Boolean
            Dim sa As Double = a.Dominate(b)
            Dim sb As Double = b.Dominate(a)

            Return sa > sb
        End Operator
    End Class
End Namespace
