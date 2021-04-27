Imports Microsoft.VisualBasic.My.JavaScript
Imports Matrix = Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.GeneralMatrix

Public Class HMM

    Friend ReadOnly states()
    Friend ReadOnly transMatrix As Double()()
    Friend ReadOnly initialProb As Double()
    Friend ReadOnly observables()
    Friend ReadOnly emissionMatrix As Double()()

    Friend ReadOnly Bayes As Bayes
    Friend ReadOnly Viterbi As Viterbi
    Friend ReadOnly Forward As Forward
    Friend ReadOnly Backward As Backward
    Friend ReadOnly BaumWelch As BaumWelch

    Sub New(states As StatesObject(), observables As Observable(), init As Double())
        Me.states = states.map(Function(s) s.state)
        Me.transMatrix = states.map(Function(s) s.prob)
        Me.initialProb = init
        Me.observables = observables.map(Function(o) o.obs)
        Me.emissionMatrix = observables.map(Function(o) o.prob)

        Me.Bayes = New Bayes(Me)
        Me.Viterbi = New Viterbi(Me)
        Me.Forward = New Forward(Me)
        Me.Backward = New Backward(Me)
        Me.BaumWelch = New BaumWelch(Me)
    End Sub

    Public Function GetTransMatrix() As Matrix
        Return New Matrix(transMatrix)
    End Function

    Public Function bayesTheorem(ob, hState) As Double
        Return Bayes.bayesTheorem(ob, hState)
    End Function

    Public Function forwardAlgorithm(obSequence As Chain) As Alpha
        Return Forward.forwardAlgorithm(obSequence)
    End Function

    Public Function backwardAlgorithm(obSequence As Chain) As Beta
        Return Backward.backwardAlgorithm(obSequence)
    End Function

    Public Function viterbiAlgorithm(obSequence As Chain) As viterbiSequence
        Return Viterbi.viterbiAlgorithm(obSequence)
    End Function

    Public Function baumWelchAlgorithm(obSequence As Chain) As HMM
        Return BaumWelch.baumWelchAlgorithm(obSequence)
    End Function
End Class

Public Class StatesObject

    Public Property state As Object
    Public Property prob As Double()

End Class

Public Class Observable

    Public Property obs As Object
    Public Property prob As Double()

End Class