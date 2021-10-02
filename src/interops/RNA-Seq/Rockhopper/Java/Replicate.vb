#Region "Microsoft.VisualBasic::5ead13a7904c097f92726e5436c5fd97, RNA-Seq\Rockhopper\Java\Replicate.vb"

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

    '     Class Replicate
    ' 
    '         Properties: avgLengthReads, avgReads, minExpression, minExpressionRNA, minExpressionUTR
    '                     name, totalReads, upperQuartile
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getBackgroundProb, getCompressedFileName, getMeanOfRange, getReads, getReadsInRange
    '                   getStdevOfRange, ToString, transformation
    ' 
    '         Sub: readInAlignmentFile
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

    ''' <summary>
    ''' A Replicate object represents information about a single RNA-seq experiment, including information about all reads from the experiment.
    ''' </summary>
    Public Class Replicate

        ''' <summary>
        ''' Path to sequencing reads file
        ''' </summary>
        Private fileName As String
        ''' <summary>
        ''' List of compressed (WIG) file names
        ''' </summary>
        Private compressedFileNames As List(Of String)
        ''' <summary>
        ''' Name of experiment
        ''' </summary>
        Private _name As String
        ''' <summary>
        ''' Reads on the plus strand for each genome
        ''' </summary>
        Private plusReads As Integer()()
        ''' <summary>
        ''' Reads on the minus strand for each genome
        ''' </summary>
        Private minusReads As Integer()()
        Private _totalReads As Long
        ''' <summary>
        ''' Number of reads for 75% most expressed gene
        ''' </summary>
        Private _upperQuartile As Long
        ''' <summary>
        ''' Number of nucleotides with few or no reads
        ''' </summary>
        Private background As List(Of Integer)
        ''' <summary>
        ''' Parameter of geometric distribution of background reads
        ''' </summary>
        Private backgroundParameter As Double
        ''' <summary>
        ''' Average number of reads mapping to nucleotides throughout genome
        ''' </summary>
        Private _avgReads As Double
        ''' <summary>
        ''' Minimum expression for a UTR region to be deemed expressed
        ''' </summary>
        Private _minExpressionUTR As Double
        ''' <summary>
        ''' Minimum expression for a ncRNA to be deemed expressed
        ''' </summary>
        Private _minExpressionRNA As Double
        ''' <summary>
        ''' Avg length of mapping reads in this replicate file
        ''' </summary>
        Private _avgLengthReads As Long

        ''' <summary>
        ''' Constructs a new Replicate object based on compressed sequencing reads files.
        ''' </summary>
        Public Sub New(fileNames As List(Of String), readFile As String, unstranded As Boolean)
            compressedFileNames = New List(Of String)()
            plusReads = New Integer(Peregrine.numSequences - 1)() {}
            minusReads = New Integer(Peregrine.numSequences - 1)() {}
            Me._totalReads = 0
            Dim backgroundLength As Integer = 2
            Me.background = New List(Of Integer)(backgroundLength)
            For i As Integer = 0 To backgroundLength - 1
                background.Add(0)
            Next
            For z As Integer = 0 To Peregrine.numSequences - 1
                Dim compressedFileName As String = fileNames(z)
                compressedFileNames.Add(compressedFileName)
                readInAlignmentFile(z, compressedFileName, unstranded)
            Next
            For i As Integer = 0 To backgroundLength - 2
                backgroundParameter += background(i)
            Next
            backgroundParameter /= (3.0 * 2.0 * genomeSize)
            If unstranded Then
                backgroundParameter /= (3.0 * 1.0 * genomeSize)
            End If
            _avgReads = _totalReads / (2.0 * genomeSize)
            If unstranded Then
                _avgReads = _totalReads / (1.0 * genomeSize)
            End If
        End Sub

