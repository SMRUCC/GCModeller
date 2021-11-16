#Region "Microsoft.VisualBasic::c68d267796159a11027074400f70e6b8, RNA-Seq\Rockhopper\Java\DeNovoTranscripts.vb"

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

    '     Class DeNovoTranscripts
    ' 
    '         Properties: averageTranscriptLength, medianTranscriptLength, minDiffExpressionLevels, numTranscripts, totalAssembledBases
    '                     transcriptSequences
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: determineTranscripts, getReadCountAtIndex, getTranscript, ToString
    ' 
    '         Sub: computeDifferentialExpression, computeExpression, computeVarianceAndLowess, correctPvalues, identifySimilarConditions
    '              merge, mergesort
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Text
Imports Oracle.Java.util.concurrent.atomic

'
' * Copyright 2014 Brian Tjaden
' *
' * This file is part of Rockhopper.
' *
' * Rockhopper is free software: you can redistribute it and/or modify
' * it under the terms of the GNU General Public License as published by
' * the Free Software Foundation, either version 3 of the License, or
' * any later version.
' *
' * Rockhopper is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' * GNU General Public License for more details.
' *
' * You should have received a copy of the GNU General Public License
' * (in the file gpl.txt) along with Rockhopper.  
' * If not, see <http://www.gnu.org/licenses/>.
' 

