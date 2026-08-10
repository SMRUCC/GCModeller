#Region "Microsoft.VisualBasic::36cd110a84f67285066bb6c64d9a8943, src\SessionManager\SessionFile.vb"

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

'   Total Lines: 176
'    Code Lines: 139 (78.98%)
' Comment Lines: 9 (5.11%)
'    - Xml Docs: 55.56%
' 
'   Blank Lines: 28 (15.91%)
'     File Size: 6.11 KB


' Class SessionFile
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: OpenKey, OpenKeyDouble, OpenKeyInteger, OpenKeyString, (+4 Overloads) SaveKey
'               SearchKey
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.IO

Public Class SessionFile

    ReadOnly keyfile As String
    ReadOnly datafile As String

    ''' <summary>
    ''' in-memory index of [key => {keyOffsetInKeyfile, dataPosition, dataSize}] built lazily to
    ''' avoid a full linear scan of the key file on every read/write.
    ''' </summary>
    ReadOnly index As New Dictionary(Of String, Long())
    ''' <summary>
    ''' protects all file access; the session store may be hit concurrently by many HTTP requests.
    ''' </summary>
    ReadOnly [syncLock] As New Object

    Sub New(keyfile As String, datafile As String)
        Me.datafile = datafile
        Me.keyfile = keyfile

        If Not Me.keyfile.FileExists Then
            Call (New Byte() {}).FlushStream(Me.keyfile)
            Call (New Byte() {}).FlushStream(Me.datafile)
        End If
    End Sub

    Public Function SaveKey(key As String, data As Byte()) As Boolean
        SyncLock [syncLock]
            Dim lastBlock As BufferRegion = Nothing
            Dim offset As Long = 0
            Dim region As BufferRegion = SearchKey(key, lastBlock, offset)

            If lastBlock Is Nothing Then
                lastBlock = New BufferRegion
            End If

            If region Is Nothing Then
                ' append new region. always write data at the current end of the
                ' data file (not lastBlock.nextBlock) to avoid overwriting an
                ' earlier key's data when keys are not strictly offset-ordered.
                Dim dataOffset As Long
                Using s As New FileStream(datafile, FileMode.Open)
                    s.Seek(0, SeekOrigin.End)
                    dataOffset = s.Position
                    s.Write(data, 0, data.Length)
                    s.Flush()
                End Using
                Using s As New BinaryDataWriter(New FileStream(keyfile, FileMode.Append), Encoding.ASCII)
                    s.Write(key, BinaryStringFormat.ZeroTerminated)
                    s.Write(dataOffset)
                    s.Write(data.Length)
                    s.Flush()
                End Using

                ' update index with the new key location
                index(key) = {offset, dataOffset, data.Length}
            ElseIf data.Length = region.size Then
                ' overrides
                Using s As New BinaryDataWriter(New FileStream(datafile, FileMode.Open), Encoding.ASCII)
                    s.Seek(region.position, SeekOrigin.Begin)
                    s.Write(data, 0, data.Length)
                    s.Flush()
                End Using

                If index.ContainsKey(key) Then
                    index(key)(2) = data.Length
                End If
            ElseIf data.Length < region.size Then
                ' update region size and then overrides data
                Using s As New BinaryDataWriter(New FileStream(keyfile, FileMode.Open), Encoding.ASCII)
                    s.Seek(offset + key.Length + 1 + 8, SeekOrigin.Begin)
                    s.Write(data.Length)
                    s.Flush()
                End Using
                Using s As New BinaryDataWriter(New FileStream(datafile, FileMode.Open), Encoding.ASCII)
                    s.Seek(region.position, SeekOrigin.Begin)
                    s.Write(data, 0, data.Length)
                    s.Flush()
                End Using

                If index.ContainsKey(key) Then
                    index(key)(2) = data.Length
                End If
            Else
                ' erase the data, and write to new location
                Dim dataOffset As Long

                Using s As New BinaryDataWriter(New FileStream(datafile, FileMode.Open), Encoding.ASCII)
                    s.Seek(s.BaseStream.Length, SeekOrigin.Begin)
                    dataOffset = s.Position
                    s.Write(data, 0, data.Length)
                    s.Flush()
                End Using
                Using s As New BinaryDataWriter(New FileStream(keyfile, FileMode.Open), Encoding.ASCII)
                    s.Seek(offset + key.Length + 1, SeekOrigin.Begin)
                    s.Write(dataOffset)
                    s.Write(data.Length)
                    s.Flush()
                End Using

                If index.ContainsKey(key) Then
                    index(key) = {offset, dataOffset, data.Length}
                End If
            End If
        End SyncLock

        Return True
    End Function

    Public Function SaveKey(key As String, data As Integer) As Boolean
        Return SaveKey(key, BitConverter.GetBytes(data))
    End Function

    Public Function SaveKey(key As String, data As Double) As Boolean
        Return SaveKey(key, BitConverter.GetBytes(data))
    End Function

    Public Function SaveKey(key As String, data As String) As Boolean
        Return SaveKey(key, Encoding.UTF8.GetBytes(data))
    End Function

    Public Function OpenKeyString(key As String) As String
        Dim s As Byte() = OpenKey(key)

        If s Is Nothing Then
            Return Nothing
        Else
            Return Encoding.UTF8.GetString(s)
        End If
    End Function

    Public Function OpenKeyInteger(key As String) As Integer
        Dim s As Byte() = OpenKey(key)

        If s Is Nothing Then
            Return Nothing
        Else
            Return BitConverter.ToInt32(s, Scan0)
        End If
    End Function

    Public Function OpenKeyDouble(key As String) As Double
        Dim s As Byte() = OpenKey(key)

        If s Is Nothing Then
            Return Nothing
        Else
            Return BitConverter.ToDouble(s, Scan0)
        End If
    End Function

    Public Function OpenKey(key As String) As Byte()
        Dim region As BufferRegion = SearchKey(key)

        If region Is Nothing Then
            Return Nothing
        Else
            Using s As New FileStream(datafile, FileMode.Open)
                Dim load As Byte() = New Byte(region.size - 1) {}

                Call s.Seek(region.position, SeekOrigin.Begin)
                Call s.Read(load, Scan0, load.Length)

                Return load
            End Using
        End If
    End Function

    ''' <summary>
    ''' [keyname => offset,length]
    ''' </summary>
    ''' <param name="key"></param>
    ''' <returns></returns>
    Public Function SearchKey(key As String,
                              Optional ByRef lastBlock As BufferRegion = Nothing,
                              Optional ByRef keyOffset As Long = 0) As BufferRegion

        SyncLock [syncLock]
            Using s As New BinaryDataReader(New FileStream(keyfile, FileMode.Open), Encoding.ASCII)
                Dim skey As String
                Dim start As Long
                Dim len As Integer

                ' rebuild the in-memory index while scanning, so subsequent
                ' lookups for any key can skip the linear scan entirely.
                If index.Count = 0 Then
                    Call buildIndex(s)
                End If

                If index.ContainsKey(key) Then
                    Dim hit As Long() = index(key)
                    keyOffset = hit(0)
                    Return New BufferRegion(hit(1), CInt(hit(2)))
                End If

                ' fall back to a linear scan only when the index is not yet complete
                ' (e.g. the file grew after the index was built)
                s.Seek(Scan0, SeekOrigin.Begin)

                While Not s.EndOfStream
                    Dim entryOffset As Long = s.Position
                    skey = s.ReadString(BinaryStringFormat.ZeroTerminated)
                    start = s.ReadInt64
                    len = s.ReadInt32

                    If skey = key Then
                        keyOffset = entryOffset
                        Return New BufferRegion(start, len)
                    Else
                        lastBlock = New BufferRegion(start, len)
                    End If
                End While
            End Using
        End SyncLock

        Return Nothing
    End Function

    ''' <summary>
    ''' build the in-memory key index from the current key file content.
    ''' caller must hold <see cref="[syncLock]"/>.
    ''' </summary>
    Private Sub buildIndex(s As BinaryDataReader)
        s.Seek(Scan0, SeekOrigin.Begin)

        While Not s.EndOfStream
            Dim entryOffset As Long = s.Position
            Dim skey As String = s.ReadString(BinaryStringFormat.ZeroTerminated)
            Dim start As Long = s.ReadInt64
            Dim len As Integer = s.ReadInt32

            If Not index.ContainsKey(skey) Then
                index(skey) = {entryOffset, start, len}
            End If
        End While
    End Sub

End Class

