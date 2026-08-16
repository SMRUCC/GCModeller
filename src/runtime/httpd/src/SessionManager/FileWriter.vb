#Region "Microsoft.VisualBasic::03e858d23b47c066e2f3c7ca9802ebfa, src\SessionManager\FileWriter.vb"

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

    '   Total Lines: 105
    '    Code Lines: 78 (74.29%)
    ' Comment Lines: 14 (13.33%)
    '    - Xml Docs: 64.29%
    ' 
    '   Blank Lines: 13 (12.38%)
    '     File Size: 3.75 KB


    ' Class FileWriter
    ' 
    '     Sub: [Overrides], Append, ReAllocate, ResizeBuffer, SaveKey
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.IO

Friend Class FileWriter

    Friend session As SessionFile

    ''' <summary>
    ''' must be the ascii char string
    ''' </summary>
    Friend key As String
    Friend data As Byte()

    Public Sub SaveKey(region As BufferRegion, lastBlock As BufferRegion, offset As Long)
        If region Is Nothing Then
            Call Append(offset)
        ElseIf data.Length = region.size Then
            Call [Overrides](region)
        ElseIf data.Length < region.size Then
            Call ResizeBuffer(region, offset)
        Else
            Call ReAllocate(offset)
        End If
    End Sub

    ''' <summary>
    ''' update region size and then overrides data
    ''' </summary>
    Private Sub ResizeBuffer(region As BufferRegion, offset As Long)
        Using s As New BinaryDataWriter(New FileStream(session.keyfile, FileMode.Open), Encoding.ASCII)
            s.Seek(offset + key.Length + 1 + 8, SeekOrigin.Begin)
            s.Write(data.Length)
            s.Flush()
        End Using
        Using s As New BinaryDataWriter(New FileStream(session.datafile, FileMode.Open), Encoding.ASCII)
            s.Seek(region.position, SeekOrigin.Begin)
            s.Write(data, 0, data.Length)
            s.Flush()
        End Using

        If session.index.ContainsKey(key) Then
            session.index(key)(2) = data.Length
        End If
    End Sub

    Private Sub Append(offset As Long)
        ' append new region. always write data at the current end of the
        ' data file (not lastBlock.nextBlock) to avoid overwriting an
        ' earlier key's data when keys are not strictly offset-ordered.
        Dim dataOffset As Long
        Using s As New FileStream(session.datafile, FileMode.Open)
            s.Seek(0, SeekOrigin.End)
            dataOffset = s.Position
            s.Write(data, 0, data.Length)
            s.Flush()
        End Using
        Using s As New BinaryDataWriter(New FileStream(session.keyfile, FileMode.Append), Encoding.ASCII)
            s.Write(key, BinaryStringFormat.ZeroTerminated)
            s.Write(dataOffset)
            s.Write(data.Length)
            s.Flush()
        End Using

        ' update index with the new key location
        session.index(key) = {offset, dataOffset, data.Length}
    End Sub

    Private Sub [Overrides](region As BufferRegion)
        ' overrides
        Using s As New BinaryDataWriter(New FileStream(session.datafile, FileMode.Open), Encoding.ASCII)
            s.Seek(region.position, SeekOrigin.Begin)
            s.Write(data, 0, data.Length)
            s.Flush()
        End Using

        If session.index.ContainsKey(key) Then
            session.index(key)(2) = data.Length
        End If
    End Sub

    ''' <summary>
    ''' erase the data, and write to new location
    ''' </summary>
    Private Sub ReAllocate(offset As Long)
        Dim dataOffset As Long

        Using s As New BinaryDataWriter(New FileStream(session.datafile, FileMode.Open), Encoding.ASCII)
            s.Seek(s.BaseStream.Length, SeekOrigin.Begin)
            dataOffset = s.Position
            s.Write(data, 0, data.Length)
            s.Flush()
        End Using
        Using s As New BinaryDataWriter(New FileStream(session.keyfile, FileMode.Open), Encoding.ASCII)
            s.Seek(offset + key.Length + 1, SeekOrigin.Begin)
            s.Write(dataOffset)
            s.Write(data.Length)
            s.Flush()
        End Using

        If session.index.ContainsKey(key) Then
            session.index(key) = {offset, dataOffset, data.Length}
        End If
    End Sub
End Class

