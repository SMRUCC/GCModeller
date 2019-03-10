﻿#Region "Microsoft.VisualBasic::45d21ea902dbe434ef06bebc197d1579, Microsoft.VisualBasic.Core\ApplicationServices\VBDev\Signature\VBCodeSignature.vb"

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

    '     Module VBCodeSignature
    ' 
    '         Function: memberList, RemoveAttributes, SummaryInternal, SummaryModules, typeSummary
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex
Imports VBCodePatterns = Microsoft.VisualBasic.Scripting.SymbolBuilder.VBLanguage.Patterns

Namespace ApplicationServices.Development

    ''' <summary>
    ''' 在这个模块之中对VB的代码文件进行大纲摘要的提取操作
    ''' </summary>
    Public Module VBCodeSignature

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function RemoveAttributes(line As String) As String
            Return r.Replace(line, VBCodePatterns.Attribute, "", RegexICSng)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function SummaryModules(vb As String) As String
            Dim vblines As Pointer(Of String) = vb _
                .LineTokens _
                .Select(AddressOf RemoveAttributes) _
                .ToArray

            With New StringBuilder
                Do While Not vblines.EndRead
                    Call .AppendLine(vblines.SummaryInternal(vb))
                Loop

                Return .ToString
            End With
        End Function

        <Extension>
        Private Function SummaryInternal(vblines As Pointer(Of String), vb$) As String
            Dim line$
            Dim tokens As Value(Of String) = ""
            Dim list As List(Of String)
            Dim type$
            Dim name$
            Dim indents$
            Dim properties As New List(Of NamedValue(Of String))
            Dim methods As New List(Of NamedValue(Of String))
            Dim operators As New List(Of NamedValue(Of String))
            Dim container As New NamedValue(Of String)
            Dim innerModules As New StringBuilder

            Do While Not vblines.EndRead
                line = ++vblines

                If Not (tokens = line.Match(VBCodePatterns.Type, RegexICMul)).StringEmpty Then
                    list = tokens.Split(" "c).AsList
                    type = list(-2)
                    name = list(-1)
                    indents = line.Match(VBCodePatterns.Indents, RegexICMul)

                    If type = "Enum" Then
                        Dim members = vb _
                            .Match("Enum\s+" & name & ".+?End Enum", RegexICSng) _
                            .LineTokens _
                            .Where(Function(s) s.IsPattern("\s+" & VBCodePatterns.Identifer & "\s*([=].+?)?\s*")) _
                            .Select(AddressOf Trim) _
                            .Where(Function(s) Not s.StringEmpty) _
                            .ToArray

                        Dim enumType As New StringBuilder
                        Dim memberList = members.memberList

                        enumType.AppendLine(indents & type & " " & name)
                        enumType.AppendLine()

                        For Each line In memberList
                            enumType.AppendLine(indents & "    " & line)
                        Next

                        If container.IsEmpty Then
                            Return enumType.ToString
                        Else
                            innerModules.AppendLine(enumType.ToString)
                        End If
                    Else
                        If container.IsEmpty Then
                            container = New NamedValue(Of String)(name, type, indents.Trim(ASCII.CR, ASCII.LF))
                        Else
                            ' 下一层堆栈
                            innerModules.AppendLine((vblines - 1).SummaryInternal(vb))
                        End If
                    End If
                End If
                If Not (tokens = line.Match(VBCodePatterns.Property, RegexICMul)).StringEmpty Then
                    list = tokens.Split(" "c).AsList
                    type = list(-2)
                    name = list(-1)
                    indents = line.Match(VBCodePatterns.Indents, RegexICMul)

                    properties += New NamedValue(Of String)(name, type, indents)
                End If
                If Not (tokens = line.Match(VBCodePatterns.Method, RegexICMul)).StringEmpty Then
                    list = tokens.Split(" "c).AsList
                    type = list(-2)
                    name = list(-1)
                    indents = line.Match(VBCodePatterns.Indents, RegexICMul)

                    If type = "Operator" Then
                        operators += New NamedValue(Of String)(name, type, indents)
                    Else
                        methods += New NamedValue(Of String)(name, type, indents)
                    End If
                End If
                If Not (tokens = line.Match(VBCodePatterns.Operator, RegexICMul)).StringEmpty Then
                    list = tokens.Split(" "c).AsList
                    type = list(-2)
                    name = list(-1)
                    indents = line.Match(VBCodePatterns.Indents, RegexICMul)

                    If type = "Operator" Then
                        operators += New NamedValue(Of String)(name, type, indents)
                    Else
                        methods += New NamedValue(Of String)(name, type, indents)
                    End If
                End If
                If Not (tokens = line.Match(VBLanguage.Patterns.CloseType, RegexICMul)).StringEmpty Then
                    Return container.typeSummary(properties, methods, operators, innerModules)
                End If
            Loop

            If Not container.IsEmpty Then
                Return container.typeSummary(properties, methods, operators, innerModules)
            ElseIf Not innerModules.Length = 0 Then
                Return innerModules.ToString
            Else
                Return ""
            End If
        End Function

        <Extension>
        Private Function typeSummary(container As NamedValue(Of String),
                                     properties As List(Of NamedValue(Of String)),
                                     methods As List(Of NamedValue(Of String)),
                                     operators As List(Of NamedValue(Of String)),
                                     innerModules As StringBuilder) As String

            Dim vbType As New StringBuilder
            Dim members As New List(Of String)
            Dim prefix$
            Dim lines$()

            vbType.AppendLine(container.Description & container.Value & " " & container.Name)
            vbType.AppendLine()

            If Not properties.IsNullOrEmpty Then
                prefix = container.Description & "    Properties: "
                lines = properties.Keys.memberList
                members += prefix & lines(Scan0)

                If lines.Length > 1 Then
                    members += lines _
                        .Skip(1) _
                        .Select(Function(l) New String(" "c, prefix.Length) & l) _
                        .JoinBy(ASCII.LF)
                End If

                If Not methods.IsNullOrEmpty Then
                    members += ""
                End If
            End If
            If Not methods.IsNullOrEmpty Then
                Dim constructors = methods _
                    .Where(Function(s) s.Name = "New") _
                    .ToArray
                Dim types = methods _
                    .Where(Function(s) s.Name <> "New") _
                    .GroupBy(Function(m) m.Value) _
                    .ToDictionary(Function(t) t.Key,
                                  Function(l) l.Keys.memberList)

                If constructors.Length > 0 Then
                    members += container.Description & $"    Constructor: (+{constructors.Count} Overloads) Sub New"

                    If types.Count > 1 Then
                        members += ""
                    End If
                End If

                If types.ContainsKey("Function") Then
                    prefix = container.Description & $"    Function: "
                    members += prefix & types!Function.First

                    If types!Function.Length > 1 Then
                        members += types!Function _
                            .Skip(1) _
                            .Select(Function(l) New String(" "c, prefix.Length) & l) _
                            .JoinBy(ASCII.LF)
                    End If

                    If types.Count > 1 Then
                        members += ""
                    End If
                End If
                If types.ContainsKey("Sub") Then
                    prefix = container.Description & $"    Sub: "
                    members += prefix & types!Sub.First

                    If types!Sub.Length > 1 Then
                        members += types!Sub _
                            .Skip(1) _
                            .Select(Function(l) New String(" "c, prefix.Length) & l) _
                            .JoinBy(ASCII.LF)
                    End If

                    If Not operators.IsNullOrEmpty Then
                        members += ""
                    End If
                End If
            End If
            If Not operators.IsNullOrEmpty Then
                prefix = container.Description & "    Operators: "
                lines = operators.Keys.memberList
                members += prefix & lines(Scan0)

                If lines.Length > 1 Then
                    members += lines _
                        .Skip(1) _
                        .Select(Function(l) New String(" "c, prefix.Length) & l) _
                        .JoinBy(ASCII.LF)
                End If
            End If

            vbType.AppendLine(members.JoinBy(ASCII.LF))

            If innerModules.Length > 0 Then
                vbType.AppendLine(innerModules.ToString)
            End If

            Return vbType.ToString
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Private Function memberList(names As IEnumerable(Of String)) As String()
            Return names _
                .GroupBy(Function(pName) pName) _
                .OrderBy(Function(pName) pName.Key) _
                .Select(Function(overload)
                            If overload.Count = 1 Then
                                Return overload.Key
                            Else
                                Return $"(+{overload.Count} Overloads) " & overload.Key
                            End If
                        End Function) _
                .Split(5) _
                .Select(Function(part) part.JoinBy(", ")) _
                .ToArray
        End Function
    End Module
End Namespace
