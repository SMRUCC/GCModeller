---
title: BsonWriter
---

# BsonWriter
_namespace: [Newtonsoft.Json.Bson](N-Newtonsoft.Json.Bson.html)_

Represents a writer that provides a fast, non-cached, forward-only way of generating JSON data.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Bson.BsonWriter.#ctor(System.IO.BinaryWriter)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Bson.BsonWriter" class.

|Parameter Name|Remarks|
|--------------|-------|
|writer|The writer.|


#### Close
```csharp
Newtonsoft.Json.Bson.BsonWriter.Close
```
Closes this stream and the underlying stream.

#### Flush
```csharp
Newtonsoft.Json.Bson.BsonWriter.Flush
```
Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.

#### WriteComment
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteComment(System.String)
```
Writes out a comment '/*...*/' containing the specified text.

|Parameter Name|Remarks|
|--------------|-------|
|text|Text to place inside the comment.|


#### WriteEnd
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteEnd(Newtonsoft.Json.JsonToken)
```
Writes the end.

|Parameter Name|Remarks|
|--------------|-------|
|token|The token.|


#### WriteNull
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteNull
```
Writes a null value.

#### WriteObjectId
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteObjectId(System.Byte[])
```
Writes a @"T:System.Byte"[] value that represents a BSON object id.

|Parameter Name|Remarks|
|--------------|-------|
|value|The Object ID value to write.|


#### WritePropertyName
```csharp
Newtonsoft.Json.Bson.BsonWriter.WritePropertyName(System.String)
```
Writes the property name of a name/value pair on a JSON object.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name of the property.|


#### WriteRaw
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteRaw(System.String)
```
Writes raw JSON.

|Parameter Name|Remarks|
|--------------|-------|
|json|The raw JSON to write.|


#### WriteRawValue
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteRawValue(System.String)
```
Writes raw JSON where a value is expected and updates the writer's state.

|Parameter Name|Remarks|
|--------------|-------|
|json|The raw JSON to write.|


#### WriteRegex
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteRegex(System.String,System.String)
```
Writes a BSON regex.

|Parameter Name|Remarks|
|--------------|-------|
|pattern|The regex pattern.|
|options|The regex options.|


#### WriteStartArray
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteStartArray
```
Writes the beginning of a JSON array.

#### WriteStartConstructor
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteStartConstructor(System.String)
```
Writes the start of a constructor with the given name.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name of the constructor.|


#### WriteStartObject
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteStartObject
```
Writes the beginning of a JSON object.

#### WriteUndefined
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteUndefined
```
Writes an undefined value.

#### WriteValue
```csharp
Newtonsoft.Json.Bson.BsonWriter.WriteValue(System.Uri)
```
Writes a @"T:System.Uri" value.

|Parameter Name|Remarks|
|--------------|-------|
|value|The @"T:System.Uri" value to write.|



### Properties

#### DateTimeKindHandling
Gets or sets the used when writing @"T:System.DateTime" values to BSON.
 When set to no conversion will occur.
