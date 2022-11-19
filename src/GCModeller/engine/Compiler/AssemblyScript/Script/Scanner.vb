#Region "Microsoft.VisualBasic::0d9d3818b7d81b7019bc25719894a84a, GCModeller\engine\Compiler\AssemblyScript\Script\Scanner.vb"

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

    '   Total Lines: 117
    '    Code Lines: 95
    ' Comment Lines: 4
    '   Blank Lines: 18
    '     File Size: 3.89 KB


    '     Class Scanner
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetTokens, populateToken, populateToken2, walkChar
    '         Class Escaping
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser

Namespace AssemblyScript.Script

    ''' <summary>
    ''' assembly script token scanner
    ''' </summary>
    Public Class Scanner

        ReadOnly script As CharPtr

        Dim buf As New CharBuffer
        Dim escapes As New Escaping

        Private Class Escaping
            Public comment As Boolean
            Public [string] As Boolean
        End Class

        Sub New(scriptText As String)
            script = scriptText
        End Sub

        Public Iterator Function GetTokens() As IEnumerable(Of Token)
            Dim token As New Value(Of Token)

            Do While script
                If Not (token = walkChar(++script)) Is Nothing Then
                    Yield token.Value
                End If

                If buf = "," OrElse buf = "=" OrElse buf = ":" Then
                    Yield populateToken()
                End If
            Loop
        End Function

        Private Function walkChar(c As Char) As Token
            If escapes.comment Then
                If c = ASCII.CR OrElse c = ASCII.LF Then
                    ' 行结束表示注释结束
                    escapes.comment = False
                    Return New Token(Tokens.comment, New String(buf.PopAllChars))
                Else
                    buf += c
                    Return Nothing
                End If
            ElseIf escapes.string Then
                buf += c

                If c = ASCII.Quot Then
                    escapes.string = False
                    Return New Token(Tokens.text, New String(buf.PopAllChars))
                Else
                    Return Nothing
                End If
            End If

            If c = " "c OrElse c = ASCII.TAB OrElse c = ASCII.CR OrElse c = ASCII.LF Then
                If buf = "--" Then
                    escapes.comment = True
                    Return Nothing
                Else
                    Return populateToken()
                End If
            ElseIf c = ","c OrElse c = "="c OrElse c = ":"c Then
                Return populateToken(c)
            ElseIf c = ASCII.Quot Then
                escapes.string = True
                Return populateToken(""""c)
            Else
                buf += c
            End If

            Return Nothing
        End Function

        Private Function populateToken(Optional cacheNext As Char? = Nothing) As Token
            Dim token As String = New String(buf.PopAllChars)

            If Not cacheNext Is Nothing Then
                buf += CChar(cacheNext)
            End If

            If token = "" Then
                Return Nothing
            Else
                Return populateToken2(token)
            End If
        End Function

        Private Function populateToken2(token As String) As Token
            Static keywords As Index(Of String) = {
                "FROM", "MAINTAINER", "KEYWORDS", "LABEL", "ENV", "ADD", "DELETE"
            }

            If token.ToUpper Like keywords Then
                Return New Token(Tokens.keyword, token)
            ElseIf token = "," Then
                Return New Token(Tokens.comma, token)
            ElseIf token = "=" Then
                Return New Token(Tokens.assign, token)
            ElseIf token = ":" Then
                Return New Token(Tokens.reference, token)
            ElseIf token.IsPattern("[a-zA-Z][:-_.a-zA-Z0-9]*") Then
                Return New Token(Tokens.symbol, token)
            ElseIf PrimitiveParser.IsInteger(token) OrElse token.IsNumeric Then
                Return New Token(Tokens.number, token)
            Else
                Throw New SyntaxErrorException($"unknown token '{token}'")
            End If
        End Function
    End Class
End Namespace
