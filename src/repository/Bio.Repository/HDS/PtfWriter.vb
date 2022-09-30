Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Annotation.Ptf

Public Class PtfWriter : Implements IDisposable

    ReadOnly stream As StreamPack
    ReadOnly id_mapping As Dictionary(Of String, Dictionary(Of String, String))

    Private disposedValue As Boolean

    Sub New(file As Stream, id_mapping As String())
        Me.stream = New StreamPack(file)
        Me.id_mapping = initIndex(id_mapping)
    End Sub

    Sub New(file As String, id_mapping As String())
        Me.stream = StreamPack.CreateNewStream(file)
        Me.id_mapping = initIndex(id_mapping)
    End Sub

    Private Shared Function initIndex(id_mapping As String()) As Dictionary(Of String, Dictionary(Of String, String))
        Return id_mapping _
            .ToDictionary(Function(dbname) dbname,
                          Function(any)
                              Return New Dictionary(Of String, String)
                          End Function)
    End Function

    Public Sub AddProtein(protein As ProteinAnnotation)
        Dim intptr As String = $"/annotation/{protein.geneId}.ptf"
        Dim file As New BinaryDataWriter(stream.OpenBlock(intptr)) With {
            .ByteOrder = ByteOrder.BigEndian,
            .Encoding = Encodings.ASCII.CodePage
        }

        Call WriteBytes(file, protein)
        Call file.Flush()
        Call file.Dispose()

        For Each dbname In id_mapping
            If protein.has(dbname.Key) Then
                For Each xref As String In protein(dbname.Key)
                    Call dbname.Value.Add(xref, protein.geneId)
                Next
            End If
        Next
    End Sub

    Public Shared Sub WriteBytes(block As BinaryDataWriter, protein As ProteinAnnotation)
        Call block.Write(protein.geneId, BinaryStringFormat.ZeroTerminated)
        Call block.Write(protein.locus_id, BinaryStringFormat.ZeroTerminated)
        Call block.Write(protein.geneName, BinaryStringFormat.ZeroTerminated)
        Call block.Write(protein.description, BinaryStringFormat.ZeroTerminated)
        Call block.Write(protein.attributes.Count)

        For Each tuple In protein.attributes
            Call block.Write(tuple.Key, BinaryStringFormat.ZeroTerminated)
            Call block.Write(tuple.Value.Length)
            Call tuple.Value.DoEach(Sub(val) block.Write(val, BinaryStringFormat.ZeroTerminated))
        Next
    End Sub

    Private Sub saveCrossReference()
        For Each dbname In id_mapping
            Dim intptr As String = $"/db_xref/{dbname.Key}.json"
            Dim file As New BinaryDataWriter(stream.OpenBlock(intptr)) With {
                .ByteOrder = ByteOrder.BigEndian,
                .Encoding = Encodings.ASCII.CodePage
            }

            Call file.Write(dbname.Value.ToBEncode(Nothing).ToBencodedString)
            Call file.Flush()
            Call file.Dispose()
        Next
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call saveCrossReference()
                Call stream.Dispose()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
