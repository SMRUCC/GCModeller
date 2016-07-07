---
title: JsonWriterException
---

# JsonWriterException
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

The exception thrown when an error occurs while reading JSON text.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonWriterException.#ctor(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)
```
Initializes a new instance of the @"T:Newtonsoft.Json.JsonWriterException" class.

|Parameter Name|Remarks|
|--------------|-------|
|info|The @"T:System.Runtime.Serialization.SerializationInfo" that holds the serialized object data about the exception being thrown.|
|context|The @"T:System.Runtime.Serialization.StreamingContext" that contains contextual information about the source or destination.|



### Properties

#### Path
Gets the path to the JSON where the error occurred.
