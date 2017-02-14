#Region "Microsoft.VisualBasic::30515471dd42a8604352b49a7370cfa8, ..\GCModeller\data\BASys\ecard\ecardParser.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language

Public Module ecardParser

    Public Function ParseFile(path As String, Optional ByRef tag As NamedValue(Of String) = Nothing) As IEnumerable(Of Dictionary(Of String, String()))
        Dim tokens = path.ReadAllText.Parsing
        Dim tagLines As String() = path.IterateAllLines.Take(4).Where(
            Function(s) Not s.StringEmpty).ToArray

        tag = New NamedValue(Of String) With {
            .Name = Strings.Split(tagLines(Scan0), vbTab).Last,
            .Value = Strings.Split(tagLines(1), vbTab).Last
        }

        Return tokens
    End Function

    Const BlastOutput As String = "Annotation::BlastOutput"

    ReadOnly __keys As String() = EcardValue.Schema.Keys.ToArray

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="content"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 请注意，迭代器函数并不会马上就执行的，所以byref或者任何想要修改参数对象里面的属性的方法在这里都已经失效了
    ''' 所以tag数据被放到了<see cref="ParseFile"/>函数之中来完成
    ''' </remarks>
    <Extension>
    Public Iterator Function Parsing(content As String) As IEnumerable(Of Dictionary(Of String, String()))
        Dim lines As String() = content.lTokens
        Dim tokens As IEnumerable(Of String()) = lines.Skip(4).Split("//")

        For Each part As String() In tokens
            Dim tmp As New List(Of NamedValue(Of String))
            Dim isBlastOutput As Boolean = False
            Dim out As StringBuilder = Nothing
            Dim isEnd As Boolean = False
            Dim value As New StringBuilder
            Dim lastKey As String = ""

            For Each line As String In If(
                String.IsNullOrEmpty(part(Scan0)),
                part.Skip(1),
                DirectCast(part, IEnumerable(Of String)))

                If isBlastOutput And out IsNot Nothing Then
                    If String.IsNullOrEmpty(line) AndAlso isEnd Then
                        isBlastOutput = False
                    Else
                        out.AppendLine(line)
                    End If

                    If Regex.Match(line, "Matrix: BLOSUM\d+").Success Then
                        isEnd = True
                    End If
                Else
                    Dim tagValue = line.GetTagValue(vbTab, True)

                    If tagValue.Value = BlastOutput Then  ' 当前的这个节点是否是blast输出
                        isBlastOutput = True
                    End If
                    If tagValue.Name = "VALUE" AndAlso isBlastOutput Then
                        ' 解析blast输出
                        out = New StringBuilder
                        out.AppendLine(tagValue.Value)
                    Else
                        If Array.IndexOf(__keys, tagValue.Name) = -1 Then
                            Call value.Append(vbCrLf & line)
                        Else
                            If Not String.IsNullOrEmpty(lastKey) Then
                                tmp += New NamedValue(Of String) With {
                                    .Name = lastKey,
                                    .Value = value.ToString
                                }
                            End If
                            lastKey = tagValue.Name
                            value.Clear()
                            value.Append(tagValue.Value)
                        End If
                    End If
                End If
            Next

            If out IsNot Nothing AndAlso out.Length > 0 Then
                tmp += New NamedValue(Of String) With {
                    .Name = "VALUE",
                    .Value = out.ToString
                }
            End If

            If Not String.IsNullOrEmpty(lastKey) Then
                tmp += New NamedValue(Of String) With {
                    .Value = value.ToString,
                    .Name = lastKey
                }
            End If

            Dim Groups = From x As NamedValue(Of String)
                         In tmp
                         Select x
                         Group x By x.Name Into Group '

            Yield Groups.ToDictionary(Function(x) x.Name,
                                      Function(x) x.Group.ToArray(
                                      Function(o) o.Value))
        Next
    End Function
End Module
