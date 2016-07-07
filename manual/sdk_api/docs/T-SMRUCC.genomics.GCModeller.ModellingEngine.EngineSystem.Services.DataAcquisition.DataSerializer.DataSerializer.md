---
title: DataSerializer
---

# DataSerializer
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer.html)_

数据采集服务与数据存储服务之间的中间层



### Methods

#### #ctor
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer.DataSerializer.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Url|文件名或者数据库连接参数|


#### Append
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer.DataSerializer.Append(System.Collections.Generic.IEnumerable{SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer.DataFlowF})
```
数据采集服务向数据存储服务写入数据包

|Parameter Name|Remarks|
|--------------|-------|
|DataPackage|-|


#### Close
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer.DataSerializer.Close(System.String)
```
关闭与数据存储服务之间的连接

#### CommitData
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer.DataSerializer.CommitData(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|arg|提交数据的时候所需要的参数|



