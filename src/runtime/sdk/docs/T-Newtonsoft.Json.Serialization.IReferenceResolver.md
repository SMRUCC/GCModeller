---
title: IReferenceResolver
---

# IReferenceResolver
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Used to resolve references when serializing and deserializing JSON by the @"T:Newtonsoft.Json.JsonSerializer".



### Methods

#### AddReference
```csharp
Newtonsoft.Json.Serialization.IReferenceResolver.AddReference(System.Object,System.String,System.Object)
```
Adds a reference to the specified object.

|Parameter Name|Remarks|
|--------------|-------|
|context|The serialization context.|
|reference|The reference.|
|value|The object to reference.|


#### GetReference
```csharp
Newtonsoft.Json.Serialization.IReferenceResolver.GetReference(System.Object,System.Object)
```
Gets the reference for the sepecified object.

|Parameter Name|Remarks|
|--------------|-------|
|context|The serialization context.|
|value|The object to get a reference for.|

_returns: The reference to the object._

#### IsReferenced
```csharp
Newtonsoft.Json.Serialization.IReferenceResolver.IsReferenced(System.Object,System.Object)
```
Determines whether the specified object is referenced.

|Parameter Name|Remarks|
|--------------|-------|
|context|The serialization context.|
|value|The object to test for a reference.|

_returns: true if the specified object is referenced; otherwise, false.
            _

#### ResolveReference
```csharp
Newtonsoft.Json.Serialization.IReferenceResolver.ResolveReference(System.Object,System.String)
```
Resolves a reference to its object.

|Parameter Name|Remarks|
|--------------|-------|
|context|The serialization context.|
|reference|The reference to resolve.|

_returns: The object that_


