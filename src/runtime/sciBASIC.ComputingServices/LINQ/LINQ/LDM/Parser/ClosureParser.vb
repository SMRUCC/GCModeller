Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq.LDM.Statements
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