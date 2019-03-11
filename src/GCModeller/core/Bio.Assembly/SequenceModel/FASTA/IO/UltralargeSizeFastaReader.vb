#Region "Microsoft.VisualBasic::e8f33439e16ac8b06ffb6d7329537621, Bio.Assembly\SequenceModel\FASTA\IO\UltralargeSizeFastaReader.vb"

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

    '     Class UltralargeSizeFastaReader
    ' 
    '         Properties: Count, IsReadOnly
    ' 
    '         Function: __saveBlock, Contains, FastSplit, GetEnumerator, GetEnumerator1
    '                   IndexOf, ReadBuffer, Remove, Save, ToString
    ' 
    '         Sub: Add, Clear, CopyTo, Insert, RemoveAt
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel

Namespace SequenceModel.FASTA

    ''' <summary>
    ''' 超大的FASTA文件的读取工具
    ''' </summary>
    ''' <remarks></remarks>
    Public Class UltralargeSizeFastaReader : Inherits ITextFile

        Implements IDisposable
        Implements IEnumerable(Of FastaSeq)
        Implements IList(Of FastaSeq)

        Dim __linkList As LinkedList(Of FastaSeq)

        Public Overrides Function ToString() As String
            Return String.Format("{0}; [{1} records]", FilePath, Count)
        End Function

        ''' <summary>
        ''' 不敢保证正确率
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="n"></param>
        ''' <param name="outDIR"></param>
        ''' <param name="chunkSize"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        Public Shared Function FastSplit(path As String,
                                         n As Integer,
                                         outDIR As String,
                                         Optional chunkSize As Long = 1024 * 1024 * 128,
                                         Optional encoding As Encoding = Nothing) As Boolean

            Using IO As System.IO.FileStream = New IO.FileStream(path, System.IO.FileMode.Open)
                Dim chunkBuffer As Byte() = New Byte(chunkSize - 1) {}

                If encoding Is Nothing Then
                    encoding = Encoding.Default
                End If

                Dim index As Integer = 0
                Dim name As String = BaseName(path)
                Dim preBlock As Byte() = Nothing

                Do While ReadBuffer(IO, chunkBuffer)
                    Call preBlock.Add(chunkBuffer)
                    chunkBuffer = preBlock
                    Call __saveBlock(preBlock, index, name, encoding, n, outDIR, last_overrides:=False)
                    Call $"{100 * IO.Position / IO.Length}% ({IO.Position}/{IO.Length})".__DEBUG_ECHO
                Loop

                Call $"Save last block [{preBlock.Length} bytes]...".__DEBUG_ECHO
                Call __saveBlock(preBlock, index, name, encoding, n, outDIR, last_overrides:=True)
            End Using

            Return True
        End Function

        'Public Shared Iterator Function ForEachBlock(path As String,
        '                                    Optional chunkSize As Long = 1024 * 1024 * 128,
        '                                    Optional encoding As System.Text.Encoding = Nothing) As Generic.IEnumerable(Of FastaToken())
        '    Using IO As System.IO.FileStream = New IO.FileStream(path, System.IO.FileMode.Open)
        '        Dim chunkBuffer As Byte() = New Byte(chunkSize - 1) {}

        '        If encoding Is Nothing Then
        '            encoding = System.Text.Encoding.Default
        '        End If

        '        Dim index As Integer = 0
        '        Dim name As String = System.basename(path)
        '        Dim preBlock As Byte() = Nothing

        '        Do While ReadBuffer(IO, chunkBuffer)
        '            Call preBlock.Add(chunkBuffer)
        '            chunkBuffer = preBlock
        '            Call __saveBlock(preBlock, index, name, encoding, n, outDIR, last_overrides:=False)
        '            Call $"{100 * IO.Position / IO.Length}% ({IO.Position}/{IO.Length})".__DEBUG_ECHO
        '        Loop

        '        Call $"Save last block [{preBlock.Length} bytes]...".__DEBUG_ECHO
        '        Call __saveBlock(preBlock, index, name, encoding, n, outDIR, last_overrides:=True)
        '    End Using
        'End Function

        'Private Shared Function __readBlock() As FastaToken()

        'End Function

        Private Shared Function __saveBlock(ByRef preBlock As Byte(), ByRef index As Integer, name As String, encoding As System.Text.Encoding, n As Integer, outDIR As String, last_overrides As Boolean) As Boolean
            Dim str As String = encoding.GetString(preBlock)
            Dim list As String() = Regex.Split(str, "^>", RegexOptions.Multiline)

            list = (From s As String In list Where Not String.IsNullOrWhiteSpace(s) Select s).ToArray

            If list.Length < n Then
                If Not last_overrides Then
                    Return False
                Else
                    Dim path As String = $"{outDIR}/{name}.{index}.fasta"
                    index += 1
                    Return str.SaveTo(path)
                End If
            End If

            Dim splitFa As String()() = list.Split(n)

            For Each block As String() In splitFa
                str = (From line As String In block Select ">" & line).ToArray.JoinBy(vbCrLf)

                If block.Length = n Then
                    Dim path As String = $"{outDIR}/{name}.{index}.fasta"
                    index += 1
                    Call str.SaveTo(path)
                Else             ' 这个是最后一个元素了
                    preBlock = encoding.GetBytes(str)
                End If
            Next

            Return True
        End Function

        Public Shared Function ReadBuffer(IO As System.IO.Stream, ByRef readsBuffer As Byte()) As Boolean
            If IO.Position + readsBuffer.Length < IO.Length Then

            Else
                Dim delta As Long = IO.Length - IO.Position
                readsBuffer = New Byte(delta - 1) {}
                Call $“File Stream length is not enough, resize buffer size to {readsBuffer.Length}”.__DEBUG_ECHO
            End If

            Return IO.Read(readsBuffer, Scan0, readsBuffer.Length) > 0
        End Function

