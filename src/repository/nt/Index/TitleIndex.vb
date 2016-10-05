Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text

Public Class TitleIndex : Inherits IndexAbstract
    Implements IDisposable

    ReadOnly __handle As BinaryDataReader
    ReadOnly __index As New SortedDictionary(Of String, BlockRange)

    Public ReadOnly Property Size As Long
        Get
            Return __handle.Length
        End Get
    End Property

    Public ReadOnly Property giKeys As IEnumerable(Of String)
        Get
            Return __index.Keys
        End Get
    End Property

    Public Sub New(DATA$, db$, uid$)
        MyBase.New(uid)

        Dim path$
        Dim file As FileStream

        path = $"{DATA}/headers/{db}/{uid}.txt"

        file = IO.File.OpenRead(path)
        __handle = New BinaryDataReader(file, Encodings.ASCII)
        Call makeIndex($"{DATA}/headers/index/{db}/{uid}.index")
    End Sub

    Private Sub makeIndex(path$)
        Using indexReader As New BinaryDataReader(File.OpenRead(path$), Encodings.ASCII)
            Dim start&, len%
            Dim gi&

            Do While Not indexReader.EndOfStream
                gi& = indexReader.ReadInt64  ' gi编号
                start& = indexReader.ReadInt64  ' 序列的起始位置
                len% = indexReader.ReadInt32  ' 序列的bytes的长度

                Call __index.Add(
                    CStr(gi), New BlockRange With {
                        .start = start,
                        .len = len
                    })
            Loop
        End Using
    End Sub

    Public Function ReadHeader_by_gi(gi$) As String
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