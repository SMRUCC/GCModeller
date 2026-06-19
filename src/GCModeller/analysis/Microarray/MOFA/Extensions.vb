Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace MultiOmics.MOFA

    Public Module Extensions

        <Extension>
        Public Function CreateDataView(mat As Matrix) As DataView
            Dim nsamples As Integer = mat.sample_count
            Dim ngenes As Integer = mat.size
            Dim data As New Tensor(nsamples, ngenes)

            For i As Integer = 0 To nsamples - 1
                For d As Integer = 0 To ngenes - 1
                    data(i, d) = mat(d)(i)
                Next
            Next

            Return New DataView(mat.tag, data, mat.sampleID, mat.rownames)
        End Function
    End Module
End Namespace