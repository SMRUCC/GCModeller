Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes
Imports RDotNET.Extensions.VisualBasic

Namespace DESeq2

    ''' <summary>
    ''' Estimate the size factors for a DESeqDataSet
    '''
    ''' This function estimates the size factors using the "median ratio method" described by Equation 5 in Anders and Huber (2010).
    ''' The estimated size factors can be accessed using sizeFactors.
    ''' Alternative library size estimators can also be supplied using sizeFactors.
    ''' </summary>
    ''' <remarks>
    ''' Typically, the function is called with the idiom:
    '''
    ''' dds &lt;- estimateSizeFactors(dds)
    '''
    ''' See DESeq For a description Of the use Of size factors In the GLM.
    ''' One should Call this Function after DESeqDataSet unless size factors are manually specified With sizeFactors.
    ''' Alternatively, gene-specific normalization factors For Each sample can be provided Using normalizationFactors
    ''' which will always preempt sizeFactors In calculations.
    '''
    ''' Internally, the function calls estimateSizeFactorsForMatrix, which provides more details on the calculation.
    ''' </remarks>
    <RFunc(NameOf(estimateSizeFactors))> Public Class estimateSizeFactors : Inherits DESeq2

        ''' <summary>
        ''' a DESeqDataSet
        ''' </summary>
        ''' <returns></returns>
        Public Property [object] As RExpression
        ''' <summary>
        ''' either "ratio" or "iterate". "ratio" uses the standard median ratio method introduced in DESeq.
        ''' The size factor is the median ratio of the sample over a pseudosample:
        ''' for each gene, the geometric mean of all samples. "iterate" offers an alternative estimator,
        ''' which can be used even when all genes contain a sample with a zero. This estimator iterates
        ''' between estimating the dispersion with a design of ~1, and finding a size factor vector by
        ''' numerically optimizing the likelihood of the ~1 model.
        ''' </summary>
        ''' <returns></returns>
        Public Property type As RExpression = c("ratio", "iterate")
        ''' <summary>
        ''' a function to compute a location for a sample. By default, the median is used. However, especially for low counts,
        ''' the shorth function from the genefilter package may give better results.
        ''' </summary>
        ''' <returns></returns>
        Public Property locfunc As RExpression = "stats::median"
        ''' <summary>
        ''' by default this is not provided and the geometric means of the counts are calculated within the function.
        ''' A vector of geometric means from another count matrix can be provided for a "frozen" size factor calculation
        ''' </summary>
        ''' <returns></returns>
        Public Property geoMeans As RExpression
        ''' <summary>
        ''' optional, numeric or logical index vector specifying those genes to use for size
        ''' factor estimation (e.g. housekeeping or spike-in genes)
        ''' </summary>
        ''' <returns></returns>
        Public Property controlGenes As RExpression
        ''' <summary>
        ''' optional, a matrix of normalization factors which do not control for library size
        ''' (e.g. average transcript length of genes for each sample).
        ''' Providing normMatrix will estimate size factors on the count matrix divided by normMatrix and store
        ''' the product of the size factors and normMatrix as normalizationFactors.
        ''' </summary>
        ''' <returns></returns>
        Public Property normMatrix As RExpression
    End Class

    ''' <summary>
    ''' Low-level function to estimate size factors with robust regression.
    '''
    ''' Given a matrix or data frame of count data, this function estimates the size factors as follows:
    ''' Each column is divided by the geometric means of the rows. The median (or, if requested,
    ''' another location estimator) of these ratios (skipping the genes with a geometric mean of zero)
    ''' is used as the size factor for this column. Typically, one will not call this function directly,
    ''' but use estimateSizeFactors.
    ''' </summary>
    ''' <remarks>
    ''' a vector with the estimates size factors, one element per column
    ''' </remarks>
    <RFunc(NameOf(estimateSizeFactorsForMatrix))> Public Class estimateSizeFactorsForMatrix : Inherits DESeq2

        ''' <summary>
        ''' a matrix or data frame of counts, i.e., non-negative integer values
        ''' </summary>
        ''' <returns></returns>
        Public Property counts As Integer
        ''' <summary>
        ''' a function to compute a location for a sample. By default, the median is used.
        ''' However, especially for low counts, the shorth function from genefilter may give better results.
        ''' </summary>
        ''' <returns></returns>
        Public Property locfunc As RExpression = "stats::median"
        ''' <summary>
        ''' by default this is not provided, and the geometric means of the counts are calculated within the function.
        ''' A vector of geometric means from another count matrix can be provided for a "frozen" size factor calculation
        ''' </summary>
        ''' <returns></returns>
        Public Property geoMeans As RExpression
        ''' <summary>
        ''' optional, numeric or logical index vector specifying those genes to use for size factor estimation
        ''' (e.g. housekeeping or spike-in genes)
        ''' </summary>
        ''' <returns></returns>
        Public Property controlGenes As RExpression
    End Class
End Namespace