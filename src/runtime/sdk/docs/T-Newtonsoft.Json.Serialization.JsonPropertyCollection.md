---
title: JsonPropertyCollection
---

# JsonPropertyCollection
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

A collection of @"T:Newtonsoft.Json.Serialization.JsonProperty" objects.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Serialization.JsonPropertyCollection.#ctor(System.Type)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Serialization.JsonPropertyCollection" class.

|Parameter Name|Remarks|
|--------------|-------|
|type|The type.|


#### AddProperty
```csharp
Newtonsoft.Json.Serialization.JsonPropertyCollection.AddProperty(Newtonsoft.Json.Serialization.JsonProperty)
```
Adds a @"T:Newtonsoft.Json.Serialization.JsonProperty" object.

|Parameter Name|Remarks|
|--------------|-------|
|property|The property to add to the collection.|


#### GetClosestMatchProperty
```csharp
Newtonsoft.Json.Serialization.JsonPropertyCollection.GetClosestMatchProperty(System.String)
```
Gets the closest matching @"T:Newtonsoft.Json.Serialization.JsonProperty" object.
 First attempts to get an exact case match of propertyName and then
 a case insensitive match.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|Name of the property.|

_returns: A matching property if found._

#### GetKeyForItem
```csharp
Newtonsoft.Json.Serialization.JsonPropertyCollection.GetKeyForItem(Newtonsoft.Json.Serialization.JsonProperty)
```
When implemented in a derived class, extracts the key from the specified element.

|Parameter Name|Remarks|
|--------------|-------|
|item|The element from which to extract the key.|

_returns: The key for the specified element._

#### GetProperty
```csharp
Newtonsoft.Json.Serialization.JsonPropertyCollection.GetProperty(System.String,System.StringComparison)
```
Gets a property by property name.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|The name of the property to get.|
|comparisonType|Type property name string comparison.|

_returns: A matching property if found._


