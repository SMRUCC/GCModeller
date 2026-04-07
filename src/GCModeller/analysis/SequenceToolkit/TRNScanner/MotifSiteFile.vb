#Region "Microsoft.VisualBasic::6530b83cf80ddc793c69f716422973b7, analysis\SequenceToolkit\TRNScanner\MotifSiteFile.vb"

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

    '   Total Lines: 43
    '    Code Lines: 34 (79.07%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (20.93%)
    '     File Size: 1.58 KB


    ' Class MotifSiteFile
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

