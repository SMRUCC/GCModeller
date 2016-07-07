---
title: SchemaCache
---

# SchemaCache
_namespace: [System.Data.SQLite.Linq.DataMapping.Interface](N-System.Data.SQLite.Linq.DataMapping.Interface.html)_

Cached for the database schema.



### Methods

#### __getValue
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache.__getValue(System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache,System.Object)
```
请注意，函数的返回值是带有数据库之中的间隔符号'的

|Parameter Name|Remarks|
|--------------|-------|
|p|-|
|obj|-|


#### CreateDeleteSQL
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache.CreateDeleteSQL(System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache[],System.Object,System.String)
```
DELETE FROM table_name WHERE col = value

|Parameter Name|Remarks|
|--------------|-------|
|SchemaCache|-|
|obj|-|
|TableName|-|


#### CreateDeleteSQL``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache.CreateDeleteSQL``1(System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache[],``0,System.String)
```
DELETE FROM table_name WHERE col = value

|Parameter Name|Remarks|
|--------------|-------|
|SchemaCache|-|
|obj|-|
|TableName|-|


#### CreateInsertSQL
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache.CreateInsertSQL(System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache[],System.Object,System.String)
```
INSERT INTO table_name (col1, col2, ...) VALUES (value1, value2, ....)

|Parameter Name|Remarks|
|--------------|-------|
|SchemaCache|-|


#### CreateInsertSQL``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache.CreateInsertSQL``1(System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache[],``0,System.String)
```
INSERT INTO table_name (col1, col2, ...) VALUES (value1, value2, ....)

|Parameter Name|Remarks|
|--------------|-------|
|SchemaCache|-|


#### CreateTableSQL
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache.CreateTableSQL(System.Data.SQLite.Linq.DataMapping.Interface.TableDump[])
```
CREATE TABLE TableName
 (
 col1 dbtype,
 col2 dbtype,
 col3 dbtype,
 ....
 )

#### CreateUpdateSQL
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache.CreateUpdateSQL(System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache[],System.Object,System.String)
```
UPDATE table_name SET col = value WHERE col = colName

|Parameter Name|Remarks|
|--------------|-------|
|SchemaCache|-|
|obj|-|


#### CreateUpdateSQL``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache.CreateUpdateSQL``1(System.Data.SQLite.Linq.DataMapping.Interface.SchemaCache[],``0,System.String)
```
UPDATE table_name SET col = value WHERE col = colName

|Parameter Name|Remarks|
|--------------|-------|
|SchemaCache|-|
|obj|-|



### Properties

#### DbType
integer(size) 仅容纳整数。在括号内规定数字的最大位数。
 int(size)
 smallint(size)
 tinyint(size)
 decimal(size,d) 容纳带有小数的数字。"size" 规定数字的最大位数。"d" 规定小数点右侧的最大位数。
 numeric(size,d)
 char(size) 容纳固定长度的字符串（可容纳字母、数字以及特殊字符）。在括号中规定字符串的长度。
 
 varchar(size) 容纳可变长度的字符串（可容纳字母、数字以及特殊的字符）。在括号中规定字符串的最大长度。
 date(yyyymmdd)
