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
Imports System.Text
Imports Microsoft.VisualBasic.Text.Parser

''' <summary>
''' 主要是针对于字符串类型的索引文件的构建, 在这里尝试使用字典树来节省存储空间
''' 只支持ASCII字符
''' </summary>
Public Class TrieIndexWriter : Implements IDisposable

    ReadOnly index As BinaryDataWriter
    ReadOnly reader As BinaryDataReader
    ReadOnly root As Long

    Dim length As Long

    Sub New(IOdev As Stream)
        index = New BinaryDataWriter(IOdev, encoding:=Encoding.ASCII)
        index.Write("TrieIndex", BinaryStringFormat.NoPrefixOrTermination)
        reader = New BinaryDataReader(IOdev, leaveOpen:=True)

        ' all of the term starts from here
        root = index.Position
        reader.Seek(root, SeekOrigin.Begin)
    End Sub

    ''' <summary>
    ''' Only supports ASCII symbols
    ''' </summary>
    ''' <param name="term"></param>
    ''' <param name="data">
    ''' 与当前的term所关联的数据块的位置值
    ''' </param>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub AddTerm(term As String, data As Long)
        Dim chars As CharPtr = term
        Dim c As Integer
        Dim offset As Integer
        Dim current As Long

        If chars = 0 Then
            ' empty string data
            Return
        Else
            Call index.Seek(root, SeekOrigin.Begin)
        End If

        Do While Not chars.EndRead
            c = Asc(++chars)
            current = reader.Position
            offset = getNextOffset(c)

            If offset = -1 Then
                ' character c is not exists in current tree routine
                Dim blocks As Integer = (length - current) / allocateSize
                ' write next offset 
                index.Seek(current + 8 + (c - base) * 4, SeekOrigin.Begin)
                index.Write(blocks)
                ' jump to location
                index.Position = length
                ' write data block pointer
                index.Seek(8, SeekOrigin.Current)
                ' write pre-allocated block
                index.Seek(allocateSize, SeekOrigin.Current)

                current = index.Position
                length = index.Position
            Else
                Call index.Seek(offset, SeekOrigin.Current)
            End If
        Loop

        ' End of the charaters is the data entry that associated with current term
        index.Seek(-(allocateSize - 8), SeekOrigin.Current)
        index.Write(data)
    End Sub

    ''' <summary>
    ''' Printable characters with a ZERO terminated mark
    ''' </summary>
    Const allocateSize As Integer = (126 - 32 + 1) * 4 + 8

    ' offset block is pre-allocated block
    ' length is (126-32) * 4 + 4 bytes

    ' block_jump offset offset offset offset offset ZERO
    ' 1. offset is the block count for next char
    ' 2. block_jump is the data location that associated with current term string.

    ''' <summary>
    ''' 32
    ''' </summary>
    Const base As Integer = Asc(" "c)

    ''' <summary>
    ''' 这个函数是以当前的位置为参考的
    ''' </summary>
    ''' <param name="code"></param>
    ''' <returns></returns>
    Private Function getNextOffset(code As Integer) As Integer
        ' character block counts
        Dim offset As Integer

        reader.Seek((code - base) * 4, SeekOrigin.Current)
        offset = reader.ReadInt32

        If offset = 0 Then
            Return -1
        Else
            Return offset * allocateSize
        End If
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