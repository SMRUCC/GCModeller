---
title: JsonReader
---

# JsonReader
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Represents a reader that provides fast, non-cached, forward-only access to serialized JSON data.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonReader.#ctor
```
Initializes a new instance of the @"T:Newtonsoft.Json.JsonReader" class with the specified @"T:System.IO.TextReader".

#### Close
```csharp
Newtonsoft.Json.JsonReader.Close
```
Changes the @"T:Newtonsoft.Json.JsonReader.State" to Closed.

#### Dispose
```csharp
Newtonsoft.Json.JsonReader.Dispose(System.Boolean)
```
Releases unmanaged and - optionally - managed resources

|Parameter Name|Remarks|
|--------------|-------|
|disposing|true to release both managed and unmanaged resources; false to release only unmanaged resources.|


#### Read
```csharp
Newtonsoft.Json.JsonReader.Read
```
Reads the next JSON token from the stream.
_returns: true if the next token was read successfully; false if there are no more tokens to read._

#### ReadAsBoolean
```csharp
Newtonsoft.Json.JsonReader.ReadAsBoolean
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsBytes
```csharp
Newtonsoft.Json.JsonReader.ReadAsBytes
```
Reads the next JSON token from the stream as a @"T:System.Byte"[].
_returns: A @"T:System.Byte"[] or a null reference if the next JSON token is null. This method will return null at the end of an array._

#### ReadAsDateTime
```csharp
Newtonsoft.Json.JsonReader.ReadAsDateTime
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsDateTimeOffset
```csharp
Newtonsoft.Json.JsonReader.ReadAsDateTimeOffset
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsDecimal
```csharp
Newtonsoft.Json.JsonReader.ReadAsDecimal
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsDouble
```csharp
Newtonsoft.Json.JsonReader.ReadAsDouble
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsInt32
```csharp
Newtonsoft.Json.JsonReader.ReadAsInt32
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsString
```csharp
Newtonsoft.Json.JsonReader.ReadAsString
```
Reads the next JSON token from the stream as a @"T:System.String".
_returns: A @"T:System.String". This method will return null at the end of an array._

#### SetStateBasedOnCurrent
```csharp
Newtonsoft.Json.JsonReader.SetStateBasedOnCurrent
```
Sets the state based on current token type.

#### SetToken
```csharp
Newtonsoft.Json.JsonReader.SetToken(Newtonsoft.Json.JsonToken,System.Object)
```
Sets the current token and value.

|Parameter Name|Remarks|
|--------------|-------|
|newToken|The new token.|
|value|The value.|


#### Skip
```csharp
Newtonsoft.Json.JsonReader.Skip
```
Skips the children of the current token.

#### System#IDisposable#Dispose
```csharp
Newtonsoft.Json.JsonReader.System#IDisposable#Dispose
```
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.


### Properties

#### CloseInput
Gets or sets a value indicating whether the underlying stream or
 @"T:System.IO.TextReader" should be closed when the reader is closed.
#### Culture
Gets or sets the culture used when reading JSON. Defaults to @"P:System.Globalization.CultureInfo.InvariantCulture".
#### CurrentState
Gets the current reader state.
#### DateFormatString
Get or set how custom date formatted strings are parsed when reading JSON.
#### DateParseHandling
Get or set how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON.
#### DateTimeZoneHandling
Get or set how @"T:System.DateTime" time zones are handling when reading JSON.
#### Depth
Gets the depth of the current token in the JSON document.
#### FloatParseHandling
Get or set how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
#### MaxDepth
Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a @"T:Newtonsoft.Json.JsonReaderException".
#### Path
Gets the path of the current JSON token.
#### QuoteChar
Gets the quotation mark character used to enclose the value of a string.
#### SupportMultipleContent
Gets or sets a value indicating whether multiple pieces of JSON content can
 be read from a continuous stream without erroring.
#### TokenType
Gets the type of the current JSON token.
#### Value
Gets the text value of the current JSON token.
#### ValueType
Gets The Common Language Runtime (CLR) type for the current JSON token.
