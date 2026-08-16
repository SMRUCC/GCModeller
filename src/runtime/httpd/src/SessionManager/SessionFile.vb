#Region "Microsoft.VisualBasic::faaea722f6dba6c153c75e55504a2895, src\SessionManager\SessionFile.vb"

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

    '   Total Lines: 184
    '    Code Lines: 129 (70.11%)
    ' Comment Lines: 20 (10.87%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 35 (19.02%)
    '     File Size: 6.06 KB


    ' Class SessionFile
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: OpenKey, OpenKeyDouble, OpenKeyInteger, OpenKeyString, (+4 Overloads) SaveKey
    '               (+2 Overloads) SearchKey
    ' 
    '     Sub: buildIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.IO

Public Class SessionFile

    Friend ReadOnly keyfile As String
    Friend ReadOnly datafile As String
    ''' <summary>
    ''' in-memory index of [key => {keyOffsetInKeyfile, dataPosition, dataSize}] built lazily to
    ''' avoid a full linear scan of the key file on every read/write.
    ''' </summary>
    Friend ReadOnly index As New Dictionary(Of String, Long())

    ''' <summary>
    ''' protects all file access; the session store may be hit concurrently by many HTTP requests.
    ''' </summary>
    ReadOnly [syncLock] As New Object
    ReadOnly writer As FileWriter

    Sub New(keyfile As String, datafile As String)
        Me.datafile = datafile
        Me.keyfile = keyfile
        Me.writer = New FileWriter With {.session = Me}

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

            writer.key = key
            writer.data = data
            writer.SaveKey(region, lastBlock, offset)
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
        SyncLock [syncLock]
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
        End SyncLock
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
                Return SearchKey(s, key, lastBlock, keyOffset)
            End Using
        End SyncLock
    End Function

    Private Function SearchKey(s As BinaryDataReader, key As String, ByRef lastBlock As BufferRegion, ByRef keyOffset As Long)
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
