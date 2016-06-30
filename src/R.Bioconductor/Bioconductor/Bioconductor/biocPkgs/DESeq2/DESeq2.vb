Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace DESeq2

    <RFunc(NameOf(DESeq2))>
    Public MustInherit Class DESeq2 : Inherits IRToken

        Sub New()
            MyBase.Requires = {NameOf(DESeq2)}
        End Sub
    End Class

    ''' <summary>
    ''' Constructs a simulated dataset of Negative Binomial data from two conditions.
    ''' By default, there are no fold changes between the two conditions, but this can be adjusted with the betaSD argument.
    ''' </summary>
    ''' <remarks>
    ''' a DESeqDataSet with true dispersion, intercept and beta values in the metadata columns.
    ''' Note that the true betas are provided on the log2 scale.
    ''' </remarks>
    <RFunc(NameOf(makeExampleDESeqDataSet))> Public Class makeExampleDESeqDataSet : Inherits DESeq2
        ''' <summary>
        ''' number of rows
        ''' </summary>
        ''' <returns></returns>
        Public Property n As Integer = 1000
        ''' <summary>
        ''' number of columns
        ''' </summary>
        ''' <returns></returns>
        Public Property m As Integer = 12
        ''' <summary>
        ''' the standard deviation for non-intercept betas, i.e. beta ~ N(0,betaSD)
        ''' </summary>
        ''' <returns></returns>
        Public Property betaSD As Integer = 0
        ''' <summary>
        ''' the mean of the intercept betas (log2 scale)
        ''' </summary>
        ''' <returns></returns>
        Public Property interceptMean As Integer = 4
        ''' <summary>
        ''' the standard deviation of the intercept betas (log2 scale)
        ''' </summary>
        ''' <returns></returns>
        Public Property interceptSD As Integer = 2
        ''' <summary>
        ''' a function specifying the relationship of the dispersions on 2^trueIntercept
        ''' </summary>
        ''' <returns></returns>
        Public Property dispMeanRel As RExpression = "function(x) 4/x + 0.1"
        ''' <summary>
        ''' multiplicative factors for each sample
        ''' </summary>
        ''' <returns></returns>
        Public Property sizeFactors As RExpression = "rep(1, m)"
    End Class
End Namespace