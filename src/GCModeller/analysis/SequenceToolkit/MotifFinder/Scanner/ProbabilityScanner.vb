#Region "Microsoft.VisualBasic::d3b88ce3f897c2bd04b833deeafe9ba1, GCModeller\analysis\SequenceToolkit\MotifFinder\Scanner\ProbabilityScanner.vb"

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

    '   Total Lines: 155
    '    Code Lines: 113
    ' Comment Lines: 21
    '   Blank Lines: 21
    '     File Size: 6.07 KB


    ' Module ProbabilityScanner
    ' 
    '     Function: Compare, pairwiseIdentities, RefLoci, (+2 Overloads) ScanSites, ToResidues
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

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
                                       Optional identities As Double = 0.5) As IEnumerable(Of MotifMatch)

        For Each scan As MotifMatch In motif.region.ScanSites(target, cutoff, minW, identities)
            scan.seeds = motif.seeds.names

            Yield scan
        Next
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
    Public Iterator Function ScanSites(PWM As IReadOnlyCollection(Of Residue), target As FastaSeq,
                                       Optional cutoff# = 0.6,
                                       Optional minW% = 6,
                                       Optional identities As Double = 0.5) As IEnumerable(Of MotifMatch)

        Dim subject As Residue() = target.ToResidues
        Dim symbol As New GenericSymbol(Of Residue)(
            equals:=Function(a, b) Compare(a, b) >= 0.85,
            similarity:=AddressOf Compare,
            toChar:=AddressOf Residue.Max,
            empty:=AddressOf Residue.GetEmpty
        )
        Dim core As New GSW(Of Residue)(PWM, subject, symbol)
        Dim result = core.BuildMatrix.GetMatches(cutoff * core.MaxScore).ToArray
        Dim pairwiseMatrix = MotifNeedlemanWunsch.defaultScoreMatrix
        Dim maxIdentities As Value(Of Double) = 0
        Dim seqTitle As String = target.Title

        For Each m As Match In result
            If (m.toB - m.fromB) < minW Then
                Continue For
            End If

            Dim maxMatch = m.pairwiseIdentities(PWM, subject, pairwiseMatrix)

            If maxMatch Is Nothing OrElse (maxIdentities = maxMatch.Identities(pairwiseMatrix)) < identities Then
                Continue For
            End If

            Dim site As SimpleSegment = target.CutSequenceLinear(m.RefLoci)

            Yield New MotifMatch With {
                .identities = maxIdentities,
                .segment = site.SequenceData,
                .motif = maxMatch.query.JoinBy(""),
                .score1 = maxMatch.score,
                .score2 = m.score,
                .title = seqTitle,
                .start = site.Start,
                .ends = site.Ends
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
        For Each b As Char In nt
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
            Return .ByRef(Which.Min(.Values)) _
                   .Value * 10
        End With
    End Function

    ReadOnly nt As Char() = {"A"c, "T"c, "G"c, "C"c}

    <Extension>
    Public Function ToResidues(seq As FastaSeq) As Residue()
        Return seq _
            .SequenceData _
            .Select(Function(base)
                        Dim frq As New Dictionary(Of Char, Double)

                        For Each b In nt
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
