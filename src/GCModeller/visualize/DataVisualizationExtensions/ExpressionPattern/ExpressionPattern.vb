#Region "Microsoft.VisualBasic::b91d5f4d76b461a88418c6c6205210db, visualize\DataVisualizationExtensions\ExpressionPattern\ExpressionPattern.vb"

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

'     Class ExpressionPattern
' 
'         Properties: [dim], centers, Patterns, sampleNames
' 
'         Function: (+2 Overloads) CMeansCluster, CMeansCluster3D, GetPartitionMatrix, populatePartitions, ToSummaryText
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.FuzzyCMeans
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace ExpressionPattern

    ''' <summary>
    ''' 表达模式聚类
    ''' </summary>
    Public Class ExpressionPattern

        Public Property Patterns As FuzzyCMeansEntity()
        Public Property sampleNames As String()
        Public Property [dim] As Integer()
        Public Property centers As Classify()

        Public Function ToSummaryText(Optional membershipCutoff As Double = 0.8) As String
            Dim sb As New StringBuilder
            Dim allPatterns As Integer() = Patterns _
                .Select(Function(v) v.memberships.Keys) _
                .IteratesALL _
                .Distinct _
                .OrderBy(Function(i) i) _
                .ToArray
            Dim nsize As Integer

            Call sb.AppendLine($"fuzzy cmeans partitions: [{[dim](0)}, {[dim](1)}]")
            Call sb.AppendLine($"base on {sampleNames.Length} samples(or groups):")
            Call sb.AppendLine(sampleNames.JoinBy(", "))
            Call sb.AppendLine($"clusters (should be #0 ~ #{[dim](0) * [dim](1) - 1}):")
            Call sb.AppendLine($"n members under membership cutoff {membershipCutoff}:")

            For Each clusterId As Integer In allPatterns
                nsize = Aggregate v As FuzzyCMeansEntity
                        In Patterns
                        Where v.memberships(key:=clusterId) > membershipCutoff
                        Into Count

                Call sb.AppendLine($" # {clusterId}: {nsize}")
            Next

            Return sb.ToString
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPartitionMatrix(membershipCutoff As Double, topMembers As Integer) As IEnumerable(Of Matrix())
            Return populatePartitions(
                clusters:=Patterns,
                [dim]:=[dim],
                sampleNames:=sampleNames,
                membershipCutoff:=membershipCutoff,
                topMembers:=topMembers
            )
        End Function

        Public Shared Function CMeansCluster(matrix As Matrix, nsize%, Optional fuzzification# = 2, Optional threshold# = 0.001) As Classify()
            Dim sampleNames As String() = matrix.sampleID
            Dim geneNodes As ClusterEntity() = matrix.expression _
                .AsParallel _
                .Select(Function(gene)
                            Dim vector As New List(Of Double)

                            For i As Integer = 0 To sampleNames.Length - 1
                                Call vector.Add(gene.experiments(i))
                            Next

                            Return New ClusterEntity With {
                                .uid = gene.geneID,
                                .entityVector = vector.ToArray
                            }
                        End Function) _
                .ToArray
            Dim centers As Classify() = geneNodes.CMeans(
                classCount:=nsize,
                fuzzification:=fuzzification,
                threshold:=threshold
            )

            Return centers
        End Function

        Public Shared Function CMeansCluster3D(matrix As Matrix, Optional fuzzification# = 2, Optional threshold# = 0.001) As ExpressionPattern
            Dim sampleNames As String() = matrix.sampleID
            Dim centers As Classify() = CMeansCluster(
                matrix:=matrix,
                nsize:=3,
                fuzzification:=fuzzification,
                threshold:=threshold
            )

            Return New ExpressionPattern With {
                .Patterns = centers _
                    .Select(Function(c) c.members) _
                    .IteratesALL _
                    .ToArray,
                .sampleNames = sampleNames,
                .[dim] = {1, 3},
                .centers = centers
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="dim">``[row, columns]``</param>
        ''' <returns></returns>
        ''' 
        Public Shared Function CMeansCluster(matrix As Matrix, [dim] As Integer(),
                                             Optional fuzzification# = 2,
                                             Optional threshold# = 0.001) As ExpressionPattern

            Dim nsize As Integer = [dim](Scan0) * [dim](1)
            Dim sampleNames As String() = matrix.sampleID
            Dim centers As Classify() = CMeansCluster(matrix, nsize, fuzzification, threshold)

            Return New ExpressionPattern With {
                .Patterns = centers _
                    .Select(Function(c) c.members) _
                    .IteratesALL _
                    .ToArray,
                .sampleNames = sampleNames,
                .[dim] = [dim],
                .centers = centers
            }
        End Function

        Private Shared Iterator Function populatePartitions(clusters As IEnumerable(Of FuzzyCMeansEntity),
                                                            dim%(),
                                                            sampleNames As String(),
                                                            membershipCutoff As Double,
                                                            topMembers As Integer) As IEnumerable(Of Matrix())
            Dim row As New List(Of Matrix)
            Dim cmeans As FuzzyCMeansEntity() = clusters.ToArray
            Dim allPatterns As Integer() = cmeans _
                .Select(Function(c) c.memberships.Keys) _
                .IteratesALL _
                .Distinct _
                .ToArray

            For Each patternId As Integer In allPatterns
                Dim membership = cmeans _
                    .Select(Function(v) New NamedValue(Of Double)(v.uid, v.memberships(patternId))) _
                    .ToArray
                Dim max As Double = membership.Select(Function(v) v.Value).Max
                Dim filter As Index(Of String) = membership _
                    .Where(Function(v) v.Value / max >= membershipCutoff) _
                    .Select(Function(v) v.Name) _
                    .Indexing
                Dim features As DataFrameRow()

                If filter.Count < topMembers Then
                    features = cmeans _
                        .OrderByDescending(Function(v) v.memberships(key:=patternId)) _
                        .Take(topMembers) _
                        .Select(Function(a)
                                    Return New DataFrameRow With {
                                        .geneID = a.uid,
                                        .experiments = a.entityVector
                                    }
                                End Function) _
                        .ToArray
                Else
                    features = cmeans _
                        .Where(Function(v) v.uid Like filter) _
                        .Select(Function(a)
                                    Return New DataFrameRow With {
                                        .geneID = a.uid,
                                        .experiments = a.entityVector
                                    }
                                End Function) _
                        .ToArray
                End If

                Dim matrix = New Matrix With {
                    .sampleID = sampleNames,
                    .expression = features,
                    .tag = patternId
                }

                row += matrix

                If row = [dim](1) Then
                    Yield row.PopAll
                End If
            Next

            If row > 0 Then
                Yield row.PopAll
            End If
        End Function
    End Class
End Namespace
