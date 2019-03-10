﻿#Region "Microsoft.VisualBasic::3eb37b09ee6d4b43b2e3cde0ceecd2bf, Data_science\DataMining\Microsoft.VisualBasic.DataMining.Framework\FuzzyCMeans\Algorithm\FuzzyCMeansAlgorithm.vb"

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
    '         Function: FuzzyCMeans
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace FuzzyCMeans

    ''' <summary>
    ''' **Fuzzy clustering** (also referred to as **soft clustering**) is a form of clustering in which 
    ''' each data point can belong to more than one cluster.
    '''
    ''' Clustering Or cluster analysis involves assigning data points to clusters (also called buckets, 
    ''' bins, Or classes), Or homogeneous classes, such that items in the same class Or cluster are as 
    ''' similar as possible, while items belonging to different classes are as dissimilar as possible. 
    ''' Clusters are identified via similarity measures. These similarity measures include distance, 
    ''' connectivity, And intensity. Different similarity measures may be chosen based on the data Or 
    ''' the application.
    ''' 
    ''' > https://en.wikipedia.org/wiki/Fuzzy_clustering
    ''' </summary>
    ''' <remarks>
    ''' Clustering problems have applications in **biology**, medicine, psychology, economics, and many other disciplines.
    '''
    ''' ##### Bioinformatics
    ''' 
    ''' In the field of bioinformatics, clustering Is used for a number of applications. One use Is as 
    ''' a pattern recognition technique to analyze gene expression data from microarrays Or other 
    ''' technology. In this case, genes with similar expression patterns are grouped into the same cluster, 
    ''' And different clusters display distinct, well-separated patterns of expression. Use of clustering 
    ''' can provide insight into gene function And regulation. Because fuzzy clustering allows genes 
    ''' to belong to more than one cluster, it allows for the identification of genes that are conditionally 
    ''' co-regulated Or co-expressed. For example, one gene may be acted on by more than one Transcription 
    ''' factor, And one gene may encode a protein that has more than one function. Thus, fuzzy clustering 
    ''' Is more appropriate than hard clustering.
    ''' </remarks>
    Public Module FuzzyCMeansAlgorithm

        ''' <summary>
        ''' **Fuzzy clustering** (also referred to as **soft clustering**) is a form of clustering in which 
        ''' each data point can belong to more than one cluster.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="numberOfClusters%"></param>
        ''' <param name="fuzzificationParameter#">This parameter value should greater than **1.0**</param>
        ''' <param name="maxIterates%"></param>
        ''' <param name="threshold#"></param>
        ''' <param name="trace"></param>
        ''' <returns></returns>
        <Extension>
        Public Function FuzzyCMeans(data As IEnumerable(Of Entity),
                                    numberOfClusters%,
                                    Optional fuzzificationParameter# = 2,
                                    Optional maxIterates% = Short.MaxValue,
                                    Optional threshold# = 0.001,
                                    Optional ByRef trace As Dictionary(Of Integer, List(Of Entity)) = Nothing) As List(Of Entity)

            Dim coordinates As New List(Of Entity)(data)
            ' Dim random As New Random()
            '  Dim bgrColor As Byte() = New Byte(2) {}
            Dim clusterCenters As List(Of Entity) = AlgorithmsUtils.MakeInitialSeeds(coordinates, numberOfClusters)
            Dim clusters As Dictionary(Of Entity, Entity)
            Dim membershipMatrix As Dictionary(Of Entity, List(Of Double)) = Nothing
            Dim iteration As VBInteger = 0
            Dim ptClone As New Value(Of Entity)
            '  Dim clusterColors As New Dictionary(Of Entity, Color)()

            'For Each clusterCenter As Entity In clusterCenters
            '    For Each annotation As Entity In coordinates

            '        If annotation = clusterCenter Then
            '            random.NextBytes(bgrColor)

            '            MarkClusterCenter(annotation, Color.FromArgb(bgrColor(0), bgrColor(1), bgrColor(2)))
            '            clusterColors.Add(clusterCenter, annotation.Fill)

            '            Call $"Inital cluster center {annotation.uid}".__DEBUG_ECHO
            '        End If
            '    Next
            'Next

            If Not trace Is Nothing Then
                Call trace.Clear()
            End If

            While ++iteration <= maxIterates
