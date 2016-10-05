Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump

Public Class TitleWriter : Inherits IndexAbstract
    Implements IDisposable

    ReadOnly __titles As BinaryDataWriter
    ReadOnly __index As BinaryDataWriter

    Public Sub New(DATA$, db$, uid$)
        MyBase.New(uid)

        Dim path As New Value(Of String)
        Dim file As FileStream

        Call (path = $"{DATA}/headers/{db}/{uid}.txt").ParentPath.MkDIR

        file = IO.File.OpenWrite(path)
        __titles = New BinaryDataWriter(file, Encodings.ASCII)

        Call (path = $"{DATA}/headers/index/{db}/{uid}.index").ParentPath.MkDIR

        file = IO.File.OpenWrite(path)
        __index = New BinaryDataWriter(file, Encodings.ASCII)
    End Sub

    Dim __pointer&

    ''' <summary>
    ''' ``{gi,len,title}``
    ''' </summary>
    ''' <param name="header"></param>
    ''' <returns></returns>
    Public Function Write(header As NTheader) As Long
        Dim title As Byte() = Encoding.ASCII.GetBytes(header.description)
        Dim gi As Byte() = BitConverter.GetBytes(CLng(header.gi))
        Dim id As Byte() = Encoding.ASCII.GetBytes(header.uid)
        Dim start& = __pointer + id.Length + tab.Length

        Call __index.Write(gi)
        Call __index.Write(start)
        Call __index.Write(title.Length)

        Call __titles.Write(id)
        Call __titles.Write(tab)
        Call __titles.Write(title)
        Call __titles.Write(lf)

        __pointer = start + title.Length + lf.Length

        Return title.Length
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        Call __index.Flush()
        Call __index.Close()
        Call __index.Dispose()
        Call __titles.Flush()
        Call __titles.Close()
        Call __titles.Dispose()

        MyBase.Dispose(disposing)
    End Sub
End Class