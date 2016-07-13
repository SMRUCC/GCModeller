---
title: combn
---

# combn
_namespace: [RDotNET.Extensions.VisualBasic.utils](N-RDotNET.Extensions.VisualBasic.utils.html)_

Generate all combinations of the elements of x taken m at a time. If x is a positive integer, returns all combinations of the elements of seq(x) taken m at a time. 
 If argument FUN is not NULL, applies a function given by the argument to each point. If simplify is FALSE, returns a list; 
 otherwise returns an array, typically a matrix. ... are passed unchanged to the FUN function, if specified.
 
 Factors x are accepted from R 3.1.0 (although coincidentally they worked for simplify = FALSE in earlier versions).




### Properties

#### FUN
function to be applied to each combination; default NULL means the identity, i.e., to return the combination (vector of length m).
#### m
number of elements to choose.
#### simplify
logical indicating if the result should be simplified to an array (typically a matrix); if FALSE, the function returns a list. Note that when simplify = TRUE as by default, the dimension of the result is simply determined from FUN(1st combination) (for efficiency reasons). This will badly fail if FUN(u) is not of constant length.
#### x
vector source for combinations, or integer n for x <- seq_len(n).
