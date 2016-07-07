---
title: Vector`1
---

# Vector`1
_namespace: [RDotNET](N-RDotNET.html)_

A vector base.



### Methods

#### #ctor
```csharp
RDotNET.Vector`1.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a vector.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a vector.|


#### CopyTo
```csharp
RDotNET.Vector`1.CopyTo(`0[],System.Int32,System.Int32,System.Int32)
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
RDotNET.Vector`1.GetArrayFast
```
Gets a representation as a one dimensional array of an R vector, with efficiency in mind for the unmanaged to managed transition, if possible.

#### GetEnumerator
```csharp
RDotNET.Vector`1.GetEnumerator
```
Gets enumerator

#### GetOffset
```csharp
RDotNET.Vector`1.GetOffset(System.Int32)
```
Gets the offset for the specified index.

|Parameter Name|Remarks|
|--------------|-------|
|index|The index.|

_returns: The offset._

#### SetVector
```csharp
RDotNET.Vector`1.SetVector(`0[])
```
Initializes the content of a vector with runtime speed in mind. This method protects the R vector, then call SetVectorDirect.

|Parameter Name|Remarks|
|--------------|-------|
|values|The values to put in the vector. Length must match exactly the vector size|


#### SetVectorDirect
```csharp
RDotNET.Vector`1.SetVectorDirect(`0[])
```
Initializes the content of a vector with runtime speed in mind. The vector must already be protected before calling this method.

|Parameter Name|Remarks|
|--------------|-------|
|values|The values to put in the vector. Length must match exactly the vector size|


#### ToArray
```csharp
RDotNET.Vector`1.ToArray
```
A method to transfer data from native to .NET managed array equivalents fast.
_returns: Array of values in the vector_


### Properties

#### DataPointer
Gets the pointer for the first element.
#### DataSize
Gets the size of an element in byte.
#### Item
Gets or sets the element at the specified name.
#### Length
Gets the number of elements.
#### Names
Gets the names of elements.
