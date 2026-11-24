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

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    ''' <summary>
    ''' ``PRIMARY``段: WGS主记录(例如``NZ_XXXX000000``)之中的组成部分列表。
    ''' </summary>
    ''' <remarks>
    ''' 形如:
    ''' 
    ''' ```
    ''' PRIMARY     TSA: RefSeq assembly WGS master record
    '''             AEXXXX01.1: 1-1000
    '''             AEXXXX02.1: 1-2000
    ''' ```
    ''' </remarks>
    <XmlType("gbff-primary")>
    Public Class PRIMARY : Inherits KeyWord

        ''' <summary>
        ''' 段的描述信息(第一行之中字段名之后的内容)
        ''' </summary>
        <XmlAttribute> Public Property Description As String
        ''' <summary>
        ''' 引用列表
        ''' </summary>
        <XmlArrayItem("reference")>
        Public Property References As String()
        ''' <summary>
        ''' 原始的文本行(用于写出的时候保证与原始的文本一致)
        ''' </summary>
        <XmlIgnore>
        Public Property Lines As String()

        ''' <summary>
        ''' 解析``PRIMARY``段
        ''' </summary>
        ''' <param name="lines">``PRIMARY``段的全部文本行(第一行为字段名所在的行)</param>
        Public Shared Function Parser(lines As String()) As PRIMARY
            If lines.IsNullOrEmpty Then
                Return New PRIMARY With {
                    .Description = "",
                    .References = New String() {},
                    .Lines = New String() {}
                }
            End If

            Dim description As String = ""
            Dim first As String = lines(0)

            If Not String.IsNullOrEmpty(first) AndAlso first.Length > GbkFormat.HEADER_INDENT Then
                description = first.Substring(GbkFormat.HEADER_INDENT).Trim()
            End If

            Dim refs As New List(Of String)

            For i As Integer = 1 To lines.Length - 1
                Dim s As String = If(lines(i), "").Trim()

                If s.Length > 0 Then
                    Call refs.Add(s)
                End If
            Next

            Return New PRIMARY With {
                .Description = description,
                .References = refs.ToArray,
                .Lines = lines
            }
        End Function

        ''' <summary>
        ''' 生成``PRIMARY``段的文本表示
        ''' </summary>
        Public Overrides Function ToString() As String
            If Not Lines.IsNullOrEmpty Then
                Return String.Join(vbCrLf, Lines)
            End If

            Dim sb As New StringBuilder("PRIMARY     " & If(Description, ""))

            For Each s As String In References.SafeQuery
                Call sb.AppendLine()
                Call sb.Append($"{New String(" "c, GbkFormat.HEADER_INDENT)}{s}")
            Next

            Return sb.ToString()
        End Function
    End Class
End Namespace
