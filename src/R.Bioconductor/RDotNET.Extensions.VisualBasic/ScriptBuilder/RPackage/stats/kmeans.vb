#Region "Microsoft.VisualBasic::5419ed5a84829f50159b437375823fa2, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\stats\kmeans.vb"

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

    '     Class kmeans
    ' 
    '         Properties: algorithm, centers, iterMax, nstart, trace
    '                     x
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.stats

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
