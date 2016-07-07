---
title: heatmap2
---

# heatmap2
_namespace: [RDotNET.Extensions.VisualBasic.gplots](N-RDotNET.Extensions.VisualBasic.gplots.html)_

A heat map is a false color image (basically image(t(x))) with a dendrogram added to the left side and/or to the top. Typically, reordering of the rows and columns according to some set of values (row or column means) within the restrictions imposed by the dendrogram is carried out.
 This heatmap provides a number Of extensions To the standard R heatmap Function.




### Properties

#### adjCol
2-element vector giving the (left-right, top-bottom) justification of row/column labels (relative to the text orientation).
#### adjRow
2-element vector giving the (left-right, top-bottom) justification of row/column labels (relative to the text orientation).
#### breaks
(optional) Either a numeric vector indicating the splitting points for binning x into colors, or a integer number of break points to be used, in which case the break points will be spaced equally between min(x) and max(x).
#### cellnote
(optional) matrix of character strings which will be placed within each color cell, e.g. p-value symbols.
#### col
colors used for the image. Defaults to heat colors (heat.colors).
#### colCol
color of row/column labels, either a scalar to set the color of all labels the same, or a vector providing the colors of each label item
#### colRow
color of row/column labels, either a scalar to set the color of all labels the same, or a vector providing the colors of each label item
#### colsep
(optional) vector of integers indicating which columns or rows should be separated from the preceding columns or rows by a narrow space of color sepcolor.
#### Colv
determines if and how the column dendrogram should be reordered.Has the options as the Rowv argument above and additionally when x is a square matrix, Colv="Rowv" means that columns should be treated identically to the rows.
#### dendrogram
character string indicating whether to draw 'none', 'row', 'column' or 'both' dendrograms. Defaults to 'both'. However, if Rowv (or Colv) is FALSE or NULL and dendrogram is 'both', then a warning is issued and Rowv (or Colv) arguments are honoured.
#### densadj
Numeric scaling value for tuning the kernel width when a density plot is drawn on the color key. (See the adjust parameter for the density function for details.) Defaults to 0.25.
#### denscol
character string giving the color for the density display specified by density.info, defaults to the same value as tracecol.
#### densityInfo
character string indicating whether to superimpose a 'histogram', a 'density' plot, or no plot ('none') on the color-key.
#### extrafun
A function to be called after all other work. See examples.
#### hline
Vector of values within cells where a horizontal or vertical dotted line should be drawn. The color of the line is controlled by linecol. Horizontal lines are only plotted if trace is 'row' or 'both'. Vertical lines are only drawn if trace 'column' or 'both'. hline and vline default to the median of the breaks, linecol defaults to the value of tracecol.
#### key
logical indicating whether a color-key should be shown.
#### keyPar
graphical parameters for the color key. Named list that can be passed to par.
#### keysize
numeric value indicating the size of the key
#### keyTitle
main title of the color key. If set to NA no title will be plotted.
#### keyxlab
x axis label of the color key. If set to NA no label will be plotted.
#### keyxtickfun
function computing tick location and labels for the xaxis of the color key. Returns a named list containing parameters that can be passed to axis. See examples.
#### keyylab
y axis label of the color key. If set to NA no label will be plotted.
#### keyytickfun
function computing tick location and labels for the y axis of the color key. Returns a named list containing parameters that can be passed to axis. See examples.
#### lhei
visual layout: position matrix, column height, column width. See below for details
#### linecol
Vector of values within cells where a horizontal or vertical dotted line should be drawn. The color of the line is controlled by linecol. Horizontal lines are only plotted if trace is 'row' or 'both'. Vertical lines are only drawn if trace 'column' or 'both'. hline and vline default to the median of the breaks, linecol defaults to the value of tracecol.
#### lmat
visual layout: position matrix, column height, column width. See below for details
#### lwid
visual layout: position matrix, column height, column width. See below for details
#### naColor
Color to use for missing value (NA). Defaults to the plot background color.
#### notecex
(optional) numeric scaling factor for cellnote items.
#### notecol
(optional) character string specifying the color for cellnote text. Defaults to "cyan".
#### offsetCol
Number of character-width spaces to place between row/column labels and the edge of the plotting region.
#### offsetRow
Number of character-width spaces to place between row/column labels and the edge of the plotting region.
#### rowsep
(optional) vector of integers indicating which columns or rows should be separated from the preceding columns or rows by a narrow space of color sepcolor.
#### Rowv
determines if and how the row dendrogram should be reordered.By default, it is TRUE, which implies dendrogram is computed and reordered based on row means. If NULL or FALSE, then no dendrogram is computed and no reordering is done. If a dendrogram, then it is used "as-is", ie without any reordering. If a vector of integers, then dendrogram is computed and reordered based on the order of the vector.
#### sepcolor
(optional) vector of integers indicating which columns or rows should be separated from the preceding columns or rows by a narrow space of color sepcolor.
#### sepwidth
(optional) Vector of length 2 giving the width (colsep) or height (rowsep) the separator box drawn by colsep and rowsep as a function of the width (colsep) or height (rowsep) of a cell. Defaults to c(0.05, 0.05)
#### srtCol
angle of row/column labels, in degrees from horizontal
#### srtRow
angle of row/column labels, in degrees from horizontal
#### symbreaks
Boolean indicating whether breaks should be made symmetric about 0. Defaults to TRUE if the data includes negative values, and to FALSE otherwise.
#### symkey
Boolean indicating whether the color key should be made symmetric about 0. Defaults to TRUE if the data includes negative values, and to FALSE otherwise.
#### trace
character string indicating whether a solid "trace" line should be drawn across 'row's or down 'column's, 'both' or 'none'. The distance of the line from the center of each color-cell is proportional to the size of the measurement. Defaults to 'column'.
#### tracecol
character string giving the color for "trace" line. Defaults to "cyan".
#### vline
Vector of values within cells where a horizontal or vertical dotted line should be drawn. The color of the line is controlled by linecol. Horizontal lines are only plotted if trace is 'row' or 'both'. Vertical lines are only drawn if trace 'column' or 'both'. hline and vline default to the median of the breaks, linecol defaults to the value of tracecol.
