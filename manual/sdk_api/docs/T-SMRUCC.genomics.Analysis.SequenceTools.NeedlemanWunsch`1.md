---
title: NeedlemanWunsch`1
---

# NeedlemanWunsch`1
_namespace: [SMRUCC.genomics.Analysis.SequenceTools](N-SMRUCC.genomics.Analysis.SequenceTools.html)_

Needleman-Wunsch Algorithm
 Bioinformatics 1, WS 15/16
 Dr. Kay Nieselt and Alexander Seitz



### Methods

#### compute
```csharp
SMRUCC.genomics.Analysis.SequenceTools.NeedlemanWunsch`1.compute
```
computes the matrix for the Needleman-Wunsch Algorithm
> 	
>  this function computes the NW-algorithm with linear gap-costs
>   - first make yourself familiar with this function and the functions used to compute the resulting alignment!
>   
>   - modify the functions used in this class such that the NW algorithm is modular
>     i.e. the following criteria should be fulfilled: 
>         - it should be easy to replace the linear gap cost function with an affine gap cost function
>         - the initialization step, fill and traceback should be modular, to allow
>           to switch between different algorithms later (NW, SW, OverlapAlignment etc.)
>     
>   - you are allowed to change the class structure, if you think that it is necessary!
>     (make sure to use object oriented programming concepts, i.e. use objects to abstract your code 
>    	-> don't do everything in a single class)    	 
>  

#### fillTracebackMatrix
```csharp
SMRUCC.genomics.Analysis.SequenceTools.NeedlemanWunsch`1.fillTracebackMatrix(System.Int32,System.Int32,System.Int32)
```
return the maximizing cell(s)
 1 , if the maximizing cell is the upper cell
 2 , if the maximizing cell is the left-upper cell
 4 , if the maximizing cell is the left cell
 if there are more than one maximizing cells, the values are summed up

|Parameter Name|Remarks|
|--------------|-------|
|upperLeft|-|
|left|-|
|upper|-|

_returns:  code for the maximizing cell(s) _

#### traceback
```csharp
SMRUCC.genomics.Analysis.SequenceTools.NeedlemanWunsch`1.traceback(System.Collections.Generic.Stack{`0},System.Collections.Generic.Stack{`0},System.Int32,System.Int32)
```
this function is called for the first time with two empty stacks
and the end indices of the matrix

the function computes a traceback over the matrix, it calls itself recursively
for each sequence, it pushes the aligned character (a,c,g,t or -)
on a stack (use java.util.Stack with the function push())

|Parameter Name|Remarks|
|--------------|-------|
|s1|-|
|s2|-|
|i|-|
|j|-|


#### writeAlignment
```csharp
SMRUCC.genomics.Analysis.SequenceTools.NeedlemanWunsch`1.writeAlignment(System.String,System.Boolean)
```
This funktion provide a easy way to write a computed alignment into a fasta file

|Parameter Name|Remarks|
|--------------|-------|
|outFile|-|
|single|-|



