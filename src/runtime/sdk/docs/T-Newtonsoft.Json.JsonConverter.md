---
title: JsonConverter
---

# JsonConverter
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Converts an object to and from JSON.



### Methods

#### CanConvert
```csharp
Newtonsoft.Json.JsonConverter.CanConvert(System.Type)
```
Determines whether this instance can convert the specified object type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: true if this instance can convert the specified object type; otherwise, false.
            _

#### GetSchema
```csharp
Newtonsoft.Json.JsonConverter.GetSchema
```
Gets the @"T:Newtonsoft.Json.Schema.JsonSchema" of the JSON produced by the JsonConverter.
 
 JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.
_returns: The @"T:Newtonsoft.Json.Schema.JsonSchema" of the JSON produced by the JsonConverter._

#### ReadJson
```csharp
Newtonsoft.Json.JsonConverter.ReadJson(Newtonsoft.Json.JsonReader,System.Type,System.Object,Newtonsoft.Json.JsonSerializer)
```
Reads the JSON representation of the object.

|Parameter Name|Remarks|
|--------------|-------|
|reader|The @"T:Newtonsoft.Json.JsonReader" to read from.|
|objectType|Type of the object.|
|existingValue|The existing value of object being read.|
|serializer|The calling serializer.|

_returns: The object value._

#### WriteJson
```csharp
Newtonsoft.Json.JsonConverter.WriteJson(Newtonsoft.Json.JsonWriter,System.Object,Newtonsoft.Json.JsonSerializer)
```
Writes the JSON representation of the object.

|Parameter Name|Remarks|
|--------------|-------|
|writer|The @"T:Newtonsoft.Json.JsonWriter" to write to.|
|value|The value.|
|serializer|The calling serializer.|



### Properties

#### CanRead
Gets a value indicating whether this @"T:Newtonsoft.Json.JsonConverter" can read JSON.
#### CanWrite
Gets a value indicating whether this @"T:Newtonsoft.Json.JsonConverter" can write JSON.
