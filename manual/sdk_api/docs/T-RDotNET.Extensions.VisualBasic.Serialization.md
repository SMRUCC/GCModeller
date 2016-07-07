---
title: Serialization
---

# Serialization
_namespace: [RDotNET.Extensions.VisualBasic](N-RDotNET.Extensions.VisualBasic.html)_

Convert the R object into a .NET object from the specific type schema information.
 (将R之中的对象内存数据转换为.NET之中指定的对象实体)



### Methods

#### __createMatrix
```csharp
RDotNET.Extensions.VisualBasic.Serialization.__createMatrix(RDotNET.SymbolicExpression,System.Type,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|RData|-|
|typeInfo|-|
|debugLv|Debugger output levels|


#### __loadFromStream
```csharp
RDotNET.Extensions.VisualBasic.Serialization.__loadFromStream(RDotNET.SymbolicExpression,System.Type,System.Int32)
```
Load the R symbolic expression data recursivly start from here.

|Parameter Name|Remarks|
|--------------|-------|
|RData|-|
|TypeInfo|-|


#### __mappingCollectionType
```csharp
RDotNET.Extensions.VisualBasic.Serialization.__mappingCollectionType(System.Object,System.Reflection.PropertyInfo,System.Object@,System.Type)
```
Object() to T()()

|Parameter Name|Remarks|
|--------------|-------|
|value|-|
|pInfo|-|
|obj|-|
|pTypeInfo|-|


#### __rVectorToNETProperty
```csharp
RDotNET.Extensions.VisualBasic.Serialization.__rVectorToNETProperty(System.Type,System.Object,System.Reflection.PropertyInfo,System.Object@)
```
All of the object in R is a vector, so that we needs this function to convert the R vector to a property value.

|Parameter Name|Remarks|
|--------------|-------|
|pTypeInfo|-|
|value|-|
|pInfo|-|
|obj|-|


#### __valueMapping
```csharp
RDotNET.Extensions.VisualBasic.Serialization.__valueMapping(System.Object,System.Reflection.PropertyInfo,System.Object@)
```


|Parameter Name|Remarks|
|--------------|-------|
|value|-|
|pInfo|-|
|obj|对象实例|


#### InternalLoadS4Object
```csharp
RDotNET.Extensions.VisualBasic.Serialization.InternalLoadS4Object(RDotNET.SymbolicExpression,System.Type,System.Int32)
```
The recursive operation of the S4Object in R starts from here. this recursive operation will stop when the property value is not a S4Object.
 (这个可能是一个递归的过程，一直解析到各个属性的R类型不再是S4对象类型为止)

|Parameter Name|Remarks|
|--------------|-------|
|RData|-|


#### LoadFromStream``1
```csharp
RDotNET.Extensions.VisualBasic.Serialization.LoadFromStream``1(RDotNET.SymbolicExpression)
```
Deserialize the R object into a specific .NET object. @"T:RDotNET.SymbolicExpression" =====> "T"

|Parameter Name|Remarks|
|--------------|-------|
|RData|-|

> 
>  反序列化的规则：
>  1. S4对象里面的Slot为对象类型之中的属性
>  2. 任何对象属性都会被表示为数组
>  

#### LoadRStream
```csharp
RDotNET.Extensions.VisualBasic.Serialization.LoadRStream(RDotNET.SymbolicExpression,System.Type)
```
Needs your manual type casting in your program.

|Parameter Name|Remarks|
|--------------|-------|
|RData|-|
|Type|-|