#Region "Implements Generic.IEnumerable(Of FastaObject)"

        Public Iterator Function GetEnumerator() As IEnumerator(Of FastaSeq) Implements IEnumerable(Of FastaSeq).GetEnumerator
            Dim Target = __linkList.First

            For i As Integer = 0 To __linkList.Count - 1
                Yield Target.Value
                Target = Target.Next
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region

#Region "Implements Generic.IList(Of SequenceModel.FASTA.FastaObject)"

        Public Sub Add(item As FastaSeq) Implements ICollection(Of FastaSeq).Add
            Call __linkList.AddLast(New LinkedListNode(Of FastaSeq)(item))
        End Sub

        Public Sub Clear() Implements ICollection(Of FastaSeq).Clear
            Call __linkList.Clear()
        End Sub

        Public Function Contains(item As FastaSeq) As Boolean Implements ICollection(Of FastaSeq).Contains
            Return __linkList.Contains(item)
        End Function

        Public Overloads Sub CopyTo(array() As FastaSeq, arrayIndex As Integer) Implements ICollection(Of FastaSeq).CopyTo
            Call __linkList.CopyTo(array, arrayIndex)
        End Sub

        Public ReadOnly Property Count As Integer Implements ICollection(Of FastaSeq).Count
            Get
                Return __linkList.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of FastaSeq).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function Remove(item As FastaSeq) As Boolean Implements ICollection(Of FastaSeq).Remove
            Return __linkList.Remove(item)
        End Function

        Public Function IndexOf(item As FastaSeq) As Integer Implements IList(Of FastaSeq).IndexOf
            Dim Target = __linkList.First

            For i As Integer = 0 To __linkList.Count - 1
                If Target.Value.Equals(item) Then
                    Return i
                End If
                Target = Target.Next
            Next

            Return -1
        End Function

        Public Sub Insert(index As Integer, item As FastaSeq) Implements IList(Of FastaSeq).Insert
            Dim Target = __linkList.First

            For i As Integer = 0 To index
                Target = Target.Next
            Next

            Call __linkList.AddBefore(Target, New LinkedListNode(Of FastaSeq)(item))
        End Sub

        ''' <summary>
        ''' 速度比较慢
        ''' </summary>
        ''' <param name="index"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public Property Item(index As Integer) As FastaSeq Implements IList(Of FastaSeq).Item
            Get
                Dim Target = __linkList.First

                For i As Integer = 0 To index
                    Target = Target.Next
                Next

                Return Target.Value
            End Get
            Set(value As FastaSeq)
                Dim Target = __linkList.First

                For i As Integer = 0 To index
                    Target = Target.Next
                Next

                Target.Value = value
            End Set
        End Property

        Public Sub RemoveAt(index As Integer) Implements IList(Of FastaSeq).RemoveAt
            Dim Target = __linkList.First

            For i As Integer = 0 To index
                Target = Target.Next
            Next

            Call __linkList.Remove(Target)
        End Sub
#End Region

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return False
        End Function

    End Class
End Namespace
