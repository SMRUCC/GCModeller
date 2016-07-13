---
title: Factor
---

# Factor
_namespace: [RDotNET](N-RDotNET.html)_

Represents factors.



### Methods

#### #ctor
```csharp
RDotNET.Factor.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a factor vector.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a factor vector.|


#### GetFactor
```csharp
RDotNET.Factor.GetFactor(System.Int32)
```
Gets the value of the vector of factors at an index

|Parameter Name|Remarks|
|--------------|-------|
|index|the zero-based index of the vector|

_returns: The string representation of the factor, or a null reference if the value in R is NA_

#### GetFactors
```csharp
RDotNET.Factor.GetFactors
```
Gets the levels of the factor.
_returns: Factors._

#### GetFactors``1
```csharp
RDotNET.Factor.GetFactors``1(System.Boolean)
```
Gets the levels of the factor as the specific enum type.

|Parameter Name|Remarks|
|--------------|-------|
|ignoreCase|The value indicating case-sensitivity.|

_returns: Factors._
> 
>  Be careful to the underlying values.
>  You had better set levels and labels argument explicitly.
>  

#### GetLevels
```csharp
RDotNET.Factor.GetLevels
```
Gets the levels of the factor.

#### SetFactor
```csharp
RDotNET.Factor.SetFactor(System.Int32,System.String)
```
Sets the value of a factor vector at an index

|Parameter Name|Remarks|
|--------------|-------|
|index|the zero-based index item to set in the vector|
|factorValue|The value of the factor - can be a null reference|



### Properties

#### IsOrdered
Gets the value which indicating the factor is ordered or not.
