---
title: RawMatrix
---

# RawMatrix
_namespace: [RDotNET](N-RDotNET.html)_

A matrix of byte values.



### Methods

#### #ctor
```csharp
RDotNET.RawMatrix.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a raw matrix.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a raw matrix.|


#### GetArrayFast
```csharp
RDotNET.RawMatrix.GetArrayFast
```
Efficient conversion from R vector representation to the array equivalent in the CLR
_returns: Array equivalent_

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
Gets the size of an Raw in byte.
#### Item
Gets or sets the element at the specified index.
