---
title: RawVector
---

# RawVector
_namespace: [RDotNET](N-RDotNET.html)_

A sequence of byte values.



### Methods

#### #ctor
```csharp
RDotNET.RawVector.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a raw vector.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a raw vector.|


#### CopyTo
```csharp
RDotNET.RawVector.CopyTo(System.Byte[],System.Int32,System.Int32,System.Int32)
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
RDotNET.RawVector.GetArrayFast
```
Efficient conversion from R vector representation to the array equivalent in the CLR
_returns: Array equivalent_

#### SetVectorDirect
```csharp
RDotNET.RawVector.SetVectorDirect(System.Byte[])
```
Sets the values of this RawVector

|Parameter Name|Remarks|
|--------------|-------|
|values|Managed values, to be converted to unmanaged equivalent|



### Properties

#### DataSize
Gets the size of a byte value in byte.
#### Item
Gets or sets the element at the specified index.
