Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text

Public Class TitleIndex : Inherits IndexAbstract
    Implements IDisposable

    ReadOnly __handle As ReaderProvider
    ReadOnly __index As New SortedDictionary(Of String, BlockRange)
    ReadOnly __locus_tagIndex As New SortedDictionary(Of String, String)

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

        path = $"{DATA}/headers/{db}/{uid}.txt"
        __handle = New ReaderProvider(path, Encodings.ASCII)
        makeIndex($"{DATA}/headers/index/{db}/{uid}.index")
    End Sub

    Private Sub makeIndex(path$)
        Using indexReader As New BinaryDataReader(File.OpenRead(path$), Encodings.ASCII),
            __handle As New BinaryDataReader(File.OpenRead(Me.__handle.URI), Encodings.ASCII)

            Dim start&, len%
            Dim gi&
            Dim gi_start&, gi_end&, gi_len%
            Dim locus_tag$

            Do While Not indexReader.EndOfStream
                gi& = indexReader.ReadInt64  ' gi编号
                start& = indexReader.ReadInt64  ' 序列的起始位置
                len% = indexReader.ReadInt32  ' 序列的bytes的长度

                gi_end& = start - tab.Length
                gi_len% = gi_end - gi_start
                locus_tag = __handle.ReadChars(gi_len%)
                locus_tag = Regex.Replace(locus_tag, "\.\d*", "").ToLower

                Call __index.Add(
                    CStr(gi), New BlockRange With {
                        .start = start,
                        .len = len
                    })
                Call __locus_tagIndex.Add(locus_tag, CStr(gi))

                gi_start = start + len + lf.Length
                __handle.Seek(gi_start, SeekOrigin.Begin)
            Loop
        End Using
    End Sub

    Public Function ReadHeader_by_gi(gi$) As String
        If Not __index.ContainsKey(gi$) Then
            Return Nothing
        End If

        Dim range As BlockRange = __index(gi$)
        Dim value As String = Nothing

        Call __handle.Read(Sub(__handle)
                               ' 这里使用匿名方法会造成程序运行时上面的性能损失，
                               ' 但是由于这点性能损失相比较于网络传输的延时是可以被忽略掉的，
                               ' 并且通过这个匿名函数可以提升数据库索引服务器在I/O方面的效率
                               Call __handle.Seek(range.start, SeekOrigin.Begin)
                               value = New String(__handle.ReadChars(range.len))
                           End Sub)
        Return value
    End Function

    Public Function ReadHeader_by_locus_Tag(locus_Tag$) As String
        locus_Tag = Regex.Replace(locus_Tag, "\.\d*", "")
        locus_Tag = locus_Tag.ToLower

        If Not __locus_tagIndex.ContainsKey(locus_Tag$) Then
            Return Nothing
        End If

        Dim range As BlockRange = __index(__locus_tagIndex(locus_Tag$))
        Dim value As String = Nothing

        Call __handle.Read(Sub(__handle)
                               Call __handle.Seek(range.start, SeekOrigin.Begin)
                               value = New String(__handle.ReadChars(range.len))
                           End Sub)
        Return value
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        Call __handle.Dispose()
        MyBase.Dispose(disposing)
    End Sub
End Class