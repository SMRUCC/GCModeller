---
title: JsonContract
---

# JsonContract
_namespace: [Microsoft.VisualBasic.Serialization.JSON](N-Microsoft.VisualBasic.Serialization.JSON.html)_

Only works on the Public visible type.
 (使用.NET系统环境之中自带的框架进行JSON序列化和反序列化)



### Methods

#### GetJson
```csharp
Microsoft.VisualBasic.Serialization.JSON.JsonContract.GetJson(System.Object,System.Type,System.Boolean)
```
使用@"T:System.Web.Script.Serialization.ScriptIgnoreAttribute"来屏蔽掉不想序列化的属性

|Parameter Name|Remarks|
|--------------|-------|
|obj|-|
|type|-|


#### GetJson``1
```csharp
Microsoft.VisualBasic.Serialization.JSON.JsonContract.GetJson``1(``0,System.Boolean)
```
Gets the json text value of the target object, the attribute @"T:System.Web.Script.Serialization.ScriptIgnoreAttribute" 
 can be used for block the property which is will not serialize to the text.
 (使用@"T:System.Web.Script.Serialization.ScriptIgnoreAttribute"来屏蔽掉不想序列化的属性)

|Parameter Name|Remarks|
|--------------|-------|
|obj|-|


#### LoadObject
```csharp
Microsoft.VisualBasic.Serialization.JSON.JsonContract.LoadObject(System.String,System.Type)
```


|Parameter Name|Remarks|
|--------------|-------|
|json|null -> Nothing|
|type|-|


#### LoadObject``1
```csharp
Microsoft.VisualBasic.Serialization.JSON.JsonContract.LoadObject``1(System.String)
```
JSON反序列化

#### WriteLargeJson``1
```csharp
Microsoft.VisualBasic.Serialization.JSON.JsonContract.WriteLargeJson``1(``0,System.String)
```
将目标对象保存为json文件

|Parameter Name|Remarks|
|--------------|-------|
|obj|-|
|path|-|



