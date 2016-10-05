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
    ''' 序列文件的文件句柄
    ''' </summary>
    ReadOnly __handle As BinaryDataReader
    ''' <summary>
    ''' ``{nt_gi, <see cref="BlockRange"/>}``.(序列数据的读取范围)
    ''' </summary>
    ReadOnly __index As New SortedDictionary(Of String, BlockRange)

    Public ReadOnly Property Size As Long
        Get
            Return __handle.Length
        End Get
    End Property

    Sub New(Data$, db$, index$)
        MyBase.New(index$)

        Dim path$ = $"{Data}/{db}/{index}.nt"
        __handle = path.OpenBinaryReader
        Call MakeIndex(path:=$"{Data}/index/{db}/{index}.index")
        __handle.Seek(Scan0, SeekOrigin.Begin)
    End Sub

    Private Sub MakeIndex(path$)
        Using indexReader As New BinaryDataReader(File.OpenRead(path$), Encodings.ASCII)
            Dim start&, len%
            Dim gi_start&, gi_end&, gi_len%
            Dim gi$

            Do While Not indexReader.EndOfStream
                start& = indexReader.ReadInt64  ' 序列的起始位置
                len% = indexReader.ReadInt32  ' 序列的bytes的长度

                gi_end& = start - tab.Length
                gi_len% = gi_end - gi_start
                gi = __handle.ReadChars(gi_len)

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

        SyncLock __handle
            Call __handle.Seek(range.start, SeekOrigin.Begin)
            Return New String(__handle.ReadChars(range.len))
        End SyncLock
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        __handle.Close()
        __handle.Dispose()
        MyBase.Dispose(disposing)
    End Sub
End Class
