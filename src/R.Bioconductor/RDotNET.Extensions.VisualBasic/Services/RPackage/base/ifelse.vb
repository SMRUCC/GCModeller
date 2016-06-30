Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace base

    <RFunc("ifelse")> Public Class ifelse : Inherits IRToken
        Public Property test As RExpression
        Public Property yes As RExpression
        Public Property no As RExpression

        Sub New(test As String, yes As String, no As String)
            Me.yes = yes
            Me.no = no
            Me.test = test
        End Sub
    End Class
End Namespace