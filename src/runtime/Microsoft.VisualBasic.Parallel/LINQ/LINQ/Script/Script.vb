Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode
Imports Microsoft.VisualBasic.Linq.LDM.Statements
Imports Microsoft.VisualBasic.Linq.Script.Tokens
Imports Microsoft.VisualBasic.Scripting

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