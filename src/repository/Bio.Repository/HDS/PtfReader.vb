Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Annotation.Ptf

Public Class PtfReader : Implements IDisposable

    ReadOnly stream As StreamPack

    Private disposedValue As Boolean

    Sub New(file As Stream)
        Call Me.New(New StreamPack(file, [readonly]:=True))
    End Sub

    Sub New(stream As StreamPack)
        Me.stream = stream
    End Sub

    Public Function getExternalReferenceList() As String()
        Return DirectCast(stream.GetObject("/db_xref/"), StreamGroup).files _
            .Select(Function(a) a.fileName.BaseName) _
            .ToArray
    End Function

    Public Function GetAnnotation(id As String) As ProteinAnnotation
        Dim path As String = $"/annotation/{id}.ptf"
        Dim file As StreamObject = stream.GetObject(path)

        If file Is Nothing Then
            Return Nothing
        Else
            Return ReadBytes(New BinaryDataReader(stream.OpenBlock(file)) With {
                .ByteOrder = ByteOrder.BigEndian,
                .Encoding = Encodings.ASCII.CodePage
            })
        End If
    End Function

    Public Shared Function ReadBytes(data As BinaryDataReader) As ProteinAnnotation
        Dim geneId As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim locus_id As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim geneName As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim desc As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim seq As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim nattrs As Integer = data.ReadInt32
        Dim attrs As New Dictionary(Of String, String())

        For i As Integer = 0 To nattrs - 1
            Dim key As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
            Dim size As Integer = data.ReadInt32
            Dim vals As String() = New String(size - 1) {}

            For j As Integer = 0 To size - 1
                vals(j) = data.ReadString(BinaryStringFormat.ZeroTerminated)
            Next

            Call attrs.Add(key, vals)
        Next

        Return New ProteinAnnotation With {
            .attributes = attrs,
            .description = desc,
            .geneId = geneId,
            .geneName = geneName,
            .locus_id = locus_id,
            .sequence = seq
        }
    End Function

    Public Function LoadCrossReference(key As String) As Dictionary(Of String, String())
        Dim intptr As String = $"/db_xref/{key}.txt"

        If Not stream.FileExists(intptr) Then
            Return New Dictionary(Of String, String())
        End If

        Dim rawtext As String = stream.ReadText(filename:=intptr)
        Dim data = BencodeDecoder.Decode(rawtext)

        If data.IsNullOrEmpty Then
            Return New Dictionary(Of String, String())
        Else
            Dim xrefs As New Dictionary(Of String, String())
            Dim list = DirectCast(data(Scan0), BDictionary)

            For Each id As BElement In list.Keys
                Dim cluster = list(DirectCast(id, BString).Value)
                Dim geneSymbols As String() = DirectCast(cluster, BList) _
                    .Select(Function(b) DirectCast(b, BString).Value) _
                    .Distinct _
                    .ToArray

                xrefs.Add(DirectCast(id, BString).Value, geneSymbols)
            Next

            Return xrefs
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call stream.Dispose()
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
