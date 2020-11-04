﻿#Region "Microsoft.VisualBasic::7f1e040a893ef7f806a5fd7f2ad2b51d, Data_science\DataMining\DataMining\Clustering\FuzzyCMeans\CMeans.vb"

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

    '     Module CMeans
    ' 
    '         Function: (+3 Overloads) CMeans, Dist, GetCenters, GetRandomMatrix, J
    '                   PopulateClusters, scanRow
    ' 
    '         Sub: updateMembership, updateMembershipParallel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.FuzzyCMeans
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports stdNum = System.Math

Namespace FuzzyCMeans

    ''' <summary>
    ''' ### the cmeans algorithm module
    ''' 
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
    Public Module CMeans

        ''' <summary>
        ''' **Fuzzy clustering** (also referred to as **soft clustering**) is a form of clustering in which 
        ''' each data point can belong to more than one cluster.
        ''' </summary>
        ''' 
        <Extension>
        Public Function CMeans(entities As IEnumerable(Of ClusterEntity), classCount As Integer) As Classify()
            Return CMeans(entities.ToArray, classCount, 2, 0.001)
        End Function

        ''' <summary>
        ''' **Fuzzy clustering** (also referred to as **soft clustering**) is a form of clustering in which 
        ''' each data point can belong to more than one cluster.
        ''' </summary>
        ''' 
        <Extension>
        Public Function CMeans(entities As IEnumerable(Of ClusterEntity), classCount As Integer, m As Double) As Classify()
            Return CMeans(entities.ToArray, classCount, m, 0.001)
        End Function

        Private Iterator Function GetRandomMatrix(classCount As Integer, nsamples As Integer) As IEnumerable(Of Double())
            For Each x As Double In Enumerable.Range(0, nsamples)
                Dim c = New Double(classCount - 1) {}

                For i = 0 To c.Length - 1
                    c(i) = randf.randf(0, 1 - c.Sum())

                    If i = c.Length - 1 Then
                        c(i) = 1 - c.Sum()
                    End If
                Next

                Yield c
            Next
        End Function

        ''' <summary>
        ''' **Fuzzy clustering** (also referred to as **soft clustering**) is a form of clustering in which 
        ''' each data point can belong to more than one cluster.
        ''' </summary>
        ''' 
        <Extension>
        Public Function CMeans(entities As ClusterEntity(),
                               classCount As Integer,
                               fuzzification As Double,
                               threshold As Double,
                               Optional parallel As Boolean = True,
                               Optional maxLoop As Integer = 10000) As Classify()

            Dim u As Double()() = GetRandomMatrix(classCount, nsamples:=entities.Length).ToArray()
            Dim j_old As Double = -1
            Dim centers As Double()() = Nothing
            Dim width As Integer = entities(0).Length
            Dim j_new As Double
            Dim membership_diff As Double
            Dim [loop] As i32 = Scan0

            While True
                centers = GetCenters(classCount, fuzzification, u, entities, width).ToArray
                j_new = J(fuzzification, u, centers, entities)
                membership_diff = stdNum.Abs(j_new - j_old)

                If j_old <> -1 AndAlso membership_diff < threshold Then
                    Exit While
                Else
                    Call $"loop_{[loop]} membership_diff: |{j_new} - {j_old}| = {membership_diff}".__DEBUG_ECHO
                End If

                j_old = j_new

                If parallel Then
                    Call u.updateMembershipParallel(entities, centers, classCount, fuzzification)
                Else
                    Call u.updateMembership(entities, centers, classCount, fuzzification)
                End If

                If ++[loop] > maxLoop Then
                    Exit While
                End If
            End While

            Return entities.PopulateClusters(classCount, u, centers)
        End Function

        <Extension>
        Private Sub updateMembershipParallel(ByRef u As Double()(), entities As ClusterEntity(), centers As Double()(), classCount As Integer, m As Double)
            u = Enumerable.Range(0, u.Length) _
                .Select(Function(i) (i, Val:=entities(i))) _
                .AsParallel _
                .Select(Function(obj)
                            Dim result As Double() = centers.scanRow(
                                entity:=obj.val,
                                classCount:=classCount,
                                m:=m
                            )
                            Dim pack = (obj.i, result)

                            Return pack
                        End Function) _
                .OrderBy(Function(pack) pack.i) _
                .Select(Function(a) a.result) _
                .ToArray
        End Sub

        <Extension>
        Private Function scanRow(centers As Double()(), entity As ClusterEntity, classCount As Integer, m As Double) As Double()
            Dim ui As Double() = New Double(classCount - 1) {}

            For j As Integer = 0 To classCount - 1
                Dim jIndex As Integer = j
                Dim sumAll As Double = Aggregate x As Integer
                                       In Enumerable.Range(0, classCount)
                                       Let a As Double = stdNum.Sqrt(Dist(entity, centers(jIndex))) / stdNum.Sqrt(Dist(entity, centers(x)))
                                       Let val As Double = a ^ (2 / (m - 1))
                                       Into Sum(val)
                ui(j) = 1 / sumAll

                If Double.IsNaN(ui(j)) Then
                    ui(j) = 1
                End If
            Next

            Return ui
        End Function

        <Extension>
        Private Sub updateMembership(u As Double()(), entities As ClusterEntity(), centers As Double()(), classCount As Integer, m As Double)
            Dim classIndex As Integer() = Enumerable.Range(0, classCount).ToArray

            For i As Integer = 0 To u.Length - 1
                Dim index As Integer = i

                For j As Integer = 0 To u(i).Length - 1
                    Dim jIndex As Integer = j
                    Dim sumAll As Double = Aggregate x As Integer
                                           In classIndex
                                           Let d1 As Double = stdNum.Sqrt(Dist(entities(index), centers(jIndex)))
                                           Let d2 As Double = stdNum.Sqrt(Dist(entities(index), centers(x)))
                                           Let a As Double = d1 / d2
                                           Let val As Double = a ^ (2 / (m - 1))
                                           Into Sum(val)
                    u(i)(j) = 1 / sumAll

                    If Double.IsNaN(u(i)(j)) Then
                        u(i)(j) = 1
                    End If
                Next
            Next
        End Sub

        <Extension>
        Private Function PopulateClusters(values As ClusterEntity(), classCount As Integer, u As Double()(), centers As Double()()) As Classify()
            Dim index As Integer
            Dim maxMembership As Double
            Dim clusterMembership As Double()
            Dim resultEntity As FuzzyCMeansEntity
            Dim result As Classify() = Enumerable.Range(0, classCount) _
                .[Select](Function(i)
                              Return New Classify() With {
                                    .Id = i + 1,
                                    .center = centers(i)
                                }
                          End Function) _
                .ToArray

            For i As Integer = 0 To values.Length - 1
                clusterMembership = u(i)
                maxMembership = clusterMembership.Max()
                index = Array.IndexOf(u(i), maxMembership)
                resultEntity = New FuzzyCMeansEntity With {
                    .cluster = index,
                    .entityVector = values(i).entityVector,
                    .uid = values(i).uid,
                    .memberships = result _
                        .ToDictionary(Function(a) a.Id - 1,
                                      Function(a)
                                          Return clusterMembership(a.Id - 1)
                                      End Function)
                }

                result(index).members.Add(resultEntity)
            Next

            Return result
        End Function

        Public Iterator Function GetCenters(classCount As Integer, m As Double, u As Double()(), entities As ClusterEntity(), width As Integer) As IEnumerable(Of Double())
            Dim entityIndex As Integer() = Enumerable.Range(0, entities.Count).ToArray

            For Each i As Integer In Enumerable.Range(0, classCount)
                Yield Enumerable.Range(0, width) _
                    .[Select](Function(x)
                                  Dim sumAll = Aggregate j As Integer In entityIndex Let val As Double = (u(j)(i) ^ m) * entities(j)(x) Into Sum(val)
                                  Dim bValue = Aggregate j As Integer In entityIndex Let val As Double = u(j)(i) ^ m Into Sum(val)

                                  Return sumAll / bValue
                              End Function) _
                    .ToArray()
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function J(m As Double, u As Double()(), centers As Double()(), entities As ClusterEntity()) As Double
            Return centers _
                .Select(Function(x, i)
                            Return entities _
                                .Select(Function(y, j1)
                                            Return (u(j1)(i) ^ m) * Dist(y, x)
                                        End Function) _
                                .Sum()
                        End Function) _
                .Sum()
        End Function

        ''' <summary>
        ''' 在这里面只会计算结果值，并不会修改数据
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="center"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function Dist(obj As ClusterEntity, center As Double()) As Double
            Return obj.entityVector.Select(Function(x, i) (x - center(i)) ^ 2).Sum()
        End Function
    End Module
End Namespace
