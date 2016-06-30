Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions.ControlFlows

    Public Class ForLoop : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.ForLoop
            End Get
        End Property

        Public Property LoopVariable As String
        Public Property Collection As ForLoopStatus
        Public Property Invoke As LDM.SyntaxModel

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class

    Public MustInherit Class ForLoopStatus : Inherits Parser.Tokens.Token

        Public Enum LoopTypes
            ForEach
            ForStep
        End Enum

        Public MustOverride ReadOnly Property LoopType As LoopTypes

        Sub New(Expression As String)
            Call MyBase.New(1, Expression)
        End Sub

        ''' <summary>
        ''' 请先去掉两个大括号
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Public Shared Function StatusParser(Expression As String) As ForLoopStatus
            Dim Tokens = New Parser.TextTokenliser.MSLTokens().Parsing(Expression).Tokens

            If Tokens.Length = 3 Then
                If Not String.Equals(Tokens(1).GetTokenValue, "to", StringComparison.OrdinalIgnoreCase) Then
                    GoTo FOR_EACH
                End If

                Dim initStart = New InternalExpression(Tokens(0))
                Dim loopStop = New InternalExpression(Tokens(2))
                Dim moveStep = New InternalExpression("1")

                Return New ForStep(Expression) With {
                    .InitStart = initStart,
                    .LoopStop = loopStop,
                    .MoveStep = moveStep
                }
            ElseIf Tokens.Length = 5 Then

                If Not (String.Equals(Tokens(1).GetTokenValue, "to", StringComparison.OrdinalIgnoreCase) AndAlso
                    String.Equals(Tokens(3).GetTokenValue, "step", StringComparison.OrdinalIgnoreCase)) Then
                    GoTo FOR_EACH
                End If

                Dim initStart = New InternalExpression(Tokens(0))
                Dim loopStop = New InternalExpression(Tokens(2))
                Dim moveStep = New InternalExpression(Tokens(4))

                Return New ForStep(Expression) With {
                    .InitStart = initStart,
                    .LoopStop = loopStop,
                    .MoveStep = moveStep
                }

            Else
FOR_EACH:       Return New ForEach(Expression) With {.ToArray = SyntaxParser.MakeCollection(Tokens)}
            End If
        End Function

        ''' <summary>
        ''' For vat in {a, b, c, d}
        ''' </summary>
        Public Class ForEach : Inherits ForLoopStatus

            Public Overrides ReadOnly Property LoopType As LoopTypes
                Get
                    Return LoopTypes.ForEach
                End Get
            End Property

            Sub New(Expression As String)
                Call MyBase.New(Expression)
            End Sub

            Public Property ToArray As Parser.Tokens.InternalExpression()
        End Class

        ''' <summary>
        ''' For var in {a to b [step n]}
        ''' 默认Step 1
        ''' </summary>
        Public Class ForStep : Inherits ForLoopStatus

            Public Overrides ReadOnly Property LoopType As LoopTypes
                Get
                    Return LoopTypes.ForStep
                End Get
            End Property

            Public Property InitStart As InternalExpression
            Public Property LoopStop As InternalExpression
            Public Property MoveStep As InternalExpression

            Sub New(Expression As String)
                Call MyBase.New(Expression)
            End Sub
        End Class
    End Class
End Namespace