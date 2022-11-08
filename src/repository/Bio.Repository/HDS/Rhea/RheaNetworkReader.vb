Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Class RheaNetworkReader

    ReadOnly stream As StreamPack
    ReadOnly enzyme_numbers As Dictionary(Of String, String())

    Sub New(file As Stream)
        Call Me.New(New StreamPack(file, [readonly]:=True))
    End Sub

    Sub New(stream As StreamPack)
        Me.stream = stream
        Me.enzyme_numbers = stream.ReadText("/rhea_index.json").LoadJSON(Of Dictionary(Of String, String()))
    End Sub

    Public Iterator Function GetByEnzymeNumber(ec_number As String) As IEnumerable(Of Rhea.Reaction)
        If Not enzyme_numbers.ContainsKey(ec_number) Then
            Return
        End If

        Dim idlist As String() = enzyme_numbers(ec_number)

        For Each id As String In idlist
            Yield GetByEntryId(id)
        Next
    End Function

    Public Function GetByEntryId(id As String) As Rhea.Reaction
        Dim path As String = $"/rhea/{id}.dat"
        Dim pack = stream.OpenBlock(path)
        Dim file As New BinaryReader(pack)
        Dim entry As String = file.ReadString
        Dim def As String = file.ReadString
        Dim enzyme As String = file.ReadString
        Dim size As Integer = file.ReadInt32
        Dim buffer As Byte() = New Byte(size - 1) {}

        Call file.Read(buffer, Scan0, size)

        Return New Rhea.Reaction With {
            .entry = entry,
            .definition = def,
            .equation = Equation.ParseBuffer(New MemoryStream(buffer)),
            .enzyme = enzyme _
                .BDecode _
                .Select(Function(b) DirectCast(b, BString).Value) _
                .ToArray
        }
    End Function
End Class
