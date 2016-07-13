---
title: DataModel
---

# DataModel
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.html)_

The data exchange service for the data model read from and write into the mysql database.
 (数据模型对象与数据库服务器之间的数据交换服务)



### Methods

#### Commit
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel.Commit
```
Commit the transaction to the database server.
 (将事务集提交至数据库服务器之中)

#### CreateTable
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel.CreateTable(System.String)
```
Create a table to store that model data in the database with the specifci name.
 (在数据库之中创建一个具有特定名称类型的用于存储模型数据的数据表)

|Parameter Name|Remarks|
|--------------|-------|
|Table|Table name.(表名)|

> 
>  Please notice that the storage format of every datamodel is in the same type, but in order to make 
>  the sql query faster, we split this models stored into several table with the same schema but the 
>  name is different. So the difference between these data table just only on the part of the table 
>  namming. 
>  (请注意，模型数据在数据库中的存储格式在各个对象之间是一模一样的，但是为了加快查询速度，故将模型数据按照分
>  类分别存放于若干个不同的数据表之中。故在这些表之中，各个表之间的格式是一致的，但是不同之处仅在于表的名称
>  不同而已)
>  

#### Delete
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel.Delete(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModelRecord,System.Boolean)
```
Delete a line of data into the database table using its unique registry number.
 (使用数据对象的登录号删除数据表之中的一行已存在的数据)

|Parameter Name|Remarks|
|--------------|-------|
|row|The row data that will be deleted in the table.(表中将要删除的一行数据对象)|
|PendingTransaction|
 Does this operation was pending to the sql transaction to save networking usage or not, default is not pending.
 (是否将本次操作排队至事务集之中以节约网络通信消耗，默认不排队)
 |


#### Flush
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel.Flush(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Table|
 The name of the table that all of its model data will be delete. If the value of this parameter is '*', 
 then the client will clear the whole model data in the database 'GCModeller'.
 (将要清除所有模型数据的表的名称，假若本参数的值为'*'，则将数据库‘GCModeller’中所有的数据表的数据清空)
 |


#### FlushAll
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel.FlushAll
```
Clear all of the model data in the database.
 (清除数据库之中的所有模型数据)

#### GetMaxHandle
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel.GetMaxHandle(System.String)
```
获取目标数据表之中的最大的句柄值

#### Insert
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel.Insert(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModelRecord,System.Boolean)
```
Insert a line of data into the database table.
 (向数据表之中插入一行新数据)

|Parameter Name|Remarks|
|--------------|-------|
|row|The row data that will be insert into the table.(将要插入表中的数据行)|
|PendingTransaction|
 Does this operation was pending to the sql transaction to save networking usage or not, default is not pending.
 (是否将本次操作排队至事务集之中以节约网络通信消耗，默认不排队)
 |


#### Load
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel.Load(System.String,System.Int32)
```
Load the data from database server. Please notice that: every time you call this function, the transaction will be commit to the database server.
 (从数据库服务器之中加载数据，请注意：每一次加载数据都会将先前的所积累下来的事务提交至数据库服务器之上)

|Parameter Name|Remarks|
|--------------|-------|
|Counts|
 The count of the record that will be read from the server. Notice: Zero or negative is stands for 
 load all records in the database.
 (从数据库中读取的记录数目。请注意：值0和负数值都表示加载数据库的表中的所有数据)
 |


#### op_Implicit
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel.op_Implicit(System.String)~SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel
```


|Parameter Name|Remarks|
|--------------|-------|
|ConnectionString|The connection string of the target mysql database.(至目标数据库的连接字符串)|


#### Update
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModel.Update(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModelRecord,System.Boolean)
```
Update a line of data into the database table using its unique registry number.
 (使用数据对象的登录号更新数据表之中的一行已存在的数据)

|Parameter Name|Remarks|
|--------------|-------|
|row|The row data that will be update into the table.(将要对表中的数据行进行更新的数据对象)|
|PendingTransaction|
 Does this operation was pending to the sql transaction to save networking usage or not, default is not pending.
 (是否将本次操作排队至事务集之中以节约网络通信消耗，默认不排队)
 |



### Properties

#### MAX_HANDLE_SQL
查询出数据库之中的某一个表的最大句柄值
#### Table
The data table that will be operated.
 (所进行操作的数据表)
