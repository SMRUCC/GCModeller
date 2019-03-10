﻿#Region "Microsoft.VisualBasic::c428122243f1935b1ebab6d70e85e78b, RNA-Seq\RNA-seq.Data\FastQ\FastQFile.vb"

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

    '     Class FastQFile
    ' 
    '         Properties: IsReadOnly, NumOfReads
    ' 
    '         Function: __trim, Contains, GetEnumerator, GetEnumerator1, IndexOf
    '                   Load, Remove, Save, ToFasta
    ' 
    '         Sub: Add, Clear, CopyTo, Insert, RemoveAt
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Text

Namespace FQ

    ''' <summary>
    ''' There is no standard file extension for a FASTQ file, but .fq and .fastq, are commonly used.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FastQFile : Inherits ITextFile
        Implements IEnumerable(Of FastQ)
        Implements IList(Of FastQ)

        Dim _innerList As New List(Of FastQ)

        ''' <summary>
        ''' Load the fastq data from a specific file handle.(从一个特定的文件句柄之中加载fastq文件的数据)
        ''' </summary>
        ''' <param name="path">The file handle of the fastq data.</param>
        ''' <returns></returns>
        Public Shared Function Load(path As String, Optional encoding As Encodings = Encodings.Default) As FastQFile
            Dim FastaqFile As New FastQFile With {
                ._innerList = Stream.ReadAllLines(path, encoding).AsList,
                .FilePath = path
            }

            Return FastaqFile
        End Function

#Region "Implements Generic.IEnumerable(Of Fastaq)"

        Public Iterator Function GetEnumerator() As IEnumerator(Of FastQ) Implements IEnumerable(Of FastQ).GetEnumerator
            For Each Fastaq As FastQ In _innerList
                Yield Fastaq
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region

#Region "Implements Generic.IList(Of Fastaq)"

        Public Sub Add(item As FastQ) Implements ICollection(Of FastQ).Add
            _innerList.Add(item)
        End Sub

        Public Sub Clear() Implements ICollection(Of FastQ).Clear
            _innerList.Clear()
        End Sub

        Public Function Contains(item As FastQ) As Boolean Implements ICollection(Of FastQ).Contains
            Throw New NotImplementedException
        End Function

        Public Overloads Sub CopyTo(array() As FastQ, arrayIndex As Integer) Implements ICollection(Of FastQ).CopyTo
            _innerList.CopyTo(array, arrayIndex)
        End Sub

        Public ReadOnly Property NumOfReads As Integer Implements ICollection(Of FastQ).Count
            Get
                Return _innerList.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of FastQ).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function Remove(item As FastQ) As Boolean Implements ICollection(Of FastQ).Remove
            Throw New NotImplementedException
        End Function

        Public Function IndexOf(item As FastQ) As Integer Implements IList(Of FastQ).IndexOf
            Throw New NotImplementedException
        End Function

        Public Sub Insert(index As Integer, item As FastQ) Implements IList(Of FastQ).Insert
            _innerList.Insert(index, item)
        End Sub

        Default Public Property Item(index As Integer) As FastQ Implements IList(Of FastQ).Item
            Get
                Return Me._innerList(index)
            End Get
            Set(value As FastQ)
                Me._innerList(index) = value
            End Set
        End Property

        Public Sub RemoveAt(index As Integer) Implements IList(Of FastQ).RemoveAt
            _innerList.RemoveAt(index)
        End Sub
#End Region

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return WriteFastQ(FilePath)
        End Function

        ''' <summary>
        ''' Convert fastaq data into a fasta data file.
        ''' </summary>
        ''' <returns></returns>
        Public Function ToFasta(Optional index As Boolean = False) As FASTA.FastaFile
            Dim sw As Stopwatch = Stopwatch.StartNew

            Call "Start to convert fastq to fastq...".__DEBUG_ECHO

            Dim __attrs As Func(Of Integer, FastQ, String())

            If index Then
                __attrs = Function(i, fq) {$"lcl={i} ", fq.SEQ_ID.ToString}
            Else
                __attrs = Function(i, fq) {fq.SEQ_ID.ToString}
            End If

            Dim LQuery As FASTA.FastaSeq() =
                LinqAPI.Exec(Of FASTA.FastaSeq) <= From fq As SeqValue(Of FastQ)
                                                     In Me.SeqIterator.AsParallel
                                                     Let read As FastQ = fq.value
                                                     Let attrs As String() = __trim(__attrs(fq.i, read))
                                                     Select fasta = New FASTA.FastaSeq With {
                                                         .SequenceData = read.SequenceData,
                                                         .Headers = attrs
                                                     }
                                                     Order By fasta.Headers.First Ascending

            Call $"[Job Done!] {sw.ElapsedMilliseconds}ms...".__DEBUG_ECHO

            Return New FASTA.FastaFile(LQuery)
        End Function

        Private Shared Function __trim(attrs As String()) As String()
            Dim last As String = attrs.Last

            If last.First = "@"c Then
                attrs(attrs.Length - 1) = Mid(last, 2).Trim
            End If

            Return attrs
        End Function
    End Class
End Namespace
