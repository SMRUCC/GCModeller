Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace base

    <RFunc("paste0")> Public Class paste0 : Inherits IRToken

        <Parameter("...", ValueTypes.List, False, True)>
        Public Property x As RExpression()
        Public Property collapse As RExpression = NULL

        Public Function Func(ParamArray x As String()) As String
            Dim action As paste0 = Me.ShadowsCopy
            action.x = x.ToArray(Function(s) New RExpression(s))
            Return action.RScript
        End Function
    End Class

    <RFunc("paste")> Public Class paste : Inherits paste0

        Public Property sep As String = " "
    End Class
End Namespace