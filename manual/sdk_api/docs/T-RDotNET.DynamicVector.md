---
title: DynamicVector
---

# DynamicVector
_namespace: [RDotNET](N-RDotNET.html)_

A collection of values.

> 
>  This vector cannot contain more than one types of values.
>  Consider to use another vector class instead.
>  


### Methods

#### #ctor
```csharp
RDotNET.DynamicVector.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a container for a collection of values

|Parameter Name|Remarks|
|--------------|-------|
|engine|The R engine|
|coerced|Pointer to the native R object, coerced to the appropriate type|


#### GetArrayFast
```csharp
RDotNET.DynamicVector.GetArrayFast
```
Gets an array representation of a vector in R. Note that the implementation cannot be particularly "fast" in spite of the name.

#### SetVectorDirect
```csharp
RDotNET.DynamicVector.SetVectorDirect(System.Object[])
```
Efficient initialisation of R vector values from an array representation in the CLR


### Properties

#### DataSize
Gets the data size of each element in this vector, i.e. the offset in memory between elements.
#### Item
Gets or sets the element at the specified index.
