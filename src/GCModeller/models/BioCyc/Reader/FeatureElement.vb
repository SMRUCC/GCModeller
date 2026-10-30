#Region "Microsoft.VisualBasic::5f02a181928904318a2fcf91ccce7d95, models\BioCyc\Reader\FeatureElement.vb"

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

    '   Total Lines: 123
    '    Code Lines: 99 (80.49%)
    ' Comment Lines: 1 (0.81%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 23 (18.70%)
    '     File Size: 4.10 KB


    ' Class FeatureElement
    ' 
    '     Properties: attributes, uniqueId
    ' 
    '     Function: ParseBuffer, removeBreakLines, ToString
    ' 
    ' Class ValueString
    ' 
    '     Properties: attributes, value
    ' 
    '     Function: getAttributes, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions

Public Class FeatureElement : Implements IReadOnlyId

    Public ReadOnly Property uniqueId As String Implements IReadOnlyId.Identity
        Get
            Return attributes("UNIQUE-ID").First.value
        End Get
    End Property

    Public Property attributes As Dictionary(Of String, ValueString())

    Default Public ReadOnly Property getValue(key As String) As ValueString()
        Get
            ' mute:=True —— BioCyc 的 .dat 是稀疏的属性-值文件：某个条目缺少某个槽位
            ' 是完全正常的，反射绑定会逐槽位查询。若不静默，DEBUG 下会为每个缺失槽位
            ' 打印一条日志，既淹没控制台也严重拖慢整库解析。
            Return attributes.TryGetValue(key, mute:=True)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return uniqueId
    End Function

    ''' <summary>
    ''' 处理 BioCyc 的对齐文本的续行规则，以及注释行的剔除：
    ''' 
    ''' 1. 以单个 ``/`` 开头的行是上一行属性值的续行；
    ''' 2. 以 ``//`` 开头的行表示该行以一个字面量 ``/`` 开始(转义规则)；
    ''' 3. 以 ``#`` 开头的行是注释行，可以出现在文件的任意位置。
    ''' </summary>
    ''' <param name="buffer"></param>
    ''' <returns></returns>
    Private Shared Function removeBreakLines(buffer As String()) As IEnumerable(Of String)
        Dim newBuf As New List(Of String)

        For i As Integer = 0 To buffer.Length - 1
            Dim line As String = buffer(i)

            If line Is Nothing Then
                Continue For
            End If

            line = line.TrimEnd()

            If line.Length = 0 Then
                Continue For
            End If

            If line.StartsWith("#") Then
                ' 注释行可以出现在文件的任意位置，直接跳过
                Continue For
            End If

            If line.StartsWith("//") Then
                ' 转义的字面量斜杠：//abc  ==>  /abc
                If newBuf.Count = 0 Then
                    Call newBuf.Add(line.Substring(2))
                Else
                    newBuf(newBuf.Count - 1) &= "/" & line.Substring(2)
                End If
            ElseIf line.StartsWith("/") Then
                ' 续行：与上一行使用单个空格连接，避免原有的直接拼接产生的单词粘连
                Dim cont As String = line.Substring(1).Trim()

                If newBuf.Count = 0 Then
                    Call newBuf.Add(cont)
                ElseIf cont.Length = 0 Then
                    ' 单独的 '/' 行表示段落分隔
                    newBuf(newBuf.Count - 1) &= " "
                ElseIf newBuf(newBuf.Count - 1).EndsWith(" ") Then
                    newBuf(newBuf.Count - 1) &= cont
                Else
                    newBuf(newBuf.Count - 1) &= " " & cont
                End If
            Else
                Call newBuf.Add(line)
            End If
        Next

        Return newBuf
    End Function

    ''' <summary>
    ''' 移除字符串之中的非法XML字符。
    ''' 
    ''' BioCyc的平面文件之中偶尔会包含不可打印的控制字符(例如 0x11)，
    ''' 这些字符会破坏最终生成的XML模型文件，因此在解析阶段就将其剔除。
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Friend Shared Function sanitize(value As String) As String
        If value.StringEmpty Then
            Return value
        End If

        Dim sb As New StringBuilder(value.Length)

        For Each ch As Char In value
            Dim code As Integer = AscW(ch)

            If ch = ControlChars.Tab OrElse ch = ControlChars.LF OrElse ch = ControlChars.CR Then
                Call sb.Append(ch)
            ElseIf code < &H20 OrElse code = &HFFFE OrElse code = &HFFFF Then
                Continue For
            Else
                Call sb.Append(ch)
            End If
        Next

        Return sb.ToString()
    End Function

    Friend Shared Function ParseBuffer(buffer As String()) As FeatureElement
        Dim attrs As New List(Of NamedValue(Of ValueString))
        Dim innerAttrs As New List(Of NamedValue(Of String))
        Dim temp As NamedValue(Of String) = Nothing

        buffer = removeBreakLines(buffer).ToArray

        For Each line As String In buffer
            If line.StartsWith("^") Then
                ' is attribute value of the previous string value
                Dim annotation = line.Trim("^").GetTagValue(" - ")

                Call innerAttrs.Add(New NamedValue(Of String)(annotation.Name, sanitize(annotation.Value)))
            ElseIf Not line.Contains(" - ") Then
                ' 不合法的属性行(缺少分隔符号)：直接跳过而不是产生一个畸形的属性对象
                Continue For
            Else
                If Not temp.IsEmpty Then
                    Call attrs.Add(New NamedValue(Of ValueString) With {
                        .Name = temp.Name,
                        .Value = New ValueString With {
                            .value = sanitize(temp.Value),
                            .attributes = innerAttrs.PopAll
                        }
                    })
                End If

                temp = line.GetTagValue(" - ")
            End If
        Next

        Call attrs.Add(New NamedValue(Of ValueString) With {
            .Name = temp.Name,
            .Value = New ValueString With {
                .value = sanitize(temp.Value),
                .attributes = innerAttrs.PopAll
            }
        })

        Return New FeatureElement With {
            .attributes = attrs _
                .Where(Function(a) Not a.Name Is Nothing) _
                .GroupBy(Function(s) s.Name) _
                .ToDictionary(Function(s) s.Key,
                              Function(s)
                                  Return (From a As NamedValue(Of ValueString) In s Select a.Value).ToArray
                              End Function)
        }
    End Function

End Class

Public Class ValueString

    Public Property value As String
    Public Property attributes As NamedValue(Of String)()

    Default Public ReadOnly Property getValue(key As String) As String
        Get
            Return attributes _
                .Where(Function(a) a.Name = key) _
                .FirstOrDefault _
                .Value
        End Get
    End Property

    Public Function getAttributes(key As String) As String()
        Return (From attr As NamedValue(Of String)
                In attributes
                Where attr.Name = key
                Select attr.Value).ToArray
    End Function

    Public Overrides Function ToString() As String
        Return value
    End Function

    Public Shared Operator &(val As ValueString, str As String) As ValueString
        val.value &= str
        Return val
    End Operator

End Class
