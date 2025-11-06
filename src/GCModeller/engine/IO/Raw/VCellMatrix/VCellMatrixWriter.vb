Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.Raw

Public Class VCellMatrixWriter : Implements IDisposable

    ReadOnly s As StreamPack
    Private disposedValue As Boolean

    Sub New(file As Stream)
        s = New StreamPack(file, meta_size:=16 * ByteSize.MB, [readonly]:=False)
        s.Clear(16 * ByteSize.MB)
    End Sub

    Private Shared Iterator Function TrimEmpty(data As IEnumerable(Of KeyValuePair(Of String, String()))) As IEnumerable(Of KeyValuePair(Of String, String()))
        For Each item In data
            Yield New KeyValuePair(Of String, String())(item.Key, item.Value.Where(Function(id) Not id.StringEmpty(, True)).ToArray)
        Next
    End Function

    Public Sub ConvertPackData(pack As Reader)
        Dim moduleSet = pack.GetMoleculeIdList
        Dim moleculeSet = TrimEmpty(moduleSet.Where(Function(m) Not m.Key.EndsWith("-Flux"))).ToArray
        Dim fluxSet = TrimEmpty(moduleSet.Where(Function(m) m.Key.EndsWith("-Flux"))).ToArray
        Dim symbolText = pack.GetStream.ReadText("/dynamics/cellular_symbols.json")
        Dim fluxText = pack.GetStream.ReadText("/dynamics/cellular_flux.json")
        Dim instance_id As Dictionary(Of String, Dictionary(Of String, String())) = symbolText _
            .LoadJSON(Of Dictionary(Of String, Dictionary(Of String, String())))

        Call s.WriteText(symbolText, "/cellular_symbols.json")
        Call s.WriteText(fluxText, "/cellular_flux.json")
        Call s.WriteText(pack.GetStream.ReadText("/compartments.txt"), "/compartments.txt")
        Call s.WriteText(pack.GetStream.ReadText("/.etc/ticks.txt"), "/ticks.txt")
        Call s.WriteText(pack.GetStream.ReadText("/symbols.json"), "/symbols.json")
        Call s.WriteText(pack.GetStream.ReadText("/cellular_graph.jsonl"), "/cellular_graph.jsonl")

        For Each file In pack.GetStream.OpenFolder("/index/").ListFiles(recursive:=False)
            Call s.WriteText(pack.GetStream.ReadText(file), file.referencePath.ToString)
        Next

        For Each loads In MakeActivityLoadsSnapshot(pack)
            Dim path As String = $"/matrix/activityLoads/{loads.Key}.vec"

            Call s.Delete(path)

            Using buf As New BinaryDataWriter(s.OpenBlock(path), byteOrder:=ByteOrder.BigEndian)
                Dim timeline As Double() = loads.Value
                buf.Write(timeline)
                buf.Flush()
            End Using
        Next

        For Each fluxGroup As KeyValuePair(Of String, String()) In TqdmWrapper.Wrap(fluxSet, wrap_console:=App.EnableTqdm)
            Dim tmp As Double()() = SaveFlux(pack, fluxGroup.Key, fluxGroup.Value, Nothing)
            Dim forward As Double()() = SaveFlux(pack, fluxGroup.Key, fluxGroup.Value, "forward")
            Dim reverse As Double()() = SaveFlux(pack, fluxGroup.Key, fluxGroup.Value, "reverse")
            Dim offset As Integer = 0

            For Each name As String In fluxGroup.Value
                Dim path As String = $"/matrix/flux/{name}.vec"
                Dim forwardFile As String = $"/matrix/flux/forward/{name}.vec"
                Dim reverseFile As String = $"/matrix/flux/reverse/{name}.vec"

                Call s.Delete(path)
                Call s.Delete(forwardFile)
                Call s.Delete(reverseFile)

                Using buf As New BinaryDataWriter(s.OpenBlock(path), byteOrder:=ByteOrder.BigEndian)
                    Dim timeline As Double() = tmp.Select(Function(ti) ti(offset)).ToArray
                    buf.Write(timeline)
                    buf.Flush()
                End Using
                Using buf As New BinaryDataWriter(s.OpenBlock(forwardFile), byteOrder:=ByteOrder.BigEndian)
                    Dim timeline As Double() = forward.Select(Function(ti) ti(offset)).ToArray
                    buf.Write(timeline)
                    buf.Flush()
                End Using
                Using buf As New BinaryDataWriter(s.OpenBlock(reverseFile), byteOrder:=ByteOrder.BigEndian)
                    Dim timeline As Double() = reverse.Select(Function(ti) ti(offset)).ToArray
                    buf.Write(timeline)
                    buf.Flush()
                End Using

                offset += 1
            Next

            Call s.WriteText(fluxGroup.Value.JoinBy(vbCrLf), $"/matrix/flux/{fluxGroup.Key}.txt")

            Erase tmp
        Next

        For Each compart_id As String In pack.comparts
            Dim objs = instance_id(compart_id)

            For Each [module] As KeyValuePair(Of String, String()) In TqdmWrapper.Wrap(objs, wrap_console:=App.EnableTqdm)
                Dim tmp As Double()() = SaveMatrix(pack, compart_id, [module])
                Dim offset As Integer = 0

                For Each name As String In [module].Value
                    Dim path As String = $"/matrix/{compart_id}/{[module].Key}/{name}.vec"

                    Call s.Delete(path)

                    Using buf As New BinaryDataWriter(s.OpenBlock(path), byteOrder:=ByteOrder.BigEndian)
                        Dim timeline As Double() = tmp.Select(Function(ti) ti(offset)).ToArray
                        buf.Write(timeline)
                        buf.Flush()
                    End Using

                    offset += 1
                Next

                Call s.WriteText([module].Value.JoinBy(vbCrLf), $"/matrix/{compart_id}/{[module].Key}.txt")

                Erase tmp
            Next
        Next
    End Sub

    Private Shared Function MakeActivityLoadsSnapshot(pack As Reader) As Dictionary(Of String, Double())
        Dim loads = pack.ActivityLoads.ToArray
        Dim idset = loads.Select(Function(ti) ti.Keys).IteratesALL.Distinct.ToArray
        Dim matrix = idset _
            .ToDictionary(Function(id) id,
                          Function(id)
                              Return loads _
                                  .Select(Function(ti) ti(id)) _
                                  .ToArray
                          End Function)

        Return matrix
    End Function

    Private Function SaveFlux(pack As Reader, group As String, idset As String(), regulation As String) As Double()()
        Dim times As Double() = pack.AllTimePoints.ToArray
        Dim mat As Double()() = RectangularArray.Matrix(Of Double)(times.Length, idset.Length)

        If regulation Is Nothing Then
            regulation = "frames"
        End If

        For i As Integer = 0 To times.Length - 1
            Dim ti As Double = times(i)
            Dim list As New BinaryDataReader(pack.GetStream.OpenFile($"/dynamics/flux/{group}/{regulation}/{ti}.dat"), byteOrder:=ByteOrder.BigEndian)
            Dim vec As Double() = list.ReadDoubles(idset.Length)

            mat(i) = vec
        Next

        Return mat
    End Function

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
