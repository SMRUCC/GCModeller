---
title: VectorAPI
---

# VectorAPI
_namespace: [SMRUCC.genomics.Analysis.Metagenome.BEBaC](N-SMRUCC.genomics.Analysis.Metagenome.BEBaC.html)_





### Methods

#### GetVector
```csharp
SMRUCC.genomics.Analysis.Metagenome.BEBaC.VectorAPI.GetVector(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel)
```
We then transform each sequence ``xi`` To a ``3-mer count vector`` **yi**, 
 Where a ``3-mer`` means 3 consecutive DNA bases ranging from ``AAA'' to ``TTT``. 
 Each element of the ``1 x 64`` vector ``yi=(yi1, yi2, ... , yij, ... yi64)``
 represents the count of its corresponding ``3-mer`` in the given sequence ``xi``. 
 Hence the sequence set ``x(N)`` Is transformed to a ``3-mer`` count set 
 ``y(N)={y1, y2, ... yn}``.

|Parameter Name|Remarks|
|--------------|-------|
|seq|-|


#### Transform``1
```csharp
SMRUCC.genomics.Analysis.Metagenome.BEBaC.VectorAPI.Transform``1(System.Collections.Generic.IEnumerable{``0},System.Func{``0,System.String})
```
We then transform each sequence ``xi`` To a ``3-mer count vector`` **yi**, 
 Where a ``3-mer`` means 3 consecutive DNA bases ranging from ``AAA'' to ``TTT``. 
 Each element of the ``1 x 64`` vector ``yi=(yi1, yi2, ... , yij, ... yi64)``
 represents the count of its corresponding ``3-mer`` in the given sequence ``xi``. 
 Hence the sequence set ``x(N)`` Is transformed to a ``3-mer`` count set 
 ``y(N)={y1, y2, ... yn}``.

|Parameter Name|Remarks|
|--------------|-------|
|x|-|
|getTag|-|



