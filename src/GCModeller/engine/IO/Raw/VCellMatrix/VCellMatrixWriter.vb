Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON
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
        Dim instance_id As Dictionary(Of String, Dictionary(Of String, String())) = pack _
            .GetStream _
            .ReadText("/dynamics/cellular_symbols.json") _
            .LoadJSON(Of Dictionary(Of String, Dictionary(Of String, String())))

        For Each compart_id As String In pack.comparts
            Dim objs = instance_id(compart_id)

            For Each [module] As KeyValuePair(Of String, String()) In objs
                Dim tmp As Double()() = SaveMatrix(pack, compart_id, [module])
                Dim offset As Integer = 0

                For Each name As String In [module].Value
                    Dim path As String = $"/matrix/{compart_id}/{name}.vec"

                    Call s.Delete(path)

                    Using buf As New BinaryDataWriter(s.OpenBlock(path), byteOrder:=ByteOrder.BigEndian)
                        Dim timeline As Double() = tmp.Select(Function(ti) ti(offset)).ToArray
                        buf.Write(timeline)
                        buf.Flush()
                    End Using

                    offset += 1
                Next

                Erase tmp
            Next
        Next
    End Sub

    Private Function SaveMatrix(pack As Reader, compartment_id As String, listSet As KeyValuePair(Of String, String())) As Double()()
        Dim times As Double() = pack.AllTimePoints.ToArray
        Dim resolve_name As String = listSet.Key
        Dim mat As Double()() = RectangularArray.Matrix(Of Double)(times.Length, listSet.Value.Length)

        For i As Integer = 0 To times.Length - 1
            Dim ti As Double = times(i)
            Dim list As BinaryDataReader = pack.GetFrameFile(resolve_name, compartment_id, ti)
            Dim vec As Double() = list.ReadDoubles(listSet.Value.Length)

            mat(i) = vec
        Next

        Return mat
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
