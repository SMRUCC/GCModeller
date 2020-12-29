#Region "Microsoft.VisualBasic::486b74b881292d13ecb1cfcc875b72c5, analysis\SequenceToolkit\MotifScanner\ProbabilityScanner.vb"

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

    ' Class MotifNeedlemanWunsch
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: defaultScoreMatrix
    ' 
    ' Module ProbabilityScanner
    ' 
    '     Function: Compare, pairwiseIdentities, RefLoci, ScanSites, ToResidues
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
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Probability
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Public Class MotifNeedlemanWunsch : Inherits NeedlemanWunsch(Of Residue)

    Sub New(query As Residue(), subject As Residue())
        Call MyBase.New(defaultScoreMatrix, symbolProvider)

        Me.Sequence1 = query
        Me.Sequence2 = subject
    End Sub

    Private Shared Function symbolProvider() As GenericSymbol(Of Residue)
        Return New GenericSymbol(Of Residue)(
            equals:=Function(a, b) a.topChar = b.topChar,
            similarity:=Function(a, b) a.Cos(b),
            toChar:=Function(x) x.topChar,
            empty:=Function()
                       Return New Residue With {
                           .frequency = New Dictionary(Of Char, Double)
                       }
                   End Function
        )
    End Function

    Public Shared Function defaultScoreMatrix() As ScoreMatrix(Of Residue)
        Return New ScoreMatrix(Of Residue)(
            Function(a, b)
                Dim maxA = Residue.Max(a)
                Dim maxB = Residue.Max(b)

                If a.isEmpty OrElse b.isEmpty Then
                    Return False
                End If

                If maxA = maxB Then
                    Return True
                Else
                    ' A是motif模型，所以不一致的时候以A为准
                    Dim freqB = b(maxA)

                    If freqB < 0.3 Then
                        Return False
                    Else
                        Return True
                    End If
                End If
            End Function) With {.MatchScore = 10}
    End Function
End Class


Public Module ProbabilityScanner

    ''' <summary>
    ''' 基于PWM的概率匹配
    ''' </summary>
    ''' <param name="prob">PWM</param>
    ''' <param name="target"></param>
    ''' <param name="cutoff#"></param>
    ''' <param name="minW%"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ScanSites(prob As IEnumerable(Of Residue), target As FastaSeq,
                              Optional cutoff# = 0.6,
                              Optional minW% = 6,
                              Optional identities As Double = 0.5) As SimpleSegment()

        Dim PWM = prob.ToArray
        Dim subject As Residue() = target.ToResidues
        Dim core As New GSW(Of Residue)(PWM, subject, AddressOf Compare, AddressOf Residue.Max)
        Dim result = core.GetMatches(cutoff * core.MaxScore).ToArray
        Dim pairwiseMatrix = MotifNeedlemanWunsch.defaultScoreMatrix
        Dim out = result _
            .OrderByDescending(Function(m) m.score) _
            .Where(Function(m)
                       Return (m.toB - m.fromB) >= minW AndAlso m.pairwiseIdentities(PWM, subject, pairwiseMatrix, identities)
                   End Function) _
            .Select(Function(m)
                        Dim frag = target.CutSequenceLinear(m.RefLoci)
                        frag.ID = m.score
                        Return frag
                    End Function) _
            .ToArray

        Return out
    End Function

    <Extension>
    Private Function pairwiseIdentities(match As Match, PWM As Residue(), subject As Residue(), pairwiseMatrix As ScoreMatrix(Of Residue), identities#) As Boolean
        Dim q = PWM.Skip(match.fromA).Take(match.toA - match.fromA).ToArray
        Dim s = subject.Skip(match.fromB).Take(match.toB - match.fromA).ToArray
        Dim pairwise As New MotifNeedlemanWunsch(q, s)

        pairwise.Compute()

        Return pairwise _
            .PopulateAlignments _
            .Any(Function(gl)
                     Return gl.Identities(pairwiseMatrix) >= identities
                 End Function)
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
