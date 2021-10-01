#Region "Microsoft.VisualBasic::8bbf6afe9926693b5fabf78c6da8d401, nt\Index\Index.vb"

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

    ' Class Index
    ' 
    '     Properties: Size, URI
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: ReadNT_by_gi
    ' 
    '     Sub: Dispose, MakeIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text

''' <summary>
''' 序列数据的索引服务
''' </summary>
Public Class Index : Inherits IndexAbstract
    Implements IDisposable

    ''' <summary>
    ''' 索引文件的文件路径
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property URI As String

    ''' <summary>
    ''' ``{nt_gi, <see cref="BlockRange"/>}``.(序列数据的读取范围)
    ''' </summary>
    ReadOnly __index As New SortedDictionary(Of String, BlockRange)
    ReadOnly __reader As ReaderProvider

    Public ReadOnly Property Size As Long

    Sub New(Data$, db$, index$)
        MyBase.New(index$)

        Dim path$ = $"{Data}/{db}/{index}.nt"
        URI = path
        Size = FileIO.FileSystem.GetFileInfo(URI).Length
        Call MakeIndex(path:=$"{Data}/index/{db}/{index}.index")
        __reader = New ReaderProvider(URI,,)
    End Sub

    Private Sub MakeIndex(path$)
        Using indexReader As New BinaryDataReader(File.OpenRead(path$), Encodings.ASCII),
            __handle As New BinaryDataReader(File.OpenRead(URI), Encodings.ASCII)

            Dim start&, len%
            Dim gi_start&, gi_end&, gi_len%
            Dim gi$

            Do While Not indexReader.EndOfStream
                start& = indexReader.ReadInt64  ' 序列的起始位置
                len% = indexReader.ReadInt32  ' 序列的bytes的长度

                gi_end& = start - tab.Length
                gi_len% = gi_end - gi_start
                gi = __handle.ReadChars(gi_len)

                If __index.ContainsKey(gi) Then
                    Call __index.Remove(gi)
                    Call $"GI:={gi} was duplicated in the index...".Warning
                End If

                Call __index.Add(
                    gi, New BlockRange With {
                        .start = start,
                        .len = len
                    })

                gi_start = start + len + lf.Length
                __handle.Seek(gi_start, SeekOrigin.Begin)
            Loop
        End Using
    End Sub

    Public Function ReadNT_by_gi(gi$) As String
        If Not __index.ContainsKey(gi$) Then
            Return Nothing
        End If

        Dim range As BlockRange = __index(gi$)
        Dim value As String = Nothing

        Call __reader.Read(Sub(__handle)
                               Call __handle.Seek(range.start, SeekOrigin.Begin)
                               value = New String(__handle.ReadChars(range.len))
                           End Sub)
        Return value
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub
End Class
