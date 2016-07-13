---
title: JConstructor
---

# JConstructor
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Represents a JSON constructor.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Linq.JConstructor.#ctor(System.String)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Linq.JConstructor" class with the specified name.

|Parameter Name|Remarks|
|--------------|-------|
|name|The constructor name.|


#### Load
```csharp
Newtonsoft.Json.Linq.JConstructor.Load(Newtonsoft.Json.JsonReader,Newtonsoft.Json.Linq.JsonLoadSettings)
```
Loads an @"T:Newtonsoft.Json.Linq.JConstructor" from a @"T:Newtonsoft.Json.JsonReader".

|Parameter Name|Remarks|
|--------------|-------|
|reader|A @"T:Newtonsoft.Json.JsonReader" that will be read for the content of the @"T:Newtonsoft.Json.Linq.JConstructor".|
|settings|The @"T:Newtonsoft.Json.Linq.JsonLoadSettings" used to load the JSON.
            If this is null, default load settings will be used.|

_returns: A @"T:Newtonsoft.Json.Linq.JConstructor" that contains the JSON that was read from the specified @"T:Newtonsoft.Json.JsonReader"._

#### WriteTo
```csharp
Newtonsoft.Json.Linq.JConstructor.WriteTo(Newtonsoft.Json.JsonWriter,Newtonsoft.Json.JsonConverter[])
```
Writes this token to a @"T:Newtonsoft.Json.JsonWriter".

|Parameter Name|Remarks|
|--------------|-------|
|writer|A @"T:Newtonsoft.Json.JsonWriter" into which this method will write.|
|converters|A collection of @"T:Newtonsoft.Json.JsonConverter" which will be used when writing the token.|



### Properties

#### ChildrenTokens
Gets the container's children tokens.
#### Item
Gets the @"T:Newtonsoft.Json.Linq.JToken" with the specified key.
#### Name
Gets or sets the name of this constructor.
#### Type
Gets the node type for this @"T:Newtonsoft.Json.Linq.JToken".
