
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


        End If
    End Function
End Module
