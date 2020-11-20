﻿#Region "Microsoft.VisualBasic::5e3fb7ff70f094915c4b59cf2120abfd, Microsoft.VisualBasic.Core\CommandLine\Parsers\CLIParser.vb"

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

    '     Module CLIParser
    ' 
    '         Function: checkKeyDuplicated, GetTokens, (+2 Overloads) TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser
Imports StringList = System.Collections.Generic.IEnumerable(Of String)

Namespace CommandLine.Parsers

    ''' <summary>
    ''' 命令行单词解析器
    ''' </summary>
    Public Module CLIParser

        ''' <summary>
        ''' 非正则表达式命令行解析引擎
        ''' </summary>
        ''' <param name="cli">the commandline string</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' + 双引号表示一个完整的token
        ''' + 空格为分隔符
        ''' </remarks>
        <Extension> Public Function GetTokens(cli As String) As String()
            Dim buffer As New CharPtr(cli)
            Dim tokens As New List(Of String)
            Dim tmp As New List(Of Char)
            Dim c As Char
            Dim quotOpen As Boolean = False

            Do While Not buffer.EndRead
                c = (+buffer)

                If quotOpen Then

                    ' 双引号是结束符，但是可以使用\"进行转义
                    If c <> ASCII.Quot Then
                        tmp += c
                    Else
                        If tmp.StartEscaping Then
                            tmp.RemoveLast
                            tmp += c
                        Else
                            ' 结束
                            tokens += tmp.CharString
                            tmp *= 0
                            quotOpen = False

                        End If
                    End If

                Else
                    If c = ASCII.Quot AndAlso tmp = 0 Then
                        quotOpen = True
                    ElseIf c = " "c Then
                        ' 分隔符
                        If tmp <> 0 Then
                            tokens += tmp.CharString
                            tmp *= 0
                        End If
                    Else
                        tmp += c
                    End If
                End If
            Loop

            If tmp <> 0 Then
                tokens += New String(tmp)
            End If

            Return tokens
        End Function

        <Extension>
        Private Iterator Function extract(tokens As IEnumerable(Of String)) As IEnumerable(Of String)
            For Each token As String In tokens
                If token.StartsWith("-") OrElse token.StartsWith("/") Then
                    If token.IndexOf("="c) > -1 Then
                        With token.GetTagValue("=", trim:=True)
                            Yield .Name
                            Yield .Value
                        End With
                    Else
                        Yield token
                    End If
                Else
                    Yield token
                End If
            Next
        End Function

        ''' <summary>
        ''' Try parsing the cli command string from the string value.(尝试着从文本行之中解析出命令行参数信息)
        ''' </summary>
        ''' <param name="args">The commandline arguments which is user inputs from the terminal.</param>
        ''' <param name="duplicatedAllows">Allow the duplicated command parameter argument name in the input, 
        ''' default is not allowed the duplication.(是否允许有重复名称的参数名出现，默认是不允许的)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("TryParse")>
        <Extension>
        Public Function TryParse(args As StringList,
                                 Optional duplicatedAllows As Boolean = False,
                                 Optional rawInput$ = Nothing) As CommandLine

            Dim tokens$() = args.SafeQuery.ToArray
            Dim singleValue$ = ""

            If tokens.Length = 0 Then
                Return New CommandLine
            Else
                tokens = tokens _
                    .fixWindowsNetworkDirectory _
                    .extract _
                    .ToArray
            End If

            Dim bools$() = tokens _
                .Skip(1) _
                .GetLogicalFlags(singleValue)
            Dim cli As New CommandLine With {
                .Name = tokens(Scan0),
                .Tokens = tokens,
                .BoolFlags = bools,
                .cliCommandArgvs = Join(tokens)
            }

            cli.SingleValue = singleValue
            cli.cliCommandArgvs = rawInput

            If cli.Parameters.Length = 1 AndAlso
                String.IsNullOrEmpty(cli.SingleValue) Then

                cli.SingleValue = cli.Parameters(0)
            End If

            If tokens.Length > 1 Then
                cli.arguments = tokens.Skip(1).ToArray.CreateParameterValues(False)

                Dim Dk As String() = checkKeyDuplicated(cli.arguments)

                If Not duplicatedAllows AndAlso Not Dk.IsNullOrEmpty Then
                    Dim Key$ = String.Join(", ", Dk)
                    Dim msg$ = String.Format(KeyDuplicated, Key, String.Join(" ", tokens.Skip(1).ToArray))

                    Throw New Exception(msg)
                End If
            End If

            Return cli
        End Function

        Const KeyDuplicated As String = "The command line switch key ""{0}"" Is already been added! Here Is your input data:  CMD {1}."

        Private Function checkKeyDuplicated(source As IEnumerable(Of NamedValue(Of String))) As String()
            Dim LQuery = (From param As NamedValue(Of String)
                          In source
                          Select param.Name.ToLower
                          Group By ToLower Into Group).ToArray

            Return LinqAPI.Exec(Of String) _
 _
                () <= From group
                      In LQuery
                      Where group.Group.Count > 1
                      Select group.ToLower
        End Function

        ''' <summary>
        ''' Try parsing the cli command string from the string value.
        ''' (尝试着从文本行之中解析出命令行参数信息，假若value里面有空格，则必须要将value添加双引号)
        ''' </summary>
        ''' <param name="CLI">The commandline arguments which is user inputs from the terminal.</param>
        ''' <param name="duplicateAllowed">Allow the duplicated command parameter argument name in the input, 
        ''' default is not allowed the duplication.(是否允许有重复名称的参数名出现，默认是不允许的)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("TryParse")>
        Public Function TryParse(<Parameter("CLI", "The CLI arguments that inputs from the console by user.")> CLI$,
                                 <Parameter("Duplicates.Allowed")>
                                 Optional duplicateAllowed As Boolean = False) As CommandLine

            If String.IsNullOrEmpty(CLI) Then
                Return New CommandLine
            Else
#Const DEBUG = False
#If DEBUG Then
                Call CLI.__DEBUG_ECHO
#End If
            End If

            Dim args As CommandLine = CLITools _
                .GetTokens(CLI) _
                .TryParse(duplicateAllowed, rawInput:=CLI)

            Return args
        End Function
    End Module
End Namespace
