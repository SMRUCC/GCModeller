
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace AssemblyScript.Script

    ''' <summary>
    ''' 
    ''' </summary>
    Public Enum Tokens
        ''' <summary>
        ''' -- comment
        ''' </summary>
        comment
        ''' <summary>
        ''' word
        ''' </summary>
        keyword
        ''' <summary>
        ''' x
        ''' </summary>
        symbol
        ''' <summary>
        ''' =
        ''' </summary>
        assign
        ''' <summary>
        ''' "..."
        ''' </summary>
        text
        ''' <summary>
        ''' ,
        ''' </summary>
        comma
        ''' <summary>
        ''' ::
        ''' </summary>
        reference
        ''' <summary>
        ''' \d
        ''' </summary>
        number
    End Enum

    Public Class Token : Inherits CodeToken(Of Tokens)

        Public Sub New(name As Tokens, value$)
            Call MyBase.New(name, value)
        End Sub
    End Class
End Namespace