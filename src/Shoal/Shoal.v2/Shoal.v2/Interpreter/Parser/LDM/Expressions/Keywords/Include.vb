Namespace Interpreter.LDM.Expressions.Keywords

    ''' <summary>
    ''' Include file1, file2, file3, file4, ...
    ''' </summary>
    Public Class Include : Inherits LDM.Expressions.PrimaryExpression

        ''' <summary>
        ''' The file path list of the external script
        ''' </summary>
        ''' <returns></returns>
        Public Property ExternalScripts As String()

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Include
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace