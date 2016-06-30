Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace DESeq2

    ''' <summary>
    ''' Plot of normalized counts for a single gene on log scale
    ''' Note: normalized counts plus a pseudocount of 0.5 are shown.
    ''' </summary>
    <RFunc(NameOf(plotCounts))> Public Class plotCounts : Inherits DESeq2

        ''' <summary>
        ''' a DESeqDataSet
        ''' </summary>
        ''' <returns></returns>
        Public Property dds As RExpression
        ''' <summary>
        ''' a character, specifying the name Of the gene To plot
        ''' </summary>
        ''' <returns></returns>
        Public Property gene As RExpression
        ''' <summary>
        ''' interesting groups: a character vector of names in colData(x) to use for grouping
        ''' </summary>
        ''' <returns></returns>
        Public Property intgroup As String = "condition"
        ''' <summary>
        ''' whether the counts should be normalized by size factor (default is TRUE)
        ''' </summary>
        ''' <returns></returns>
        Public Property normalized As Boolean = True
        ''' <summary>
        ''' whether to present log2 counts (TRUE) or to present the counts on the log scale (FALSE, default)
        ''' </summary>
        ''' <returns></returns>
        Public Property transform As Boolean = False
        ''' <summary>
        ''' as in 'plot'
        ''' </summary>
        ''' <returns></returns>
        Public Property main As String
        ''' <summary>
        ''' as in 'plot'
        ''' </summary>
        ''' <returns></returns>
        Public Property xlab As String = "group"
        ''' <summary>
        ''' should the function only return the data.frame of counts and covariates for custom plotting (default is FALSE)
        ''' </summary>
        ''' <returns></returns>
        Public Property returnData As Boolean = False
        ''' <summary>
        ''' use the outlier-replaced counts if they exist
        ''' </summary>
        ''' <returns></returns>
        Public Property replaced As Boolean = False
    End Class
End Namespace