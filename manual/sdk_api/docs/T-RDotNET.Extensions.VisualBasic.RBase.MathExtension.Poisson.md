---
title: Poisson
---

# Poisson
_namespace: [RDotNET.Extensions.VisualBasic.RBase.MathExtension](N-RDotNET.Extensions.VisualBasic.RBase.MathExtension.html)_

Density, distribution function, quantile function and random generation for the Poisson distribution with parameter lambda.



### Methods

#### Dpois
```csharp
RDotNET.Extensions.VisualBasic.RBase.MathExtension.Poisson.Dpois(RDotNET.Extensions.VisualBasic.RBase.Vectors.Vector,RDotNET.Extensions.VisualBasic.RBase.Vectors.Vector,System.Boolean)
```
Density, distribution function, quantile function and random generation for the Poisson distribution with parameter lambda.

|Parameter Name|Remarks|
|--------------|-------|
|x|vector of (non-negative integer) quantiles.|
|lambda|vector of (non-negative) means.|
|log|logical; if TRUE, probabilities p are given as log(p).|


#### rPois
```csharp
RDotNET.Extensions.VisualBasic.RBase.MathExtension.Poisson.rPois(System.Int32,RDotNET.Extensions.VisualBasic.RBase.Vectors.Vector)
```
Density, distribution function, quantile function and random generation for the Poisson distribution with parameter lambda.

|Parameter Name|Remarks|
|--------------|-------|
|n|number of random values to return.|
|lambda|vector of (non-negative) means.|



