#Region "Microsoft.VisualBasic::e172424164cc415f035295ce627774ef, ..\sciBASIC.ComputingServices\LINQ\TestMain\Parser\Enums.vb"

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

Namespace Parser

    ''' <summary>
    ''' Enumerates the types of tokens that can be classified by the tokenizer.
    ''' </summary>
    Public Enum TokenType
        ''' <summary>
        ''' This is reserved for the NullToken.
        ''' </summary>
        NotAToken

        ''' <summary>
        ''' An identifier can be either a class or property name.  The tokenizer does 
        ''' not have enough information to make that distinction.
        ''' </summary>
        Identifier

        ''' <summary>
        ''' An operator like + or *.
        ''' </summary>
        [Operator]

        ''' <summary>
        ''' A comma
        ''' </summary>
        Comma

        ''' <summary>
        ''' A dot (".").
        ''' </summary>
        Dot

        ''' <summary>
        ''' A primitive like a quoted string, boolean value, or number.
        ''' </summary>
        Primitive

        ''' <summary>
        ''' Open parenthesis
        ''' </summary>
        OpenParens

        ''' <summary>
        ''' Close parenthesis
        ''' </summary>
        CloseParens

        ''' <summary>
        ''' Open bracket
        ''' </summary>
        OpenBracket

        ''' <summary>
        ''' Close bracket
        ''' </summary>
        CloseBracket

        ''' <summary>
        ''' Double quote token, only used internally by tokenizer.
        ''' </summary>
        Quote
    End Enum

    ''' <summary>
    ''' Indicates priority in order of operations.
    ''' </summary>
    Public Enum TokenPriority
        ''' <summary>
        ''' Default
        ''' </summary>
        None

        ''' <summary>
        ''' Bitwise or
        ''' </summary>
        [Or]

        ''' <summary>
        ''' Bitwise and
        ''' </summary>
        [And]

        ''' <summary>
        ''' Bitwise not
        ''' </summary>
        [Not]

        ''' <summary>
        ''' Equality comparisons like &gt;, &lt;=, ==, etc.
        ''' </summary>
        Equality

        ''' <summary>
        ''' Plus or minus
        ''' </summary>
        PlusMinus

        ''' <summary>
        ''' Modulus
        ''' </summary>
        [Mod]

        ''' <summary>
        ''' Multiply or divide
        ''' </summary>
        MulDiv

        ''' <summary>
        ''' Unary minus
        ''' </summary>
        UnaryMinus
    End Enum
End Namespace
