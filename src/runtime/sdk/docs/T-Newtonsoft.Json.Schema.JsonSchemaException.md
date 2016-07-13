---
title: JsonSchemaException
---

# JsonSchemaException
_namespace: [Newtonsoft.Json.Schema](N-Newtonsoft.Json.Schema.html)_

Returns detailed information about the schema exception.
 
 JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Schema.JsonSchemaException.#ctor(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Schema.JsonSchemaException" class.

|Parameter Name|Remarks|
|--------------|-------|
|info|The @"T:System.Runtime.Serialization.SerializationInfo" that holds the serialized object data about the exception being thrown.|
|context|The @"T:System.Runtime.Serialization.StreamingContext" that contains contextual information about the source or destination.|



### Properties

#### LineNumber
Gets the line number indicating where the error occurred.
#### LinePosition
Gets the line position indicating where the error occurred.
#### Path
Gets the path to the JSON where the error occurred.
