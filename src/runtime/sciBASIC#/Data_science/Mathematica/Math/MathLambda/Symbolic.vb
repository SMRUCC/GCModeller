
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression.Impl

''' <summary>
''' symbolic computation engine
''' </summary>
Public Module Symbolic

    Public Function Simplify(raw As Expression) As Expression
        If Not TypeOf raw Is BinaryExpression Then
            Return raw
        Else
            Dim bin As BinaryExpression = raw
            Dim left = Simplify(bin.left)
            Dim right = Simplify(bin.right)

            If TypeOf left Is Literal AndAlso TypeOf right Is Literal Then
                Return Literal.Evaluate(left, bin.operator, right)
            ElseIf TypeOf left Is Literal Then
                Return New BinaryExpression(left, right, bin.operator)
            ElseIf TypeOf right Is Literal Then
                If bin.operator = "+"c OrElse bin.operator = "*"c Then
                    ' 加法与乘法可以交换位置
                    Return New BinaryExpression(right, left, bin.operator)
                ElseIf bin.operator = "-"c Then
                    Return New BinaryExpression(DirectCast(right, Literal).GetNegative, left, bin.operator)
                ElseIf bin.operator = "/"c Then
                    Return New BinaryExpression(DirectCast(right, Literal).GetReciprocal, left, bin.operator)
                Else
                    Return New BinaryExpression(left, right, bin.operator)
                End If
            Else
                raw = New BinaryExpression(left, right, bin.operator)
            End If

            ' 都是binary表达式
            ' 并且都已经归一化为左边为常数，右边为变量
            Dim a As BinaryExpression = left
            Dim b As BinaryExpression = right

            If Not (a.isNormalized AndAlso b.isNormalized) Then
                Return raw
            ElseIf DirectCast(a.right, SymbolExpression).symbolName <> DirectCast(b.right, SymbolExpression).symbolName Then
                Return raw
            End If

            Dim symbol As New SymbolExpression(DirectCast(a.right, SymbolExpression).symbolName)

            Select Case bin.operator
                Case "+"c
                    Dim aplusb As Literal = Literal.Evaluate(a.left, "+", b.left)
                    Dim result As New BinaryExpression(aplusb, symbol, "*")
                    Return result
                Case "-"c
                    Dim aminusb As Literal = Literal.Evaluate(a.left, "-", b.left)
                    Dim result As New BinaryExpression(aminusb, symbol, "*")
                    Return result
                Case "*"c
                    Dim atimesb As Literal = Literal.Evaluate(a.left, "*", b.left)
                    Dim result As New BinaryExpression(atimesb, New BinaryExpression(symbol, New Literal(2), "^"), "*")
                    Return result
                Case "/"c
                    Dim adivb As Literal = Literal.Evaluate(a.left, "/", b.left)
                    Return adivb
                Case "%"c
                    Return raw
                Case "^"c
                    Return raw
                Case Else
                    Throw New NotImplementedException(bin.ToString)
            End Select
        End If
    End Function

    <Extension>
    Private Function isNormalized(exp As BinaryExpression) As Boolean
        Return TypeOf exp.left Is Literal AndAlso TypeOf exp.right Is SymbolExpression
    End Function
End Module
