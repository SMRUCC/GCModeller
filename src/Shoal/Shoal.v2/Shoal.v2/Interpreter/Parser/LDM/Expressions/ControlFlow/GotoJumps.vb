Namespace Interpreter.LDM.Expressions.ControlFlows

    ''' <summary>
    ''' Goto语句
    ''' </summary>
    Public Class [GOTO] : Inherits PrimaryExpression

        'GOTO <FLAG> When <boolean_expression>
        'GOTO 33 When {$boolean}
        'Goto Label1

        ''' <summary>
        ''' When后面的逻辑条件表达式
        ''' </summary>
        ''' <returns></returns>
        Public Property ExprWhen As Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens.InternalExpression
        Public Property JumpsLabel As Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens.InternalExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.GoTo
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public Overrides Function ToString() As String
            If ExprWhen Is Nothing Then
                Return $"GOTO {JumpsLabel}"
            Else
                Return $"GOTO {JumpsLabel} When {ExprWhen.ToString}"
            End If
        End Function
    End Class

    ''' <summary>
    ''' Goto的标签
    ''' </summary>
    Public Class LineLabel : Inherits PrimaryExpression

        Public Property Label As String

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.LineLable
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace