Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions

Namespace Interpreter.Parser.Tokens

    ''' <summary>
    ''' 这个是参数引用之中的内部表达式，只有单行的
    ''' </summary>
    Public Class InternalExpression : Inherits Token

        Public Property Expression As PrimaryExpression

        ''' <summary>
        ''' 是一个表达式，需要进行计算才可以得到值
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsExpr As Boolean
            Get
                Return Len(_TokenValue) > 2 AndAlso Me._TokenValue.First = "{"c AndAlso Me._TokenValue.Last = "}"c
            End Get
        End Property

        ''' <summary>
        ''' 是一个变量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsVariable As Boolean
            Get
                Return _TokenValue.First = "$"c
            End Get
        End Property

        Public ReadOnly Property IsConstant As Boolean
            Get
                Return _TokenValue.First = "&"
            End Get
        End Property

        Public ReadOnly Property IsPrimaryValue As String
            Get
                Return Not (IsExpr OrElse IsVariable OrElse IsConstant)
            End Get
        End Property

        Public Overrides ReadOnly Property TokenType As TokenTypes
            Get
                Return TokenTypes.InternalExpression
            End Get
        End Property

        Sub New(Expr As String)
            Call MyBase.New(0, Expr)

            If IsExpr Then
                Expr = Mid(Expr, 2, Len(Expr) - 2)
                Expression = Interpreter.InternalExpressionParser(Expr)
            Else
                Expression = New DynamicsExpression(Expr)
            End If
        End Sub

        Sub New(Expr As String, Expression As PrimaryExpression)
            Call MyBase.New(0, Expr)
            Me.Expression = Expression
        End Sub

        Sub New(Token As Token)
            Call Me.New(Token.GetTokenValue)
        End Sub

        Public Overrides Function ToString() As String
            If IsExpr Then
                Return $"Inner Expression {NameOf(Expression)}:={Expression}"
            Else
                Return _TokenValue
            End If
        End Function
    End Class
End Namespace