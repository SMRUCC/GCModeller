---
title: ComplexVector
---

# ComplexVector
_namespace: [RDotNET](N-RDotNET.html)_

A collection of complex numbers.



### Methods

#### #ctor
```csharp
RDotNET.ComplexVector.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a complex number vector.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a complex number vector.|


#### GetArrayFast
```csharp
RDotNET.ComplexVector.GetArrayFast
```
Gets an array representation in the CLR of a vector in R.

#### SetVectorDirect
```csharp
RDotNET.ComplexVector.SetVectorDirect(System.Numerics.Complex[])
```
Efficient initialisation of R vector values from an array representation in the CLR


### Properties

#### DataSize
Gets the size of a complex number in byte.
#### Item
Gets or sets the element at the specified index.
