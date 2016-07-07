---
title: ConnectionUri
---

# ConnectionUri
_namespace: [Oracle.LinuxCompatibility.MySQL](N-Oracle.LinuxCompatibility.MySQL.html)_

The connection parameter for the MYSQL database server.(MySQL服务器的远程连接参数)



### Methods

#### CreateObject
```csharp
Oracle.LinuxCompatibility.MySQL.ConnectionUri.CreateObject(System.String,System.Func{System.String,System.String})
```
从配置数据之中加载数据库的连接信息

|Parameter Name|Remarks|
|--------------|-------|
|url|-|
|passwordDecryption|-|


#### GenerateUri
```csharp
Oracle.LinuxCompatibility.MySQL.ConnectionUri.GenerateUri(System.Func{System.String,System.String})
```
重新生成链接url字符串

|Parameter Name|Remarks|
|--------------|-------|
|passwordEncryption|用户自定义的密码加密信息|


#### GetConnectionString
```csharp
Oracle.LinuxCompatibility.MySQL.ConnectionUri.GetConnectionString
```
Get a connection string for the connection establish of a client to a mysql database 
 server using the specific paramenter that was assigned by the user.
 (获取一个由用户指定连接参数的用于建立客户端和MySql数据库服务器之间的连接的连接字符串)

#### MySQLParser
```csharp
Oracle.LinuxCompatibility.MySQL.ConnectionUri.MySQLParser(System.String)
```
Database={0}; Data Source={1}; User Id={2}; Password={3}; Port={4}

|Parameter Name|Remarks|
|--------------|-------|
|cnn|-|


#### op_Explicit
```csharp
Oracle.LinuxCompatibility.MySQL.ConnectionUri.op_Explicit(Oracle.LinuxCompatibility.MySQL.ConnectionUri)~System.String
```
Conver the ConnectionHelper object to a mysql connection string using 
 the specific parameter which assigned by the user.
 (将使用由用户指定连接参数的连接生成器转换为Mysql数据库的连接字符串)

|Parameter Name|Remarks|
|--------------|-------|
|uri|-|


#### op_Implicit
```csharp
Oracle.LinuxCompatibility.MySQL.ConnectionUri.op_Implicit(System.String)~Oracle.LinuxCompatibility.MySQL.ConnectionUri
```


|Parameter Name|Remarks|
|--------------|-------|
|url|MySql connection string.(MySql连接字符串)|

> 
>  Example: 
>  http://localhost:8080/client?user=username%password=password%database=database
>  

#### TryParsing
```csharp
Oracle.LinuxCompatibility.MySQL.ConnectionUri.TryParsing(System.String)
```
函数会自动解析MySQL格式或者uri拓展格式

|Parameter Name|Remarks|
|--------------|-------|
|uri|MySQL连接字符串或者uri拓展格式|



### Properties

#### Database
Using <database_name>.(数据库的名称)
#### IPAddress
The server IP address, you can using 'localhost' to specific the local machine.(服务器的IP地址，可以使用localhost来指代本机)
#### ServicesPort
The port number of the remote database server.(数据库服务器的端口号)