#If DEBUG Then
                Call $"Iteration = {iteration}".__DEBUG_ECHO
#End If
                clusters = MakeFuzzyClusters(coordinates, clusterCenters, fuzzificationParameter, membershipMatrix)

                'For Each pair As KeyValuePair(Of Entity, Entity) In clusters
                '    For Each annotation As Entity In coordinates

                '        If VectorEqualityComparer.VectorEqualsToAnother(annotation.Properties, pair.Key.Properties) Then
                '            annotation.MarkClusterCenter(clusterColors(pair.Value))
                '        End If
                '    Next
                'Next

                For Each pair As KeyValuePair(Of Entity, List(Of Double)) In membershipMatrix
                    For Each annotation As Entity In coordinates

                        If VectorEqualityComparer.VectorEqualsToAnother(annotation.Properties, pair.Key.Properties) Then
                            Dim tooltip As New Dictionary(Of Integer, Double)

                            For i As Integer = 0 To pair.Value.Count - 1
                                Dim value As Double = pair.Value(i)
                                Call tooltip.Add(i, Math.Round(value, 2))
                            Next

                            annotation.Memberships = tooltip
                        End If
                    Next
                Next

                Dim oldClusterCenters As List(Of Entity) = clusterCenters

                clusterCenters = RecalculateCoordinateOfFuzzyClusterCenters(clusterCenters, membershipMatrix, fuzzificationParameter)
                If Not trace Is Nothing Then
                    Call trace.Add(iteration, clusterCenters)
                End If

                Dim distancesToClusterCenters As Dictionary(Of Entity, List(Of Double)) = coordinates.DistanceToClusterCenters(clusterCenters)
                Dim newMembershipMatrix As Dictionary(Of Entity, List(Of Double)) = CreateMembershipMatrix(distancesToClusterCenters, fuzzificationParameter)

                Dim differences As List(Of List(Of Double)) = newMembershipMatrix.Values.DifferenceMatrix(membershipMatrix.Values.AsList())
                Dim maxElement As Double = GetMaxElement(differences)

                Call $"Max element: {maxElement}".__DEBUG_ECHO

                If maxElement <= threshold Then
                    Exit While
                End If

                'Dim colorValues As New List(Of Color)(clusterColors.Values)

                'Call clusterColors.Clear()

                'For i As Integer = 0 To clusterCenters.Count - 1
                '    clusterColors.Add(clusterCenters(i), colorValues(i))
                'Next

                For Each oldClusterCenter As Entity In oldClusterCenters
                    Dim isClusterCenterDataPoint As Boolean = False

                    For Each coordinate As Entity In coordinates

                        If VectorEqualityComparer.VectorEqualsToAnother(oldClusterCenter.Properties, coordinate.Properties) Then
#If DEBUG Then
                            Call $"ex-center {oldClusterCenter.uid}".__DEBUG_ECHO
#End If
                            isClusterCenterDataPoint = True
                            Exit For
                        End If
                    Next

                    If Not isClusterCenterDataPoint Then
                        For Each annotation As Entity In coordinates

                            If VectorEqualityComparer.VectorEqualsToAnother(annotation.Properties, oldClusterCenter.Properties) Then
#If DEBUG Then
                                Call $"remove center with coordinate {annotation.uid}".__DEBUG_ECHO
#End If
                                coordinates.Remove(annotation)
                                Exit For
                            End If
                        Next
                    End If
                Next

                For i As Integer = 0 To clusterCenters.Count - 1
                    Dim clusterCenter As Entity = clusterCenters(i)
                    Dim isExists As Boolean = False

                    For Each annotation As Entity In coordinates

                        If VectorEqualityComparer.VectorEqualsToAnother(annotation.Properties, clusterCenter.Properties) Then
                            '  MarkClusterCenter(annotation, colorValues(i))
                            isExists = True
                            Exit For
                        End If
                    Next

                    If Not isExists Then

                        '  Call MarkClusterCenter(ptClone, colorValues(i))
                        coordinates += ptClone = New Entity With {
                            .Properties = clusterCenter.Properties.Clone,
                            .uid = clusterCenter.uid
                        }
#If DEBUG Then
                        Call $"add center with coordinate {(+ptClone).uid}".__DEBUG_ECHO
#End If
                    End If
                Next
            End While

            Return clusterCenters
        End Function
    End Module
End Namespace
