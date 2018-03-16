#Region "Microsoft.VisualBasic::e246ad0ce926c5508343b0efc5ebeadc, RNA-Seq\Rockhopper\Java\Operations\Alignment.vb"

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

    '     Class Alignment
    ' 
    '         Properties: [stop], errors, numErrors, score, start
    '                     threshold
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: getMatchMismatchChar, min, mismatch, readSequenceFromFile, tableToString_post
    '                   tableToString_pre, ToString
    ' 
    '         Sub: (+2 Overloads) align, backtrack_post, backtrack_pre, Main, postAlign
    '              preAlign
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

    ''' <summary>
    '''***********************************************************
    ''' An instance of the Alignment class represents an alignment
    ''' of a sequence read to a reference (genome) sequence.
    ''' </summary>
    Public Class Alignment

        ''' <summary>
        '''*******************************************************
        ''' ****************** INSTANCE VARIABLES *******************
        ''' </summary>

        ' Parameters
        Private _numErrors As Integer
        Private _threshold As Integer

        Private seq1 As String
        Private start1 As Integer
        Private stop1 As Integer
        Private seq2 As String
        Private start2 As Integer
        Private stop2 As Integer
        Private qualities As Integer()
        Private table As Integer()()
        Private _errors As Integer()()
        Private backtrack As Integer()()
        ' 1=LEFT, 2=DIAGONAL, 3=ABOVE, -1=DONE
        Private size As Integer
        Private optimalScore_post As Integer
        Private optimalRow_post As Integer
        Private optimalCol_post As Integer
        Private optimalError_post As Integer
        Private optimalScore_pre As Integer
        Private optimalRow_pre As Integer
        Private optimalCol_pre As Integer
        Private optimalError_pre As Integer
        Private formattedAlignment As StringBuilder

        ''' <summary>
        '''*******************************************************
        ''' ****************** CONSTRUCTORS *************************
        ''' </summary>

        Public Sub New()
            Me.New(0, 0)
        End Sub

        Public Sub New(numErrors As Integer, threshold As Integer)
            Me.size = 120
            'ORIGINAL LINE: this.table = new int[size][size];
            'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            Me.table = ReturnRectangularIntArray(size, size)
            'ORIGINAL LINE: this.errors_Renamed = new int[size][size];
            'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            Me._errors = ReturnRectangularIntArray(size, size)
            Me._numErrors = numErrors
            Me._threshold = threshold
        End Sub



        ''' <summary>
        '''*******************************************************
        ''' ****************** PUBLIC INSTANCE METHODS **************
        ''' </summary>

        Public Overridable WriteOnly Property numErrors() As Integer
            Set
                Me._numErrors = Value
            End Set
        End Property

        Public Overridable WriteOnly Property threshold() As Integer
            Set
                Me._threshold = Value
            End Set
        End Property

        Public Overridable Sub align(file1 As Oracle.Java.IO.File, file2 As Oracle.Java.IO.File)
            Dim genome As String = readSequenceFromFile(file1)
            Dim read As String = readSequenceFromFile(file2)
            Dim qualityScores As Integer() = New Integer(read.Length - 1) {}
            For i As Integer = 0 To qualityScores.Length - 1
                qualityScores(i) = 40
            Next
            Me.align(genome, -1, 0, read, -1, 0,
            qualityScores)
        End Sub

        ''' <summary>
        ''' Align seq1 (genome) and seq2 (read). The exclusive (start1:stop1) and (start2:stop2)
        ''' coordinates represent a perfect match seed between the two sequences.
        ''' </summary>
        Public Overridable Sub align(seq1 As String, start1 As Integer, stop1 As Integer, seq2 As String, start2 As Integer, stop2 As Integer,
        qualities As Integer())
            optimalError_post = 0
            optimalError_pre = 0

            If seq1.Length - stop1 < seq2.Length - stop2 Then
                ' genome is not long enough for read
                optimalScore_post = -2
                optimalScore_pre = -2
                Return
            End If
            If start1 < start2 Then
                ' genome is not long enough for read
                optimalScore_post = -2
                optimalScore_pre = -2
                Return
            End If

            Me.seq1 = seq1
            Me.start1 = start1
            Me.stop1 = stop1
            Me.seq2 = seq2
            Me.start2 = start2
            Me.stop2 = stop2
            Me.qualities = qualities
            If Me.seq2.Length > Me.size - Me._numErrors Then
                Me.size = Me.seq2.Length + Me._numErrors
                'ORIGINAL LINE: this.table = new int[this.size][this.size];
                'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
                Me.table = ReturnRectangularIntArray(Me.size, Me.size)
                'ORIGINAL LINE: this.errors_Renamed = new int[this.size][this.size];
                'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
                Me._errors = ReturnRectangularIntArray(Me.size, Me.size)
            End If

            ' Align post-seed
            Me.optimalScore_post = Integer.MaxValue
            Me.optimalRow_post = -2
            Me.optimalCol_post = -2
            Me.optimalError_post = 0
            postAlign()

            ' Align pre-seed
            Me.optimalScore_pre = Integer.MaxValue
            Me.optimalRow_pre = -2
            Me.optimalCol_pre = -2
            Me.optimalError_pre = 0
            preAlign()
        End Sub

        Public Overridable ReadOnly Property score() As Integer
            Get
                If (optimalScore_pre = -2) OrElse (optimalScore_post = -2) Then
                    Return -2
                End If
                Return optimalScore_pre + optimalScore_post
            End Get
        End Property

        Public Overridable ReadOnly Property errors() As Integer
            Get
                Return optimalError_pre + optimalError_post
            End Get
        End Property

        ''' <summary>
        ''' Return the starting coordinate in the genome (seq1) of the alignment.
        ''' </summary>
        Public Overridable ReadOnly Property start() As Integer
            Get
                If optimalRow_pre >= 0 Then
                    ' There is a region preceding the seed
                    Return start1 - optimalRow_pre + 1
                Else
                    ' No region in front of the seed
                    Return start1 + 2
                End If
            End Get
        End Property

        ''' <summary>
        ''' Return the stopping coordinate in the genome (seq1) of the alignment.
        ''' </summary>
        Public Overridable ReadOnly Property [stop]() As Integer
            Get
                If optimalRow_post >= 0 Then
                    ' There is a region following the seed
                    Return stop1 + optimalRow_post + 1
                Else
                    ' No region following the seed
                    Return stop1
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns a String representation of this Alignment.
        ''' </summary>
        Public Overridable Overloads Function ToString() As String
            If (optimalScore_pre = -2) OrElse (optimalScore_post = -2) Then
                Return "No Alignment."
            End If
            Return vbLf & "Errors:" & vbTab & (optimalError_post + optimalError_pre) & vbLf & "Score:" & vbTab & (optimalScore_post + optimalScore_pre)
        End Function



        ''' <summary>
        '''*******************************************************
        ''' ****************** PUBLIC CLASS METHODS *****************
        ''' </summary>



        ''' <summary>
        '''*******************************************************
        ''' ****************** PRIVATE INSTANCE METHODS *************
        ''' </summary>

        ''' <summary>
        ''' Align region after seed.
        ''' </summary>
        Private Sub postAlign()

            ' Initialize first row and column in table
            If (stop1 >= seq1.Length) OrElse (stop2 >= seq2.Length) Then
                optimalScore_post = 0
                optimalError_post = 0
                optimalRow_post = -2
                Return
            End If
            table(0)(0) = mismatch(stop1, stop2) * qualities(stop2)
            _errors(0)(0) = mismatch(stop1, stop2)
            Dim j As Integer = 0
            For j = 1 To Math.Min(_numErrors + 1, seq2.Length - stop2) - 1
                ' First row
                Dim gapLeft As Integer = table(0)(j - 1) + qualities(stop2 + j)
                Dim match As Integer = j * qualities(stop2 + j) + mismatch(stop1, stop2 + j) * qualities(stop2 + j)
                If optimalError_pre + _errors(0)(j - 1) + 1 > _numErrors Then
                    gapLeft = Integer.MaxValue
                End If
                If optimalError_pre + j + mismatch(stop1, stop2 + j) > _numErrors Then
                    match = Integer.MaxValue
                End If
                Dim minimum As Integer = Math.Min(gapLeft, match)
                table(0)(j) = minimum
                If match = minimum Then
                    _errors(0)(j) = j + mismatch(stop1, stop2 + j)
                Else
                    ' Gap left
                    _errors(0)(j) = _errors(0)(j - 1) + 1
                End If
            Next
            For i As Integer = 1 To Math.Min(_numErrors + 1, seq1.Length - stop1) - 1
                ' First column
                Dim gapAbove As Integer = table(i - 1)(0) + qualities(stop2)
                Dim match As Integer = i * qualities(stop2) + mismatch(stop1 + i, stop2) * qualities(stop2)
                If optimalError_pre + _errors(i - 1)(0) + 1 > _numErrors Then
                    gapAbove = Integer.MaxValue
                End If
                If optimalError_pre + i + mismatch(stop1 + i, stop2) > _numErrors Then
                    match = Integer.MaxValue
                End If
                Dim minimum As Integer = Math.Min(gapAbove, match)
                table(i)(0) = minimum
                If match = minimum Then
                    _errors(i)(0) = i + mismatch(stop1 + i, stop2)
                Else
                    ' Gap above
                    _errors(i)(0) = _errors(i - 1)(0) + 1
                End If
            Next

            ' Check if this is optimal alignment (i.e., final column in table)
            If stop2 = seq2.Length - 1 Then
                ' Only one column in table
                optimalScore_post = table(0)(0)
                optimalRow_post = 0
                optimalCol_post = 0
                optimalError_post = _errors(0)(0)
                For i As Integer = 1 To Math.Min(_numErrors + 1, seq1.Length - stop1) - 1
                    If table(i)(0) < optimalScore_post Then
                        optimalScore_post = table(i)(0)
                        optimalRow_post = i
                        optimalCol_post = 0
                        optimalError_post = _errors(i)(0)
                    End If
                Next
            ElseIf stop2 + j - 1 = seq2.Length - 1 Then
                optimalScore_post = table(0)(j - 1)
                optimalRow_post = 0
                optimalCol_post = j - 1
                optimalError_post = _errors(0)(j - 1)
            End If

            ' Populate table
            For i As Integer = 1 To Math.Min(seq2.Length - stop2 + _numErrors, seq1.Length - stop1) - 1
                For j = Math.Max(i - _numErrors, 1) To Math.Min(i + _numErrors + 1, seq2.Length - stop2) - 1

                    Dim gapLeft As Integer = Integer.MaxValue
                    Dim gapAbove As Integer = Integer.MaxValue
                    Dim gapLeft_errors As Integer = Integer.MaxValue
                    Dim gapAbove_errors As Integer = Integer.MaxValue
                    If j = i - _numErrors Then
                        ' Left table entry is unavailable
                        gapAbove = table(i - 1)(j) + qualities(stop2 + j)
                        gapAbove_errors = _errors(i - 1)(j) + 1
                        ' Above table entry is unavailable
                    ElseIf j = i + _numErrors Then
                        gapLeft = table(i)(j - 1) + qualities(stop2 + j)
                        gapLeft_errors = _errors(i)(j - 1) + 1
                    Else
                        ' Both left and above table entries are available
                        gapAbove = table(i - 1)(j) + qualities(stop2 + j)
                        gapLeft = table(i)(j - 1) + qualities(stop2 + j)
                        gapAbove_errors = _errors(i - 1)(j) + 1
                        gapLeft_errors = _errors(i)(j - 1) + 1
                    End If
                    Dim match As Integer = table(i - 1)(j - 1) + mismatch(stop1 + i, stop2 + j) * qualities(stop2 + j)
                    Dim match_errors As Integer = _errors(i - 1)(j - 1) + mismatch(stop1 + i, stop2 + j)
                    If (match_errors < gapAbove_errors) OrElse (gapLeft_errors < gapAbove_errors) OrElse (gapAbove_errors + optimalError_pre > _numErrors) Then
                        gapAbove = Integer.MaxValue
                    End If
                    If (match_errors < gapLeft_errors) OrElse (gapAbove_errors < gapLeft_errors) OrElse (gapLeft_errors + optimalError_pre > _numErrors) Then
                        gapLeft = Integer.MaxValue
                    End If
                    If (gapAbove_errors < match_errors) OrElse (gapLeft_errors < match_errors) OrElse (match_errors + optimalError_pre > _numErrors) Then
                        match = Integer.MaxValue
                    End If
                    Dim minimum As Integer = min(match, gapAbove, gapLeft)

                    ' Assign value to table
                    table(i)(j) = minimum
                    If match = minimum Then
                        _errors(i)(j) = _errors(i - 1)(j - 1) + mismatch(stop1 + i, stop2 + j)
                    ElseIf gapAbove = minimum Then
                        _errors(i)(j) = _errors(i - 1)(j) + 1
                    ElseIf gapLeft = minimum Then
                        _errors(i)(j) = _errors(i)(j - 1) + 1
                        ' Impossible case. Do nothing.
                    Else
                    End If

                    ' Check if this is optimal alignment (i.e., final column in table)
                    If (stop2 + j = seq2.Length - 1) AndAlso (table(i)(j) < optimalScore_post) Then
                        optimalScore_post = table(i)(j)
                        optimalRow_post = i
                        optimalCol_post = j
                        optimalError_post = _errors(i)(j)
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' Align region before seed.
        ''' </summary>
        Private Sub preAlign()

            ' Initialize first row and column in table
            If (start1 < 0) OrElse (start2 < 0) Then
                optimalScore_pre = 0
                optimalError_pre = 0
                optimalRow_pre = -2
                Return
            End If
            table(0)(0) = mismatch(start1, start2) * qualities(start2)
            _errors(0)(0) = mismatch(start1, start2)
            Dim j As Integer = 0
            For j = 1 To Math.Min(_numErrors + 1, start2 + 1) - 1
                ' First row
                Dim gapLeft As Integer = table(0)(j - 1) + qualities(start2 - j)
                Dim match As Integer = j * qualities(start2 - j) + mismatch(start1, start2 - j) * qualities(start2 - j)
                If optimalError_post + _errors(0)(j - 1) + 1 > _numErrors Then
                    gapLeft = Integer.MaxValue
                End If
                If optimalError_post + j + mismatch(start1, start2 - j) > _numErrors Then
                    match = Integer.MaxValue
                End If
                Dim minimum As Integer = Math.Min(gapLeft, match)
                table(0)(j) = minimum
                If match = minimum Then
                    _errors(0)(j) = j + mismatch(start1, start2 - j)
                Else
                    ' Gap left
                    _errors(0)(j) = _errors(0)(j - 1) + 1
                End If
            Next
            For i As Integer = 1 To Math.Min(_numErrors + 1, start1 + 1) - 1
                ' First column
                Dim gapAbove As Integer = table(i - 1)(0) + qualities(start2)
                Dim match As Integer = i * qualities(start2) + mismatch(start1 - i, start2) * qualities(start2)
                If optimalError_post + _errors(i - 1)(0) + 1 > _numErrors Then
                    gapAbove = Integer.MaxValue
                End If
                If optimalError_post + i + mismatch(start1 - i, start2) > _numErrors Then
                    match = Integer.MaxValue
                End If
                Dim minimum As Integer = Math.Min(gapAbove, match)
                table(i)(0) = minimum
                If match = minimum Then
                    _errors(i)(0) = i + mismatch(start1 - i, start2)
                Else
                    ' Gap above
                    _errors(i)(0) = _errors(i - 1)(0) + 1
                End If
            Next

            ' Check if this is optimal alignment (i.e., final column in table)
            If start2 = 0 Then
                ' Only one column in table
                optimalScore_pre = table(0)(0)
                optimalRow_pre = 0
                optimalCol_pre = 0
                optimalError_pre = _errors(0)(0)
                For i As Integer = 1 To Math.Min(_numErrors + 1, start1 + 1) - 1
                    If table(i)(0) < optimalScore_pre Then
                        optimalScore_pre = table(i)(0)
                        optimalRow_pre = i
                        optimalCol_pre = 0
                        optimalError_pre = _errors(i)(0)
                    End If
                Next
            ElseIf start2 - j + 1 = 0 Then
                optimalScore_pre = table(0)(j - 1)
                optimalRow_pre = 0
                optimalCol_pre = j - 1
                optimalError_pre = _errors(0)(j - 1)
            End If

            ' Populate table
            For i As Integer = 1 To Math.Min(start2 + 1 + _numErrors, start1 + 1) - 1
                For j = Math.Max(i - _numErrors, 1) To Math.Min(i + _numErrors + 1, start2 + 1) - 1

                    Dim gapLeft As Integer = Integer.MaxValue
                    Dim gapAbove As Integer = Integer.MaxValue
                    Dim gapLeft_errors As Integer = Integer.MaxValue
                    Dim gapAbove_errors As Integer = Integer.MaxValue
                    If j = i - _numErrors Then
                        ' Left table entry is unavailable
                        gapAbove = table(i - 1)(j) + qualities(start2 - j)
                        gapAbove_errors = _errors(i - 1)(j) + 1
                        ' Above table entry is unavailable
                    ElseIf j = i + _numErrors Then
                        gapLeft = table(i)(j - 1) + qualities(start2 - j)
                        gapLeft_errors = _errors(i)(j - 1) + 1
                    Else
                        ' Both left and above table entries are available
                        gapAbove = table(i - 1)(j) + qualities(start2 - j)
                        gapLeft = table(i)(j - 1) + qualities(start2 - j)
                        gapAbove_errors = _errors(i - 1)(j) + 1
                        gapLeft_errors = _errors(i)(j - 1) + 1
                    End If
                    Dim match As Integer = table(i - 1)(j - 1) + mismatch(start1 - i, start2 - j) * qualities(start2 - j)
                    Dim match_errors As Integer = _errors(i - 1)(j - 1) + mismatch(start1 - i, start2 - j)
                    If (match_errors < gapAbove_errors) OrElse (gapLeft_errors < gapAbove_errors) OrElse (gapAbove_errors + optimalError_post > _numErrors) Then
                        gapAbove = Integer.MaxValue
                    End If
                    If (match_errors < gapLeft_errors) OrElse (gapAbove_errors < gapLeft_errors) OrElse (gapLeft_errors + optimalError_post > _numErrors) Then
                        gapLeft = Integer.MaxValue
                    End If
                    If (gapAbove_errors < match_errors) OrElse (gapLeft_errors < match_errors) OrElse (match_errors + optimalError_post > _numErrors) Then
                        match = Integer.MaxValue
                    End If
                    Dim minimum As Integer = min(match, gapAbove, gapLeft)

                    ' Assign value to table
                    table(i)(j) = minimum
                    If match = minimum Then
                        _errors(i)(j) = _errors(i - 1)(j - 1) + mismatch(start1 - i, start2 - j)
                    ElseIf gapAbove = minimum Then
                        _errors(i)(j) = _errors(i - 1)(j) + 1
                    ElseIf gapLeft = minimum Then
                        _errors(i)(j) = _errors(i)(j - 1) + 1
                        ' Impossible case. Do nothing.
                    Else
                    End If

                    ' Check if this is optimal alignment (i.e., final column in table)
                    If (start2 - j = 0) AndAlso (table(i)(j) < optimalScore_pre) Then
                        optimalScore_pre = table(i)(j)
                        optimalRow_pre = i
                        optimalCol_pre = j
                        optimalError_pre = _errors(i)(j)
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' Once an alignment score has been computed, the backtracking table is searched to
        ''' determine a StringBuilder representation of the alignment.
        ''' </summary>
        Private Sub backtrack_post(s1_alignment As StringBuilder, alignment As StringBuilder, s2_alignment As StringBuilder)

            ' Begin backtracking search at optimal alignment score (found in last column in table)
            Dim i As Integer = optimalRow_post
            Dim j As Integer = optimalCol_post
            ' Last column in table
            ' Backtrack through table to beginning of alignment
            While (i >= 0) OrElse (j >= 0)
                If backtrack(i)(j) = 2 Then
                    ' Diagonal
                    s1_alignment.Append(seq1(stop1 + i))
                    alignment.Append(getMatchMismatchChar(seq1(stop1 + i), seq2(stop2 + j)))
                    s2_alignment.Append(seq2(stop2 + j))
                    i -= 1
                    j -= 1
                    ' Gap above
                ElseIf backtrack(i)(j) = 3 Then
                    s1_alignment.Append(seq1(stop1 + i))
                    alignment.Append(" "c)
                    s2_alignment.Append("-"c)
                    i -= 1
                    ' Gap left
                ElseIf backtrack(i)(j) = 1 Then
                    s1_alignment.Append("-"c)
                    alignment.Append(" "c)
                    s2_alignment.Append(seq2(stop2 + j))
                    j -= 1
                    ' DONE
                ElseIf backtrack(i)(j) = -1 Then
                    s1_alignment.Append(seq1(stop1 + i))
                    alignment.Append(getMatchMismatchChar(seq1(stop1 + i), seq2(stop2 + j)))
                    s2_alignment.Append(seq2(stop2 + j))
                    While i <> 0
                        ' We're in first column. Alignment begins with gaps.
                        s1_alignment.Append(seq1(stop1 + i - 1))
                        alignment.Append(" "c)
                        s2_alignment.Append("-"c)
                        i -= 1
                    End While
                    While j <> 0
                        ' We're in first row. Alignment begins with gaps.
                        s1_alignment.Append("-"c)
                        alignment.Append(" "c)
                        s2_alignment.Append(seq2(stop2 + j - 1))
                        j -= 1
                    End While
                    i -= 1
                    j -= 1
                Else
                    ' Impossible case.
                    Peregrine.output(vbLf & "There was an error when backtracking." & vbLf & vbLf)
                    i = -1
                    j = -1
                End If
            End While

            ' Add seed region to alignment
            i = stop1 - 1
            j = stop2 - 1
            While (i > start1) AndAlso (j > start2)
                s1_alignment.Append(seq1(i))
                alignment.Append(getMatchMismatchChar(seq1(i), seq2(j)))
                s2_alignment.Append(seq2(j))
                i -= 1
                j -= 1
            End While

            ' Reverse the alignment
            'JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
            s1_alignment.reverse()
            'JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
            alignment.reverse()
            'JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
            s2_alignment.reverse()
        End Sub

        ''' <summary>
        ''' Once an alignment score has been computed, the backtracking table is searched to
        ''' determine a StringBuilder representation of the alignment.
        ''' </summary>
        Private Sub backtrack_pre(s1_alignment As StringBuilder, alignment As StringBuilder, s2_alignment As StringBuilder)

            ' Begin backtracking search at optimal alignment score (found in last column in table)
            Dim i As Integer = optimalRow_pre
            Dim j As Integer = optimalCol_pre
            ' Last column in table
            ' Backtrack through table to beginning of alignment
            While (i >= 0) OrElse (j >= 0)
                If backtrack(i)(j) = 2 Then
                    ' Diagonal
                    s1_alignment.Append(seq1(start1 - i))
                    alignment.Append(getMatchMismatchChar(seq1(start1 - i), seq2(start2 - j)))
                    s2_alignment.Append(seq2(start2 - j))
                    i -= 1
                    j -= 1
                    ' Gap above
                ElseIf backtrack(i)(j) = 3 Then
                    s1_alignment.Append(seq1(start1 - i))
                    alignment.Append(" "c)
                    s2_alignment.Append("-"c)
                    i -= 1
                    ' Gap left
                ElseIf backtrack(i)(j) = 1 Then
                    s1_alignment.Append("-"c)
                    alignment.Append(" "c)
                    s2_alignment.Append(seq2(start2 - j))
                    j -= 1
                    ' DONE
                ElseIf backtrack(i)(j) = -1 Then
                    s1_alignment.Append(seq1(start1 - i))
                    alignment.Append(getMatchMismatchChar(seq1(start1 - i), seq2(start2 - j)))
                    s2_alignment.Append(seq2(start2 - j))
                    While i <> 0
                        ' We're in first column. Alignment begins with gaps.
                        s1_alignment.Append(seq1(start1 - i + 1))
                        alignment.Append(" "c)
                        s2_alignment.Append("-"c)
                        i -= 1
                    End While
                    While j <> 0
                        ' We're in first row. Alignment begins with gaps.
                        s1_alignment.Append("-"c)
                        alignment.Append(" "c)
                        s2_alignment.Append(seq2(start2 - j + 1))
                        j -= 1
                    End While
                    i -= 1
                    j -= 1
                Else
                    ' Impossible case.
                    Peregrine.output(vbLf & "There was an error when backtracking." & vbLf & vbLf)
                    i = -1
                    j = -1
                End If
            End While
        End Sub

        ''' <summary>
        ''' Returns 1 if the characters at the specified indices in
        ''' the two sequences mismatch.
        ''' Returns 0 if the characters at the specified indices in
        ''' the two sequence are the same.
        ''' </summary>
        Private Function mismatch(x As Integer, y As Integer) As Integer
            If seq1(x) = seq2(y) Then
                Return 0
            End If
            Return 1
        End Function

        ''' <summary>
        ''' Returns an alignment character if the two specified characters are the same.
        ''' Returns a space character otherwise.
        ''' </summary>
        Private Function getMatchMismatchChar(a As Char, b As Char) As Char
            If a = b Then
                Return "|"c
            Else
                Return " "c
            End If
        End Function

        ''' <summary>
        ''' Returns a String representation of a 2D integer array.
        ''' </summary>
        Private Function tableToString_post(a As Integer()()) As String
            Dim sb As New StringBuilder()
            For j As Integer = 0 To seq2.Length - stop2 - 1
                sb.Append(vbTab & seq2(stop2 + j))
            Next
            sb.AppendLine()
            For i As Integer = 0 To seq1.Length - stop1 - 1
                sb.Append(seq1(stop1 + i))
                For j As Integer = 0 To seq2.Length - stop2 - 1
                    sb.Append(vbTab & a(i)(j))
                Next
                sb.AppendLine()
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Returns a String representation of a 2D integer array.
        ''' </summary>
        Private Function tableToString_pre(a As Integer()()) As String
            Dim sb As New StringBuilder()
            For j As Integer = 0 To start2
                sb.Append(vbTab & seq2(start2 - j))
            Next
            sb.AppendLine()
            For i As Integer = 0 To start1
                sb.Append(seq1(start1 - i))
                For j As Integer = 0 To start2
                    sb.Append(vbTab & a(i)(j))
                Next
                sb.AppendLine()
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Returns the minimum of three integers.
        ''' </summary>
        Private Function min(a As Integer, b As Integer, c As Integer) As Integer
            Return Math.Min(a, Math.Min(b, c))
        End Function



        ''' <summary>
        '''*******************************************************
        ''' ****************** PRIVATE CLASS METHODS ****************
        ''' </summary>

        ''' <summary>
        ''' Reads in and returns a genomic sequence from the specified FASTA file.
        ''' </summary>
        ''' <param name="f">   a <code>File</code> object referring to a FASTA file containing a genomic sequence </param>
        ''' <returns>      the genomic sequence read in from a FASTA file </returns>
        Private Shared Function readSequenceFromFile(f As Oracle.Java.IO.File) As String
            Dim sequence As New StringBuilder()
            Try
                Dim reader As New Scanner(f)
                Dim header As String = reader.nextLine()
                ' Header line of FASTA file
                If (header.Length = 0) OrElse (header(0) <> ">"c) Then
                    Peregrine.output("Error - first line of file " & Convert.ToString(f) & " is not in FASTA format." & vbLf)
                    Return sequence.ToString()
                End If
                While reader.hasNext()
                    ' continue until we reach end of file
                    sequence.Append(reader.nextLine())
                End While
                reader.close()
            Catch e As FileNotFoundException
                Peregrine.output("Error - the file " & Convert.ToString(f) & " could not be found and opened." & vbLf)
                Return sequence.ToString()
            End Try
            Return sequence.ToString()
        End Function



        ''' <summary>
        '''*******************************************************
        ''' ****************** MAIN METHOD **************************
        ''' </summary>

        Private Shared Sub Main(args As String())

            If args.Length < 2 Then
                Oracle.Java.System.Err.println(vbLf & "When executing this program, please enter the name of two files,")
                Oracle.Java.System.Err.println("each containing a sequence. The program will align the two sequences." & vbLf)
                Oracle.Java.System.Err.println(vbTab & "java Alignment file1.txt file2.txt" & vbLf)
                Environment.[Exit](0)
            End If

            Dim a As New Alignment(2, 100)
            a.align(New Oracle.Java.IO.File(args(0)), New Oracle.Java.IO.File(args(1)))
            Console.WriteLine(a)
        End Sub

    End Class

End Namespace
