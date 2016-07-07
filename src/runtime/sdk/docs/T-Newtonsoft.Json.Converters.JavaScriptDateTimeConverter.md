---
title: JavaScriptDateTimeConverter
---

# JavaScriptDateTimeConverter
_namespace: [Newtonsoft.Json.Converters](N-Newtonsoft.Json.Converters.html)_

Converts a @"T:System.DateTime" to and from a JavaScript date constructor (e.g. new Date(52231943)).



### Methods

#### ReadJson
```csharp
Newtonsoft.Json.Converters.JavaScriptDateTimeConverter.ReadJson(Newtonsoft.Json.JsonReader,System.Type,System.Object,Newtonsoft.Json.JsonSerializer)
```
Reads the JSON representation of the object.

|Parameter Name|Remarks|
|--------------|-------|
|reader|The @"T:Newtonsoft.Json.JsonReader" to read from.|
|objectType|Type of the object.|
|existingValue|The existing property value of the JSON that is being converted.|
|serializer|The calling serializer.|

_returns: The object value._

#### WriteJson
```csharp
Newtonsoft.Json.Converters.JavaScriptDateTimeConverter.WriteJson(Newtonsoft.Json.JsonWriter,System.Object,Newtonsoft.Json.JsonSerializer)
```
Writes the JSON representation of the object.

|Parameter Name|Remarks|
|--------------|-------|
|writer|The @"T:Newtonsoft.Json.JsonWriter" to write to.|
|value|The value.|
|serializer|The calling serializer.|



