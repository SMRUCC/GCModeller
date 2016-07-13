---
title: vennDiagramPlot
---

# vennDiagramPlot
_namespace: [RDotNet.Extensions.Bioinformatics.VennDiagram](N-RDotNet.Extensions.Bioinformatics.VennDiagram.html)_

This function takes a list and creates a publication-quality TIFF Venn Diagram




### Properties

#### areaVector
An argument to be used when direct.area is true. These are the areas of the corresponding regions in the Venn Diagram
#### categoryNames
Allow specification of category names using plotmath syntax
#### compression
What compression algorithm should be applied to the final tiff
#### directArea
If this is equal to true, then the vector passed into area.vector will be directly assigned to the areas of the corresponding regions. Only use this if you know which positions in the vector correspond to which regions in the diagram
#### filename
Filename for image output, Or if NULL returns the grid object itself
#### fill
The partition fill color
#### forceUnique
Logical specifying whether to use only unique elements in each item of the input list or use all elements. Defaults to FALSE
#### height
Integer giving the height Of the output figure In units
#### hyperTest
If there are only two categories in the venn diagram and total.population is not NULL, then perform the hypergeometric test and add it to the sub title.
#### imagetype
Specification of the image format (e.g. tiff, png or svg)
#### main
Character giving the main title of the diagram
#### mainCex
Number giving the cex (font size) of the main title
#### mainCol
Character giving the colour of the main title
#### mainFontface
Character giving the fontface (font style) of the main title
#### mainFontfamily
Character giving the fontfamily (font type) of the main title
#### mainJust
Vector of length 2 indicating horizontal and vertical justification of the main title
#### mainPos
Vector of length 2 indicating (x,y) of the main title
#### na
Missing value handling method: "none", "stop", "remove"
#### printMode
Can be either 'raw' or 'percent'. This is the format that the numbers will be printed in. Can pass in a vector with the second element being printed under the first
#### resolution
Resolution of the final figure in DPI
#### sigdigs
If one of the elements in print.mode is 'percent', then this is how many significant digits will be kept
#### sub
Character giving the subtitle of the diagram
#### subCex
Number giving the cex (font size) of the subtitle
#### subCol
Character Colour of the subtitle
#### subFontface
Character giving the fontface (font style) of the subtitle
#### subFontfamily
Character giving the fontfamily (font type) of the subtitle
#### subJust
Vector of length 2 indicating horizontal and vertical justification of the subtitle
#### subPos
Vector of length 2 indicating (x,y) of the subtitle
#### totalPopulation
An argument to be used when hyper.test is true. This is the total population size
#### units
Size-units to use for the final figure
#### width
Integer giving the width of the output figure in units
#### x
A list of vectors (e.g., integers, chars), with each component corresponding to a separate circle in the Venn diagram
