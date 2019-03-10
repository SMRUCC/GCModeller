﻿#Region "Microsoft.VisualBasic::c94a702ce89117e5ac9fb25793321523, analysis\SequenceToolkit\MSA\CenterStar.vb"

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

    ' Class CenterStar
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: calculateEditDistance, calculateMinimum, calculateTotalCost, Compute
    ' 
    '     Sub: findStarIndex, multipleAlignmentImpl
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' ##### Multiple-sequence-alignment
''' 
''' This program calculates the multiple sequence alignment of k>1 DNA sequences.
''' 
''' The program use the Matrix.txt file For the substitution matrix. The matrix 
''' can be changed, And it used With Default values As: 
''' 
''' + 0 - Match
''' + 1 - Missmatch
''' + 2 - Indel
''' 
''' Algorithm used For this purpose Is Center Star Algotrithm
''' 
''' > https://github.com/EranCohenSW/Multiple-sequence-alignment/blob/master/Project/src/CenterStar.java
''' </summary>
Public Class CenterStar

    Dim starIndex%
    Dim centerString$
    Dim direction%
    Dim globalAlign$() = New String(2) {}
    Dim multipleAlign$()
    Dim sequence$()
    Dim totalScore% = 0
    Dim names$()

    Sub New(input As IEnumerable(Of FastaSeq))
        With input.ToArray
            sequence = .Select(Function(fa) fa.SequenceData) _
                       .ToArray
            names = .Select(Function(fa) fa.Title) _
                    .ToArray
        End With
    End Sub

    Sub New(input As IEnumerable(Of String))
        sequence = input.ToArray
        names = sequence.Select(Function(s, i) "seq" & i).ToArray
    End Sub

    ''' <summary>
    ''' Main
    ''' </summary>
    ''' <param name="matrix">得分矩阵</param>
    ''' <returns></returns>
    Public Function Compute(Optional matrix As ScoreMatrix = Nothing) As MSAOutput
        Dim n = sequence.Length
        Dim totalCost#

        If sequence.All(Function(s) s = sequence(Scan0)) Then
            ' 所输入的序列全部都是一样的？？
            multipleAlign = sequence.ToArray
            totalCost = 0
        Else
#If Not DEBUG Then
            Try
#End If
                findStarIndex()
                centerString = sequence(starIndex)
                multipleAlign = New String(n - 1) {}
                multipleAlignmentImpl()
                totalCost = calculateTotalCost((matrix Or ScoreMatrix.DefaultMatrix).Matrix, n)
#If Not DEBUG Then
            Catch ex As Exception
                Throw New Exception(sequence.JoinBy(vbCrLf), ex)
            End Try
#End If
        End If

        Return New MSAOutput With {
            .names = names.ToArray,
            .MSA = multipleAlign.ToArray,
            .cost = totalCost
        }
    End Function

    ''' <summary>
    ''' this Function calculate() the total cost
    ''' </summary>
    ''' <returns></returns>
    ''' 
    Private Function calculateTotalCost(matrix As Char()(), n%) As Double
        Dim length = multipleAlign(0).Length

        For i As Integer = 0 To n - 1
            For j As Integer = 0 To n - 1
                If (j > i) Then
                    For k As Integer = 0 To length - 1
                        For c As Integer = 0 To 24 - 1
                            If (multipleAlign(i)(k) = matrix(c)(0) AndAlso multipleAlign(j)(k) = matrix(c)(1)) Then
                                totalScore += Val(matrix(c)(2))
                            End If
                        Next
                    Next
                End If
            Next
        Next

        Return totalScore
    End Function

    ''' <summary>
    ''' The Function do the multiple alignment according to the center string 
    ''' </summary>
    Private Sub multipleAlignmentImpl()
        Dim centerString2$ = centerString
        Dim n = sequence.Length

        For i As Integer = 0 To n - 1
            If (i = starIndex) Then
                multipleAlign(i) = centerString2
                Continue For
            End If

            calculateEditDistance(centerString, sequence(i))
            multipleAlign(i) = globalAlign(1)

            If (globalAlign(0).Length > centerString2.Length) Then
                Dim j2 = 0

                For j1 As Integer = 0 To globalAlign(0).Length - 1
                    If (centerString2.CharAtOrDefault(j2, "-"c) <> globalAlign(0)(j1)) Then
                        Dim a As StringBuilder

                        For k As Integer = 0 To i - 1
                            With multipleAlign(k)
                                If .Length > j1 Then
                                    a = New StringBuilder(multipleAlign(k))
                                    a.Insert(j1, "-"c)
                                    multipleAlign(k) = a.ToString
                                Else
                                    multipleAlign(k) = .ByRef & New String("-"c, j1 - .Length)
                                End If
                            End With
                        Next

                    Else
                        j2 += 1
                    End If
                Next
                centerString2 = globalAlign(0)
            ElseIf (globalAlign(0).Length < centerString2.Length) Then
                Dim j2 = 0
                Dim globalAlign0 = globalAlign(Scan0)

                For j1 As Integer = 0 To centerString2.Length - 1
                    If (centerString2(j1) <> globalAlign0.CharAtOrDefault(j2)) Then
                        With multipleAlign(i)
                            If .Length > j1 Then
                                Dim a As New StringBuilder(multipleAlign(i))
                                a.Insert(j1, "-"c)
                                multipleAlign(i) = a.ToString()
                            Else
                                multipleAlign(i) = .ByRef & New String("-"c, j1 - .Length)
                            End If
                        End With
                    Else
                        j2 += 1
                    End If
                Next
            End If
        Next

        Dim maxLen = Aggregate a In multipleAlign Into Max(a.Length)

        For i As Integer = 0 To multipleAlign.Length - 1
            multipleAlign(i) &= New String("-"c, maxLen - multipleAlign(i).Length)
        Next
    End Sub

    ''' <summary>
    ''' This Function finds the minimum star cost from all sequences
    ''' </summary>
    Private Sub findStarIndex()
        Dim editDist = 0
        Dim minEditDist = Integer.MaxValue
        Dim n = sequence.Length

        For i As Integer = 0 To n - 1
            For j As Integer = 0 To n - 1
                editDist += calculateEditDistance(sequence(i), sequence(j))
            Next

            If (editDist < minEditDist) Then
                minEditDist = editDist
                starIndex = i
            End If

            editDist = 0
        Next
    End Sub

    ''' <summary>
    ''' Global alignment and function to calculate the edit distances
    ''' 
    ''' + 0   diagonal
    ''' + 1   left
    ''' + 2   up
    ''' 
    ''' </summary>
    ''' <param name="seq1$"></param>
    ''' <param name="seq2$"></param>
    ''' <returns></returns>
    Public Function calculateEditDistance(seq1$, seq2$) As Integer
        Dim l1 = seq1.Length
        Dim l2 = seq2.Length
        Dim match = 0

        If (seq1 = seq2) Then
            Return 0
        End If

        Dim i, j, k As Integer
        Dim score()() = MAT(Of Integer)(l1 + 1, l2 + 1)
        Dim trace()() = MAT(Of Integer)(l1 + 1, l2 + 1)

        score(0)(0) = 0
        trace(0)(0) = 0

        For i = 1 To l2 - 1
            score(0)(i) = i
            trace(0)(i) = 1
        Next
        For j = 1 To l1 - 1
            score(j)(0) = j
            trace(j)(0) = 2
        Next

        ' Filling the remaining cells in the matrix
        For i = 1 To l1 - 1
            For j = 1 To l2 - 1
                If (seq1(i - 1) = seq2(j - 1)) Then
                    match = 0
                Else
                    match = 1
                End If
                score(i)(j) = calculateMinimum(score(i - 1)(j - 1) + match, score(i)(j - 1) + 1, score(i - 1)(j) + 1)
                trace(i)(j) = direction
            Next
        Next

        ' Creating the global alignment by the trace found
        i = l1
        j = l2
        k = 0

        Dim pairAlignment As Char()() = MAT(Of Char)(2, l1 + l2)

        Do While i <> 0 OrElse j <> 0
            If (trace(i)(j) = 0) Then
                pairAlignment(0)(k) = seq1(i - 1)
                pairAlignment(1)(k) = seq2(j - 1)
                i -= 1
                j -= 1
                k += 1

            ElseIf (trace(i)(j) = 1) Then
                pairAlignment(0)(k) = "-"c
                pairAlignment(1)(k) = seq2(j - 1)
                j -= 1
                k += 1

            Else
                pairAlignment(0)(k) = seq1(i - 1)
                pairAlignment(1)(k) = "-"c
                i -= 1
                k += 1
            End If
        Loop

        Dim input$
        Dim stringReverse = MAT(Of Char)(2, k)

        i = 0

        Do While (k > 0)
            stringReverse(0)(i) = pairAlignment(0)(k - 1)
            stringReverse(1)(i) = pairAlignment(1)(k - 1)
            i += 1
            k -= 1
        Loop

        input = New String(stringReverse(0))
        globalAlign(0) = input
        input = New String(stringReverse(1))
        globalAlign(1) = input

        Return score(l1)(l2)
    End Function

    ''' <summary>
    ''' This Function calculates the minimum choice of three choices in the next move
    ''' </summary>
    ''' <param name="diagonal%"></param>
    ''' <param name="left%"></param>
    ''' <param name="up%"></param>
    ''' <returns></returns>
    Public Function calculateMinimum(diagonal%, left%, up%) As Integer
        Dim temp = diagonal

        direction = 0

        If (temp > left) Then
            temp = left
            direction = 1
        End If

        If (temp > up) Then
            temp = up
            direction = 2
        End If

        Return temp
    End Function
End Class
