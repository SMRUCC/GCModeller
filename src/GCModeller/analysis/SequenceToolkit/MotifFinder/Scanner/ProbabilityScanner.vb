#Region "Microsoft.VisualBasic::b0bcab52bc6fe829c25bbe8634e2a728, analysis\SequenceToolkit\MotifFinder\Scanner\ProbabilityScanner.vb"

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

'   Total Lines: 153
'    Code Lines: 112 (73.20%)
' Comment Lines: 21 (13.73%)
'    - Xml Docs: 71.43%
' 
'   Blank Lines: 20 (13.07%)
'     File Size: 6.03 KB


' Module ProbabilityScanner
' 
'     Function: Compare, pairwiseIdentities, RefLoci, (+2 Overloads) ScanSites, ToResidues
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DataMining.AprioriRules
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Python
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports std = System.Math

Public Module ProbabilityScanner

    ''' <summary>
    ''' 基于PWM的概率匹配
    ''' </summary>
    ''' <param name="target"></param>
    ''' <param name="cutoff#"></param>
    ''' <param name="minW%"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function ScanSites(motif As SequenceMotif, target As FastaSeq,
                                       Optional cutoff# = 0.6,
                                       Optional minW% = 6,
                                       Optional pvalue As Double = 0.05,
                                       Optional top As Integer = 9) As IEnumerable(Of MotifMatch)

        For Each scan As MotifMatch In motif.region.ScanSites(target, cutoff, minW, pvalue_cut:=pvalue, top:=top)
            If Not motif.seeds Is Nothing Then
                scan.seeds = motif.seeds.names
            Else
                scan.seeds = {motif.tag}
            End If

            Yield scan
        Next
    End Function

    Public Class ZERO

        ReadOnly nucleotides As Char()
        ReadOnly cumulativeProbs As IReadOnlyCollection(Of Double)

        Sub New(background As Dictionary(Of Char, Double))
            Dim cumulativeProbs As New List(Of Double)()
            Dim nucleotides As Char() = background.Keys.ToArray
            ' 构建累积概率分布
            Dim cumulative As Double = 0
            For Each NT As Char In background.Keys
                cumulative += background(NT)
                cumulativeProbs.Add(cumulative)
            Next

            Me.nucleotides = nucleotides
            Me.cumulativeProbs = cumulativeProbs
        End Sub

        Public Function NextSequence(length As Integer) As String
            ' 生成随机序列
            Dim sequence As Char() = New Char(length - 1) {}

            For i As Integer = 1 To length
                Dim rndValue As Double = randf.NextDouble()

                ' 选择核苷酸
                For j As Integer = 0 To nucleotides.Length - 1
                    If rndValue <= cumulativeProbs(j) Then
                        sequence(i - 1) = nucleotides(j)
                        Exit For
                    End If
                Next
            Next

            Return New String(sequence)
        End Function

    End Class

    <Extension>
    Public Iterator Function LinearScan(motif As SequenceMotif, target As FastaSeq, Optional n As Integer = 1000) As IEnumerable(Of MotifMatch)
        For Each scan As MotifMatch In motif.region.LinearScan(target, n)
            If Not motif.seeds Is Nothing Then
                scan.seeds = motif.seeds.names
            Else
                scan.seeds = {motif.tag}
            End If

            Yield scan
        Next
    End Function

    <Extension>
    Public Function LinearScan(PWM As IReadOnlyCollection(Of Residue), target As FastaSeq, Optional n As Integer = 1000) As IEnumerable(Of MotifMatch)
        Dim slices = target.SequenceData.CreateSlideWindows(PWM.Count)
        Dim motifStr As String = PWM.JoinBy("")
        Dim seqTitle As String = target.Title
        Dim zero As String() = New String(n - 1) {}
        Dim background As New ZERO(background:=NT.ToDictionary(Function(b) b, Function(b) target.SequenceData.Count(b) / target.Length))

        For i As Integer = 0 To zero.Length - 1
            zero(i) = background.NextSequence(PWM.Count)
        Next

        Dim one As Vector = Vector.Ones(PWM.Count)
        Dim matches = slices.Select(Function(frag, offset)
                                        Dim total As Double = 0
                                        Dim v As Double() = New Double(frag.Length - 1) {}

                                        For i As Integer = 0 To frag.Length - 1
                                            v(i) = PWM(i)(frag(i))
                                            total += v(i)
                                        Next

                                        Dim score2 As Double = one.SSM(v.AsVector)
                                        Dim extremes = zero.Select(Function(si) score(si, PWM)).Count(Function(a) a >= total)
                                        Dim pvalue = (extremes + 1) / (n + 1)

                                        Return New MotifMatch With {
                                            .start = offset + 1,
                                            .ends = .start + frag.Length,
                                            .motif = motifStr,
                                            .segment = frag.CharString,
                                            .title = seqTitle,
                                            .score1 = total,
                                            .score2 = -std.Log10(pvalue),
                                            .identities = score2
                                        }
                                    End Function).OrderByDescending(Function(a) a.score1 * a.score2).ToArray

        Return matches.Take(5)
    End Function

    Private Function score(seq As Char(), PWM As IReadOnlyCollection(Of Residue)) As Double
        Dim total As Double = 0

        For i As Integer = 0 To seq.Length - 1
            total += PWM(i)(seq(i))
        Next

        Return total
    End Function

    ''' <summary>
    ''' 基于PWM的概率匹配
    ''' </summary>
    ''' <param name="PWM">PWM</param>
    ''' <param name="target"></param>
    ''' <param name="cutoff#"></param>
    ''' <param name="minW%"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function ScanSites(PWM As Residue(), target As FastaSeq,
                                       Optional cutoff# = 0.6,
                                       Optional minW% = 6,
                                       Optional pvalue_cut As Double = 0.05,
                                       Optional n As Integer = 500,
                                       Optional top As Integer = 9) As IEnumerable(Of MotifMatch)

        Dim chars As Char() = target.SequenceData.ToCharArray
        Dim subject As Residue() = target.ToResidues
        Dim symbol As New GenericSymbol(Of Residue)(
            equals:=Function(a, b) Compare(a, b) >= 0.85,
            similarity:=AddressOf Compare,
            toChar:=AddressOf Residue.Max,
            empty:=AddressOf Residue.GetEmpty
        )
        Dim core As New GSW(Of Residue)(PWM, subject, symbol)
        Dim result = core.BuildMatrix.GetMatches(cutoff * core.MaxScore) _
            .OrderByDescending(Function(a) a.score) _
            .Take(top) _
            .ToArray
        Dim seqTitle As String = target.Title
        Dim background As New ZERO(background:=NT.ToDictionary(Function(b) b, Function(b) target.SequenceData.Count(b) / target.Length))

        For Each m As Match In result
            If (m.toB - m.fromB) < minW Then
                Continue For
            End If

            Dim zero As String() = New String(n - 1) {}
            Dim len As Integer = std.Min(m.toA - m.fromA, m.toB - m.fromB)
            Dim motifSpan As New Span(Of Residue)(PWM, m.fromA, len)
            Dim motifSlice = motifSpan.SpanView
            Dim site As New Span(Of Char)(chars, m.fromB, motifSpan.Length)
            Dim motifStr As String = motifSpan.SpanCopy.JoinBy("")
            Dim v As Double() = New Double(motifStr.Length - 1) {}
            Dim total As Double = 0
            Dim one As Vector = Vector.Ones(v.Length)

            For i As Integer = 0 To motifStr.Length - 1
                v(i) = motifSpan(i)(site(i))
                total += v(i)
            Next
            For i As Integer = 0 To zero.Length - 1
                zero(i) = background.NextSequence(v.Length)
            Next

            Dim extremes = zero.Select(Function(si) score(si, motifSlice)).Count(Function(a) a >= total)
            Dim pvalue = (extremes + 1) / (n + 1)

            If pvalue >= pvalue_cut Then
                Continue For
            End If

            Dim score2 As Double = one.SSM(v.AsVector)

            Yield New MotifMatch With {
                .identities = score2,
                .segment = site.SpanCopy.CharString,
                .motif = motifStr,
                .score1 = m.score,
                .score2 = total,
                .title = seqTitle,
                .start = m.fromB,
                .ends = m.toB,
                .pvalue = pvalue
            }
        Next
    End Function

    <Extension>
    Private Function pairwiseIdentities(match As Match,
                                        PWM As IReadOnlyCollection(Of Residue),
                                        subject As Residue(),
                                        pairwiseMatrix As ScoreMatrix(Of Residue)) As GlobalAlign(Of Residue)

        Dim q = PWM.Skip(match.fromA).Take(match.toA - match.fromA).ToArray
        Dim s = subject.Skip(match.fromB).Take(match.toB - match.fromA).ToArray
        Dim pairwise As New MotifNeedlemanWunsch(q, s, ResidueScore.Gene)

        Return pairwise _
            .Compute() _
            .PopulateAlignments _
            .OrderByDescending(Function(gl)
                                   Return gl.Identities(pairwiseMatrix)
                               End Function) _
            .FirstOrDefault
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function RefLoci(m As Match) As Location
        Return New Location(m.fromB, m.toB)
    End Function

    Private Function Compare(prob As Residue, base As Residue) As Double
        For Each b As Char In NT
            ' 如果当前的碱基是b的时候
            If base.frequency(b) = 1.0R Then
                ' 则比较的得分就是当前的碱基b在motif模型中
                ' 对应的出现频率的高低
                ' 很明显，出现的频率越高，得分越高
                Return prob.frequency(b) * 10
            End If
        Next

        ' 当前的序列位点为N任意碱基的时候
        ' 则取最大的出现频率的得分
        With prob.frequency.ToArray
            Return .ElementAt(which.Min(.Values)) _
                   .Value * 10
        End With
    End Function

    <Extension>
    Public Function ToResidues(seq As FastaSeq) As Residue()
        Return seq _
            .SequenceData _
            .Select(Function(base)
                        Dim frq As New Dictionary(Of String, Double)

                        For Each b As Char In NT
                            If base = b Then
                                frq(b) = 1
                            Else
                                frq(b) = 0
                            End If
                        Next

                        Return New Residue With {
                            .frequency = frq
                        }
                    End Function) _
            .ToArray
    End Function
End Module
