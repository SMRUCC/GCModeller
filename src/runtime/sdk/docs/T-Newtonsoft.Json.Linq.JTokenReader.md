---
title: JTokenReader
---

# JTokenReader
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Represents a reader that provides fast, non-cached, forward-only access to serialized JSON data.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Linq.JTokenReader.#ctor(Newtonsoft.Json.Linq.JToken)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Linq.JTokenReader" class.

|Parameter Name|Remarks|
|--------------|-------|
|token|The token to read from.|


#### Read
```csharp
Newtonsoft.Json.Linq.JTokenReader.Read
```
Reads the next JSON token from the stream.
_returns: true if the next token was read successfully; false if there are no more tokens to read.
            _


### Properties

#### CurrentToken
Gets the @"T:Newtonsoft.Json.Linq.JToken" at the reader's current position.
#### Path
Gets the path of the current JSON token.
