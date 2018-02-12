Imports System.Runtime.CompilerServices
Imports System.Text
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
Public Module CenterStar

    Public starIndex%
    Public centerString$
    Public direction%
    Public globalAlign$() = New String(2) {}
    Public multipleAlign$()
    Public sequence$() = New String(10000) {}
    Public totalScore% = 0

    ''' <summary>
    ''' Main
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="n%"></param>
    ''' <returns></returns>
    Public Function cumpute(matrix As Char()(), n%) As String

        findStarIndex(n, sequence)
        centerString = sequence(starIndex)
        multipleAlign = New String(n) {}
        MultipleAlignment(sequence, n)
        println("total cost: " + calculateTotalCost(matrix, n))

        Dim result As New StringBuilder

        For i As Integer = 0 To n - 1
            result.AppendLine(multipleAlign(i))
        Next

        Return result.ToString
    End Function

    ''' <summary>
    ''' this Function calculate() the total cost
    ''' </summary>
    ''' <returns></returns>
    Public Function calculateTotalCost(matrix As Char()(), n%) As Double
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
    ''' <param name="input"></param>
    ''' <returns></returns>
    <Extension>
    Public Function MultipleAlignment(input As IEnumerable(Of FastaSeq), n%) As String()
        Dim sequence$() = input.Select(Function(seq) seq.SequenceData).ToArray
        Dim centerString2$ = centerString

        For i As Integer = 0 To n - 1
            If (i = starIndex) Then
                multipleAlign(i) = centerString2
                Continue For
            End If

            calculateEditDistance(centerString, sequence(i))
            multipleAlign(i) = globalAlign(1)

            If (globalAlign(0).Length > centerString2.Length) Then

                Dim j2 = 0
                For j1 As Integer = 0 To globalAlign(0).Length
                    If (centerString2(j2) <> globalAlign(0)(j1)) Then
                        Dim a As StringBuilder
                        For k As Integer = 0 To i - 1
                            a = New StringBuilder(multipleAlign(k))
                            a.Insert(j1, "-")
                            multipleAlign(k) = a.ToString
                        Next

                    Else
                        j2 += 1
                    End If
                Next
                centerString2 = globalAlign(0)
            End If
            If (globalAlign(0).Length < centerString2.Length) Then
                Dim j2 = 0

                For j1 As Integer = 0 To centerString2.Length
                    If (centerString2(j1) <> globalAlign(0)(j2)) Then
                        Dim a As New StringBuilder(multipleAlign(i))
                        a.Insert(j1, "-")
                        multipleAlign(i) = a.ToString()
                    Else
                        j2 += 1
                    End If
                Next
            End If
        Next
    End Function

    ''' <summary>
    ''' This Function finds the minimum star cost from all sequences
    ''' </summary>
    ''' <param name="n%"></param>
    ''' <param name="s$"></param>
    Public Sub findStarIndex(n%, s$())
        Dim editDist = 0
        Dim minEditDist = Integer.MaxValue

        For i As Integer = 0 To n - 1
            For j As Integer = 0 To n - 1
                editDist = editDist + calculateEditDistance(s(i), s(j))
            Next

            If (editDist < minEditDist) Then
                minEditDist = editDist
                starIndex = i
            End If
            editDist = 0
        Next
    End Sub

    ''' <summary>
    ''' Function to calculate the edit distances
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
        If (seq1 = seq2) Then
            Return 0
        End If

        Dim l1 = seq1.Length
        Dim l2 = seq2.Length
        Dim match = 0
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
                pairAlignment(0)(k) = "-"
                pairAlignment(1)(k) = seq2(j - 1)
                j -= 1
                k += 1

            Else
                pairAlignment(0)(k) = seq1(i - 1)
                pairAlignment(1)(k) = "-"
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
End Module
