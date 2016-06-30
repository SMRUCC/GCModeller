Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes
Imports RDotNET.Extensions.VisualBasic

Namespace WGCNA

    ''' <summary>
    ''' Iterative filtering of samples and genes with too many missing entries
    ''' </summary>
    <RFunc("goodSamplesGenes")>
    Public Class goodSamplesGenes : Inherits WGCNAFunction

        ''' <summary>
        ''' This function checks data for missing entries and zero-variance genes, and returns a list of samples and genes that pass criteria maximum number of missing values. 
        ''' If necessary, the filtering is iterated.
        ''' </summary>
        ''' <param name="datExpr"></param>
        ''' <param name="verbose"></param>
        ''' <returns></returns>
        <Ignored> Default Public ReadOnly Property Func(datExpr As String,
                                                        Optional minFraction As Double = 1 / 2,
                                                        Optional minNSamples As String = "..minNSamples",
                                                        Optional minNGenes As String = "..minNGenes",
                                                        Optional tol As String = NULL,
                                                        Optional verbose As Integer = 1,
                                                        Optional indent As Integer = 0) As RExpression
            Get
                Dim x As goodSamplesGenes = Me.ShadowsCopy

                x.datExpr = datExpr
                x.minFraction = __assertion(minFraction, 1 / 2, Me.minFraction)
                x.minNSamples = __assertion(minNSamples, "..minNSamples", Me.minNSamples.RScript)
                x.minNGenes = __assertion(minNGenes, "..minNGenes", Me.minNGenes.RScript)
                x.tol = __assertion(tol, NULL, Me.tol.RScript)
                x.verbose = __assertion(verbose, 1, Me.verbose)
                x.indent = __assertion(indent, 0, Me.indent)

                Return x.RScript
            End Get
        End Property

        ''' <summary>
        ''' expression data. A data frame in which columns are genes and rows ar samples.
        ''' </summary>
        ''' <returns></returns>
        Public Property datExpr As RExpression
        ''' <summary>
        ''' minimum fraction of non-missing samples for a gene to be considered good.
        ''' </summary>
        ''' <returns></returns>
        Public Property minFraction As Double = 1 / 2
        ''' <summary>
        ''' minimum number of non-missing samples for a gene to be considered good.
        ''' </summary>
        ''' <returns></returns>
        Public Property minNSamples As RExpression = "..minNSamples"
        ''' <summary>
        ''' minimum number of good genes for the data set to be considered fit for analysis. 
        ''' If the actual number of good genes falls below this threshold, an error will be issued.
        ''' </summary>
        ''' <returns></returns>
        Public Property minNGenes As RExpression = "..minNGenes"
        ''' <summary>
        ''' an optional 'small' number to compare the variance against. Defaults to the square of 1e-10 * max(abs(datExpr), na.rm = TRUE). 
        ''' The reason of comparing the variance to this number, rather than zero, is that the fast way of computing variance used by this function sometimes causes small numerical overflow errors which make variance of constant vectors slightly non-zero; comparing the variance to tol rather than zero prevents the retaining of such genes as 'good genes'.
        ''' </summary>
        ''' <returns></returns>
        Public Property tol As RExpression = NULL
        ''' <summary>
        ''' integer level of verbosity. Zero means silent, higher values make the output progressively more and more verbose.
        ''' </summary>
        ''' <returns></returns>
        Public Property verbose As Integer = 1
        ''' <summary>
        ''' indentation for diagnostic messages. Zero means no indentation, each unit adds two spaces.
        ''' </summary>
        ''' <returns></returns>
        Public Property indent As Integer = 0
    End Class
End Namespace