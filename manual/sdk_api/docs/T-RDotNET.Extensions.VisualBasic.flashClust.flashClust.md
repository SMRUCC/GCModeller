---
title: flashClust
---

# flashClust
_namespace: [RDotNET.Extensions.VisualBasic.flashClust](N-RDotNET.Extensions.VisualBasic.flashClust.html)_

Faster alternative to hclust.
 This function implements optimal hierarchical clustering with the same interface as hclust.

> 
>  See the description of hclust for details on available clustering methods.
> 
>  If members! = NULL, Then d Is taken To be a dissimilarity matrix between clusters instead Of dissimilarities between singletons And members gives the number Of observations per cluster. 
>  This way the hierarchical cluster algorithm can be 'started in the middle of the dendrogram’, e.g., in order to reconstruct the part of the tree above a cut (see examples). 
>  Dissimilarities between clusters can be efficiently computed (i.e., without hclust itself) only for a limited number of distance/linkage combinations, the simplest one being squared Euclidean distance and centroid linkage. 
>  In this case the dissimilarities between the clusters are the squared Euclidean distances between cluster means.
>  flashClust Is a wrapper for compatibility with older code.
>  



### Properties

#### d
a dissimilarity Structure As produced by 'dist'.
#### members
NULL Or a vector with length size of d. See the 'Details’ section.
#### method
the agglomeration method To be used. This should be (an unambiguous abbreviation Of) one Of "ward", "single", "complete", "average", "mcquitty", "median" Or "centroid".
