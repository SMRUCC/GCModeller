#Region "Microsoft.VisualBasic::17a7bce7e836b2daa1b813720ff1becc, RNA-Seq\Rockhopper\Java\DataStructure\Dictionary.vb"

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

    '     Class Dictionary
    ' 
    '         Properties: averageTranscriptLength, numTranscripts, seed, size
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: charToInt, informationContent, intToChar, longToString, (+2 Overloads) stringToLong
    '                   (+2 Overloads) stringToLong_Reverse
    ' 
    '         Sub: add, assembleTranscripts, buildTranscriptsAndClearTable, extendSeedBackward, extendSeedForward
    '              initializeReadMapping, Main, (+2 Overloads) mapFullLengthRead, prepareSeeds
    ' 
    '     Class KMer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: IComparable_CompareTo
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic

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
    ''' A Dictionary instance represents a dictionary of k-mers 
    ''' found in a set of sequencing reads.
    ''' </summary>


    Public Class Dictionary

        ''' <summary>
        '''******************************************
        ''' **********   Instance Variables   **********
        ''' </summary>

        Public dict As Table
        Private _size As Integer
        Private transcripts As New List(Of StringBuilder)()
        Private seeds As List(Of KMer)
        Private seedIndex As Integer
        Public bwtIndex As DeNovoIndex



        ''' <summary>
        '''***************************************
        ''' **********   Class Variables   **********
        ''' </summary>

        Private Shared k As Integer
        ' Size of k-mer
        Private Shared alphabet_1 As Char() = {"A"c, "C"c, "G"c, "T"c}
        Private Shared alphabet_2 As Integer() = New Integer(127) {}
        ' ASCII values


        ''' <summary>
        '''************************************
        ''' **********   Constructors   **********
        ''' </summary>

        Public Sub New(k As Integer)
            Dictionary.k = k
            dict = New Table()
            alphabet_2("A"c.Asc) = 0
            alphabet_2("C"c.Asc) = 1
            alphabet_2("G"c.Asc) = 2
            alphabet_2("T"c.Asc) = 3
            alphabet_2("^"c.Asc) = 4
            alphabet_2("$"c.Asc) = 5
            ' Empty transcript index initially
            bwtIndex = New DeNovoIndex(New List(Of StringBuilder)())
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   Public Instance Methods   **********
        ''' </summary>

        Public Overridable Sub add(read As String)
            Dim read_RC As String = ""
            If Assembler.unstranded Then
                read_RC = Assembler.reverseComplement(read)
            End If
            For i As Integer = 0 To read.Length - k
                If Not bwtIndex.exactMatch(read, i, i + k) Then
                    ' k-mer not in BWT transcript index
                    If Assembler.unstranded Then
                        ' Strand ambiguous
                        If Not bwtIndex.exactMatch(read_RC, read.Length - i - k, read.Length - i) Then
                            ' revComp of k-mer not in index
                            Dim k_mer As Long = stringToLong(read, i, i + k)
                            Dim k_mer_RC As Long = stringToLong(read_RC, read.Length - i - k, read.Length - i)
                            dict.add(k_mer, k_mer_RC)
                        End If
                    Else
                        ' Strand specific
                        Dim k_mer As Long = stringToLong(read, i, i + k)
                        dict.add(k_mer)
                    End If
                End If
            Next
            If dict.exceedsLoadFactor() Then
                ' Table is full
                buildTranscriptsAndClearTable()
            End If
        End Sub

        ''' <summary>
        ''' Returns the number of unique elements in the dictionary.
        ''' </summary>
        Public Overridable ReadOnly Property size() As Integer
            Get
                Return _size
            End Get
        End Property

        ''' <summary>
        ''' Returns the number of transcripts.
        ''' </summary>
        Public Overridable ReadOnly Property numTranscripts() As Integer
            Get
                Dim size As Integer = 0
                For Each sb As StringBuilder In transcripts
                    If sb.Length >= Assembler.minTranscriptLength Then
                        size += 1
                    End If
                Next
                Return size
            End Get
        End Property

        ''' <summary>
        ''' Returns the average transcript length.
        ''' </summary>
        Public Overridable ReadOnly Property averageTranscriptLength() As Integer
            Get
                Dim length As Long = 0
                Dim count As Integer = 0
                If transcripts.Count = 0 Then
                    Return 0
                End If
                For Each sb As StringBuilder In transcripts
                    If sb.Length >= Assembler.minTranscriptLength Then
                        length += sb.Length
                        count += 1
                    End If
                Next
                If count = 0 Then
                    Return 0
                Else
                    Return CInt(length \ count)
                End If
            End Get
        End Property

        Public Overridable Sub initializeReadMapping(stopAfterOneHit As Boolean)
            bwtIndex.initializeReadMapping(stopAfterOneHit)
        End Sub

        ''' <summary>
        ''' Map the full length read to the index of assembled transcripts.
        ''' </summary>
        Public Overridable Sub mapFullLengthRead(read As String)
            bwtIndex.exactMatch_fullRead(read)
        End Sub

        ''' <summary>
        ''' Map the full length *paired-end* read to the index of assembled transcripts.
        ''' </summary>
        Public Overridable Sub mapFullLengthRead(read As String, read2 As String)
            bwtIndex.exactMatch_fullRead(read, read2)
        End Sub

        Public Overridable Sub assembleTranscripts()
            _size = dict.size()
            ' Compute size
            ' Extend existing transcripts
            For i As Integer = 0 To transcripts.Count - 1
                extendSeedForward(transcripts(i))
                extendSeedBackward(transcripts(i))
            Next

            ' Create new transcripts
            prepareSeeds()
            Dim seed As Long = seed
            While seed > 0
                Dim transcript As New StringBuilder(longToString(seed))
                dict.remove(seed)
                ' Remove seed from dict
                extendSeedForward(transcript)
                extendSeedBackward(transcript)
                seed = seed
                transcripts.Add(transcript)
            End While
            dict = New Table()
            bwtIndex = New DeNovoIndex(transcripts)
            Oracle.Java.System.GC()
        End Sub



        ''' <summary>
        '''************************************************
        ''' **********   Private Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Check if the table is full.
        ''' If so, assemble transcripts via deBruijn graph and
        ''' clear the dictionary.
        ''' </summary>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Private Sub buildTranscriptsAndClearTable()
            If dict.exceedsLoadFactor() Then
                ' Table is full
                assembleTranscripts()
            End If
        End Sub

        ''' <summary>
        ''' Returns the most frequently occurring k-mer subject
        ''' to the following restrictions:
        '''  - The seed is sufficiently expressed, i.e., sufficiently many reads supporting the k-mer
        '''  - The information content is at least 1.5
        '''  - The k-mer is not palindromic (deleted).
        ''' </summary>
        Private ReadOnly Property seed() As Long
            Get
                While seedIndex < seeds.Count
                    Dim key As Long = seeds(seedIndex).key
                    seedIndex += 1
                    If dict.containsKey(key) AndAlso (informationContent(key) >= 1.5) Then
                        Return key
                    End If
                End While
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Since we need to repeatedly determine the best seed in the
        ''' dictionary (an expensive operation for a hashtable), we 
        ''' create a sorted vector so that finding the best seed is
        ''' efficient.
        ''' </summary>
        Private Sub prepareSeeds()
            seeds = New List(Of KMer)()
            For i As Integer = 0 To dict.capacity - 1
                Dim value As Integer = dict.getValueAtIndex(i)
                If value = 0 Then
                    Continue For
                End If
                Dim key As Long = dict.getKeyAtIndex(i)
                If value >= Assembler.minSeedExpression Then
                    seeds.Add(New KMer(key, value))
                End If
            Next
            Collections.Sort(seeds, Collections.ReverseOrder())
            seedIndex = 0
        End Sub

        ''' <summary>
        ''' Extends transcript 5' to 3'.
        ''' </summary>
        Private Sub extendSeedForward(transcript As StringBuilder)
            Dim done As Boolean = False
            While Not done
                Dim max As Integer = Assembler.minExpression - 1
                Dim maxIndex As Integer = -1
                transcript.Append("?"c)
                For i As Integer = 0 To alphabet_1.Length - 1
                    transcript(transcript.Length - 1) = alphabet_1(i)
                    Dim k_mer As Long = stringToLong(transcript, transcript.Length - k, transcript.Length)
                    If dict.containsKey(k_mer) AndAlso (dict.[get](k_mer) > max) Then
                        max = dict.[get](k_mer)
                        maxIndex = i
                    End If
                    If Assembler.unstranded Then
                        ' Strand ambiguous
                        Dim k_mer_RC As Long = stringToLong(Assembler.reverseComplement(transcript.SubString(transcript.Length - k)), 0, k)
                        If dict.containsKey(k_mer_RC) AndAlso (dict.[get](k_mer_RC) > max) Then
                            max = dict.[get](k_mer_RC)
                            maxIndex = i
                        End If
                    End If
                Next
                If maxIndex >= 0 Then
                    ' Found an extension
                    transcript(transcript.Length - 1) = alphabet_1(maxIndex)
                    Dim key As Long = stringToLong(transcript, transcript.Length - k, transcript.Length)
                    dict.remove(key)
                    ' Remove k-mer from dict
                    If Assembler.unstranded Then
                        ' Strand ambiguous
                        Dim key_RC As Long = stringToLong(Assembler.reverseComplement(transcript.SubString(transcript.Length - k)), 0, k)
                        ' Remove k-mer from dict
                        dict.remove(key_RC)
                    End If
                Else
                    ' No extension
                    transcript.Remove(transcript.Length - 1, 1)
                    done = True
                End If
            End While
        End Sub

        ''' <summary>
        ''' Extends transcript 3' to 5'.
        ''' </summary>
        Private Sub extendSeedBackward(transcript As StringBuilder)
            'JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
            transcript.Reverse()
            Dim done As Boolean = False
            While Not done
                Dim max As Integer = Assembler.minExpression - 1
                Dim maxIndex As Integer = -1
                transcript.Append("?"c)
                For i As Integer = 0 To alphabet_1.Length - 1
                    transcript(transcript.Length - 1) = alphabet_1(i)
                    Dim k_mer As Long = stringToLong_Reverse(transcript, transcript.Length - k, transcript.Length)
                    If dict.containsKey(k_mer) AndAlso (dict.[get](k_mer) > max) Then
                        max = dict.[get](k_mer)
                        maxIndex = i
                    End If
                    If Assembler.unstranded Then
                        ' Strand ambiguous
                        Dim k_mer_RC As Long = stringToLong_Reverse(Assembler.reverseComplement(transcript.SubString(transcript.Length - k)), 0, k)
                        If dict.containsKey(k_mer_RC) AndAlso (dict.[get](k_mer_RC) > max) Then
                            max = dict.[get](k_mer_RC)
                            maxIndex = i
                        End If
                    End If
                Next
                If maxIndex >= 0 Then
                    ' Found an extension
                    transcript(transcript.Length - 1) = alphabet_1(maxIndex)
                    Dim key As Long = stringToLong_Reverse(transcript, transcript.Length - k, transcript.Length)
                    dict.remove(key)
                    ' Remove k-mer from dict
                    If Assembler.unstranded Then
                        ' Strand ambiguous
                        Dim key_RC As Long = stringToLong_Reverse(Assembler.reverseComplement(transcript.SubString(transcript.Length - k)), 0, k)
                        ' Remove k-mer from dict
                        dict.remove(key_RC)
                    End If
                Else
                    ' No extension
                    transcript.Remove(transcript.Length - 1, 1)
                    done = True
                End If
            End While
            'JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
            transcript.Reverse()
        End Sub



        ''' <summary>
        '''*********************************************
        ''' **********   Private Class Methods   **********
        ''' </summary>

        Private Shared Function stringToLong(s As String, index1 As Integer, index2 As Integer) As Long
            Dim num As Long = 0
            For i As Integer = index1 To index2 - 1
                num = num << 2
                num += charToInt(s(i))
            Next
            Return (num)
        End Function

        Private Shared Function stringToLong(sb As StringBuilder, index1 As Integer, index2 As Integer) As Long
            Dim num As Long = 0
            For i As Integer = index1 To index2 - 1
                num = num << 2
                num += charToInt(sb(i))
            Next
            Return (num)
        End Function

        Private Shared Function stringToLong_Reverse(s As String, index1 As Integer, index2 As Integer) As Long
            Dim num As Long = 0
            For i As Integer = index2 - 1 To index1 Step -1
                num = num << 2
                num += charToInt(s(i))
            Next
            Return num
        End Function

        Private Shared Function stringToLong_Reverse(sb As StringBuilder, index1 As Integer, index2 As Integer) As Long
            Dim num As Long = 0
            For i As Integer = index2 - 1 To index1 Step -1
                num = num << 2
                num += charToInt(sb(i))
            Next
            Return (num)
        End Function

        Private Shared Function longToString(key As Long) As String
            Dim num As Long = CLng(key)
            Dim sb As New StringBuilder(k + 1)
            For i As Integer = 0 To k - 1
                sb.Append(intToChar(CInt(num) And 3))
                num = CLng(CULng(num) >> 2)
            Next
            'JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
            Return sb.Reverse().ToString()
        End Function

        Public Shared Function charToInt(c As Char) As Integer
            Return alphabet_2(c.Asc)
        End Function

        Private Shared Function intToChar(num As Integer) As Char
            Return alphabet_1(num)
        End Function

        Private Shared Function informationContent(k_mer As Long) As Double
            Dim counts As Double() = New Double(3) {}
            Dim num As Long = CLng(k_mer)
            For i As Integer = 0 To k - 1
                counts((CInt(num) And 3)) += 1.0
                num = CLng(CULng(num) >> 2)
            Next

            Dim sum As Double = 0.0
            For i As Integer = 0 To counts.Length - 1
                sum += counts(i)
            Next

            Dim _informationContent As Double = 0.0
            For i As Integer = 0 To counts.Length - 1
                If counts(i) > 0.0 Then
                    Dim frequency As Double = counts(i) / sum
                    _informationContent += frequency * Math.Log(frequency) / Math.Log(2.0)
                End If
            Next
            Return 0.0 - _informationContent
        End Function



        ''' <summary>
        '''***********************************
        ''' **********   Main Method   **********
        ''' </summary>

        Private Shared Sub Main(args As String())

            Dim k As Integer = 25
            Dim d As New Dictionary(k)

            Dim s As String = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAG"
            Console.WriteLine(s)
            For i As Integer = 0 To d.dict.capacity - 1
                Dim value As Integer = d.dict.getValueAtIndex(i)
                If value = 0 Then
                    Continue For
                End If
                Dim key As Long = d.dict.getKeyAtIndex(i)
                Console.WriteLine(longToString(key) & vbTab & value)
            Next
        End Sub

    End Class



    ''' <summary>
    '''************************************
    ''' **********   Helper Class   **********
    ''' </summary>

    Friend Class KMer
        Implements IComparable(Of KMer)

        Public key As Long
        Public count As Integer

        Public Sub New(key As Long, count As Integer)
            Me.key = key
            Me.count = count
        End Sub

        Private Function IComparable_CompareTo(other As KMer) As Integer Implements IComparable(Of KMer).CompareTo
            Return count.Equals(other.count)
        End Function
    End Class
End Namespace
