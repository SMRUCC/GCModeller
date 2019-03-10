Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    ''' <summary>
    ''' Throw Exception
    ''' </summary>
    Public Class Die : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Die
            End Get
        End Property

        Public Property ExceptionMessage As String
        ''' <summary>
        ''' Boolean Expression
        ''' </summary>
        ''' <returns></returns>
        Public Property [When] As InternalExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace