Imports System.Linq.Expressions
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression

Public Structure Kinetics

    Dim formula As Impl.Expression
    Dim parameters As String()
    Dim paramVals As Object()
    ''' <summary>
    ''' enzyme target
    ''' </summary>
    Dim enzyme As String
    ''' <summary>
    ''' target reaction id
    ''' </summary>
    Dim target As String

    Public Overrides Function ToString() As String
        Return $"[{target}] {formula}"
    End Function

    Public Function CompileLambda() As Func(Of Func(Of String, Double), Double)
        Dim lambda As LambdaExpression = ExpressionCompiler.CreateLambda(parameters, formula)
        Dim handler As [Delegate] = lambda.Compile
        Dim vm = Me

        Return Function(getVal As Func(Of String, Double)) As Double
                   Dim vals = vm.paramVals.ToArray

                   For i As Integer = 0 To vals.Length - 1
                       If TypeOf vals(i) Is String Then
                           vals(i) = getVal(vals(i))
                       End If
                   Next

                   Return handler.DynamicInvoke(vals)
               End Function
    End Function

End Structure
