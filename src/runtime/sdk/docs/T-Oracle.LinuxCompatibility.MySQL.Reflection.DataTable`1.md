---
title: DataTable`1
---

# DataTable`1
_namespace: [Oracle.LinuxCompatibility.MySQL.Reflection](N-Oracle.LinuxCompatibility.MySQL.Reflection.html)_

A table object of a specific table schema that mapping a table object in the mysql database.
 (一个映射到MYSQL数据库中的指定的表之上的表对象)



### Methods

#### Commit
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DataTable`1.Commit
```
Commit the transaction to the database server to make the change permanently.
 (将事务集提交至数据库服务器之上以永久的修改数据库)
_returns: The transaction commit is successfully or not.(事务集是否被成功提交)_

#### Delete
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DataTable`1.Delete(`0)
```
Delete a record in the table. Please notice that, in order to decrease the usage of CPU and networking traffic, the 
 change is not directly affect on the database server, it will be store as a delete sql in the memory and accumulated 
 as a transaction, the change of the database will not happen until you call the commit method to make this change 
 permanently in the database.
 (删除表中的一条记录。请注意：为了减少服务器的计算资源和网络流量的消耗，在使用本模块对数据库作出修改的时候，更改并不会直接提
 交至数据库之中的，而是将修改作为一条SQL语句存储下来并对这些命令进行积累作为一个事务存在，即数据库不会受到修改的影响直到你将
 本事务提交至数据库服务器之上)

|Parameter Name|Remarks|
|--------------|-------|
|Record|-|


#### Fetch
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DataTable`1.Fetch(System.Int32)
```
Load the data from database server. Please notice that: every time you call this function, the transaction will be commit to the database server in.
 (从数据库服务器之中加载数据，请注意：每一次加载数据都会将先前的所积累下来的事务提交至数据库服务器之上)

|Parameter Name|Remarks|
|--------------|-------|
|Count|
 The count of the record that will be read from the server. Notice: Zero or negative is stands for 
 load all records in the database.
 (从数据库中读取的记录数目。请注意：值0和负数值都表示加载数据库的表中的所有数据)
 |


#### GetHandle
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DataTable`1.GetHandle(`0)
```
Get a specific record in the dataset by compaired the UNIQUE_INDEX field value.
 (通过值唯一的索引字段来获取一个特定的数据记录)

|Parameter Name|Remarks|
|--------------|-------|
|Record|-|


#### Insert
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DataTable`1.Insert(`0)
```
Insert a record in the table. Please notice that, in order to decrease the usage of CPU and networking traffic, the 
 change is not directly affect on the database server, it will be store as a insert sql in the memory and accumulated 
 as a transaction, the change of the database will not happen until you call the commit method to make this change 
 permanently in the database.
 (向表中插入一条新记录。请注意：为了减少服务器的计算资源和网络流量的消耗，在使用本模块对数据库作出修改的时候，更改并不会直接提
 交至数据库之中的，而是将修改作为一条SQL语句存储下来并对这些命令进行积累作为一个事务存在，即数据库不会受到修改的影响直到你将
 本事务提交至数据库服务器之上)

|Parameter Name|Remarks|
|--------------|-------|
|Record|-|


#### op_Explicit
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DataTable`1.op_Explicit(Oracle.LinuxCompatibility.MySQL.Reflection.DataTable{`0})~Microsoft.VisualBasic.List{`0}
```
Convert the mapping object to a dataset

|Parameter Name|Remarks|
|--------------|-------|
|schema|-|


#### op_Implicit
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DataTable`1.op_Implicit(System.Xml.Linq.XElement)~Oracle.LinuxCompatibility.MySQL.Reflection.DataTable{`0}
```
Initialize the mapping from a connection string

|Parameter Name|Remarks|
|--------------|-------|
|xml|-|


#### Query
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DataTable`1.Query(System.String)
```
Query a data table using Reflection.(使用反射机制来查询一个数据表)

|Parameter Name|Remarks|
|--------------|-------|
|SQL|Sql 'SELECT' query statement.(Sql 'SELECT' 查询语句)|

_returns: The target data table.(目标数据表)_

#### Update
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DataTable`1.Update(`0)
```
Update a record in the table. Please notice that, in order to decrease the usage of CPU and networking traffic, the 
 change is not directly affect on the database server, it will be store as a update sql in the memory and accumulated 
 as a transaction, the change of the database will not happen until you call the commit method to make this change 
 permanently in the database.
 (修改表中的一条记录。请注意：为了减少服务器的计算资源和网络流量的消耗，在使用本模块对数据库作出修改的时候，更改并不会直接提
 交至数据库之中的，而是将修改作为一条SQL语句存储下来并对这些命令进行积累作为一个事务存在，即数据库不会受到修改的影响直到你将
 本事务提交至数据库服务器之上)

|Parameter Name|Remarks|
|--------------|-------|
|Record|-|



### Properties

#### _listData
DataSet of the table in the database.
 (数据库的表之中的数据集)
#### DeleteSQL
'DELETE' sql text generator of a record that type of schema.
#### ErrorMessage
The error information that come from MYSQL database server.
 (来自于MYSQL数据库服务器的错误信息)
#### InsertSQL
'INSERT' sql text generator of a record that type of schema.
#### ListData
DataSet of the table in the database. Do not edit the data directly from this property...
 (数据库的表之中的数据集，请不要直接在这个属性之上修改数据)
#### TableSchema
The structure definition information which was parsed from the custom attribut on a class object.
 (从一个类对象上面的自定义属性之中解析出来的表结构信息)
#### Transaction
The sql transaction that will be commit to the mysql database.
 (将要被提交至MYSQL数据库之中的SQL事务集)
#### UpdateSQL
'UPDATE' sql text generator of a record that type of schema.
