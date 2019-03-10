Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    Public MustInherit Class PrimaryExpression : Inherits MachineElement

        ''' <summary>
        ''' <see cref="ExpressionTypes.BlankLine"/>, <see cref="ExpressionTypes.Comments"/>, 
        ''' <see cref="ExpressionTypes.SyntaxError"/>, <see cref="ExpressionTypes.LineLable"/>.
        ''' (这个表达式是否为非执行代码)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsNonExecuteCode As Boolean
            Get
                Return ExprTypeID = ExpressionTypes.BlankLine OrElse
                    ExprTypeID = ExpressionTypes.Comments OrElse
                    ExprTypeID = ExpressionTypes.SyntaxError OrElse
                    ExprTypeID = ExpressionTypes.LineLable
            End Get
        End Property

        ''' <summary>
        ''' 表达式的类型编号
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride ReadOnly Property ExprTypeID As ExpressionTypes
        ''' <summary>
        ''' 本句代码在脚本之中的原始的行数
        ''' </summary>
        ''' <returns></returns>
        Public Property LineNumber As Integer
        ''' <summary>
        ''' 语句后面所出现的注释信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Comments As String '语句后面所出现的注释信息

        Protected _PrimaryExpression As String

        ''' <summary>
        ''' 原始的表达式字符串
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property PrimaryExpression As String
            Get
                Return _PrimaryExpression
            End Get
        End Property

        Sub New(Expression As String)
            _PrimaryExpression = Expression
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{ExprTypeID.ToString}]  {_PrimaryExpression}"
        End Function

        Public Overrides Function ExceptionExpr() As String
            Return ToString()
        End Function

        Public Overridable Iterator Function GetTokens() As IEnumerable(Of Token)
        End Function
    End Class
End Namespace