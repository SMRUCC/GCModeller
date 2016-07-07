---
title: IAttributeProvider
---

# IAttributeProvider
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Provides methods to get attributes.



### Methods

#### GetAttributes
```csharp
Newtonsoft.Json.Serialization.IAttributeProvider.GetAttributes(System.Type,System.Boolean)
```
Returns a collection of attributes, identified by type, or an empty collection if there are no attributes.

|Parameter Name|Remarks|
|--------------|-------|
|attributeType|The type of the attributes.|
|inherit|When true, look up the hierarchy chain for the inherited custom attribute.|

_returns: A collection of @"T:System.Attribute"s, or an empty collection._


