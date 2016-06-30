Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Mathematical.Types
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Script

    Public Class SEquation

        ''' <summary>
        ''' UniqueId，由于可能会存在多个过程，所以这里的值不再唯一
        ''' </summary>
        ''' <returns></returns>
        Public Property x As String
        Public Property Expression As String

        Sub New(id As String, expr As String)
            x = id
            Expression = expr
        End Sub

        Sub New()
        End Sub

        Public Function GetModel(engine As Mathematical.Expression) As SimpleExpression
            Return ExpressionParser.TryParse(Expression, engine)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace