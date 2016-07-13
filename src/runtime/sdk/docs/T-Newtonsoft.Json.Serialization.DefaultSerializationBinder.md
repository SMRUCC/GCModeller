---
title: DefaultSerializationBinder
---

# DefaultSerializationBinder
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

The default serialization binder used when resolving and loading classes from type names.



### Methods

#### BindToName
```csharp
Newtonsoft.Json.Serialization.DefaultSerializationBinder.BindToName(System.Type,System.String@,System.String@)
```
When overridden in a derived class, controls the binding of a serialized object to a type.

|Parameter Name|Remarks|
|--------------|-------|
|serializedType|The type of the object the formatter creates a new instance of.|
|assemblyName|Specifies the @"T:System.Reflection.Assembly" name of the serialized object. |
|typeName|Specifies the @"T:System.Type" name of the serialized object. |


#### BindToType
```csharp
Newtonsoft.Json.Serialization.DefaultSerializationBinder.BindToType(System.String,System.String)
```
When overridden in a derived class, controls the binding of a serialized object to a type.

|Parameter Name|Remarks|
|--------------|-------|
|assemblyName|Specifies the @"T:System.Reflection.Assembly" name of the serialized object.|
|typeName|Specifies the @"T:System.Type" name of the serialized object.|

_returns: 
            The type of the object the formatter creates a new instance of.
            _


