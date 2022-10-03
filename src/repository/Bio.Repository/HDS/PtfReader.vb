Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.Bencoding

Public Class PtfReader : Implements IDisposable

    ReadOnly stream As StreamPack

    Private disposedValue As Boolean

    Sub New(file As Stream)
        Call Me.New(New StreamPack(file, [readonly]:=True))
    End Sub

    Sub New(stream As StreamPack)
        Me.stream = stream
    End Sub

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
                Dim geneSymbols As String()

                xrefs.Add(DirectCast(id, BString).Value, geneSymbols)
            Next
        End If

        Return Nothing
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
