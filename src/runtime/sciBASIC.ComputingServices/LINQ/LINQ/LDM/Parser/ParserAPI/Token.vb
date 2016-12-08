#Region "Microsoft.VisualBasic::4ea3e2ca89762c37fa9e03f5ddcfe295, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Parser\ParserAPI\Token.vb"

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

Imports Microsoft.VisualBasic.Linq.LDM.Statements.TokenIcer
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace LDM.Parser


    ''' <summary>
    ''' Represents a token that is parsed out by the <see cref="Tokenizer"/>.
    ''' </summary>
    Public NotInheritable Class Token : Inherits Token(Of Tokens)

        ''' <summary>
        ''' If the token can be parsed into a type like an integer, this property holds that value.
        ''' </summary>
        Public ReadOnly Property ParsedObject() As Object

        ''' <summary>
        ''' Token priority
        ''' </summary>
        Public ReadOnly Property Priority() As TokenPriority

        ''' <summary>
        ''' Constructor for tokens that are not parsed.
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="type"></param>
        ''' <param name="priority"></param>
        Public Sub New(text As String, type As Tokens, priority As TokenPriority)
            Call MyBase.New(type, text)
            _Priority = priority
            _ParsedObject = text
        End Sub

        Sub New(source As Token(Of Tokens), priority As TokenPriority)
            Call Me.New(source.TokenValue, source.TokenName, priority)
        End Sub

        Sub New(source As Token(Of Tokens))
            Call Me.New(source.TokenValue, source.TokenName, TokenPriority.None)
        End Sub

        ''' <summary>
        ''' Constructor for tokens that are parsed.
        ''' </summary>
        ''' <param name="parsedObj"></param>
        ''' <param name="type"></param>
        ''' <param name="priority"></param>
        Public Sub New(parsedObj As Object, type As Tokens, priority As TokenPriority)
            Call MyBase.New(type, Scripting.ToString(parsedObj))
            _ParsedObject = parsedObj
            _Priority = priority
        End Sub

        ''' <summary>
        ''' The null token represents a state where the <see cref="Tokenizer"/> encountered an error
        ''' or has not begun parsing yet.
        ''' </summary>
        Public Shared ReadOnly Property NullToken As New Token("", Tokens.UNDEFINED, TokenPriority.None)

        Public Overrides Function ToString() As String
            Return MyBase.ToString()
        End Function
    End Class
End Namespace
