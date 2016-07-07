---
title: Guid
---

# Guid
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine](N-SMRUCC.genomics.GCModeller.ModellingEngine.html)_

The Global Unique Identifier object using for identify a model component in the database.
 (用于在数据库之中标识一个模型组件对象的全局唯一标识符对象)



### Methods

#### AbsolutelyEqual
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Guid.AbsolutelyEqual(SMRUCC.genomics.GCModeller.ModellingEngine.Guid)
```
Judge that those two GUID object instance is absolutely equals to each other. 
 (判断这两个GUID是否绝对相等)

|Parameter Name|Remarks|
|--------------|-------|
|e|
 The GUID object 'e' which will be judge with this GUID object instance.
 (将要与本GUID对象实例进行比较的目标GUID对象实例)
 |

_returns: 
 Get the value from the judge operation of those two GUID object equals from its string value wheter they are equals or not.
 (通过判断两个GUID对象的字符串形式的GUID值是否相等来判断这两个GUID对象实例是否绝对相等)
 _

#### AbsolutelyEquals
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Guid.AbsolutelyEquals(SMRUCC.genomics.GCModeller.ModellingEngine.Guid,SMRUCC.genomics.GCModeller.ModellingEngine.Guid)
```
Judge that those two GUID object instance is absolutely equals to each other. 
 (判断这两个GUID是否绝对相等)

|Parameter Name|Remarks|
|--------------|-------|
|a|The GUID object 'a'|
|b|The GUID object 'b'|

_returns: 
 Get the value from the judge operation of those two GUID object equals from its string value wheter they are equals or not.
 (通过判断两个GUID对象的字符串形式的GUID值是否相等来判断这两个GUID对象实例是否绝对相等)
 _

#### Generate
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Guid.Generate
```
Generate the guid string from this guid object.
 (生成本GUID对象所代表的GUID字符串)

#### op_Equality
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Guid.op_Equality(SMRUCC.genomics.GCModeller.ModellingEngine.Guid,SMRUCC.genomics.GCModeller.ModellingEngine.Guid)
```
The GUID relative equals, this operation just needs the 3 part of the GUID object is equal 
 that these two GUID object is equal.
 (相对相等，只要两个GUID对象的前三位相等即可)

|Parameter Name|Remarks|
|--------------|-------|
|a|-|
|b|-|


#### op_Implicit
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Guid.op_Implicit(System.String)~SMRUCC.genomics.GCModeller.ModellingEngine.Guid
```
Convert a guid string into a guid object instance.
 (将一个GUID字符串转换为一个GUID对象实例)

|Parameter Name|Remarks|
|--------------|-------|
|GUID|The target guid string.(目标待转换的GUID字符串)|



