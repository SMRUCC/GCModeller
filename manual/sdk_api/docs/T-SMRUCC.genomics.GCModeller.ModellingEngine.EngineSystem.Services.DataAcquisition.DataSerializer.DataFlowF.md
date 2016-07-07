---
title: DataFlowF
---

# DataFlowF
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer.html)_

(数据包中的一个数据点：表示为一个节点处的流量值或者该节点大小)



### Methods

#### ToCsvRow
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer.DataFlowF.ToCsvRow
```
将本记录中的数据转换为Csv数据表文件中的一行数据 [Id, Time, Handle, Value]


### Properties

#### CREATE_TABLE_SQL
创建存储所使用的数据表所使用的SQL语句
#### Handle
目标对象唯一的句柄值
#### Id
当前记录在数据表中的Id编号
#### INSERT_SQL
插入一条记录所使用的SQL语句
#### SQL_DROP_TABLE
清空表中所有的数据所使用到的SQL语句
#### Time
当前的迭代次数
#### Value
模拟计算所产生的值
