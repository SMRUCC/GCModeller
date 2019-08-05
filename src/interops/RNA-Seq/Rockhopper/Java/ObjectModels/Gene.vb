#Region "Microsoft.VisualBasic::4648e06229d1d077c557ec7560464f4b, RNA-Seq\Rockhopper\Java\ObjectModels\Gene.vb"

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

    '     Class Gene
    ' 
    '         Properties: [stop], first, last, maxCoordinate, minCoordinate
    '                     minQvalue, name, oRF, product, start
    '                     startT, stopT, strand
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: expressionToString, getAvg, getMean, getNormalizedCount, getNumReplicates
    '                   getRawCount, getRawCount_reads, hasQvalue, isDifferntiallyExpressedORF, ToString
    ' 
    '         Sub: computeDifferentialExpression, computeExpression, computeVariance, correctPvalues, merge
    '              mergesort, setLowessVariances, setNormalizedCount, setRawCount, setRawCount_reads
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Text

'
' * Copyright 2013 Brian Tjaden
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
    ''' A Gene object represents a gene (either protein-coding or RNA).
    ''' A Gene object consists of a variety of data about a gene,
    ''' including its coordinates, strand, name, product, and 
    ''' expression information in each replicate (raw counts mapping
    ''' to gene and normalized counts mapping to gene) or in each
    ''' condition (mean, variance, lowess, RPKM) or in a pair of
    ''' conditions (p-value of differential expression).
    ''' </summary>
    Public Class Gene

        ''' <summary>
        '''***************************************
        ''' **********   CLASS VARIABLES   **********
        ''' </summary>

        Private Shared rand As New Random()
        ' Random number generator


        ''' <summary>
        '''******************************************
        ''' **********   INSTANCE VARIABLES   **********
        ''' </summary>

        Dim _start As Integer
        ' Translation start
        Dim _stop As Integer
        ' Translation stop
        Dim _strand As Char
        Dim ID As String
        Dim _name As String
        Dim synonym As String
        Dim _product As String
        Dim type As String
        ' ORF or RNA
        Dim tStart As Integer
        ' Transcription start
        Dim tStop As Integer
        ' Transcription stop
        Public rawCounts As List(Of List(Of Long))
        ' Reads mapping to each NT of gene
        Public rawCounts_reads As List(Of List(Of Long))
        ' Reads mapping to gene
        Dim normalizedCounts As List(Of List(Of Long))
        Dim RPKMs As List(Of Long)
        Dim means As List(Of Long)
        Dim variances As Long()()
        ' 2D array
        Dim lowess As Long()()
        ' 2D array
        Dim pValues As List(Of Double)
        ' Differential expression
        Dim qValues As List(Of Double)
        ' Differential expression (corrected)


        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        ''' <summary>
        ''' Constructs a new Genome object based on a line from a
        ''' gene file (either *.ptt or *.rnt).
        ''' </summary>
        Public Sub New(line As String, type As String)
            Dim parse_line As String() = StringSplit(line, vbTab, True)
            If parse_line.Length < 9 Then
                Output("Error - expecting 9 columns of gene information but found less than 9:" & vbTab & line & vbLf)
                Return
            End If
            Dim parse_coords As String() = StringSplit(parse_line(0), "\.", True)
            Me.type = type
            Me._strand = parse_line(1)(0)
            Me.ID = parse_line(3)
            Me._name = parse_line(4)
            Me.synonym = parse_line(5)
            Me._product = parse_line(8)

            ' Set coordinates based on strand and type
            Dim x As Integer = Convert.ToInt32(parse_coords(0))
            ' First coord
            Dim y As Integer = Convert.ToInt32(parse_coords(2))
            ' Second coord
            If type.Equals("ORF") AndAlso (_strand = "+"c) Then
                ' Plus strand ORF
                _start = x
                _stop = y
                tStart = 0
                tStop = 0
                ' Minus strand ORF
            ElseIf type.Equals("ORF") AndAlso (_strand = "-"c) Then
                _start = y
                _stop = x
                tStart = 0
                tStop = 0
                ' Plus strand RNA
            ElseIf type.Equals("RNA") AndAlso (_strand = "+"c) Then
                _start = 0
                _stop = 0
                tStart = x
                tStop = y
                ' Minus strand RNA
            ElseIf type.Equals("RNA") AndAlso (_strand = "-"c) Then
                _start = 0
                _stop = 0
                tStart = y
                tStop = x
                ' Ambiguous strand RNA
            ElseIf type.Equals("RNA") AndAlso (_strand = "?"c) Then
                _start = 0
                _stop = 0
                tStart = x
                tStop = y
            Else
                Output("Error - this case should be unreachable!" & vbLf)
            End If
        End Sub

        ''' <summary>
        '''***********************************************
        ''' **********   PUBLIC INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Return the gene's strand.
        ''' </summary>
        Public Overridable ReadOnly Property strand() As Char
            Get
                Return _strand
            End Get
        End Property

        ''' <summary>
        ''' Return the gene's start coordinate of translation.
        ''' </summary>
        Public Overridable ReadOnly Property start() As Integer
            Get
                Return _start
            End Get
        End Property

        ''' <summary>
        ''' Return the gene's stop coordinate of translation.
        ''' </summary>
        Public Overridable ReadOnly Property [stop]() As Integer
            Get
                Return _stop
            End Get
        End Property

        ''' <summary>
        ''' Reeturn the gene's name.
        ''' </summary>
        Public Overridable ReadOnly Property name() As String
            Get
                If _name.Length > 1 Then
                    Return _name
                End If
                Return synonym
            End Get
        End Property

        ''' <summary>
        ''' Returns true if this gene is a protein coding gene,
        ''' false otherwise.
        ''' </summary>
        Public Overridable ReadOnly Property oRF() As Boolean
            Get
                Return type.Equals("ORF")
            End Get
        End Property

        ''' <summary>
        ''' Returns this gene's product.
        ''' </summary>
        Public Overridable ReadOnly Property product() As String
            Get
                Return _product
            End Get
        End Property

        ''' <summary>
        ''' Set the transcription start coordinate.
        ''' </summary>
        Public Overridable Property startT() As Integer
            Get
                Return Me.tStart
            End Get
            Set
                Me.tStart = Value
            End Set
        End Property

        ''' <summary>
        ''' Set the transcription stop coordinate.
        ''' </summary>
        Public Overridable Property stopT() As Integer
            Get
                Return Me.tStop
            End Get
            Set
                Me.tStop = Value
            End Set
        End Property

        ''' <summary>
        ''' Returns the first (smallest) coordinate of this Gene.
        ''' If this Gene is an ORF it returns the smallest translation coordinate.
        ''' If this Gene is an RNA it returns the smallest transcription coordinate.
        ''' </summary>
        Public Overridable ReadOnly Property first() As Integer
            Get
                If oRF Then
                    Return Math.Min(_start, _stop)
                End If
                Return Math.Min(tStart, tStop)
            End Get
        End Property

        ''' <summary>
        ''' Returns the last (largest) coordinate of this Gene.
        ''' If this Gene is an ORF it returns the largest translation coordinate.
        ''' If this Gene is an RNA it returns the largest transcription coordinate.
        ''' </summary>
        Public Overridable ReadOnly Property last() As Integer
            Get
                If oRF Then
                    Return Math.Max(_start, _stop)
                End If
                Return Math.Max(tStart, tStop)
            End Get
        End Property

        ''' <summary>
        ''' Return the number of reads mapping to the gene in the
        ''' specified condition and specified replicate.
        ''' </summary>
        Public Overridable Function getRawCount(condition As Integer, replicate As Integer) As Long
            If condition < rawCounts.Count Then
                If replicate < rawCounts(condition).Count Then
                    Return rawCounts(condition)(replicate)
                End If
            End If
            Return 0
        End Function

        ''' <summary>
        ''' Return the number of reads mapping to the gene in the
        ''' specified condition and specified replicate.
        ''' </summary>
        Public Overridable Function getRawCount_reads(condition As Integer, replicate As Integer) As Long
            If condition < rawCounts_reads.Count Then
                If replicate < rawCounts_reads(condition).Count Then
                    Return rawCounts_reads(condition)(replicate)
                End If
            End If
            Return 0
        End Function

        ''' <summary>
        ''' Return the normalized number of reads mapping to the gene in the
        ''' specified condition and specified replicate.
        ''' </summary>
        Public Overridable Function getNormalizedCount(condition As Integer, replicate As Integer) As Long
            If condition < normalizedCounts.Count Then
                If replicate < normalizedCounts(condition).Count Then
                    Return normalizedCounts(condition)(replicate)
                End If
            End If
            Return 0
        End Function

        ''' <summary>
        ''' Return the mean reads mapping to the gene in the
        ''' specified condition.
        ''' </summary>
        Public Overridable Function getMean(condition As Integer) As Long
            If condition < means.Count Then
                Return means(condition)
            End If
            Return 0
        End Function

        ''' <summary>
        ''' Returns the average expression of the gene (averaged
        ''' over the length of the gene) in the specified
        ''' condition.
        ''' </summary>
        Public Overridable Function getAvg(condition As Integer) As Long
            Dim avg As Long = 0
            If type.CompareTo("ORF") = 0 Then
                avg = means(condition) / (Math.Max(Me._start, Me._stop) - Math.Min(Me._start, Me._stop) + 1)
            End If
            If type.CompareTo("RNA") = 0 Then
                avg = means(condition) / (Math.Max(Me.tStart, Me.tStop) - Math.Min(Me.tStart, Me.tStop) + 1)
            End If
            Return avg
        End Function

        ''' <summary>
        ''' Return the number of replicates in the specified condition.
        ''' </summary>
        Public Overridable Function getNumReplicates(condition As Integer) As Integer
            If condition >= rawCounts.Count Then
                Return 0
            Else
                Return rawCounts(condition).Count
            End If
        End Function

        ''' <summary>
        ''' Returns the minimum q-value for this Gene.
        ''' </summary>
        Public Overridable ReadOnly Property minQvalue() As Double
            Get
                Dim min As Double = 1.0
                For i As Integer = 0 To qValues.Count - 1
                    min = Math.Min(min, CDbl(qValues(i)))
                Next
                Return min
            End Get
        End Property

        Public Overridable Function hasQvalue(c As Integer) As Boolean
            Return ((qValues IsNot Nothing) AndAlso (qValues.Count > c))
        End Function

        ''' <summary>
        ''' Returns a String representation of a a Gene's expression 
        ''' in each condition and p-values of differential expression.
        ''' </summary>
        Public Overridable Function expressionToString() As String
            Dim sb As New StringBuilder()
            For i As Integer = 0 To means.Count - 1
                If verbose Then
                    ' Verbose output
                    For j As Integer = 0 To rawCounts(i).Count - 1
                        sb.Append(vbTab & Convert.ToString(rawCounts_reads(i)(j)))
                    Next
                    For j As Integer = 0 To normalizedCounts(i).Count - 1
                        sb.Append(vbTab & Convert.ToString(normalizedCounts(i)(j)))
                    Next
                    sb.Append(vbTab & Convert.ToString(RPKMs(i)))
                End If
                sb.Append(vbTab & getAvg(i))
            Next
            For i As Integer = 0 To qValues.Count - 1
                If verbose Then
                    sb.Append(vbTab & pValues(i).ToString())
                End If
                sb.Append(vbTab & qValues(i).ToString())
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Returns a String representation of a Gene.
        ''' </summary>
        Public Overridable Overloads Function ToString() As String
            Try
                Dim sb As New StringBuilder()
                Dim tStart As String = ""
                Dim start As String = ""
                Dim [stop] As String = ""
                Dim tStop As String = ""
                If Me.tStart > 0 Then
                    tStart += Me.tStart
                End If
                If Me._start > 0 Then
                    start += Me._start
                End If
                If Me._stop > 0 Then
                    [stop] += Me._stop
                End If
                If Me.tStop > 0 Then
                    tStop += Me.tStop
                End If
                sb.Append(tStart & vbTab & start & vbTab & [stop] & vbTab & tStop & vbTab & _strand & vbTab & _name & vbTab & synonym & vbTab & _product)
                Return sb.ToString()
            Catch ex As Exception
                Return synonym
            End Try
        End Function

        ''' <summary>
        ''' Returns the coordinate (among transcription start/stop coordinates
        ''' and translation start/stop coordinates) with minimum value.
        ''' </summary>
        Public Overridable ReadOnly Property minCoordinate() As Integer
            Get
                Dim minCoord As Integer = Integer.MaxValue
                If tStart > 0 Then
                    minCoord = Math.Min(minCoord, tStart)
                End If
                If _start > 0 Then
                    minCoord = Math.Min(minCoord, _start)
                End If
                If _stop > 0 Then
                    minCoord = Math.Min(minCoord, _stop)
                End If
                If tStop > 0 Then
                    minCoord = Math.Min(minCoord, tStop)
                End If
                Return minCoord
            End Get
        End Property

        ''' <summary>
        ''' Returns the coordinate (among transcription start/stop coordinates
        ''' and translation start/stop coordinates) with maximum value.
        ''' </summary>
        Public Overridable ReadOnly Property maxCoordinate() As Integer
            Get
                Dim maxCoord As Integer = -1
                If tStart > 0 Then
                    maxCoord = Math.Max(maxCoord, tStart)
                End If
                If _start > 0 Then
                    maxCoord = Math.Max(maxCoord, _start)
                End If
                If _stop > 0 Then
                    maxCoord = Math.Max(maxCoord, _stop)
                End If
                If tStop > 0 Then
                    maxCoord = Math.Max(maxCoord, tStop)
                End If
                Return maxCoord
            End Get
        End Property

        ''' <summary>
        ''' Set the number of reads mapping to this Gene in the specified
        ''' replicate in the specified condition.
        ''' </summary>
        Public Overridable Sub setRawCount(condition As Integer, replicate As Integer, readsForGene As Long)
            ' Initialize instance variables
            If rawCounts Is Nothing Then
                rawCounts = New List(Of List(Of Long))()
                rawCounts_reads = New List(Of List(Of Long))()
                normalizedCounts = New List(Of List(Of Long))()
                RPKMs = New List(Of Long)()
                means = New List(Of Long)()
            End If
            While rawCounts.Count < condition + 1
                rawCounts.Add(New List(Of Long)())
                rawCounts_reads.Add(New List(Of Long)())
                normalizedCounts.Add(New List(Of Long)())
                RPKMs.Add(CLng(0))
                means.Add(CLng(0))
            End While
            While rawCounts(condition).Count < replicate + 1
                rawCounts(condition).Add(CLng(0))
                rawCounts_reads(condition).Add(CLng(0))
                normalizedCounts(condition).Add(CLng(0))
            End While

            ' Populate rawCounts
            rawCounts(condition)(replicate) = readsForGene
        End Sub

        ''' <summary>
        ''' Set the number of reads mapping to this Gene in the specified
        ''' replicate in the specified condition.
        ''' </summary>
        Public Overridable Sub setRawCount_reads(condition As Integer, replicate As Integer, readsForGene As Long)
            If condition < normalizedCounts.Count Then
                If replicate < normalizedCounts(condition).Count Then
                    rawCounts_reads(condition)(replicate) = readsForGene
                End If
            End If
        End Sub

        ''' <summary>
        ''' Set the normalized number of reads mapping to this Gene in the specified
        ''' replicate in the specified condition.
        ''' </summary>
        Public Overridable Sub setNormalizedCount(condition As Integer, replicate As Integer, scalingFactor As Double, upperQuartile As Long)
            If condition < normalizedCounts.Count Then
                If replicate < normalizedCounts(condition).Count Then
                    Dim multiplier As Double = scalingFactor / CDbl(upperQuartile)
                    Dim normalizedCount As Long = CLng(Math.Truncate(multiplier * getRawCount(condition, replicate)))
                    normalizedCounts(condition)(replicate) = normalizedCount
                End If
            End If
        End Sub

        ''' <summary>
        ''' For each condition, across all replicates of the conditions, compute
        ''' the mean and RPKM for this Gene.
        ''' </summary>
        Public Overridable Sub computeExpression(conditions As List(Of Condition))
            For i As Integer = 0 To means.Count - 1
                ' For each condition

                ' Compute RPKM and mean
                Dim sumRawCounts As Long = 0
                Dim totalCounts As Long = 0
                Dim mean As Long = 0
                For j As Integer = 0 To rawCounts(i).Count - 1
                    sumRawCounts += rawCounts(i)(j)
                    totalCounts += conditions(i).getReplicate(j).totalReads
                    mean += normalizedCounts(i)(j)
                Next
                sumRawCounts /= rawCounts(i).Count
                totalCounts /= rawCounts(i).Count
                mean /= normalizedCounts(i).Count
                If totalCounts = 0 Then
                    ' Avoid divide-by-zero error
                    RPKMs(i) = CLng(0)
                Else
                    RPKMs(i) = CLng(1000000000 * sumRawCounts \ (totalCounts * (maxCoordinate - minCoordinate + 1)))
                End If
                means(i) = mean
            Next
        End Sub

        ''' <summary>
        ''' For each condition, across all replicates of the conditions, compute
        ''' the variance for this Gene.
        ''' </summary>
        Public Overridable Sub computeVariance(conditions As List(Of Condition))
            Dim varianceAdjustmentNoReplicates As Double = 1.1
            Dim varianceAdjustmentReplicates As Double = 1.2
            'ORIGINAL LINE: variances = new long[conditions.Count][conditions.Count];
            'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            variances = ReturnRectangularLongArray(conditions.Count, conditions.Count)
            'ORIGINAL LINE: lowess = new long[conditions.Count][conditions.Count];
            'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            lowess = ReturnRectangularLongArray(conditions.Count, conditions.Count)

            For x As Integer = 0 To conditions.Count - 1
                ' First of pair
                For y As Integer = 0 To conditions.Count - 1
                    ' Second of pair

                    If x = y Then
                        ' No need to compute variance with self
                        Continue For
                    End If

                    If conditions(x).numReplicates() = 1 Then
                        ' No replicates. Use partner surrogate.
                        Dim partner As Integer = y
                        Dim mean As Long = (means(x) + means(partner)) / 2
                        Dim variance As Long = ((means(x) - mean) * (means(x) - mean) + (means(partner) - mean) * (means(partner) - mean)) / 1
                        ' Sample standard deviation
                        variances(x)(y) = CLng(Math.Truncate(Math.Pow(variance, varianceAdjustmentNoReplicates)))
                    Else
                        ' We have replicates. Use the replicates to compute the variance.
                        Dim variance As Long = 0
                        For j As Integer = 0 To conditions(x).numReplicates() - 1
                            variance += CLng(normalizedCounts(x)(j) - means(x)) * CLng(normalizedCounts(x)(j) - means(x))
                        Next
                        variance /= (conditions(x).numReplicates() - 1)
                        ' Sample standard deviation
                        variances(x)(y) = CLng(Math.Truncate(Math.Pow(variance, varianceAdjustmentReplicates)))
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' For each pair of conditions, compute the p-value of differntial
        ''' expression for this Gene.
        ''' </summary>
        Public Overridable Sub computeDifferentialExpression()
            pValues = New List(Of Double)()
            qValues = New List(Of Double)()
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
                    Dim variance_A As Double = lowess(x)(y)
                    Dim variance_B As Double = lowess(y)(x)

                    Dim p_a As Double = mean_A / variance_A
                    Dim p_b As Double = mean_B / variance_B
                    Dim r_a As Double = Math.Max(mean_A * mean_A / (variance_A - mean_A), 1.0)
                    ' r should not be < 1
                    Dim r_b As Double = Math.Max(mean_B * mean_B / (variance_B - mean_B), 1.0)
                    ' r should not be < 1
                    If (p_a < 0.0) OrElse (p_b < 0.0) OrElse (p_a > 1.0) OrElse (p_b > 1.0) OrElse (variance_A = 0.0) OrElse (variance_B = 0.0) Then
                        pValues.Add(1.0)
                        qValues.Add(1.0)
                        Continue For
                    End If

                    ' Compute p-value of differential expression in two conditions
                    Dim p_ab As Double = NegativeBinomial.pmf(r_a - 1, k_A + r_a - 1, p_a) * NegativeBinomial.pmf(r_b - 1, k_B + r_b - 1, p_b)
                    Dim k_sum As Long = CLng(Math.Truncate(k_A + k_B))

                    ' Fast p-value estimation
                    Dim numerator As Double = 0.0
                    Dim denominator As Double = 0.0
                    Dim mode As Long = CLng(Math.Truncate(k_B))

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
                    pValues.Add(p_value)
                    qValues.Add(1.0)
                Next
            Next
        End Sub

        ''' <summary>
        ''' Returns true is this Gene is an ORF and if it is differentially 
        ''' expressed in at least one pair of conditions at the specified
        ''' significance level. Returns false otherwise.
        ''' </summary>
        Public Overridable Function isDifferntiallyExpressedORF(significance As Double) As Boolean
            If Not oRF Then
                Return False
            End If
            For i As Integer = 0 To qValues.Count - 1
                If qValues(i) <= significance Then
                    Return True
                End If
            Next
            Return False
        End Function



        ''' <summary>
        '''********************************************
        ''' **********   PUBLIC CLASS METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Computes the Lowess variance for each Gene in the Genome.
        ''' </summary>
        Public Shared Sub setLowessVariances(genomes As List(Of Genome), conditions As List(Of Condition))
            For x As Integer = 0 To conditions.Count - 1
                ' First of pair
                For y As Integer = 0 To conditions.Count - 1
                    ' Second of pair

                    If x = y Then
                        ' No need to compute lowess for self
                        Continue For
                    End If
                    '
                    '		if ((x > 0) && (conditions.get(x).numReplicates() > 1)) {  // We have replicates
                    '		    for (int z=0; z<genomes.size(); z++) {
                    '			Genome genome = genomes.get(z);
                    '			for (int j=0; j<genome.numGenes(); j++)
                    '			    genome.getGene(j).lowess[x][y] = genome.getGene(j).lowess[0][y];
                    '		    }		    
                    '		    continue;
                    '		}
                    '		

                    ' (x,y) lowess is NOT the same as (y,x) lowess
                    '		if ((x > y) && (conditions.get(x).numReplicates() == 1) && (conditions.get(y).numReplicates() == 1)) {  // Already computed lowess[y][x], which is same as lowess[x][y]
                    '		    for (int z=0; z<genomes.size(); z++) {
                    '			Genome genome = genomes.get(z);
                    '			for (int j=0; j<genome.numGenes(); j++)
                    '			    genome.getGene(j).lowess[x][y] = genome.getGene(j).lowess[y][x];
                    '		    }
                    '		    continue;
                    '		}
                    '		


                    Dim b As Double = 0.0
                    ' Bias correction term
                    For j As Integer = 0 To conditions(x).numReplicates() - 1
                        b += 100000.0 / CDbl(conditions(x).getReplicate(j).upperQuartile)
                    Next
                    b /= conditions(x).numReplicates()

                    ' Create list of gene expressions and list of gene variances
                    Dim expression As New List(Of Long)()
                    Dim variance As New List(Of Long)()
                    For z As Integer = 0 To genomes.Count - 1
                        Dim genome As Genome = genomes(z)
                        For j As Integer = 0 To genome.numGenes() - 1
                            expression.Add(genome.getGene(j).means(x))
                            variance.Add(genome.getGene(j).variances(x)(y))
                        Next
                    Next

                    ' Perform Lowess computation
                    Dim lowessVariance As List(Of Long) = Toolkits.RNA_Seq.Rockhopper.Lowess.lowess(expression, variance)

                    ' Uncomment to output data for Lowess graph to StdOut
                    'System.out.println("Mean" + "\t" + "Variance" + "\t" + "Lowess");
                    'for (int k=0; k<lowessVariance.size(); k++) System.out.println(expression.get(k) + "\t" + variance.get(k) + "\t" + ((long)(lowessVariance.get(k) - expression.get(k)*b)));

                    ' Assign each gene its lowess variances (after subtracting bias correction term)
                    Dim previousGenomeSizes As Integer = 0
                    For z As Integer = 0 To genomes.Count - 1
                        Dim genome As Genome = genomes(z)
                        For j As Integer = 0 To genome.numGenes() - 1
                            genome.getGene(j).lowess(x)(y) = CLng(lowessVariance(previousGenomeSizes + j) - (genome.getGene(j).getMean(x) * b))
                        Next
                        previousGenomeSizes += genome.numGenes()
                    Next
                Next
            Next
        End Sub

        ''' <summary>
        ''' Computes q-values for each gene, i.e., corrected p-values,
        ''' using Benjamini Hochberg correction.
        ''' </summary>
        Public Shared Sub correctPvalues(genomes As List(Of Genome), conditions As List(Of Condition))
            Dim totalGenes As Integer = 0
            For z As Integer = 0 To genomes.Count - 1
                totalGenes += genomes(z).numGenes()
            Next

            Dim pValue_index As Integer = 0
            For x As Integer = 0 To genomes(0).getGene(0).means.Count - 2
                ' First in pair
                For y As Integer = x + 1 To genomes(0).getGene(0).means.Count - 1
                    ' Second in pair

                    Dim indices As Integer() = New Integer(totalGenes - 1) {}
                    Dim genomeIndices As Integer() = New Integer(totalGenes - 1) {}
                    Dim pvalues As Double() = New Double(totalGenes - 1) {}
                    Dim previousGenomeSizes As Integer = 0
                    For z As Integer = 0 To genomes.Count - 1
                        Dim genome As Genome = genomes(z)
                        For j As Integer = 0 To genome.numGenes() - 1
                            pvalues(previousGenomeSizes + j) = genome.getGene(j).pValues(pValue_index)
                            indices(previousGenomeSizes + j) = j
                            genomeIndices(previousGenomeSizes + j) = z

                            ' Check if there is too little expression to compute a p-value
                            Dim g As Gene = genome.getGene(j)
                            Dim e1 As Double = g.means(x)
                            Dim e2 As Double = g.means(y)
                            If g.oRF Then
                                ' ORF
                                e1 /= Math.Max(g._start, g._stop) - Math.Min(g._start, g._stop) + 1
                                e2 /= Math.Max(g._start, g._stop) - Math.Min(g._start, g._stop) + 1
                            Else
                                ' RNA
                                e1 /= Math.Max(g.tStart, g.tStop) - Math.Min(g.tStart, g.tStop) + 1
                                e2 /= Math.Max(g.tStart, g.tStop) - Math.Min(g.tStart, g.tStop) + 1
                            End If
                            If (e1 < conditions(x).minDiffExpressionLevel) AndAlso (e2 < conditions(y).minDiffExpressionLevel) Then
                                pvalues(previousGenomeSizes + j) = 1.0
                            End If
                        Next
                        previousGenomeSizes += genome.numGenes()
                    Next
                    mergesort(pvalues, indices, genomeIndices, 0, totalGenes - 1)
                    Dim previous_BH_value As Double = 0.0
                    For k As Integer = 0 To pvalues.Length - 1
                        Dim BH_value As Double = pvalues(k) * totalGenes / (k + 1)
                        BH_value = Math.Min(BH_value, 1.0)
                        ' Disallow values greater than 1
                        ' To preserve monotonicity, we take maximum to ensure we don't
                        ' generate a value less than the previous value.
                        BH_value = Math.Max(BH_value, previous_BH_value)
                        previous_BH_value = BH_value
                        genomes(genomeIndices(k)).getGene(indices(k)).qValues(pValue_index) = BH_value
                    Next
                    pValue_index += 1
                Next
            Next
        End Sub



        ''' <summary>
        '''*********************************************
        ''' **********   PRIVATE CLASS METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Mergesort parallel arrays "a" and "b" and "c" based on values in "a"
        ''' </summary>
        Private Shared Sub mergesort(a As Double(), b As Integer(), c As Integer(), lo As Integer, hi As Integer)
            If lo < hi Then
                Dim q As Integer = (lo + hi) \ 2
                mergesort(a, b, c, lo, q)
                mergesort(a, b, c, q + 1, hi)
                merge(a, b, c, lo, q, hi)
            End If
        End Sub

        ''' <summary>
        ''' Mergesort helper method
        ''' </summary>
        Private Shared Sub merge(a As Double(), b As Integer(), c As Integer(), lo As Integer, q As Integer, hi As Integer)
            Dim a1 As Double() = New Double(q - lo) {}
            Dim a2 As Double() = New Double(hi - q - 1) {}
            Dim b1 As Integer() = New Integer(q - lo) {}
            Dim b2 As Integer() = New Integer(hi - q - 1) {}
            Dim c1 As Integer() = New Integer(q - lo) {}
            Dim c2 As Integer() = New Integer(hi - q - 1) {}
            Dim i As Integer = 0
            Dim j As Integer = 0

            For i = 0 To a1.Length - 1
                a1(i) = a(lo + i)
                b1(i) = b(lo + i)
                c1(i) = c(lo + i)
            Next
            For j = 0 To a2.Length - 1
                a2(j) = a(q + 1 + j)
                b2(j) = b(q + 1 + j)
                c2(j) = c(q + 1 + j)
            Next
            i = 0
            ' Index for first half arrays
            j = 0
            ' Index for second half arrays
            For k As Integer = lo To hi
                If i >= a1.Length Then
                    a(k) = a2(j)
                    b(k) = b2(j)
                    c(k) = c2(j)
                    j += 1
                ElseIf j >= a2.Length Then
                    a(k) = a1(i)
                    b(k) = b1(i)
                    c(k) = c1(i)
                    i += 1
                ElseIf a1(i) <= a2(j) Then
                    a(k) = a1(i)
                    b(k) = b1(i)
                    c(k) = c1(i)
                    i += 1
                Else
                    a(k) = a2(j)
                    b(k) = b2(j)
                    c(k) = c2(j)
                    j += 1
                End If
            Next
        End Sub

    End Class

End Namespace
