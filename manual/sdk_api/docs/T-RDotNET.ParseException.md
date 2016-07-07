---
title: ParseException
---

# ParseException
_namespace: [RDotNET](N-RDotNET.html)_

Thrown when an engine comes to an error.



### Methods

#### #ctor
```csharp
RDotNET.ParseException.#ctor(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)
```
Creates a new ParseException

|Parameter Name|Remarks|
|--------------|-------|
|info|The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.|
|context|-|


#### GetObjectData
```csharp
RDotNET.ParseException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)
```
Sets the serialization info about the exception thrown

|Parameter Name|Remarks|
|--------------|-------|
|info|Serialized object data.|
|context|Contextual information about the source or destination|



### Properties

#### ErrorStatement
The statement caused the error.
#### Status
The error.
