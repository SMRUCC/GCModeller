#Region "Microsoft.VisualBasic::061cabebbc64c5b6319eeba62fa863a4, sub-system\PLAS.NET\SSystem\Script\ScriptText\TokenIcer.vb"

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

    '   Total Lines: 55
    '    Code Lines: 40 (72.73%)
    ' Comment Lines: 5 (9.09%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (18.18%)
    '     File Size: 1.74 KB


    '     Module TokenIcer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: tokenParser, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Script

    Public Module TokenIcer

        ReadOnly tokens As New Dictionary(Of String, ScriptTokens)

        Sub New()
            For Each token As ScriptTokens In Enums(Of ScriptTokens)()
                If Not String.IsNullOrEmpty(token.Description) Then
                    tokens.Add(token.Description, token)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 注释行的前缀符号，以这些符号开头的脚本行会被直接忽略掉
        ''' </summary>
        ReadOnly commentPrefix As String() = {"//", "/*", "*", "#", "'"}

        ''' <summary>
        ''' 判断目标脚本文本行是否为注释行
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Public Function IsCommentLine(line As String) As Boolean
            If String.IsNullOrWhiteSpace(line) Then
                Return True
            End If

            Dim trim As String = line.Trim

            For Each prefix As String In commentPrefix
                If trim.StartsWith(prefix) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Public Function TryParse(script As IEnumerable(Of String)) As ScriptToken()
            Dim errors As List(Of ScriptError) = Nothing

            Return TryParse(script, errors)
        End Function

        ''' <summary>
        ''' 会忽略掉注释信息，无法识别的脚本行会被记录在<paramref name="errors"/>诊断列表之中
        ''' </summary>
        ''' <param name="script"></param>
        ''' <param name="errors">脚本文法错误信息的收集容器</param>
        ''' <returns></returns>
        Public Function TryParse(script As IEnumerable(Of String), ByRef errors As List(Of ScriptError)) As ScriptToken()
            Dim i As Integer = 0
            Dim token As ScriptToken
            Dim out As New List(Of ScriptToken)

            For Each line As String In script
                i += 1
                token = tokenParser(line, i, errors)

                If Not token Is Nothing Then
                    Call out.Add(token)
                End If
            Next

            Return out.ToArray
        End Function

        ''' <summary>
        ''' 会忽略掉注释信息
        ''' </summary>
        ''' <param name="line">类型的前缀应该是已经被切割掉了</param>
        ''' <param name="lineNumber">当前文本行在脚本之中的行号，从1开始计数</param>
        ''' <param name="errors">脚本文法错误信息的收集容器</param>
        ''' <returns></returns>
        Private Function tokenParser(line As String, lineNumber As Integer, ByRef errors As List(Of ScriptError)) As ScriptToken
            Dim raw As String = line
            Dim x As String
            Dim type As ScriptTokens
            Dim text As String

            line = If(line, "").Trim()

            If IsCommentLine(line) Then
                Return Nothing
            End If

            x = line.Split.First.ToUpper

            If Not tokens.ContainsKey(x) Then
                Call appendError(errors, lineNumber, raw, $"Unknown script keyword ""{x}"", this line was ignored by the script compiler.")
                Return Nothing
            Else
                type = tokens(x)
            End If

            text = Mid(line, x.Length + 1).Trim

            Return New ScriptToken(type, text, lineNumber, raw)
        End Function

        Private Sub appendError(ByRef errors As List(Of ScriptError), lineNumber As Integer, line As String, message As String)
            If errors Is Nothing Then
                errors = New List(Of ScriptError)
            End If

            Call errors.Add(New ScriptError With {
                .line = line,
                .lineNumber = lineNumber,
                .message = message
            })
        End Sub
    End Module
End Namespace
