Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Serialization.JSON
Imports stdNum = System.Math

''' <summary>
''' Performs simple logistic regression.
''' User: tpeng
''' Date: 6/22/12
''' Time: 11:01 PM
''' 
''' @author tpeng
''' @author Matthieu Labas
''' </summary>
Public Class Logistic

    ''' <summary>
    ''' the learning rate 
    ''' </summary>
    Public Property rate As Double = 0.0001
    ''' <summary>
    ''' the number of iterations 
    ''' </summary>
    Public Property ITERATIONS As Integer = 3000

    ''' <summary>
    ''' the weight to learn 
    ''' </summary>
    Friend theta As Vector

    Dim println As Action(Of String)

    Public Sub New(n As Integer, Optional rate As Double = 0.0001, Optional println As Action(Of String) = Nothing)
        Me.rate = rate
        Me.theta = Vector.rand(n) * n
        Me.println = println
    End Sub

    Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Friend Shared Function sigmoid(z As Double) As Double
        Return 1.0 / (1.0 + stdNum.Exp(-z))
    End Function

    Public Function train(instances As IEnumerable(Of Instance)) As LogisticFit
        Dim matrix As Instance() = instances.ToArray
        Dim weights As Double() = Me.theta.Array

        For n As Integer = 0 To ITERATIONS - 1
            Dim lik As Double = 0.0

            For i As Integer = 0 To matrix.Length - 1
                Dim x = matrix(i).x
                Dim predicted = predict(x, weights)
                Dim label = matrix(i).label

                For j As Integer = 0 To weights.Length - 1
                    weights(j) = weights(j) + rate * (label - predicted) * x(j)
                Next

                ' not necessary for learning
                lik += label * stdNum.Log(predict(x, weights)) + (1 - label) * stdNum.Log(1 - predict(x, weights))
            Next

            If Not println Is Nothing Then
                Call println("iteration: " & n & " " & weights.GetJson & " mle: " & lik)
            End If
        Next

        Me.theta = New Vector(weights)

        Return LogisticFit.CreateFit(Me, matrix)
    End Function

    Private Function predict(x As Double(), theta As Double()) As Double
        Dim logit As Double = theta.Select(Function(wi, i) wi * x(i)).Sum
        Dim p = sigmoid(logit)

        Return p
    End Function

    Public Function predict(x As Double()) As Double
        Dim logit As Double = (theta * x).Sum
        Dim p = sigmoid(logit)

        Return p
    End Function
End Class