---
title: JsonValidatingReader
---

# JsonValidatingReader
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Represents a reader that provides @"T:Newtonsoft.Json.Schema.JsonSchema" validation.
 
 JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonValidatingReader.#ctor(Newtonsoft.Json.JsonReader)
```
Initializes a new instance of the @"T:Newtonsoft.Json.JsonValidatingReader" class that
 validates the content returned from the given @"T:Newtonsoft.Json.JsonReader".

|Parameter Name|Remarks|
|--------------|-------|
|reader|The @"T:Newtonsoft.Json.JsonReader" to read from while validating.|


#### Read
```csharp
Newtonsoft.Json.JsonValidatingReader.Read
```
Reads the next JSON token from the stream.
_returns: true if the next token was read successfully; false if there are no more tokens to read.
            _

#### ReadAsBoolean
```csharp
Newtonsoft.Json.JsonValidatingReader.ReadAsBoolean
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1"._

#### ReadAsBytes
```csharp
Newtonsoft.Json.JsonValidatingReader.ReadAsBytes
```
Reads the next JSON token from the stream as a @"T:System.Byte"[].
_returns: 
            A @"T:System.Byte"[] or a null reference if the next JSON token is null.
            _

#### ReadAsDateTime
```csharp
Newtonsoft.Json.JsonValidatingReader.ReadAsDateTime
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1". This method will return null at the end of an array._

#### ReadAsDateTimeOffset
```csharp
Newtonsoft.Json.JsonValidatingReader.ReadAsDateTimeOffset
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1"._

#### ReadAsDecimal
```csharp
Newtonsoft.Json.JsonValidatingReader.ReadAsDecimal
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1"._

#### ReadAsDouble
```csharp
Newtonsoft.Json.JsonValidatingReader.ReadAsDouble
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1"._

#### ReadAsInt32
```csharp
Newtonsoft.Json.JsonValidatingReader.ReadAsInt32
```
Reads the next JSON token from the stream as a @"T:System.Nullable`1".
_returns: A @"T:System.Nullable`1"._

#### ReadAsString
```csharp
Newtonsoft.Json.JsonValidatingReader.ReadAsString
```
Reads the next JSON token from the stream as a @"T:System.String".
_returns: A @"T:System.String". This method will return null at the end of an array._


### Properties

#### Depth
Gets the depth of the current token in the JSON document.
#### Path
Gets the path of the current JSON token.
#### QuoteChar
Gets the quotation mark character used to enclose the value of a string.
#### Reader
Gets the @"T:Newtonsoft.Json.JsonReader" used to construct this @"T:Newtonsoft.Json.JsonValidatingReader".
#### Schema
Gets or sets the schema.
#### TokenType
Gets the type of the current JSON token.
#### Value
Gets the text value of the current JSON token.
#### ValueType
Gets the Common Language Runtime (CLR) type for the current JSON token.
