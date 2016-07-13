---
title: XmlNodeConverter
---

# XmlNodeConverter
_namespace: [Newtonsoft.Json.Converters](N-Newtonsoft.Json.Converters.html)_

Converts XML to and from JSON.



### Methods

#### CanConvert
```csharp
Newtonsoft.Json.Converters.XmlNodeConverter.CanConvert(System.Type)
```
Determines whether this instance can convert the specified value type.

|Parameter Name|Remarks|
|--------------|-------|
|valueType|Type of the value.|

_returns: true if this instance can convert the specified value type; otherwise, false.
            _

#### IsNamespaceAttribute
```csharp
Newtonsoft.Json.Converters.XmlNodeConverter.IsNamespaceAttribute(System.String,System.String@)
```
Checks if the attributeName is a namespace attribute.

|Parameter Name|Remarks|
|--------------|-------|
|attributeName|Attribute name to test.|
|prefix|The attribute name prefix if it has one, otherwise an empty string.|

_returns: true if attribute name is for a namespace attribute, otherwise false._

#### ReadJson
```csharp
Newtonsoft.Json.Converters.XmlNodeConverter.ReadJson(Newtonsoft.Json.JsonReader,System.Type,System.Object,Newtonsoft.Json.JsonSerializer)
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
Newtonsoft.Json.Converters.XmlNodeConverter.WriteJson(Newtonsoft.Json.JsonWriter,System.Object,Newtonsoft.Json.JsonSerializer)
```
Writes the JSON representation of the object.

|Parameter Name|Remarks|
|--------------|-------|
|writer|The @"T:Newtonsoft.Json.JsonWriter" to write to.|
|serializer|The calling serializer.|
|value|The value.|



### Properties

#### DeserializeRootElementName
Gets or sets the name of the root element to insert when deserializing to XML if the JSON structure has produces multiple root elements.
#### OmitRootObject
Gets or sets a value indicating whether to write the root JSON object.
#### WriteArrayAttribute
Gets or sets a flag to indicate whether to write the Json.NET array attribute.
 This attribute helps preserve arrays when converting the written XML back to JSON.
