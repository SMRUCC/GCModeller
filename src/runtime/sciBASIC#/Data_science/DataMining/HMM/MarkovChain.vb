Imports Microsoft.VisualBasic.My.JavaScript
Imports Matrix = Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.GeneralMatrix

Public Class MarkovChain

    Friend ReadOnly states As Object()
    Friend ReadOnly transMatrix As Double()()
    Friend ReadOnly initialProb As Double()
    Friend ReadOnly prob As CalculateProb2

    Sub New(states As StatesObject(), init As Double())
        Me.states = states.map(Function(s) s.state)
        Me.transMatrix = states.map(Function(s) s.prob)
        Me.initialProb = init
        Me.prob = New CalculateProb2(transMatrix, init, states)
    End Sub

    Public Function GetTransMatrix() As Matrix
        Return New Matrix(transMatrix)
    End Function

    Public Function sequenceProb(sequence As Chain) As Double
        Return prob.sequenceProb(sequence)
    End Function
End Class

