---
title: ReflectionAttributeProvider
---

# ReflectionAttributeProvider
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Provides methods to get attributes from a @"T:System.Type", @"T:System.Reflection.MemberInfo", @"T:System.Reflection.ParameterInfo" or @"T:System.Reflection.Assembly".



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Serialization.ReflectionAttributeProvider.#ctor(System.Object)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Serialization.ReflectionAttributeProvider" class.

|Parameter Name|Remarks|
|--------------|-------|
|attributeProvider|The instance to get attributes for. This parameter should be a @"T:System.Type", @"T:System.Reflection.MemberInfo", @"T:System.Reflection.ParameterInfo" or @"T:System.Reflection.Assembly".|


#### GetAttributes
```csharp
Newtonsoft.Json.Serialization.ReflectionAttributeProvider.GetAttributes(System.Type,System.Boolean)
```
Returns a collection of attributes, identified by type, or an empty collection if there are no attributes.

|Parameter Name|Remarks|
|--------------|-------|
|attributeType|The type of the attributes.|
|inherit|When true, look up the hierarchy chain for the inherited custom attribute.|

_returns: A collection of @"T:System.Attribute"s, or an empty collection._


