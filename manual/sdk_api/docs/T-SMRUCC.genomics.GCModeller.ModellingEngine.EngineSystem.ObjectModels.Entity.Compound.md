---
title: Compound
---

# Compound
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.html)_

Entity basetype in GCModeller ObjectModels.(GCModeller计算引擎之中的对象模型Entity类型的基类，本类型对象是整个系统的的运行基础，也可以认为生命是建立在Compound这种物质基础上的相互作用)



### Methods

#### SetFluxValue
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Compound.SetFluxValue(System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|rate|净生成速率|



### Properties

#### _n_AssociatedFluxObjects
利用到本代谢底物的@"T:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.FluxObject"[代谢流对象]的总数，即消耗掉本代谢物的流对象
#### _Quantity
实际浓度
#### EntityBaseType
对于细胞内的一种Entity物质实体而言，其在具有其他的功能的同时，自身也是一种代谢物，本属性表示该物质的Entity的基本的代谢物属性
#### PBS_MMF_DATA
获取一个PBS系统数据交换对象
#### Quantity
The quantity amount of this entity object instance in the system.(本实体对象在系统内的数量，在本属性中，返回的是实际浓度与本对象相关的流对象的数目的商，假若需要得到实际浓度的话，请使用@"P:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Compound.DataSource"属性，
 进行这种处理的原因是由于在实际的细胞过程之中，流对象之间都是平行发生的，并且使用本操作也可以用来表示代谢物的在整个空间范围内的均匀分布)
