---
title: LogicalVector
---

# LogicalVector
_namespace: [RDotNET](N-RDotNET.html)_

A collection of Boolean values.



### Methods

#### #ctor
```csharp
RDotNET.LogicalVector.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a Boolean vector.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a Boolean vector.|


#### GetArrayFast
```csharp
RDotNET.LogicalVector.GetArrayFast
```
Efficient conversion from R vector representation to the array equivalent in the CLR
_returns: Array equivalent_

#### SetVectorDirect
```csharp
RDotNET.LogicalVector.SetVectorDirect(System.Boolean[])
```
Efficient initialisation of R vector values from an array representation in the CLR


### Properties

#### DataSize
Gets the size of a Boolean value in byte.
#### Item
Gets or sets the element at the specified index.
