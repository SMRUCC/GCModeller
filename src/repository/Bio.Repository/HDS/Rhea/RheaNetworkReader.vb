Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Public Class RheaNetworkReader

    ReadOnly stream As StreamPack

    Sub New(file As Stream)
        Call Me.New(New StreamPack(file, [readonly]:=True))
    End Sub

    Sub New(stream As StreamPack)
        Me.stream = stream
    End Sub
End Class
