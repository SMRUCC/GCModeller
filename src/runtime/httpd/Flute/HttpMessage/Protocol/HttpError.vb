Imports Microsoft.VisualBasic.Net.Http
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

        Shared ReadOnly httpRFC As Dictionary(Of String, String)

        Shared Sub New()
            httpRFC = Enums(Of HTTP_RFC)() _
                .Select(Function(a) (a.Description, CLng(a).ToString)) _
                .Where(Function(a)
                           Return Not a.Description.StringEmpty
                       End Function) _
                .ToDictionary(Function(a) a.Item2,
                              Function(a)
                                  Return a.Description
                              End Function)
        End Sub

        Public Shared Function getRFCMessage(code As String) As String
            If httpRFC.ContainsKey(code) Then
                Return httpRFC(code)
            Else
                Return "Unknown Status"
            End If
        End Function

    End Class
End Namespace