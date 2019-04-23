#Region "Microsoft.VisualBasic::eff06b7025b8c1c0e14304c5078d4d17, Data\BinaryData\BinaryData\Repository\BinarySearchIndex.vb"

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

' Class BinarySearchIndex
' 
' 
' 
' Class Index
' 
'     Properties: Key, left, Offset, right
' 
'     Function: Write
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Parser

''' <summary>
''' 主要是针对于字符串类型的索引文件的构建, 在这里尝试使用字典树来节省存储空间
''' </summary>
Public Class TrieIndexWriter : Implements IDisposable

    ReadOnly index As BinaryDataWriter
    ReadOnly reader As BinaryDataReader
    ReadOnly root As Long

    Dim length As Long

    Sub New(IOdev As BinaryDataWriter)
        index = IOdev
        index.Write("TrieIndex", BinaryStringFormat.NoPrefixOrTermination)
        reader = New BinaryDataReader(IOdev.BaseStream, leaveOpen:=True)

        ' all of the term starts from here
        root = index.Position
        reader.Seek(root, SeekOrigin.Begin)
    End Sub

    Public Sub buildIndex(terms As IEnumerable(Of String))
        For Each word As String In terms
            Call AddTerm(word)
        Next
    End Sub

    ''' <summary>
    ''' Only supports ASCII symbols
    ''' </summary>
    ''' <param name="term"></param>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub AddTerm(term As String)
        Dim chars As CharPtr = term
        Dim c As Char
        Dim offset As Integer
        Dim i As Integer

        If chars = 0 Then
            ' empty string data
            Return
        Else
            Call index.Seek(root, SeekOrigin.Begin)
        End If

        Do While Not chars.EndRead
            c = ++chars
            offset = getNextOffset(c, i)

            If offset = -1 Then
                ' character c is not exists in current tree routine

            Else
                Call index.Seek(offset, SeekOrigin.Current)
            End If
        Loop
    End Sub

    ' offset block is pre-allocated block
    ' length is 26+26+10 * 4 bytes
    ' char offset offset offset offset jump offset_next_block offset ZERO
    ' offset is the location offset for next char

    ''' <summary>
    ''' 这个函数是以当前的位置为参考的
    ''' </summary>
    ''' <param name="c"></param>
    ''' <returns></returns>
    Private Function getNextOffset(c As Char, ByRef i As Integer) As Integer
        Dim code As Integer = Asc(c)
        Dim read As Integer
        Dim offset As Integer

        ' 0 表示结束
        ' -max 表示跳转
        For i = 0 To Integer.MaxValue Step 4
            ' read offset
            offset = reader.ReadInt32

            If offset = 0 Then
                ' 当前的字典已经结束了
                Return -1
            ElseIf offset = Integer.MinValue Then
                ' jump
                ' next integer is the jump offset
                offset = reader.ReadInt32
                ' jump to next block
                reader.Seek(offset, SeekOrigin.Current)
                offset = reader.ReadInt32
            End If

            reader.TemporarySeek(offset, SeekOrigin.Current, Sub() read = reader.ReadInt32)

            If read = code Then
                ' is target char
                Return offset
            End If
        Next

        Return -1
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Call index.Flush()
                Call index.Close()
                Call index.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class

Public Class CharNode

    Public Property [char] As Char

    ''' <summary>
    ''' 在这里是接下来的字符的在索引文件之中的位置
    ''' </summary>
    ''' <returns></returns>
    Public Property [next] As Long()

End Class