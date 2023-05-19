Imports Microsoft.VisualBasic.Math.GibbsSampling
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class GibbsScan : Inherits SeedScanner

    Public Sub New(param As PopulatorParameter, debug As Boolean)
        MyBase.New(param, debug)
    End Sub

    Public Overrides Iterator Function GetSeeds(regions() As FastaSeq) As IEnumerable(Of HSP)
        For i As Integer = param.minW To param.maxW
            Dim gibbs As New Gibbs(regions.Select(Function(a) a.SequenceData).ToArray, i)
            Dim find = gibbs.sample

            For Each hit In find
                Yield New HSP
            Next
        Next
    End Function
End Class
