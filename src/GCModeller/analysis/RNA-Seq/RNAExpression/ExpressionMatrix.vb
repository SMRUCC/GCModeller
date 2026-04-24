Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.SequenceModel.GeneQuantification

Public Module ExpressionMatrix

    <Extension>
    Public Function TPMExpression(dataset As IEnumerable(Of GeneSampleSet)) As Matrix
        Dim sampleData = dataset.ToArray
        Dim sample_ids As String() = sampleData.SelectMany(Function(gene) gene.FPKM.Keys).Distinct().ToArray()
        Dim matrix As New Matrix With {
            .sampleID = sample_ids,
            .tag = "TPM",
            .expression = sampleData _
                .GetGeneExpression(isFpkm:=False, sample_ids:=sample_ids) _
                .ToArray
        }

        Return matrix
    End Function

    <Extension>
    Public Function FPKMExpression(dataset As IEnumerable(Of GeneSampleSet)) As Matrix
        Dim sampleData = dataset.ToArray
        Dim sample_ids As String() = sampleData.SelectMany(Function(gene) gene.FPKM.Keys).Distinct().ToArray()
        Dim matrix As New Matrix With {
            .sampleID = sample_ids,
            .tag = "FPKM",
            .expression = sampleData _
                .GetGeneExpression(isFpkm:=True, sample_ids:=sample_ids) _
                .ToArray
        }

        Return matrix
    End Function

    <Extension>
    Private Iterator Function GetGeneExpression(dataset As IEnumerable(Of GeneSampleSet), isFpkm As Boolean, sample_ids As String()) As IEnumerable(Of DataFrameRow)
        For Each gene As GeneSampleSet In dataset
            Yield New DataFrameRow With {
                .geneID = gene.GeneID,
                .experiments = gene(sample_ids, isFpkm)
            }
        Next
    End Function
End Module
