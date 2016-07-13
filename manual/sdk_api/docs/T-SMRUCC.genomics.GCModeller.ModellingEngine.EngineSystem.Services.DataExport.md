---
title: DataExport
---

# DataExport
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.html)_

数据库中的计算数据导出服务



### Methods

#### FetchData
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataExport.FetchData(System.String)
```
从指定名称的数据表之中加载数据，当指定名称的数据表不存在的时候，则返回0(会生成一个空的CSV文件)

|Parameter Name|Remarks|
|--------------|-------|
|TableName|-|

_returns: 返回所获取的数据的行数_


### Properties

#### SQL_DATA_FETCH
The sql command text for load the data from the database server.(从数据库服务器之中加载数据的SQL命令语句)
