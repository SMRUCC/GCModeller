#Region "Microsoft.VisualBasic::6ad3a65182c4d92ae5b0804ae62f3b33, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\Graphics\gplots\heatmap2.vb"

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

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.stats

Namespace SymbolBuilder.packages.gplots

    ''' <summary>
    ''' A heat map is a false color image (basically image(t(x))) with a dendrogram added to the left side and/or to the top. Typically, reordering of the rows and columns according to some set of values (row or column means) within the restrictions imposed by the dendrogram is carried out.
    ''' This heatmap provides a number Of extensions To the standard R heatmap Function.
    ''' </summary>
    <RFunc("heatmap.2")> Public Class heatmap2 : Inherits heatmap_plot

        Sub New()
            Requires = {"gplots"}
        End Sub

        ' # dendrogram control

        ''' <summary>
        ''' determines if and how the row dendrogram should be reordered.	By default, it is TRUE, which implies dendrogram is computed and reordered based on row means. If NULL or FALSE, then no dendrogram is computed and no reordering is done. If a dendrogram, then it is used "as-is", ie without any reordering. If a vector of integers, then dendrogram is computed and reordered based on the order of the vector.
        ''' </summary>
        ''' <returns></returns>
        Public Property Rowv As Boolean = True
        ''' <summary>
        ''' determines if and how the column dendrogram should be reordered.	Has the options as the Rowv argument above and additionally when x is a square matrix, Colv="Rowv" means that columns should be treated identically to the rows.
        ''' </summary>
        ''' <returns></returns>
        Public Property Colv As RExpression = "if(symm)""Rowv"" else TRUE"

        ''' <summary>
        ''' character string indicating whether to draw 'none', 'row', 'column' or 'both' dendrograms. Defaults to 'both'. However, if Rowv (or Colv) is FALSE or NULL and dendrogram is 'both', then a warning is issued and Rowv (or Colv) arguments are honoured.
        ''' </summary>
        ''' <returns></returns>
        Public Property dendrogram As String = "both"

        ' # data scaling

        ' # image plot

        ' # mapping data to colors
        ''' <summary>
        ''' (optional) Either a numeric vector indicating the splitting points for binning x into colors, or a integer number of break points to be used, in which case the break points will be spaced equally between min(x) and max(x).
        ''' </summary>
        ''' <returns></returns>
        Public Property breaks As RExpression
        ''' <summary>
        ''' Boolean indicating whether breaks should be made symmetric about 0. Defaults to TRUE if the data includes negative values, and to FALSE otherwise.
        ''' </summary>
        ''' <returns></returns>
        Public Property symbreaks As RExpression = [TRUE] ' "any(x < 0, na.rm = TRUE) || scale!=""none"""

        ' # colors
        ''' <summary>
        ''' colors used for the image. Defaults to heat colors (heat.colors).
        ''' </summary>
        ''' <returns></returns>
        Public Property col As RExpression = "heat.colors"

        ' # block sepration
        ''' <summary>
        ''' (optional) vector of integers indicating which columns or rows should be separated from the preceding columns or rows by a narrow space of color sepcolor.
        ''' </summary>
        ''' <returns></returns>
        Public Property colsep As RExpression
        ''' <summary>
        ''' (optional) vector of integers indicating which columns or rows should be separated from the preceding columns or rows by a narrow space of color sepcolor.
        ''' </summary>
        ''' <returns></returns>
        Public Property rowsep As RExpression
        ''' <summary>
        ''' (optional) vector of integers indicating which columns or rows should be separated from the preceding columns or rows by a narrow space of color sepcolor.
        ''' </summary>
        ''' <returns></returns>
        Public Property sepcolor As String = "white"
        ''' <summary>
        ''' (optional) Vector of length 2 giving the width (colsep) or height (rowsep) the separator box drawn by colsep and rowsep as a function of the width (colsep) or height (rowsep) of a cell. Defaults to c(0.05, 0.05)
        ''' </summary>
        ''' <returns></returns>
        Public Property sepwidth As RExpression = c(0.05, 0.05)

        ' # cell labeling
        ''' <summary>
        ''' (optional) matrix of character strings which will be placed within each color cell, e.g. p-value symbols.
        ''' </summary>
        ''' <returns></returns>
        Public Property cellnote As RExpression
        ''' <summary>
        ''' (optional) numeric scaling factor for cellnote items.
        ''' </summary>
        ''' <returns></returns>
        Public Property notecex As Double = 1.0
        ''' <summary>
        ''' (optional) character string specifying the color for cellnote text. Defaults to "cyan".
        ''' </summary>
        ''' <returns></returns>
        Public Property notecol As String = "cyan"
        ''' <summary>
        ''' Color to use for missing value (NA). Defaults to the plot background color.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("na.color")> Public Property naColor As RExpression = par("bg")

        ' # level trace
        ''' <summary>
        ''' character string indicating whether a solid "trace" line should be drawn across 'row's or down 'column's, 'both' or 'none'. The distance of the line from the center of each color-cell is proportional to the size of the measurement. Defaults to 'column'.
        ''' </summary>
        ''' <returns></returns>
        Public Property trace As RExpression = c("column", "row", "both", "none")
        ''' <summary>
        ''' character string giving the color for "trace" line. Defaults to "cyan".
        ''' </summary>
        ''' <returns></returns>
        Public Property tracecol As String = "cyan"
        ''' <summary>
        ''' Vector of values within cells where a horizontal or vertical dotted line should be drawn. The color of the line is controlled by linecol. Horizontal lines are only plotted if trace is 'row' or 'both'. Vertical lines are only drawn if trace 'column' or 'both'. hline and vline default to the median of the breaks, linecol defaults to the value of tracecol.
        ''' </summary>
        ''' <returns></returns>
        Public Property hline As RExpression = median("breaks")
        ''' <summary>
        ''' Vector of values within cells where a horizontal or vertical dotted line should be drawn. The color of the line is controlled by linecol. Horizontal lines are only plotted if trace is 'row' or 'both'. Vertical lines are only drawn if trace 'column' or 'both'. hline and vline default to the median of the breaks, linecol defaults to the value of tracecol.
        ''' </summary>
        ''' <returns></returns>
        Public Property vline As RExpression = median("breaks")
        ''' <summary>
        ''' Vector of values within cells where a horizontal or vertical dotted line should be drawn. The color of the line is controlled by linecol. Horizontal lines are only plotted if trace is 'row' or 'both'. Vertical lines are only drawn if trace 'column' or 'both'. hline and vline default to the median of the breaks, linecol defaults to the value of tracecol.
        ''' </summary>
        ''' <returns></returns>
        Public Property linecol As RExpression = "tracecol"

        ' # Row/Column Labeling

        ''' <summary>
        '''	angle of row/column labels, in degrees from horizontal
        ''' </summary>
        ''' <returns></returns>
        Public Property srtRow As RExpression = NULL
        ''' <summary>
        ''' angle of row/column labels, in degrees from horizontal
        ''' </summary>
        ''' <returns></returns>
        Public Property srtCol As RExpression = NULL
        ''' <summary>
        ''' 2-element vector giving the (left-right, top-bottom) justification of row/column labels (relative to the text orientation).
        ''' </summary>
        ''' <returns></returns>
        Public Property adjRow As RExpression = c(New RExpression(0), NA)
        ''' <summary>
        ''' 2-element vector giving the (left-right, top-bottom) justification of row/column labels (relative to the text orientation).
        ''' </summary>
        ''' <returns></returns>
        Public Property adjCol As RExpression = c(NA, New RExpression(0))
        ''' <summary>
        ''' Number of character-width spaces to place between row/column labels and the edge of the plotting region.
        ''' </summary>
        ''' <returns></returns>
        Public Property offsetRow As Double = 0.5
        ''' <summary>
        ''' Number of character-width spaces to place between row/column labels and the edge of the plotting region.
        ''' </summary>
        ''' <returns></returns>
        Public Property offsetCol As Double = 0.5
        ''' <summary>
        ''' color of row/column labels, either a scalar to set the color of all labels the same, or a vector providing the colors of each label item
        ''' </summary>
        ''' <returns></returns>
        Public Property colRow As RExpression = NULL
        ''' <summary>
        ''' color of row/column labels, either a scalar to set the color of all labels the same, or a vector providing the colors of each label item
        ''' </summary>
        ''' <returns></returns>
        Public Property colCol As RExpression = NULL

        ' # color key + density info
        ''' <summary>
        ''' logical indicating whether a color-key should be shown.
        ''' </summary>
        ''' <returns></returns>
        Public Property key As Boolean = True
        ''' <summary>
        ''' numeric value indicating the size of the key
        ''' </summary>
        ''' <returns></returns>
        Public Property keysize As RExpression = 1.5
        ''' <summary>
        ''' character string indicating whether to superimpose a 'histogram', a 'density' plot, or no plot ('none') on the color-key.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("density.info")> Public Property densityInfo As RExpression = c("histogram", "density", "none")
        ''' <summary>
        ''' character string giving the color for the density display specified by density.info, defaults to the same value as tracecol.
        ''' </summary>
        ''' <returns></returns>
        Public Property denscol As RExpression = "tracecol"
        ''' <summary>
        ''' Boolean indicating whether the color key should be made symmetric about 0. Defaults to TRUE if the data includes negative values, and to FALSE otherwise.
        ''' </summary>
        ''' <returns></returns>
        Public Property symkey As RExpression = [TRUE]
        ''' <summary>
        ''' Numeric scaling value for tuning the kernel width when a density plot is drawn on the color key. (See the adjust parameter for the density function for details.) Defaults to 0.25.
        ''' </summary>
        ''' <returns></returns>
        Public Property densadj As Double = 0.25
        ''' <summary>
        ''' main title of the color key. If set to NA no title will be plotted.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("key.title")> Public Property keyTitle As RExpression = NULL
        ''' <summary>
        ''' x axis label of the color key. If set to NA no label will be plotted.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("key.xlab")> Public Property keyxlab As RExpression = NULL
        ''' <summary>
        ''' y axis label of the color key. If set to NA no label will be plotted.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("key.ylab")> Public Property keyylab As RExpression = NULL
        ''' <summary>
        ''' function computing tick location and labels for the xaxis of the color key. Returns a named list containing parameters that can be passed to axis. See examples.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("key.xtickfun")> Public Property keyxtickfun As RExpression = NULL
        ''' <summary>
        ''' function computing tick location and labels for the y axis of the color key. Returns a named list containing parameters that can be passed to axis. See examples.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("key.ytickfun")> Public Property keyytickfun As RExpression = NULL
        ''' <summary>
        ''' graphical parameters for the color key. Named list that can be passed to par.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("key.par")> Public Property keyPar As RExpression = "list()"

        ' # plot labels

        ' # plot layout
        ''' <summary>
        ''' visual layout: position matrix, column height, column width. See below for details
        ''' </summary>
        ''' <returns></returns>
        Public Property lmat As RExpression = NULL
        ''' <summary>
        ''' visual layout: position matrix, column height, column width. See below for details
        ''' </summary>
        ''' <returns></returns>
        Public Property lhei As RExpression = NULL
        ''' <summary>
        ''' visual layout: position matrix, column height, column width. See below for details
        ''' </summary>
        ''' <returns></returns>
        Public Property lwid As RExpression = NULL

        ' # extras
        ''' <summary>
        ''' A function to be called after all other work. See examples.
        ''' </summary>
        ''' <returns></returns>
        Public Property extrafun As RExpression = NULL

        Public Shared Function Puriney() As heatmap2
            Return New heatmap2 With {
                .Rowv = True,
                .Colv = [TRUE],
                .col = "rev(brewer.pal(10,""RdYlBu""))",
                .revC = [TRUE],
                .scale = "row",
                .margins = c(15, 15),
                .key = True,
                .densityInfo = Rstring("none"),
                .trace = Rstring("none"),
                .cexCol = "2",
                .cexRow = "2",
                .symkey = [FALSE],
                .srtCol = 45,
 _
 _
                .addExpr = Nothing,
                .adjCol = Nothing,
                .adjRow = Nothing,
                .breaks = Nothing,
                .cellnote = Nothing,
                .colCol = Nothing,
                .colRow = Nothing,
                .colsep = Nothing,
                .ColSideColors = Nothing,
                .dendrogram = Nothing,
                .densadj = Nothing,
                .denscol = Nothing,
                .distfun = Nothing,
                .extrafun = Nothing,
                .hclustfun = Nothing,
                .hline = Nothing,
                .keepDendro = Nothing,
                .keyPar = Nothing,
                .keyTitle = Nothing,
                .keyxlab = Nothing,
                .keyxtickfun = Nothing,
                .keyylab = Nothing,
                .keyytickfun = Nothing,
                .labCol = Nothing,
                .labRow = Nothing,
                .lhei = Nothing,
                .linecol = Nothing,
                .lmat = Nothing,
                .lwid = Nothing,
                .main = Nothing,
                .naColor = Nothing,
                .naRM = Nothing,
                .notecex = Nothing,
                .notecol = Nothing,
                .offsetCol = Nothing,
                .offsetRow = Nothing,
                .reorderfun = Nothing,
                .rowsep = Nothing,
                .RowSideColors = Nothing,
                .sepcolor = Nothing,
                .sepwidth = Nothing,
                .srtRow = Nothing,
                .symbreaks = Nothing,
                .symm = Nothing,
                .tracecol = Nothing,
                .verbose = Nothing,
                .vline = Nothing,
                .x = Nothing,
                .xlab = Nothing,
                .ylab = Nothing
            }
        End Function
    End Class
End Namespace
