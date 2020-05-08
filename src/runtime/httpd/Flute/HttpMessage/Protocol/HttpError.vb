Imports Microsoft.VisualBasic.Scripting.SymbolBuilder

Namespace Core.Message.HttpHeader

    Public Class HttpError

        ReadOnly template As String

        Sub New(template As String)
            Me.template = template
        End Sub

        Sub New()
            Call Me.New("{$message}")
        End Sub

        Public Function GetErrorPage(message As String) As String
            With New ScriptBuilder(template)
                !message = message

                Return .ToString
            End With
        End Function
    End Class
End Namespace