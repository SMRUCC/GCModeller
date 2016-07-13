---
title: kmeans
---

# kmeans
_namespace: [RDotNET.Extensions.VisualBasic.stats](N-RDotNET.Extensions.VisualBasic.stats.html)_

Perform k-means clustering on a data matrix.




### Properties

#### algorithm
character: may be abbreviated. Note that "Lloyd" and "Forgy" are alternative names for one algorithm.
#### centers
either the number of clusters, say k, or a set of initial (distinct) cluster centres. If a number, a random set of (distinct) rows in x is chosen as the initial centres.
#### iterMax
the maximum number of iterations allowed.
#### nstart
if centers is a number, how many random sets should be chosen?
#### trace
logical Or integer number, currently only used in the default method ("Hartigan-Wong") If positive(Or True), tracing information On the progress Of the algorithm Is produced. Higher values may produce more tracing information.
#### x
numeric matrix Of data, Or an Object that can be coerced To such a matrix (such As a numeric vector Or a data frame With all numeric columns).
