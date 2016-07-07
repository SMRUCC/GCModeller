---
title: QueryBuilder
---

# QueryBuilder
_namespace: [System.Data.SQLite.Linq.DataMapping.Interface](N-System.Data.SQLite.Linq.DataMapping.Interface.html)_

SQL query statement builder for the SELECT query.



### Methods

#### Escaping
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.QueryBuilder.Escaping(System.String)
```
转码SQlite的分割字符 ( ' --> '' )

|Parameter Name|Remarks|
|--------------|-------|
|fieldValue|-|


#### QueryByStringCompares
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.QueryBuilder.QueryByStringCompares(System.String,System.String,System.String,System.Data.SQLite.Linq.DataMapping.Interface.QueryBuilder.StringCompareMethods)
```


|Parameter Name|Remarks|
|--------------|-------|
|Field|-|
|value|-|
|ComparedMethod|默认是使用最宽松的匹配条件|


#### QueryByStringCompares``1
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.QueryBuilder.QueryByStringCompares``1(System.String,System.String,System.Data.SQLite.Linq.DataMapping.Interface.QueryBuilder.StringCompareMethods)
```


|Parameter Name|Remarks|
|--------------|-------|
|Field|-|
|value|-|
|ComparedMethod|默认是使用最宽松的匹配条件|


#### TrimmingSQLConserved
```csharp
System.Data.SQLite.Linq.DataMapping.Interface.QueryBuilder.TrimmingSQLConserved(System.String)
```
将SQL语句之中的特殊字符进行移除

|Parameter Name|Remarks|
|--------------|-------|
|str|-|



