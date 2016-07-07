---
title: NamingStrategy
---

# NamingStrategy
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

A base class for resolving how property names and dictionary keys are serialized.



### Methods

#### GetDictionaryKey
```csharp
Newtonsoft.Json.Serialization.NamingStrategy.GetDictionaryKey(System.String)
```
Gets the serialized key for a given dictionary key.

|Parameter Name|Remarks|
|--------------|-------|
|key|The initial dictionary key.|

_returns: The serialized dictionary key._

#### GetPropertyName
```csharp
Newtonsoft.Json.Serialization.NamingStrategy.GetPropertyName(System.String,System.Boolean)
```
Gets the serialized name for a given property name.

|Parameter Name|Remarks|
|--------------|-------|
|name|The initial property name.|
|hasSpecifiedName|A flag indicating whether the property has had a name explicitly specfied.|

_returns: The serialized property name._

#### ResolvePropertyName
```csharp
Newtonsoft.Json.Serialization.NamingStrategy.ResolvePropertyName(System.String)
```
Resolves the specified property name.

|Parameter Name|Remarks|
|--------------|-------|
|name|The property name to resolve.|

_returns: The resolved property name._


### Properties

#### OverrideSpecifiedNames
A flag indicating whether explicitly specified property names,
 e.g. a property name customized with a @"T:Newtonsoft.Json.JsonPropertyAttribute", should be processed.
 Defaults to false.
#### ProcessDictionaryKeys
A flag indicating whether dictionary keys should be processed.
 Defaults to false.
