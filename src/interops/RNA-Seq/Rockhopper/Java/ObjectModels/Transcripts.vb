#Region "Microsoft.VisualBasic::a3adad305cf31cd2e217c6348cbd240c, RNA-Seq\Rockhopper\Java\ObjectModels\Transcripts.vb"

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

    '     Class Transcripts
    ' 
    '         Properties: num3UTRs, num5UTRs, numAntisenseRNAs, numSenseRNAs
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getAnnotationOfIG, getAntisenseAnnotation, getUTR_length
    ' 
    '         Sub: identifyNovelTranscriptsInIG, identifyRNAs, identifyUTRs, merge_RNAs, output_IG_to_file
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
    ''' The Transcript class identifies UTRs of genes as well
    ''' ncRNA transcripts based on RNA-seq data.
    ''' </summary>
    Public Class Transcripts

        ''' <summary>
        '''***************************************
        ''' **********   CLASS VARIABLES   **********
        ''' </summary>


        '''<summary>Number Of nucleotides In sliding window</summary>
        Public Const WINDOW As Integer = 10

#Region "**********   INSTANCE VARIABLES   **********"

        Dim genome As Genome
        ''' <summary>
        ''' Index of genome in list of all genomes
        ''' </summary>
        Dim z As Integer
        Dim conditions As List(Of Condition)
        ''' <summary>
        ''' Is RNA-seq data strand specific or ambiguous?
        ''' </summary>
        Dim unstranded As Boolean
        ''' <summary>
        ''' Number of 5'UTRs
        ''' </summary>
        Dim _num5UTRs As Integer = 0
        ''' <summary>
        ''' Number of 3'UTRs
        ''' </summary>
        Dim _num3UTRs As Integer = 0
        ''' <summary>
        ''' Number of sense ncRNAs
        ''' </summary>
        Dim _numSenseRNAs As Integer = 0
        ''' <summary>
        ''' Number of antisense ncRNAs
        ''' </summary>
        Dim _numAntisenseRNAs As Integer = 0
        ' 
        'public long[] distribution = new long[201];  // Distribution of expressed gene reads
