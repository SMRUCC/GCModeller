---
title: IntegerVector
---

# IntegerVector
_namespace: [RDotNET](N-RDotNET.html)_

A collection of integers from -2^31 + 1 to 2^31 - 1.

> 
>  The minimum value of IntegerVector is different from that of System.Int32 in .NET Framework.
>  


### Methods

#### #ctor
```csharp
RDotNET.IntegerVector.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for an integer vector.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to an integer vector.|


#### CopyTo
```csharp
RDotNET.IntegerVector.CopyTo(System.Int32[],System.Int32,System.Int32,System.Int32)
```
Copies the elements to the specified array.

|Parameter Name|Remarks|
|--------------|-------|
|destination|The destination array.|
|length__1|The length to copy.|
|sourceIndex|The first index of the vector.|
|destinationIndex|The first index of the destination array.|


#### GetArrayFast
```csharp
RDotNET.IntegerVector.GetArrayFast
```
Efficient conversion from R vector representation to the array equivalent in the CLR
_returns: Array equivalent_

#### SetVectorDirect
```csharp
RDotNET.IntegerVector.SetVectorDirect(System.Int32[])
```
Efficient initialisation of R vector values from an array representation in the CLR


### Properties

#### DataSize
Gets the size of an integer in byte.
#### Item
Gets or sets the element at the specified index.
#### NACode
Gets the code used for NA for integer vectors
