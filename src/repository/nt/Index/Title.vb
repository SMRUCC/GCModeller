Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Language

Public Class TitleIndex
    Inherits IndexAbstract



    Public Sub New(DATA$, db$, uid$)
        MyBase.New(uid)
    End Sub


End Class

Public Class TitleWriter
    Inherits IndexAbstract

    ReadOnly __titles As BinaryDataWriter
    ReadOnly __index As BinaryDataWriter

    Public Sub New(DATA$, db$, uid$)
        MyBase.New(uid)

        Dim path As New Value(Of String)
        Dim file As FileStream

        Call (path = $"{DATA}/headers/{db}/{uid}.dat").ParentPath.MkDIR

        file = IO.File.OpenWrite(path)
        __titles = New BinaryDataWriter(file, Encodings.ASCII)

        Call (path = $"{DATA}/headers/index/{db}/{uid}.dat").ParentPath.MkDIR

        file = IO.File.OpenWrite(path)
        __index = New BinaryDataWriter(file, Encodings.ASCII)
    End Sub

    ReadOnly __pointer&

    ''' <summary>
    ''' ``{gi,len,title}``
    ''' </summary>
    ''' <param name="header"></param>
    ''' <returns></returns>
    Public Function Write(header As NTheader) As Long
        Dim start& = __pointer
        Dim gi As Byte() = BitConverter.GetBytes(CLng(header.gi))
        Dim title As Byte() = Encoding.ASCII.GetBytes(header.description)

    End Function
End Class