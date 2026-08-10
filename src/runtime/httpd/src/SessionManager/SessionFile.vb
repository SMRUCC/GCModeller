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

    Sub New(keyfile As String, datafile As String)
        Me.datafile = datafile
        Me.keyfile = keyfile

        If Not Me.keyfile.FileExists Then
            Call (New Byte() {}).FlushStream(Me.keyfile)
            Call (New Byte() {}).FlushStream(Me.datafile)
        End If
    End Sub

    Public Function SaveKey(key As String, data As Byte()) As Boolean
        Dim lastBlock As BufferRegion = Nothing
        Dim offset As Long = 0
        Dim region As BufferRegion = SearchKey(key, lastBlock, offset)

        If lastBlock Is Nothing Then
            lastBlock = New BufferRegion
        End If

        If region Is Nothing Then
            ' append new region
            Using s As New BinaryDataWriter(New FileStream(keyfile, FileMode.Append), Encoding.ASCII)
                s.Write(key, BinaryStringFormat.ZeroTerminated)
                s.Write(lastBlock.nextBlock)
                s.Write(data.Length)
                s.Flush()
            End Using
            Using s As New FileStream(datafile, FileMode.Open)
                s.Seek(lastBlock.nextBlock, SeekOrigin.Begin)
                s.Write(data, 0, data.Length)
                s.Flush()
            End Using
        ElseIf data.Length = region.size Then
            ' overrides
            Using s As New BinaryDataWriter(New FileStream(datafile, FileMode.Open), Encoding.ASCII)
                s.Seek(region.position, SeekOrigin.Begin)
                s.Write(data, 0, data.Length)
                s.Flush()
            End Using
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
        End If

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

        Using s As New BinaryDataReader(New FileStream(keyfile, FileMode.Open), Encoding.ASCII)
            Dim skey As String
            Dim start As Long
            Dim len As Integer

            For i As Integer = 0 To 100000
                If s.EndOfStream Then
                    Exit For
                Else
                    skey = s.ReadString(BinaryStringFormat.ZeroTerminated)
                    start = s.ReadInt64
                    len = s.ReadInt32
                End If

                If skey = key Then
                    keyOffset = s.Position - 8 - 4 - skey.Length - 1
                    Return New BufferRegion(start, len)
                Else
                    lastBlock = New BufferRegion(start, len)
                End If
            Next

            Return Nothing
        End Using
    End Function

End Class

