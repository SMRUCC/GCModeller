Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class ScanFile : Implements IDisposable

    ReadOnly pack As StreamPack

    Private disposedValue As Boolean

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(file As Stream)
        pack = New StreamPack(file, meta_size:=1024 * 1024 * 32)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub AddSeed(name As String, seed As HSP)
        Dim path = $"/seeds/{Mid(name, 1, 2)}/{name}.json"

        Call pack.Delete(path)
        Call pack.WriteText(seed.GetJson, fileName:=path)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call pack.Dispose()
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
