#Region "Microsoft.VisualBasic::c352df1fb3a66a783bebdcb5e865022a, RNA-Seq\Rockhopper\Java\ObjectModels\Genome.vb"

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

    '     Class Genome
    ' 
    '         Properties: baseFileName, codingGenes, formalGenomeName, genes, iD
    '                     name
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: genesToString, getAnnotations, getGene, getSeq, mergeGenes
    '                   numGenes, readInGenes, size
    ' 
    '         Sub: addPredictedRNAs, create_GFF_file, Main, readInGenome
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.IO
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
    ''' An instance of the Genome class represents a genome and its
    ''' annotation, including the genome sequence, protein-coding
    ''' genes, and RNA genes in the genome.
    ''' </summary>
    Public Class Genome

        ''' <summary>
        '''******************************************
        ''' **********   INSTANCE VARIABLES   **********
        ''' </summary>

        Private m_ID As String
        Private _name As String
        ''' <summary>
        ''' We add a '?' character so it is 1-indexed.(整个基因组的核酸序列)
        ''' </summary>
        Private genome As String
        ''' <summary>
        ''' First token of FASTA header line, e.g., gi|49175990|ref|NC_000913.2|
        ''' </summary>
        Private _formalGenomeName As String = ""
        ''' <summary>
        ''' Path and name of genome file sans .fna extension
        ''' </summary>
        Private _baseFileName As String = ""
        Private _codingGenes As New List(Of Gene)()
        Private rnas As New List(Of Gene)()
        ''' <summary>
        ''' Merged coding genes and RNAs
        ''' </summary>
        Private _genes As New List(Of Gene)()

        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        ''' <summary>
        ''' Constructs a new Genome object based on files in the specified
        ''' directory (e.g., genome.fna, genes.ptt, rna.rnt).
        ''' </summary>
        Public Sub New(directory As String)
            Dim genomeFileName As String = Nothing
            Dim geneFileName As String = Nothing
            Dim rnaFileName As String = Nothing

            If Not directory.EndsWith("/") Then
                directory += "/"
            End If
            Dim dir As New Oracle.Java.IO.File(directory)
            Dim files As String() = dir.list()
            For i As Integer = 0 To files.Length - 1
                If files(i).EndsWith(".fna") Then
                    genomeFileName = directory & files(i)
                End If
                If files(i).EndsWith(".ptt") Then
                    geneFileName = directory & files(i)
                End If
                If files(i).EndsWith(".rnt") Then
                    rnaFileName = directory & files(i)
                End If
            Next

            If genomeFileName IsNot Nothing Then
                readInGenome(genomeFileName)
            End If
            If geneFileName IsNot Nothing Then
                Me._codingGenes = readInGenes(geneFileName, "ORF")
            End If
            If rnaFileName IsNot Nothing Then
                Me.rnas = readInGenes(rnaFileName, "RNA")
            End If
            Me._genes = mergeGenes(Me._codingGenes, Me.rnas)
            If genomeFileName IsNot Nothing Then
                create_GFF_file(Me._genes, genomeFileName, Me.genome.Length - 1)
            End If
            If genomeFileName IsNot Nothing Then
                _baseFileName = genomeFileName.Substring(0, genomeFileName.Length - 4)
            End If
        End Sub

        ''' <summary>
        ''' Constructs a new Genome object based on the specified files 
        ''' (e.g., genome.fna, genes.ptt, rna.rnt). If any of the file
        ''' names are empty, then they are simply ignored.
        ''' </summary>
        Public Sub New(genomeFileName As String, geneFileName As String, rnaFileName As String)
            If genomeFileName IsNot Nothing Then
                readInGenome(genomeFileName)
            End If
            If geneFileName IsNot Nothing Then
                Me._codingGenes = readInGenes(geneFileName, "ORF")
            End If
            If rnaFileName IsNot Nothing Then
                Me.rnas = readInGenes(rnaFileName, "RNA")
            End If
            Me._genes = mergeGenes(Me._codingGenes, Me.rnas)
            If genomeFileName IsNot Nothing Then
                create_GFF_file(Me._genes, genomeFileName, Me.genome.Length - 1)
            End If
        End Sub


        ''' <summary>
        '''***********************************************
        ''' **********   PUBLIC INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Returns the size in nucleotides of the genome
        ''' sequence. Note: the returned size will be 1
        ''' bigger than the actual genome size since we
        ''' add a character to the beginning of the 
        ''' genome sequence to ensure 1-indexing.
        ''' </summary>
        Public Overridable Function size() As Integer
            Return genome.Length
        End Function

        ''' <summary>
        ''' Returns the ID of the genome.
        ''' </summary>
        Public Overridable ReadOnly Property iD() As String
            Get
                Return m_ID
            End Get
        End Property

        ''' <summary>
        ''' Returns the name of the genome.
        ''' </summary>
        Public Overridable ReadOnly Property name() As String
            Get
                Return _name
            End Get
        End Property

        ''' <summary>
        ''' Returns the path and name of the genome file sans .fna extension.
        ''' </summary>
        Public Overridable ReadOnly Property baseFileName() As String
            Get
                Return _baseFileName
            End Get
        End Property

        ''' <summary>
        ''' Returns a subsequence of the (1-indexed) genome
        ''' as specified by the two coordinates, inclusive.
        ''' </summary>
        Public Overridable Function getSeq(start As Integer, [stop] As Integer) As String
            Return genome.Substring(Math.Max(start, 1), Math.Min([stop] + 1, genome.Length) - (Math.Max(start, 1)))
        End Function

        ''' <summary>
        ''' Return the number of genes in the genome.
        ''' </summary>
        Public Overridable Function numGenes() As Integer
            Return _genes.Count
        End Function

        ''' <summary>
        ''' Returns the formal genome name, e.g., gi|49175990|ref|NC_000913.2|
        ''' </summary>
        Public Overridable ReadOnly Property formalGenomeName() As String
            Get
                Return Me._formalGenomeName
            End Get
        End Property

        ''' <summary>
        ''' Return the Gene at the specified index.
        ''' </summary>
        Public Overridable Function getGene(i As Integer) As Gene
            If (i >= 0) AndAlso (i < _genes.Count) Then
                Return _genes(i)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Return the collection of protein coding genes.
        ''' </summary>
        Public Overridable ReadOnly Property codingGenes() As List(Of Gene)
            Get
                Return Me._codingGenes
            End Get
        End Property

        Public Overridable Sub addPredictedRNAs(predictedRNAs As List(Of Gene))
            Me._genes = mergeGenes(Me._genes, predictedRNAs)
        End Sub

        ''' <summary>
        ''' Return the collection of all genes.
        ''' </summary>
        Public Overridable ReadOnly Property genes() As List(Of Gene)
            Get
                Return Me._genes
            End Get
        End Property

        ''' <summary>
        ''' Returns a String[] representing the annotation
        ''' (gene, rRNA, tRNA, RNA, "") for each
        ''' coordinate in the genome on the specified strand.
        ''' </summary>
        Public Overridable Function getAnnotations(strand As Char) As String()
            Dim annotation As String() = New String(genome.Length - 1) {}
            For i As Integer = 0 To annotation.Length - 1
                ' Initialize
                annotation(i) = ""
            Next
            For i As Integer = 0 To _codingGenes.Count - 1
                Dim g As Gene = _codingGenes(i)
                If g.strand = strand Then
                    For j As Integer = g.first To g.last
                        annotation(j) = g.name
                    Next
                End If
            Next
            For i As Integer = 0 To rnas.Count - 1
                Dim g As Gene = rnas(i)
                If g.strand = strand Then
                    For j As Integer = g.first To g.last
                        If g.product.Contains("tRNA") Then
                            annotation(j) = "tRNA"
                        ElseIf (g.product.Contains("rRNA")) OrElse (g.product.Contains("ribosomal")) Then
                            annotation(j) = "rRNA"
                        Else
                            ' Miscellaneous RNA
                            annotation(j) = "RNA"
                        End If
                    Next
                End If
            Next
            Return annotation
        End Function

        ''' <summary>
        ''' Return a String representation of all genes in the genome.
        ''' </summary>
        Public Overridable Function genesToString(conditions As List(Of Condition), labels As String()) As String
            Dim sb As New StringBuilder()
            sb.Append("Transcription Start" & vbTab & "Translation Start" & vbTab & "Translation Stop" & vbTab & "Transcription Stop" & vbTab & "Strand" & vbTab & "Name" & vbTab & "Synonym" & vbTab & "Product")
            For j As Integer = 0 To conditions.Count - 1
                Dim conditionName As String = "" & (j + 1)
                If (labels IsNot Nothing) AndAlso (labels.Length = conditions.Count) Then
                    conditionName = labels(j)
                End If
                If verbose Then
                    ' Verbose output
                    If conditions(j).numReplicates() = 1 Then
                        sb.Append(vbTab & "Raw Counts " & conditionName)
                        sb.Append(vbTab & "Normalized Counts " & conditionName)
                    Else
                        For k As Integer = 0 To conditions(j).numReplicates() - 1
                            sb.Append(vbTab & "Raw Counts " & conditionName & " Replicate " & (k + 1))
                        Next
                        For k As Integer = 0 To conditions(j).numReplicates() - 1
                            sb.Append(vbTab & "Normalized Counts " & conditionName & " Replicate " & (k + 1))
                        Next
                    End If
                    sb.Append(vbTab & "RPKM " & conditionName)
                End If
                ' Label experiment conditions
                sb.Append(vbTab & "Expression " & conditionName)
            Next
            Dim numQvalues As Integer = 0
            For x As Integer = 0 To conditions.Count - 2
                ' First in pair
                For y As Integer = x + 1 To conditions.Count - 1
                    ' Second in pair
                    If verbose Then
                        ' Verbose output
                        If (labels IsNot Nothing) AndAlso (labels.Length = conditions.Count) Then
                            ' Use labels
                            sb.Append(vbTab & "pValue " & labels(x) & " vs " & labels(y))
                        Else
                            ' Do not use labels
                            sb.Append(vbTab & "pValue " & (x + 1) & " vs " & (y + 1))
                        End If
                    End If
                    If (numGenes() > 0) AndAlso (_genes(0).hasQvalue(numQvalues)) Then
                        If (labels IsNot Nothing) AndAlso (labels.Length = conditions.Count) Then
                            ' Use labels
                            sb.Append(vbTab & "qValue " & labels(x) & " vs " & labels(y))
                        Else
                            ' Do not use labels
                            sb.Append(vbTab & "qValue " & (x + 1) & " vs " & (y + 1))
                        End If
                    End If
                    numQvalues += 1
                Next
            Next
            sb.AppendLine()
            For i As Integer = 0 To numGenes() - 1
                sb.Append(_genes(i).ToString() & _genes(i).expressionToString() & vbLf)
            Next
            Return sb.ToString()
        End Function



        ''' <summary>
        '''************************************************
        ''' **********   PRIVATE INSTANCE METHODS   **********
        ''' </summary>

        ' Sets the "genome" instance variable with the FASTA formatted
        '	 * sequence found in the specified file. Also, sets the "ID"
        '	 * instance variable and the "name" instance variable based
        '	 * on the FASTA header line.
        '	 

        Private Sub readInGenome(fileName As String)
            Me.m_ID = ""
            Me._name = ""
            Me.genome = ""

            Dim Fasta As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = LANS.SystemsBiology.SequenceModel.FASTA.FastaToken.Load(fileName)
            Me.genome = "?" & Fasta.SequenceData

            Dim parse_header As String() = StringSplit(Fasta.Title, "\|", True)
            If parse_header.Length >= 5 Then
                Dim parse_ID As String() = StringSplit(parse_header(3), "\.", True)
                Me.m_ID = parse_ID(0)
                Dim parse_name As String() = StringSplit(parse_header(4), ",", True)
                Me._name = parse_name(0).Trim()
            End If
        End Sub

        ''' <summary>
        ''' Reads in a file of genes (either *.ptt or *.rnt) and returns
        ''' an ArrayList of gene objects.
        ''' </summary>
        Private Function readInGenes(fileName As String, type As String) As List(Of Gene)
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
                    listOfGenes.Add(New Gene(reader.nextLine(), type))
                End While
                reader.close()
            Catch e As FileNotFoundException
                Output("Error - the file " & fileName & " could not be found and opened." & vbLf)
            End Try
            Return listOfGenes
        End Function

        ''' <summary>
        ''' Given two lists of Genes, creates a new list of Genes containing
        ''' all the Genes from the two specified lists combined and sorted
        ''' by their coordinates.
        ''' </summary>
        Private Function mergeGenes(genes1 As List(Of Gene), genes2 As List(Of Gene)) As List(Of Gene)
            Dim listOfGenes As New List(Of Gene)()

            If Not genes1.IsNullOrEmpty Then
                Call listOfGenes.AddRange(genes1)
            End If
            If Not genes2.IsNullOrEmpty Then
                Call listOfGenes.AddRange(genes2)
            End If

            Return listOfGenes
        End Function

        ''' <summary>
        ''' Based on a list of genes and a FASTA genome file, 
        ''' create a GFF file that can be read by genome browser.
        ''' The newly created file is output to the same directory
        ''' as the FASTA genome file and has the same name except
        ''' with the extension *.gff.
        ''' </summary>
        Private Sub create_GFF_file(genes As List(Of Gene), genomeFileName As String, genomeSize As Integer)
            Dim GFF_fileName As String = genomeFileName.Substring(0, genomeFileName.Length - 4) & ".gff"
            Try

                ' Get info from FASTA genome file
                Dim reader As New Scanner(New Oracle.Java.IO.File(genomeFileName))
                Dim line As String = reader.nextLine()
                ' Header line
                Me._formalGenomeName = StringSplit(line, "\s+", True)(0).Substring(1)
                reader.close()

                ' Create GFF file
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(GFF_fileName))
                writer.println("track name=Genes color=255,0,255")
                writer.println("##gff-version 3")
                ' Header line
                writer.println("##sequence-region " & _formalGenomeName & " 1 " & genomeSize)
                For i As Integer = 0 To genes.Count - 1
                    Dim g As Gene = genes(i)
                    Dim type As String = "RNA"
                    Dim start As Integer = g.minCoordinate
                    Dim [stop] As Integer = g.maxCoordinate
                    If g.oRF Then
                        type = "Coding gene"
                        start = Math.Min(g.start, g.[stop])
                        [stop] = Math.Max(g.start, g.[stop])
                    End If
                    writer.println(_formalGenomeName & vbTab & "RefSeq" & vbTab & type & vbTab & start & vbTab & [stop] & vbTab & "." & vbTab & g.strand & vbTab & "." & vbTab & "name=" & g.name & ";" & "product=" & """" & g.product & """")
                Next
                writer.close()
            Catch e As IOException
                Output("Error - could not create GFF file " & GFF_fileName & vbLf)
                Return
            End Try
        End Sub

        ''' <summary>
        '''***********************************
        ''' **********   MAIN METHOD   **********
        ''' </summary>

        ''' <summary>
        ''' The main method is used to test the methods of this class.
        ''' </summary>
        Private Shared Sub Main(args As String())

            If args.Length = 0 Then
                Oracle.Java.System.Err.println(vbLf & "The Genome application must be invoked with a command line argument corresponding to the name of a directory containing genome files (e.g., *.fna, *.ptt, *.rnt)." & vbLf)
                Environment.[Exit](0)
            End If

            Dim g As New Genome(args(0))
            Console.WriteLine()
            Console.WriteLine("Genome ID:" & vbTab & vbTab & g.iD)
            Console.WriteLine("Genome name:" & vbTab & vbTab & g._name)
            Console.WriteLine("Size of genome is:" & vbTab & g.genome.Length)
            Console.WriteLine("First 10 nucleotides:" & vbTab & g.genome.Substring(0, 10))
            Console.WriteLine("Final 10 nucleotides:" & vbTab & g.genome.Substring(g.genome.Length - 10))
            Console.WriteLine("Number of ORFs:" & vbTab & vbTab & g._codingGenes.Count)
            Console.WriteLine("Number of RNAs:" & vbTab & vbTab & g.rnas.Count)
            Console.WriteLine("Number of all genes:" & vbTab & g._genes.Count)
            Console.WriteLine()
        End Sub

    End Class
End Namespace