#End Region

        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        Public Sub New(z As Integer, genome As Genome, conditions As List(Of Condition), unstranded As Boolean)
            Me.z = z
            Me.genome = genome
            Me.conditions = conditions
            Me.unstranded = unstranded
        End Sub

        ''' <summary>
        '''***********************************************
        ''' **********   PUBLIC INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Returns the number of 5'UTRs.
        ''' </summary>
        Public Overridable ReadOnly Property num5UTRs() As Integer
            Get
                Return Me._num5UTRs
            End Get
        End Property

        ''' <summary>
        ''' Returns the number of 3'UTRs.
        ''' </summary>
        Public Overridable ReadOnly Property num3UTRs() As Integer
            Get
                Return Me._num3UTRs
            End Get
        End Property

        ''' <summary>
        ''' Returns the number of sense ncRNAs.
        ''' </summary>
        Public Overridable ReadOnly Property numSenseRNAs() As Integer
            Get
                Return Me._numSenseRNAs
            End Get
        End Property

        ''' <summary>
        ''' Returns the number of antisense ncRNAs.
        ''' </summary>
        Public Overridable ReadOnly Property numAntisenseRNAs() As Integer
            Get
                Return Me._numAntisenseRNAs
            End Get
        End Property

        ''' <summary>
        ''' For each gene, identify its 5'UTR and 3'UTR based on
        ''' the expression data.
        ''' </summary>
        Public Overridable Sub identifyUTRs()
            For x As Integer = 0 To genome.numGenes()
                ' For each IG region
                Dim g1 As Gene = Nothing
                Dim g2 As Gene = Nothing

                ' Identify downstream gene and IG stop coordinate
                If x = genome.numGenes() Then
                    g2 = New Gene((genome.size() - 1) & ".." & (genome.size() - 1) & vbTab & genome.getGene(x - 1).strand & vbTab & "0" & vbTab & "-" & vbTab & "???" & vbTab & "???" & vbTab & "-" & vbTab & "-" & vbTab & "???", "ORF")
                Else
                    g2 = genome.getGene(x)
                End If
                Dim [stop] As Integer = Math.Min(g2.start, g2.[stop]) - 1
                If Not g2.oRF Then
                    [stop] = g2.minCoordinate - 1
                End If

                ' Identify upstream gene and IG start coordinate
                Dim start As Integer = 1
                Dim upstreamGeneIndex As Integer = x - 1
                While upstreamGeneIndex >= 0
                    If unstranded Then
                        ' Strand ambiguous
                        start = Math.Max(genome.getGene(upstreamGeneIndex).start, genome.getGene(upstreamGeneIndex).[stop]) + 1
                        If Not genome.getGene(upstreamGeneIndex).oRF Then
                            start = genome.getGene(upstreamGeneIndex).maxCoordinate + 1
                        End If
                        Exit While
                    Else
                        ' Strand specific
                        If g2.strand = genome.getGene(upstreamGeneIndex).strand Then
                            start = Math.Max(genome.getGene(upstreamGeneIndex).start, genome.getGene(upstreamGeneIndex).[stop]) + 1
                            If Not genome.getGene(upstreamGeneIndex).oRF Then
                                start = genome.getGene(upstreamGeneIndex).maxCoordinate + 1
                            End If
                            Exit While
                        End If
                        upstreamGeneIndex -= 1
                    End If
                End While
                If upstreamGeneIndex < 0 Then
                    g1 = New Gene(1 & ".." & 1 & vbTab & g2.strand & vbTab & "0" & vbTab & "-" & vbTab & "???" & vbTab & "???" & vbTab & "-" & vbTab & "-" & vbTab & "???", "ORF")
                Else
                    g1 = genome.getGene(upstreamGeneIndex)
                End If

                Dim IG_length As Integer = [stop] - start + 1
                If IG_length <= 0 Then
                    ' No action necessary if there is no IG region.
                    Continue For
                End If

                Dim frontUTR_length_merged As Integer = -1
                Dim backUTR_length_merged As Integer = -1
                For i As Integer = 0 To conditions.Count - 1
                    For j As Integer = 0 To conditions(i).numReplicates() - 1

                        Dim r As Replicate = conditions(i).getReplicate(j)

                        ' Get UTR lengths
                        Dim frontUTR_length As Integer = getUTR_length(g1, start, [stop], r, True)
                        Dim backUTR_length As Integer = getUTR_length(g2, start, [stop], r, False)

                        ' Distinguish overlapping UTRs
                        If (frontUTR_length > 0) AndAlso (backUTR_length > 0) AndAlso (Math.Max(g1.start, g1.[stop]) + frontUTR_length >= Math.Min(g2.start, g2.[stop]) - backUTR_length) Then

                            ' Determine mean of genes' expression
                            Dim mean1 As Double = r.getMeanOfRange(z, g1.start, g1.[stop], g1.strand)
                            Dim mean2 As Double = r.getMeanOfRange(z, g2.start, g2.[stop], g2.strand)
                            If unstranded Then
                                mean1 = r.getMeanOfRange(z, g1.start, g1.[stop], "?"c)
                                mean2 = r.getMeanOfRange(z, g2.start, g2.[stop], "?"c)
                            End If

                            Dim overlapStart As Integer = Math.Min(g2.start, g2.[stop]) - backUTR_length
                            Dim overlapStop As Integer = Math.Max(g1.start, g1.[stop]) + frontUTR_length
                            'while ((overlapStart < Math.min(g2.getStart(), g2.getStop())) && (getPoissonPDF(r.getReads(overlapStart, g1.getStrand()), mean1) > getPoissonPDF(r.getReads(overlapStart, g2.getStrand()), mean2))) overlapStart++;
                            If Not unstranded Then
                                ' Strand specific
                                While (overlapStart <= overlapStop) AndAlso (Math.Abs(r.getReads(z, overlapStart, g1.strand) - mean1) < Math.Abs(r.getReads(z, overlapStart, g2.strand) - mean2))
                                    overlapStart += 1
                                End While
                                'while ((overlapStop > Math.max(g1.getStart(), g1.getStop())) && (getPoissonPDF(r.getReads(overlapStop, g1.getStrand()), mean1) < getPoissonPDF(r.getReads(overlapStop, g2.getStrand()), mean2))) overlapStop--;
                                While (overlapStop >= overlapStart) AndAlso (Math.Abs(r.getReads(z, overlapStop, g1.strand) - mean1) > Math.Abs(r.getReads(z, overlapStop, g2.strand) - mean2))
                                    overlapStop -= 1
                                End While
                            Else
                                ' Strand ambiguous
                                While (overlapStart <= overlapStop) AndAlso (Math.Abs(r.getReads(z, overlapStart, "?"c) - mean1) < Math.Abs(r.getReads(z, overlapStart, "?"c) - mean2))
                                    overlapStart += 1
                                End While
                                While (overlapStop >= overlapStart) AndAlso (Math.Abs(r.getReads(z, overlapStop, "?"c) - mean1) > Math.Abs(r.getReads(z, overlapStop, "?"c) - mean2))
                                    overlapStop -= 1
                                End While
                            End If
                            If overlapStop - overlapStart < -1 Then
                                Output("Error - overlapping UTR region has invalid size." & vbLf)
                                ' Do nothing. There is no overlap.
                            ElseIf overlapStop - overlapStart = -1 Then
                            Else
                                ' We still have overlapping UTR regions
                                Dim mean_of_overlap As Double = r.getMeanOfRange(z, overlapStart, overlapStop, g1.strand)
                                If unstranded Then
                                    mean_of_overlap = r.getMeanOfRange(z, overlapStart, overlapStop, "?"c)
                                End If
                                If Math.Abs(mean_of_overlap - mean1) <= Math.Abs(mean_of_overlap - mean2) Then
                                    overlapStart = overlapStop + 1
                                Else
                                    overlapStop = overlapStart - 1
                                    'double prob1 = 1.0;
                                    'double prob2 = 1.0;
                                    'for (int y=overlapStart; y<=overlapStop; y++) {
                                    'prob1 *= getPoissonPDF(r.getReads(y, g1.getStrand()), mean1);
                                    'prob2 *= getPoissonPDF(r.getReads(y, g2.getStrand()), mean2);
                                    '}
                                    'if (prob1 >= prob2) overlapStart = overlapStop + 1;
                                    'else overlapStop = overlapStart - 1;
                                End If
                            End If
                            frontUTR_length = overlapStart - (Math.Max(g1.start, g1.[stop]) + 1)
                            backUTR_length = Math.Min(g2.start, g2.[stop]) - 1 - overlapStop
                        End If

                        ' Merge UTRs from different experiments
                        'frontUTR_length_merged = Math.max(frontUTR_length_merged, frontUTR_length);
                        If frontUTR_length_merged = -1 Then
                            frontUTR_length_merged = frontUTR_length
                        ElseIf frontUTR_length_merged = 0 Then
                            frontUTR_length_merged = Math.Max(frontUTR_length_merged, frontUTR_length)
                        Else
                            frontUTR_length_merged = Math.Min(frontUTR_length_merged, frontUTR_length)
                        End If
                        'backUTR_length_merged = Math.max(backUTR_length_merged, backUTR_length);
                        If backUTR_length_merged = -1 Then
                            backUTR_length_merged = backUTR_length
                        ElseIf backUTR_length_merged = 0 Then
                            backUTR_length_merged = Math.Max(backUTR_length_merged, backUTR_length)
                        Else
                            backUTR_length_merged = Math.Min(backUTR_length_merged, backUTR_length)
                        End If
                    Next
                Next

                '  Update transcription start/stop of genes (if genes are expressed)
                If frontUTR_length_merged >= 0 Then
                    ' Gene is expressed and has flanking IG region
                    If g1.strand = "+"c Then
                        g1.stopT = g1.[stop] + frontUTR_length_merged
                    End If
                    If g1.strand = "-"c Then
                        g1.startT = g1.start + frontUTR_length_merged
                    End If
                End If
                If backUTR_length_merged >= 0 Then
                    ' Gene is expressed and has flanking IG region
                    If g2.strand = "+"c Then
                        g2.startT = g2.start - backUTR_length_merged
                    End If
                    If g2.strand = "-"c Then
                        g2.stopT = g2.[stop] - backUTR_length_merged
                    End If
                End If
            Next

            ' Keep track of the number of 5'UTRs and 3'UTRs
            For x As Integer = 0 To genome.numGenes() - 1
                Dim g As Gene = genome.getGene(x)
                If g.oRF AndAlso (g.startT > 0) AndAlso (g.strand = "+"c) AndAlso (g.start - g.startT > 0) Then
                    _num5UTRs += 1
                End If
                If g.oRF AndAlso (g.startT > 0) AndAlso (g.strand = "-"c) AndAlso (g.startT - g.start > 0) Then
                    _num5UTRs += 1
                End If
                If g.oRF AndAlso (g.stopT > 0) AndAlso (g.strand = "+"c) AndAlso (g.stopT - g.[stop] > 0) Then
                    _num3UTRs += 1
                End If
                If g.oRF AndAlso (g.stopT > 0) AndAlso (g.strand = "-"c) AndAlso (g.[stop] - g.stopT > 0) Then
                    _num3UTRs += 1
                End If
            Next
        End Sub

        ''' <summary>
        ''' For each IG, identify ncRNAs based on
        ''' the expression data.
        ''' </summary>
        Public Overridable Sub identifyRNAs()

            ' Generate coordinate map of genes
            Dim genesPlus As String() = New String(genome.size()) {}
            Dim genesMinus As String() = New String(genome.size()) {}
            For i As Integer = 0 To genesPlus.Length - 1
                genesPlus(i) = ""
                genesMinus(i) = ""
            Next
            For i As Integer = 0 To genome.numGenes() - 1
                Dim g As Gene = genome.getGene(i)
                Dim geneStart As Integer = Math.Min(g.start, g.[stop])
                Dim geneStop As Integer = Math.Max(g.start, g.[stop])
                If Not g.oRF Then
                    geneStart = g.minCoordinate
                    geneStop = g.maxCoordinate
                End If
                Dim genes As String() = genesPlus
                ' Plus strand
                If g.strand = "-"c Then
                    ' Minus strand
                    genes = genesMinus
                End If
                For j As Integer = geneStart To geneStop
                    genes(j) = g.name
                Next
            Next

            Dim rnaGenes As New List(Of Gene)()
            ' List of new predicted RNAs
            For x As Integer = 0 To genome.numGenes()
                ' For each IG region   Private 
                Dim g1 As Gene = Nothing
                Dim g2 As Gene = Nothing

                ' Identify downstream gene and IG stop coordinate
                If x = genome.numGenes() Then
                    g2 = New Gene((genome.size() - 1) & ".." & (genome.size() - 1) & vbTab & genome.getGene(x - 1).strand & vbTab & "0" & vbTab & "-" & vbTab & "???" & vbTab & "???" & vbTab & "-" & vbTab & "-" & vbTab & "???", "ORF")
                Else
                    g2 = genome.getGene(x)
                End If
                If Not g2.oRF Then
                    ' Do not predict novel ncRNAs near known RNA genes
                    Continue For
                End If
                Dim [stop] As Integer = g2.minCoordinate - 1

                ' Identify upstream gene and IG start coordinate
                Dim start As Integer = 1
                Dim upstreamGeneIndex As Integer = x - 1
                While upstreamGeneIndex >= 0
                    If unstranded Then
                        ' Strand ambiguous
                        start = genome.getGene(upstreamGeneIndex).maxCoordinate + 1
                        Exit While
                    Else
                        ' Strand specific
                        If g2.strand = genome.getGene(upstreamGeneIndex).strand Then
                            start = genome.getGene(upstreamGeneIndex).maxCoordinate + 1
                            Exit While
                        End If
                        upstreamGeneIndex -= 1
                    End If
                End While
                If upstreamGeneIndex < 0 Then
                    g1 = New Gene(1 & ".." & 1 & vbTab & g2.strand & vbTab & "0" & vbTab & "-" & vbTab & "???" & vbTab & "???" & vbTab & "-" & vbTab & "-" & vbTab & "???", "ORF")
                Else
                    g1 = genome.getGene(upstreamGeneIndex)
                End If
                If Not g1.oRF Then
                    ' Do not predict novel ncRNAs near known RNA genes
                    Continue For
                End If

                ' Determine strand
                Dim strand As Char = "?"c
                If unstranded Then
                    ' Strand ambiguous
                    strand = "?"c
                ElseIf (g1.strand = "+"c) OrElse (g2.strand = "+"c) Then
                    ' Plus strand
                    strand = "+"c
                ElseIf (g1.strand = "-"c) OrElse (g2.strand = "-"c) Then
                    ' Minus strand
                    strand = "-"c
                Else
                    strand = "?"c
                End If

                ' No action necessary if there is no IG region.
                If [stop] - start + 1 <= 0 Then
                    'output_IG_to_file(g1, g2, strand, new ArrayList<RNA>(), "IGs/Strep/");
                    Continue For
                End If

                Dim rnas As New List(Of RNA)()
                For i As Integer = 0 To conditions.Count - 1
                    For j As Integer = 0 To conditions(i).numReplicates() - 1
                        ' Find novel transcripts on both strands separately
                        identifyNovelTranscriptsInIG(rnas, conditions(i).getReplicate(j), start, [stop], strand)
                    Next
                Next

                ' Merge novel RNA transcripts (from different experiments and nearby novel RNAs)
                Dim merged_RNAs As New List(Of RNA)()
                If unstranded Then
                    ' Strand ambiguous
                    merge_RNAs(g1, g2, "?"c, rnas, merged_RNAs)
                Else
                    ' Strand specific
                    merge_RNAs(g1, g2, "+"c, rnas, merged_RNAs)
                    merge_RNAs(g1, g2, "-"c, rnas, merged_RNAs)
                End If
                'output_IG_to_file(g1, g2, strand, merged_RNAs, "IGs/Strep/");

                '
                '		if (merged_RNAs.size() > 0) {
                '		int IG_start = Math.max(g1.getStart(), g1.getStop()) + 1;
                '		int IG_stop = Math.min(g2.getStart(), g2.getStop()) - 1;
                '		if (!g1.isORF()) IG_start = g1.getMaxCoordinate() + 1;
                '		if (!g2.isORF()) IG_stop = g2.getMinCoordinate() - 1;
                '		output(IG_start + "_" + IG_stop + "\t" + g1.getName() + "\t" + g1.getStrand() + "\t" + g2.getName() + "\t" + g2.getStrand() + "\t" + rnas.size() + "\t" + merged_RNAs.size() + "\n");
                '		}
                '		


                ' Convert RNAs to Genes
                For i As Integer = 0 To merged_RNAs.Count - 1
                    Dim product As String = getAntisenseAnnotation(merged_RNAs(i), genesPlus, genesMinus)
                    If product.IndexOf("antisense") >= 0 Then
                        Me._numAntisenseRNAs += 1
                    End If
                    rnaGenes.Add(New Gene(merged_RNAs(i).start & ".." & merged_RNAs(i).[stop] & vbTab & merged_RNAs(i).strand & vbTab & "0" & vbTab & "-" & vbTab & "-" & vbTab & "predicted RNA" & vbTab & "-" & vbTab & "-" & vbTab & getAntisenseAnnotation(merged_RNAs(i), genesPlus, genesMinus), "RNA"))
                Next
            Next
            Me._numSenseRNAs = rnaGenes.Count - Me._numAntisenseRNAs

            ' Compute expression of each predicted RNA
            For i As Integer = 0 To rnaGenes.Count - 1
                Dim g As Gene = rnaGenes(i)
                For j As Integer = 0 To conditions.Count - 1
                    For k As Integer = 0 To conditions(j).numReplicates() - 1
                        Dim r As Replicate = conditions(j).getReplicate(k)
                        Dim readsForGene As Long = r.getReadsInRange(z, g.minCoordinate, g.maxCoordinate, g.strand)
                        g.setRawCount(j, k, readsForGene)
                        ' Set raw counts for gene
                        If r.avgLengthReads = 0 Then
                            g.setRawCount_reads(j, k, 0)
                        Else
                            g.setRawCount_reads(j, k, readsForGene \ r.avgLengthReads)
                        End If
                        'g.setNormalizedCount(j, k, Condition.getAvgUpperQuartile(), r.getUpperQuartile());
                        g.setNormalizedCount(j, k, 100000.0, r.upperQuartile)
                    Next
                Next

                ' Compute mean and RPKM for each gene in each condition
                g.computeExpression(conditions)

                ' Compute variance for each gene in each condition
                g.computeVariance(conditions)
            Next

            ' Add transcripts to list of genes
            genome.addPredictedRNAs(rnaGenes)
        End Sub



        ''' <summary>
        '''************************************************
        ''' **********  Dim  INSTANCE METHODS   **********
        ''' </summary>

        Private Function getUTR_length(g As Gene, start As Integer, [stop] As Integer, r As Replicate, isFront As Boolean) As Integer

            ' If we do not have a real gene but merely a place holder, do not compute UTR.
            If g.start = g.[stop] Then
                Return -1
            End If

            ' If we have an RNA gene rather than an ORF, do not compute UTR.
            If Not g.oRF Then
                Return -1
            End If

            ' Determine strand
            Dim strand As Char = g.strand
            If unstranded Then
                strand = "?"c
            End If

            ' Determine mean of gene's expression
            Dim mean As Double = r.getMeanOfRange(z, g.start, g.[stop], strand)
            If mean < r.minExpressionUTR Then
                ' Gene is not expressed
                Return -1
            End If

            ' Compute expression distribution
            '
            '	double thresh = 1.0;
            '	if (mean >= thresh * r.getMinExpressionUTR()) {
            '	    int start2 = g.getStart();
            '	    int stop2 = g.getStop();
            '	    if (stop2 < start2) {  // Swap
            '		int temp = start2;
            '		start2 = stop2;
            '		stop2 = temp;
            '	    }
            '	    for (int i=start2; i<=stop2; i++) {
            '		int x = (int)Math.round(20000*((r.getMeanOfRange(z, i, i, strand) - mean) / stdev));
            '		if (Math.abs(x) >= distribution.length/2) x = (distribution.length/2)*(x/Math.abs(x));
            '		distribution[x+distribution.length/2]++;
            '	    }
            '	}
            '	


            'int[] IG = new int[stop-start+1+WINDOW];
            'if (isFront) {  // Front UTR
            '   for (int i=start-WINDOW/2; i<=Math.min(stop+WINDOW/2, genome.size()-1); i++) IG[i-start+WINDOW/2] = r.getReads(i, g.getStrand());
            '} else {  // Back UTR
            '    for (int i=stop+WINDOW/2; i>=Math.max(start-WINDOW/2,1); i--) IG[stop+WINDOW/2-i] = r.getReads(i, g.getStrand());
            '}
            Dim IG As Integer() = New Integer([stop] - start) {}
            If isFront Then
                ' Front UTR
                For i As Integer = start To Math.Min([stop], genome.size() - 1)
                    IG(i - start) = r.getReads(z, i, strand)
                Next
            Else
                ' Back UTR
                For i As Integer = [stop] To Math.Max(start, 1) Step -1
                    IG([stop] - i) = r.getReads(z, i, strand)
                Next
            End If

            'for (int i=0; i<IG.length-WINDOW; i++) {
            For i As Integer = 0 To IG.Length - 1

                Dim SCALE As Double = 1.5
                If IG(i) = 0 Then
                    Return i
                End If
                If IG(i) >= SCALE * mean Then
                    Continue For
                End If
                If IG(i) >= SCALE * r.minExpressionUTR Then
                    Continue For
                End If
                If SCALE * r.getBackgroundProb(IG(i)) > PoissonPDF(IG(i), mean) Then
                    Return i
                Else
                    Continue For
                    '
                    '		// Quick check to see if we can easily classify the window as IG or UTR
                    '		int numZeros = 0;
                    '		int numExpressed = 0;
                    '		for (int j=i; j<i+WINDOW; j++) {
                    '		if (IG[j] == 0) numZeros++;
                    '		if (IG[j] >= mean) numExpressed++;
                    '		}
                    '		if (numZeros >= WINDOW/2) return i;
                    '		if (numExpressed >= WINDOW/2) continue;
                    '
                    '		// More extensive check if probability is closer to IG or UTR
                    '		double probIG = 1.0;
                    '		double probUTR = 1.0;
                    '		for (int j=i; j<i+WINDOW; j++) {
                    '		probIG *= r.getBackgroundProb(IG[j]);
                    '		probUTR *= getPoissonPDF(IG[j], mean);
                    '		}
                    '		if (probIG >= probUTR) return i;
                    '		

                End If
            Next
            'return IG.length-WINDOW;
            Return IG.Length
        End Function

        ''' <summary>
        ''' For a given IG region, identifies novel transcripts on the specified strand.
        ''' Each novel transcript is added to the list of "rnas".
        ''' </summary>
        Private Sub identifyNovelTranscriptsInIG(rnas As List(Of RNA), r As Replicate, start As Integer, [stop] As Integer, strand As Char)

            ' Search for a transcript seed, i.e., n consecutive nucleotides above some threshold.
            ' Then we extend this seed in both directions.
            'int n = WINDOW/2;
            Dim THRESHOLD As Double = r.minExpressionRNA * 2.0
            Dim MIN_RNA_LENGTH As Integer = 10

            'int[] IG = new int[stop-start+1+WINDOW];
            'int startReadsForIG = Math.max(start-WINDOW/2, 1);
            'for (int i=startReadsForIG; i<=Math.min(stop+WINDOW/2, genome.size()-1); i++) IG[i-startReadsForIG] = r.getReads(i, strand);
            Dim IG As Integer() = New Integer([stop] - start) {}
            Dim startReadsForIG As Integer = Math.Max(start, 1)
            Dim i As Integer = 0

            For i = startReadsForIG To Math.Min([stop], genome.size() - 1)
                IG(i - startReadsForIG) = r.getReads(z, i, strand)
            Next

            'int i = WINDOW/2;
            i = 0
            Dim startT As Integer = -1
            Dim stopT As Integer = -1
            'while (i < IG.length-WINDOW-n+1) {
            While i < IG.Length
                If (IG(i) >= THRESHOLD) AndAlso (startT = -1) Then
                    ' Start of new possible seed
                    startT = i
                    stopT = i
                    ' Within possible seed
                ElseIf (IG(i) >= THRESHOLD) AndAlso (startT >= 0) Then
                    stopT = i
                    ' Within non-seed
                    ' Do nothing
                ElseIf (IG(i) < THRESHOLD) AndAlso (startT = -1) Then
                    ' Just ended possible seed
                ElseIf (IG(i) < THRESHOLD) AndAlso (startT >= 0) Then
                    'if (stopT - startT + 1 >= n) {  // We have a seed
                    If stopT - startT + 1 >= MIN_RNA_LENGTH Then
                        ' We have a seed

                        ' Extend the seed downstream
                        'boolean done = (stopT == IG.length-WINDOW);
                        Dim done As Boolean = (stopT = IG.Length - 1)
                        While Not done
                            Dim mean As Double = r.getMeanOfRange(z, startT, stopT, strand)
                            If mean < THRESHOLD Then
                                Exit While
                            End If

                            If (IG(stopT + 1) >= THRESHOLD) OrElse (r.getBackgroundProb(IG(stopT + 1)) <= PoissonPDF(IG(stopT + 1), mean)) Then
                                stopT += 1
                            Else
                                Exit While
                            End If
                            '
                            '			double probIG = 1.0;
                            '			double probRNA = 1.0;
                            '			for (int j=stopT+1-WINDOW/2+1; j<Math.min(stopT+1+WINDOW/2+1,IG.length); j++) {
                            '			    probIG *= r.getBackgroundProb(IG[j]);
                            '			    probRNA *= getPoissonPDF(IG[j], mean);
                            '			}
                            '			if (probIG >= probRNA) break;
                            '			stopT++;
                            '			if (stopT == IG.length-1-WINDOW) done = true;
                            '			

                            done = (stopT = IG.Length - 1)
                        End While

                        ' Extend the seed upstream
                        'done = (startT == WINDOW/2);
                        done = (startT = 0)
                        While Not done
                            Dim mean As Double = r.getMeanOfRange(z, startT, stopT, strand)
                            If mean < THRESHOLD Then
                                Exit While
                            End If

                            If (IG(startT - 1) >= THRESHOLD) OrElse (r.getBackgroundProb(IG(startT - 1)) <= PoissonPDF(IG(startT - 1), mean)) Then
                                startT -= 1
                            Else
                                Exit While
                            End If
                            '
                            '			double probIG = 1.0;
                            '			double probRNA = 1.0;
                            '			for (int j=startT-1+WINDOW/2-1; j>Math.max(startT-1-WINDOW/2-1,-1); j--) {
                            '			    probIG *= r.getBackgroundProb(IG[j]);
                            '			    probRNA *= getPoissonPDF(IG[j], mean);
                            '			}
                            '			if (probIG >= probRNA) break;
                            '			startT--;
                            '			if (startT == WINDOW/2) done = true;
                            '			

                            done = (startT = 0)
                        End While

                        ' Add transcript to list
                        rnas.Add(New RNA(start + startT, start + stopT, strand))

                        i = stopT
                        startT = -1
                    Else
                        ' Seed is too short, so it is not a seed.
                        startT = -1
                    End If
                    ' Impossible to reach this case.
                Else
                End If
                i += 1
            End While

            ' Handle case where seed extends all the way to end of IG region
            If startT >= 0 Then
                ' Ended possible seed
                'if (stopT - startT + 1 >= n) {  // We have a seed
                If stopT - startT + 1 >= MIN_RNA_LENGTH Then
                    ' We have a seed

                    ' Extend the seed downstream
                    'boolean done = (stopT == IG.length-WINDOW);
                    Dim done As Boolean = (stopT = IG.Length - 1)
                    While Not done
                        Dim mean As Double = r.getMeanOfRange(z, startT, stopT, strand)
                        If mean < THRESHOLD Then
                            Exit While
                        End If

                        If (IG(stopT + 1) >= THRESHOLD) OrElse (r.getBackgroundProb(IG(stopT + 1)) <= PoissonPDF(IG(stopT + 1), mean)) Then
                            stopT += 1
                        Else
                            Exit While
                        End If
                        '
                        '			double probIG = 1.0;
                        '			double probRNA = 1.0;
                        '			for (int j=stopT+1-WINDOW/2+1; j<Math.min(stopT+1+WINDOW/2+1,IG.length); j++) {
                        '			probIG *= r.getBackgroundProb(IG[j]);
                        '			probRNA *= getPoissonPDF(IG[j], mean);
                        '			}
                        '			if (probIG >= probRNA) break;
                        '			stopT++;
                        '			if (stopT == IG.length-1-WINDOW) done = true;
                        '			

                        done = (stopT = IG.Length - 1)
                    End While

                    ' Extend the seed upstream
                    'done = (startT == WINDOW/2);
                    done = (startT = 0)
                    While Not done
                        Dim mean As Double = r.getMeanOfRange(z, startT, stopT, strand)
                        If mean < THRESHOLD Then
                            Exit While
                        End If

                        If (IG(startT - 1) >= THRESHOLD) OrElse (r.getBackgroundProb(IG(startT - 1)) <= PoissonPDF(IG(startT - 1), mean)) Then
                            startT -= 1
                        Else
                            Exit While
                        End If
                        '		    
                        '			double probIG = 1.0;
                        '			double probRNA = 1.0;
                        '			for (int j=startT-1+WINDOW/2-1; j>Math.max(startT-1-WINDOW/2-1,-1); j--) {
                        '			probIG *= r.getBackgroundProb(IG[j]);
                        '			probRNA *= getPoissonPDF(IG[j], mean);
                        '			}
                        '			if (probIG >= probRNA) break;
                        '			startT--;
                        '			if (startT == WINDOW/2) done = true;
                        '			

                        done = (startT = 0)
                    End While

                    ' Add transcript to list
                    rnas.Add(New RNA(start + startT, start + stopT, strand))
                End If
            End If
        End Sub

        ''' <summary>
        ''' Given a list of RNAs, merges overlapping RNAs from different experiments
        ''' and RNAs that are close in proximity. Adds the merged RNAs to the 
        ''' list "merged_RNAs".
        ''' </summary>
        Private Sub merge_RNAs(g1 As Gene, g2 As Gene, strand As Char, rnas As List(Of RNA), merged_RNAs As List(Of RNA))
            If rnas.Count = 0 Then
                Return
            End If

            ' Get coordinates of genes flanking the IG region
            Dim start1 As Integer = g1.start
            Dim stop1 As Integer = g1.[stop]
            If Not g1.oRF Then
                start1 = g1.startT
                stop1 = g1.stopT
            End If
            If start1 > stop1 Then
                ' Swap
                Dim temp As Integer = start1
                start1 = stop1
                stop1 = temp
            End If
            Dim start2 As Integer = g2.start
            Dim stop2 As Integer = g2.[stop]
            If Not g2.oRF Then
                start2 = g2.startT
                stop2 = g2.stopT
            End If
            If start2 > stop2 Then
                ' Swap
                Dim temp As Integer = start2
                start2 = stop2
                stop2 = temp
            End If

            Dim IG_start As Integer = stop1 + 1
            Dim IG_stop As Integer = start2 - 1
            Dim IG_length As Integer = IG_stop - IG_start + 1
            If IG_length <= 0 Then
                ' IG region has length 0. Do not output it.
                Return
            End If

            ' For each nucleotide in an IG region, indicates if a RNA is predicted
            ' for the nucleotide or not.
            Dim IG As Boolean() = New Boolean(IG_length - 1) {}
            For i As Integer = 0 To rnas.Count - 1
                If (rnas(i).strand = strand) OrElse unstranded Then
                    For j As Integer = rnas(i).start To rnas(i).[stop]
                        If j < IG_start Then
                            Return
                        End If
                        IG(j - IG_start) = True
                    Next
                End If
            Next

            ' Determine set of merged RNAs
            Dim PROXIMITY As Integer = 50
            ' RNAs within this many NTs of each other are merged
            Dim start As Integer = -1
            Dim [stop] As Integer = -1
            For i As Integer = 0 To IG.Length - 1
                If IG(i) AndAlso (start = -1) Then
                    ' Start of RNA
                    start = i
                    [stop] = i
                    ' Within RNA
                ElseIf IG(i) AndAlso (start >= 0) Then
                    [stop] = i
                    ' Within non-RNA
                    ' do nothing
                ElseIf Not IG(i) AndAlso (start = -1) Then
                    ' RNA just ended
                ElseIf Not IG(i) AndAlso (start >= 0) Then
                    For j As Integer = i To Math.Min(i + PROXIMITY, IG.Length) - 1
                        If IG(j) Then
                            [stop] = j
                        End If
                    Next
                    If [stop] > i - 1 Then
                        ' Continue the loop from the end of the RNA
                        i = [stop]
                    Else
                        ' Ignore predicted RNAs that abut annotated RNAs
                        If ((g1.oRF) OrElse (start > 0)) AndAlso ((g2.oRF) OrElse ([stop] < IG.Length - 1)) Then
                            merged_RNAs.Add(New RNA(IG_start + start, IG_start + [stop], strand))
                        End If
                        start = -1
                        [stop] = -1
                    End If
                Else
                    Output("Error - this case should be unreachable!" & vbLf)
                End If
            Next
            ' Check if IG region ends with RNA
            If start >= 0 Then
                Dim previousStop As Integer = [stop]
                For j As Integer = previousStop + 1 To Math.Min(previousStop + 1 + PROXIMITY, IG.Length) - 1
                    If IG(j) Then
                        [stop] = j
                    End If
                Next
                ' Ignore predicted RNAs that abut annotated RNAs
                If ((g1.oRF) OrElse (start > 0)) AndAlso ((g2.oRF) OrElse ([stop] < IG.Length - 1)) Then
                    merged_RNAs.Add(New RNA(IG_start + start, IG_start + [stop], strand))
                End If
            End If
        End Sub

        ''' <summary>
        ''' Given a predicted RNA, determines if it is antisense to any annotated genes.
        ''' Returns "-" if it is not antisense to any genes.
        ''' Otherwise, returns "antisense" followed by a list of genes it is antisense to.
        ''' </summary>
        Private Function getAntisenseAnnotation(rna As RNA, genesPlus As String(), genesMinus As String()) As String
            Dim genes As New Dictionary(Of String, String)()
            For i As Integer = rna.start To rna.[stop]
                If unstranded OrElse (rna.strand = "+"c) Then
                    genes.Add(genesMinus(i), "")
                End If
                If unstranded OrElse (rna.strand = "-"c) Then
                    genes.Add(genesPlus(i), "")
                End If
            Next

            Dim s As String = ""
            For Each key As String In genes.Keys
                If key.Length > 0 Then
                    s += " " & key
                End If
            Next

            If s.Length = 0 Then
                Return "-"
            Else
                Return "antisense:" & s
            End If
        End Function

        ''' <summary>
        ''' Output reads in IG region along with UTR and RNA annotation of region.
        ''' </summary>
        Private Sub output_IG_to_file(g1 As Gene, g2 As Gene, strand As Char, rnas As List(Of RNA), DIR__1 As String)

            ' Get coordinates of genes flanking the IG region
            Dim start1 As Integer = g1.start
            Dim stop1 As Integer = g1.[stop]
            If Not g1.oRF Then
                start1 = g1.startT
                stop1 = g1.stopT
            End If
            If start1 > stop1 Then
                ' Swap
                Dim temp As Integer = start1
                start1 = stop1
                stop1 = temp
            End If
            Dim start2 As Integer = g2.start
            Dim stop2 As Integer = g2.[stop]
            If Not g2.oRF Then
                start2 = g2.startT
                stop2 = g2.stopT
            End If
            If start2 > stop2 Then
                ' Swap
                Dim temp As Integer = start2
                start2 = stop2
                stop2 = temp
            End If

            Dim IG_start As Integer = stop1 + 1
            Dim IG_stop As Integer = start2 - 1
            Dim IG_length As Integer = IG_stop - IG_start + 1
            If IG_length <= 0 Then
                ' IG region has length 0. Do not output it.
                Return
            End If

            Dim mean1 As Integer = 0
            Dim mean2 As Integer = 0
            For x As Integer = 0 To conditions.Count - 1
                For y As Integer = 0 To conditions(x).numReplicates() - 1
                    If Not unstranded Then
                        ' Strand specific
                        mean1 = Math.Max(mean1, CInt(Math.Truncate(conditions(x).getReplicate(y).getMeanOfRange(z, start1, stop1, g1.strand))))
                        mean2 = Math.Max(mean2, CInt(Math.Truncate(conditions(x).getReplicate(y).getMeanOfRange(z, start2, stop2, g2.strand))))
                    Else
                        ' Starnd ambiguous
                        mean1 = Math.Max(mean1, CInt(Math.Truncate(conditions(x).getReplicate(y).getMeanOfRange(z, start1, stop1, "?"c))))
                        mean2 = Math.Max(mean2, CInt(Math.Truncate(conditions(x).getReplicate(y).getMeanOfRange(z, start2, stop2, "?"c))))
                    End If
                Next
            Next
            Dim IG As Integer() = New Integer(IG_length - 1) {}
            For i As Integer = IG_start To IG_stop
                Dim maxRead As Integer = 0
                For x As Integer = 0 To conditions.Count - 1
                    For y As Integer = 0 To conditions(x).numReplicates() - 1
                        maxRead = Math.Max(maxRead, conditions(x).getReplicate(y).getReads(z, i, strand))
                    Next
                Next
                IG(i - IG_start) = maxRead
            Next
            Dim annotation As StringBuilder = getAnnotationOfIG(IG, g1, g2, strand, rnas)

            Try

                ' Create directory (if it doesn't exist) to write files to
                If DIR__1(DIR__1.Length - 1) <> "/"c Then
                    DIR__1 += "/"c
                End If
                Dim dir__2 As New Oracle.Java.IO.File(DIR__1)
                If Not dir__2.exists() Then
                    dir__2.mkdir()
                End If

                Dim fileName As String = IG_start & "_" & IG_stop & ".txt"
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(DIR__1 & fileName))
                writer.println(g1.name & vbTab & g1.strand & vbTab & mean1)
                writer.println(g2.name & vbTab & g2.strand & vbTab & mean2)
                writer.println(vbLf)
                writer.println(annotation.ToString())
                writer.close()
            Catch ex As FileNotFoundException
                Output("Error - could not output file." & vbLf)
                Environment.[Exit](0)
            End Try
        End Sub

        ''' <summary>
        ''' Output reads in IG region and UTR of specified gene.
        ''' </summary>
        '
        '	private void output_IG_to_file(Gene g1, Gene g2, int frontUTR_length, int backUTR_length, int start, int stop, Replicate r, String DIR) {
        '
        '	int mean1 = (int)r.getMeanOfRange(g1.getStart(), g1.getStop(), g1.getStrand());
        '	int mean2 = (int)r.getMeanOfRange(g2.getStart(), g2.getStop(), g2.getStrand());
        '	int[] IG1 = new int[stop-start+1];
        '	int[] IG2 = null;
        '	if ((g1.getStrand() != '+') && (g1.getStrand() != '-')) {  // Placeholder gene
        '		for (int i=start; i<=stop; i++) IG1[i-start] = r.getReads(i, g2.getStrand());
        '	} else if ((g2.getStrand() != '+') && (g2.getStrand() != '-')) {  // Placeholder gene
        '		for (int i=start; i<=stop; i++) IG1[i-start] = r.getReads(i, g1.getStrand());
        '	} else if (g1.getStrand() == g2.getStrand()) {  // Same strand
        '		for (int i=start; i<=stop; i++) IG1[i-start] = r.getReads(i, g1.getStrand());
        '	} else {  // Different strands
        '		for (int i=start; i<=stop; i++) IG1[i-start] = r.getReads(i, g1.getStrand());
        '		IG2 = new int[stop-start+1];
        '		for (int i=start; i<=stop; i++) IG2[i-start] = r.getReads(i, g2.getStrand());
        '	}
        '	StringBuilder annotation1 = getAnnotationOfIG(IG1, g1, g2, frontUTR_length, backUTR_length);
        '	StringBuilder annotation2 = getAnnotationOfIG(IG2, g1, g2, frontUTR_length, backUTR_length);
        '
        '	try {
        '
        '		// Create directory (if it doesn't exist) to write files to
        '		if (DIR.charAt(DIR.length()-1) != '/') DIR += '/';
        '		File dir = new File(DIR);
        '		if (!dir.exists()) dir.mkdir();
        '		dir = new File(DIR + r.getName() + "/");
        '		if (!dir.exists()) dir.mkdir();
        '		DIR += r.getName() + "/";
        '
        '		String fileName = start + "_" + stop + ".txt";
        '		PrintWriter writer = new PrintWriter(new File(DIR + fileName));
        '		writer.println(g1.getName() + "\t" + g1.getStrand() + "\t" + mean1);
        '		writer.println(g2.getName() + "\t" + g2.getStrand() + "\t" + mean2);
        '		writer.println("\n");
        '		writer.println(annotation1.toString());
        '		if (annotation2 != null) writer.println("\n" + annotation2.toString());
        '		writer.close();
        '	} catch (FileNotFoundException ex) {
        '		output("Error - could not output file.\n");
        '		System.exit(0);
        '	}
        '	}
        '	


        ''' <summary>
        ''' Return a StringBuilder representation of an IG region consisting 
        ''' of reads and an annotation (5'UTR and 3'UTR and RNA) of those reads.
        ''' </summary>
        Private Function getAnnotationOfIG(IG As Integer(), g1 As Gene, g2 As Gene, strand As Char, rnas As List(Of RNA)) As StringBuilder
            Dim FASTA As Integer = 25
            Dim annotation As New StringBuilder(IG.Length)
            Dim i As Integer = 0

            For i = 0 To IG.Length - 1
                annotation.Append(".")
            Next

            ' Front UTR
            If (g1.strand = strand) OrElse (Me.unstranded) Then
                Dim frontUTR_count As Integer = g1.maxCoordinate - Math.Max(g1.start, g1.[stop])
                If Not g1.oRF Then
                    frontUTR_count = 0
                End If
                Dim UTR_char As Char = "3"c
                ' Plus strand
                If g1.strand = "-"c Then
                    ' Minus strand
                    UTR_char = "5"c
                End If
                For i = 0 To frontUTR_count - 1
                    annotation(i) = UTR_char
                Next
            End If

            ' Back UTR
            If (g2.strand = strand) OrElse (Me.unstranded) Then
                Dim backUTR_count As Integer = Math.Min(g2.start, g2.[stop]) - g2.minCoordinate
                If Not g2.oRF Then
                    backUTR_count = 0
                End If
                Dim UTR_char As Char = "5"c
                ' Plus strand
                If g2.strand = "-"c Then
                    ' Minus strand
                    UTR_char = "3"c
                End If
                For i = 0 To backUTR_count - 1
                    annotation(annotation.Length - 1 - i) = UTR_char
                Next
            End If

            ' RNAs
            Dim IG_start As Integer = Math.Max(g1.start, g1.[stop]) + 1
            If Not g1.oRF Then
                IG_start = g1.maxCoordinate + 1
            End If
            Dim IG_stop As Integer = Math.Min(g2.start, g2.[stop]) - 1
            If Not g2.oRF Then
                IG_stop = g2.minCoordinate - 1
            End If
            For i = 0 To rnas.Count - 1
                Dim rna As rna = rnas(i)
                If (rna.strand = strand) OrElse (Me.unstranded) Then
                    For j As Integer = rna.start To rna.[stop]
                        annotation(j - IG_start) = "R"c
                    Next
                End If
            Next

            Dim IG_annotation As New StringBuilder(IG.Length)
            i = 0
            For i = 0 To IG.Length - 1
                Dim num As String = "" & IG(i)
                If IG(i) < 10 Then
                    num = " " & num & " "
                ElseIf IG(i) < 100 Then
                    num = num & " "
                Else
                    num = num(0) & "e" & (num.Length - 1)
                End If
                IG_annotation.Append(" " & num)
                If (i + 1) Mod FASTA = 0 Then
                    IG_annotation.AppendLine()
                    For j As Integer = 0 To FASTA - 1
                        IG_annotation.Append(" " & " " & annotation((i + 1) - FASTA + j) & " ")
                    Next
                    IG_annotation.Append(vbLf & vbLf)
                End If
            Next
            If i Mod FASTA <> 0 Then
                IG_annotation.AppendLine()
                For j As Integer = 0 To (i Mod FASTA) - 1
                    IG_annotation.Append(" " & " " & annotation(i - (i Mod FASTA) + j) & " ")
                Next
                IG_annotation.Append(vbLf & vbLf)
            End If
            Return IG_annotation
        End Function

        ''' <summary>
        ''' Return a StringBuilder representation of an IG region consisting 
        ''' of reads and an annotation (5'UTR and 3'UTR) of those reads.
        ''' </summary>
        '
        '	private StringBuilder getAnnotationOfIG(int[] IG, Gene g1, Gene g2, int frontUTR_length, int backUTR_length) {
        '	int FASTA = 25;
        '	if (IG == null) return null;
        '	StringBuilder annotation = new StringBuilder();
        '	int frontUTR_count = 0;
        '	int backUTR_count = 0;
        '	int i = 0;
        '	for (i=0; i<IG.length; i++) {
        '		String num = "" + IG[i];
        '		if (IG[i] < 10) num  = " " + num + " ";
        '		else if (IG[i] < 100) num = num + " ";
        '		else num = num.charAt(0) + "e" + (num.length()-1);
        '		annotation.append(" " + num);
        '		if ((i+1) % FASTA == 0) {
        '		annotation.append("\n");
        '		for (int j=0; j<FASTA; j++) {
        '			if (frontUTR_count < frontUTR_length) {
        '			if (g1.getStrand() == '+') annotation.append(" " + " 3 ");
        '			else annotation.append(" " + " 5 ");
        '			frontUTR_count++;
        '			}
        '			else if (i+1-FASTA+j >= IG.length - backUTR_length) {
        '			if (g2.getStrand() == '+') annotation.append(" " + " 5 ");
        '			else annotation.append(" " + " 3 ");
        '			} else annotation.append(" " + " . ");
        '		}
        '		annotation.append("\n\n");
        '		}
        '	}
        '	if ((i+1) % FASTA != 0) annotation.append("\n");
        '	for (int j=0; j<(IG.length%FASTA); j++) {
        '		if (frontUTR_count < frontUTR_length) {
        '		if (g1.getStrand() == '+') annotation.append(" " + " 3 ");
        '		else annotation.append(" " + " 5 ");
        '		frontUTR_count++;
        '		}
        '		else if (IG.length - (IG.length%FASTA) + j >= IG.length - backUTR_length) {
        '		if (g2.getStrand() == '+') annotation.append(" " + " 5 ");
        '		else annotation.append(" " + " 3 ");
        '		} else annotation.append(" " + " . ");
        '	}
        '	annotation.append("\n\n");
        '	return annotation;
        '	}
        '	
    End Class
End Namespace
