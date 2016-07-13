---
title: venn
---

# venn
_namespace: [RDotNET.Extensions.VisualBasic.gplots](N-RDotNET.Extensions.VisualBasic.gplots.html)_

Plot a Venn diagrams for up to 5 sets

> 
>  data should be either a named list of vectors containing character string names ("GeneAABBB", "GeneBBBCY", .., "GeneXXZZ")
>  or indexes of group intersections (1, 2, .., N), or a data frame containing indicator variables (TRUE, FALSE, TRUE, ..)
>  for group intersectionship. Group names will be taken from the component list element or column names.
> 
>  Invisibly returns an object of class "venn", containing a matrix of all possible sets of groups, and the observed count of
>  items belonging to each The fist column contains observed counts, subsequent columns contain 0-1 indicators of group
>  intersectionship.
>  



### Properties

#### data
Either a list list containing vectors of names or indices of group intersections,
 or a data frame containing boolean indicators of group intersectionship (see below)
#### intersections
Logical flag indicating if the returned object should have the attribute "individuals.in.intersections"
 featuring for every set a list of individuals that are assigned to it.
#### showPlot
Logical flag indicating whether the plot should be displayed. If false, simply returns the group count matrix.
#### showSetLogicLabel
Logical flag indicating whether the internal group label should be displayed
#### simplify
Logical flag indicating whether unobserved groups should be omitted.
#### small
Character scaling of the smallest group counts
#### universe
Subset of valid name/index elements. Values ignore values in codedata not
 in this list will be ignored. Use NA to use all elements of data (the default).
