Namespace Interpreter.LDM.Expressions.Keywords

    ''' <summary>
    ''' Dynamics install a external module in the runtime.
    ''' </summary>
    Public Class Library : Inherits LDM.Expressions.PrimaryExpression

        Public Property Assembly As String

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Library
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public Overrides Function ToString() As String
            Return $"{NameOf(Library)} ==> {Assembly.ToFileURL}"
        End Function
    End Class
End Namespace