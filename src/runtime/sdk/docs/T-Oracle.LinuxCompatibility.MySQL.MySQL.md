---
title: MySQL
---

# MySQL
_namespace: [Oracle.LinuxCompatibility.MySQL](N-Oracle.LinuxCompatibility.MySQL.html)_

MySql database server connection module.
 (与MySql数据库服务器之间的通信操作的封装模块)



### Methods

#### CommitTransaction
```csharp
Oracle.LinuxCompatibility.MySQL.MySQL.CommitTransaction(System.String)
```
Commit a transaction command collection to the database server and then return the 
 result that this transaction is commit successfully or not. 
 (向数据库服务器提交一个事务之后返回本事务是否被成功提交)

|Parameter Name|Remarks|
|--------------|-------|
|Transaction|-|

_returns: 
 Return the result that this transaction is commit succeedor not.
 (返回本事务是否被成功提交至数据库服务器)
 _

#### Connect
```csharp
Oracle.LinuxCompatibility.MySQL.MySQL.Connect(System.String)
```
Connect to the database server using a assigned mysql connection string.
 (使用一个由用户所制定的连接字符串连接MySql数据库服务器)

|Parameter Name|Remarks|
|--------------|-------|
|ConnectionString|-|


#### Execute
```csharp
Oracle.LinuxCompatibility.MySQL.MySQL.Execute(System.String)
```
Execute a DML/DDL sql command and then return the row number that the row was affected 
 by this command, and you should open a connection to a database server before you call 
 this function. 
 (执行一个DML/DDL命令并且返回受此命令的执行所影响的行数，你应该在打开一个数据库服务器的连接之
 后调用本函数，执行SQL语句发生错误时会返回负数)

|Parameter Name|Remarks|
|--------------|-------|
|SQL|DML/DDL sql command, not a SELECT command(DML/DDL 命令，而非一个SELECT语句)|

_returns: 
 Return the row number that was affected by the DML/DDL command, if the databse 
 server connection is interrupt or errors occurred during the executes, this 
 function will return a negative number.
 (返回受DML/DDL命令所影响的行数，假若数据库服务器断开连接或者在命令执行的期间发生错误，
 则这个函数会返回一个负数)
 _

#### ExecuteAggregate``1
```csharp
Oracle.LinuxCompatibility.MySQL.MySQL.ExecuteAggregate``1(System.String)
```
执行聚合函数并返回值

|Parameter Name|Remarks|
|--------------|-------|
|SQL|-|


#### ExecuteScalar``1
```csharp
Oracle.LinuxCompatibility.MySQL.MySQL.ExecuteScalar``1(System.String)
```
Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.

|Parameter Name|Remarks|
|--------------|-------|
|SQL|请手工添加 limit 1 限定|


#### ExecuteScalarAuto``1
```csharp
Oracle.LinuxCompatibility.MySQL.MySQL.ExecuteScalarAuto``1(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|[where]|只需要给出条件WHERE表达式即可，函数会自动生成SQL查询语句|


#### Fetch
```csharp
Oracle.LinuxCompatibility.MySQL.MySQL.Fetch(System.String)
```
Execute a 'SELECT' query command and then returns the query result of this sql command.
 (执行一个'SELECT'查询命令之后返回本查询命令的查询结果)

|Parameter Name|Remarks|
|--------------|-------|
|SQL|-|


#### GetErrMessage
```csharp
Oracle.LinuxCompatibility.MySQL.MySQL.GetErrMessage
```
Get the error message that throw by the client during the time of the sql command executed.
 (获取在客户端执行Sql命令的时候所捕获的错误的描述，会将错误信息清除)

#### op_Implicit
```csharp
Oracle.LinuxCompatibility.MySQL.MySQL.op_Implicit(Oracle.LinuxCompatibility.MySQL.ConnectionUri)~Oracle.LinuxCompatibility.MySQL.MySQL
```
Open a mysql connection using a connection helper object

|Parameter Name|Remarks|
|--------------|-------|
|uri_obj|The connection helper object|


#### Ping
```csharp
Oracle.LinuxCompatibility.MySQL.MySQL.Ping
```
Test the connection of the client to the mysql database server and then 
 return the communication delay time between the client and the server. 
 This function should be call after you connection to a database server.
 (测试客户端和MySql数据库服务器之间的通信连接并且返回二者之间的通信延时。
 这个函数应该在你连接上一个数据库服务器之后进行调用，-1值表示客户端与服务器之间通信失败.)
_returns: 当函数返回一个负数的时候，表明Ping操作失败，即无数据库服务器连接_


### Properties

#### UriMySQL
A Formatted connection string using for the connection established to the database server.
