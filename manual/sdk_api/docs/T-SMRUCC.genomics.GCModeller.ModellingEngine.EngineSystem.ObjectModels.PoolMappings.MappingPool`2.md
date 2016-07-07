---
title: MappingPool`2
---

# MappingPool`2
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.PoolMappings](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.PoolMappings.html)_

Handler for the network node mapping in the evolution experiment.(节点映射管理器：突变与进化过程之中的映射关系)



### Methods

#### ModifyMapping
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.PoolMappings.MappingPool`2.ModifyMapping(`1,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Node|-|
|NewClass|新的EC编号|

> 这里不要使用并行化，因为需要使用@"P:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.PoolMappings.MotifClass.Handle"或者@"P:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.PoolMappings.EnzymeClass.Handle"进行映射操作

#### UpdateEnzymes
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.PoolMappings.MappingPool`2.UpdateEnzymes(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.PoolMappings.IMappingEdge{`0,`1})
```
在执行了突变或者进化实验时候，假若改变了节点的映射关系的话，则需要调用本方法更新整个引擎之中的映射关系

|Parameter Name|Remarks|
|--------------|-------|
|Edge|-|



### Properties

#### _CACHE_MappingPool
Cache data
#### _DICT_MappingPool
Nodes mapping dictionary data
