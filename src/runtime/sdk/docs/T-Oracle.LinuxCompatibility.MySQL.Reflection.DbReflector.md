---
title: DbReflector
---

# DbReflector
_namespace: [Oracle.LinuxCompatibility.MySQL.Reflection](N-Oracle.LinuxCompatibility.MySQL.Reflection.html)_





### Methods

#### __getObject``1
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector.__getObject``1(System.Object,System.Type,System.Collections.Generic.KeyValuePair{System.Int32,System.Reflection.PropertyInfo}[])
```
查询并行化

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|type|-|
|FieldList|-|


#### __queryEngine``1
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector.__queryEngine``1(System.String,Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector.QueryInvoke{``0},System.String@)
```
执行具体的数据映射操作

|Parameter Name|Remarks|
|--------------|-------|
|SQL|-|
|queryEngine|-|
|getError|-|


#### AsQuery``1
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector.AsQuery``1(System.String,System.String@)
```
Linq to MySQL.(Linq to MySQL对象实体数据映射)

|Parameter Name|Remarks|
|--------------|-------|
|SQL|-|


#### ForEach``1
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector.ForEach``1(System.String,System.Func{``0,System.Boolean},System.String@)
```
LINQ to MySQL interactor.

|Parameter Name|Remarks|
|--------------|-------|
|SQL|-|
|invoke|-|
|getErr|-|


#### op_Implicit
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector.op_Implicit(System.String)~Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector
```


|Parameter Name|Remarks|
|--------------|-------|
|s_cnn|MySql connection string.(MySql连接字符串)|

> 
>  Example: 
>  http://localhost:8080/client?user=username%password=password%database=database
>  

#### ParallelQuery``1
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector.ParallelQuery``1(System.String,System.String@)
```
Optimization for the large dataset parallel query.(数据集合非常大的时候，可以尝试使用这个并行查询函数)

|Parameter Name|Remarks|
|--------------|-------|
|SQL|-|
|GetErr|-|


#### Query``1
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector.Query``1(System.String,System.String@)
```
Query a data table using Reflection.(使用反射机制来查询一个数据表，请注意，假若返回的是Nothing，则说明发生了错误)

|Parameter Name|Remarks|
|--------------|-------|
|SQL|Sql 'SELECT' query statement.(Sql 'SELECT' 查询语句)|

_returns: The target data table.(目标数据表)_

#### ReadFirst``1
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector.ReadFirst``1(System.Data.DataTableReader)
```
假若目标数据表不存在数据记录，则会返回空值

|Parameter Name|Remarks|
|--------------|-------|
|Reader|-|



