---
title: getVennPartitions
---

# getVennPartitions
_namespace: [RDotNet.Extensions.Bioinformatics.VennDiagram](N-RDotNet.Extensions.Bioinformatics.VennDiagram.html)_

Partitions a list into Venn regions.
 
 If force.unique is FALSE, then there are two supported methods of grouping categories with duplicated elements in common. 
 If hierarchical is FALSE, then any common elements are gathered into a pool. So if x <- list(a = c(1,1,2,2,3,3), b=c(1,2,3,4,4,5), c=c(1,4)) then (b intersect c)/(a) would contain three 4's. Since the 4's are pooled, (b)/(a union c) contains no 4's. 
 If hierachical is TRUE, then (b intersect c)/(a) would contain one 4.Then (b)/(a union c) cotains one 4.




### Properties

#### forceUnique
A logical value. Should only unique values be considered?
#### hierarchical
A logical value. Changed the way overlapping elements are treated if force.unique is TRUE.
#### keepElements
A logical value. Should the elements in each region be returned?
#### x
A list of vectors.
