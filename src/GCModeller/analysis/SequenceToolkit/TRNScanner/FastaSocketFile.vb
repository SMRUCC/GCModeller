Imports System.IO
Imports Darwinism.HPC.Parallel
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class FastaSocketFile : Implements IEmitStream

    Public Function BufferInMemory(obj As Object) As Boolean Implements IEmitStream.BufferInMemory
        Return True
    End Function

    Public Function WriteBuffer(obj As Object, file As Stream) As Boolean Implements IEmitStream.WriteBuffer
        Dim fa As FastaSeq = DirectCast(obj, FastaSeq)
        Dim buf As Byte() = BSONFormat.GetBuffer(JSONSerializer.CreateJSONElement(fa)).ToArray

        Call file.Write(buf, Scan0, buf.Length)
        Call file.Flush()

        Return True
    End Function

    Public Function WriteBuffer(obj As Object) As Stream Implements IEmitStream.WriteBuffer
        Dim ms As New MemoryStream
        Call WriteBuffer(obj, ms)
        Call ms.Seek(0, SeekOrigin.Begin)
        Return ms
    End Function

    Public Function ReadBuffer(file As Stream) As Object Implements IEmitStream.ReadBuffer
        Dim json As JsonObject = BSONFormat.Load(file)
        Dim fa As FastaSeq = json.CreateObject(Of FastaSeq)(decodeMetachar:=False)
        Return fa
    End Function
End Class
