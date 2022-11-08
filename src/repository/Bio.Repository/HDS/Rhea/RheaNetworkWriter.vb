Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.Rhea

Public Class RheaNetworkWriter

    ReadOnly stream As StreamPack

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
    End Sub

End Class
