---
title: JTokenWriter
---

# JTokenWriter
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Represents a writer that provides a fast, non-cached, forward-only way of generating JSON data.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Linq.JTokenWriter.#ctor
```
Initializes a new instance of the @"T:Newtonsoft.Json.Linq.JTokenWriter" class.

#### Close
```csharp
Newtonsoft.Json.Linq.JTokenWriter.Close
```
Closes this stream and the underlying stream.

#### Flush
```csharp
Newtonsoft.Json.Linq.JTokenWriter.Flush
```
Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.

#### WriteComment
```csharp
Newtonsoft.Json.Linq.JTokenWriter.WriteComment(System.String)
```
Writes out a comment '/*...*/' containing the specified text.

|Parameter Name|Remarks|
|--------------|-------|
|text|Text to place inside the comment.|


#### WriteEnd
```csharp
Newtonsoft.Json.Linq.JTokenWriter.WriteEnd(Newtonsoft.Json.JsonToken)
```
Writes the end.

|Parameter Name|Remarks|
|--------------|-------|
|token|The token.|


#### WriteNull
```csharp
Newtonsoft.Json.Linq.JTokenWriter.WriteNull
```
Writes a null value.

#### WritePropertyName
```csharp
Newtonsoft.Json.Linq.JTokenWriter.WritePropertyName(System.String)
```
Writes the property name of a name/value pair on a JSON object.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name of the property.|


#### WriteRaw
```csharp
Newtonsoft.Json.Linq.JTokenWriter.WriteRaw(System.String)
```
Writes raw JSON.

|Parameter Name|Remarks|
|--------------|-------|
|json|The raw JSON to write.|


#### WriteStartArray
```csharp
Newtonsoft.Json.Linq.JTokenWriter.WriteStartArray
```
Writes the beginning of a JSON array.

#### WriteStartConstructor
```csharp
Newtonsoft.Json.Linq.JTokenWriter.WriteStartConstructor(System.String)
```
Writes the start of a constructor with the given name.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name of the constructor.|


#### WriteStartObject
```csharp
Newtonsoft.Json.Linq.JTokenWriter.WriteStartObject
```
Writes the beginning of a JSON object.

#### WriteUndefined
```csharp
Newtonsoft.Json.Linq.JTokenWriter.WriteUndefined
```
Writes an undefined value.

#### WriteValue
```csharp
Newtonsoft.Json.Linq.JTokenWriter.WriteValue(System.Uri)
```
Writes a @"T:System.Uri" value.

|Parameter Name|Remarks|
|--------------|-------|
|value|The @"T:System.Uri" value to write.|



### Properties

#### CurrentToken
Gets the @"T:Newtonsoft.Json.Linq.JToken" at the writer's current position.
#### Token
Gets the token being writen.
