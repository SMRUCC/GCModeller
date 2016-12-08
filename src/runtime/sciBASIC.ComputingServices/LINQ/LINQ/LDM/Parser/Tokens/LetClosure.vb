#Region "Microsoft.VisualBasic::a10af2c0e0d1a667b3d13593433b59cb, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Parser\Tokens\LetClosure.vb"

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

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports sciBASIC.ComputingServices.Linq.Framework.DynamicCode
Imports sciBASIC.ComputingServices.Linq.Framework.Provider

Namespace LDM.Statements.Tokens

    ''' <summary>
    ''' Object declared using a LET expression.(使用Let语句所声明的只读对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class LetClosure : Inherits Closure
        Implements ICodeProvider

        ''' <summary>
        ''' Variable name
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        ''' <summary>
        ''' Optional
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As String

        Public ReadOnly Property Code As String Implements ICodeProvider.Code

        ''' <summary>
        ''' Let var = expression
        ''' </summary>
        ''' <remarks></remarks>
        Sub New(token As ClosureTokens, parent As LinqStatement, Optional types As TypeRegistry = Nothing)
            Call MyBase.New(token, parent)

            Name = Source.Tokens.First.TokenValue

            If __isEquals(Source.Tokens(1)) Then
                Code = Source.Tokens.ToArray(Function(x) x.TokenValue).JoinBy(" ")
            Else
                Dim sk As Integer

                If Source.Tokens(1).TokenName = TokenIcer.Tokens.As Then
                    Type = Source.Tokens(2).TokenValue
                    sk = 4

                    If Not types Is Nothing Then
                        Dim t = types.Find(Type)
                        If Not t Is Nothing Then
                            Type = t.GetType.FullName
                        End If
                    Else
                        Type = Scripting.GetType(Type).FullName
                    End If
                Else
                    Throw New SyntaxErrorException
                End If

                Dim expr = Source.Tokens.Skip(sk)
                Code = expr.ToArray(Function(x) x.TokenValue).JoinBy(" ")
                Code = $"{Name} As {Type} = {Code}"
            End If
        End Sub

        Private Shared Function __isEquals(t As Token(Of TokenIcer.Tokens)) As Boolean
            If t.TokenName = TokenIcer.Tokens.Code AndAlso String.Equals(t.TokenValue, "=") Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Overrides Function ToString() As String
            Return Source.ToString
        End Function
    End Class

    Public Module Parser

        Public Function GetPreDeclare(tokens As ClosureTokens(), parent As LinqStatement, Optional types As TypeRegistry = Nothing) As LetClosure()
            Dim i As Integer = 2
            Dim current As ClosureTokens = Nothing
            Dim list As New List(Of ClosureTokens)

            Do While tokens.Read(i, current).Token = TokenIcer.Tokens.Let
                Call list.Add(current)
                If i = tokens.Length Then
                    Exit Do
                End If
            Loop

            Dim value = list.ToArray(Function(x) New LetClosure(x, parent, types))
            Return value
        End Function

        Public Function GetAfterDeclare(tokens As ClosureTokens(), parent As LinqStatement, Optional types As TypeRegistry = Nothing) As LetClosure()
            Dim i As Integer = 2
            Dim current As ClosureTokens = Nothing
            Dim list As New List(Of ClosureTokens)

            Do While tokens.Read(i, current).Token <> TokenIcer.Tokens.Where
                If i = tokens.Length Then
                    Exit Do
                End If
            Loop
            Do While tokens.Read(i, current).Token = TokenIcer.Tokens.Let
                Call list.Add(current)
                If i = tokens.Length Then
                    Exit Do
                End If
            Loop

            Dim value = list.ToArray(Function(x) New LetClosure(x, parent, types))
            Return value
        End Function
    End Module
End Namespace
