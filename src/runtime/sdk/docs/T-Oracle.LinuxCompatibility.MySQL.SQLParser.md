---
title: SQLParser
---

# SQLParser
_namespace: [Oracle.LinuxCompatibility.MySQL](N-Oracle.LinuxCompatibility.MySQL.html)_





### Methods

#### __createDataType
```csharp
Oracle.LinuxCompatibility.MySQL.SQLParser.__createDataType(System.String)
```
Mapping the MySQL database type and visual basic data type

|Parameter Name|Remarks|
|--------------|-------|
|TypeDef|-|


#### __createSchema
```csharp
Oracle.LinuxCompatibility.MySQL.SQLParser.__createSchema(System.String[],System.String,System.String,System.String)
```
Create a MySQL table schema object.

|Parameter Name|Remarks|
|--------------|-------|
|Fields|-|
|TableName|-|
|PrimaryKey|-|
|CreateTableSQL|-|


#### __getDBName
```csharp
Oracle.LinuxCompatibility.MySQL.SQLParser.__getDBName(System.String)
```
获取数据库的名称

|Parameter Name|Remarks|
|--------------|-------|
|SQL|-|


#### LoadSQLDoc
```csharp
Oracle.LinuxCompatibility.MySQL.SQLParser.LoadSQLDoc(System.String)
```
Loading the table schema from a specific SQL doucment.

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|



### Properties

#### FIELD_COMMENTS
Regex expression for parsing the comments of the field in a table definition.
#### SQL_CREATE_TABLE
Parsing the create table statement in the SQL document.
