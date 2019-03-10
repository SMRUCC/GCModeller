Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions.HybridScript

    Public Class HybridsScript : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.HybridsScript
            End Get
        End Property

        Public Property LeftAssignedVariable As LeftAssignedVariable
        ''' <summary>
        ''' 计算得到脚本值，再由引擎计算值之后赋值给本脚本之中的变量
        ''' </summary>
        ''' <returns></returns>
        Public Property ExternalScript As InternalExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace