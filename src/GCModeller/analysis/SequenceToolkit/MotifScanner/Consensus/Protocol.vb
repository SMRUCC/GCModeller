#Region "Microsoft.VisualBasic::8a2cafb339e296c6e2f192f9eb8319ab, analysis\SequenceToolkit\MotifScanner\Consensus\Protocol.vb"

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

' Module Protocol
' 
'     Function: Consensus, pairwiseSeeding, PopulateMotifs, PWM
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Text.Levenshtein
Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Public Class Parameter

    Public Property minW As Integer
    Public Property seedingCutoff As Double
    Public Property ScanMinW As Integer
    Public Property ScanCutoff As Double

    Public Shared Function DefaultParameter() As DefaultValue(Of Parameter)
        Return New Parameter With {
            .minW = 10,
            .seedingCutoff = 0.85,
            .ScanCutoff = 0.6,
            .ScanMinW = 6
        }
    End Function

End Class

Public Module Protocol

    <Extension>
    Private Function seeding(regions As IEnumerable(Of FastaSeq), q As FastaSeq, param As Parameter) As IEnumerable(Of HSP)
        Dim seeds As New List(Of HSP)

        For Each s As FastaSeq In regions.Where(Function(seq) Not seq Is q)
            seeds += pairwiseSeeding(q, s, param)
        Next

        Return seeds
    End Function

    <Extension>
    Private Function matrixRow(q As HSP, i%, seeds As IEnumerable(Of HSP)) As DataSet
        Dim row As New DataSet With {
                .ID = i,
                .Properties = New Dictionary(Of String, Double)
            }
        Dim j As int = 1

        For Each s As HSP In seeds
            ' 因为在这里需要构建一个矩阵，所以自己比对自己这个情况也需要放进去了
            Dim score = RunNeedlemanWunsch.RunAlign(
                New FastaSeq With {.SequenceData = q.Query},
                New FastaSeq With {.SequenceData = s.Query},
                [single]:=True,
                echo:=False
            )

            row(++j) = score
        Next

        Return row
    End Function

    <Extension>
    Public Iterator Function PopulateMotifs(inputs As IEnumerable(Of FastaSeq), Optional expectedMotifs% = 10, Optional param As Parameter = Nothing) As IEnumerable(Of Probability)
        Dim regions As FastaSeq() = inputs.ToArray

        param = param Or Parameter.DefaultParameter

        ' 先进行两两局部最优比对，得到最基本的种子
        Dim seeds As List(Of HSP) = regions _
            .AsParallel _
            .Select(Function(q) regions.seeding(q, param)) _
            .IteratesALL _
            .AsList

        ' 之后对得到的种子序列进行两两全局比对，得到距离矩阵
        Dim i As int = 1
        Dim repSeq As New Dictionary(Of String, String)

        For Each q As HSP In seeds
            repSeq(CStr(++i)) = q.Consensus
        Next

        Dim matrix As DataSet() = seeds _
            .SeqIterator _
            .AsParallel _
            .Select(Function(seed)
                        Dim q = seed.value
                        Dim row = q.matrixRow(seed.i, seeds)
                        Return row
                    End Function) _
            .ToArray

        ' 进行聚类分簇
        Dim clusters = matrix _
            .ToKMeansModels _
            .Kmeans(expected:=expectedMotifs, debug:=False)
        Dim motifs = clusters _
            .GroupBy(Function(c) c.Cluster) _
            .ToArray

        ' 对聚类簇进行多重序列比对得到概率矩阵
        For Each group As IGrouping(Of String, EntityClusterModel) In motifs
            Dim MSA = group _
                .Select(Function(seq)
                            Return New FastaSeq With {
                                .SequenceData = repSeq(seq.ID),
                                .Headers = {seq.ID}
                            }
                        End Function) _
                .MultipleAlignment(Nothing)
            Dim motif As Probability = MSA.MSA.PWM(members:=regions, param:=param)

            If motif.score > 0 Then
                Yield motif
            End If
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="MSA$">经过了多重序列比对之后，所有的成员的长度都已经是一致的了</param>
    ''' <param name="members"></param>
    ''' <returns></returns>
    <Extension>
    Private Function PWM(MSA$(), members As FastaSeq(), param As Parameter) As Probability
        Dim residues As New List(Of Probability.Residue)
        Dim nt = {"A"c, "T"c, "G"c, "C"c}

        For i As Integer = 0 To MSA(Scan0).Length - 1
            Dim index% = i
            Dim P = MSA _
                .Select(Function(seq) seq(index)) _
                .GroupBy(Function(c) c) _
                .ToDictionary(Function(c) c.Key,
                              Function(g) g.Count / MSA.Length)
            Dim Pi = nt.ToDictionary(
                Function(base) base,
                Function(base) P.TryGetValue(base))

            residues += New Probability.Residue With {
                .frequency = Pi
            }
        Next

        ' pvalue / scores
        ' 在这里score是这个motif的多重比对的结果的PWM矩阵对原始序列的扫描结果的最高得分值
        Dim scores As Vector = members _
            .Select(Function(fa)
                        Dim best As SimpleSegment = residues _
                            .ScanSites(fa, param.ScanCutoff, param.ScanMinW) _
                            .FirstOrDefault

                        If best Is Nothing Then
                            Return 0
                        Else
                            Return best.ID.ParseDouble
                        End If
                    End Function) _
            .AsVector
        Dim pvalue# = t.Test(scores, Vector.Zero(Dim:=scores.Length), Hypothesis.TwoSided).Pvalue

        Return New Probability With {
            .region = residues,
            .pvalue = pvalue,
            .score = scores.Sum
        }
    End Function

    Public Function pairwiseSeeding(q As FastaSeq, s As FastaSeq, param As Parameter) As IEnumerable(Of HSP)
        Dim smithWaterman As New SmithWaterman(q.SequenceData, s.SequenceData)
        Dim result = smithWaterman.GetOutput(param.seedingCutoff, param.minW)
        Return result.HSP
    End Function

    <Extension>
    Public Function Consensus(pairwise As HSP) As String
        Dim globalAlign = LevenshteinDistance.ComputeDistance(pairwise.Query, pairwise.Subject)
        Return globalAlign.Matches
    End Function
End Module

