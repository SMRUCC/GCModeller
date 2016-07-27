---
title: Extensions
---

# Extensions
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Provides methods for converting between common language runtime types and JSON types.



### Methods

#### CreateObject
```csharp
Newtonsoft.Json.Extensions.CreateObject(System.String,System.Type)
```
Create a class object from the input json data. Deserializes the JSON to the specified .NET type.

|Parameter Name|Remarks|
|--------------|-------|
|json|The JSON to deserialize.|
|type|The @"T:System.Type" of object being deserialized.|

_returns: The deserialized object from the JSON string._

#### Json
```csharp
Newtonsoft.Json.Extensions.Json(System.Object)
```
Serializes the specified object to a JSON string.

|Parameter Name|Remarks|
|--------------|-------|
|obj|The object to serialize.|

_returns: A JSON string representation of the object._

#### Json``1
```csharp
Newtonsoft.Json.Extensions.Json``1(``0)
```
Serializes the specified object to a JSON string.

|Parameter Name|Remarks|
|--------------|-------|
|obj|The object to serialize.|

_returns: A JSON string representation of the object._

#### LoadAnonymousObject``1
```csharp
Newtonsoft.Json.Extensions.LoadAnonymousObject``1(System.String,``0@)
```
加载匿名类型的数据，由于匿名类型找不到定义，所以需要使用参数来对泛型产生类型约束

|Parameter Name|Remarks|
|--------------|-------|
|json|-|
|obj|真正的匿名类型的信息是来自于函数的这个参数的|


#### LoadObject``1
```csharp
Newtonsoft.Json.Extensions.LoadObject``1(System.String)
```
Json deserializes.(JSON反序列化)

|Parameter Name|Remarks|
|--------------|-------|
|json|-|



