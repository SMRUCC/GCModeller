Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Serialization

Public Class SessionFile

    ReadOnly file As String

    Sub New(filepath As String)
        file = filepath

        If Not file.FileExists Then
            Call New Byte() {}.FlushStream(file)
        End If
    End Sub

    Public Function SaveKey(key As String, data As Byte()) As Boolean
        Dim lastBlock As BufferRegion = Nothing
        Dim region As BufferRegion = SearchKey(key, lastBlock)

        If region Is Nothing Then
            ' append new region
            Using s As New BinaryDataWriter(New FileStream(file, FileMode.Append), Encoding.ASCII)
                s.Write(key, BinaryStringFormat.ZeroTerminated)
                s.Write(data.Length)
                s.Write(data)
                s.Flush()
            End Using
        ElseIf data.Length = region.size Then
            ' overrides
            Using s As New BinaryDataWriter(New FileStream(file, FileMode.Open), Encoding.ASCII)
                s.Seek(region.position, SeekOrigin.Begin)
                s.Write(data)
                s.Flush()
            End Using
        ElseIf data.Length < region.size Then
            ' update region size and then overrides data
            Using s As New BinaryDataWriter(New FileStream(file, FileMode.Open), Encoding.ASCII)
                s.Seek(region.position - RawStream.INT32, SeekOrigin.Begin)
                s.Write(data.Length)
                s.Write(data)
                s.Flush()
            End Using
        Else
            ' erase the data, and write to new location

        End If
    End Function

    Public Function SaveKey(key As String, data As String) As Boolean
        Return SaveKey(key, Encoding.UTF8.GetBytes(data))
    End Function

    Public Function OpenKeyString(key As String) As String
        Dim s As MemoryStream = OpenKey(key)

        If s Is Nothing Then
            Return Nothing
        Else
            Return Encoding.UTF8.GetString(s.ToArray)
        End If
    End Function

    Public Function OpenKey(key As String) As MemoryStream
        Dim region As BufferRegion = SearchKey(key)

        If region Is Nothing Then
            Return Nothing
        Else
            Using s As New FileStream(file, FileMode.Open)
                Dim load As Byte() = New Byte(region.size - 1) {}

                Call s.Seek(region.position, SeekOrigin.Begin)
                Call s.Read(load, Scan0, load.Length)

                Return New MemoryStream(load)
            End Using
        End If
    End Function

    ''' <summary>
    ''' [keyname => length,next]
    ''' </summary>
    ''' <param name="key"></param>
    ''' <returns></returns>
    Public Function SearchKey(key As String, Optional ByRef lastBlock As BufferRegion = Nothing) As BufferRegion
        Using s As New BinaryDataReader(New FileStream(file, FileMode.Open), Encoding.ASCII)
            For i As Integer = 0 To 100000
                Dim skey As String = s.ReadString(BinaryStringFormat.ZeroTerminated)
                Dim len As Integer = s.ReadInt32
                Dim start As Long = s.BaseStream.Position

                If skey = key Then
                    Return New BufferRegion(start, len)
                Else
                    lastBlock = New BufferRegion(start, len)
                    s.Seek(lastBlock.nextBlock, SeekOrigin.Begin)

                    If s.EndOfStream Then
                        Exit For
                    End If
                End If
            Next

            Return Nothing
        End Using
    End Function

End Class
