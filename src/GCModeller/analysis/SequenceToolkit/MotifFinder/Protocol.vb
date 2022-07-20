#Region "Microsoft.VisualBasic::c7b5363a9e73b650c22e30c130f5d681, analysis\SequenceToolkit\MotifScanner\Consensus\Protocol.vb"

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
'     Function: (+2 Overloads) Consensus, pairwiseSeeding, PopulateMotifs, PWM, seeding
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Protocol

    Private Class TaskPayload

        Public q As FastaSeq
        Public regions As FastaSeq()
        Public param As PopulatorParameter

        Sub New(q As FastaSeq, regions As IEnumerable(Of FastaSeq), param As PopulatorParameter)
            Me.q = New FastaSeq With {.Headers = q.Headers, .SequenceData = q.SequenceData}
            Me.regions = regions _
                .Select(Function(qi) New FastaSeq(fa:=qi)) _
                .ToArray
            Me.param = New PopulatorParameter(param)
        End Sub

        Public Function Seeding() As HSP()
            Return regions.seeding(q, param).ToArray
        End Function

    End Class

    ''' <summary>
    ''' create seeds via pairwise alignment, use 
    ''' the smith-waterman HSP as motif seeds.
    ''' </summary>
    ''' <param name="regions"></param>
    ''' <param name="q"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    <Extension>
    Private Function seeding(regions As IEnumerable(Of FastaSeq), q As FastaSeq, param As PopulatorParameter) As IEnumerable(Of HSP)
        Dim seeds As New List(Of HSP)

        For Each s As FastaSeq In regions.Where(Function(seq) Not seq Is q)
            seeds += pairwiseSeeding(q, s, param)
        Next

        Return seeds
    End Function

    <Extension>
    Public Iterator Function PopulateMotifs(inputs As IEnumerable(Of FastaSeq),
                                            Optional leastN% = 5,
                                            Optional cleanMotif As Double = 0.5,
                                            Optional param As PopulatorParameter = Nothing) As IEnumerable(Of SequenceMotif)

        Dim regions As FastaSeq() = inputs.ToArray

        param = param Or PopulatorParameter.DefaultParameter

        Call param.log()("create parallel task payload...")

        Dim payloads As TaskPayload()() = regions _
            .Select(Function(q) New TaskPayload(q, regions, param)) _
            .Split(partitionSize:=regions.Length / (App.CPUCoreNumbers * 2)) _
            .ToArray

        Call param.log()($"run task on {payloads.Length} parallel process!")
        Call param.log()($"there are {Aggregate x In payloads Into Average(x.Length)} task in each parallel process.")
        Call param.log()("seeding...")

        ' 先进行两两局部最优比对，得到最基本的种子
        ' 2018-3-2 在这里应该选取的是短的高相似度的序列
        Dim seeds As List(Of HSP) = payloads _
            .AsParallel _
            .Select(Function(q)
                        Return q.Select(Function(qi) qi.Seeding).IteratesALL.ToArray
                    End Function) _
            .ToArray _
            .IteratesALL _
            .AsList

        Call param.log()($"create {seeds.Count} seeds...")
        Call param.log()("create motif cluster tree!")

        ' 构建出二叉树
        ' 每一个node都是一个cluster
        ' 可以按照成员的数量至少要满足多少条来取cluster的结果
        Dim tree = seeds _
            .Select(Function(q) New NamedValue(Of String)(q.Query, q.Query)) _
            .BuildAVLTreeCluster(param.seedingCutoff)

        Call param.log()("populate motifs...")

        ' 对聚类簇进行多重序列比对得到概率矩阵
        For Each motif As SequenceMotif In tree _
            .PopulateNodes _
            .Where(Function(group) group.MemberSize >= leastN) _
            .AsParallel _
            .Select(Function(group)
                        Return group.motif(regions, param)
                    End Function)

            motif = motif.Cleanup(cutoff:=cleanMotif)

            If motif.score > 0 Then
                Yield motif
            End If
        Next
    End Function

    <Extension>
    Private Function motif(group As BinaryTree(Of String, String), regions As FastaSeq(), param As PopulatorParameter) As SequenceMotif
        Dim members As List(Of String) = group!values
        Dim MSA As MSAOutput = members _
                .Select(Function(seq)
                            Return New FastaSeq With {
                                .SequenceData = seq,
                                .Headers = {""}
                            }
                        End Function) _
                .MultipleAlignment(Nothing)
        Dim PWM As SequenceMotif = MSA.PWM(members:=regions, param:=param)

        Return PWM
    End Function

    ''' <summary>
    ''' create PWM matrix of a motif model
    ''' </summary>
    ''' <param name="alignment">经过了多重序列比对之后，所有的成员的长度都已经是一致的了</param>
    ''' <param name="members"></param>
    ''' <returns></returns>
    <Extension>
    Private Function PWM(alignment As MSAOutput, members As FastaSeq(), param As PopulatorParameter) As SequenceMotif
        Dim residues As New List(Of Residue)
        Dim nt = {"A"c, "T"c, "G"c, "C"c}
        Dim MSA As String() = alignment.MSA

        For i As Integer = 0 To MSA(Scan0).Length - 1
            Dim index% = i
            Dim P = MSA _
                .Select(Function(seq) seq(index)) _
                .GroupBy(Function(c) c) _
                .ToDictionary(Function(c) c.Key,
                              Function(g)
                                  Return g.Count / MSA.Length
                              End Function)
            Dim Pi = nt.ToDictionary(
                Function(base) base,
                Function(base)
                    Return P.TryGetValue(base)
                End Function)

            residues += New Residue With {
                .frequency = Pi,
                .index = i
            }
        Next

        ' pvalue / scores
        ' 在这里score是这个motif的多重比对的结果的PWM矩阵对原始序列的扫描结果的最高得分值
        Dim scores As Vector = members _
            .Select(Function(fa)
                        Dim best As MotifMatch = residues _
                            .ScanSites(fa, param.ScanCutoff, param.ScanMinW) _
                            .OrderByDescending(Function(a) a.identities) _
                            .FirstOrDefault

                        If best Is Nothing Then
                            Return 0
                        Else
                            Return best.identities
                        End If
                    End Function) _
            .AsVector
        Dim pvalue# = t.Test(scores, Vector.Zero(Dim:=scores.Length), Hypothesis.TwoSided).Pvalue
        Dim motif As New SequenceMotif With {
            .region = residues,
            .pvalue = pvalue,
            .score = scores.Sum,
            .seeds = alignment
        }

        Return motif
    End Function

    Public Function pairwiseSeeding(q As FastaSeq, s As FastaSeq, param As PopulatorParameter) As IEnumerable(Of HSP)
        Dim smithWaterman As New SmithWaterman(q.SequenceData, s.SequenceData, New DNAMatrix)
        Call smithWaterman.BuildMatrix()
        Dim result = smithWaterman.GetOutput(param.seedingCutoff, param.minW)
        Return result.HSP.Where(Function(seed) seed.LengthHit <= param.maxW)
    End Function

    ''' <summary>
    ''' get consensus sequence of a pairwise alignment seed
    ''' </summary>
    ''' <param name="pairwise"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Consensus(pairwise As HSP) As String
        Dim query As New FastaSeq With {.SequenceData = pairwise.Query, .Headers = {"query"}}
        Dim subject As New FastaSeq With {.SequenceData = pairwise.Subject, .Headers = {"subject"}}
        Dim globalAlign As GlobalAlign(Of Char) = RunNeedlemanWunsch.RunAlign(query, subject, 0).First
        Return globalAlign.Consensus
    End Function

    <Extension>
    Public Function Consensus(ga As GlobalAlign(Of Char)) As String
        Dim c1 = ga.query.Where(Function(c) c = "-"c).Count
        Dim c2 = ga.subject.Where(Function(c) c = "-"c).Count

        If c1 > c2 Then
            Return New String(ga.query)
        Else
            Return New String(ga.subject)
        End If
    End Function
End Module
