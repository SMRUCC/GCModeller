---
title: PolynomialFit
---

# PolynomialFit
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.html)_

The original works was comes from here: http://www.vb-helper.com/howto_net_polynomial_least_squares.html



### Methods

#### ErrorSquared
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.PolynomialFit.ErrorSquared(SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.PolynomialFit.PointF[],System.Double[])
```
Return the error squared.

|Parameter Name|Remarks|
|--------------|-------|
|points|-|
|coeffs|-|


#### F
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.PolynomialFit.F(System.Double[],System.Double)
```
Calculate the function value for a specific X.

|Parameter Name|Remarks|
|--------------|-------|
|coeffs|Calculation result from @"M:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.PolynomialFit.FindPolynomialLeastSquaresFit(SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.PolynomialFit.PointF[],System.Int32)"[the polynomial fit function]|
|x|-|


#### FindPolynomialLeastSquaresFit
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.PolynomialFit.FindPolynomialLeastSquaresFit(SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.PolynomialFit.PointF[],System.Int32)
```
Find the least squares linear fit.

|Parameter Name|Remarks|
|--------------|-------|
|points|-|
|degree|-|



