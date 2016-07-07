---
title: BsonReader
---

# BsonReader
_namespace: [Newtonsoft.Json.Bson](N-Newtonsoft.Json.Bson.html)_

Represents a reader that provides fast, non-cached, forward-only access to serialized JSON data.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Bson.BsonReader.#ctor(System.IO.BinaryReader,System.Boolean,System.DateTimeKind)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Bson.BsonReader" class.

|Parameter Name|Remarks|
|--------------|-------|
|reader|The reader.|
|readRootValueAsArray|if set to true the root object will be read as a JSON array.|
|dateTimeKindHandling|The  used when reading @"T:System.DateTime" values from BSON.|


#### Close
```csharp
Newtonsoft.Json.Bson.BsonReader.Close
```
Changes the @"T:Newtonsoft.Json.JsonReader.State" to Closed.

#### Read
```csharp
Newtonsoft.Json.Bson.BsonReader.Read
```
Reads the next JSON token from the stream.
_returns: true if the next token was read successfully; false if there are no more tokens to read.
            _


### Properties

#### DateTimeKindHandling
Gets or sets the used when reading @"T:System.DateTime" values from BSON.
#### JsonNet35BinaryCompatibility
Gets or sets a value indicating whether binary data reading should compatible with incorrect Json.NET 3.5 written binary.
#### ReadRootValueAsArray
Gets or sets a value indicating whether the root object will be read as a JSON array.