#Region "**********   PUBLIC INSTANCE METHODS   **********"

        ''' <summary>
        ''' Return the name of this Replicate.
        ''' </summary>
        Public Overridable ReadOnly Property name() As String
            Get
                Return Me._name
            End Get
        End Property

        ''' <summary>
        ''' Return the name of the compressed WIG file for this Replicate
        ''' for the specified genome at index z.
        ''' </summary>
        Public Overridable Function getCompressedFileName(z As Integer) As String
            Return Me.compressedFileNames(z)
        End Function

        ''' <summary>
        ''' Return total number of reads in this Replicate.
        ''' </summary>
        Public Overridable ReadOnly Property totalReads() As Long
            Get
                Return Me._totalReads
            End Get
        End Property

        ''' <summary>
        ''' Return average number of reads in this Replicate.
        ''' </summary>
        Public Overridable ReadOnly Property avgReads() As Double
            Get
                Return Me._avgReads
            End Get
        End Property

        ''' <summary>
        ''' Return the number of reads mapping to the specified coordinate
        ''' on the specified strand for the specified genome at index z.
        ''' </summary>
        Public Overridable Function getReads(z As Integer, coord As Integer, strand As Char) As Integer
            If strand = "+"c Then
                Return plusReads(z)(coord)
            ElseIf strand = "-"c Then
                Return minusReads(z)(coord)
            Else
                Return plusReads(z)(coord) + minusReads(z)(coord)
            End If
        End Function

        ''' <summary>
        ''' Return the number of reads on the given strand mapping
        ''' to the given range of genomic coordinates for the specified
        ''' genome at index z.
        ''' </summary>
        Public Overridable Function getReadsInRange(z As Integer, start As Integer, [stop] As Integer, strand As Char) As Long
            If [stop] < start Then
                ' Swap
                Dim temp As Integer = start
                start = [stop]
                [stop] = temp
            End If

            Dim sum As Long = 0
            For i As Integer = Math.Max(start, 1) To Math.Min([stop] + 1, plusReads(z).Length) - 1
                If strand = "+"c Then
                    ' Plus strand
                    sum += Me.plusReads(z)(i)
                ElseIf strand = "-"c Then
                    ' Minus strand
                    sum += Me.minusReads(z)(i)
                Else
                    ' Ambiguous strand
                    sum += Me.plusReads(z)(i) + Me.minusReads(z)(i)
                End If
            Next
            Return sum
        End Function

        ''' <summary>
        ''' Return the upper quartile for this Replicate.
        ''' </summary>
        Public Overridable Property upperQuartile() As Long
            Get
                Return _upperQuartile
            End Get
            Set
                Me._upperQuartile = Value
            End Set
        End Property


        ''' <summary>
        ''' Sets the minimum level of expression (for a UTR region and
        ''' ncRNA to be considered expressed) in this Replicate based on 
        ''' the average number of reads per nucleotide in this Replicate
        ''' and the specified transcript sensitivity between 0.0 and 1.0, 
        ''' inclusive.
        ''' </summary>
        Public Overridable WriteOnly Property minExpression() As Double
            Set
                _minExpressionUTR = transformation(Math.Pow(Value, 3.3))
                _minExpressionRNA = transformation(Math.Pow(Value, 0.4))
            End Set
        End Property

        ''' <summary>
        ''' Return the minimum level of expression for a UTR region to be
        ''' considered expressed in this Replicate.
        ''' </summary>
        Public Overridable ReadOnly Property minExpressionUTR() As Double
            Get
                Return Me._minExpressionUTR
            End Get
        End Property

        ''' <summary>
        ''' Return the minimum level of expression for a ncRNA to be
        ''' considered expressed in this Replicate.
        ''' </summary>
        Public Overridable ReadOnly Property minExpressionRNA() As Double
            Get
                Return Me._minExpressionRNA
            End Get
        End Property

        ''' <summary>
        ''' Return average length of sequencing reads in this Replicate.
        ''' </summary>
        Public Overridable ReadOnly Property avgLengthReads() As Long
            Get
                Return Me._avgLengthReads
            End Get
        End Property

        ''' <summary>
        ''' Returns the probability that the given number of reads
        ''' at some nucleotide corresponds to the background, i.e.,
        ''' a non-transcript.
        ''' </summary>
        Public Overridable Function getBackgroundProb(numReads As Integer) As Double
            ' Based on geometric distribution
            Return Math.Pow(1.0 - Me.backgroundParameter, numReads) * Me.backgroundParameter
        End Function

        ''' <summary>
        ''' Return the mean number of reads on the given strand mapping
        ''' to the given range of genomic coordinates in the specified
        ''' genome at index z.
        ''' </summary>
        Public Overridable Function getMeanOfRange(z As Integer, start As Integer, [stop] As Integer, strand As Char) As Double
            Return getReadsInRange(z, start, [stop], strand) / CDbl(Math.Abs([stop] - start) + 1)
        End Function

        ''' <summary>
        ''' Return the standard deviation of reads on the given strand mapping
        ''' to the given range of genomic coordinates in the specified genome
        ''' at index z.
        ''' </summary>
        Public Overridable Function getStdevOfRange(z As Integer, start As Integer, [stop] As Integer, strand As Char, mean As Double) As Double
            If [stop] < start Then
                ' Swap
                Dim temp As Integer = start
                start = [stop]
                [stop] = temp
            End If

            Dim stdev As Double = 0.0
            For i As Integer = Math.Max(start, 1) To Math.Min([stop] + 1, plusReads(z).Length) - 1
                If strand = "+"c Then
                    ' Plus strand
                    stdev += Math.Pow(plusReads(z)(i) - mean, 2.0)
                ElseIf strand = "-"c Then
                    ' Minus strand
                    stdev += Math.Pow(minusReads(z)(i) - mean, 2.0)
                Else
                    ' Ambiguous strand
                    stdev += Math.Pow(plusReads(z)(i) + minusReads(z)(i) - mean, 2.0)
                End If
            Next
            Return stdev / Math.Sqrt(Math.Min([stop], plusReads(z).Length - 1) - Math.Max(start, 1) + 1)
        End Function

        ''' <summary>
        ''' Returns a String representation of this object.
        ''' </summary>
        Public Overridable Overloads Function ToString() As String
            Return _name
        End Function
#End Region

        ''' <summary>
        '''************************************************
        ''' **********   PRIVATE INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Reads in a compressed alignment file for a genome with
        ''' the specified index z. Stores the 
        ''' RNA-seq data in two lists, one for the plus strand 
        ''' and one for the minus strand. Also computes the 
        ''' total reads in the file.
        ''' </summary>
        Private Sub readInAlignmentFile(z As Integer, inFile As String, unstranded As Boolean)
            Dim foo As New FileOps(inFile)
            If Not foo.valid Then
                Output("Error - could not read in file " & inFile & vbLf)
                Return
            End If

            Me.fileName = foo.readFileName
            Dim slashIndex As Integer = fileName.LastIndexOf(Oracle.Java.IO.File.separator)
            Dim periodIndex As Integer = fileName.LastIndexOf(".")
            If periodIndex < 0 Then
                periodIndex = fileName.Length
            End If
            Me._name = fileName.Substring(slashIndex + 1, periodIndex - (slashIndex + 1))
            Me._avgLengthReads = foo.avgLengthReads
            Dim coordinates_plus As Integer() = foo.coordinates_plus
            Dim coordinates_minus As Integer() = foo.coordinates_minus
            Me.plusReads(z) = New Integer(genomeSizes(z) - 1) {}
            Me.minusReads(z) = New Integer(genomeSizes(z) - 1) {}
            For i As Integer = 0 To genomeSizes(z) - 1
                Dim reads_plus As Integer = coordinates_plus(i)
                Dim reads_minus As Integer = coordinates_minus(i)
                plusReads(z)(i) = reads_plus
                minusReads(z)(i) = reads_minus
                _totalReads += reads_plus + reads_minus
                If Not unstranded Then
                    ' Strand specific
                    If reads_plus < background.Count Then
                        background(reads_plus) = background(reads_plus) + 1
                    Else
                        background(background.Count - 1) = background(background.Count - 1) + 1
                    End If
                    If reads_minus < background.Count Then
                        background(reads_minus) = background(reads_minus) + 1
                    Else
                        background(background.Count - 1) = background(background.Count - 1) + 1
                    End If
                Else
                    ' Strand ambiguous
                    If reads_plus + reads_minus < background.Count Then
                        background(reads_plus + reads_minus) = background(reads_plus + reads_minus) + 1
                    Else
                        background(background.Count - 1) = background(background.Count - 1) + 1
                    End If
                End If
            Next
        End Sub

        ''' <summary>
        ''' Helper method for setting the minimum expression level for UTRs and ncRNAs.
        ''' </summary>
        Private Function transformation(transcriptSensitivity As Double) As Double
            If transcriptSensitivity <= 0.5 Then
                ' less than or equal to 0.5
                Return Math.Pow(_avgReads / 2, Math.Pow(transcriptSensitivity / 0.5, 0.25))
            Else
                ' greater than 0.5
                Return Math.Pow(_avgReads / 2, 1 + Math.Pow(2.0 * (transcriptSensitivity - 0.5), 2))
            End If
        End Function
    End Class
End Namespace
