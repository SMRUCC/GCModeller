---
title: ExpressionVector
---

# ExpressionVector
_namespace: [RDotNET](N-RDotNET.html)_

A vector of S expressions



### Methods

#### #ctor
```csharp
RDotNET.ExpressionVector.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for an expression vector.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to an expression vector.|


#### GetArrayFast
```csharp
RDotNET.ExpressionVector.GetArrayFast
```
Gets an array representation of a vector of SEXP in R. Note that the implementation cannot be particularly "fast" in spite of the name.

#### SetVectorDirect
```csharp
RDotNET.ExpressionVector.SetVectorDirect(RDotNET.Expression[])
```
Efficient initialisation of R vector values from an array representation in the CLR


### Properties

#### DataSize
Gets the size of a pointer in byte.
#### Item
Gets/sets the expression for an index
