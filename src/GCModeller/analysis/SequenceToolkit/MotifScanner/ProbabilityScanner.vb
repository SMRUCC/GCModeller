#Region "Microsoft.VisualBasic::de3e74514c1c45631a06cf3d9ea70bf9, analysis\SequenceToolkit\MotifScanner\ProbabilityScanner.vb"

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

    ' Module ProbabilityScanner
    ' 
    '     Function: Compare, RefLoci, ScanSites, ToResidues
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Probability
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

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
    Public Function ScanSites(prob As IEnumerable(Of Residue), target As FastaSeq, Optional cutoff# = 0.6, Optional minW% = 6) As SimpleSegment()
        Dim core As New GSW(Of Residue)(prob.ToArray, target.ToResidues, AddressOf Compare, AddressOf Residue.Max)
        Dim result = core.GetMatches(cutoff * core.MaxScore)
        Dim out = result _
            .OrderByDescending(Function(m) m.Score) _
            .Where(Function(m) (m.ToB - m.FromB) >= minW) _
            .Select(Function(m)
                        Dim frag = target.CutSequenceLinear(m.RefLoci)
                        frag.ID = m.Score
                        Return frag
                    End Function) _
            .ToArray

        Return out
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function RefLoci(m As Match) As Location
        Return New Location(m.FromB, m.ToB)
    End Function

    Private Function Compare(prob As Residue, base As Residue) As Integer
        For Each b As Char In nt
            If base.frequency(b) = 1.0R Then
                Return prob.frequency(b) * 10
            End If
        Next

        With prob.frequency.ToArray
            Return .ByRef(Which.Min(.Values)) _
                   .Value * 10
        End With
    End Function

    ReadOnly nt = {"A"c, "T"c, "G"c, "C"c}

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
