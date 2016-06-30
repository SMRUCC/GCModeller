Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace stats

    ''' <summary>
    ''' Perform k-means clustering on a data matrix.
    ''' </summary>
    <RFunc("kmeans")> Public Class kmeans : Inherits IRToken
        ''' <summary>
        ''' numeric matrix Of data, Or an Object that can be coerced To such a matrix (such As a numeric vector Or a data frame With all numeric columns).
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        ''' <summary>
        ''' either the number of clusters, say k, or a set of initial (distinct) cluster centres. If a number, a random set of (distinct) rows in x is chosen as the initial centres.
        ''' </summary>
        ''' <returns></returns>
        Public Property centers As Integer
        ''' <summary>
        ''' the maximum number of iterations allowed.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("iter.max")> Public Property iterMax As Integer = 10
        ''' <summary>
        ''' if centers is a number, how many random sets should be chosen?
        ''' </summary>
        ''' <returns></returns>
        Public Property nstart As Integer = 1
        ''' <summary>
        ''' character: may be abbreviated. Note that "Lloyd" and "Forgy" are alternative names for one algorithm.
        ''' </summary>
        ''' <returns></returns>
        Public Property algorithm As RExpression = c("Hartigan-Wong", "Lloyd", "Forgy", "MacQueen")
        ''' <summary>
        ''' logical Or integer number, currently only used in the default method ("Hartigan-Wong") If positive(Or True), tracing information On the progress Of the algorithm Is produced. Higher values may produce more tracing information.
        ''' </summary>
        ''' <returns></returns>
        Public Property trace As Boolean = False
    End Class
End Namespace