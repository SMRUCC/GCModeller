Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.Microarray.MultiOmics.MOFA

Public Module DataExtensions

    <Extension>
    Public Function CreateDataView(mat As Matrix, Optional name As String = Nothing) As DataView
        Dim nsamples As Integer = mat.sample_count
        Dim ngenes As Integer = mat.size
        Dim data As New Tensor(nsamples, ngenes)

        For i As Integer = 0 To nsamples - 1
            For d As Integer = 0 To ngenes - 1
                data(i, d) = mat(d)(i)
            Next
        Next

        Return New DataView(If(name, mat.tag), data, mat.sampleID, mat.rownames)
    End Function

    <Extension>
    Public Function CreateExpressionMatrix(t As Tensor, sampleIDs As String(), featureIDs As String(), Optional ref_tag As String = Nothing) As Matrix
        Dim nsamples As Integer = sampleIDs.Length
        Dim ngenes As Integer = featureIDs.Length
        Dim data As DataFrameRow() = New DataFrameRow(ngenes - 1) {}

        For i As Integer = 0 To ngenes - 1
            data(i) = New DataFrameRow With {
                .geneID = featureIDs(i),
                .experiments = New Double(nsamples - 1) {}
            }
        Next

        For i As Integer = 0 To nsamples - 1
            For d As Integer = 0 To ngenes - 1
                data(d).experiments(i) = t(i, d)
            Next
        Next

        Return New Matrix With {
            .expression = data,
            .sampleID = sampleIDs,
            .tag = If(ref_tag.StringEmpty, "MOFA_reconstruct", $"MOFA_reconstruct({ref_tag})")
        }
    End Function
End Module
