Imports System.CodeDom
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.LINQ.Framework.DynamicCode
Imports Microsoft.VisualBasic.LINQ.Framework.DynamicCode.VBC

Namespace LDM.Expression

    Public Class SelectClosure : Inherits Closure

        Friend SelectMethod As System.Reflection.MethodInfo

        Sub New(source As Statements.Tokens.SelectClosure)
            Call MyBase.New(source)


        End Sub

        Protected Overrides Function __parsing() As CodeExpression

        End Function
    End Class
End Namespace