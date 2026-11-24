#Region "Microsoft.VisualBasic::c61950af5b33ea23c840a33d5e18c2de, sub-system\PLAS.NET\SSystem\Script\ScriptText\Tokens.vb"

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

    '   Total Lines: 57
    '    Code Lines: 22 (38.60%)
    ' Comment Lines: 30 (52.63%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (8.77%)
    '     File Size: 1.47 KB


    '     Class ScriptToken
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Enum ScriptTokens
    ' 
    '         UNDEFINE
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Script

    Public Class ScriptToken : Inherits CodeToken(Of ScriptTokens)

        ''' <summary>
        ''' 当前记号所在的脚本文本的行号，从1开始计数，用于语法错误的精确定位
        ''' </summary>
        ''' <returns></returns>
        Public Property lineNumber As Integer

        ''' <summary>
        ''' 该记号所对应的原始脚本文本行
        ''' </summary>
        ''' <returns></returns>
        Public Property raw As String

        Sub New(token As ScriptTokens, text As String)
            Call MyBase.New(token, text)
        End Sub

        Sub New(token As ScriptTokens, text As String, lineNumber As Integer, raw As String)
            Call MyBase.New(token, text)
            Me.lineNumber = lineNumber
            Me.raw = raw
        End Sub

        ''' <summary>
        ''' 唯一的记号描述信息
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"[{lineNumber}] {name}: {text}"
        End Function
    End Class

    ''' <summary>
    ''' 脚本解析过程之中的诊断信息（并非致命错误，可视具体情况忽略或者抛出异常）
    ''' </summary>
    Public Class ScriptError

        ''' <summary>
        ''' 出错的脚本行号，从1开始计数
        ''' </summary>
        ''' <returns></returns>
        Public Property lineNumber As Integer
        ''' <summary>
        ''' 出错的脚本行的原始文本内容
        ''' </summary>
        ''' <returns></returns>
        Public Property line As String
        ''' <summary>
        ''' 问题描述信息
        ''' </summary>
        ''' <returns></returns>
        Public Property message As String

        ''' <summary>
        ''' Gets the display text of current syntax diagnostic message.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"line {lineNumber}: {message}    {line}"
        End Function
    End Class

    Public Enum ScriptTokens
        UNDEFINE

        ''' <summary>
        ''' TITLE
        ''' </summary>
        <Description("TITLE")> Title
        ''' <summary>
        ''' RXN
        ''' </summary>
        <Description("RXN")> Reaction
        ''' <summary>
        ''' INIT
        ''' </summary>
        <Description("INIT")> InitValue
        ''' <summary>
        ''' FUNC
        ''' </summary>
        <Description("FUNC")> [Function]
        ''' <summary>
        ''' FINALTIME
        ''' </summary>
        <Description("FINALTIME")> Time
        ''' <summary>
        ''' NAMED
        ''' </summary>
        <Description("NAMED")> [Alias]
        ''' <summary>
        ''' STIMULATE
        ''' </summary>
        <Description("STIMULATE")> Disturb
        ''' <summary>
        ''' COMMENT
        ''' </summary>
        <Description("COMMENT")> Comment
        ''' <summary>
        ''' REM-SUB
        ''' </summary>
        <Description("REM-SUB")> SubsComments
        ''' <summary>
        ''' CONST
        ''' </summary>
        <Description("CONST")> Constant
    End Enum
End Namespace
