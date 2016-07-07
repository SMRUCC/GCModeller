---
title: JsonArrayContract
---

# JsonArrayContract
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Contract details for a @"T:System.Type" used by the @"T:Newtonsoft.Json.JsonSerializer".



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Serialization.JsonArrayContract.#ctor(System.Type)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Serialization.JsonArrayContract" class.

|Parameter Name|Remarks|
|--------------|-------|
|underlyingType|The underlying type for the contract.|



### Properties

#### CollectionItemType
Gets the @"T:System.Type" of the collection items.
#### HasParameterizedCreator
Gets a value indicating whether the creator has a parameter with the collection values.
#### IsMultidimensionalArray
Gets a value indicating whether the collection type is a multidimensional array.
#### OverrideCreator
Gets or sets the function used to create the object. When set this function will override @"P:Newtonsoft.Json.Serialization.JsonContract.DefaultCreator".
