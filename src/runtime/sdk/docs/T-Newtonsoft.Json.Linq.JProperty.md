---
title: JProperty
---

# JProperty
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Represents a JSON property.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Linq.JProperty.#ctor(System.String,System.Object)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Linq.JProperty" class.

|Parameter Name|Remarks|
|--------------|-------|
|name|The property name.|
|content|The property content.|


#### Load
```csharp
Newtonsoft.Json.Linq.JProperty.Load(Newtonsoft.Json.JsonReader,Newtonsoft.Json.Linq.JsonLoadSettings)
```
Loads an @"T:Newtonsoft.Json.Linq.JProperty" from a @"T:Newtonsoft.Json.JsonReader".

|Parameter Name|Remarks|
|--------------|-------|
|reader|A @"T:Newtonsoft.Json.JsonReader" that will be read for the content of the @"T:Newtonsoft.Json.Linq.JProperty".|
|settings|The @"T:Newtonsoft.Json.Linq.JsonLoadSettings" used to load the JSON.
            If this is null, default load settings will be used.|

_returns: A @"T:Newtonsoft.Json.Linq.JProperty" that contains the JSON that was read from the specified @"T:Newtonsoft.Json.JsonReader"._

#### WriteTo
```csharp
Newtonsoft.Json.Linq.JProperty.WriteTo(Newtonsoft.Json.JsonWriter,Newtonsoft.Json.JsonConverter[])
```
Writes this token to a @"T:Newtonsoft.Json.JsonWriter".

|Parameter Name|Remarks|
|--------------|-------|
|writer|A @"T:Newtonsoft.Json.JsonWriter" into which this method will write.|
|converters|A collection of @"T:Newtonsoft.Json.JsonConverter" which will be used when writing the token.|



### Properties

#### ChildrenTokens
Gets the container's children tokens.
#### Name
Gets the property name.
#### Type
Gets the node type for this @"T:Newtonsoft.Json.Linq.JToken".
#### Value
Gets or sets the property value.
