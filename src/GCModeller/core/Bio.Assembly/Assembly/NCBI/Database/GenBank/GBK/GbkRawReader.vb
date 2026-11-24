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

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' Genbank 平文件的逐行原始读取器。
    ''' </summary>
    ''' <remarks>
    ''' 与<c>StreamReader</c>不同的是，这个读取器会记录每一行在文件之中的<b>字节偏移量</b>
    ''' (<see cref="LineStart"/>以及<see cref="LineEnd"/>)。这是低内存模式以及记录的随机访问
    ''' 所必须的基础能力: 低内存模式需要在后续重新定位到``ORIGIN``段的指定位置按需回读序列数据，
    ''' 而<see cref="GbkRecordIndex"/>需要记录每一条记录的起始偏移量。
    ''' 
    ''' 由于GenBank是ASCII/Latin1文本，一个字节即对应一个字符，因此字节偏移量与字符偏移量是一致的。
    ''' </remarks>
    Friend NotInheritable Class GbkRawReader : Implements IDisposable

        Private Const LF As Integer = 10
        Private Const CR As Integer = 13

        Private ReadOnly stream As IO.Stream
        Private ReadOnly ownStream As Boolean
        Private ReadOnly buffer As Byte()

        Private bufPos As Integer
        Private bufLen As Integer
        ''' <summary>
        ''' 当前缓冲区之中第一个字节在文件之中的绝对偏移量
        ''' </summary>
        Private streamOffset As Long

        Private ReadOnly sb As New Text.StringBuilder(1024)
        Private _line As String
        Private _lineNumber As Long
        Private _lineStart As Long
        Private _lineEnd As Long

        ''' <summary>
        ''' 从文件之中创建读取器
        ''' </summary>
        ''' <param name="path">目标文件的路径</param>
        ''' <param name="bufferSize">读取缓冲区的大小</param>
        Sub New(path As String, Optional bufferSize As Integer = 1024 * 1024)
            Dim size As Integer = If(bufferSize < 4096, 4096, bufferSize)

            Me.stream = New IO.FileStream(path,
                                          IO.FileMode.Open,
                                          IO.FileAccess.Read,
                                          IO.FileShare.Read,
                                          size,
                                          IO.FileOptions.SequentialScan)
            Me.ownStream = True
            Me.buffer = New Byte(size - 1) {}
        End Sub

        ''' <summary>
        ''' 从数据流之中创建读取器
        ''' </summary>
        ''' <param name="stream">目标数据流</param>
        ''' <param name="bufferSize">读取缓冲区的大小</param>
        Sub New(stream As IO.Stream, Optional bufferSize As Integer = 1024 * 1024)
            Dim size As Integer = If(bufferSize < 4096, 4096, bufferSize)

            Me.stream = stream
            ' 调用者所提供的数据流由调用者负责释放
            Me.ownStream = False
            Me.buffer = New Byte(size - 1) {}
        End Sub

        ''' <summary>
        ''' 当前行的文本(不包含行结束符)
        ''' </summary>
        Public ReadOnly Property Line As String
            Get
                Return _line
            End Get
        End Property

        ''' <summary>
        ''' 已经读取的行数(从1开始计数)
        ''' </summary>
        Public ReadOnly Property LineNumber As Long
            Get
                Return _lineNumber
            End Get
        End Property

        ''' <summary>
        ''' 当前行的第一个字节在文件之中的偏移量
        ''' </summary>
        Public ReadOnly Property LineStart As Long
            Get
                Return _lineStart
            End Get
        End Property

        ''' <summary>
        ''' 当前行的内容结束位置(不包含行结束符)之后的一个字节的偏移量
        ''' </summary>
        Public ReadOnly Property LineEnd As Long
            Get
                Return _lineEnd
            End Get
        End Property

        ''' <summary>
        ''' 当前读取器所处的绝对偏移量
        ''' </summary>
        Public ReadOnly Property Position As Long
            Get
                Return streamOffset + bufPos
            End Get
        End Property

        ''' <summary>
        ''' 读取下一行文本，没有更多的数据的时候返回<c>False</c>
        ''' </summary>
        Public Function ReadLine() As Boolean
            Call sb.Clear()

            _lineStart = Position
            _lineEnd = _lineStart

            Do
                If bufPos >= bufLen Then
                    If Not __fill() Then
                        If sb.Length = 0 Then
                            _line = ""
                            Return False
                        Else
                            Exit Do
                        End If
                    End If
                End If

                Dim b As Integer = buffer(bufPos)
                bufPos += 1

                If b = LF Then
                    Exit Do
                ElseIf b = CR Then
                    ' 忽略行结束符之中的回车
                Else
                    Call sb.Append(ChrW(b))
                    _lineEnd = Position
                End If
            Loop

            _lineNumber += 1
            _line = sb.ToString()

            Return True
        End Function

        ''' <summary>
        ''' 将读取器定位到指定的字节偏移量
        ''' </summary>
        Public Sub Seek(offset As Long)
            If Not stream.CanSeek Then
                Throw New NotSupportedException("the target stream does not support seeking!")
            End If

            Call stream.Seek(offset, IO.SeekOrigin.Begin)

            streamOffset = offset
            bufPos = 0
            bufLen = 0
            _line = Nothing
        End Sub

        Private Function __fill() As Boolean
            streamOffset += bufLen
            bufLen = stream.Read(buffer, 0, buffer.Length)
            bufPos = 0

            If bufLen <= 0 Then
                bufLen = 0
                Return False
            End If

            Return True
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            If ownStream AndAlso stream IsNot Nothing Then
                Call stream.Dispose()
            End If
        End Sub
    End Class
End Namespace
