Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract

Public Class SequenceMotif : Inherits Probability

    Public Property seeds As MSAOutput
    Public Property length As Integer

    Public ReadOnly Property AverageScore As Double
        Get
            Return score / seeds.MSA.Length
        End Get
    End Property
End Class