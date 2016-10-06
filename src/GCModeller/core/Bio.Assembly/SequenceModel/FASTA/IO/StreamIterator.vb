#Region "Microsoft.VisualBasic::eb15d1dd3296a1d6b72f390cb363216b, ..\GCModeller\core\Bio.Assembly\SequenceModel\FASTA\IO\StreamIterator.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SequenceModel.FASTA

    Public Class StreamIterator : Inherits BufferedStream

        Sub New(path As String)
            Call MyBase.New(path, maxBufferSize:=1024 * 1024 * 128)
        End Sub

        ''' <summary>
        ''' Read all sequence from the fasta file.
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function ReadStream() As IEnumerable(Of FastaToken)
            Dim stream As New List(Of String)

            Do While Not Me.EndRead  ' 一直循环直到读完文件为止
                For Each fa As FastaToken In __loops(stream)
                    Yield fa
                Next
            Loop

            If Not stream.Count = 0 Then
                Yield FastaToken.ParseFromStream(stream, {"|"c})
            End If
        End Function

        Public Const SOH As Char = Chr(1)

        ReadOnly __deli As Char() = {"|"c}

        ''' <summary>
        ''' Loops on each block of data
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <returns></returns>
        Private Iterator Function __loops(stream As List(Of String)) As IEnumerable(Of FastaToken)
            For Each line As String In MyBase.BufferProvider   ' 读取一个数据块
                If line.IsBlank Then  ' 跳过空白的行
                    Continue For
                End If

                If line.First = ">"c AndAlso stream.Count > 0 Then  ' 在这里碰见了一个fasta头部

                    ' 则解析临时数据，然后清空临时缓存变量
                    Dim fa As FastaToken = FastaToken.ParseFromStream(stream, __deli)
                    Yield fa

                    stream.Clear()
                End If

                ' 因为当前行可能是起始于 >，即fasta序列的头部，则会在if之中清空数据，则在这里添加的是新的fasta头部
                Call stream.Add(line)  ' 否则添加当前行到缓存之中
            Next

            ' 循环完毕，但是stream缓存之中可能还有数据剩余
        End Function

        ''' <summary>
        ''' 子集里面的序列元素的数目
        ''' </summary>
        ''' <param name="size"></param>
        ''' <returns></returns>
        Public Iterator Function Split(size As Integer) As IEnumerable(Of FastaFile)
            Dim temp As New List(Of FastaToken)
            Dim i As Integer

            Call MyBase.Reset()

            For Each fa As FastaToken In Me.ReadStream
                If i < size Then
                    Call temp.Add(fa)
                    i += 1
                Else
                    i = 0
                    Yield New FastaFile(temp)
                    Call temp.Clear()
                End If
            Next

            If Not temp.Count = 0 Then
                Yield New FastaFile(temp)
            End If
        End Function

        ''' <summary>
        ''' 默认的Fasta文件拓展名列表
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property DefaultSuffix As String() = {"*.fasta", "*.fa", "*.fsa", "*.fas"}

        ''' <summary>
        ''' 全部都是使用<see cref="StreamIterator"/>对象来进行读取的
        ''' </summary>
        ''' <param name="handle">File path or directory.</param>
        ''' <returns></returns>
        Public Shared Iterator Function SeqSource(handle As String, Optional ext As String() = Nothing, Optional debug As Boolean = False) As IEnumerable(Of FastaToken)
            If handle.FileExists Then
                If debug Then
                    Call "File exists, reading fasta data from file...".__DEBUG_ECHO
                End If

                For Each fa As FastaToken In New StreamIterator(handle).ReadStream
                    Yield fa
                Next
            Else
                If ext.IsNullOrEmpty Then
                    ext = DefaultSuffix
                End If
                If debug Then
                    Call "Directory exists, reading fasta data from files in DATA directory...".__DEBUG_ECHO
                    Call $"File types: {ext.GetJson}".__DEBUG_ECHO
                End If

                For Each file As String In ls - l - r - wildcards(ext) <= handle
                    If debug Then
                        Call file.ToFileURL.__DEBUG_ECHO
                    End If
                    For Each nt As FastaToken In New StreamIterator(file).ReadStream
                        Yield nt
                    Next
                Next
            End If
        End Function
    End Class
End Namespace
