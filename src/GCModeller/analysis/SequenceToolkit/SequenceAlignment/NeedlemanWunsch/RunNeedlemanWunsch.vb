#Region "Microsoft.VisualBasic::14764cb25b71aeed2f415be1f1f34380, analysis\SequenceToolkit\NeedlemanWunsch\RunNeedlemanWunsch.vb"

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

    '   Total Lines: 74
    '    Code Lines: 43 (58.11%)
    ' Comment Lines: 23 (31.08%)
    '    - Xml Docs: 91.30%
    ' 
    '   Blank Lines: 8 (10.81%)
    '     File Size: 2.86 KB


    ' Module RunNeedlemanWunsch
    ' 
    '     Function: (+2 Overloads) RunAlign
    ' 
    '     Sub: Print
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel

Namespace GlobalAlignment

    ''' <summary>
    ''' Application of the ``Needleman-Wunsch Algorithm``
    ''' Bioinformatics 1, WS 15/16
    ''' Dr. Kay Nieselt and Alexander Seitz
    ''' 
    ''' * Benjamin Schroeder
    ''' * Jonas Ditz
    ''' </summary>
    Public Module RunNeedlemanWunsch

        ''' <summary>
        ''' Run the Needleman-Wunsch Algorithm 
        ''' </summary>
        ''' <param name="fasta1"> commandline arguments </param>
        ''' <exception cref="Exception"> </exception>
        ''' <returns>This function returns the alignment score</returns>
        Public Function RunAlign(fasta1 As IPolymerSequenceModel,
                                 fasta2 As IPolymerSequenceModel,
                                 Optional ByRef score# = 0) As IEnumerable(Of GlobalAlign(Of Char))

            Dim nw As New NeedlemanWunsch(fasta1.SequenceData, fasta2.SequenceData)

            ' run algorithm
            Call nw.Compute()
            Call score.InlineCopy(nw.Score)

            Return nw.PopulateAlignments.ToArray
        End Function

        ''' <summary>
        ''' Run the Needleman-Wunsch Algorithm 
        ''' </summary>
        ''' <param name="fasta1"> commandline arguments </param>
        ''' <exception cref="Exception"> </exception>
        ''' <remarks>
        ''' 如果两条序列长度不一样，则较短的序列会被补充长度到最长的一条序列
        ''' </remarks>
        Public Function RunAlign(fasta1 As FASTA.FastaSeq, fasta2 As FASTA.FastaSeq, Optional dev As TextWriter = Nothing) As Double
            Dim score# = 0
            RunAlign(fasta1, fasta2, score).Print(dev)
            Return score
        End Function

        <Extension>
        Public Sub Print(results As IEnumerable(Of GlobalAlign(Of Char)), Optional dev As TextWriter = Nothing)
            With dev Or Console.Out.AsDefault
                For Each alignment In results
                    Call .WriteLine("align1: " & alignment.query)
                    Call .WriteLine("align2: " & alignment.subject)
                    Call .WriteLine("        " & alignment.query _
                         .Select(Function(c, i)
                                     If alignment.subject(i) = c Then
                                         Return "*"c
                                     Else
                                         Return " "c
                                     End If
                                 End Function) _
                         .CharString)
                    Call .WriteLine("score=" & alignment.Score)
                Next

                Call .Flush()
            End With
        End Sub
    End Module
End Namespace