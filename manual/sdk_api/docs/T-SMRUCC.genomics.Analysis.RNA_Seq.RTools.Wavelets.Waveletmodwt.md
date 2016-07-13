---
title: Waveletmodwt
---

# Waveletmodwt
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.RTools.Wavelets](N-SMRUCC.genomics.Analysis.RNA_Seq.RTools.Wavelets.html)_

小波变换的输出结果



### Methods

#### Load
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.Wavelets.Waveletmodwt.Load(RDotNET.SymbolicExpression)
```


|Parameter Name|Remarks|
|--------------|-------|
|Expr|R的输出|



### Properties

#### aligned
A logical value indicating whether the wavelet and scaling coefficients have been phase shifted so as to be aligned with relevant time information from the original series. The value of this slot is initially FALSE and can only be changed to TRUE via the align function, with the modwt object as input.
#### attrX
A list containing the attributes information of the original time series, X. This is useful if X is an object of 
 class ts or mts and it is desired to retain relevant time information. If the original time series, X, is a 
 matrix or has no attributes, then attr.X is an empty list.
#### boundary
A character string indicating the boundary method used in the decomposition. Valid values are "periodic" or "reflection".
#### classX
A character string indicating the class of the input series. Possible values are "ts", "mts", "numeric", "matrix", or "data.frame".
#### coe
A logical value indicating whether the center of energy method was used in phase alignement of the wavelet and scaling coefficients. By default, this value is FALSE (and will always be FALSE when aligned is FALSE) and will be set to true if the modwt object is phase shifted via the align function and center of energy method.
#### filter
A wt.filter object containing information for the filter used in the decomposition. See help(wt.filter) for details.
#### Level
An integer value representing the level of wavelet decomposition.
#### nboundary
A numeric vector indicating the number of boundary coefficients at each level of the decomposition.
#### series
The original time series, X, in matrix format.
#### V
A list with element i comprised of a matrix containing the ith level scaling coefficients.
#### W
A list with element i comprised of a matrix containing the ith level wavelet coefficients.
