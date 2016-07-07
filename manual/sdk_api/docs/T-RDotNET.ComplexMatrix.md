---
title: ComplexMatrix
---

# ComplexMatrix
_namespace: [RDotNET](N-RDotNET.html)_

A matrix of complex numbers.



### Methods

#### #ctor
```csharp
RDotNET.ComplexMatrix.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a complex number matrix.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a complex number matrix.|


#### GetArrayFast
```csharp
RDotNET.ComplexMatrix.GetArrayFast
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
Gets the size of a complex number in byte.
#### Item
Gets or sets the element at the specified index.
