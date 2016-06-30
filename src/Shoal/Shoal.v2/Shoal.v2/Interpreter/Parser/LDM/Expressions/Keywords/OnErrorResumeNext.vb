Imports System.ComponentModel

Namespace Interpreter.LDM.Expressions.Keywords

    ''' <summary>
    ''' On Error Resume Next.(脚本程序在遇到错误之后忽略掉错误尝试继续执行下去)
    ''' </summary>
    ''' 
    <Description("On Error Resume Next")>
    Public Class OnErrorResumeNext : Inherits PrimaryExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.OnErrorResumeNext
            End Get
        End Property

        Public Shared Function IsOnErrorResumeNext(Tokens As Parser.Tokens.Token()) As Boolean

            If Tokens.Length <> 4 Then
                Return False
            End If

            Dim Flag As String() = {"On", "Error", "Resume", "Next"}

            For p As Integer = 0 To Flag.Length - 1
                If Not String.Equals(Tokens(p).GetTokenValue, Flag(p), StringComparison.OrdinalIgnoreCase) Then
                    Return False
                End If
            Next

            Return True
        End Function
    End Class
End Namespace