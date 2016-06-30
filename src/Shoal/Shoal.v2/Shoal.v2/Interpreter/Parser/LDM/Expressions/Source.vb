Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    ''' <summary>
    ''' var &lt;= ${path argvs}
    ''' 这个语法是函数<see cref="InternalExtension.Source(String, IEnumerable(Of KeyValuePair(Of String, Object)))"/>函数的简写形式
    ''' </summary>
    Public Class Source : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Source
            End Get
        End Property

        Public Property Path As InternalExpression
        Public Property args As KeyValuePair(Of String, InternalExpression)()
        Public Property LeftAssigned As LeftAssignedVariable

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace