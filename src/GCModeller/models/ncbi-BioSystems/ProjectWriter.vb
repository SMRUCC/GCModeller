Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Public Class ProjectWriter

    ReadOnly stream As StreamPack

    Sub New(file As Stream)
        stream = New StreamPack(file)
    End Sub


End Class
