#Region "Microsoft.VisualBasic::d5dc33be7b24113f9b7e872415e6ceda, analysis\SequenceToolkit\TRNScanner\FastaSocketFile.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 36
    '    Code Lines: 29 (80.56%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (19.44%)
    '     File Size: 1.38 KB


    ' Class FastaSocketFile
    ' 
    '     Function: BufferInMemory, ReadBuffer, (+2 Overloads) WriteBuffer
    ' 
    ' /********************************************************************************/

#End Region

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

