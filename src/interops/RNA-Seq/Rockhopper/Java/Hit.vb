#Region "Microsoft.VisualBasic::78393e99db733d07f62ceec6ea5d46c4, RNA-Seq\Rockhopper\Java\Hit.vb"

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

    '     Class Hit
    ' 
    '         Properties: [stop], errors, length, repliconIndex, start
    '                     strand
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: compareTo, getFormattedHit, ToString
    ' 
    '         Sub: combinePairedEndHits
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
    ''' A Hit represents an alignment between a read and a genome.
    ''' </summary>
    Public Class Hit : Implements IComparable(Of Hit)

        ''' <summary>
        ''' Start coordinate of alignment in genome
        ''' </summary>
        Private _start As Integer
        ''' <summary>
        ''' Stop coordinate of alignment in genome
        ''' </summary>
        Private _stop As Integer
        ''' <summary>
        ''' Strand of alignment in genome
        ''' </summary>
        Private _strand As Char
        ''' <summary>
        ''' Number of errors/mismatches in alignment
        ''' </summary>
        Private _errors As Integer
        ''' <summary>
        ''' The index of the replicon to which the read was mapped
        ''' </summary>
        Private _repliconIndex As Integer

        Public Sub New(start As Integer, [stop] As Integer, strand As Char, errors As Integer, repliconIndex As Integer)
            Me._start = start
            Me._stop = [stop]
            Me._strand = strand
            Me._errors = errors
            Me._repliconIndex = repliconIndex
        End Sub

        Public Overridable Function getFormattedHit(sequence As String, read As String) As String
            Dim sb As New StringBuilder(read.Length * 3 + 40)
            sb.Append(vbTab & (_start + 1) & vbTab)
            Dim subSequence As String = sequence.Substring(_start - 1, _stop - (_start - 1))
            If _strand = "-"c Then
                subSequence = Index.reverseComplement(subSequence)
            End If
            sb.Append(subSequence)
            sb.Append(vbTab & (_stop + 1) & vbTab & "(" & _strand & ")" & vbLf & vbTab & vbTab)
            For i As Integer = 0 To read.Length - 1
                If subSequence(i) = read(i) Then
                    sb.Append("|"c)
                Else
                    sb.Append(" "c)
                End If
            Next
            sb.Append(vbTab & _errors & vbLf & vbTab & vbTab)
            sb.Append(read)
            sb.AppendLine()
            Return sb.ToString()
        End Function

        Public Overridable Overloads Function ToString() As String
            Return _start & vbTab & _stop & vbTab & _strand & vbTab & _errors
        End Function

        Public Overridable ReadOnly Property start() As Integer
            Get
                Return _start
            End Get
        End Property

        Public Overridable ReadOnly Property [stop]() As Integer
            Get
                Return _stop
            End Get
        End Property

        Public Overridable ReadOnly Property strand() As Char
            Get
                Return _strand
            End Get
        End Property

        Public Overridable ReadOnly Property errors() As Integer
            Get
                Return _errors
            End Get
        End Property

        Public Overridable ReadOnly Property repliconIndex() As Integer
            Get
                Return Me._repliconIndex
            End Get
        End Property

        Public Overridable ReadOnly Property length() As Integer
            Get
                Return _stop - _start + 1
            End Get
        End Property

        Public Overridable Function compareTo(h As Hit) As Integer Implements IComparable(Of Hit).CompareTo
            If Me._repliconIndex < h._repliconIndex Then
                Return -1
            ElseIf Me._repliconIndex > h._repliconIndex Then
                Return 1
            Else
                ' Same replicon
                If Me._start < h._start Then
                    Return -1
                ElseIf Me._start > h._start Then
                    Return 1
                Else
                    ' start coordinates are equal
                    If Me._stop < h._stop Then
                        Return -1
                    ElseIf Me._stop > h._stop Then
                        Return -1
                    Else
                        ' stop coordinates are equal
                        If (Me._strand = "+"c) AndAlso (h._strand = "-"c) Then
                            Return -1
                        ElseIf (Me._strand = "-"c) AndAlso (h._strand = "+"c) Then
                            Return 1
                        Else
                            Return 0
                        End If
                    End If
                End If
            End If
        End Function



        ''' <summary>
        '''********************************************
        ''' **********   Public Class Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Combines two lists of Hits, one for each read in a paired-end read,
        ''' into a list of paired-end Hits. A combined Hit occurs when
        ''' there are Hits from each of the two input lists that are
        ''' on the same strand and within the specified distance from each other.
        ''' </summary>
        Public Shared Sub combinePairedEndHits(combinedHits As List(Of Hit), hits1 As List(Of Hit), hits2 As List(Of Hit), maxPairedEndLength As Integer)
            combinedHits.Clear()
            For Each h1 As Hit In hits1
                For Each h2 As Hit In hits2
                    If (h1._strand = h2._strand) AndAlso (h1._repliconIndex = h2._repliconIndex) Then
                        If Math.Max(h1._stop, h2._stop) - Math.Min(h1._start, h2._start) <= maxPairedEndLength Then
                            combinedHits.Add(New Hit(Math.Min(h1._start, h2._start), Math.Max(h1._stop, h2._stop), h1._strand, Math.Max(h1._errors, h2._errors), h1._repliconIndex))
                            Exit For
                        End If
                    End If
                Next
            Next
        End Sub
    End Class

End Namespace
