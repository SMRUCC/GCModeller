---
title: GenericVector
---

# GenericVector
_namespace: [RDotNET](N-RDotNET.html)_

A generic list. This is also known as list in R.



### Methods

#### #ctor
```csharp
RDotNET.GenericVector.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a list.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a list.|


#### GetArrayFast
```csharp
RDotNET.GenericVector.GetArrayFast
```
Efficient conversion from R vector representation to the array equivalent in the CLR
_returns: Array equivalent_

#### GetMetaObject
```csharp
RDotNET.GenericVector.GetMetaObject(System.Linq.Expressions.Expression)
```
returns a new ListDynamicMeta for this Generic Vector

|Parameter Name|Remarks|
|--------------|-------|
|parameter|-|


#### SetVectorDirect
```csharp
RDotNET.GenericVector.SetVectorDirect(RDotNET.SymbolicExpression[])
```
Efficient initialisation of R vector values from an array representation in the CLR

#### ToPairlist
```csharp
RDotNET.GenericVector.ToPairlist
```
Converts into a @"T:RDotNET.Pairlist".
_returns: The pairlist._


### Properties

#### DataSize
Gets the size of each item in this vector
#### Item
Gets or sets the element at the specified index.
