#Region "Microsoft.VisualBasic::97c5f94cdee8264d441f6e161b874171, ..\sciBASIC.ComputingServices\LINQ\TestMain\Parser\Token.vb"

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
    ''' Represents a token that is parsed out by the <see cref="Tokenizer"/>.
    ''' </summary>
    Public NotInheritable Class Token
        Private _Text As String
        Private _ParsedObject As Object
        Private _Type As TokenType
        Private _Priority As TokenPriority

        ''' <summary>
        ''' The text that makes up the token.
        ''' </summary>
        Public ReadOnly Property Text() As String
            Get
                Return _Text
            End Get
        End Property

        ''' <summary>
        ''' If the token can be parsed into a type like an integer, this property holds that value.
        ''' </summary>
        Public ReadOnly Property ParsedObject() As Object
            Get
                Return _ParsedObject
            End Get
        End Property

        ''' <summary>
        ''' Token type
        ''' </summary>
        Public ReadOnly Property Type() As TokenType
            Get
                Return _Type
            End Get
        End Property

        ''' <summary>
        ''' Token priority
        ''' </summary>
        Public ReadOnly Property Priority() As TokenPriority
            Get
                Return _Priority
            End Get
        End Property

        ''' <summary>
        ''' Constructor for tokens that are not parsed.
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="type"></param>
        ''' <param name="priority"></param>
        Public Sub New(text As String, type As TokenType, priority As TokenPriority)
            _Text = text
            _Type = type
            _Priority = priority
            _ParsedObject = text
        End Sub

        ''' <summary>
        ''' Constructor for tokens that are parsed.
        ''' </summary>
        ''' <param name="parsedObj"></param>
        ''' <param name="type"></param>
        ''' <param name="priority"></param>
        Public Sub New(parsedObj As Object, type As TokenType, priority As TokenPriority)
            _ParsedObject = parsedObj
            _Text = ParsedObject.ToString()
            _Type = type
            _Priority = priority
        End Sub

        ''' <summary>
        ''' The null token represents a state where the <see cref="Tokenizer"/> encountered an error
        ''' or has not begun parsing yet.
        ''' </summary>
        Public Shared NullToken As New Token("", TokenType.NotAToken, TokenPriority.None)
    End Class
End Namespace
