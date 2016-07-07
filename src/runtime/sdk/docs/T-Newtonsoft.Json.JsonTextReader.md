---
title: JsonTextReader
---

# JsonTextReader
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Represents a reader that provides fast, non-cached, forward-only access to JSON text data.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonTextReader.#ctor(System.IO.TextReader)
```
Initializes a new instance of the @"T:Newtonsoft.Json.JsonReader" class with the specified @"T:System.IO.TextReader".

|Parameter Name|Remarks|
|--------------|-------|
|reader|The TextReader containing the XML data to read.|


#### Close
```csharp
Newtonsoft.Json.JsonTextReader.Close
```
Changes the state to closed.

#### HasLineInfo
```csharp
Newtonsoft.Json.JsonTextReader.HasLineInfo
```
Gets a value indicating whether the class can return line information.
_returns: true if LineNumber and LinePosition can be provided; otherwise, false.
            _

#### Read
```csharp
Newtonsoft.Json.JsonTextReader.Read
```
Reads the next JSON token from the stream.
_returns: true if the next token was read successfully; false if there are no more tokens to read.
            _

#### ReadAsBoolean
```csharp
Newtonsoft.Json.JsonTextReader.ReadAsBoolean
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsBytes
```csharp
Newtonsoft.Json.JsonTextReader.ReadAsBytes
```
Reads the next JSON token from the stream as a @"T:System.Byte"[].
_returns: A @"T:System.Byte"[] or a null reference if the next JSON token is null. This method will return null at the end of an array._

#### ReadAsDateTime
```csharp
Newtonsoft.Json.JsonTextReader.ReadAsDateTime
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsDateTimeOffset
```csharp
Newtonsoft.Json.JsonTextReader.ReadAsDateTimeOffset
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsDecimal
```csharp
Newtonsoft.Json.JsonTextReader.ReadAsDecimal
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsDouble
```csharp
Newtonsoft.Json.JsonTextReader.ReadAsDouble
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsInt32
```csharp
Newtonsoft.Json.JsonTextReader.ReadAsInt32
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsString
```csharp
Newtonsoft.Json.JsonTextReader.ReadAsString
```
Reads the next JSON token from the stream as a @"T:System.String".
_returns: A @"T:System.String". This method will return null at the end of an array._


### Properties

#### ArrayPool
Gets or sets the reader's character buffer pool.
#### LineNumber
Gets the current line number.
#### LinePosition
Gets the current line position.
