Namespace Matrix

    Public Class SequenceMatrix : Inherits WeightMatrix

        Private ReadOnly sequences As IList(Of String)
        Private ReadOnly sequenceCount As Integer
        Private ReadOnly sequenceLength As Integer

        Public Sub New(sequences As IList(Of String))
            Me.sequences = sequences
            sequenceCount = sequences.Count
            rowSum = sequenceCount
            sequenceLength = sequences(0).Length
            MyBase.initMatrix(sequenceLength)
            initSequenceMatrix()
        End Sub

        ''' <summary>
        ''' Counts the occurrences of each base along each position of each sequence
        ''' </summary>
        Private Sub initSequenceMatrix()
            Enumerable.Range(0, sequenceCount).ForEach(Sub(i, z)
                                                           Dim sequence = sequences(i)
                                                           Enumerable.Range(0, sequenceLength).ForEach(Sub(j, y)
                                                                                                           Dim b = Utils.indexOfBase(sequence(j))
                                                                                                           countsMatrix(j)(b) += 1
                                                                                                       End Sub)
                                                       End Sub)
        End Sub

        ''' <summary>
        ''' Returns the probability of seeing the base in the index </summary>
        ''' <param name="index">, index of base </param>
        ''' <param name="base">, base in the index </param>
        Public Overridable Function probability(index As Integer, base As Integer) As Double
            Return countsMatrix(index)(base) / rowSum
        End Function
    End Class

End Namespace
