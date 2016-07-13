---
title: JsonTextWriter
---

# JsonTextWriter
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Represents a writer that provides a fast, non-cached, forward-only way of generating JSON data.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonTextWriter.#ctor(System.IO.TextWriter)
```
Creates an instance of the JsonWriter class using the specified @"T:System.IO.TextWriter".

|Parameter Name|Remarks|
|--------------|-------|
|textWriter|The TextWriter to write to.|


#### Close
```csharp
Newtonsoft.Json.JsonTextWriter.Close
```
Closes this stream and the underlying stream.

#### Flush
```csharp
Newtonsoft.Json.JsonTextWriter.Flush
```
Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.

#### WriteComment
```csharp
Newtonsoft.Json.JsonTextWriter.WriteComment(System.String)
```
Writes out a comment '/*...*/' containing the specified text.

|Parameter Name|Remarks|
|--------------|-------|
|text|Text to place inside the comment.|


#### WriteEnd
```csharp
Newtonsoft.Json.JsonTextWriter.WriteEnd(Newtonsoft.Json.JsonToken)
```
Writes the specified end token.

|Parameter Name|Remarks|
|--------------|-------|
|token|The end token to write.|


#### WriteIndent
```csharp
Newtonsoft.Json.JsonTextWriter.WriteIndent
```
Writes indent characters.

#### WriteIndentSpace
```csharp
Newtonsoft.Json.JsonTextWriter.WriteIndentSpace
```
Writes an indent space.

#### WriteNull
```csharp
Newtonsoft.Json.JsonTextWriter.WriteNull
```
Writes a null value.

#### WritePropertyName
```csharp
Newtonsoft.Json.JsonTextWriter.WritePropertyName(System.String,System.Boolean)
```
Writes the property name of a name/value pair on a JSON object.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name of the property.|
|escape|A flag to indicate whether the text should be escaped when it is written as a JSON property name.|


#### WriteRaw
```csharp
Newtonsoft.Json.JsonTextWriter.WriteRaw(System.String)
```
Writes raw JSON.

|Parameter Name|Remarks|
|--------------|-------|
|json|The raw JSON to write.|


#### WriteStartArray
```csharp
Newtonsoft.Json.JsonTextWriter.WriteStartArray
```
Writes the beginning of a JSON array.

#### WriteStartConstructor
```csharp
Newtonsoft.Json.JsonTextWriter.WriteStartConstructor(System.String)
```
Writes the start of a constructor with the given name.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name of the constructor.|


#### WriteStartObject
```csharp
Newtonsoft.Json.JsonTextWriter.WriteStartObject
```
Writes the beginning of a JSON object.

#### WriteUndefined
```csharp
Newtonsoft.Json.JsonTextWriter.WriteUndefined
```
Writes an undefined value.

#### WriteValue
```csharp
Newtonsoft.Json.JsonTextWriter.WriteValue(System.Uri)
```
Writes a @"T:System.Uri" value.

|Parameter Name|Remarks|
|--------------|-------|
|value|The @"T:System.Uri" value to write.|


#### WriteValueDelimiter
```csharp
Newtonsoft.Json.JsonTextWriter.WriteValueDelimiter
```
Writes the JSON value delimiter.

#### WriteWhitespace
```csharp
Newtonsoft.Json.JsonTextWriter.WriteWhitespace(System.String)
```
Writes out the given white space.

|Parameter Name|Remarks|
|--------------|-------|
|ws|The string of white space characters.|



### Properties

#### ArrayPool
Gets or sets the writer's character array pool.
#### Indentation
Gets or sets how many IndentChars to write for each level in the hierarchy when @"T:Newtonsoft.Json.Formatting" is set to Formatting.Indented.
#### IndentChar
Gets or sets which character to use for indenting when @"T:Newtonsoft.Json.Formatting" is set to Formatting.Indented.
#### QuoteChar
Gets or sets which character to use to quote attribute values.
#### QuoteName
Gets or sets a value indicating whether object names will be surrounded with quotes.
