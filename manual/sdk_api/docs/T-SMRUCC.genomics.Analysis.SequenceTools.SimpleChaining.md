---
title: SimpleChaining
---

# SimpleChaining
_namespace: [SMRUCC.genomics.Analysis.SequenceTools](N-SMRUCC.genomics.Analysis.SequenceTools.html)_





### Methods

#### chaining
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SimpleChaining.chaining(System.Collections.Generic.List{SMRUCC.genomics.Analysis.SequenceTools.Match},System.Boolean)
```
Identify the best chain from given list of match

|Parameter Name|Remarks|
|--------------|-------|
|matches| a list of match |
|debug|  if true, print list of input match, adjacency, score matrix, best chain found. |

_returns:  the optimal chain as a list of match _

#### printLowerMatrix
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SimpleChaining.printLowerMatrix(System.Double[],System.Int32)
```
System out the input array as an strict lower diagonal matrix


