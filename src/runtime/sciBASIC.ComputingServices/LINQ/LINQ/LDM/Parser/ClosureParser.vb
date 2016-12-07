#Region "Microsoft.VisualBasic::154c4cd6ab03607837a4d7aba7b7c1fe, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Parser\ClosureParser.vb"

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

Imports System.Runtime.CompilerServices
Imports sciBASIC.ComputingServices.Linq.LDM.Statements
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace LDM.Statements

    Public Module ClosureParser

        <Extension>
        Public Function TryParse(source As IEnumerable(Of Token(Of TokenIcer.Tokens))) As ClosureTokens()
            Dim parts As New List(Of ClosureTokens)
            Dim tmp As New List(Of Token(Of TokenIcer.Tokens))
            Dim current As TokenIcer.Tokens
            Dim closure As ClosureTokens

            For Each token As Token(Of TokenIcer.Tokens) In source
                Select Case token.TokenName
                    Case TokenIcer.Tokens.Imports,
                     TokenIcer.Tokens.In,
                     TokenIcer.Tokens.Let,
                     TokenIcer.Tokens.Select,
                     TokenIcer.Tokens.Where

                        closure = New ClosureTokens With {
                            .Token = current,
                            .Tokens = tmp.ToArray
                        }
                        Call parts.Add(closure)
                        Call tmp.Clear()
                        current = token.TokenName
                    Case TokenIcer.Tokens.From
                        current = TokenIcer.Tokens.From
                    Case TokenIcer.Tokens.WhiteSpace
                        ' Do Nothing
                    Case Else
                        Call tmp.Add(token)
                End Select
            Next

            closure = New ClosureTokens With {
                .Token = current,
                .Tokens = tmp.ToArray
            }
            Call parts.Add(closure)

            Return parts.ToArray
        End Function

        Public Function TryParse(st As String) As ClosureTokens()
            Dim source As Token(Of TokenIcer.Tokens)() = TokenIcer.GetTokens(st)
            Return source.TryParse
        End Function
    End Module
End Namespace
