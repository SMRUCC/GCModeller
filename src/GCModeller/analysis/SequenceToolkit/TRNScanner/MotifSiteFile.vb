Imports System.IO
Imports Darwinism.HPC.Parallel
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns

Public Class MotifSiteFile : Implements IEmitStream

    Public Function BufferInMemory(obj As Object) As Boolean Implements IEmitStream.BufferInMemory
        Return True
    End Function

    Const NIL As Byte = 0

    Public Function WriteBuffer(obj As Object, file As Stream) As Boolean Implements IEmitStream.WriteBuffer
        Dim sites As MotifMatch() = DirectCast(obj, MotifMatch())
        Dim buf As Byte()

        For Each site As MotifMatch In sites
            buf = BSONFormat.GetBuffer(JSONSerializer.CreateJSONElement(site)).ToArray
            file.Write(buf, Scan0, buf.Length)
            file.WriteByte(NIL)
        Next

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
        Return (From json As JsonObject
                In BSONFormat.LoadList(file, tqdm:=False)
                Select json.CreateObject(Of MotifMatch)(decodeMetachar:=False)).ToArray
    End Function
End Class
