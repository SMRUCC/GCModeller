---
title: LogicalMatrix
---

# LogicalMatrix
_namespace: [RDotNET](N-RDotNET.html)_

A matrix of Boolean values.



### Methods

#### #ctor
```csharp
RDotNET.LogicalMatrix.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a Boolean matrix.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a Boolean matrix.|


#### GetArrayFast
```csharp
RDotNET.LogicalMatrix.GetArrayFast
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
