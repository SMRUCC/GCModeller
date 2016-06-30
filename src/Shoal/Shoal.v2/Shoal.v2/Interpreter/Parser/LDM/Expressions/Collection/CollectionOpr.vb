Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    ''' <summary>
    ''' Dim array &lt;= {expression} as type 
    ''' </summary>
    Public Class CollectionOpr : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.CollectionOpr
            End Get
        End Property

        Public Property DeclareNew As Boolean
        Public Property InitLeft As LeftAssignedVariable
        Public Property Array As InternalExpression()
        Public Property Type As String

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace