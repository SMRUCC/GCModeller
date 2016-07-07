---
title: NumericVector
---

# NumericVector
_namespace: [RDotNET](N-RDotNET.html)_

A collection of real numbers in double precision.



### Methods

#### #ctor
```csharp
RDotNET.NumericVector.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a numeric vector.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a numeric vector.|


#### CopyTo
```csharp
RDotNET.NumericVector.CopyTo(System.Double[],System.Int32,System.Int32,System.Int32)
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
RDotNET.NumericVector.GetArrayFast
```
Efficient conversion from R vector representation to the array equivalent in the CLR
_returns: Array equivalent_

#### SetVectorDirect
```csharp
RDotNET.NumericVector.SetVectorDirect(System.Double[])
```
Efficient initialisation of R vector values from an array representation in the CLR


### Properties

#### DataSize
Gets the size of a real number in byte.
#### Item
Gets or sets the element at the specified index.
