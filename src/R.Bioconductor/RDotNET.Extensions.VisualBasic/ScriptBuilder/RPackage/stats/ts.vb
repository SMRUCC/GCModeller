Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.stats

    <RFunc("ts")> Public Class ts : Inherits IRToken

        ''' <summary>
        ''' a vector Or matrix Of the observed time-series values. A data frame will be coerced To a numeric matrix via data.matrix. (See also 'Details’.)
        ''' </summary>
        ''' <returns></returns>
        Public Property data As RExpression

        ''' <summary>
        ''' the time Of the first observation. Either a Single number Or a vector Of two integers, which specify a natural time unit And a (1-based) number Of samples into the time unit. See the examples For the use Of the second form.
        ''' </summary>
        ''' <returns></returns>
        Public Property start As RExpression

        ''' <summary>
        ''' the time Of the last observation, specified In the same way As start.
        ''' </summary>
        ''' <returns></returns>
        Public Property [end] As RExpression
        ''' <summary>
        ''' the number Of observations per unit Of time.
        ''' </summary>
        ''' <returns></returns>
        Public Property frequency As Integer
        ''' <summary>
        ''' the fraction Of the sampling period between successive observations; e.g., 1/12 For monthly data. Only one Of frequency Or deltat should be provided.
        ''' </summary>
        ''' <returns></returns>
        Public Property deltat As RExpression

        ''' <summary>
        ''' time series comparison tolerance. Frequencies are considered equal If their absolute difference Is less than ts.eps.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("ts.eps")> Public Property ts_eps As RExpression

        ''' <summary>
        ''' Class to be given To the result, Or none If NULL Or "none". The Default Is "ts" For a Single series, c(Of"mts", "ts", "matrix") for multiple series.
        ''' </summary>
        ''' <returns></returns>
        Public Property [class] As RExpression

        ''' <summary>
        ''' a character vector Of names For the series In a multiple series: defaults to the colnames of data, Or Series 1, Series 2, ....
        ''' </summary>
        ''' <returns></returns>
        Public Property names As RExpression

    End Class
End Namespace