#Region "Microsoft.VisualBasic::83845cccc71760278daae19ac99364f9, typescript\Linq.ts\TsBuild\TsBuild\Syntax\TypeScriptDefinition\Symbols.vb"

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

    ' Enum TypeScriptTokens
    ' 
    '     [declare], [function], [operator], [string], closeStack
    '     comment, constructor, delimiter, functionCalls, functionName
    '     funcType, identifier, keyword, logicalLiteral, numberLiteral
    '     openStack, typeName
    ' 
    '  
    ' 
    ' 
    ' 
    ' Class Token
    ' 
    '     Properties: ends, start, text, type
    ' 
    '     Function: ToString
    '     Operators: (+2 Overloads) <>, (+2 Overloads) =
    ' 
    ' Class Escapes
    ' 
    '     Properties: BlockTextComment, SingleLineComment, StringContent
    ' 
    ' Class JavaScriptEscapes
    ' 
    '     Properties: StringStackSymbol
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Public Enum TypeScriptTokens
    undefined = 0
    [declare]
    keyword
    [function]
    functionName
    functionCalls
    identifier
    typeName
    funcType
    comment
    delimiter
    logicalLiteral
    numberLiteral
    constructor
    [operator]
    openStack
    ''' <summary>
    ''' 是一个字符串常量表达式
    ''' </summary>
    [string]
    closeStack
End Enum

<XmlType("token")> Public Class Token

    <XmlAttribute("type")> Public Property type As TypeScriptTokens
    <XmlText> Public Property text As String

    <XmlAttribute> Public Property start As Integer
    <XmlAttribute> Public Property ends As Integer

    Public Overrides Function ToString() As String
        Return $"[{type}] {text}"
    End Function

    Public Shared Operator =(t As Token, type As TypeScriptTokens) As Boolean
        Return t.type = type
    End Operator

    Public Shared Operator <>(t As Token, type As TypeScriptTokens) As Boolean
        Return t.type <> type
    End Operator

    Public Shared Operator <>(t As Token, text As String) As Boolean
        Return t.text <> text
    End Operator

    Public Shared Operator =(t As Token, text As String) As Boolean
        Return t.text = text
    End Operator
End Class

Public Class Escapes

    Public Property SingleLineComment As Boolean
    Public Property BlockTextComment As Boolean
    Public Property StringContent As Boolean

End Class

Public Class JavaScriptEscapes : Inherits Escapes
    Public Property StringStackSymbol As Char
End Class
