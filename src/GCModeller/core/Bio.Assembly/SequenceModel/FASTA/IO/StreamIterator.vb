#Region "Microsoft.VisualBasic::9bf2e39c6d6b51cf3bf926f6c49d630c, GCModeller\core\Bio.Assembly\SequenceModel\FASTA\IO\StreamIterator.vb"

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


    ' Code Statistics:

    '   Total Lines: 170
    '    Code Lines: 96
    ' Comment Lines: 46
    '   Blank Lines: 28
    '     File Size: 6.68 KB


    '     Class StreamIterator
    ' 
    '         Properties: DefaultSuffix
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __loops, BufferProvider, ReadStream, SeqSource, Split
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SequenceModel.FASTA

    ''' <summary>
    ''' 读取超大型的fasta文件所需要的一个数据对象
    ''' </summary>
    Public Class StreamIterator : Implements IDisposable

        ReadOnly _file As StreamReader

        ''' <summary>
        ''' 从指定的文件之中构建一个读取超大型的fasta文件所需要的一个数据对象
        ''' </summary>
        ''' <param name="path"></param>
        Sub New(path As String)
            _file = path.OpenReader()
        End Sub

        ''' <summary>
        ''' Read all sequence from the fasta file.
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function ReadStream() As IEnumerable(Of FastaSeq)
            Dim stream As New List(Of String)

            For Each fa As FastaSeq In __loops(stream)
                Yield fa
            Next

            If Not stream.Count = 0 Then
                Yield FastaSeq.ParseFromStream(stream, {"|"c})
            End If
        End Function

        Private Iterator Function BufferProvider() As IEnumerable(Of String)
            Do While Not _file.EndOfStream
                Yield _file.ReadLine
            Loop
        End Function

        Public Const SOH As Char = Chr(1)

        ReadOnly __deli As Char() = {"|"c}
        Private disposedValue As Boolean

        ''' <summary>
        ''' Loops on each block of data
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <returns></returns>
        Private Iterator Function __loops(stream As List(Of String)) As IEnumerable(Of FastaSeq)
            For Each line As String In BufferProvider()   ' 读取一个数据块
                If line.StringEmpty Then  ' 跳过空白的行
                    Continue For
                End If

                If line.First = ">"c AndAlso stream.Count > 0 Then  ' 在这里碰见了一个fasta头部

                    ' 则解析临时数据，然后清空临时缓存变量
                    Dim fa As FastaSeq = FastaSeq.ParseFromStream(stream, __deli)
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
            Dim temp As New List(Of FastaSeq)
            Dim i As Integer

            Call _file.BaseStream.Seek(Scan0, SeekOrigin.Begin)

            For Each fa As FastaSeq In Me.ReadStream
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
        Public Shared ReadOnly Property DefaultSuffix As [Default](Of String()) = {"*.fasta", "*.fa", "*.fsa", "*.fas"}

        ''' <summary>
        ''' 全部都是使用<see cref="StreamIterator"/>对象来进行读取的
        ''' </summary>
        ''' <param name="handle">File path or directory.(函数会自动检测所输入的路径的类型为文件夹或者是文件)</param>
        ''' <param name="ext">
        ''' 文件搜索的文件名匹配模式，如果<paramref name="handle"/>是一个文件夹的话
        ''' </param>
        ''' <returns></returns>
        Public Shared Iterator Function SeqSource(handle$, Optional ext$() = Nothing, Optional debug As Boolean = False) As IEnumerable(Of FastaSeq)
            If (handle.Last <> "/"c AndAlso handle.Last <> "\"c) AndAlso handle.FixPath.FileExists Then
                If debug Then
                    Call "File exists, reading fasta data from file...".__DEBUG_ECHO
                End If

                For Each fa As FastaSeq In New StreamIterator(handle).ReadStream
                    Yield fa
                Next
            Else
                If debug Then
                    Call "Directory exists, reading fasta data from files in DATA directory...".__DEBUG_ECHO
                    Call $"File types: {(ext Or DefaultSuffix).GetJson}".__DEBUG_ECHO
                End If

                For Each file As String In ls - l - r - (ext Or DefaultSuffix) <= handle
                    If debug Then
                        Call file.ToFileURL.__DEBUG_ECHO
                    End If
                    For Each nt As FastaSeq In New StreamIterator(file).ReadStream
                        Yield nt
                    Next
                Next
            End If
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call _file.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
