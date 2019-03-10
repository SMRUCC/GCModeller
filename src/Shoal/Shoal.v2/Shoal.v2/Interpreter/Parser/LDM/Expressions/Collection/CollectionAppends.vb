Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    ''' <summary>
    ''' 向集合之中添加一个元素
    ''' 
    ''' [$experiments] &lt;= $exper  ' 添加至末尾;
    ''' [$experiments[index]] &lt;= $exper  ' 给指定元素赋值
    ''' 元素位置可用的位置表达式:  ~First, ~Last
    ''' 元素位置可用的条件表达式: [Where &lt;Element Bool Expression>]
    ''' </summary>
    Public Class CollectionAppends : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.CollectionElementAssigned
            End Get
        End Property

        Public Property Collection As DynamicsExpression
        Public Property ElementIndex As DynamicsExpression
        Public Property Value As LDM.Expressions.PrimaryExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace