---
title: IntegerMatrix
---

# IntegerMatrix
_namespace: [RDotNET](N-RDotNET.html)_

A matrix of integers from -2^31 + 1 to 2^31 - 1.

> 
>  The minimum value of IntegerVector is different from that of System.Int32 in .NET Framework.
>  


### Methods

#### #ctor
```csharp
RDotNET.IntegerMatrix.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for an integer matrix.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to an integer matrix.|


#### GetArrayFast
```csharp
RDotNET.IntegerMatrix.GetArrayFast
```
Gets a rectangular array representation in the CLR, equivalent of a matrix in R.
_returns: Rectangular array with values representing the content of the R matrix. Beware NA codes_

#### InitMatrixFastDirect
```csharp
])
```
Initializes this R matrix, using the values in a rectangular array.

|Parameter Name|Remarks|
|--------------|-------|
|matrix|-|



### Properties

#### DataSize
Gets the size of an integer in byte.
#### Item
Gets or sets the element at the specified index.
