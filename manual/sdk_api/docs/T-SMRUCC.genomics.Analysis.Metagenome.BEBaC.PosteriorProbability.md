---
title: PosteriorProbability
---

# PosteriorProbability
_namespace: [SMRUCC.genomics.Analysis.Metagenome.BEBaC](N-SMRUCC.genomics.Analysis.Metagenome.BEBaC.html)_





### Methods

#### MarginalLikelihood
```csharp
SMRUCC.genomics.Analysis.Metagenome.BEBaC.PosteriorProbability.MarginalLikelihood(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.Metagenome.BEBaC.Cluster})
```
Provides an analytical form of the marginal likelihood Of ``y(N)`` 
 given the partition ``S``, which Is proportional to the posterior 
 probability as suggested by Equation (2).

|Parameter Name|Remarks|
|--------------|-------|
|s|-|


#### nj
```csharp
SMRUCC.genomics.Analysis.Metagenome.BEBaC.PosteriorProbability.nj(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.Metagenome.BEBaC.I3merVector},SMRUCC.genomics.Analysis.Metagenome.BEBaC.I3Mers)
```
Where ``ncj = ∑yij`` is the total count Of the j-th ``3-mer`` in cluster ``**c**``.

|Parameter Name|Remarks|
|--------------|-------|
|c|-|
|j|-|


#### Probability
```csharp
SMRUCC.genomics.Analysis.Metagenome.BEBaC.PosteriorProbability.Probability(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.Metagenome.BEBaC.I3merVector})
```
For all ``3-mer`` count vectors in a crude cluster ``c``, **we
 assume the probability To observe any ``3-mer`` Is the same.**
 
 Here we denote the probabilities To observe the ``3-mers`` In
 cluster ``c`` As ``(pc1, pc2, ... , pc64)``. Then, the conditional 
 likelihood of the data Is defined as
 
 ```
 p(y{n}|D,S) = ∏{c,1->k}∏{j,1->64}pcj
 ```


