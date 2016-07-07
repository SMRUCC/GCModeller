---
title: heatmap_plot
---

# heatmap_plot
_namespace: [RDotNET.Extensions.VisualBasic.stats](N-RDotNET.Extensions.VisualBasic.stats.html)_






### Properties

#### addExpr
expression that will be evaluated after the call to image. Can be used to add components to the plot.
#### cexCol
positive numbers, used as cex.axis in for the row or column axis labeling. The defaults currently only use number of rows or columns, respectively.
#### cexRow
positive numbers, used as cex.axis in for the row or column axis labeling. The defaults currently only use number of rows or columns, respectively.
#### ColSideColors
(optional) character vector of length ncol(x) containing the color names for a horizontal side bar that may be used to annotate the columns of x.
#### distfun
function used to compute the distance (dissimilarity) between both rows and columns. Defaults to dist.
#### hclustfun
function used to compute the hierarchical clustering when Rowv or Colv are not dendrograms. Defaults to hclust. Should take as argument a result of distfun and return an object to which as.dendrogram can be applied.
#### keepDendro
logical indicating if the dendrogram(s) should be kept as part of the result (when Rowv and/or Colv are not NA).
#### labCol
character vectors with row and column labels to use; these default to rownames(x) or colnames(x), respectively.
#### labRow
character vectors with row and column labels to use; these default to rownames(x) or colnames(x), respectively.
#### main
main, x- and y-axis titles; defaults to none.
#### margins
numeric vector of length 2 containing the margins (see par(mar = *)) for column and row names, respectively.
#### naRM
logical indicating whether NA's should be removed.
#### reorderfun
function(d, w) of dendrogram and weights for reordering the row and column dendrograms. The default uses reorder.dendrogram.
#### revC
logical indicating if the column order should be reversed for plotting, such that e.g., for the symmetric case, the symmetry axis is as usual.
#### RowSideColors
(optional) character vector of length nrow(x) containing the color names for a vertical side bar that may be used to annotate the rows of x.
#### scale
character indicating if the values should be centered and scaled in either the row direction or the column direction, or none. The default is "row" if symm false, and "none" otherwise.
#### symm
logical indicating if x should be treated symmetrically; can only be true when x is a square matrix.
#### verbose
logical indicating if information should be printed.
#### x
numeric matrix of the values to be plotted.
#### xlab
main, x- and y-axis titles; defaults to none.
#### ylab
main, x- and y-axis titles; defaults to none.
