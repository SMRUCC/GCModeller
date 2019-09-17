#Region "Microsoft.VisualBasic::c9ea67634baf5ff1256d63925b349bf1, analysis\SequenceToolkit\SmithWaterman\CoreDump.vb"

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

    ' Module CoreDump
    ' 
    '     Sub: (+2 Overloads) printAlignments, printDPMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman

Public Module CoreDump

    ''' <summary>
    ''' print the dynmaic programming matrix
    ''' </summary>
    ''' 
    <Extension>
    Public Sub printDPMatrix(Of T)(core As GSW(Of T), Optional dev As StreamWriter = Nothing)
        Dim subject = core.subject
        Dim query = core.query

        Console.Write(vbTab)
        For j As Integer = 1 To core.subjectLength
            Dim ch As Char = core.ToChar(subject(j - 1))
            Console.Write(vbTab & ch)
        Next
        Console.WriteLine()
        For i As Integer = 0 To core.queryLength

            If i > 0 Then
                Dim ch As Char = core.ToChar(query(i - 1))
                Console.Write(ch & vbTab)
            Else
                Console.Write(vbTab)
            End If
            For j As Integer = 0 To core.subjectLength
                Console.Write(core.score(i)(j) / GSW(Of T).NORM_FACTOR & vbTab)
            Next
            Console.WriteLine()
        Next
    End Sub

    ''' <summary>
    ''' Output the local alignments with the maximum score.
    ''' </summary>
    ''' 
    <Extension>
    Public Sub printAlignments(Of T)(core As GSW(Of T), Optional dev As StreamWriter = Nothing)
        ' find the cell with the maximum score
        Dim maxScore As Double = core.MaxScore

        ' skip the first row and column
        For i As Integer = 1 To core.queryLength

            For j As Integer = 1 To core.subjectLength

                If core.score(i)(j) = maxScore Then
                    core.printAlignments(i, j, "", "")
                End If
            Next
        Next
        ' Note: empty alignments are not printed.
    End Sub

    ''' <summary>
    ''' Output the local alignments ending in the (i, j) cell.
    ''' aligned1 and aligned2 are suffixes of final aligned strings
    ''' found in backtracking before calling this function.
    ''' Note: the strings are replicated at each recursive call.
    ''' Use buffers or stacks to improve efficiency.
    ''' </summary>
    ''' 
    <Extension>
    Private Sub printAlignments(Of T)(core As GSW(Of T), i%, j%, aligned1$, aligned2$)
        Dim query = core.query
        Dim subject = core.subject

        ' we've reached the starting point, so print the alignments	

        If (core.prevCells(i)(j) And GSW(Of T).DR_ZERO) > 0 Then
            Console.WriteLine(aligned1)
            Console.WriteLine(aligned2)
            Console.WriteLine("")

            ' Note: we could check other directions for longer alignments
            ' with the same score. we don't do it here.
            Return
        End If

        ' find out which directions to backtrack
        If (core.prevCells(i)(j) And GSW(Of T).DR_LEFT) > 0 Then
            Dim ch As Char = core.ToChar(query(i - 1))
            core.printAlignments(i - 1, j, ch & aligned1, "_" & aligned2)
        End If
        If (core.prevCells(i)(j) And GSW(Of T).DR_UP) > 0 Then
            Dim ch As Char = core.ToChar(subject(j - 1))
            core.printAlignments(i, j - 1, "_" & aligned1, ch & aligned2)
        End If
        If (core.prevCells(i)(j) And GSW(Of T).DR_DIAG) > 0 Then
            Dim q As Char = core.ToChar(query(i - 1))
            Dim s As Char = core.ToChar(subject(j - 1))
            core.printAlignments(i - 1, j - 1, q & aligned1, s & aligned2)
        End If
    End Sub
End Module
