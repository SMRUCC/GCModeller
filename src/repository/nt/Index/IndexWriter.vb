Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump

Public Class IndexWriter : Inherits IndexAbstract

    Dim seqDB As BinaryDataWriter
    Dim index As BinaryDataWriter

    Sub New(EXPORT$, db$, index$)
        MyBase.New(index)

        Call $"{EXPORT}/index/{db}".MkDIR
        Call $"{EXPORT}/{db}".MkDIR

        Me.seqDB = New BinaryDataWriter(File.OpenWrite($"{EXPORT}/{db}/{gi}.nt"))
        Me.index = New BinaryDataWriter(File.OpenWrite($"{EXPORT}/index/{db}/{gi}.index"))
    End Sub

    Dim pointer&

    Public Sub Write(nt$, header As NTheader)
        Dim nt_bufs As Byte() = Encoding.ASCII.GetBytes(nt)
        Dim gi As Byte() = Encoding.ASCII.GetBytes(header.gi)
        Dim start& = pointer + gi.Length + tab.Length

        Call index.Write(start)
        Call index.Write(nt_bufs.Length)

        Call seqDB.Write(gi)
        Call seqDB.Write(tab)
        Call seqDB.Write(nt_bufs)
        Call seqDB.Write(lf)

        pointer = start + nt_bufs.Length + lf.Length
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        Call seqDB.Flush()
        Call seqDB.Close()
        Call seqDB.Dispose()
        Call index.Flush()
        Call index.Close()
        Call index.Dispose()

        MyBase.Dispose(disposing)
    End Sub
End Class
