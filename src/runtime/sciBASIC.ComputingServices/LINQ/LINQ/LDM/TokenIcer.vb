#Region "Microsoft.VisualBasic::02586dc19378e7a8117eec6568f4eb82, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\TokenIcer.vb"

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

' This file was Auto Generated with TokenIcer
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace LDM.Statements.TokenIcer

    ' This is our token enumeration. It holds every token defined in the grammar
    ''' <summary>
    ''' Tokens is an enumeration of all possible token values.
    ''' </summary>
    Public Enum Tokens
        UNDEFINED = 0
        [As] = 1
        From = 2
        Where = 3
        [Select] = 4
        [Imports] = 5
        [In] = 6
        [Let] = 7
        Join = 8
        Group = 9
        By = 10
        Order = 11
        Into = 12
        [Return] = 13
        OpenParens = 14
        CloseParens = 15
        Pretend = 16
        Comma = 17
        WhiteSpace = 18
        [String] = 19
        VarRef = 20
        Code = 21
    End Enum

    Public Module Parser

        'Public ReadOnly Property stackT As StackTokens(Of Tokens) =
        '    New StackTokens(Of Tokens)(Function(a, b) a = b) With {
        '        .LPair = Tokens.LPair,
        '        .ParamDeli = Tokens.ParamDeli,
        '        .Pretend = Tokens.Pretend,
        '        .RPair = Tokens.RPair,
        '        .WhiteSpace = Tokens.WhiteSpace
        '}

        Private Function __getParser() As TokenParser(Of Tokens)
            Dim _tokens As New Dictionary(Of Tokens, String)()

            ' These lines add each grammar rule to the dictionary
            _tokens.Add(Tokens.[As], "[aA][sS]")
            _tokens.Add(Tokens.From, "[fF][rR][oO][mM]")
            _tokens.Add(Tokens.Where, "[wW][hH][eE][rR][eE]")
            _tokens.Add(Tokens.[Select], "[sS][eE][lL][eE][cC][tT]")
            _tokens.Add(Tokens.[Imports], "^\s*[Ii][mM][pP][oO][rR][tT][sS]")
            _tokens.Add(Tokens.[In], "[iI][nN]\s")
            _tokens.Add(Tokens.[Let], "[Ll][Ee][Tt]")
            _tokens.Add(Tokens.Join, "[Jj][oO][iI][nN]")
            _tokens.Add(Tokens.Group, "[Gg][rR][oO][uU][pP]")
            _tokens.Add(Tokens.By, "[Bb][Yy]")
            _tokens.Add(Tokens.Order, "[Oo][rR][dD][eE][rR]")
            _tokens.Add(Tokens.Into, "[Ii][nN][tT][oO]")
            _tokens.Add(Tokens.[Return], "[Rr][eE][tT][uU][rR][nN]")
            _tokens.Add(Tokens.OpenParens, "\(")
            _tokens.Add(Tokens.CloseParens, "\)")
            _tokens.Add(Tokens.Pretend, "\t[%]\t")
            _tokens.Add(Tokens.Comma, ",")
            _tokens.Add(Tokens.WhiteSpace, "[ \t]+")
            _tokens.Add(Tokens.[String], """.+?""")
            _tokens.Add(Tokens.VarRef, "[\$]\S+")
            _tokens.Add(Tokens.Code, "\S+")

            Return New TokenParser(Of Tokens)(_tokens, Tokens.UNDEFINED)
        End Function

        ' Our Constructor, which simply initializes values
        ''' <summary>
        ''' Default Constructor
        ''' </summary>
        ''' <remarks>
        ''' The constructor initalizes memory and adds all of the tokens to the token dictionary.
        ''' </remarks>
        <Extension> Public Function GetTokens(expr As String) As Token(Of Tokens)()
            Return __getParser.GetTokens(expr)
        End Function

        <Extension>
        Public Function TrimWhiteSpace(source As IEnumerable(Of Token(Of Tokens))) As Token(Of Tokens)()
            Dim LQuery = (From x As Token(Of Tokens)
                          In source
                          Where x.TokenName <> Tokens.WhiteSpace
                          Select x).ToArray
            Return LQuery
        End Function
    End Module
End Namespace


