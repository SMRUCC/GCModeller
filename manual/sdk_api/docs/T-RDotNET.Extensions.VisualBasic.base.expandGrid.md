---
title: expandGrid
---

# expandGrid
_namespace: [RDotNET.Extensions.VisualBasic.base](N-RDotNET.Extensions.VisualBasic.base.html)_

Create a data frame from all combinations of the supplied vectors or factors. See the description of the return value for precise details of the way this is done.
 
 A data frame containing one row for each combination of the supplied factors. The first factors vary fastest. The columns are labelled by the factors if these are supplied as named arguments or named components of a list. The row names are ‘automatic’.
 Attribute "out.attrs" Is a list which gives the dimension And dimnames for use by predict methods.




### Properties

#### KEEP_OUT_ATTRS
a logical indicating the "out.attrs" attribute (see below) should be computed and returned.
#### stringsAsFactors
logical specifying if character vectors are converted to factors.
#### x
vectors, factors or a list containing these.
