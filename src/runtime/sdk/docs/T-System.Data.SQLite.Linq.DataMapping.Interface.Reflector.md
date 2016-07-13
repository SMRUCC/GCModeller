---
title: Reflector
---

# Reflector
_namespace: [System.Data.SQLite.Linq.DataMapping.Interface](N-System.Data.SQLite.Linq.DataMapping.Interface.html)_

Provides the reflection operation extensions for the generic collection data to updates database.



### Methods

#### __getSchemaCache``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.Reflector.__getSchemaCache``1
```
Loading the database table schema information using the reflection operation from the meta data storted in the type schema.

#### CommitData``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.Reflector.CommitData``1(System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure,System.Collections.Generic.IEnumerable{``0})
```
批量更新数据

|Parameter Name|Remarks|
|--------------|-------|
|DbTransaction|-|
|data|-|


#### FlushData``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.Reflector.FlushData``1(System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure,System.Collections.Generic.IEnumerable{``0})
```
批量删除数据

|Parameter Name|Remarks|
|--------------|-------|
|DbTransaction|-|
|data|-|


#### GetTableName
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.Reflector.GetTableName(System.Type)
```
Get the table name from the type schema.

|Parameter Name|Remarks|
|--------------|-------|
|type|-|


#### GetTableName``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.Reflector.GetTableName``1
```
Get the table name from the type schema.

#### Insert``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.Reflector.Insert``1(System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure,System.Data.SQLite.Linq.DataMapping.Interface.TableSchema,``0)
```
INSERT INTO Table VALUES (value1, value2, ....)
 INSERT INTO table_name (col1, col2, ...) VALUES (value1, value2, ....)

|Parameter Name|Remarks|
|--------------|-------|
|DbTransaction|-|
|obj|-|


#### InternalGetSchemaCache
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.Reflector.InternalGetSchemaCache(System.Type)
```
Loading the database table schema information using the reflection operation from the meta data storted in the type schema.

|Parameter Name|Remarks|
|--------------|-------|
|type|-|


#### Load
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.Reflector.Load(System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure,System.Type)
```
这个函数会一次性加载所有的数据

|Parameter Name|Remarks|
|--------------|-------|
|DbTransaction|-|
|SchemaInfo|-|


#### Load``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.Reflector.Load``1(System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure)
```
这个函数会一次性加载所有的数据

|Parameter Name|Remarks|
|--------------|-------|
|DbTransaction|-|


#### RecordExists
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.Reflector.RecordExists(System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure,System.Object)
```
判断某一个实体对象是否存在于数据库之中

|Parameter Name|Remarks|
|--------------|-------|
|DbTransaction|-|
|obj|-|



