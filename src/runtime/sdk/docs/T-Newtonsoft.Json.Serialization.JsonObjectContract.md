---
title: JsonObjectContract
---

# JsonObjectContract
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Contract details for a @"T:System.Type" used by the @"T:Newtonsoft.Json.JsonSerializer".



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Serialization.JsonObjectContract.#ctor(System.Type)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Serialization.JsonObjectContract" class.

|Parameter Name|Remarks|
|--------------|-------|
|underlyingType|The underlying type for the contract.|



### Properties

#### ConstructorParameters
Gets the constructor parameters required for any non-default constructor
#### CreatorParameters
Gets a collection of @"T:Newtonsoft.Json.Serialization.JsonProperty" instances that define the parameters used with @"P:Newtonsoft.Json.Serialization.JsonObjectContract.OverrideCreator".
#### ExtensionDataGetter
Gets or sets the extension data getter.
#### ExtensionDataSetter
Gets or sets the extension data setter.
#### ExtensionDataValueType
Gets or sets the extension data value type.
#### ItemRequired
Gets or sets a value that indicates whether the object's properties are required.
#### MemberSerialization
Gets or sets the object member serialization.
#### OverrideConstructor
Gets or sets the override constructor used to create the object.
 This is set when a constructor is marked up using the
 JsonConstructor attribute.
#### OverrideCreator
Gets or sets the function used to create the object. When set this function will override @"P:Newtonsoft.Json.Serialization.JsonContract.DefaultCreator".
 This function is called with a collection of arguments which are defined by the @"P:Newtonsoft.Json.Serialization.JsonObjectContract.CreatorParameters" collection.
#### ParametrizedConstructor
Gets or sets the parametrized constructor used to create the object.
#### Properties
Gets the object's properties.
