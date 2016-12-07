#Region "Microsoft.VisualBasic::fcf025553d618e136a75cb212d4ccfc4, ..\sciBASIC.ComputingServices\LINQ\LINQ\Script\Script.vb"

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

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting
Imports sciBASIC.ComputingServices.Linq.Framework.DynamicCode
Imports sciBASIC.ComputingServices.Linq.LDM.Statements
Imports sciBASIC.ComputingServices.Linq.Script.Tokens

Namespace Script

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Script

        Public ReadOnly Property Runtime As DynamicsRuntime
        Public ReadOnly Property Compiler As DynamicCompiler

        Dim __script As New List(Of TokenBase)

        Sub New(script As String, runtime As DynamicsRuntime)
            Dim lines As String() = script.lTokens

            For Each line As String In lines
                Dim tokens = TokenIcer.GetTokens(line.Trim)

                If tokens(Scan0).Type = TokenIcer.Tokens.Imports Then
                    __script += New [Imports](tokens)

                ElseIf tokens(Scan0).Type = TokenIcer.Tokens.Return Then
                    __script += New Returns(tokens)

                Else
                    __script += New Tokens.Linq(tokens, runtime.Types)

                End If
            Next

            Me.Runtime = runtime
            Me.Compiler = runtime.Compiler
        End Sub

        Public Function Evaluate() As IEnumerable

            For Each line As TokenBase In __script

                If TypeOf line Is [Imports] Then
                    For Each ns As String In line.As(Of [Imports])
                        Call Compiler.Imports(ns)
                    Next

                ElseIf TypeOf line Is Returns Then
                    ' Reutrns the function value from here
                    Return Runtime.GetResource(line.As(Of Returns).Ref)

                Else

                    Dim Linq As Tokens.Linq = line.As(Of Tokens.Linq)
                    Dim value As IEnumerable = Runtime.EXEC(Linq)
                    Call Runtime.SetObject(Linq.Name, value)

                End If
            Next

            Return Nothing
        End Function
    End Class
End Namespace
