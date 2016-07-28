Imports Microsoft.VisualBasic.Linq.Framework.Provider
Imports Microsoft.VisualBasic.Linq.Framework.Provider.ImportsAPI

Namespace LDM.Expression

    Public MustInherit Class Closure

        Protected _expression As CodeDom.CodeExpression
        Protected _source As Statements.Tokens.Closure

        Sub New(source As Statements.Tokens.Closure)
            _source = source
            Call __init()
        End Sub

        Protected MustOverride Function __parsing() As CodeDom.CodeExpression

        Protected Sub __init()
            _expression = __parsing()
        End Sub

        Public Overrides Function ToString() As String
            Return _source.ToString
        End Function
    End Class

    Public Interface ICompiler
        Sub CompileToken(types As TypeRegistry, api As APIProvider)
    End Interface
End Namespace