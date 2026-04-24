Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.SequenceModel.GeneQuantification
Imports SMRUCC.genomics.SequenceModel.SAM.featureCount

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

    ''' <summary>
    ''' Export the raw feature count matrix data for make data normalizatiomn via DeSeq2 or edgeR. 
    ''' The matrix will be in the format of geneID as row names and sampleID as column names, and the values are the raw counts. 
    ''' This function is useful for users who want to perform their own normalization and differential 
    ''' expression analysis using R packages like DESeq2 or edgeR.
    ''' </summary>
    ''' <param name="featureCounts"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FeatureCountMatrix(featureCounts As IEnumerable(Of featureCounts)) As Matrix
        Dim sampleData = featureCounts.ToArray
        Dim sample_ids As String() = sampleData.SelectMany(Function(gene) gene.SampleCounts.Keys).Distinct().ToArray()
        Dim geneCounts As DataFrameRow() = (From gene As featureCounts
                                            In sampleData
                                            Let counts = gene(sample_ids).AsDouble
                                            Select New DataFrameRow With {
                                                .geneID = gene.Geneid,
                                                .experiments = counts
                                           }).ToArray
        Dim matrix As New Matrix With {
            .sampleID = sample_ids,
            .tag = "RawCounts",
            .expression = geneCounts
        }

        Return matrix
    End Function
End Module
