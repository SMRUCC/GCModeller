---
title: IsoDateTimeConverter
---

# IsoDateTimeConverter
_namespace: [Newtonsoft.Json.Converters](N-Newtonsoft.Json.Converters.html)_

Converts a @"T:System.DateTime" to and from the ISO 8601 date format (e.g. 2008-04-12T12:53Z).



### Methods

#### ReadJson
```csharp
Newtonsoft.Json.Converters.IsoDateTimeConverter.ReadJson(Newtonsoft.Json.JsonReader,System.Type,System.Object,Newtonsoft.Json.JsonSerializer)
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
Newtonsoft.Json.Converters.IsoDateTimeConverter.WriteJson(Newtonsoft.Json.JsonWriter,System.Object,Newtonsoft.Json.JsonSerializer)
```
Writes the JSON representation of the object.

|Parameter Name|Remarks|
|--------------|-------|
|writer|The @"T:Newtonsoft.Json.JsonWriter" to write to.|
|value|The value.|
|serializer|The calling serializer.|



### Properties

#### Culture
Gets or sets the culture used when converting a date to and from JSON.
#### DateTimeFormat
Gets or sets the date time format used when converting a date to and from JSON.
#### DateTimeStyles
Gets or sets the date time styles used when converting a date to and from JSON.
