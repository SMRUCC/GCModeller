Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports SMRUCC.genomics.Data

Public Class ProjectReader

    Dim buffer As StreamPack
    Dim proteins As PtfReader

    Sub New(stream As Stream)
        buffer = New StreamPack(stream)
        proteins = New PtfReader(buffer)
    End Sub
End Class
