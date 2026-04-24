Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.GeneQuantification
Imports SMRUCC.genomics.SequenceModel.SAM.featureCount
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

<Package("gene_quantification")>
Module Quantification

    <ExportAPI("read_featureCounts")>
    <RApiReturn(GetType(featureCounts))>
    Public Function read_featureCounts(file As String) As Object
        Return featureCounts.ReadTable(file).ToArray
    End Function

    <ExportAPI("counts_matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function as_countmatrix(<RRawVectorArgument> counts As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of featureCounts)(counts, env)

        If pull.isError Then
            Return pull.getError
        End If

        Return pull.populates(Of featureCounts)(env).FeatureCountMatrix
    End Function

    ''' <summary>
    ''' Apply DESeq2 Median of Ratios normalization method to the raw counts matrix.
    ''' </summary>
    ''' <param name="counts">The raw counts matrix.</param>
    ''' <returns>The normalized counts matrix.</returns>
    <ExportAPI("deseq2_norm")>
    Public Function deseq2_norm(counts As Matrix) As Matrix
        Return counts.DESeq2Normalize
    End Function

    ''' <summary>
    ''' Apply of the edgeR TMM factor normalization method to the raw counts matrix
    ''' </summary>
    ''' <param name="counts"></param>
    ''' <returns></returns>
    <ExportAPI("edgeR_norm")>
    Public Function edgeR_norm(counts As Matrix, Optional trimFractionM As Double = 0.3, Optional trimFractionA As Double = 0.05) As Matrix
        Return counts.EdgeRTMMNormalize(trimFractionM, trimFractionA)
    End Function

    <ExportAPI("edgeR_tmm")>
    Public Function edgeR_tmm_factors(countData As Matrix, Optional trimFractionM As Double = 0.3, Optional trimFractionA As Double = 0.05) As Object
        Dim tmmFactors = countData.CalcTMMFactors(trimFractionA:=trimFractionA, trimFractionM:=trimFractionM)
        Dim tmm As New list(
            slot("norm_factor") = tmmFactors.normFactors,
            slot("reference_index") = tmmFactors.referenceSampleIndex,
            slot("reference_sample") = countData.sampleID(tmmFactors.referenceSampleIndex)
        )

        Return tmm
    End Function

    <ExportAPI("gene_indexstats")>
    <RApiReturn(GetType(GeneData))>
    Public Function sample_indexstats(file As String) As Object
        Return GeneQuantification.ConvertCountsToTPM(IndexStats.Parse(file.OpenReadonly)).ToArray
    End Function

    <ExportAPI("read_genedata")>
    <RApiReturn(GetType(GeneData))>
    Public Function read_genedata(file As String) As GeneData()
        Return file.LoadCsv(Of GeneData)(mute:=True)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="counts">A collection of the gene <see cref="featureCounts"/> data.</param>
    ''' <param name="env">The R environment.</param>
    ''' <returns>A collection of gene expression data in TPM format.</returns>
    <ExportAPI("convert_to_tpm")>
    <RApiReturn(GetType(GeneSampleSet))>
    Public Function convert_to_tpm(<RRawVectorArgument> counts As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of featureCounts)(counts, env)

        If pull.isError Then
            Return pull.getError
        End If

        Return pull.populates(Of featureCounts)(env).MakeGeneExpressions.ToArray
    End Function

    <ExportAPI("expression_data")>
    <RApiReturn(GetType(Matrix))>
    Public Function expression_data(<RRawVectorArgument> sampledata As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of GeneSampleSet)(sampledata, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim data As GeneSampleSet() = pull.populates(Of GeneSampleSet)(env).ToArray
        Dim tpm As Matrix = data.TPMExpression
        Dim fpkm As Matrix = data.FPKMExpression

        Return New list(slot("tpm") = tpm, slot("fpkm") = fpkm)
    End Function
End Module
