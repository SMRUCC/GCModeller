#Region "Microsoft.VisualBasic::bf025d66f69cd04b4ae2f325733aaaae, analysis\SequenceToolkit\MotifScanner\Consensus\SeedCluster.vb"

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
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' Clustering seeds using binary tree
''' </summary>
Public Module SeedCluster

    ''' <summary>
    ''' 将任意的两条序列转换为得分向量用以进行相似度的比较
    ''' </summary>
    ''' <param name="compares"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ScoreVector(compares As (q$, s$)) As (q As Vector, s As Vector)
        ' 先进行全局比对，将qs序列都变为等长序列
        Dim query As New FastaSeq With {.SequenceData = compares.q.ToUpper, .Headers = {"query"}}
        Dim subject As New FastaSeq With {.SequenceData = compares.s.ToUpper, .Headers = {"subject"}}
        Dim globalAlign As GlobalAlign(Of Char) = RunNeedlemanWunsch.RunAlign(query, subject, 0).First
        Dim q = globalAlign.query.AsEnumerable
        Dim s = globalAlign.subject.AsEnumerable
        Dim a As New List(Of Double)
        Dim b As New List(Of Double)

        For Each nt As SeqValue(Of (q As Char, s As Char)) In (q, s).SeqTuple
            With nt.value.ScoreTuple
                a.Add(.a)
                b.Add(.b)
            End With
        Next

        Return (a.AsVector, b.AsVector)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="nt">全部都是大写字母</param>
    ''' <returns></returns>
    <Extension>
    Private Function ScoreTuple(nt As (q As Char, s As Char)) As (a#, b#)
        Dim q = nt.q, s = nt.s

        If q = "-"c Then
            If s = "-"c Then
                Return (0.5, 0.5)
            Else
                Return (0.5, 0.0)
            End If
        End If

        ' q 不是任意碱基
        If s = "-"c Then
            If q = "-"c Then
                Return (0.5, 0.5)
            Else
                Return (0.0, 0.5)
            End If
        End If

        ' s 不是任意碱基
        If q = s Then
            Return (1.0, 1.0)
        Else
            Return (-1.0, -1.0)
        End If
    End Function

    ''' <summary>
    ''' 使用余弦相似度来计算出两个等长向量之间的相似度
    ''' </summary>
    ''' <param name="vector"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Compare(vector As (q As Vector, s As Vector)) As Double
        Return SSM(vector.q, vector.s)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Compare(q$, s$) As Double
        Return (q, s).ScoreVector.Compare
    End Function

    <Extension>
    Public Function BuildAVLTreeCluster(seeds As IEnumerable(Of NamedValue(Of String)), Optional cutoff# = 0.95) As BinaryTree(Of String, String)
        Dim divid# = cutoff / 2
        Dim cluster As New AVLTree(Of String, String)(
            Function(q, s)
                Dim SSM# = SeedCluster.Compare(q, s)

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
