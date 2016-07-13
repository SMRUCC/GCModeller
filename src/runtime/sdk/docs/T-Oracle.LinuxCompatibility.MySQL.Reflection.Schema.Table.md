---
title: Table
---

# Table
_namespace: [Oracle.LinuxCompatibility.MySQL.Reflection.Schema](N-Oracle.LinuxCompatibility.MySQL.Reflection.Schema.html)_

The table schema that we define on the custom attributes of a Class.



### Methods

#### __indexing
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.Schema.Table.__indexing(System.String,System.Reflection.PropertyInfo,System.Reflection.PropertyInfo[])
```
Indexing from the primary key attributed field.

|Parameter Name|Remarks|
|--------------|-------|
|Index2|-|
|Indexproperty2|-|
|ItemProperty|-|



### Properties

#### Database
这张数据表所在数据库的名称
#### DatabaseField
查找不到则返回空值
#### Index
The index field when execute the update/delete sql.
#### PrimaryFields
主键，主要根据这个属性来生成WHERE条件
#### TableName

