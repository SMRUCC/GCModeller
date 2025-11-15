#Region "Microsoft.VisualBasic::9883142f626b573f0ccce20d4bd5c65b, analysis\SequenceToolkit\MotifFinder\TrimMotif.vb"

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

'   Total Lines: 82
'    Code Lines: 70 (85.37%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 12 (14.63%)
'     File Size: 2.72 KB


' Module TrimMotif
' 
'     Function: (+2 Overloads) Cleanup, getBits
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Patterns

Module TrimMotif

    <Extension>
    Public Function Cleanup(motif As SequenceMotif, Optional cutoff As Double = 0.5) As SequenceMotif
        Dim bits As Double() = motif.getBits
        Dim start As Integer = Scan0
        Dim ends As Integer = bits.Length - 1

        For i As Integer = 0 To bits.Length - 1
            If bits(i) < cutoff Then
                start = i
            Else
                Exit For
            End If
        Next

        For i As Integer = bits.Length - 1 To 0 Step -1
            If bits(i) < cutoff Then
                ends = i
            Else
                Exit For
            End If
        Next

        If ends <= start Then
            Return Nothing
        End If

        Return New SequenceMotif With {
            .seeds = motif.seeds.Cleanup(start, ends),
            .alignments = motif.alignments.Skip(start).Take(ends - start).ToArray,
            .pvalue = motif.pvalue,
            .score = .alignments.Sum,
            .region = motif.region _
                .Skip(start) _
                .Take(ends - start) _
                .ToArray
        }
    End Function

    <Extension>
    Private Function Cleanup(seeds As MSAOutput, start As Integer, ends As Integer) As MSAOutput
        Dim MSA As New List(Of String)

        For Each line As String In seeds.MSA
            Call MSA.Add(Mid(line, start + 1, ends - start))
        Next

        Return New MSAOutput With {
            .cost = seeds.cost,
            .names = seeds.names,
            .MSA = MSA.ToArray
        }
    End Function

    <Extension>
    Private Function getBits(motif As SequenceMotif) As Double()
        Dim En As Double = SequenceMotif.E(nsize:=motif.seeds.size)
        Dim MSA As FastaFile = motif.seeds.ToFasta
        Dim f As PatternModel = PatternsAPI.Frequency(MSA)
        Dim bits As Double() = motif.region _
            .Select(Function(a, i)
                        Dim aa As IPatternSite = f(i)
                        Dim hi As Double = Probability.HI(aa)
                        Dim bit As Double = SequenceMotif.CalculatesBits(hi, En, NtMol:=True)
                        Dim h As Double = bit * aa.EnumerateValues.Sum

                        Return h
                    End Function) _
            .ToArray

        Return bits
    End Function
End Module
