---
title: SQLProcedure
---

# SQLProcedure
_namespace: [System.Data.SQLite.Linq.DataMapping.Interface](N-System.Data.SQLite.Linq.DataMapping.Interface.html)_

The API interface wrapper of the SQLite.(SQLite的存储引擎的接口)



### Methods

#### ___SQLDump
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure.___SQLDump(System.Data.SQLite.Linq.DataMapping.Interface.TableDump[])
```
Export the data dump in the format of a INSERT INTO SQL statement for transfer the data in this database into another database.

|Parameter Name|Remarks|
|--------------|-------|
|Table|The table schema of the target table which will be transfer.|


#### CreateSQLDump``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure.CreateSQLDump``1
```
Export the data dump in the format of a INSERT INTO SQL statement for transfer the data in this database into another database.

#### CreateSQLTransaction
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure.CreateSQLTransaction(System.String)
```
Establishing the protocol of the SQLite connection between you program and the database file "**url**".

|Parameter Name|Remarks|
|--------------|-------|
|url|The path of the SQLite database file.|


#### CreateTableFor
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure.CreateTableFor(System.Type)
```
Create a table in current database file for the specific table schema **TypeInfo** .

#### CreateTableFor``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure.CreateTableFor``1
```
Create a table in current database file for the specific table schema T .

#### DbDump
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure.DbDump(System.String)
```
转储整个数据库

#### DeleteTable
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure.DeleteTable(System.String)
```
Delete the target table.

#### Execute
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure.Execute(System.String,System.String[])
```
If the SQL is a SELECT statement, then this function returns a table object, if not then it returns nothing.

|Parameter Name|Remarks|
|--------------|-------|
|SQL|-|


#### ExecuteTransaction
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure.ExecuteTransaction(System.String[])
```
Batch execute a SQL collection as a SQL transaction.

|Parameter Name|Remarks|
|--------------|-------|
|SQL|-|


#### ExistsTable
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.SQLProcedure.ExistsTable(System.Type)
```
Get a value to knows that wether the target table is exists in the database or not.


### Properties

#### SQLiteCnn
SQLite 连接字符串
#### URL
Get the filename of the connected SQLite database file.(返回数据库文件的文件位置)
