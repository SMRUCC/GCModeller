---
title: customFit
---

# customFit
_namespace: [RDotNet.Extensions.Bioinformatics.declares.bnlearn](N-RDotNet.Extensions.Bioinformatics.declares.bnlearn.html)_

custom.fit takes a set of user-specified distributions and their parameters and uses them to build a bn.fit object. 
 Its purpose is to specify a Bayesian network (complete with the parameters, not only the structure) using knowledge from experts in the field instead of learning it from a data set. 
 The distributions must be passed to the function in a list, with elements named after the nodes of the network structure x. 
 Each element of the list must be in one of the formats described above for in-place replacement.




### Properties

#### dist
a named list, with element for each node of x. See below.
#### ordinal
a vector Of character strings, the labels Of the discrete nodes which should be saved As ordinal random variables (bn.fit.onode) instead Of unordered factors (bn.fit.dnode).
#### x
an object of class bn (for bn.fit and custom.fit) or an object of class bn.fit (for bn.net).
