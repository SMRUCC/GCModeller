Imports SMRUCC.genomics.Analysis.SequenceTools.MSA

Public Class SequenceMotif : Inherits Probability

    Public Property seeds As MSAOutput

    ''' <summary>
    ''' the length of the MSA alignment
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property width As Integer
        Get
            Return seeds.MSA(Scan0).Length
        End Get
    End Property

    Public ReadOnly Property AverageScore As Double
        Get
            Return score / seeds.MSA.Length
        End Get
    End Property
End Class