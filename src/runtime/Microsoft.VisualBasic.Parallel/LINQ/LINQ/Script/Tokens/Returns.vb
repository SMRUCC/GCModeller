Imports Microsoft.VisualBasic.Linq.LDM.Statements
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Script.Tokens

    Public Class Returns : Inherits TokenBase

        Public ReadOnly Property Ref As String

        Sub New(source As IEnumerable(Of Token(Of TokenIcer.Tokens)))
            Call MyBase.New(source)

            For Each x In source.Skip(1)
                If Not x.Type = TokenIcer.Tokens.WhiteSpace Then
                    Ref = x.Text
                    Exit For
                End If
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return "Return " & Ref
        End Function
    End Class
End Namespace