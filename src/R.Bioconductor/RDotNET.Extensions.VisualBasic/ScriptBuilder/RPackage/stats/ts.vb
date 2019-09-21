#Region "Microsoft.VisualBasic::19c8aa8060855c9309ef444add0443ba, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\stats\ts.vb"

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

    '     Class ts
    ' 
    '         Properties: [class], [end], data, deltat, frequency
    '                     names, start, ts_eps
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
