---
title: JsonWriter
---

# JsonWriter
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Represents a writer that provides a fast, non-cached, forward-only way of generating JSON data.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonWriter.#ctor
```
Creates an instance of the JsonWriter class.

#### Close
```csharp
Newtonsoft.Json.JsonWriter.Close
```
Closes this stream and the underlying stream.

#### Dispose
```csharp
Newtonsoft.Json.JsonWriter.Dispose(System.Boolean)
```
Releases unmanaged and - optionally - managed resources

|Parameter Name|Remarks|
|--------------|-------|
|disposing|true to release both managed and unmanaged resources; false to release only unmanaged resources.|


#### Flush
```csharp
Newtonsoft.Json.JsonWriter.Flush
```
Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.

#### SetWriteState
```csharp
Newtonsoft.Json.JsonWriter.SetWriteState(Newtonsoft.Json.JsonToken,System.Object)
```
Sets the state of the JsonWriter,

|Parameter Name|Remarks|
|--------------|-------|
|token|The JsonToken being written.|
|value|The value being written.|


#### WriteComment
```csharp
Newtonsoft.Json.JsonWriter.WriteComment(System.String)
```
Writes out a comment '/*...*/' containing the specified text.

|Parameter Name|Remarks|
|--------------|-------|
|text|Text to place inside the comment.|


#### WriteEnd
```csharp
Newtonsoft.Json.JsonWriter.WriteEnd(Newtonsoft.Json.JsonToken)
```
Writes the specified end token.

|Parameter Name|Remarks|
|--------------|-------|
|token|The end token to write.|


#### WriteEndArray
```csharp
Newtonsoft.Json.JsonWriter.WriteEndArray
```
Writes the end of an array.

#### WriteEndConstructor
```csharp
Newtonsoft.Json.JsonWriter.WriteEndConstructor
```
Writes the end constructor.

#### WriteEndObject
```csharp
Newtonsoft.Json.JsonWriter.WriteEndObject
```
Writes the end of a JSON object.

#### WriteIndent
```csharp
Newtonsoft.Json.JsonWriter.WriteIndent
```
Writes indent characters.

#### WriteIndentSpace
```csharp
Newtonsoft.Json.JsonWriter.WriteIndentSpace
```
Writes an indent space.

#### WriteNull
```csharp
Newtonsoft.Json.JsonWriter.WriteNull
```
Writes a null value.

#### WritePropertyName
```csharp
Newtonsoft.Json.JsonWriter.WritePropertyName(System.String,System.Boolean)
```
Writes the property name of a name/value pair on a JSON object.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name of the property.|
|escape|A flag to indicate whether the text should be escaped when it is written as a JSON property name.|


#### WriteRaw
```csharp
Newtonsoft.Json.JsonWriter.WriteRaw(System.String)
```
Writes raw JSON without changing the writer's state.

|Parameter Name|Remarks|
|--------------|-------|
|json|The raw JSON to write.|


#### WriteRawValue
```csharp
Newtonsoft.Json.JsonWriter.WriteRawValue(System.String)
```
Writes raw JSON where a value is expected and updates the writer's state.

|Parameter Name|Remarks|
|--------------|-------|
|json|The raw JSON to write.|


#### WriteStartArray
```csharp
Newtonsoft.Json.JsonWriter.WriteStartArray
```
Writes the beginning of a JSON array.

#### WriteStartConstructor
```csharp
Newtonsoft.Json.JsonWriter.WriteStartConstructor(System.String)
```
Writes the start of a constructor with the given name.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name of the constructor.|


#### WriteStartObject
```csharp
Newtonsoft.Json.JsonWriter.WriteStartObject
```
Writes the beginning of a JSON object.

#### WriteToken
```csharp
Newtonsoft.Json.JsonWriter.WriteToken(Newtonsoft.Json.JsonToken)
```
Writes the @"T:Newtonsoft.Json.JsonToken" token.

|Parameter Name|Remarks|
|--------------|-------|
|token|The @"T:Newtonsoft.Json.JsonToken" to write.|


#### WriteUndefined
```csharp
Newtonsoft.Json.JsonWriter.WriteUndefined
```
Writes an undefined value.

#### WriteValue
```csharp
Newtonsoft.Json.JsonWriter.WriteValue(System.Object)
```
Writes a @"T:System.Object" value.
 An error will raised if the value cannot be written as a single JSON token.

|Parameter Name|Remarks|
|--------------|-------|
|value|The @"T:System.Object" value to write.|


#### WriteValueDelimiter
```csharp
Newtonsoft.Json.JsonWriter.WriteValueDelimiter
```
Writes the JSON value delimiter.

#### WriteWhitespace
```csharp
Newtonsoft.Json.JsonWriter.WriteWhitespace(System.String)
```
Writes out the given white space.

|Parameter Name|Remarks|
|--------------|-------|
|ws|The string of white space characters.|



### Properties

#### CloseOutput
Gets or sets a value indicating whether the underlying stream or
 @"T:System.IO.TextReader" should be closed when the writer is closed.
#### Culture
Gets or sets the culture used when writing JSON. Defaults to @"P:System.Globalization.CultureInfo.InvariantCulture".
#### DateFormatHandling
Get or set how dates are written to JSON text.
#### DateFormatString
Get or set how @"T:System.DateTime" and @"T:System.DateTimeOffset" values are formatting when writing JSON text.
#### DateTimeZoneHandling
Get or set how @"T:System.DateTime" time zones are handling when writing JSON text.
#### FloatFormatHandling
Get or set how special floating point numbers, e.g. @"F:System.Double.NaN",
 @"F:System.Double.PositiveInfinity" and @"F:System.Double.NegativeInfinity",
 are written to JSON text.
#### Formatting
Indicates how JSON text output is formatted.
#### Path
Gets the path of the writer.
#### StringEscapeHandling
Get or set how strings are escaped when writing JSON text.
#### Top
Gets the top.
#### WriteState
Gets the state of the writer.
