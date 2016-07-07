---
title: NumericMatrix
---

# NumericMatrix
_namespace: [RDotNET](N-RDotNET.html)_

A matrix of real numbers in double precision.



### Methods

#### #ctor
```csharp
RDotNET.NumericMatrix.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a numeric matrix.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a numeric matrix.|


#### GetArrayFast
```csharp
RDotNET.NumericMatrix.GetArrayFast
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
Gets the size of a real number in byte.
#### Item
Gets or sets the element at the specified index.
