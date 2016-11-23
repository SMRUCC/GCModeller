# API
_namespace: [RDotNET.Extensions.Bioinformatics.LDheatmap](./index.md)_





### Methods

#### LDheatmap
```csharp
RDotNET.Extensions.Bioinformatics.LDheatmap.API.LDheatmap(System.String,System.String,System.String,System.String,System.String,System.Boolean,System.Boolean,System.Double,System.String,System.String,System.String,System.String,System.Boolean,System.String,System.String,System.Boolean,System.String,System.Boolean,System.String@)
```
``LDheatmap()`` is used to produce a graphical display, as a heat map, of pairwise linkage disequilibrium (LD) measurements for SNPs. 
 The heat map is a false color image in the upper-left diagonal of a square plot. Optionally, a line parallel to the diagonal of the 
 image indicating the physical or genetic map positions of the SNPs may be added, along with text reporting the total length of the 
 genomic region considered.

|Parameter Name|Remarks|
|--------------|-------|
|gdat|SNP data: a data frame of genotype objects, a square matrix of pairwise linkage disequilibrium measurements or an object of class "LDheatmap" (the returned object of this function).|
|geneticDistances|A numeric vector of map locations of the SNPs, in the same order as SNPs listed in gdat, in terms of genetic or physical distances. Physical distances should be in bases, genetic distances should be in centiMorgans (cM). When gdat is not an object of class LDheatmap, the default is a vector that represents equi-spaced markers, 1kb (1000 bases) apart. When gdat is an object of class LDheatmap, the genetic.distances argument is taken to be the genetic.distances list item of gdat.|
|distances|A character string to specify whether the provided map locations are in physical or genetic distances. If distances="physical" (default), the text describing the total length of the region will be “Physical Length:XXkb” where XX is the length of the region in kilobases. If distances="genetic", the text will be “Genetic Map Length:YYcM” where YY is the length of the region in centiMorgans. If gdat is an object of class LDheatmap, distances is taken from gdat.|
|LDmeasure|A character string specifying the measure of LD - either allelic correlation r^2 or Lewontin's |D'|; default = "r" for r^2; type "D'" for |D'|. This argument is ignored when the user has already supplied calculated LD measurements through gdat (i.e., when gdat is a matrix of pairwise LD measurements or an object of class "LDheatmap").|
|title|A character string for the main title of the plot. Default is “Pairwise LD”.|
|addMap|If TRUE (default), a diagonal line indicating the physical or genetic map positions of the SNPs will be added to the plot, along with text indicating the total length of the genetic region.|
|addKey|If TRUE (default) the color legend is drawn.|
|geneMapLocation|A numeric value specifying the position of the line parallel to the diagonal of the matrix; the larger the value, the farther it lies from the matrix diagonal. Ignored when add.map=FALSE.|
|geneMapLabelX|A numeric value specifying the x-coordinate of the text indicating the total length of the genomic region being considered. Ignored when add.map=FALSE.|
|geneMapLabelY|A numeric value specifying the y-coordinate of the text indicating the total length of the genomic region being considered. Ignored when add.map=FALSE.|
|SNPname|A vector of character string(s) of SNP name(s) to be labelled. Should match the names of SNPs in the provided object gdat, otherwise nothing is done.|
|color|A range of colors to be used for drawing the heat map. Default is grey.colors(20).|
|newpage|If TRUE (default), the heat map will be drawn on a new page.|
|name|A character string specifying the name of the LDheatmap graphical object (grob) to be produced.|
|vpname|A character string specifying the name of the viewport where the heat map is going to be drawn.|
|pop|If TRUE, the viewport where the heat map is drawn is popped (i.e. removed) from the viewport tree after drawing. Default=FALSE.|
|flip|If TRUE, the LDheatmap plot is flipped below a horizontal line, in the style of Haploview. Default is FALSE.|
|text|If TRUE, the LD measurements are printed on each cell.|


_returns: For gdat, any square matrix with values between 0 and 1 inclusive can be passed, and the values above the diagonal will be plotted. In the display of LD, SNPs appear in the order supplied by the user as the horizontal and vertical coordinates are increased and one moves along the off-diagonal line, from the bottom-left to the top-right corner. To achieve this, the conventions of the image() function have been adopted, in which horizontal coordinates correspond to the rows of the matrix and vertical coordinates correspond to columns, and vertical coordinates are indexed in increasing order from bottom to top.
 For the argument color, an appropriate color palette for quantitative data Is recommended, as outlined in the help page of the brewer.pal() function of the RColorBrewer package.
 See the package vignette LDheatmap For more examples And details Of the implementation. Examples Of adding “tracks” Of genomic annotation above a flipped heatmap are In the package vignette addTracks.
 _
> The produced heat map can be modified in two ways. First, it is possible to edit interactively the grob components of the heat map, by using the function grid.edit; the function will not work if there is no open graphical device showing the heat map. Alternatively, the user can use the function editGrob and work with the grob LDheatmapGrob returned by LDheatmap. See Examples for usage.
>  LDheatmap() uses Grid, which does Not respond to par() settings. Hence modifying par() settings of mfrow And mfcol will Not work with LDheatmap(). The Examples section shows how to display multiple heat maps on one plot without the use of par().
>  


