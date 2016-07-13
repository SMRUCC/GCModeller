---
title: Base
---

# Base
_namespace: [SMRUCC.genomics.Analysis.PFSNet.R](N-SMRUCC.genomics.Analysis.PFSNet.R.html)_





### Methods

#### Quantile
```csharp
SMRUCC.genomics.Analysis.PFSNet.R.Base.Quantile(System.Double[],System.Double)
```
The generic function quantile produces sample quantiles corresponding to the given probabilities. The smallest observation corresponds to a probability of 0 and the largest to a probability of 1.

|Parameter Name|Remarks|
|--------------|-------|
|x|numeric vector whose sample quantiles are wanted, or an object of a class for which a method has been defined (see also ‘details’). NA and NaN values are not allowed in numeric vectors unless na.rm is TRUE.|
|probs|numeric vector of probabilities with values in [0,1]. (Values up to 2e-14 outside that range are accepted and moved to the nearby endpoint.)|


#### Sample``1
```csharp
SMRUCC.genomics.Analysis.PFSNet.R.Base.Sample``1(``0[],System.Int32)
```
sample.int(n, size = n, replace = FALSE, prob = NULL)
 sample takes a sample of the specified size from the elements of x using either with or without replacement.

|Parameter Name|Remarks|
|--------------|-------|
|x|-|
|size|-|



