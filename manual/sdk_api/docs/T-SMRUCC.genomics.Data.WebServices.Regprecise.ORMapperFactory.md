---
title: ORMapperFactory
---

# ORMapperFactory
_namespace: [SMRUCC.genomics.Data.WebServices.Regprecise](N-SMRUCC.genomics.Data.WebServices.Regprecise.html)_





### Methods

#### CreateArrayMapper
```csharp
SMRUCC.genomics.Data.WebServices.Regprecise.ORMapperFactory.CreateArrayMapper(System.Type,System.String)
```
创建动态类型进行json反序列化

|Parameter Name|Remarks|
|--------------|-------|
|mappedType|动态集合|
|name|集合的属性名称|


#### CreateConstructor
```csharp
SMRUCC.genomics.Data.WebServices.Regprecise.ORMapperFactory.CreateConstructor(System.Reflection.Emit.TypeBuilder)
```
create constructor

|Parameter Name|Remarks|
|--------------|-------|
|typeBuilder|-|


#### CreateType
```csharp
SMRUCC.genomics.Data.WebServices.Regprecise.ORMapperFactory.CreateType(System.Reflection.Emit.ModuleBuilder,System.String)
```
create new type for table name

|Parameter Name|Remarks|
|--------------|-------|
|modBuilder|-|
|typeName|-|



