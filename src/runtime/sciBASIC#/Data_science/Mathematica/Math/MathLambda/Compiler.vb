Imports lambda = Microsoft.VisualBasic.MIME.application.xml.MathML.LambdaExpression
Imports ML = Microsoft.VisualBasic.MIME.application.xml.MathML.BinaryExpression
Imports System.Linq.Expressions
Imports Microsoft.VisualBasic.MIME.application.xml
Imports Microsoft.VisualBasic.Language

''' <summary>
''' mathML -> lambda -> linq expression -> compile VB lambda
''' </summary>
Public Module Compiler

    Public Function CreateLambda(lambda As lambda) As LambdaExpression
        Dim parameters = lambda.parameters.Select(Function(name) Expression.Parameter(GetType(Double), name)).ToArray
        Dim body As Expression = CreateBinary(lambda.lambda)
        Dim expr As LambdaExpression = Expression.Lambda(body, parameters)

        Return expr
    End Function

    Private Function CreateBinary(member As [Variant](Of ML, String)) As Expression
        If member Like GetType(String) Then
            Return Expression.Variable(GetType(Double), member.TryCast(Of String))
        Else
            Return CreateBinary(member.TryCast(Of ML))
        End If
    End Function

    Private Function CreateBinary(member As ML) As Expression
        Select Case MathML.ContentBuilder.SimplyOperator(member.operator)
            Case "+" : Return Expression.Add(CreateBinary(member.applyleft), CreateBinary(member.applyright))
            Case "-" : Return Expression.Subtract(CreateBinary(member.applyleft), CreateBinary(member.applyright))
            Case "*" : Return Expression.Multiply(CreateBinary(member.applyleft), CreateBinary(member.applyright))
            Case "/" : Return Expression.Divide(CreateBinary(member.applyleft), CreateBinary(member.applyright))
            Case "^" : Return Expression.Power(CreateBinary(member.applyleft), CreateBinary(member.applyright))
            Case Else
                Throw New InvalidCastException
        End Select
    End Function
End Module
