---
title: CharacterVector
---

# CharacterVector
_namespace: [RDotNET](N-RDotNET.html)_

A collection of strings.



### Methods

#### #ctor
```csharp
RDotNET.CharacterVector.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a string vector.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a string vector.|


#### GetArrayFast
```csharp
RDotNET.CharacterVector.GetArrayFast
```
Gets an array representation of this R vector. Note that the implementation is not as fast as for numeric vectors.

#### SetVectorDirect
```csharp
RDotNET.CharacterVector.SetVectorDirect(System.String[])
```
Efficient initialisation of R vector values from an array representation in the CLR


### Properties

#### DataSize
Gets the size of a pointer in byte.
#### Item
Gets or sets the element at the specified index.
