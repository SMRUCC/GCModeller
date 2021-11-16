#Region "Microsoft.VisualBasic::92457c628f11d1ecdeb2f4e11da9ab86, RNA-Seq\Rockhopper\Java\DeNovoIndex.vb"

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

    '     Class DeNovoIndex
    ' 
    '         Properties: avgLengthOfReads, numMappingReads, numReads, totalReads, transform
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) exactMatch, getC, getNumMappingReads, getNumReads, getOcc
    '                   getRotations, getStringInRotationsMatrix, lessThanOrEqualTo, partition, rotations
    '                   stepLeft, unpermute
    ' 
    '         Sub: combinePairedEndHits, (+2 Overloads) exactMatch_fullRead, halveReads, initializeReadMapping, Main
    '              precomputeCharacterInfo, quicksort, swap
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

    Public Class DeNovoIndex

        ''' <summary>
        '''******************************************
        ''' **********   Instance Variables   **********
        ''' </summary>

        Public sequence As String
        ' Concatenated transcripts (separated by '^' with '$' at end)
        Private length As Integer
        ' Length of sequence
        Private _rotations As Integer()
        ' Burrows-Wheeler matrix of cyclic rotations
        Private BWT As String
        ' Borrows-Wheeler transform of original sequence
        Private stopAfterOneHit As Boolean
        ' Report only one mapping as opposed to all mappings
        Public readCounts As AtomicIntegerArray()
        ' Number of reads mapping to each nt of each transcript from each sequencing reads file
        Private _numReads As AtomicIntegerArray
        ' Num reads from each file
        Private _numMappingReads As AtomicIntegerArray
        ' Num reads from each file mapping to transcripts
        Private avgLengthReads As AtomicLongArray
        ' Avg length of mapping reads from each file
        Private C As Integer()
        ' Number of chars lexicographically less than...
        Private _occ As Integer()()
        ' Number of occurrences of char up to row...


        ''' <summary>
        '''***************************************
        ''' **********   Class Variables   **********
        ''' </summary>

        Private Shared rand As New Random()

        ''' <summary>
        '''************************************
        ''' **********   Constructors   **********
        ''' </summary>

        Public Sub New(transcripts As List(Of StringBuilder))
            Dim sb As New StringBuilder()
            For Each transcript As StringBuilder In transcripts
                sb.Append(transcript)
                sb.Append("^"c)
            Next
            sb.Append("$"c)
            Me.sequence = sb.ToString()
            Me.length = sequence.Length
            Me._rotations = rotations()
            Me.BWT = transform
            ' Populate "C" and "occ" instance variables
            precomputeCharacterInfo()
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   Public Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Returns true if s is in index, false otherwise.
        ''' </summary>
        Public Overridable Function exactMatch(s As String) As Boolean
            Return exactMatch(s, 0, s.Length)
        End Function

        ''' <summary>
        ''' Returns true if s[start:end] is in index, false otherwise.
        ''' Note "start" is inclusive and "end" is exclusive.
        ''' </summary>
        Public Overridable Function exactMatch(s As String, start As Integer, [end] As Integer) As Boolean
            Dim i As Integer = [end] - 1
            Dim c As Char = s(i)
            Dim sp As Integer = getC(Dictionary.charToInt(c))
            Dim ep As Integer = -1
            If Dictionary.charToInt(c) < 4 Then
                ep = getC(Dictionary.charToInt(c) + 1)
            Else
                ep = length
            End If
            i -= 1
            While (sp < ep) AndAlso (i >= start)
                Dim index As Integer = Dictionary.charToInt(s(i))
                sp = getC(index) + getOcc(index, sp)
                ep = getC(index) + getOcc(index, ep)
                i -= 1
            End While
            If ep > sp Then
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Tallies the number of full length reads mapping to 
        ''' each nucleotide of each transcript.
        ''' If full length read does not map EXACTLY, then the
        ''' tally is not incremented.
        ''' </summary>
        Public Overridable Sub exactMatch_fullRead(s As String)
            _numReads.getAndIncrement(Assembler.readsFileIndex)
            Dim start As Integer = 0
            Dim [end] As Integer = s.Length
            Dim i As Integer = [end] - 1
            Dim c As Char = s(i)
            Dim sp As Integer = getC(Dictionary.charToInt(c))
            Dim ep As Integer = -1
            If Dictionary.charToInt(c) < 4 Then
                ep = getC(Dictionary.charToInt(c) + 1)
            Else
                ep = length
            End If
            i -= 1
            While (sp < ep) AndAlso (i >= start)
                Dim index As Integer = Dictionary.charToInt(s(i))
                sp = getC(index) + getOcc(index, sp)
                ep = getC(index) + getOcc(index, ep)
                i -= 1
            End While
            If Not stopAfterOneHit Then
                ' Report all mappings of read
                For j As Integer = sp To ep - 1
                    For k As Integer = getRotations(j) To getRotations(j) + (s.Length - 1)
                        readCounts(Assembler.readsFileIndex).incrementAndGet(k)
                    Next
                Next
                If ep > sp Then
                    avgLengthReads.getAndAdd(Assembler.readsFileIndex, s.Length)
                    _numMappingReads.getAndIncrement(Assembler.readsFileIndex)
                End If
            Else
                ' Report only one mapping of read
                If ep > sp Then
                    Dim randIndex As Integer = sp + rand.[Next](ep - sp)
                    For k As Integer = getRotations(randIndex) To getRotations(randIndex) + (s.Length - 1)
                        readCounts(Assembler.readsFileIndex).incrementAndGet(k)
                    Next
                    avgLengthReads.getAndAdd(Assembler.readsFileIndex, s.Length)
                    _numMappingReads.getAndIncrement(Assembler.readsFileIndex)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Tallies the number of full length *paired-end* reads mapping to 
        ''' each nucleotide of each transcript.
        ''' If full length *paired-end* read does not map EXACTLY, then the
        ''' tally is not incremented.
        ''' </summary>
        Public Overridable Sub exactMatch_fullRead(s As String, s2 As String)
            _numReads.getAndIncrement(Assembler.readsFileIndex)

            ' Read 1
            Dim start As Integer = 0
            Dim [end] As Integer = s.Length
            Dim i As Integer = [end] - 1
            Dim c As Char = s(i)
            Dim sp As Integer = getC(Dictionary.charToInt(c))
            Dim ep As Integer = -1
            If Dictionary.charToInt(c) < 4 Then
                ep = getC(Dictionary.charToInt(c) + 1)
            Else
                ep = length
            End If
            i -= 1
            While (sp < ep) AndAlso (i >= start)
                Dim index As Integer = Dictionary.charToInt(s(i))
                sp = getC(index) + getOcc(index, sp)
                ep = getC(index) + getOcc(index, ep)
                i -= 1
            End While
            If sp >= ep Then
                Return
            End If

            ' Read 2
            start = 0
            [end] = s2.Length
            i = [end] - 1
            c = s2(i)
            Dim sp2 As Integer = getC(Dictionary.charToInt(c))
            Dim ep2 As Integer = -1
            If Dictionary.charToInt(c) < 4 Then
                ep2 = getC(Dictionary.charToInt(c) + 1)
            Else
                ep2 = length
            End If
            i -= 1
            While (sp2 < ep2) AndAlso (i >= start)
                Dim index As Integer = Dictionary.charToInt(s2(i))
                sp2 = getC(index) + getOcc(index, sp2)
                ep2 = getC(index) + getOcc(index, ep2)
                i -= 1
            End While
            If sp2 >= ep2 Then
                Return
            End If

            ' Combine paired-end hits
            Dim hits1 As New List(Of Integer)(ep - sp)
            For j As Integer = sp To ep - 1
                hits1.Add(getRotations(j))
            Next
            Dim hits2 As New List(Of Integer)(ep2 - sp2)
            For j As Integer = sp2 To ep2 - 1
                hits2.Add(getRotations(j))
            Next
            Dim starts As New List(Of Integer)()
            Dim ends As New List(Of Integer)()
            combinePairedEndHits(s.Length, s2.Length, hits1, hits2, starts, ends)

            If Not stopAfterOneHit Then
                ' Report all mappings of read
                Dim sum As Integer = 0
                For j As Integer = 0 To starts.Count - 1
                    For k As Integer = starts(j) To ends(j) - 1
                        readCounts(Assembler.readsFileIndex).incrementAndGet(k)
                    Next
                    sum += ends(j) - starts(j)
                Next
                If starts.Count > 0 Then
                    avgLengthReads.getAndAdd(Assembler.readsFileIndex, sum \ starts.Count)
                    _numMappingReads.getAndIncrement(Assembler.readsFileIndex)
                End If
            Else
                ' Report only one mapping of read
                If starts.Count > 0 Then
                    Dim randIndex As Integer = rand.[Next](starts.Count)
                    For k As Integer = starts(randIndex) To ends(randIndex) - 1
                        readCounts(Assembler.readsFileIndex).incrementAndGet(k)
                    Next
                    avgLengthReads.getAndAdd(Assembler.readsFileIndex, ends(randIndex) - starts(randIndex))
                    _numMappingReads.getAndIncrement(Assembler.readsFileIndex)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Creates infrastructure to store information about
        ''' full length reads mapping to the length (rather
        ''' than just k-mers).
        ''' </summary>
        Public Overridable Sub initializeReadMapping(stopAfterOneHit As Boolean)
            Dim numReadsFiles As Integer = Assembler.readsFileIndex
            readCounts = New AtomicIntegerArray(numReadsFiles - 1) {}
            _numReads = New AtomicIntegerArray(numReadsFiles)
            avgLengthReads = New AtomicLongArray(numReadsFiles)
            _numMappingReads = New AtomicIntegerArray(numReadsFiles)
            For i As Integer = 0 To numReadsFiles - 1
                readCounts(i) = New AtomicIntegerArray(length)
            Next
            Me.stopAfterOneHit = stopAfterOneHit
        End Sub

        Public Overridable ReadOnly Property avgLengthOfReads() As Integer()
            Get
                Dim avgLengths As Integer() = New Integer(avgLengthReads.length - 1) {}
                For i As Integer = 0 To avgLengths.Length - 1
                    If _numMappingReads.[Get](i) = 0 Then
                        avgLengths(i) = 0
                    Else
                        avgLengths(i) = CInt(avgLengthReads.[Get](i) / _numMappingReads.[Get](i))
                    End If
                Next
                Return avgLengths
            End Get
        End Property

        ''' <summary>
        ''' In the case of unstranded (strand ambiguous) reads,
        ''' we invoke exactMatch_fullRead twice, once per strand,
        ''' thereby double counting each read. Here we fix the
        ''' double dip.
        ''' </summary>
        Public Overridable Sub halveReads(fileIndex As Integer)
            _numReads.[Set](fileIndex, _numReads.[Get](fileIndex) / 2)
        End Sub

        ''' <summary>
        ''' Returns an array containing the total number of full-length reads
        ''' in each file.
        ''' </summary>
        Public Overridable ReadOnly Property numReads() As Integer()
            Get
                Dim reads As Integer() = New Integer(_numReads.length - 1) {}
                For i As Integer = 0 To reads.Length - 1
                    reads(i) = _numReads.[Get](i)
                Next
                Return reads
            End Get
        End Property

        ''' <summary>
        ''' Returns an array containing the number of full-length reads
        ''' in each file mapping to assembled transcripts.
        ''' </summary>
        Public Overridable ReadOnly Property numMappingReads() As Integer()
            Get
                Dim mappingReads As Integer() = New Integer(_numMappingReads.length - 1) {}
                For i As Integer = 0 To mappingReads.Length - 1
                    mappingReads(i) = _numMappingReads.[Get](i)
                Next
                Return mappingReads
            End Get
        End Property

        ''' <summary>
        ''' Returns the total number of full-length reads
        ''' in the specified file.
        ''' </summary>
        Public Overridable Function getNumReads(fileIndex As Integer) As Integer
            Return _numReads.[Get](fileIndex)
        End Function

        ''' <summary>
        ''' Returns the number of full-length reads
        ''' in the specified file mapping to assembled transcripts.
        ''' </summary>
        Public Overridable Function getNumMappingReads(fileIndex As Integer) As Integer
            Return _numMappingReads.[Get](fileIndex)
        End Function

        ''' <summary>
        ''' Returns a 2D array representating the total number of reads mapping to every nt
        ''' in each replicate.
        ''' </summary>
        Public Overridable ReadOnly Property totalReads() As List(Of List(Of Long))
            Get
                Dim _totalReads As New List(Of List(Of Long))(Assembler.conditionFiles.Count)
                Dim linearIndex As Integer = 0
                For i As Integer = 0 To Assembler.conditionFiles.Count - 1
                    Dim files As String() = StringSplit(Assembler.conditionFiles(i), ",", True)
                    _totalReads.Add(New List(Of Long)(files.Length))
                    For j As Integer = 0 To files.Length - 1
                        _totalReads(i).Add(CLng(0))
                        For k As Integer = 0 To readCounts(linearIndex).length - 1
                            _totalReads(i)(j) = _totalReads(i)(j) + readCounts(linearIndex).[Get](k)
                        Next
                        linearIndex += 1
                    Next
                Next
                Return _totalReads
            End Get
        End Property



        ''' <summary>
        '''************************************************
        ''' **********   Private Instance Methods   **********
        ''' </summary>

        ' Return a row of the cyclic rotations matrix.
        Private Function getStringInRotationsMatrix(row As Integer) As String
            Return sequence.Substring(_rotations(row)) & sequence.Substring(0, _rotations(row))
        End Function

        ' Reverse the BWT transform
        Private Function unpermute() As String
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
        ''' Returns number of characters lexographically less than x.
        ''' </summary>
        Private Function getC(x As Integer) As Integer
            Return Me.C(x)
        End Function

        ''' <summary>
        ''' Returns number of occurrences of character x up to row y.
        ''' </summary>
        Private Function getOcc(x As Integer, y As Integer) As Integer
            Return Me._occ(x)(y)
        End Function

        ''' <summary>
        ''' Returns index in Burrows-Wheeler rotations matrix at row x.
        ''' </summary>
        Private Function getRotations(x As Integer) As Integer
            Return Me._rotations(x)
        End Function

        ''' <summary>
        ''' Compute the Burrows-Wheeler matrix of cyclic rotations.
        ''' Rather than return a matrix, we return an array of integers
        ''' where each value in the array is an index into the sequence.
        ''' So a "row" of the matrix corresponds to the cyclic sequence
        ''' beginnning at the specified index. All rows of the matrix
        ''' have length equal to the length of the sequence.
        ''' </summary>
        Private Function rotations() As Integer()
            Dim indices As Integer() = New Integer(Me.length - 1) {}
            For i As Integer = 0 To length - 1
                indices(i) = i
            Next
            quicksort(indices, 0, length - 1)
            Return indices
        End Function

        ''' <summary>
        ''' Compute the Burrows-Wheeler transform from the "rotations"
        ''' matrix, i.e., the last column of the matrix.
        ''' </summary>
        Private ReadOnly Property transform() As String
            Get
                Dim sb As New StringBuilder(length)
                For i As Integer = 0 To length - 1
                    sb.Append(sequence((_rotations(i) - 1 + length) Mod length))
                Next
                Return sb.ToString()
            End Get
        End Property

        ''' <summary>
        ''' Precompute information about how often each character occurs in BWT
        ''' </summary>
        Private Sub precomputeCharacterInfo()

            ' Populate instance variable "C"
            Me.C = New Integer(5) {}
            ' All characters in DNA sequence have
            ' ASCII value less than 123.
            For i As Integer = 0 To length - 1
                C(Dictionary.charToInt(BWT(i))) += 1
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
            'ORIGINAL LINE: this.occ_Renamed = new int[5][length+1];
            'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            Me._occ = ReturnRectangularIntArray(5, length + 1)
            For i As Integer = 0 To length - 1
                For j As Integer = 0 To 4
                    _occ(j)(i + 1) = _occ(j)(i)
                Next
                Dim index As Integer = Dictionary.charToInt(BWT(i))
                If (index >= 0) AndAlso (index < 5) Then
                    _occ(index)(i + 1) += 1
                End If
            Next
        End Sub

        ''' <summary>
        ''' Helper method for Burrows-Wheeler transform.
        ''' Taken from appendix of Bowtie article.
        ''' </summary>
        Private Function stepLeft(r As Integer) As Integer
            Dim index As Integer = Dictionary.charToInt(BWT(r))
            Return C(index) + _occ(index)(r)
        End Function

        ''' <summary>
        ''' We quicksort the array. But we don't sort the values at
        ''' each array index, since each value is an index 
        ''' corresponding to a String. We sort the Strings specified
        ''' by each array index.
        ''' </summary>
        Private Sub quicksort(a As Integer(), p As Integer, r As Integer)
            If p < r Then
                Dim q As Integer = partition(a, p, r)
                quicksort(a, p, q - 1)
                quicksort(a, q + 1, r)
            End If
        End Sub

        ''' <summary>
        ''' Quicksort helper method
        ''' </summary>
        Private Function partition(a As Integer(), p As Integer, r As Integer) As Integer
            Dim x As Integer = rand.[Next](r - p + 1) + p
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

        ''' <summary>
        ''' Quicksort partition helper method
        ''' </summary>
        Private Sub swap(a As Integer(), i As Integer, j As Integer)
            Dim temp As Integer = a(i)
            a(i) = a(j)
            a(j) = temp
        End Sub

        ''' <summary>
        ''' Given two indices in "sequence", determine whether the
        ''' cyclic String corresponding to the first index is less 
        ''' than or equal to the cyclic String corresponding to
        ''' the second index.
        ''' </summary>
        Private Function lessThanOrEqualTo(i As Integer, j As Integer) As Boolean
            For z As Integer = 0 To length - 1
                If sequence((i + z) Mod length) < sequence((j + z) Mod length) Then
                    Return True
                End If
                If sequence((i + z) Mod length) > sequence((j + z) Mod length) Then
                    Return False

                End If
            Next
            Return True
            ' We have equality of the two Strings
        End Function



        ''' <summary>
        '''********************************************
        ''' **********   Public Class Methods   **********
        ''' </summary>



        ''' <summary>
        '''*********************************************
        ''' **********   Private Class Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Combines two lists of exact matches, one for each read in a paired-end read,
        ''' into parallel lists of coordinates of paired-end matches. A combined match occurs when
        ''' there are matches from each of the two input lists that are
        ''' within the specified distance from each other.
        ''' </summary>
        Private Shared Sub combinePairedEndHits(length1 As Integer, length2 As Integer, hits1 As List(Of Integer), hits2 As List(Of Integer), starts As List(Of Integer), ends As List(Of Integer))
            For Each start1 As Integer In hits1
                For z As Integer = 0 To hits2.Count - 1
                    Dim start2 As Integer = hits2(z)
                    If (start1.CompareTo(start2) <= 0) AndAlso (start2 + length2 - start1 <= Assembler.maxPairedEndLength) Then
                        starts.Add(start1)
                        ends.Add(start2 + length2)
                        hits2.Remove(z)
                        Exit For
                    End If
                    If (start2.CompareTo(start1) <= 0) AndAlso (start1 + length1 - start2 <= Assembler.maxPairedEndLength) Then
                        starts.Add(start2)
                        ends.Add(start1 + length1)
                        hits2.Remove(z)
                        Exit For
                    End If
                Next
            Next
        End Sub



        ''' <summary>
        '''***********************************
        ''' **********   Main Method   **********
        ''' </summary>

        Private Shared Sub Main(args As String())

            ' Test DeNovoIndex class
            Dim sb1 As New StringBuilder("AAAAACCCCCGGGGGTTTTT")
            Dim sb2 As New StringBuilder("AAAAAAAAAAAAAAAAAAAA")
            Dim sb3 As New StringBuilder("CCCCCCCCCCCCCCCCCCCC")
            Dim sb4 As New StringBuilder("GTGTGTGTGTGTGTGTGTGT")
            Dim v As New List(Of StringBuilder)()
            v.Add(sb1)
            v.Add(sb2)
            v.Add(sb3)
            v.Add(sb4)
            Dim d As New Dictionary(25)
            ' Needed to populate alphabet_2
            Dim index As New DeNovoIndex(v)
            Console.WriteLine(index.sequence)
            Console.WriteLine("Length:" & vbTab & index.length)
            Console.WriteLine()
            Console.WriteLine(index.BWT)
            Console.WriteLine()
            Console.WriteLine(index.unpermute())
            Console.WriteLine()

            Dim s1 As String = "AAAAA"
            Console.WriteLine("Is exact match:" & vbTab & s1 & vbTab & index.exactMatch(s1))
            Dim s2 As String = "TGTGT"
            Console.WriteLine("Is exact match:" & vbTab & s2 & vbTab & index.exactMatch(s2))
            Dim s3 As String = "CCGTG"
            Console.WriteLine("Is exact match:" & vbTab & s3 & vbTab & index.exactMatch(s3))
            Dim s4 As String = "ACAC"
            Console.WriteLine("Is exact match:" & vbTab & s4 & vbTab & index.exactMatch(s4))
            Dim s5 As String = "CCCCCCCCCCCCCCCCCCCC"
            Console.WriteLine("Is exact match:" & vbTab & s5 & vbTab & index.exactMatch(s5))
            Dim s6 As String = "CCCCCCCCCCCCCCCCCCCCC"
            Console.WriteLine("Is exact match:" & vbTab & s6 & vbTab & index.exactMatch(s6))
            Dim s7 As String = "AAAAAAAAAACCCCCCCCCC"
            Console.WriteLine("Is exact match:" & vbTab & s7 & vbTab & index.exactMatch(s7))
            Dim s8 As String = "GGGGGAAAAA"
            Console.WriteLine("Is exact match:" & vbTab & s8 & vbTab & index.exactMatch(s8))
        End Sub

    End Class

End Namespace
