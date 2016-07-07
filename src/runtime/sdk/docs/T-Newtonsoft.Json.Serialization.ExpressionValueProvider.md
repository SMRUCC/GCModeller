---
title: ExpressionValueProvider
---

# ExpressionValueProvider
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Get and set values for a @"T:System.Reflection.MemberInfo" using dynamic methods.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Serialization.ExpressionValueProvider.#ctor(System.Reflection.MemberInfo)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Serialization.ExpressionValueProvider" class.

|Parameter Name|Remarks|
|--------------|-------|
|memberInfo|The member info.|


#### GetValue
```csharp
Newtonsoft.Json.Serialization.ExpressionValueProvider.GetValue(System.Object)
```
Gets the value.

|Parameter Name|Remarks|
|--------------|-------|
|target|The target to get the value from.|

_returns: The value._

#### SetValue
```csharp
Newtonsoft.Json.Serialization.ExpressionValueProvider.SetValue(System.Object,System.Object)
```
Sets the value.

|Parameter Name|Remarks|
|--------------|-------|
|target|The target to set the value on.|
|value|The value to set on the target.|



