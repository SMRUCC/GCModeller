Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions.HybridScript

    ''' <summary>
    ''' @name
    ''' </summary>
    Public Class RedirectStream : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.RedirectStream
            End Get
        End Property

        Public Property EntryPoint As String

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace