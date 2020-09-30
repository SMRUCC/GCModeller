
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace AssemblyScript.Script

    ''' <summary>
    ''' 
    ''' </summary>
    Public Enum Tokens
        keyword
        symbol
        assign
        text
    End Enum

    Public Class Token : Inherits CodeToken(Of Tokens)

        Public Sub New(name As Tokens, value$)
            Call MyBase.New(name, value)
        End Sub
    End Class
End Namespace