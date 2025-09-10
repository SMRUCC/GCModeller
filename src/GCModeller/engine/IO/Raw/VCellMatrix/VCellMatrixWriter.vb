Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.Raw

Public Class VCellMatrixWriter : Implements IDisposable

    ReadOnly s As StreamPack
    Private disposedValue As Boolean

    Sub New(file As Stream)
        s = New StreamPack(file, [readonly]:=False)
        s.Clear()
    End Sub

    Public Sub ConvertPackData(pack As Reader)
        Dim moduleSet = pack.GetMoleculeIdList
        Dim moleculeSet = moduleSet.Where(Function(m) Not m.Key.EndsWith("-Flux")).ToArray
        Dim fluxSet = moduleSet.Where(Function(m) m.Key.EndsWith("-Flux")).ToArray
        Dim moleculeExpr = SaveMatrix(pack, moleculeSet)
        Dim fluxExpr = SaveMatrix(pack, fluxSet)

        For Each compart_id As String In pack.comparts
            Call s.Delete($"/matrix/{compart_id}/molecule.dat")
            Call s.Delete($"/matrix/{compart_id}/flux.dat")


        Next
    End Sub

    Private Function SaveMatrix(pack As Reader, listSet As KeyValuePair(Of String, String())()) As Stream

    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
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
