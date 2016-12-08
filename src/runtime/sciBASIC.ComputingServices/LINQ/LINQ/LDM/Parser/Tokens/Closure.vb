#Region "Microsoft.VisualBasic::4e3b7be2db2f43b95d57f09037e28ddb, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Parser\Tokens\Closure.vb"

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

Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace LDM.Statements.Tokens

    Public MustInherit Class Closure

        Protected _statement As LinqStatement
        Public ReadOnly Property Source As ClosureTokens

        Sub New(type As TokenIcer.Tokens, tokens As ClosureTokens(), parent As LinqStatement)
            _Source = GetTokens(type, from:=tokens)
            _statement = parent

            If _Source Is Nothing Then
                If type = TokenIcer.Tokens.From OrElse
                    type = TokenIcer.Tokens.In OrElse
                    type = TokenIcer.Tokens.Select Then
                    ' LET and SELECT is optional
                    ' But From, In and Select is required
                    ' If missing, then syntax error, throw exception
                    Dim msg As String = String.Format(MissingRequiredField, type)
                    Throw New SyntaxErrorException(msg)
                End If
            End If
        End Sub

        Sub New(token As ClosureTokens, parent As LinqStatement)
            _Source = token
            _statement = parent
        End Sub

        Sub New(type As TokenIcer.Tokens, expr As IEnumerable(Of Token(Of TokenIcer.Tokens)))
            _Source = New ClosureTokens With {
                .Token = type,
                .Tokens = expr.ToArray
            }
            _statement = Nothing
        End Sub

        Const MissingRequiredField As String = "Missing the required LINQ statement token {0}!"

        Public Overrides Function ToString() As String
            Return _Source.ToString
        End Function

        Public Shared Function GetTokens(type As TokenIcer.Tokens, from As ClosureTokens()) As ClosureTokens
            Dim LQuery = (From x As ClosureTokens In from Where x.Token = type Select x).FirstOrDefault
            Return LQuery
        End Function
    End Class
End Namespace
