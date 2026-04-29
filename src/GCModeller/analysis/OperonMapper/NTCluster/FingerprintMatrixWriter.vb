
Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON

Public Class FingerprintMatrixWriter : Implements IDisposable

    Dim disposedValue As Boolean
    Dim s As Stream

    Sub New(s As Stream)
        Me.s = s
    End Sub

    Public Sub Add(fingerprint As NTCluster)
        Dim buffer As Byte() = BSONFormat.SafeGetBuffer(fingerprint.CreateJSONElement).ToArray
        Call s.Write(buffer, Scan0, buffer.Length)
    End Sub

    Public Shared Function BSONReader(s As Stream) As IEnumerable(Of NTCluster)
        Return BSONFormat.LoadList(s, tqdm:=False).Select(Function(json) json.CreateObject(Of NTCluster))
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call s.Flush()
                Call s.Close()
                Call s.Dispose()
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
