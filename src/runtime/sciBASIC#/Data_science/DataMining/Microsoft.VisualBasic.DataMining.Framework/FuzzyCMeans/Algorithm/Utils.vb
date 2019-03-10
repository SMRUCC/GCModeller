﻿#Region "Microsoft.VisualBasic::4882028e9fbed4159b86996552ea4c8d, Data_science\DataMining\Microsoft.VisualBasic.DataMining.Framework\FuzzyCMeans\Algorithm\Utils.vb"

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

    '     Module FuzzyCMeansAlgorithm
    ' 
    '         Function: CreateMembershipMatrix, Fill, MakeFuzzyClusters, RecalculateCoordinateOfFuzzyClusterCenters
    ' 
    '         Sub: MarkClusterCenter
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace FuzzyCMeans

    Partial Public Module FuzzyCMeansAlgorithm

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="clusterCenterPoint"></param>
        ''' <param name="color"></param>
        ''' 
        <Extension>
        Public Sub MarkClusterCenter(clusterCenterPoint As Entity, color As Color)
            clusterCenterPoint.ReadProperty(Of Color)(NameOf(MarkClusterCenter)).value = color
        End Sub

        <Extension>
        Public Function Fill(x As Entity) As Color
            Return x.ReadProperty(Of Color)(NameOf(MarkClusterCenter))
        End Function

        Private Function MakeFuzzyClusters(points As List(Of Entity),
                                   clusterCenters As List(Of Entity),
                           fuzzificationParameter As Double,
                           ByRef membershipMatrix As Dictionary(Of Entity, List(Of Double))) As Dictionary(Of Entity, Entity)

            Dim distancesToClusterCenters As Dictionary(Of Entity, List(Of Double)) =
                points.DistanceToClusterCenters(clusterCenters)
            Dim clusters As New Dictionary(Of Entity, Entity)()

            membershipMatrix = CreateMembershipMatrix(distancesToClusterCenters, fuzzificationParameter)

            For Each value As KeyValuePair(Of Entity, List(Of Double)) In membershipMatrix
                Dim clusterNumber As Integer = Which.Max(value.Value)
                clusters.Add(value.Key, clusterCenters(clusterNumber))
            Next

            Return clusters
        End Function

        Private Function CreateMembershipMatrix(distancesToClusterCenters As Dictionary(Of Entity, List(Of Double)),
                                                   fuzzificationParameter As Double) As Dictionary(Of Entity, List(Of Double))

            Dim map As New Dictionary(Of Entity, List(Of Double))()

            For Each pair As KeyValuePair(Of Entity, List(Of Double)) In distancesToClusterCenters
                Dim unNormaizedMembershipValues As New List(Of Double)()
                Dim sum As Double = 0

                For i As Integer = 0 To pair.Value.Count - 1
                    Dim distance As Double = pair.Value(i)
                    If distance = 0 Then
                        distance = 0.0000001
                    End If

                    Dim membershipValue As Double = Math.Pow(1 / distance, (1 / (fuzzificationParameter - 1)))
                    sum += membershipValue
                    unNormaizedMembershipValues.Add(membershipValue)
                Next

                Dim membershipValues As New List(Of Double)()
                For Each membershipValue As Double In unNormaizedMembershipValues
                    membershipValues.Add((membershipValue / sum))
                Next
                map.Add(pair.Key, membershipValues)
            Next

            Return map
        End Function

        Private Function RecalculateCoordinateOfFuzzyClusterCenters(clusterCenters As List(Of Entity),
                                                                  membershipMatrix As Dictionary(Of Entity, List(Of Double)),
                                                            fuzzificationParameter As Double) As List(Of Entity)

            Dim clusterMembershipValuesSums As New List(Of Double)()
            For Each clusterCenter As Entity In clusterCenters
                clusterMembershipValuesSums.Add(0)
            Next

            Dim weightedPointCoordinateSums As New List(Of List(Of Double))()

            For i As Integer = 0 To clusterCenters.Count - 1
                Dim clusterCoordinatesSum As New List(Of Double)()
                For Each coordinate As Double In clusterCenters(0).Properties
                    clusterCoordinatesSum.Add(0)
                Next

                For Each pair As KeyValuePair(Of Entity, List(Of Double)) In membershipMatrix
                    Dim pointCoordinates As Entity = pair.Key
                    Dim membershipValues As List(Of Double) = pair.Value

                    clusterMembershipValuesSums(i) += Math.Pow(membershipValues(i), fuzzificationParameter)

                    For j As Integer = 0 To pointCoordinates.Length - 1
                        clusterCoordinatesSum(j) += pointCoordinates(j) * Math.Pow(membershipValues(i), fuzzificationParameter)
                    Next
                Next

                weightedPointCoordinateSums.Add(clusterCoordinatesSum)
            Next

            Dim newClusterCenters As New List(Of Entity)()
            For i As Integer = 0 To clusterCenters.Count - 1
                Dim coordinatesSums As List(Of Double) = weightedPointCoordinateSums(i)
                Dim newCoordinates As New Entity() With {
                    .Properties = New Double(coordinatesSums.Count - 1) {},
                    .uid = clusterCenters(i).uid
                }

                For j As Integer = 0 To coordinatesSums.Count - 1
                    newCoordinates(j) = coordinatesSums(j) / clusterMembershipValuesSums(i)
                Next

                newClusterCenters.Add(newCoordinates)
            Next

            Return newClusterCenters
        End Function
    End Module
End Namespace
