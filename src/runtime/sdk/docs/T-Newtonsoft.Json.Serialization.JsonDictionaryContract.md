---
title: JsonDictionaryContract
---

# JsonDictionaryContract
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Contract details for a @"T:System.Type" used by the @"T:Newtonsoft.Json.JsonSerializer".



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Serialization.JsonDictionaryContract.#ctor(System.Type)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Serialization.JsonDictionaryContract" class.

|Parameter Name|Remarks|
|--------------|-------|
|underlyingType|The underlying type for the contract.|



### Properties

#### DictionaryKeyResolver
Gets or sets the dictionary key resolver.
#### DictionaryKeyType
Gets the @"T:System.Type" of the dictionary keys.
#### DictionaryValueType
Gets the @"T:System.Type" of the dictionary values.
#### HasParameterizedCreator
Gets a value indicating whether the creator has a parameter with the dictionary values.
#### OverrideCreator
Gets or sets the function used to create the object. When set this function will override @"P:Newtonsoft.Json.Serialization.JsonContract.DefaultCreator".
#### PropertyNameResolver
Gets or sets the property name resolver.
