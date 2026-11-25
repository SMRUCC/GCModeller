Imports System.Linq

Namespace MathML

    ''' <summary>
    ''' A bounded mathematical operator such as sum, product, integral or limit.
    ''' </summary>
    Public Class BoundedOperatorExpression : Inherits MathExpression

        Public Property [operator] As String
        Public Property variable As String
        Public Property lower As MathExpression
        Public Property upper As MathExpression
        Public Property degree As MathExpression
        Public Property body As MathExpression

        Public Overrides Function ToString() As String
            Dim range As String = If(lower Is Nothing, "", lower.ToString)

            If upper IsNot Nothing Then
                range &= ".." & upper.ToString
            End If

            Dim op As String = Me.[operator]
            Return $"{op}[{variable}={range}]({body})"
        End Function

    End Class

    ''' <summary>
    ''' A matrix expression parsed from Content MathML matrix/matrixrow elements.
    ''' </summary>
    Public Class MatrixExpression : Inherits MathExpression

        Public Property rows As MathExpression()()

        Public Overrides Function ToString() As String
            If rows Is Nothing Then
                Return "[]"
            End If

            Dim texts As String() = rows _
                .Select(Function(row) String.Join(", ", row.Select(Function(cell) cell.ToString))) _
                .ToArray

            Return "[" & String.Join("; ", texts) & "]"
        End Function

    End Class
End Namespace
