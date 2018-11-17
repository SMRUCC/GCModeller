Imports System.Runtime.CompilerServices

Public Module Extensions

    <Extension> Public Function EvalEffects(regMode As String) As Double
        If regMode.StringEmpty Then
            Return 0.25
        End If

        If regMode.TextEquals("repressor") Then
            Return -1
        ElseIf regMode.TextEquals("activator") Then
            Return 1
        Else
            Return 0.25
        End If
    End Function
End Module
