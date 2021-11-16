#Region "Microsoft.VisualBasic::d1deddf5ba388e5342e047660d56f1f3, RNA-Seq\Rockhopper\Java\DataStructure\Index.vb"

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

    '     Class Index
    ' 
    '         Properties: numReplicons, randomSeed, stopAfterOneHit
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: charToInt, getReplicon, reverseComplement
    ' 
    '         Sub: (+3 Overloads) exactMatch, (+2 Overloads) inexactMatch, Main
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Oracle.Java.Collections
Imports Oracle.Java
Imports Microsoft.VisualBasic

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

    Public Class Index

        ''' <summary>
        '''******************************************
        ''' **********   Instance Variables   **********
        ''' </summary>

        Private _numReplicons As Integer
        ' Number of indexed replicons
        Private replicons As List(Of Replicon)
        ' Index for each replicon
        Private _stopAfterOneHit As Boolean
        ' Terminate the search when the first hit is found


        ''' <summary>
        '''***************************************
        ''' **********   Class Variables   **********
        ''' </summary>

        Public Shared rand As New Random()
        ' Random number generator


        ''' <summary>
        '''************************************
        ''' **********   Constructors   **********
        ''' </summary>

        Public Sub New(sequenceFiles As String())
            replicons = New List(Of Replicon)(sequenceFiles.Length)
            For i As Integer = 0 To sequenceFiles.Length - 1
                replicons.Add(New Replicon(sequenceFiles(i)))
            Next
            Me._numReplicons = replicons.Count
            Me._stopAfterOneHit = True
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   Public Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Set whether the search should terminate after finding the first hit,
        ''' even if there are other hits with the same score.
        ''' </summary>
        Public Overridable WriteOnly Property stopAfterOneHit() As Boolean
            Set
                Me._stopAfterOneHit = Value
            End Set
        End Property

        ''' <summary>
        ''' Return the number of indexed replicons.
        ''' </summary>
        Public Overridable ReadOnly Property numReplicons() As Integer
            Get
                Return Me._numReplicons
            End Get
        End Property

        ''' <summary>
        ''' Returns the ith Replicon index.
        ''' </summary>
        Public Overridable Function getReplicon(i As Integer) As Replicon
            Return Me.replicons(i)
        End Function

        ''' <summary>
        ''' Return all hits of s to the genome sequence.
        ''' </summary>
        Public Overridable Sub exactMatch(s As String, hits As List(Of Hit))
            hits.Clear()
            If Not _stopAfterOneHit Then
                ' Find all optimal hits
                For i As Integer = 0 To _numReplicons - 1
                    exactMatch(s, "+"c, i, 0, hits)
                    ' Align to plus strand
                    ' Align to minus strand
                    exactMatch(reverseComplement(s), "-"c, i, 0, hits)
                Next
                Collections.Sort(hits)
                Return
            Else
                ' Find only one hit
                For i As Integer = 0 To _numReplicons - 1
                    If rand.NextBoolean() Then
                        ' Randomly align first to plus strand
                        exactMatch(s, "+"c, i, 0, hits)
                        If hits.Count = 0 Then
                            exactMatch(reverseComplement(s), "-"c, i, 0, hits)
                        End If
                        Return
                    Else
                        ' Randomly align first to minus strand
                        exactMatch(reverseComplement(s), "-"c, i, 0, hits)
                        If hits.Count = 0 Then
                            exactMatch(s, "+"c, i, 0, hits)
                        End If
                        Return
                    End If
                Next
            End If
            Return
            ' This line should never be executed.
        End Sub

        ''' <summary>
        ''' Return all hits of s to the genome sequence.
        ''' </summary>
        Public Overridable Sub inexactMatch(s As String, qualities As Integer(), a As Alignment, hits As List(Of Hit), seedHits As List(Of Hit))
            hits.Clear()
            seedHits.Clear()
            inexactMatch(s, qualities, 0, a, hits, seedHits)
            Collections.Sort(hits)
            Return
        End Sub



        ''' <summary>
        '''************************************************
        ''' **********   Private Instance Methods   **********
        ''' </summary>

        ' Find indices of all occurrences of s in the sequence.
        Private Sub exactMatch(s As String, strand As Char, repliconIndex As Integer, errors As Integer, hits As List(Of Hit))
            exactMatch(s, strand, repliconIndex, errors, hits, Me._stopAfterOneHit)
        End Sub

        ' Find indices of all occurrences of s in the sequence.
        Private Sub exactMatch(s As String, strand As Char, repliconIndex As Integer, errors As Integer, hits As List(Of Hit), stopAfterOneHit_local As Boolean)
            Dim r As Replicon = replicons(repliconIndex)
            Dim i As Integer = s.Length - 1
            Dim c As Char = s(i)
            Dim sp As Integer = r.getC(charToInt(c))
            Dim ep As Integer = -1
            If charToInt(c) < 4 Then
                ep = r.getC(charToInt(c) + 1)
            Else
                ep = r.length
            End If
            i -= 1
            While (sp < ep) AndAlso (i >= 0)
                Dim index As Integer = charToInt(s(i))
                sp = r.getC(index) + r.getOcc(index, sp)
                ep = r.getC(index) + r.getOcc(index, ep)
                i -= 1
            End While
            If Not stopAfterOneHit_local Then
                ' Find all optimal hits
                For z As Integer = sp To ep - 1
                    ' 1-indexed
                    hits.Add(New Hit(r.getRotations(z) + 1, r.getRotations(z) + 1 + s.Length - 1, strand, errors, repliconIndex))
                Next
            Else
                ' Find only one hit
                If ep > sp Then
                    Dim randIndex As Integer = sp + rand.[Next](ep - sp)
                    ' 1-indexed
                    hits.Add(New Hit(r.getRotations(randIndex) + 1, r.getRotations(randIndex) + 1 + s.Length - 1, strand, errors, repliconIndex))
                End If
            End If
        End Sub

        ''' <summary>
        ''' Find all indices in the sequence (with the same 
        ''' best score) that match the specified query String 
        ''' with errors that sum to a value at or below the threshold.
        ''' </summary>
        Private Sub inexactMatch(s As String, qualities As Integer(), threshold As Integer, a As Alignment, hits As List(Of Hit), seedHits As List(Of Hit))
            Dim j As Integer = 0

            ' Exact match search
            If Not _stopAfterOneHit Then
                ' Find all optimal hits
                For i As Integer = 0 To _numReplicons - 1
                    exactMatch(s, "+"c, i, 0, hits)
                    ' Align to plus strand
                    ' Align to minus strand
                    exactMatch(reverseComplement(s), "-"c, i, 0, hits)
                Next
            Else
                ' Find only one hit
                For i As Integer = 0 To _numReplicons - 1
                    If rand.NextBoolean() Then
                        ' Randomly align first to plus strand
                        exactMatch(s, "+"c, i, 0, hits)
                        If hits.Count = 0 Then
                            exactMatch(reverseComplement(s), "-"c, i, 0, hits)
                        End If
                    Else
                        ' Randomly align first to minus strand
                        exactMatch(reverseComplement(s), "-"c, i, 0, hits)
                        If hits.Count = 0 Then
                            exactMatch(s, "+"c, i, 0, hits)
                        End If
                    End If
                    If Not hits.Count = 0 Then
                        Return
                    End If
                Next
            End If
            If Not hits.Count = 0 Then
                Return
            End If

            ' Inexact search via alignment
            Dim qualityThreshold As Integer = CInt(Math.Truncate(40 * s.Length * Peregrine.percentMismatches))
            Dim mismatches As Integer = CInt(Math.Truncate(s.Length * Peregrine.percentMismatches))
            Dim minSeedLength As Integer = Math.Max(CInt(Math.Truncate(s.Length * Peregrine.percentSeedLength)), Peregrine.minReadLength)
            a.numErrors = mismatches
            a.threshold = qualityThreshold
            If (mismatches = 0) OrElse (s.Length < minSeedLength) Then
                Return
            End If
            Dim minScore As Integer = Integer.MaxValue
            Dim numSeeds As Integer = Math.Min(mismatches + 1, CInt(Math.Truncate(Math.Ceiling((s.Length + 1) / CDbl(minSeedLength)))))
            Dim increment As Integer = s.Length \ numSeeds
            Dim seedLength As Integer = Math.Max(increment, minSeedLength)
            Dim reverseQualities As Integer() = New Integer(qualities.Length - 1) {}
            For q As Integer = 0 To qualities.Length - 1
                reverseQualities(qualities.Length - 1 - q) = qualities(q)
            Next
            For i As Integer = 0 To _numReplicons - 1
                ' Find seed in each replicon

                ' Determine all seeds from read
                Dim x As Integer = 0
                Dim count As Integer = 0
                While count < numSeeds
                    ' Try each seed from read
                    Dim start As Integer = x
                    Dim [stop] As Integer = x + seedLength - 1
                    If count = numSeeds - 1 Then
                        ' Final read
                        start = s.Length - seedLength
                        [stop] = s.Length - 1
                    End If
                    x += increment
                    count += 1

                    seedHits.Clear()
                    exactMatch(s.Substring(start, [stop] + 1 - start), "+"c, i, 0, seedHits, False)
                    exactMatch(reverseComplement(s.Substring(start, [stop] + 1 - start)), "-"c, i, 0, seedHits, False)
                    ' Align to minus strand
                    ' Align read to replicon sequence based on seedHits
                    For j = 0 To seedHits.Count - 1
                        If seedHits(j).strand = "+"c Then
                            ' Plus strand
                            a.align(replicons(i).sequence, seedHits(j).start - 2, seedHits(j).[stop], s, start - 1, [stop] + 1,
                            qualities)
                        Else
                            ' Minus strand
                            a.align(replicons(i).sequence, seedHits(j).start - 2, seedHits(j).[stop], reverseComplement(s), s.Length - 1 - [stop] - 1, s.Length - 1 - start + 1,
                            reverseQualities)
                        End If
                        ' Add alignment (if found and if among *best* scoring hits found so far) to "hits"
                        If (a.score >= 0) AndAlso (a.score <= qualityThreshold) AndAlso (a.errors <= mismatches) Then
                            ' Valid alignment
                            If a.score < minScore Then
                                ' This is the new best scoring alignment
                                hits.Clear()
                                hits.Add(New Hit(a.start, a.[stop], seedHits(j).strand, a.errors, i))
                                minScore = a.score
                                ' This is another best scoring alignment
                            ElseIf a.score = minScore Then
                                hits.Add(New Hit(a.start, a.[stop], seedHits(j).strand, a.errors, i))
                            Else
                                ' Do nothing.
                                ' We have previously found a better scoring alignment
                            End If
                        End If
                    Next
                End While

                ' Remove neighboring Hits
                Collections.Sort(hits)
                j = 1
                While j < hits.Count
                    If hits(j).start - hits(j - 1).[stop] < s.Length Then
                        hits.RemoveAt(j)
                    Else
                        j += 1
                    End If
                End While
            Next

            ' Randomly choose one best hit to return (as opposed to all best hits)
            If (hits.Count > 1) AndAlso _stopAfterOneHit Then
                ' Find only one hit (not all optimal hits)
                Dim randomBest As Hit = hits(rand.[Next](hits.Count))
                hits.Clear()
                hits.Add(randomBest)
            End If
        End Sub



        ''' <summary>
        '''********************************************
        ''' **********   Public Class Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Set seed for pseudorandom number generator.
        ''' </summary>
        Public Shared WriteOnly Property randomSeed() As Long
            Set
                rand = New Random(value)
            End Set
        End Property

        ' Convert a nucleotide character to an integer
        Public Shared Function charToInt(c As Char) As Integer
            If c = "A"c Then
                Return 0
            End If
            If c = "C"c Then
                Return 1
            End If
            If c = "G"c Then
                Return 2
            End If
            If c = "T"c Then
                Return 3
            End If
            If c = "^"c Then
                Return 4
            End If
            If c = "$"c Then
                Return 5
            End If
            Return -1
        End Function

        ''' <summary>
        ''' Return the reverse complement of the input sequence.
        ''' </summary>
        Public Shared Function reverseComplement(s As String) As String
            Dim sb As New StringBuilder(s.Length)
            For i As Integer = s.Length - 1 To 0 Step -1
                If s(i) = "A"c Then
                    sb.Append("T"c)
                ElseIf s(i) = "C"c Then
                    sb.Append("G"c)
                ElseIf s(i) = "G"c Then
                    sb.Append("C"c)
                ElseIf s(i) = "T"c Then
                    sb.Append("A"c)
                ElseIf s(i) = "^"c Then
                    sb.Append("^"c)
                Else
                    Peregrine.output("Invalid DNA nucleotide character: " & s(i) & vbLf)
                End If
            Next
            Return sb.ToString()
        End Function



        ''' <summary>
        '''*********************************************
        ''' **********   Private Class Methods   **********
        ''' </summary>



        ''' <summary>
        '''***********************************
        ''' **********   Main Method   **********
        ''' </summary>

        Private Shared Sub Main(args As String())

            If args.Length < 1 Then
                Oracle.Java.System.Err.println(vbLf & "USAGE: java Index <index1.fna,index2.fna,index3.fna>" & vbLf)
                Oracle.Java.System.Err.println("Index takes a comma-separated set of sequence files (such as FASTA genome files) to be indexed and it computes the Burrows-Wheeler transformed index for each sequence. If run from the command line, it simply outputs the size of each index to STDOUT. The Index class is meant to be instantiated from another application. " & vbLf)
                Environment.[Exit](0)
            End If

            Dim index As New Index(StringSplit(args(0), ",", True))
            For i As Integer = 0 To index.numReplicons - 1
                Console.WriteLine(index.getReplicon(i).name & " has size " & index.getReplicon(i).length)
            Next
        End Sub

    End Class

End Namespace
