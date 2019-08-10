#Region "Microsoft.VisualBasic::427186fce00ee2d0725f2d9443c097ba, RNA-Seq\Rockhopper\Java\DeNovoTranscript.vb"

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

    '     Class DeNovoTranscript
    ' 
    '         Properties: lowess, means, numPvalues, pvalues, qvalues
    '                     rPKMs, variances
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: array2D_to_1D_long, getMean, getNormalizedCounts_array, getPvalue, getRawCountNTs
    '                   getRawCountReads, getRawCounts_nts_array, getRawCounts_reads_array, Length, listToArray_double
    '                   listToArray_long, SequenceData, ToString
    ' 
    '         Sub: computeDifferentialExpression, computeExpression, computeVariance, setNormalizedCount, setQvalue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Text

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
    ''' An instance of the DeNovoTranscript class represents
    ''' a de novo assembled transcript.
    ''' </summary>
    Public Class DeNovoTranscript

        ''' <summary>
        '''******************************************
        ''' **********   Instance Variables   **********
        ''' </summary>

        Private sequence As String
        Private rawCounts_nts As List(Of List(Of Long))
        Private rawCounts_reads As List(Of List(Of Long))
        Private normalizedCounts As List(Of List(Of Long))
        Private m_RPKMs As List(Of Long)
        Private _means As List(Of Long)
        Public _variances As Long()()
        ' 2D array
        Public _lowess As Long()()
        ' 2D array
        Private m_pValues As List(Of Double)
        ' Differential expression
        Private m_qValues As List(Of Double)
        ' Differential expression (corrected)


        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        Public Sub New(sequence As String, readCounts_nts_linear As Long())
            Me.sequence = sequence

            ' Convert linear array to 2D ArrayList based on Conditions/Replicates.
            rawCounts_nts = New List(Of List(Of Long))(Assembler.conditionFiles.Count)
            rawCounts_reads = New List(Of List(Of Long))(Assembler.conditionFiles.Count)
            normalizedCounts = New List(Of List(Of Long))(Assembler.conditionFiles.Count)
            m_RPKMs = New List(Of Long)(Assembler.conditionFiles.Count)
            _means = New List(Of Long)(Assembler.conditionFiles.Count)
            Dim linearIndex As Integer = 0
            For i As Integer = 0 To Assembler.conditionFiles.Count - 1
                Dim files As String() = StringSplit(Assembler.conditionFiles(i), ",", True)
                rawCounts_nts.Add(New List(Of Long)(files.Length))
                rawCounts_reads.Add(New List(Of Long)(files.Length))
                normalizedCounts.Add(New List(Of Long)(files.Length))
                m_RPKMs.Add(CLng(0))
                _means.Add(CLng(0))
                For j As Integer = 0 To files.Length - 1
                    rawCounts_nts(i).Add(readCounts_nts_linear(linearIndex))
                    rawCounts_reads(i).Add(readCounts_nts_linear(linearIndex) \ DeNovoTranscripts.avgLengthOfReads(linearIndex))
                    normalizedCounts(i).Add(CLng(0))
                    linearIndex += 1
                Next
            Next
        End Sub

        ''' <summary>
        ''' Used when reading in from compressed file.
        ''' </summary>
        Public Sub New(sequence As String, rawCounts_nts As List(Of List(Of Long)), rawCounts_reads As List(Of List(Of Long)), normalizedCounts As List(Of List(Of Long)), RPKMs As List(Of Long), means As List(Of Long),
        variances As Long()(), lowess As Long()(), pValues As List(Of Double), qValues As List(Of Double))
            Me.sequence = sequence
            Me.rawCounts_nts = rawCounts_nts
            Me.rawCounts_reads = rawCounts_reads
            Me.normalizedCounts = normalizedCounts
            Me.m_RPKMs = RPKMs
            Me._means = means
            Me._variances = variances
            Me._lowess = lowess
            Me.m_pValues = pValues
            Me.m_qValues = qValues
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   Public Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Returns the length of the transcript sequence.
        ''' </summary>
        Public Overridable Function Length() As Integer
            Return Me.sequence.Length
        End Function

        ''' <summary>
        ''' Returns the sequence of this transcript.
        ''' </summary>
        Public Overridable Function SequenceData() As String
            Return Me.sequence
        End Function

        ''' <summary>
        ''' Returns the raw count of nts reads mapping to 
        ''' this transcript for the specified condition and
        ''' replicate.
        ''' </summary>
        Public Overridable Function getRawCountNTs(condition As Integer, replicate As Integer) As Long
            Return rawCounts_nts(condition)(replicate)
        End Function

        ''' <summary>
        ''' Returns the raw count of full length reads mapping
        ''' to this transcript for the specified condition and
        ''' replicate.
        ''' </summary>
        Public Overridable Function getRawCountReads(condition As Integer, replicate As Integer) As Long
            Return rawCounts_reads(condition)(replicate)
        End Function

        ''' <summary>
        ''' Returns the mean expression of this transcript in
        ''' the specified condition.
        ''' </summary>
        Public Overridable Function getMean(condition As Integer) As Long
            Return _means(condition)
        End Function

        ''' <summary>
        ''' Returns the p-value of this transcript for
        ''' the specified pair of conditions.
        ''' </summary>
        Public Overridable Function getPvalue(conditionPair As Integer) As Double
            Return m_pValues(conditionPair)
        End Function

        ''' <summary>
        ''' Returns the number of p-values, i.e.,
        ''' the number of pairs of conditions, 
        ''' for this transcript.
        ''' </summary>
        Public Overridable ReadOnly Property numPvalues() As Integer
            Get
                Return m_pValues.Count
            End Get
        End Property

        ''' <summary>
        ''' Set the normalized number of reads mapping to this
        ''' transcript in the specified condition and replicate.
        ''' </summary>
        Public Overridable Sub setNormalizedCount(condition As Integer, replicate As Integer, scalingFactor As Double, upperQuartile As Long)
            Dim multiplier As Double = scalingFactor / CDbl(upperQuartile)
            Dim normalizedCount As Long = CLng(multiplier * getRawCountNTs(condition, replicate))
            normalizedCounts(condition)(replicate) = normalizedCount
        End Sub

        ''' <summary>
        ''' Sets the q-value for the specified pair of conditions.
        ''' </summary>
        Public Overridable Sub setQvalue(conditionPair As Integer, value As Double)
            m_qValues(conditionPair) = value
        End Sub

        ''' <summary>
        ''' Returns an array representation of rawCounts_nts
        ''' in the specified condition.
        ''' </summary>
        Public Overridable Function getRawCounts_nts_array(condition As Integer) As Long()
            Return listToArray_long(rawCounts_nts(condition))
        End Function

        ''' <summary>
        ''' Returns an array representation of rawCounts_reads
        ''' in the specified condition.
        ''' </summary>
        Public Overridable Function getRawCounts_reads_array(condition As Integer) As Long()
            Return listToArray_long(rawCounts_reads(condition))
        End Function

        ''' <summary>
        ''' Returns an array representation of normalizedCounts
        ''' in the specified condition.
        ''' </summary>
        Public Overridable Function getNormalizedCounts_array(condition As Integer) As Long()
            Return listToArray_long(normalizedCounts(condition))
        End Function

        ''' <summary>
        ''' Returns an array representation of RPKMs.
        ''' </summary>
        Public Overridable ReadOnly Property rPKMs() As Long()
            Get
                Return listToArray_long(m_RPKMs)
            End Get
        End Property

        ''' <summary>
        ''' Returns an array representation of means.
        ''' </summary>
        Public Overridable ReadOnly Property means() As Long()
            Get
                Return listToArray_long(_means)
            End Get
        End Property

        ''' <summary>
        ''' Returns a 1D array representation of variances.
        ''' </summary>
        Public Overridable ReadOnly Property variances() As Long()
            Get
                Return array2D_to_1D_long(_variances)
            End Get
        End Property

        ''' <summary>
        ''' Returns a 1D array representation of lowess.
        ''' </summary>
        Public Overridable ReadOnly Property lowess() As Long()
            Get
                Return array2D_to_1D_long(_lowess)
            End Get
        End Property

        ''' <summary>
        ''' Returns an array representation of pValues.
        ''' </summary>
        Public Overridable ReadOnly Property pvalues() As Double()
            Get
                Return listToArray_double(m_pValues)
            End Get
        End Property

        ''' <summary>
        ''' Returns an array representation of qValues.
        ''' </summary>
        Public Overridable ReadOnly Property qvalues() As Double()
            Get
                Return listToArray_double(m_qValues)
            End Get
        End Property

        ''' <summary>
        ''' Calculates the RPKM and mean expression for
        ''' this transcript in the specified condition.
        ''' </summary>
        Public Overridable Sub computeExpression(condition As Integer)
            Dim sumRawCounts As Long = 0
            Dim totalCounts As Long = 0
            Dim mean As Long = 0
            For j As Integer = 0 To rawCounts_nts(condition).Count - 1
                sumRawCounts += rawCounts_nts(condition)(j)
                totalCounts += DeNovoTranscripts.totalReads(condition)(j)
                mean += normalizedCounts(condition)(j)
            Next
            sumRawCounts /= rawCounts_nts(condition).Count
            totalCounts /= rawCounts_nts(condition).Count
            mean /= normalizedCounts(condition).Count
            If totalCounts = 0 Then
                ' Avoid divide-by-zero error
                m_RPKMs(condition) = CLng(0)
            Else
                m_RPKMs(condition) = CLng(1000000000 * sumRawCounts / (totalCounts * sequence.Length))
            End If
            _means(condition) = mean
        End Sub

        ''' <summary>
        ''' Calculates the variance for
        ''' this transcript in the specified condition.
        ''' </summary>
        Public Overridable Sub computeVariance(numConditions As Integer)
            Dim varianceAdjustmentNoReplicates As Double = 1.1
            Dim varianceAdjustmentReplicates As Double = 1.2
            'ORIGINAL LINE: variances_Renamed = new long[numConditions][numConditions];
            'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            _variances = ReturnRectangularLongArray(numConditions, numConditions)
            'ORIGINAL LINE: lowess_Renamed = new long[numConditions][numConditions];
            'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            _lowess = ReturnRectangularLongArray(numConditions, numConditions)

            For x As Integer = 0 To numConditions - 1
                ' First of pair
                For y As Integer = 0 To numConditions - 1
                    ' Second of pair

                    If x = y Then
                        ' No need to compute variance with self
                        Continue For
                    End If

                    If normalizedCounts(x).Count = 1 Then
                        ' No replicates. Use partner surrogate.
                        Dim partner As Integer = y
                        Dim mean As Long = (_means(x) + _means(partner)) / 2
                        Dim variance As Long = ((_means(x) - mean) * (_means(x) - mean) + (_means(partner) - mean) * (_means(partner) - mean)) / 1
                        ' Sample standard deviation
                        'variances[x][y] = variance;
                        _variances(x)(y) = CLng(Math.Truncate(Math.Pow(variance, varianceAdjustmentNoReplicates)))
                    Else
                        ' We have replicates. Use the replicates to compute the variance.
                        Dim variance As Long = 0
                        For j As Integer = 0 To normalizedCounts(x).Count - 1
                            variance += CLng(normalizedCounts(x)(j) - _means(x)) * CLng(normalizedCounts(x)(j) - _means(x))
                        Next
                        variance /= (normalizedCounts(x).Count - 1)
                        ' Sample standard deviation
                        'variances[x][y] = variance;
                        _variances(x)(y) = CLng(Math.Truncate(Math.Pow(variance, varianceAdjustmentReplicates)))
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' For each pair of conditions, compute the p-value of differntial
        ''' expression for this transcript.
        ''' </summary>
        Public Overridable Sub computeDifferentialExpression()
            m_pValues = New List(Of Double)()
            m_qValues = New List(Of Double)()
            For x As Integer = 0 To normalizedCounts.Count - 2
                ' First of two conditions
                For y As Integer = x + 1 To normalizedCounts.Count - 1
                    ' Second of two conditions

                    Dim k_A As Double = 0.0
                    Dim k_B As Double = 0.0
                    For j As Integer = 0 To normalizedCounts(x).Count - 1
                        k_A += normalizedCounts(x)(j)
                    Next
                    For j As Integer = 0 To normalizedCounts(y).Count - 1
                        k_B += normalizedCounts(y)(j)
                    Next

                    If normalizedCounts(x).Count < normalizedCounts(y).Count Then
                        k_B *= normalizedCounts(x).Count / CDbl(normalizedCounts(y).Count)
                    ElseIf normalizedCounts(x).Count > normalizedCounts(y).Count Then
                        k_A *= normalizedCounts(y).Count / CDbl(normalizedCounts(x).Count)
                    End If

                    Dim q As Double = k_A + k_B

                    Dim mean_A As Double = q
                    Dim mean_B As Double = q
                    Dim variance_A As Double = _lowess(x)(y)
                    Dim variance_B As Double = _lowess(y)(x)

                    Dim p_a As Double = mean_A / variance_A
                    Dim p_b As Double = mean_B / variance_B
                    Dim r_a As Double = Math.Max(mean_A * mean_A / (variance_A - mean_A), 1.0)
                    ' r should not be < 1
                    Dim r_b As Double = Math.Max(mean_B * mean_B / (variance_B - mean_B), 1.0)
                    ' r should not be < 1
                    If (p_a < 0.0) OrElse (p_b < 0.0) OrElse (p_a > 1.0) OrElse (p_b > 1.0) OrElse (variance_A = 0.0) OrElse (variance_B = 0.0) Then
                        m_pValues.Add(1.0)
                        m_qValues.Add(1.0)
                        Continue For
                    End If

                    ' Compute p-value of differential expression in two conditions
                    Dim p_ab As Double = NegativeBinomial.pmf(r_a - 1, k_A + r_a - 1, p_a) * NegativeBinomial.pmf(r_b - 1, k_B + r_b - 1, p_b)
                    Dim k_sum As Long = CLng(k_A + k_B)

                    ' Fast p-value estimation
                    Dim numerator As Double = 0.0
                    Dim denominator As Double = 0.0
                    Dim mode As Long = CLng(k_B)

                    Dim a As Long = mode
                    ' Begin near middle
                    Dim increment As Long = 1
                    Dim alpha As Long = 1000
                    ' Number of times we increment by 1 (raising alpha raises precision but slows down computation)
                    Dim previous_p As Double = 0.0
                    While a <= k_sum
                        Dim b As Long = k_sum - a
                        Dim current_p As Double = NegativeBinomial.pmf(r_a - 1, a + r_a - 1, p_a) * NegativeBinomial.pmf(r_b - 1, b + r_b - 1, p_b)
                        denominator += current_p
                        If current_p <= p_ab Then
                            numerator += current_p
                        End If
                        If increment > 1 Then
                            Dim average_p As Double = (current_p + previous_p) / 2.0
                            denominator += average_p * (increment - 1)
                            If average_p <= p_ab Then
                                numerator += average_p * (increment - 1)
                            End If
                        End If
                        previous_p = current_p
                        If a - mode >= alpha Then
                            alpha *= 2
                            increment *= 2
                        End If
                        a += increment
                    End While
                    a = mode
                    ' Begin near middle
                    Dim decrement As Long = 1
                    alpha = 1000
                    ' Number of times we increment by 1 (raising alpha raises precision but slows down computation)
                    previous_p = 0.0
                    While a >= 0
                        Dim b As Long = k_sum - a
                        Dim current_p As Double = NegativeBinomial.pmf(r_a - 1, a + r_a - 1, p_a) * NegativeBinomial.pmf(r_b - 1, b + r_b - 1, p_b)
                        denominator += current_p
                        If current_p <= p_ab Then
                            numerator += current_p
                        End If
                        If decrement > 1 Then
                            Dim average_p As Double = (previous_p + current_p) / 2.0
                            denominator += average_p * (decrement - 1)
                            If average_p <= p_ab Then
                                numerator += average_p * (decrement - 1)
                            End If
                        End If
                        previous_p = current_p
                        If mode - a >= alpha Then
                            alpha *= 2
                            decrement *= 2
                        End If
                        a -= decrement
                    End While
                    Dim p_value As Double = 1.0
                    If denominator <> 0.0 Then
                        p_value = numerator / denominator
                    End If
                    m_pValues.Add(p_value)
                    m_qValues.Add(1.0)
                Next
            Next
        End Sub

        ''' <summary>
        ''' Returns a String representation of this transcript.
        ''' </summary>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append((SequenceData() & vbTab) + SequenceData().Length)
            For i As Integer = 0 To normalizedCounts.Count - 1
                If Assembler.verbose Then
                    ' Verbose output
                    For j As Integer = 0 To normalizedCounts(i).Count - 1
                        sb.Append(vbTab + rawCounts_reads(i)(j))
                    Next
                    For j As Integer = 0 To normalizedCounts(i).Count - 1
                        sb.Append(vbTab + normalizedCounts(i)(j))
                    Next
                    sb.Append(vbTab + m_RPKMs(i))
                End If
                sb.Append(vbTab + (_means(i) / Me.SequenceData().Length))
            Next
            For i As Integer = 0 To m_qValues.Count - 1
                If Assembler.verbose Then
                    sb.Append(vbTab & m_pValues(i).ToString())
                End If
                sb.Append(vbTab & m_qValues(i).ToString())
            Next
            Return sb.ToString()
        End Function



        ''' <summary>
        '''************************************************
        ''' **********   Private Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Returns an array representation of a list of longs.
        ''' </summary>
        Private Function listToArray_long(x As List(Of Long)) As Long()
            Dim a As Long() = New Long(x.Count - 1) {}
            For i As Integer = 0 To x.Count - 1
                a(i) = x(i)
            Next
            Return a
        End Function

        ''' <summary>
        ''' Returns an array representation of a list of doubles.
        ''' </summary>
        Private Function listToArray_double(x As List(Of Double)) As Double()
            Dim a As Double() = New Double(x.Count - 1) {}
            For i As Integer = 0 To x.Count - 1
                a(i) = x(i)
            Next
            Return a
        End Function

        ''' <summary>
        ''' Returns a 1D array representation of a 2D array of longs.
        ''' </summary>
        Private Function array2D_to_1D_long(x As Long()()) As Long()
            Dim a As Long() = New Long(x.Length * x.Length - 1) {}
            Dim index As Integer = 0
            For i As Integer = 0 To x.Length - 1
                For j As Integer = 0 To x.Length - 1
                    a(index) = x(i)(j)
                    index += 1
                Next
            Next
            Return a
        End Function

    End Class

End Namespace
