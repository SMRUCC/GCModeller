#Region "Microsoft.VisualBasic::77b50d5bdc0bfa32131e162a80fed41c, visualize\Cytoscape\Cytoscape\csv\csv.vb"

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

    '     Class Edge
    ' 
    ' 
    ' 
    '     Class Node
    ' 
    '         Properties: NodeType
    ' 
    '         Function: Dominate
    '         Operators: <, >
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Math.Correlations

Namespace Tables

    ' Cytoscape exports csv files

    Public Class Edge : Inherits FileStream.Cytoscape.Edges

    End Class

    Public Class Node : Inherits FileStream.Cytoscape.Nodes

        Public Property NodeType As String()

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
