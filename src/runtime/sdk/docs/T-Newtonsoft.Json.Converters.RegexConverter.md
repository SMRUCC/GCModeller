---
title: RegexConverter
---

# RegexConverter
_namespace: [Newtonsoft.Json.Converters](N-Newtonsoft.Json.Converters.html)_

Converts a @"T:System.Text.RegularExpressions.Regex" to and from JSON and BSON.



### Methods

#### CanConvert
```csharp
Newtonsoft.Json.Converters.RegexConverter.CanConvert(System.Type)
```
Determines whether this instance can convert the specified object type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: true if this instance can convert the specified object type; otherwise, false.
            _

#### ReadJson
```csharp
Newtonsoft.Json.Converters.RegexConverter.ReadJson(Newtonsoft.Json.JsonReader,System.Type,System.Object,Newtonsoft.Json.JsonSerializer)
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
Newtonsoft.Json.Converters.RegexConverter.WriteJson(Newtonsoft.Json.JsonWriter,System.Object,Newtonsoft.Json.JsonSerializer)
```
Writes the JSON representation of the object.

|Parameter Name|Remarks|
|--------------|-------|
|writer|The @"T:Newtonsoft.Json.JsonWriter" to write to.|
|value|The value.|
|serializer|The calling serializer.|



