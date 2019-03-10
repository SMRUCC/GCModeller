Namespace Interpreter.LDM.Expressions.Keywords

    Public Class SyntaxError : Inherits LDM.Expressions.PrimaryExpression

        Dim _Ex As String

#Region "If any one of the property in this region is true, that means the line of this code is not a True syntax error, it just can't be parsing as any statement expression in the shoal language."

        ''' <summary>
        ''' 对于空白行，是无法被解析出来的，但是空白行不是语法错误
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsBlankLineSyntax As Boolean
            Get
                Return String.IsNullOrEmpty(MyBase._PrimaryExpression.TrimNewLine)
            End Get
        End Property

        Public ReadOnly Property IsCommentsSyntax As Boolean
            Get
                If IsBlankLineSyntax Then
                    Return False
                End If

                Return Trim(_PrimaryExpression).First = "#"c
            End Get
        End Property
#End Region
        Public ReadOnly Property IsSyntaxError As Boolean
            Get
                Return Not (IsBlankLineSyntax OrElse IsCommentsSyntax)
            End Get
        End Property

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.SyntaxError
            End Get
        End Property

        Sub New(ex As String, Expression As String)
            Call MyBase.New(Expression)
            _Ex = If(IsBlankLineSyntax OrElse IsCommentsSyntax, Expression, ex)
        End Sub

        Public Shared Function BlankCode() As SyntaxError
            Return New SyntaxError("", "")
        End Function

        Public Overrides Function ToString() As String
            If IsSyntaxError Then
                Return $"{_Ex}  at line: {LineNumber}"
            Else
                If Me.IsBlankLineSyntax Then
                    Return $"[{NameOf(BlankCode)}]"
                Else
                    Return $"[{NameOf(Comments)}]  {Comments}"
                End If
            End If
        End Function
    End Class
End Namespace