#Region "Microsoft.VisualBasic::95c7c1c222fcd8f9286f88ae72790194, RNA-Seq\Rockhopper\Java\ObjectModels\Operons.vb"

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

    '     Class Operons
    ' 
    '         Properties: operonPrior
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: computeProbabilityOfSameTranscript, getCorrelation, getIGlength, getNumOperonGenePairs, getPercentIGexpressed
    '                   isGenePairAnOperon, operonExpression, outputMergedOperons, readInGenes
    ' 
    '         Sub: determineOperonCorrelationDistributions, determineOperonLengthDistributions, Main, outputGenePairOperons
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

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

    Public Class Operons

        ''' <summary>
        '''******************************************
        ''' **********   INSTANCE VARIABLES   **********
        ''' </summary>

        Private lengthSame As SmoothDistribution
        Private lengthOpp As SmoothDistribution
        Private corrSame As SmoothDistribution
        Private corrOpp As SmoothDistribution
        Private _operonPrior As Double
        Private nonOperonPrior As Double
        Private operonIGexpression As Double()
        ' Distribution of IG expression for operons
        Private nonOperonIGexpression As Double()
        ' Distribution of IG expression for non-operons
        Private p_values As List(Of Double)



        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        Public Sub New(geneFileName As String)
            Me.New(readInGenes(geneFileName), readInGenes(geneFileName))
        End Sub

        Public Sub New(codingGenes As List(Of Gene), genes As List(Of Gene))
            operonPrior = codingGenes
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   PUBLIC INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Returns the number of gene pairs predicted to be co-transcribed
        ''' as part of an operon.
        ''' </summary>
        Public Overridable Function getNumOperonGenePairs(genes As List(Of Gene)) As Integer
            Dim numOperonGenePairs As Integer = 0
            For i As Integer = 1 To genes.Count - 1
                If isGenePairAnOperon(genes, i - 1, i) Then
                    numOperonGenePairs += 1
                End If
            Next
            Return numOperonGenePairs
        End Function

        ''' <summary>
        ''' Returns true if the two genes at the specified indices, which must be consecutive, 
        ''' are predicted to be co-transcribed. Returns false otherwise.
        ''' </summary>
        Public Overridable Function isGenePairAnOperon(genes As List(Of Gene), x As Integer, y As Integer) As Boolean
            If x <> y - 1 Then
                Output("Error - two indices must be consecutive." & vbLf)
                Return False
            End If
            If genes(x).strand <> genes(y).strand Then
                ' Genes on different strands
                Return False
            End If
            Dim length As Integer = getIGlength(genes(x), genes(y))
            If (length < 40) OrElse ((length < 100) AndAlso (getCorrelation(genes(x), genes(y)) >= 0.5)) Then
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Output to file gene-pairs predicted to be part of the same operon.
        ''' </summary>
        Public Overridable Sub outputGenePairOperons(operonOutputFile As String, genes As List(Of Gene))
            Try
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(operonOutputFile))
                writer.println("Transcription Start" & vbTab & "Translation Start" & vbTab & "Translation Stop" & vbTab & "Transcription Stop" & vbTab & "Strand" & vbTab & "Name" & vbTab & "Synonym" & vbTab & "Product" & vbTab & "Transcription Start" & vbTab & "Translation Start" & vbTab & "Translation Stop" & vbTab & "Transcription Stop" & vbTab & "Strand" & vbTab & "Name" & vbTab & "Synonym" & vbTab & "Product" & vbTab & "Predicted Polycistronic?")
                For i As Integer = 1 To genes.Count - 1
                    Dim length As Integer = getIGlength(genes(i - 1), genes(i))
                    writer.print(genes(i - 1).ToString() & vbTab & genes(i).ToString())
                    If genes(i - 1).strand <> genes(i).strand Then
                        writer.print(vbLf)
                    ElseIf length < 40 Then
                        writer.print(vbTab & "YES" & vbLf)
                    ElseIf length >= 100 Then
                        writer.print(vbLf)
                    ElseIf getCorrelation(genes(i - 1), genes(i)) >= 0.5 Then
                        writer.print(vbTab & "YES" & vbLf)
                    Else
                        writer.print(vbLf)
                    End If
                Next
                writer.close()
            Catch e As FileNotFoundException
                Output(vbLf & "Error - could not open file " & operonOutputFile & vbLf & vbLf)
                Environment.[Exit](0)
            End Try
        End Sub

        ''' <summary>
        ''' Output to file merged operons.
        ''' Return the number of predicted merged operons.
        ''' </summary>
        Public Overridable Function outputMergedOperons(operonOutputFile As String, browserOutputFile As String, genomeName As String, genes As List(Of Gene), size As Integer) As Integer
            Dim numMergedOperons As Integer = 0
            Dim operonCoordinates As New List(Of Integer)(size)
            Try
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(operonOutputFile))
                writer.println("Start" & vbTab & "Stop" & vbTab & "Strand" & vbTab & "Number of Genes" & vbTab & "Genes")
                Dim genesInOperon As New List(Of String)()
                Dim start As Integer = -1
                Dim [stop] As Integer = -1
                Dim strand As Char = "?"c
                For i As Integer = 1 To genes.Count - 1
                    If isGenePairAnOperon(genes, i - 1, i) Then
                        ' Operon
                        If start = -1 Then
                            ' Start of new operon
                            start = Math.Min(genes(i - 1).first, genes(i).first)
                            [stop] = Math.Max(genes(i - 1).last, genes(i).last)
                            strand = genes(i).strand
                            genesInOperon.Clear()
                            genesInOperon.Add(genes(i - 1).name)
                            genesInOperon.Add(genes(i).name)
                            ' Within operon
                        ElseIf strand = genes(i).strand Then
                            start = Math.Min(start, genes(i).first)
                            [stop] = Math.Max([stop], genes(i).last)
                            genesInOperon.Add(genes(i).name)
                        Else
                            ' New operon
                            writer.print(start & vbTab & [stop] & vbTab & strand & vbTab & genesInOperon.Count & vbTab)
                            For z As Integer = 0 To genesInOperon.Count - 1
                                If z = 0 Then
                                    writer.print(genesInOperon(z))
                                Else
                                    writer.print(", " & genesInOperon(z))
                                End If
                            Next
                            writer.println()
                            numMergedOperons += 1
                            For j As Integer = start To [stop]
                                ' Keep track of multi-gene operon coords
                                While operonCoordinates.Count <= [stop]
                                    operonCoordinates.Add(0)
                                End While
                                If strand = "+"c Then
                                    operonCoordinates(j) = 1
                                End If
                                If strand = "-"c Then
                                    operonCoordinates(j) = -1
                                End If
                            Next
                            start = -1
                            [stop] = -1
                            strand = "?"c
                            genesInOperon.Clear()
                            genesInOperon.Add(genes(i - 1).name)
                            genesInOperon.Add(genes(i).name)
                        End If
                    Else
                        ' Non-operon
                        If start >= 0 Then
                            ' End of operon
                            writer.print(start & vbTab & [stop] & vbTab & strand & vbTab & genesInOperon.Count & vbTab)
                            For z As Integer = 0 To genesInOperon.Count - 1
                                If z = 0 Then
                                    writer.print(genesInOperon(z))
                                Else
                                    writer.print(", " & genesInOperon(z))
                                End If
                            Next
                            writer.println()
                            numMergedOperons += 1
                            For j As Integer = start To [stop]
                                ' Keep track of multi-gene operon coords
                                While operonCoordinates.Count <= [stop]
                                    operonCoordinates.Add(0)
                                End While
                                If strand = "+"c Then
                                    operonCoordinates(j) = 1
                                End If
                                If strand = "-"c Then
                                    operonCoordinates(j) = -1
                                End If
                            Next
                            start = -1
                            [stop] = -1
                            strand = "?"c
                            genesInOperon.Clear()
                        Else
                            ' Do nothing.
                            ' Within non-operon
                        End If
                    End If
                Next
                If start >= 0 Then
                    ' Include final operon
                    writer.print(start & vbTab & [stop] & vbTab & strand & vbTab & genesInOperon.Count & vbTab)
                    For z As Integer = 0 To genesInOperon.Count - 1
                        If z = 0 Then
                            writer.print(genesInOperon(z))
                        Else
                            writer.print(", " & genesInOperon(z))
                        End If
                    Next
                    writer.println()
                    numMergedOperons += 1
                    For j As Integer = start To [stop]
                        ' Keep track of multi-gene operon coords
                        While operonCoordinates.Count <= [stop]
                            operonCoordinates.Add(0)
                        End While
                        If strand = "+"c Then
                            operonCoordinates(j) = 1
                        End If
                        If strand = "-"c Then
                            operonCoordinates(j) = -1
                        End If
                    Next
                    start = -1
                    [stop] = -1
                    strand = "?"c
                    genesInOperon.Clear()
                End If
                writer.close()
            Catch e As FileNotFoundException
                Output(vbLf & "Error - could not open file " & operonOutputFile & vbLf & vbLf)
                Environment.[Exit](0)
            End Try

            ' Output genome browser file with multi-gene operon information
            Try
                Dim browserWriter As New PrintWriter(New Oracle.Java.IO.File(browserOutputFile))
                browserWriter.println("track name=" & """" & "Multi-gene Operons" & """" & " color=255,0,255 altColor=255,0,255 graphType=bar viewLimits=-1:1")
                browserWriter.println("fixedStep chrom=" & genomeName & " start=1 step=1")
                For j As Integer = 1 To operonCoordinates.Count - 1
                    browserWriter.println(operonCoordinates(j))
                Next
                browserWriter.close()
            Catch e As FileNotFoundException
                Output(vbLf & "Error - could not open file " & browserOutputFile & vbLf & vbLf)
                Environment.[Exit](0)
            End Try

            Return numMergedOperons
        End Function



        ''' <summary>
        '''************************************************
        ''' **********   PRIVATE INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Determine the distribution of lengths between gene pairs.
        ''' One distribution for consecutive genes on the same strand and
        ''' one distribution for consecutive genes on the opposite strand.
        ''' </summary>
        Private Sub determineOperonLengthDistributions(genes As List(Of Gene))
            Dim length_same As New List(Of Integer)()
            Dim length_opp As New List(Of Integer)()
            For i As Integer = 1 To genes.Count - 1
                Dim length As Integer = Math.Min(genes(i).start, genes(i).[stop]) - Math.Max(genes(i - 1).start, genes(i - 1).[stop]) - 1
                If genes(i).strand = genes(i - 1).strand Then
                    length_same.Add(length)
                Else
                    length_opp.Add(length)
                End If
            Next

            ' Uncomment the below lines to output operon length distributions
            '
            '	lengthSame = new SmoothDistribution(length_same, 30.0, 10, -100, 300);
            '	lengthOpp = new SmoothDistribution(length_opp, 30.0, 10, -100, 300);
            '	output(lengthSame.toString() + "\n" + lengthOpp.toString() + "\n");
            '	


            lengthSame = New SmoothDistribution(length_same, 30.0, 1, -50, 200)
            lengthOpp = New SmoothDistribution(length_opp, 30.0, 1)
            lengthSame.pseudocount = 0.0
        End Sub

        ''' <summary>
        ''' Determine the distribution of correlations between gene pairs.
        ''' One distribution for consecutive genes on the same strand and
        ''' one distribution for consecutive genes on the opposite strand.
        ''' </summary>
        Private Sub determineOperonCorrelationDistributions(genes As List(Of Gene))
            Dim corr_same As New List(Of Integer)()
            Dim corr_opp As New List(Of Integer)()
            For i As Integer = 1 To genes.Count - 1
                Dim corr As Double = getCorrelation(genes(i - 1), genes(i))
                If genes(i).strand = genes(i - 1).strand Then
                    corr_same.Add(CInt(Math.Truncate(corr * 20)))
                Else
                    corr_opp.Add(CInt(Math.Truncate(corr * 20)))
                End If
            Next

            ' Uncomment the below lines to output operon correlation distributions
            '
            '	corrSame = new SmoothDistribution(corr_same, 5.0, 1, -20, 20);
            '	corrOpp = new SmoothDistribution(corr_opp, 5.0, 1, -20, 20);
            '	output(corrSame.toString() + "\n" + corrOpp.toString() + "\n");
            '	


            corrSame = New SmoothDistribution(corr_same, 5.0, 1, -20, 20)
            corrOpp = New SmoothDistribution(corr_opp, 5.0, 1, -20, 20)
            corrSame.pseudocount = 0.0
        End Sub

        Private Function getCorrelation(g1 As Gene, g2 As Gene) As Double
            Dim e1 As New List(Of Long)()
            Dim e2 As New List(Of Long)()
            For i As Integer = 0 To numConditions - 1
                e1.Add(g1.getAvg(i))
                e2.Add(g2.getAvg(i))
            Next
            Return Misc.correlation(e1, e2)
        End Function

        ''' <summary>
        ''' Set prior probability of same-strand gene pair being an operon
        ''' (based on section 2.2 of Westover 2005).
        ''' </summary>
        Private WriteOnly Property operonPrior() As List(Of Gene)
            Set
                Dim numberDirectons As Integer = 0
                ' Two or more consecutive value on the same strand
                Dim numberPairs As Integer = 0
                ' Pair of consecutive value on the same strand
                Dim previousStrand As Char = "?"c
                Dim currentDirectonLength As Integer = 1
                ' Length of current directon under consideration
                For i As Integer = 0 To Value.Count - 1
                    Dim g As Gene = Value(i)
                    If (g.strand <> previousStrand) AndAlso (currentDirectonLength > 1) Then
                        numberDirectons += 1
                        previousStrand = g.strand
                        currentDirectonLength = 1
                    ElseIf (g.strand <> previousStrand) AndAlso (currentDirectonLength = 1) Then
                        previousStrand = g.strand
                        currentDirectonLength = 1
                    ElseIf (g.strand = previousStrand) AndAlso (currentDirectonLength > 1) Then
                        numberPairs += 1
                        previousStrand = g.strand
                        currentDirectonLength += 1
                    ElseIf (g.strand = previousStrand) AndAlso (currentDirectonLength = 1) Then
                        numberPairs += 1
                        previousStrand = g.strand
                        currentDirectonLength += 1
                        ' Do nothing
                    Else
                    End If
                Next
                If currentDirectonLength > 1 Then
                    numberDirectons += 1
                End If
                _operonPrior = 1.0 - numberDirectons / CDbl(numberPairs)
                'output("Prior probability of operon:\t" + operonPrior + "\n");  // Output operon prior
                nonOperonPrior = 1.0 - _operonPrior
            End Set
        End Property

        ''' <summary>
        ''' Compute probability that two consecutive genes are co-transcribed.
        ''' Return a list of p-values for all gene pairs.
        ''' </summary>
        Private Function operonExpression(genes As List(Of Gene)) As List(Of Double)
            Dim p_values As New List(Of Double)(genes.Count)
            For i As Integer = 0 To genes.Count - 1
                p_values.Add(Double.MaxValue)
            Next
            p_values(0) = 0.0
            Dim all_p_values As New List(Of List(Of Double))()
            For i As Integer = 0 To genes.Count - 1
                all_p_values.Add(New List(Of Double)())
            Next
            For i As Integer = 0 To numConditions - 1
                Dim means As New List(Of Double)(genes.Count)
                Dim variances As New List(Of Double)(genes.Count)
                For z As Integer = 0 To genes.Count - 1
                    means.Add(0.0)
                    variances.Add(0.0)
                Next
                For z As Integer = 1 To genes.Count - 1
                    Dim numReplicates As Integer = genes(z).getNumReplicates(i)

                    ' Compute mean expression in each condition.
                    For j As Integer = 0 To numReplicates - 1
                        means(z) = means(z) + 1000.0 * genes(z).getNormalizedCount(i, j) / (Math.Abs(genes(z).[stop] - genes(z).start) + 1)
                    Next
                    means(z) = means(z) / numReplicates

                    ' Compute variance.
                    Dim varianceAdjustment As Double = 1.15
                    ' If we have NO replicates, then we use neighboring genes.
                    If numReplicates = 1 Then
                        ' We have no replicates. Use neighboring gene
                        Dim mean As Double = (1000.0 * genes(z - 1).getNormalizedCount(i, 0) / (Math.Abs(genes(z - 1).[stop] - genes(z - 1).start) - 1) + 1000.0 * genes(z).getNormalizedCount(i, 0) / (Math.Abs(genes(z).[stop] - genes(z).start) + 1)) / 2.0
                        variances(z) = Math.Pow(1000.0 * genes(z - 1).getNormalizedCount(i, 0) / (Math.Abs(genes(z - 1).[stop] - genes(z - 1).start) + 1) - mean, 2.0) + Math.Pow(1000.0 * genes(z).getNormalizedCount(i, 0) / (Math.Abs(genes(z).[stop] - genes(z).start) + 1) - mean, 2.0)
                        variances(z) = variances(z) / (2 - 1)
                        variances(z) = Math.Pow(variances(z), varianceAdjustment)
                    End If

                    ' Compute variance.
                    ' If we DO have replicates, then we use the replicates.
                    If numReplicates > 1 Then
                        ' We have replicates. Use them
                        For j As Integer = 0 To numReplicates - 1
                            variances(z) = Math.Pow(1000.0 * genes(z).getNormalizedCount(i, j) / (Math.Abs(genes(z).[stop] - genes(z).start) + 1) - means(z), 2.0)
                        Next
                        variances(z) = variances(z) / (numReplicates - 1)
                        variances(z) = Math.Pow(variances(z), varianceAdjustment)
                    End If
                Next

                ' Generate Lowess variances
                means(0) = means(1)
                variances(0) = variances(1)
                Dim means_Long As New List(Of Long)(means.Count)
                Dim variances_Long As New List(Of Long)(variances.Count)
                For w As Integer = 0 To means.Count - 1
                    means_Long.Add(CLng(Math.Truncate(CDbl(means(w)))))
                    variances_Long.Add(CLng(Math.Truncate(CDbl(variances(w)))))
                Next
                Dim lowessVariance As List(Of Long) = Lowess.lowess(means_Long, variances_Long)

                ' Determine operon probabilities
                For z As Integer = 1 To genes.Count - 1
                    Dim p_value As Double = computeProbabilityOfSameTranscript(genes(z - 1), genes(z), CDbl(lowessVariance(z - 1)), CDbl(lowessVariance(z)), i)
                    p_values(z) = Math.Min(CDbl(p_values(z)), p_value)
                    all_p_values(z).Add(p_value)
                Next
            Next
            For z As Integer = 1 To genes.Count - 1
                Dim avg As Double = 0.0
                For y As Integer = 0 To all_p_values(z).Count - 1
                    avg += all_p_values(z)(y)
                Next
                avg /= all_p_values(z).Count
                p_values(z) = avg
            Next
            Return p_values
        End Function



        ''' <summary>
        '''*********************************************
        ''' **********   PRIVATE CLASS METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Reads in a file of genes (either *.ptt or *.rnt) and returns
        ''' an ArrayList of gene objects.
        ''' </summary>
        Private Shared Function readInGenes(fileName As String) As List(Of Gene)
            Dim listOfGenes As New List(Of Gene)()
            Try
                Dim reader As New Scanner(New Oracle.Java.IO.File(fileName))
                For i As Integer = 0 To 2
                    ' Ignore 3 header lines
                    reader.nextLine()
                Next
                While reader.hasNext()
                    ' Continue until end of file
                    ' Create new gene
                    listOfGenes.Add(New Gene(reader.nextLine(), "ORF"))
                End While
                reader.close()
            Catch e As FileNotFoundException
                Output("Error - the file " & fileName & " could not be found and opened." & vbLf)
            End Try
            Return listOfGenes
        End Function

        ''' <summary>
        ''' Returns the probability, based on expression in the given condition "i", that "g1"
        ''' is NOT differentially expressed from "g2".
        ''' I.e., return the probability that "g1" is part of the same polycistronic transcript
        ''' as "g2" in the specified condition "i".
        ''' </summary>
        Private Shared Function computeProbabilityOfSameTranscript(g1 As Gene, g2 As Gene, lowessVar1 As Double, lowessVar2 As Double, i As Integer) As Double
            Dim numReplicates As Integer = g1.getNumReplicates(i)

            Dim k_A As Double = 0.0
            Dim k_B As Double = 0.0
            For j As Integer = 0 To numReplicates - 1
                k_A += 1000.0 * g1.getNormalizedCount(i, j) / (Math.Abs(g1.[stop] - g1.start) + 1)
                k_B += 1000.0 * g2.getNormalizedCount(i, j) / (Math.Abs(g2.[stop] - g2.start) + 1)
            Next
            Dim q As Double = k_A + k_B

            Dim mean_A As Double = q
            Dim mean_B As Double = q
            Dim variance_A As Double = lowessVar1
            Dim variance_B As Double = lowessVar2
            Dim p_a As Double = mean_A / variance_A
            Dim p_b As Double = mean_B / variance_B
            Dim r_a As Double = Math.Max(mean_A * mean_A / (variance_A - mean_A), 1.0)
            ' r should never be below 1
            Dim r_b As Double = Math.Max(mean_B * mean_B / (variance_B - mean_B), 1.0)
            ' r should never be below 1
            If (p_a < 0.0) OrElse (p_b < 0.0) OrElse (p_a > 1.0) OrElse (p_b > 1.0) OrElse (variance_A = 0.0) OrElse (variance_B = 0.0) Then
                Return 0.0
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
            ' Number of times we decrement by 1 (raising alpha raises precision but slows down computation)
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
            Return p_value
        End Function

        ''' <summary>
        ''' Returns the number of nucleotides in the IG region between the
        ''' two genes. g1 precede g2. May return a negative number if the
        ''' genes overlap.
        ''' </summary>
        Private Shared Function getIGlength(g1 As Gene, g2 As Gene) As Integer
            Dim IG_start As Integer = -1
            If g1.oRF Then
                IG_start = Math.Max(g1.start, g1.[stop]) + 1
            Else
                IG_start = Math.Max(g1.startT, g1.stopT) + 1
            End If
            Dim IG_stop As Integer = -1
            If g2.oRF Then
                IG_stop = Math.Min(g2.start, g2.[stop]) - 1
            Else
                IG_stop = Math.Min(g2.startT, g2.stopT) - 1
            End If
            Return IG_stop - IG_start + 1
        End Function

        ''' <summary>
        ''' Returns the percentage of nucleotides in the IG region
        ''' that are expressed, i.e., that correspond to predicted
        ''' UTRs.
        ''' </summary>
        Private Shared Function getPercentIGexpressed(g1 As Gene, g2 As Gene) As Double
            If Not g1.oRF OrElse Not g2.oRF Then
                ' Only consider two ORFs
                Return 0.0
            End If
            Dim IG_length As Integer = getIGlength(g1, g2)
            If IG_length <= 0 Then
                ' Only consider IGs with non-zero length
                Return 0.0
            End If

            Dim UTR1 As Integer = 0
            If (g1.strand = "+"c) AndAlso (g1.stopT > 0) Then
                UTR1 = g1.stopT - g1.[stop]
            End If
            If (g1.strand = "-"c) AndAlso (g1.startT > 0) Then
                UTR1 = g1.startT - g1.start
            End If
            Dim UTR2 As Integer = 0
            If (g2.strand = "+"c) AndAlso (g2.startT > 0) Then
                UTR2 = g2.start - g2.startT
            End If
            If (g2.strand = "-"c) AndAlso (g2.stopT > 0) Then
                UTR2 = g2.[stop] - g2.stopT
            End If
            Dim UTR_length As Integer = UTR1 + UTR2
            Return Math.Min(UTR_length / CDbl(IG_length), 1.0)
        End Function



        ''' <summary>
        '''***********************************
        ''' **********   MAIN METHOD   **********
        ''' </summary>

        ''' <summary>
        ''' The Main method, when invoked with the name of a gene file (*.ptt)
        ''' as a command line argument, computes the distribution of lengths between
        ''' consecutive genes on the same strand and the distribution of lengths
        ''' between consecutive genes on the opposite strand. Output is to same.dist
        ''' and opp.dist.
        ''' </summary>
        Private Shared Sub Main(args As String())
            If args.Length < 1 Then
                Oracle.Java.System.Err.println(vbLf & "The Operons application requires one command line argument, the name of a gene file (*.ptt). The application computes the distribution of lengths between consecutive genes on the same strand and the distribution of lengths between consecutive genes on the opposite strand. Output is to same.dist and opp.dist.")
                Oracle.Java.System.Err.println(vbLf & vbTab & "java Operons NC_******.ptt" & vbLf)
                Environment.[Exit](0)
            End If

            Dim ops As New Operons(args(0))

        End Sub

    End Class
End Namespace
