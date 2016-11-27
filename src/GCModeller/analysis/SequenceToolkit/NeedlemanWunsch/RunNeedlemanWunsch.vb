#Region "Microsoft.VisualBasic::58d94bc223b57d0ea733e97da646e225, ..\GCModeller\analysis\SequenceToolkit\NeedlemanWunsch\RunNeedlemanWunsch.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel

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
    Public Function RunAlign(fasta1 As FASTA.FastaToken,
                             fasta2 As FASTA.FastaToken,
                             [single] As Boolean,
                             Optional output As StreamWriter = Nothing,
                             Optional echo As Boolean = True) As Double

        Dim nw As New NeedlemanWunsch(fasta1.SequenceData, fasta2.SequenceData)

        ' display input
        If echo Then
            Console.WriteLine("Input:")
            Console.WriteLine(vbTab & "seq1 = " & nw.Query)
            Console.WriteLine(vbTab & "seq2 = " & nw.Subject)
            Console.WriteLine()
        End If

        ' run algorithm
        Call nw.compute()

        If [single] Then
            If output Is Nothing Then
                Console.WriteLine("Alignment :")
                Console.WriteLine(vbTab & "aligned1 = " & nw.getAligned1(0))
                Console.WriteLine(vbTab & "aligned2 = " & nw.getAligned2(0))
                Console.WriteLine("--------------------------------")
                Console.WriteLine("Alignment-Score = " & nw.Score)
            Else
                SyncLock output
                    Call nw.writeAlignment(output, True)
                End SyncLock
            End If
        Else
            If output Is Nothing Then
                ' display all possible optimal alignments
                For i As Integer = 0 To nw.NumberOfAlignments - 1
                    Console.WriteLine("Alignment " & (i + 1) & ":")
                    Console.WriteLine(vbTab & "aligned1 = " & nw.getAligned1(i))
                    Console.WriteLine(vbTab & "aligned2 = " & nw.getAligned2(i))
                Next

                Console.WriteLine("--------------------------------")
                Console.WriteLine("Alignment-Score = " & nw.Score)
            Else
                SyncLock output
                    Call nw.writeAlignment(output, False)
                End SyncLock
            End If
        End If

        Return nw.Score
    End Function

    ''' <summary>
    ''' Run the Needleman-Wunsch Algorithm 
    ''' </summary>
    ''' <param name="fasta1"> commandline arguments </param>
    ''' <exception cref="Exception"> </exception>
    ''' <returns>This function returns the alignment score</returns>
    Public Function RunAlign(fasta1 As FASTA.FastaToken, fasta2 As FASTA.FastaToken, [single] As Boolean, Optional outfile$ = Nothing) As Double
        If String.IsNullOrEmpty(outfile) Then
            Return RunAlign(fasta1, fasta2, [single], output:=Nothing)
        Else
            Using writer As StreamWriter = outfile.OpenWriter(Encodings.ASCII)
                Return RunAlign(fasta1, fasta2, [single], writer)
            End Using
        End If
    End Function
End Module
