#Region "Microsoft.VisualBasic::e7799a4b4a2b34faadf56721b5f94005, ..\R.Bioconductor\Bioconductor\Bioconductor\biocPkgs\DESeq2\plotCounts.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

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
