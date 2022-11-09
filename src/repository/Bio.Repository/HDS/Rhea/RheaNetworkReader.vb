Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Class RheaNetworkReader

    ReadOnly stream As StreamPack
    ReadOnly enzyme_numbers As Dictionary(Of String, String())
    ReadOnly class_list As Dictionary(Of String, String())

    Sub New(file As Stream)
        Call Me.New(New StreamPack(file, [readonly]:=True))
    End Sub

    Sub New(stream As StreamPack)
        Me.stream = stream
        Me.enzyme_numbers = stream.ReadText("/rhea_index.json").LoadJSON(Of Dictionary(Of String, String()))
        Me.class_list = enzyme_numbers.Keys _
            .GroupBy(Function(ec) $"ec_{ec.Split("."c).First}") _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)
    End Sub

    Private Function queryRange(ec_number As String) As String()
        Dim className As String = $"ec_{ec_number.Split("."c).First}"
        Dim category As String() = class_list(className)

        ec_number = ec_number.Trim("-"c)


    End Function

    Public Iterator Function GetByEnzymeNumber(ec_number As String) As IEnumerable(Of Rhea.Reaction)
        Dim idlist As String()

        If Not enzyme_numbers.ContainsKey(ec_number) Then
            idlist = queryRange(ec_number)
        Else
            idlist = enzyme_numbers(ec_number)
        End If

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
                .Select(Function(b) DirectCast(b, BList).ToArray) _
                .IteratesALL _
                .Select(Function(str) DirectCast(str, BString).Value) _
                .ToArray
        }
    End Function
End Class
