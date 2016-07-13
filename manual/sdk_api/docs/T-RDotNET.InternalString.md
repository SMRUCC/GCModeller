---
title: InternalString
---

# InternalString
_namespace: [RDotNET](N-RDotNET.html)_

Internal string.



### Methods

#### #ctor
```csharp
RDotNET.InternalString.#ctor(RDotNET.REngine,System.String)
```
Creates a new instance.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|s|The string|


#### GetInternalValue
```csharp
RDotNET.InternalString.GetInternalValue
```
Gets the string representation of the string object.
 This returns null if the value is NA, whereas @"M:RDotNET.InternalString.ToString" returns "NA".
_returns: The string representation._

#### op_Implicit
```csharp
RDotNET.InternalString.op_Implicit(RDotNET.InternalString)~System.String
```
Converts to the string into .NET Framework string.

|Parameter Name|Remarks|
|--------------|-------|
|s|The R string.|

_returns: The .NET Framework string._

#### ToString
```csharp
RDotNET.InternalString.ToString
```
Gets the string representation of the string object.
 This returns "NA" if the value is NA, whereas @"M:RDotNET.InternalString.GetInternalValue" returns null.
_returns: The string representation._


