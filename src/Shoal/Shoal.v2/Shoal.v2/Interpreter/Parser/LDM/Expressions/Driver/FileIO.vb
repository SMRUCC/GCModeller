Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions.Driver

    Public Class FileIO : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.FileIO
            End Get
        End Property

        ''' <summary>
        ''' 左端的将要写入文件的表达式
        ''' </summary>
        ''' <returns></returns>
        Public Property Value As InternalExpression
        Public Property Path As InternalExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace