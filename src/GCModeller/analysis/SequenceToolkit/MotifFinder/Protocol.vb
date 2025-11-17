#Region "Microsoft.VisualBasic::f37e96d1f03cee5a102952534dcf1372, analysis\SequenceToolkit\MotifFinder\Protocol.vb"

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

'   Total Lines: 213
'    Code Lines: 160 (75.12%)
' Comment Lines: 25 (11.74%)
'    - Xml Docs: 76.00%
' 
'   Blank Lines: 28 (13.15%)
'     File Size: 8.82 KB


' Module Protocol
' 
'     Function: BuildMotifPWM, (+2 Overloads) Consensus, motif, (+2 Overloads) PopulateMotifs, PWM
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.Analysis.SequenceAlignment.GlobalAlignment
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Protocol

    ''' <summary>
    ''' Motif motif finding workflow start from here
    ''' </summary>
    ''' <param name="inputs"></param>
    ''' <param name="leastN%"></param>
    ''' <param name="cleanMotif"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    <Extension>
    Public Function PopulateMotifs(inputs As IEnumerable(Of FastaSeq), param As PopulatorParameter,
                                   Optional leastN% = 5,
                                   Optional cleanMotif As Double = 0.5,
                                   Optional debug As Boolean = False) As IEnumerable(Of SequenceMotif)

        Dim regions As FastaSeq() = inputs.ToArray
        Dim scanner As SeedScanner = Activator.CreateInstance(type:=param.GetScanner, param, debug)
        Dim seeds As HSP() = scanner.GetSeeds(regions).ToArray

        Call param.logText($"create {seeds.Length} seeds...")
        Call param.logText("create motif cluster tree!")

        Return seeds.PopulateMotifs(param, leastN, cleanMotif, debug)
    End Function

    <Extension>
    Public Iterator Function PopulateMotifs(seeds As IEnumerable(Of HSP),
                                            param As PopulatorParameter,
                                            Optional leastN% = 5,
                                            Optional cleanMotif As Double = 0.5,
                                            Optional debug As Boolean = False) As IEnumerable(Of SequenceMotif)
        ' 构建出二叉树
        ' 每一个node都是一个cluster
        ' 可以按照成员的数量至少要满足多少条来取cluster的结果
        Dim pullSeeds As IEnumerable(Of NamedValue(Of String)) = seeds _
            .Select(Iterator Function(q) As IEnumerable(Of NamedValue(Of String))
                        Yield New NamedValue(Of String)(q.Query, q.Query)
                        Yield New NamedValue(Of String)(q.Subject, q.Subject)
                    End Function) _
            .IteratesALL

        Call param.logText("populate motifs...")
        Dim tree = pullSeeds.BuildAVLTreeCluster(param.seedingCutoff)
        Call param.logText("filter motif groups...")
        Dim filterGroups = tree _
            .PopulateNodes _
            .Where(Function(group) group.MemberSize >= leastN) _
            .OrderByDescending(Function(g) g.MemberSize) _
            .ToArray

        Call param.logText("build PWM!")

        ' 对聚类簇进行多重序列比对得到概率矩阵
        For Each motif As SequenceMotif In filterGroups _
            .Populate(parallel:=Not debug) _
            .Select(Function(group) group.motif(param)) _
            .IteratesALL

            If motif Is Nothing Then
                Continue For
            End If

            motif = motif.Cleanup(cutoff:=cleanMotif)

            If motif Is Nothing Then
                Continue For
            End If

            If motif.score > 0 AndAlso motif.SignificantSites >= param.significant_sites Then
                Yield motif
            End If
        Next
    End Function

    <Extension>
    Private Iterator Function motif(group As BinaryTree(Of String, String), param As PopulatorParameter) As IEnumerable(Of SequenceMotif)
        Dim members As List(Of String) = group!values
        Dim top As Integer = Integer.MaxValue - 1

        If members.Count > top Then
            For Each sample As SeqValue(Of String()) In Bootstraping.Samples(members, top, members.Count / top + 3)
                Yield sample.value.BuildMotifPWM(param)
            Next
        Else
            Yield members.BuildMotifPWM(param)
        End If
    End Function

    <Extension>
    Public Function BuildMotifPWM(members As IEnumerable(Of String), param As PopulatorParameter) As SequenceMotif
        Dim regions As FastaSeq() = members _
            .Select(Function(seq)
                        Return New FastaSeq With {
                            .SequenceData = seq,
                            .Headers = {FNV1a.GetDeterministicHashCode(seq).ToString}
                        }
                    End Function) _
            .ToArray
        Dim MSA As MSAOutput = regions.MultipleAlignment(Nothing)
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
        Dim nt As String() = SequenceModel.NT.Select(Function(c) CStr(c)).ToArray
        Dim MSA As String() = alignment.MSA

        For i As Integer = 0 To MSA(Scan0).Length - 1
            Dim index% = i
            Dim P = MSA _
                .Select(Function(seq) seq(index)) _
                .GroupBy(Function(c) c) _
                .ToDictionary(Function(c) c.Key.ToString,
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
        Dim PWMvec As Residue() = residues.ToArray
        Dim scores As Vector = members _
            .Select(Function(fa)
                        Dim best As MotifMatch = PWMvec _
                            .ScanSites(fa, param.ScanCutoff, param.ScanMinW) _
                            .OrderByDescending(Function(a) a.identities) _
                            .FirstOrDefault

                        If best Is Nothing Then
                            Return 0
                        ElseIf best.identities > param.ScanCutoff Then
                            Return best.identities
                        Else
                            Return 0
                        End If
                    End Function) _
            .AsVector

        If scores.All(Function(xi) xi = 0.0) OrElse (scores > 0).Sum / scores.Length < 0.85 Then
            Return Nothing
        Else
            scores(0) += 0.0000001
        End If

        Dim pvalue# = t.Test(scores, Vector.Zero(Dim:=scores.Length), Hypothesis.Less, strict:=False).Pvalue
        Dim motif As New SequenceMotif With {
            .region = residues,
            .pvalue = pvalue,
            .score = scores.Sum,
            .seeds = alignment,
            .alignments = scores.ToArray
        }

        Return motif
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
