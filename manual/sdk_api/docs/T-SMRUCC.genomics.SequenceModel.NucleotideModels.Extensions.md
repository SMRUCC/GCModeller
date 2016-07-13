---
title: Extensions
---

# Extensions
_namespace: [SMRUCC.genomics.SequenceModel.NucleotideModels](N-SMRUCC.genomics.SequenceModel.NucleotideModels.html)_





### Methods

#### LevenshteinDistance
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Extensions.LevenshteinDistance(System.String,System.String)
```
Compute Levenshtein distance Michael Gilleland, Merriam Park Software.(http://www.merriampark.com/ld.htm)

|Parameter Name|Remarks|
|--------------|-------|
|s|-|
|t|-|


#### LevenshteinDistance2
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Extensions.LevenshteinDistance2(System.String,System.String)
```
Chas Emerick.(http://www.merriampark.com/ldjava.htm)

|Parameter Name|Remarks|
|--------------|-------|
|s|-|
|t|-|

> 
>  The difference between this impl. and the previous is that, rather
>  than creating and retaining a matrix of size s.length()+1 by t.length()+1,
>  we maintain two single-dimensional arrays of length s.length()+1.  The first, d,
>  is the 'current working' distance array that maintains the newest distance cost
>  counts as we iterate through the characters of String s.  Each time we increment
>  the index of String t we are comparing, d is copied to p, the second int[].  Doing so
>  allows us to retain the previous cost counts as required by the algorithm (taking
>  the minimum of the cost count to the left, up one, and diagonally up and to the left
>  of the current cost count being calculated).  (Note that the arrays aren't really
>  copied anymore, just switched...this is clearly much better than cloning an array
>  or doing a System.arraycopy() each time  through the outer loop.)
>  
>  Effectively, the difference between the two implementations is this one does not
>  cause an out of memory condition when calculating the LD over two very large strings.
>  