Namespace Java

    ''' <summary>
    ''' An instance of the DeNovoTranscripts class represents
    ''' a collection of de novo assembled transcripts.
    ''' </summary>
    Public Class DeNovoTranscripts

        ''' <summary>
        '''******************************************
        ''' **********   Instance Variables   **********
        ''' </summary>

        Private transcripts As List(Of DeNovoTranscript)
        Private upperQuartiles As List(Of List(Of Long))
        Private _minDiffExpressionLevels As List(Of Integer)
        ' Min diff expression level per condition


        ''' <summary>
        '''***************************************
        ''' **********   Class Variables   **********
        ''' </summary>

        Public Shared avgLengthOfReads As Integer()
        ' Avg length of mapping reads in each file
        Public Shared numReads As Integer()
        ' Total reads in each file
        Public Shared numMappingReads As Integer()
        ' Reads mapping to transcripts in each file
        Public Shared totalReads As List(Of List(Of Long))
        ' Total number of nt reads in each replicate
        Private Shared rand As New Random()
        ' Random number generator


        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        Public Sub New(bwtIndex As DeNovoIndex)
            DeNovoTranscripts.avgLengthOfReads = bwtIndex.avgLengthOfReads
            DeNovoTranscripts.numReads = bwtIndex.numReads
            DeNovoTranscripts.numMappingReads = bwtIndex.numMappingReads
            DeNovoTranscripts.totalReads = bwtIndex.totalReads
            transcripts = determineTranscripts(bwtIndex.sequence, bwtIndex.readCounts)
            computeExpression()
        End Sub

        ''' <summary>
        ''' Used when reading transcripts in from compressed file.
        ''' </summary>
        Public Sub New(transcripts As List(Of DeNovoTranscript))
            Me.transcripts = transcripts
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   Public Instance Methods   **********
        ''' </summary>

        Public Overridable ReadOnly Property numTranscripts() As Integer
            Get
                Return transcripts.Count
            End Get
        End Property

        Public Overridable Function getTranscript(i As Integer) As DeNovoTranscript
            Return transcripts(i)
        End Function

        ''' <summary>
        ''' Returns the average transcript length.
        ''' </summary>
        Public Overridable ReadOnly Property averageTranscriptLength() As Integer
            Get
                Dim length As Long = 0
                If transcripts.Count = 0 Then
                    Return 0
                End If
                For Each transcript As DeNovoTranscript In transcripts
                    length += transcript.Length
                Next
                Return CInt(length \ transcripts.Count)
            End Get
        End Property

        ''' <summary>
        ''' Returns the median transcript length.
        ''' </summary>
        Public Overridable ReadOnly Property medianTranscriptLength() As Integer
            Get
                Dim a As New List(Of Long)(transcripts.Count)
                For Each transcript As DeNovoTranscript In transcripts
                    a.Add(CLng(transcript.Length))
                Next
                Return CInt(Misc.select_Long(a, a.Count \ 2))
            End Get
        End Property

        ''' <summary>
        ''' Returns the sum of all transcript lengths.
        ''' </summary>
        Public Overridable ReadOnly Property totalAssembledBases() As Long
            Get
                Dim sum As Long = 0
                For Each transcript As DeNovoTranscript In transcripts
                    sum += transcript.Length
                Next
                Return sum
            End Get
        End Property

        Public Overridable ReadOnly Property transcriptSequences() As String
            Get
                Dim sb As New StringBuilder()
                For z As Integer = 0 To transcripts.Count - 1
                    sb.Append(transcripts(z).SequenceData() & vbLf)
                Next
                Return sb.ToString()
            End Get
        End Property

        Public Overridable Sub computeDifferentialExpression()
            computeVarianceAndLowess()
            For Each transcript As DeNovoTranscript In transcripts
                transcript.computeDifferentialExpression()
            Next
            correctPvalues()
        End Sub

        ''' <summary>
        ''' The background probabilty decreases as the number of reads increases.
        ''' We use this fact to estimate a minimum level of expression necessary
        ''' for us to be able to compute a p-value of differential expression.
        ''' For a given Condition, we ensure that all Replicates meet the 
        ''' expression threshold.
        ''' Currently, the expression threshold is set (based on anecdote and
        ''' experience) to 0.005.
        ''' </summary>
        Public Overridable WriteOnly Property minDiffExpressionLevels() As DeNovoIndex
            Set
                Dim THRESHOLD As Double = 0.005
                Dim allSequences As String = Value.sequence
                Dim readCounts As AtomicIntegerArray() = Value.readCounts
                Dim backgroundLength As Integer = 2
                Dim linearIndex As Integer = 0
                _minDiffExpressionLevels = New List(Of Integer)(Assembler.conditionFiles.Count)
                For i As Integer = 0 To Assembler.conditionFiles.Count - 1
                    ' For each condition
                    Dim files As String() = StringSplit(Assembler.conditionFiles(i), ",", True)
                    'ORIGINAL LINE: int[][] background = new int[files.Length][backgroundLength];
                    'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
                    Dim background As Integer()() = ReturnRectangularIntArray(files.Length, backgroundLength)
                    Dim backgroundParameter As Double() = New Double(files.Length - 1) {}
                    ' Parameter of geometric distribution of background reads. One per replicate in current condition.
                    For j As Integer = 0 To files.Length - 1
                        ' For each replicate

                        ' Populate background for current replicate
                        For z As Integer = 0 To allSequences.Length - 2
                            ' We go to length-1 to ignore final '$'
                            If readCounts(linearIndex).[Get](z) < background(j).Length Then
                                background(j)(readCounts(linearIndex).[Get](z)) += 1
                            Else
                                background(j)(background(j).Length - 1) += 1
                            End If
                        Next

                        ' Compute background parameter for current replicate
                        For k As Integer = 0 To backgroundLength - 2
                            backgroundParameter(j) += background(j)(k)
                        Next
                        backgroundParameter(j) /= (3.0 * 1.0 * allSequences.Length)
                        ' We use 3.0*1.0 since there is only one strand
                        linearIndex += 1
                    Next

                    ' Compute minDiffExpressionLevel for current condition
                    Dim minDiffExpressionLevel As Integer = 0
                    While True
                        Dim foundThreshold As Boolean = True
                        For j As Integer = 0 To files.Length - 1
                            Dim backgroundProb As Double = Math.Pow(1.0 - backgroundParameter(j), minDiffExpressionLevel) * backgroundParameter(j)
                            ' Based on geometric distribution
                            If backgroundProb >= THRESHOLD Then
                                foundThreshold = False
                            End If
                        Next
                        If foundThreshold Then
                            Exit While
                        End If
                        minDiffExpressionLevel += 1
                    End While
                    _minDiffExpressionLevels.Add(minDiffExpressionLevel)
                Next
            End Set
        End Property

        Public Overridable Overloads Function ToString(labels As String()) As String

            ' Include Header
            Dim sb As New StringBuilder()
            sb.Append("Sequence" & vbTab & "Length")
            For i As Integer = 0 To Assembler.conditionFiles.Count - 1
                Dim conditionName As String = "" & (i + 1)
                If (labels IsNot Nothing) AndAlso (labels.Length = Assembler.conditionFiles.Count) Then
                    conditionName = labels(i)
                End If
                If Assembler.verbose Then
                    ' Verbose output
                    Dim files As String() = StringSplit(Assembler.conditionFiles(i), ",", True)
                    If files.Length = 1 Then
                        sb.Append(vbTab & "Raw Counts " & conditionName)
                        sb.Append(vbTab & "Normalized Counts " & conditionName)
                    Else
                        For j As Integer = 0 To files.Length - 1
                            sb.Append(vbTab & "Raw Counts " & conditionName & " Replicate " & (j + 1))
                        Next
                        For j As Integer = 0 To files.Length - 1
                            sb.Append(vbTab & "Normalized Counts " & conditionName & " Replicate " & (j + 1))
                        Next
                    End If
                    sb.Append(vbTab & "RPKM " & conditionName)
                End If
                sb.Append(vbTab & "Expression " & conditionName)
            Next
            For x As Integer = 0 To Assembler.conditionFiles.Count - 2
                ' First in pair
                Dim conditionName1 As String = "" & (x + 1)
                If (labels IsNot Nothing) AndAlso (labels.Length = Assembler.conditionFiles.Count) Then
                    conditionName1 = labels(x)
                End If
                For y As Integer = x + 1 To Assembler.conditionFiles.Count - 1
                    ' Second in pair
                    Dim conditionName2 As String = "" & (y + 1)
                    If (labels IsNot Nothing) AndAlso (labels.Length = Assembler.conditionFiles.Count) Then
                        conditionName2 = labels(y)
                    End If

                    If Assembler.verbose Then
                        ' Verbose output
                        sb.Append(vbTab & "pValue " & conditionName1 & " vs " & conditionName2)
                    End If
                    sb.Append(vbTab & "qValue " & conditionName1 & " vs " & conditionName2)
                Next
            Next
            sb.AppendLine()

            ' Output each transcript
            For z As Integer = 0 To transcripts.Count - 1
                sb.Append(Convert.ToString(transcripts(z)) & vbLf)
            Next
            Return sb.ToString()
        End Function



        ''' <summary>
        '''************************************************
        ''' **********   Private Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Determine transcripts based on DeNovoIndex.
        ''' </summary>
        Private Function determineTranscripts(allSequences As String, readCounts As AtomicIntegerArray()) As List(Of DeNovoTranscript)
            Dim transcripts As New List(Of DeNovoTranscript)()
            Dim start As Integer = -1
            ' -1 is sentinel indicating no transcript
            Dim [end] As Integer = -1
            ' inclusive
            Dim readCounts_nts As Long() = New Long(readCounts.Length - 1) {}
            For i As Integer = 0 To allSequences.Length - 2
                ' We go to length-1 to ignore final '$'
                If allSequences(i) = "^"c Then
                    ' End of transcript.
                    If (start >= 0) AndAlso ([end] - start + 1 >= Assembler.minTranscriptLength) Then
                        transcripts.Add(New DeNovoTranscript(allSequences.Substring(start, [end] + 1 - start), readCounts_nts))
                    End If
                    start = -1
                    [end] = -1
                    For j As Integer = 0 To readCounts.Length - 1
                        readCounts_nts(j) = 0
                    Next
                Else
                    ' Within transcript
                    If (start < 0) AndAlso (getReadCountAtIndex(readCounts, i) >= Assembler.MIN_READS_MAPPING) Then
                        ' Start of new expressed transcript
                        start = i
                        For j As Integer = 0 To readCounts.Length - 1
                            readCounts_nts(j) += readCounts(j).[Get](i)
                        Next
                    End If
                    If getReadCountAtIndex(readCounts, i) >= Assembler.MIN_READS_MAPPING Then
                        ' Within expressed transcript
                        [end] = i
                        For j As Integer = 0 To readCounts.Length - 1
                            readCounts_nts(j) += readCounts(j).[Get](i)
                        Next
                    Else
                        ' It is possible expressed transcript just ended
                        If (start >= 0) AndAlso ([end] - start + 1 >= Assembler.minTranscriptLength) Then
                            transcripts.Add(New DeNovoTranscript(allSequences.Substring(start, [end] + 1 - start), readCounts_nts))
                        End If
                        start = -1
                        [end] = -1
                        For j As Integer = 0 To readCounts.Length - 1
                            readCounts_nts(j) = 0
                        Next
                    End If
                End If
            Next
            Return transcripts
        End Function

        ''' <summary>
        ''' For each transcript, compute its normalized expression, RPKM, and
        ''' mean expression in each condition and/or replicate.
        ''' </summary>
        Private Sub computeExpression()
            upperQuartiles = New List(Of List(Of Long))(Assembler.conditionFiles.Count)
            For i As Integer = 0 To Assembler.conditionFiles.Count - 1
                Dim files As String() = StringSplit(Assembler.conditionFiles(i), ",", True)
                upperQuartiles.Add(New List(Of Long)(files.Length))
                For j As Integer = 0 To files.Length - 1
                    Dim expressions As New List(Of Long)(transcripts.Count)
                    For k As Integer = 0 To transcripts.Count - 1
                        expressions.Add(transcripts(k).getRawCountNTs(i, j))
                    Next
                    Dim upperQuartile As Long = Misc.select_Long(expressions, CInt(Math.Truncate(0.75 * transcripts.Count)))
                    upperQuartiles(i).Add(upperQuartile)
                    For k As Integer = 0 To transcripts.Count - 1
                        transcripts(k).setNormalizedCount(i, j, 100000.0, upperQuartile)
                    Next
                Next
                For k As Integer = 0 To transcripts.Count - 1
                    transcripts(k).computeExpression(i)
                Next
            Next
        End Sub

        ''' <summary>
        ''' For each transcripts, compute its expression variance and lowess
        ''' in each condition.
        ''' </summary>
        Private Sub computeVarianceAndLowess()
            ' Compute variance
            For k As Integer = 0 To transcripts.Count - 1
                transcripts(k).computeVariance(Assembler.conditionFiles.Count)
            Next

            ' Compute Lowess
            For x As Integer = 0 To Assembler.conditionFiles.Count - 1
                ' First of pair
                Dim files As String() = StringSplit(Assembler.conditionFiles(x), ",", True)
                For y As Integer = 0 To Assembler.conditionFiles.Count - 1
                    ' Second of pair

                    If x = y Then
                        ' No need to compute lowess for self
                        Continue For
                    End If
                    '
                    '		if ((x > 0) && (files.length > 1)) {  // We have replicates
                    '		    for (DeNovoTranscript transcript : transcripts)
                    '			transcript.lowess[x][y] = transcript.lowess[0][y];
                    '		    continue;
                    '		}
                    '		

                    ' (x,y) lowess is NOT the same as (y,x) lowess
                    '		String[] files2 = Assembler.conditionFiles.get(y).split(",");
                    '		if ((x > y) && (files.length == 1) && (files2.length == 1)) {  // Already computed lowess[y][x], which is same as lowess[x][y]
                    '		    for (DeNovoTranscript transcript : transcripts)
                    '			transcript.lowess[x][y] = transcript.lowess[y][x];
                    '		    continue;
                    '		}
                    '		


                    Dim b As Double = 0.0
                    ' Bias correction term
                    For j As Integer = 0 To files.Length - 1
                        b += 100000.0 / CDbl(upperQuartiles(x)(j))
                    Next
                    b /= files.Length

                    ' Create list of gene expressions and list of gene variances
                    Dim expression As New List(Of Long)()
                    Dim variance As New List(Of Long)()
                    For Each transcript As DeNovoTranscript In transcripts
                        expression.Add(transcript.getMean(x))
                        variance.Add(transcript._variances(x)(y))
                    Next

                    ' Perform Lowess computation
                    Dim lowessVariance As List(Of Long) = Lowess.lowess(expression, variance)

                    ' Uncomment to output data for Lowess graph to StdOut
                    'System.out.println("Mean" + "\t" + "Variance" + "\t" + "Lowess");
                    'for (int k=0; k<lowessVariance.size(); k++) System.out.println(expression.get(k) + "\t" + variance.get(k) + "\t" + ((long)(lowessVariance.get(k) - expression.get(k)*b)));

                    ' Assign each gene its lowess variances (after subtracting bias correction term)
                    For k As Integer = 0 To transcripts.Count - 1
                        transcripts(k)._lowess(x)(y) = CLng(lowessVariance(k) - transcripts(k).getMean(x) * b)
                    Next
                Next
            Next
        End Sub

        ''' <summary>
        ''' Computes q-values for each transcript, i.e., corrected p-values,
        ''' using Benjamini Hochberg correction.
        ''' </summary>
        Private Sub correctPvalues()
            If transcripts.Count = 0 Then
                Return
            End If
            Dim pValue_index As Integer = 0
            For x As Integer = 0 To transcripts(0)._variances.Length - 2
                ' First in pair
                For y As Integer = x + 1 To transcripts(0)._variances.Length - 1
                    ' Second in pair

                    Dim indices As Integer() = New Integer(transcripts.Count - 1) {}
                    Dim pvalues As Double() = New Double(transcripts.Count - 1) {}
                    For j As Integer = 0 To transcripts.Count - 1
                        Dim transcript As DeNovoTranscript = transcripts(j)
                        pvalues(j) = transcript.getPvalue(pValue_index)
                        indices(j) = j

                        ' Check if there is too little expression to compute a p-value
                        Dim e1 As Double = transcript.getMean(x)
                        Dim e2 As Double = transcript.getMean(y)
                        e1 /= transcript.Length
                        e2 /= transcript.Length
                        If (e1 < _minDiffExpressionLevels(x)) AndAlso (e2 < _minDiffExpressionLevels(y)) Then
                            pvalues(j) = 1.0
                        End If
                    Next
                    mergesort(pvalues, indices, 0, transcripts.Count - 1)
                    Dim previous_BH_value As Double = 0.0
                    For k As Integer = 0 To pvalues.Length - 1
                        Dim BH_value As Double = pvalues(k) * transcripts.Count / (k + 1)
                        BH_value = Math.Min(BH_value, 1.0)
                        ' Disallow values greater than 1
                        ' To preserve monotonicity, we take maximum to ensure we don't
                        ' generate a value less than the previous value.
                        BH_value = Math.Max(BH_value, previous_BH_value)
                        previous_BH_value = BH_value
                        transcripts(indices(k)).setQvalue(pValue_index, BH_value)
                    Next
                    pValue_index += 1
                Next
            Next
        End Sub

        ''' <summary>
        ''' For each condition, determines the other condition that is most similar,
        ''' i.e., its "partner". If there are no replicate experiments then the
        ''' partner of each condition is used as a surrogate replicate.
        ''' Similarity between two conditions is measured by Pearson correlation
        ''' coefficient of normalized gene expression.
        ''' This method sets the "partner" of each condition.
        ''' </summary>
        Private Sub identifySimilarConditions()
            ' Unused.
        End Sub

        ''' <summary>
        ''' Returns the number of reads mapping to nucleotide index i
        ''' summed over all sequencing read files.
        ''' </summary>
        Private Function getReadCountAtIndex(readCounts As AtomicIntegerArray(), index As Integer) As Integer
            Dim sum As Integer = 0
            For i As Integer = 0 To readCounts.Length - 1
                sum += readCounts(i).[Get](index)
            Next
            Return sum
        End Function

        ''' <summary>
        ''' Mergesort parallel arrays "a" and "b" based on values in "a"
        ''' </summary>
        Private Shared Sub mergesort(a As Double(), b As Integer(), lo As Integer, hi As Integer)
            If lo < hi Then
                Dim q As Integer = (lo + hi) \ 2
                mergesort(a, b, lo, q)
                mergesort(a, b, q + 1, hi)
                merge(a, b, lo, q, hi)
            End If
        End Sub

        ''' <summary>
        ''' Mergesort helper method
        ''' </summary>
        Private Shared Sub merge(a As Double(), b As Integer(), lo As Integer, q As Integer, hi As Integer)
            Dim a1 As Double() = New Double(q - lo) {}
            Dim a2 As Double() = New Double(hi - q - 1) {}
            Dim b1 As Integer() = New Integer(q - lo) {}
            Dim b2 As Integer() = New Integer(hi - q - 1) {}
            Dim j As Integer = 0
            Dim i As Integer = 0

            For i = 0 To a1.Length - 1
                a1(i) = a(lo + i)
                b1(i) = b(lo + i)
            Next
            For j = 0 To a2.Length - 1
                a2(j) = a(q + 1 + j)
                b2(j) = b(q + 1 + j)
            Next
            i = 0
            ' Index for first half arrays
            j = 0
            ' Index for second half arrays
            For k As Integer = lo To hi
                If i >= a1.Length Then
                    a(k) = a2(j)
                    b(k) = b2(j)
                    j += 1
                ElseIf j >= a2.Length Then
                    a(k) = a1(i)
                    b(k) = b1(i)
                    i += 1
                ElseIf a1(i) <= a2(j) Then
                    a(k) = a1(i)
                    b(k) = b1(i)
                    i += 1
                Else
                    a(k) = a2(j)
                    b(k) = b2(j)
                    j += 1
                End If
            Next
        End Sub

    End Class


End Namespace
