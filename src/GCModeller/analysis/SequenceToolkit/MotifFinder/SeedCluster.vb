#Region "Microsoft.VisualBasic::ff6cdfd422fab5e0e9a3f72e865ad463, GCModeller\analysis\SequenceToolkit\MotifFinder\SeedCluster.vb"

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

    '   Total Lines: 172
    '    Code Lines: 122
    ' Comment Lines: 27
    '   Blank Lines: 23
    '     File Size: 5.91 KB


    ' Module SeedCluster
    ' 
    '     Function: BuildAVLTreeCluster, BuildKmeansCluster, (+2 Overloads) Compare, matrixRow, ScoreTuple
    '               ScoreVector
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Model.MotifGraph
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' Clustering seeds using binary tree
''' </summary>
Public Module SeedCluster

    Private Class VectorCompares

        ReadOnly cache As New Dictionary(Of String, Vector)

        Public Function Compare(q$, s$) As Double
            Dim g1 = GetVector(q)
            Dim g2 = GetVector(s)
            Dim score As Double = SSM(g1, g2)

            Return score
        End Function

        Private Function GetVector(s As String) As Vector
            If Not cache.ContainsKey(s) Then
                cache.Add(s, Builder.SequenceGraph(s, SequenceModel.NT).GetVector(SequenceModel.NT))
            End If

            Return cache(s)
        End Function
    End Class

    <Extension>
    Public Function BuildAVLTreeCluster(seeds As IEnumerable(Of NamedValue(Of String)),
                                        Optional cutoff# = 0.95) As BinaryTree(Of String, String)
        Dim divid# = cutoff * (2 / 3)
        Dim align As New VectorCompares
        Dim cluster As New AVLTree(Of String, String)(
            Function(q, s)
                Dim SSM# = align.Compare(q, s)

                If SSM >= cutoff Then
                    Return 0
                ElseIf SSM >= divid Then
                    Return 1
                Else
                    Return -1
                End If
            End Function, Function(s) s)

        For Each seed As NamedValue(Of String) In seeds
            Call cluster.Add(seed.Value, seed.Name, valueReplace:=False)
        Next

        Return cluster.root
    End Function

    <Extension>
    Private Function matrixRow(q As HSP, i%, seeds As IGrouping(Of String, SeqValue(Of HSP))()) As NamedCollection(Of Double)
        Dim row As New NamedCollection(Of Double) With {
            .name = i,
            .value = New Double(seeds.Length - 1) {}
        }
        Dim j As i32 = 1
        Dim query As New FastaSeq With {
            .SequenceData = q.Query
        }

        For Each s As IGrouping(Of String, SeqValue(Of HSP)) In seeds
            ' 因为在这里需要构建一个矩阵，所以自己比对自己这个情况也需要放进去了
            Dim score# = 0
            Dim subject As New FastaSeq With {
                .SequenceData = s.Key
            }

            RunNeedlemanWunsch.RunAlign(query, subject, score)

            row(++j) = score
        Next

        Return row
    End Function

    <Extension>
    Public Function BuildKmeansCluster(seeds As IEnumerable(Of HSP), clusterN%) As IEnumerable(Of EntityClusterModel)
        ' 先做一次group操作，这样子可以将重复的序列减少
        ' 做出矩阵之后直接复制就可
        Dim queryGroup = seeds _
            .SeqIterator _
            .GroupBy(Function(q) q.value.Query) _
            .ToArray
        Dim matrix As NamedCollection(Of Double)() = queryGroup _
            .SeqIterator _
            .AsParallel _
            .Select(Function(seed)
                        Dim q As HSP = seed.value.First.value
                        ' 在这里的i是针对前面的queryGroup的
                        Dim row = q.matrixRow(seed.i, queryGroup)
                        Return row
                    End Function) _
            .ToArray

        Call "Kmeans...".__DEBUG_ECHO

        ' 进行聚类分簇
        Dim clusters = matrix _
            .ToKMeansModels _
            .Kmeans(expected:=clusterN, debug:=True)

        Return clusters
    End Function
End Module
