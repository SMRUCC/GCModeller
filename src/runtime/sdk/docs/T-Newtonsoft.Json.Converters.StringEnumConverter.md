---
title: StringEnumConverter
---

# StringEnumConverter
_namespace: [Newtonsoft.Json.Converters](N-Newtonsoft.Json.Converters.html)_

Converts an @"T:System.Enum" to and from its name string value.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Converters.StringEnumConverter.#ctor(System.Boolean)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Converters.StringEnumConverter" class.

|Parameter Name|Remarks|
|--------------|-------|
|camelCaseText|true if the written enum text will be camel case; otherwise, false.|


#### CanConvert
```csharp
Newtonsoft.Json.Converters.StringEnumConverter.CanConvert(System.Type)
```
Determines whether this instance can convert the specified object type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: true if this instance can convert the specified object type; otherwise, false.
            _

#### ReadJson
```csharp
Newtonsoft.Json.Converters.StringEnumConverter.ReadJson(Newtonsoft.Json.JsonReader,System.Type,System.Object,Newtonsoft.Json.JsonSerializer)
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
Newtonsoft.Json.Converters.StringEnumConverter.WriteJson(Newtonsoft.Json.JsonWriter,System.Object,Newtonsoft.Json.JsonSerializer)
```
Writes the JSON representation of the object.

|Parameter Name|Remarks|
|--------------|-------|
|writer|The @"T:Newtonsoft.Json.JsonWriter" to write to.|
|value|The value.|
|serializer|The calling serializer.|



### Properties

#### AllowIntegerValues
Gets or sets a value indicating whether integer values are allowed.
#### CamelCaseText
Gets or sets a value indicating whether the written enum text should be camel case.
