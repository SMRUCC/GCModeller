#Region "Microsoft.VisualBasic::cbaf5b3bf5d78738218c480ca6226ddf, R#\rnaseq\Quantification.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 113
    '    Code Lines: 79 (69.91%)
    ' Comment Lines: 16 (14.16%)
    '    - Xml Docs: 93.75%
    ' 
    '   Blank Lines: 18 (15.93%)
    '     File Size: 4.67 KB


    ' Module Quantification
    ' 
    '     Function: as_countmatrix, convert_to_tpm, deseq2_norm, edgeR_norm, edgeR_tmm_factors
    '               expression_data, read_featureCounts, read_genedata, sample_indexstats
    ' 
    ' /********************************************************************************/

#End Region

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

''' <summary>
''' gene expression quantify tools
''' </summary>
<Package("gene_quantification")>
Module Quantification

    ''' <summary>
    ''' read feature counts tsv table file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read_featureCounts")>
    <RApiReturn(GetType(featureCounts))>
    Public Function read_featureCounts(file As String) As Object
        Return featureCounts.ReadTable(file).ToArray
    End Function

    ''' <summary>
    ''' make assemble of the reads sample counts as a gene expression matrix object
    ''' </summary>
    ''' <param name="counts">feature counts data of multiple samples which is read via ``read_featureCounts`` api function</param>
    ''' <param name="env"></param>
    ''' <returns>the gene expression matrix object</returns>
    <ExportAPI("counts_matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function as_countmatrix(<RRawVectorArgument(GetType(featureCounts))> counts As Object, Optional env As Environment = Nothing) As Object
        Dim pull As PipeIterator(Of featureCounts) = pipeline.Stream(Of featureCounts)(counts, env)

        If pull.isError Then
            Return pull.getError
        End If

        Return pull.FeatureCountMatrix
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

    ''' <summary>
    ''' get edgeR TMM factors
    ''' </summary>
    ''' <param name="countData"></param>
    ''' <param name="trimFractionM"></param>
    ''' <param name="trimFractionA"></param>
    ''' <returns></returns>
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

    ''' <summary>
    ''' read gene data csv table file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read_genedata")>
    <RApiReturn(GetType(GeneData))>
    Public Function read_genedata(file As String) As GeneData()
        Return file.LoadCsv(Of GeneData)(mute:=True)
    End Function

    ''' <summary>
    ''' make normalize of the feature counts data as TPM expression value
    ''' </summary>
    ''' <param name="counts">A collection of the gene <see cref="featureCounts"/> data.</param>
    ''' <param name="env">The R environment.</param>
    ''' <returns>A collection of gene expression data in TPM format.</returns>
    <ExportAPI("convert_to_tpm")>
    <RApiReturn(GetType(GeneSampleSet))>
    Public Function convert_to_tpm(<RRawVectorArgument(GetType(featureCounts))> counts As Object, Optional env As Environment = Nothing) As Object
        Dim pull As PipeIterator(Of featureCounts) = pipeline.Stream(Of featureCounts)(counts, env)

        If pull.isError Then
            Return pull.getError
        End If

        Return pull.MakeGeneExpressions.ToArray
    End Function

    <ExportAPI("expression_data")>
    <RApiReturn(GetType(Matrix))>
    Public Function expression_data(<RRawVectorArgument(GetType(GeneSampleSet))> sampledata As Object, Optional env As Environment = Nothing) As Object
        Dim pull As PipeIterator(Of GeneSampleSet) = pipeline.Stream(Of GeneSampleSet)(sampledata, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim data As GeneSampleSet() = pull.ToArray
        Dim tpm As Matrix = data.TPMExpression
        Dim fpkm As Matrix = data.FPKMExpression

        Return New list(slot("tpm") = tpm, slot("fpkm") = fpkm)
    End Function
End Module

