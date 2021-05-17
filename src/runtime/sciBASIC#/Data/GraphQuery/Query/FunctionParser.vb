Imports Microsoft.VisualBasic.MIME.Markup.HTML

Public Class FunctionParser : Inherits Parser

    Protected Overrides Function ParseImpl(document As InnerPlantText, isArray As Boolean, env As Engine) As InnerPlantText
        Return env.Execute(document, func, parameters, isArray)
    End Function
End Class
