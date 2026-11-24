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
    ''' Genbank(GBFF) 文件格式错误。
    ''' </summary>
    ''' <remarks>
    ''' 旧版本的解析器在遇到长度不足的行的时候会直接抛出裸的<c>InvalidProgramException</c>，
    ''' 这种异常既没有记载名也没有行号，对于数Gb大小的基因组数据库文件而言是完全无法定位问题的。
    ''' 这里改为携带记载名、行号以及该行的原文的格式异常对象。
    ''' </remarks>
    Public Class GbkFormatException : Inherits Exception

        ''' <summary>
        ''' 出现错误的记录的编号(一般是``LOCUS``行的第一个字段)
        ''' </summary>
        Public ReadOnly Property RecordName As String
        ''' <summary>
        ''' 出现错误的行号(从1开始计数)
        ''' </summary>
        Public ReadOnly Property LineNumber As Long
        ''' <summary>
        ''' 出现错误的行文本
        ''' </summary>
        Public ReadOnly Property LineText As String

        Sub New(message As String,
                Optional record As String = Nothing,
                Optional lineNumber As Long = -1,
                Optional lineText As String = Nothing,
                Optional innerException As Exception = Nothing)

            MyBase.New(__buildMessage(message, record, lineNumber, lineText), innerException)

            Me.RecordName = record
            Me.LineNumber = lineNumber
            Me.LineText = lineText
        End Sub

        Private Shared Function __buildMessage(message As String, record As String, lineNumber As Long, lineText As String) As String
            Dim sb As New Text.StringBuilder()

            Call sb.Append(If(String.IsNullOrEmpty(message), "invalid genbank file format!", message))

            If Not String.IsNullOrEmpty(record) Then
                Call sb.Append($" (record: {record})")
            End If
            If lineNumber > 0 Then
                Call sb.Append($" (line: {lineNumber})")
            End If
            If Not String.IsNullOrEmpty(lineText) Then
                Dim text As String = lineText

                If text.Length > 120 Then
                    text = text.Substring(0, 120) & "..."
                End If

                Call sb.Append($"{vbCrLf}> {text}")
            End If

            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Dim text As String = MyBase.ToString()

            If Not String.IsNullOrEmpty(RecordName) Then
                text = $"[{RecordName}] {text}"
            End If

            Return text
        End Function
    End Class
End Namespace
