---
title: drawQuadVenn
---

# drawQuadVenn
_namespace: [RDotNet.Extensions.Bioinformatics.VennDiagram](N-RDotNet.Extensions.Bioinformatics.VennDiagram.html)_

Creates a Venn diagram with four sets.

> 
>  The function defaults to placing the ellipses so that area1 corresponds to lower left,
>  area2 corresponds to lower right, area3 corresponds to middle left and area4 corresponds
>  to middle right. Refer to the example below to see how the 31 partial areas are ordered.
>  Arguments with length of 15 (label.col, cex, fontface, fontfamily) will follow the order
>  in the example.
> 
>  Value
> 
>  Returns an Object Of Class gList containing the grid objects that make up the diagram.
>  Also displays the diagram In a graphical device unless specified With ind = False.
>  Grid:grid.draw can be used to draw the gList object in a graphical device.
>  



### Properties

#### alpha
A vector (length 4) giving the alpha transparency of the circles' areas
#### area1
The size of the first set
#### area2
The size of the second set
#### area3
The size of the third set
#### area4
The size of the fourth set
#### areaVector
An argument to be used when direct.area is true. These are the areas of the corresponding
 regions in the Venn Diagram
#### catCex
A vector (length 4) giving the colours of the category names
#### catCol
A vector (length 4) giving the size of the category names
#### catDist
A vector (length 4) giving the distances (in npc units) of the category names from the edges
 of the circles (can be negative)
#### category
A vector (length 4) of strings giving the category names of the sets
#### catFontface
A vector (length 4) giving the fontface of the category names
#### catFontfamily
A vector (length 4) giving the fontfamily of the category names
#### catJust
List of 4 vectors of length 2 indicating horizontal and vertical justification of each category name
#### catPos
A vector (length 4) giving the positions (in degrees) of the category names along the circles,
 with 0 (default) at 12 o'clock
#### cex
A vector (length 15) giving the size of the areas' labels
#### cexProp
A function or string used to rescale areas
#### col
A vector (length 4) giving the colours of the circles' circumferences
#### directArea
If this is equal to true, then the vector passed into area.vector will be directly assigned
 to the areas of the corresponding regions. Only use this if you know which positions in the
 vector correspond to which regions in the diagram
#### fill
A vector (length 4) giving the colours of the circles' areas
#### fontface
A vector (length 15) giving the fontface of the areas' labels
#### fontfamily
A vector (length 15) giving the fontfamily of the areas' labels
#### ind
Boolean indicating whether the function is to automatically draw the diagram before returning
 the gList object or not
#### labelCol
A vector (length 15) giving the colours of the areas' labels
#### lty
A vector (length 4) giving the dash pattern of the circles' circumferences
#### lwd
A vector (length 4) of numbers giving the line width of the circles' circumferences
#### n12
The size of the intersection between the first and the second set
#### n123
The size of the intersection between the first, second and third sets
#### n1234
The size of the intersection between all four sets
#### n124
The size of the intersection between the first, second and fourth sets
#### n13
The size of the intersection between the first and the third set
#### n134
The size of the intersection between the first, third and fourth sets
#### n14
The size of the intersection between the first and the fourth set
#### n23
The size of the intersection between the second and the third set
#### n234
The size of the intersection between the second, third and fourth sets
#### n24
The size of the intersection between the second and the fourth set
#### n34
The size of the intersection between the third and the fourth set
#### printMode
Can be either 'raw' or 'percent'. This is the format that the numbers will be printed in.
 Can pass in a vector with the second element being printed under the first
#### rotationCentre
A vector (length 2) indicating (x,y) of the rotation centre
#### rotationDegree
Number of degrees to rotate the entire diagram
#### sigdigs
If one of the elements in print.mode is 'percent', then this is how many significant digits will be kept
