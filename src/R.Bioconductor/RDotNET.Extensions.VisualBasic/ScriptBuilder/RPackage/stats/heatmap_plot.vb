#Region "Microsoft.VisualBasic::673f12a65142fb6d6ae7f197185ac8e4, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\stats\heatmap_plot.vb"

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

    '     Class heatmap_plot
    ' 
    '         Properties: addExpr, cexCol, cexRow, ColSideColors, distfun
    '                     hclustfun, keepDendro, labCol, labRow, main
    '                     margins, naRM, reorderfun, revC, RowSideColors
    '                     scale, symm, verbose, x, xlab
    '                     ylab
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.stats

    Public MustInherit Class heatmap_plot : Inherits IRToken

        ''' <summary>
        ''' numeric matrix of the values to be plotted.
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression

        ''' <summary>
        ''' function used to compute the distance (dissimilarity) between both rows and columns. Defaults to dist.
        ''' </summary>
        ''' <returns></returns>
        Public Property distfun As RExpression = "dist"
        ''' <summary>
        ''' function used to compute the hierarchical clustering when Rowv or Colv are not dendrograms. Defaults to hclust. Should take as argument a result of distfun and return an object to which as.dendrogram can be applied.
        ''' </summary>
        ''' <returns></returns>
        Public Property hclustfun As RExpression = "hclust"
        ''' <summary>
        ''' function(d, w) of dendrogram and weights for reordering the row and column dendrograms. The default uses reorder.dendrogram.
        ''' </summary>
        ''' <returns></returns>
        Public Property reorderfun As RExpression = "function(d, w) reorder(d, w)"
        ''' <summary>
        ''' expression that will be evaluated after the call to image. Can be used to add components to the plot.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("add.expr")> Public Property addExpr As RExpression
        ''' <summary>
        ''' logical indicating if x should be treated symmetrically; can only be true when x is a square matrix.
        ''' </summary>
        ''' <returns></returns>
        Public Property symm As Boolean = False
        ''' <summary>
        ''' logical indicating if the column order should be reversed for plotting, such that e.g., for the symmetric case, the symmetry axis is as usual.
        ''' </summary>
        ''' <returns></returns>
        Public Property revC As RExpression = "identical(Colv, ""Rowv"")"
        ''' <summary>
        ''' character indicating if the values should be centered and scaled in either the row direction or the column direction, or none. The default is "row" if symm false, and "none" otherwise.
        ''' </summary>
        ''' <returns></returns>
        Public Property scale As String = "column"
        ''' <summary>
        ''' logical indicating whether NA's should be removed.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("na.rm")> Public Property naRM As Boolean = True
        ''' <summary>
        ''' numeric vector of length 2 containing the margins (see par(mar = *)) for column and row names, respectively.
        ''' </summary>
        ''' <returns></returns>
        Public Property margins As RExpression = c(5, 5)
        ''' <summary>
        ''' (optional) character vector of length ncol(x) containing the color names for a horizontal side bar that may be used to annotate the columns of x.
        ''' </summary>
        ''' <returns></returns>
        Public Property ColSideColors As RExpression
        ''' <summary>
        ''' (optional) character vector of length nrow(x) containing the color names for a vertical side bar that may be used to annotate the rows of x.
        ''' </summary>
        ''' <returns></returns>
        Public Property RowSideColors As RExpression
        ''' <summary>
        ''' positive numbers, used as cex.axis in for the row or column axis labeling. The defaults currently only use number of rows or columns, respectively.
        ''' </summary>
        ''' <returns></returns>
        Public Property cexRow As RExpression = "0.2 + 1 / log10(nr)"
        ''' <summary>
        ''' positive numbers, used as cex.axis in for the row or column axis labeling. The defaults currently only use number of rows or columns, respectively.
        ''' </summary>
        ''' <returns></returns>
        Public Property cexCol As RExpression = "0.2 + 1 / log10(nc)"
        ''' <summary>
        ''' character vectors with row and column labels to use; these default to rownames(x) or colnames(x), respectively.
        ''' </summary>
        ''' <returns></returns>
        Public Property labRow As RExpression = NULL
        ''' <summary>
        ''' character vectors with row and column labels to use; these default to rownames(x) or colnames(x), respectively.
        ''' </summary>
        ''' <returns></returns>
        Public Property labCol As RExpression = NULL
        ''' <summary>
        ''' main, x- and y-axis titles; defaults to none.
        ''' </summary>
        ''' <returns></returns>
        Public Property main As RExpression = NULL
        ''' <summary>
        ''' main, x- and y-axis titles; defaults to none.
        ''' </summary>
        ''' <returns></returns>
        Public Property xlab As RExpression = NULL
        ''' <summary>
        ''' main, x- and y-axis titles; defaults to none.
        ''' </summary>
        ''' <returns></returns>
        Public Property ylab As RExpression = NULL
        ''' <summary>
        ''' logical indicating if the dendrogram(s) should be kept as part of the result (when Rowv and/or Colv are not NA).
        ''' </summary>
        ''' <returns></returns>
        <Parameter("keep.dendro")> Public Property keepDendro As RExpression = [FALSE]
        ''' <summary>
        ''' logical indicating if information should be printed.
        ''' </summary>
        ''' <returns></returns>
        Public Property verbose As RExpression = getOption("verbose")
    End Class
End Namespace
