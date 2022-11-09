Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.Rhea

Public Class RheaNetworkWriter : Implements IDisposable

    ReadOnly stream As StreamPack
    ReadOnly enzymatics As New Dictionary(Of String, List(Of String))

    Private disposedValue As Boolean

    Sub New(stream As StreamPack)
        Me.stream = stream
    End Sub

    Public Sub AddReaction(reaction As Reaction)
        Dim eq As Byte() = Equation.GetBuffer(reaction.equation)
        Dim path As String = $"/rhea/{reaction.entry}.dat"
        Dim pack = stream.OpenBlock(path)
        Dim file As New BinaryWriter(pack)

        Call file.Write(reaction.entry)
        Call file.Write(reaction.definition)
        Call file.Write(If(reaction.enzyme, {}).ToBEncode.ToBencodedString)
        Call file.Write(eq.Length)
        Call file.Write(eq)
        Call file.Flush()
        ' 20221108 dispose the file writer will also dispose the stream pack object
        ' so we removes this code, just needs to flush data
        ' Call file.Dispose()
        Call pack.Dispose()

        If Not reaction.enzyme.IsNullOrEmpty Then
            For Each number As String In reaction.enzyme
                If Not enzymatics.ContainsKey(number) Then
                    enzymatics.Add(number, New List(Of String))
                End If

                Call enzymatics(number).Add(reaction.entry)
            Next
        End If
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Dim path As String = "/rhea_index.json"
                Dim json As String = enzymatics.ToDictionary(Function(a) a.Key, Function(a) a.Value.ToArray).GetJson

                Call stream.WriteText(json, path)
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
