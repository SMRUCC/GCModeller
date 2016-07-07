---
title: CustomCreationConverter`1
---

# CustomCreationConverter`1
_namespace: [Newtonsoft.Json.Converters](N-Newtonsoft.Json.Converters.html)_

Create a custom object



### Methods

#### CanConvert
```csharp
Newtonsoft.Json.Converters.CustomCreationConverter`1.CanConvert(System.Type)
```
Determines whether this instance can convert the specified object type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: true if this instance can convert the specified object type; otherwise, false.
            _

#### Create
```csharp
Newtonsoft.Json.Converters.CustomCreationConverter`1.Create(System.Type)
```
Creates an object which will then be populated by the serializer.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: The created object._

#### ReadJson
```csharp
Newtonsoft.Json.Converters.CustomCreationConverter`1.ReadJson(Newtonsoft.Json.JsonReader,System.Type,System.Object,Newtonsoft.Json.JsonSerializer)
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
Newtonsoft.Json.Converters.CustomCreationConverter`1.WriteJson(Newtonsoft.Json.JsonWriter,System.Object,Newtonsoft.Json.JsonSerializer)
```
Writes the JSON representation of the object.

|Parameter Name|Remarks|
|--------------|-------|
|writer|The @"T:Newtonsoft.Json.JsonWriter" to write to.|
|value|The value.|
|serializer|The calling serializer.|



### Properties

#### CanWrite
Gets a value indicating whether this @"T:Newtonsoft.Json.JsonConverter" can write JSON.
