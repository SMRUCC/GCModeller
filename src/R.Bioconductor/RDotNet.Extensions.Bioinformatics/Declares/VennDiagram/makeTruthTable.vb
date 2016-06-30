Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace VennDiagram

    ''' <summary>
    ''' Makes a truth table of the inputs.
    ''' 
    ''' A data frame with length(x) logical vector columns and 2 ^ length(x) rows.
    ''' </summary>
    <RFunc("make.truth.table")> Public Class makeTruthTable : Inherits vennBase

        ''' <summary>
        ''' A short vector.
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
    End Class
End Namespace