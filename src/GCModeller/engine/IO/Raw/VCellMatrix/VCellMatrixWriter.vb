Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Public Class VCellMatrixWriter

    ReadOnly s As StreamPack

    Sub New(file As Stream)
        s = New StreamPack(file, [readonly]:=False)
        s.Clear()
    End Sub

End Class
