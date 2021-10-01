#Region "Microsoft.VisualBasic::24000008ecbedb9437c9bdbc23f26e1e, RNA-Seq\Rockhopper\Java\Replicon.vb"

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

    '     Class Replicon
    ' 
    '         Properties: length, name, sequence, transform
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getC, getOcc, getRotations, getStringInRotationsMatrix, lessThanOrEqualTo
    '                   partition, readInFastaFile, replaceAmbiguousCharacters, rotations, stepLeft
    '                   unpermute
    ' 
    '         Sub: Main, precomputeCharacterInfo, quicksort, swap
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

    Public Class Replicon

        ''' <summary>
        '''******************************************
        ''' **********   Instance Variables   **********
        ''' </summary>

        Private _name As String
        ' Name of replicon (first line of FASTA file)
        Private _sequence As String
        ' Original DNA sequence with '$' appended
        Private _length As Integer
        ' Length of sequence
        Private _rotations As Integer()
        ' Burrows-Wheeler matrix of cyclic rotations
        Private BWT As String
        ' Borrows-Wheeler transform of original sequence
        Private C As Integer()
        ' Number of chars lexicographically less than...
        Private _occ As Integer()()
        ' Number of occurrences of char up to row...


        ''' <summary>
        '''************************************
        ''' **********   Constructors   **********
        ''' </summary>

        Public Sub New(fastaFile As String)
            Me._sequence = readInFastaFile(fastaFile)
            Me._sequence = Me._sequence.ToUpper()
            Me._sequence = Me._sequence.Replace("U"c, "T"c)
            Me._sequence = replaceAmbiguousCharacters()
            Me._sequence = Me._sequence & "$"
            Me._length = _sequence.Length
            Me._rotations = rotations()
            Me.BWT = transform

            ' Populate "C" and "occ" instance variables
            precomputeCharacterInfo()
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   Public Instance Methods   **********
        ''' </summary>

        ' Return a row of the cyclic rotations matrix.
        Public Overridable Function getStringInRotationsMatrix(row As Integer) As String
            Return _sequence.Substring(_rotations(row)) & _sequence.Substring(0, _rotations(row))
        End Function

        ' Reverse the BWT transform
        Public Overridable Function unpermute() As String
            Dim sb As New StringBuilder()
            Dim r As Integer = 0
            While BWT(r) <> "$"c
                sb.Append(BWT(r))
                r = stepLeft(r)
            End While
            'JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
            Return sb.Reverse().ToString()
        End Function

        ''' <summary>
        ''' Returns the name of the genome sequence (FASTA header line).
        ''' </summary>
        Public Overridable ReadOnly Property name() As String
            Get
                Return Me._name
            End Get
        End Property

        ''' <summary>
        ''' Returns the DNA sequence of this Replicon.
        ''' </summary>
        Public Overridable ReadOnly Property sequence() As String
            Get
                Return Me._sequence
            End Get
        End Property

        ''' <summary>
        ''' Returns the length of the genome sequence (plus 1).
        ''' </summary>
        Public Overridable ReadOnly Property length() As Integer
            Get
                Return Me._length
            End Get
        End Property

        ''' <summary>
        ''' Returns number of characters lexographically less than x.
        ''' </summary>
        Public Overridable Function getC(x As Integer) As Integer
            Return Me.C(x)
        End Function

        ''' <summary>
        ''' Returns number of occurrences of character x up to row y.
        ''' </summary>
        Public Overridable Function getOcc(x As Integer, y As Integer) As Integer
            Return Me._occ(x)(y)
        End Function

        ''' <summary>
        ''' Returns index in Burrows-Wheeler rotations matrix at row x.
        ''' </summary>
        Public Overridable Function getRotations(x As Integer) As Integer
            Return Me._rotations(x)
        End Function



        ''' <summary>
        '''************************************************
        ''' **********   Private Instance Methods   **********
        ''' </summary>

        ' Read in sequence from from file.
        Private Function readInFastaFile(fastaFile As String) As String
            Try
                Dim reader As New Scanner(New Oracle.Java.IO.File(fastaFile))
                Dim sb As New StringBuilder()

                ' Handle header line
                If reader.hasNextLine() Then
                    Dim line As String = reader.nextLine()
                    If line.StartsWith(">") Then
                        ' Header line
                        Me._name = line.Substring(1).Trim()
                    Else
                        ' Not header line
                        sb.Append(line)
                    End If
                End If

                ' Read in file
                While reader.hasNextLine()
                    sb.Append(reader.nextLine())
                End While

                reader.close()
                Return sb.ToString()
            Catch e As FileNotFoundException
                Peregrine.output(vbLf & "Error - could not open file " & fastaFile & vbLf & vbLf)
                Environment.[Exit](0)
            End Try
            Return ""
        End Function

        ' Replace any non-nucleotide character with '^'.
        ' We use '^' because its ASCII value is greater than A,C,G,T
        Private Function replaceAmbiguousCharacters() As String
            Dim sb As New StringBuilder(Me._sequence)
            For i As Integer = 0 To sb.Length - 1
                If (sb(i) <> "A"c) AndAlso (sb(i) <> "C"c) AndAlso (sb(i) <> "G"c) AndAlso (sb(i) <> "T"c) Then
                    sb(i) = "^"c
                End If
            Next
            Return sb.ToString()
        End Function

        ' Compute the Burrows-Wheeler matrix of cyclic rotations.
        ' Rather than return a matrix, we return an array of integers
        ' where each value in the array is an index into the sequence.
        ' So a "row" of the matrix corresponds to the cyclic sequence
        ' beginnning at the specified index. All rows of the matrix
        ' have length equal to the length of the sequence.
        Private Function rotations() As Integer()
            Dim indices As Integer() = New Integer(Me._length - 1) {}
            For i As Integer = 0 To _length - 1
                indices(i) = i
            Next
            quicksort(indices, 0, _length - 1)
            Return indices
        End Function

        ' Compute the Burrows-Wheeler transform from the "rotations"
        ' matrix, i.e., the last column of the matrix.
        Private ReadOnly Property transform() As String
            Get
                Dim sb As New StringBuilder()
                For i As Integer = 0 To _length - 1
                    sb.Append(_sequence((_rotations(i) - 1 + _length) Mod _length))
                Next
                Return sb.ToString()
            End Get
        End Property

        ' Precompute information about how often each character occurs in BWT
        Private Sub precomputeCharacterInfo()

            ' Populate instance variable "C"
            Me.C = New Integer(5) {}
            ' All characters in DNA sequence have
            ' ASCII value less than 123.
            For i As Integer = 0 To _length - 1
                C(Index.charToInt(BWT(i))) += 1
            Next

            ' Cumulative
            Dim temp As Integer() = New Integer(5) {}
            temp(0) = C(0) + C(5)
            ' Number of characters <= 'A'
            temp(5) = C(5)
            ' Number of characters <= '$'
            For i As Integer = 1 To 4
                temp(i) = C(i) + temp(i - 1)
            Next

            ' Strictly less than
            For i As Integer = 0 To 5
                C(i) = temp(i) - C(i)
            Next

            ' Populate instance variable "occ"
            'ORIGINAL LINE: this.occ_Renamed = new int[5][length_Renamed+1];
            'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            Me._occ = ReturnRectangularIntArray(5, _length + 1)
            For i As Integer = 0 To _length - 1
                For j As Integer = 0 To 4
                    _occ(j)(i + 1) = _occ(j)(i)
                Next
                Dim index__1 As Integer = Index.charToInt(BWT(i))
                If (index__1 >= 0) AndAlso (index__1 < 5) Then
                    _occ(index__1)(i + 1) += 1
                End If
            Next
        End Sub

        ' Helper method for Burrows-Wheeler transform.
        ' Taken from appendix of Bowtie article.
        Private Function stepLeft(r As Integer) As Integer
            Dim index__1 As Integer = Index.charToInt(BWT(r))
            Return C(index__1) + _occ(index__1)(r)
        End Function

        ' We quicksort the array. But we don't sort the values at
        ' each array index, since each value is an index 
        ' corresponding to a String. We sort the Strings specified
        ' by each array index.
        Private Sub quicksort(a As Integer(), p As Integer, r As Integer)
            If p < r Then
                Dim q As Integer = partition(a, p, r)
                quicksort(a, p, q - 1)
                quicksort(a, q + 1, r)
            End If
        End Sub

        ' Quicksort helper method
        Private Function partition(a As Integer(), p As Integer, r As Integer) As Integer
            Dim x As Integer = Index.rand.[Next](r - p + 1) + p
            swap(a, x, r)
            x = a(r)
            Dim i As Integer = p - 1
            For j As Integer = p To r - 1
                If lessThanOrEqualTo(a(j), x) Then
                    ' a[j] <= x
                    i += 1
                    swap(a, i, j)
                End If
            Next
            swap(a, i + 1, r)
            Return i + 1
        End Function

        ' Quicksort partition helper method
        Private Sub swap(a As Integer(), i As Integer, j As Integer)
            Dim temp As Integer = a(i)
            a(i) = a(j)
            a(j) = temp
        End Sub

        ' Given two indices in "sequence", determine whether the
        ' cyclic String corresponding to the first index is less 
        ' than or equal to the cyclic String corresponding to
        ' the second index.
        Private Function lessThanOrEqualTo(i As Integer, j As Integer) As Boolean
            For z As Integer = 0 To _length - 1
                If _sequence((i + z) Mod _length) < _sequence((j + z) Mod _length) Then
                    Return True
                End If
                If _sequence((i + z) Mod _length) > _sequence((j + z) Mod _length) Then
                    Return False

                End If
            Next
            Return True
            ' We have equality of the two Strings
        End Function



        ''' <summary>
        '''***********************************
        ''' **********   Main Method   **********
        ''' </summary>

        Private Shared Sub Main(args As String())

            If args.Length < 1 Then
                Oracle.Java.System.Err.println(vbLf & "USAGE: java Replicon <index.fna>" & vbLf)
                Oracle.Java.System.Err.println("Replicon takes a sequence file (such as a FASTA genome) to be indexed and it computes the Burrows-Wheeler transformed index. If run from the command line, it simply outputs the size of the index to STDOUT. The Replicon class is meant to be instantiated from another application. " & vbLf)
                Environment.[Exit](0)
            End If

            Dim index As New Replicon(args(0))
            Console.WriteLine("Size of index is:" & vbTab & index.BWT.Length)
        End Sub

    End Class
End Namespace
