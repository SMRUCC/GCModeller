---
title: MetabolismFlux
---

# MetabolismFlux
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.html)_

Metaolism reaction basetype in GCModeller ObjectModels.(GCModeller计算引擎之中的代谢反应对象模型类型的基类)



### Methods

#### GetCoefficient
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.MetabolismFlux.GetCoefficient(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|UniqueId|-|


#### Initialize
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.MetabolismFlux.Initialize(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Compound[],Microsoft.VisualBasic.Logging.LogFile)
```
将SpeciesReference转换为相应的代谢物的对象模型，其实最主要的部分是对动力学模型的初始化操作

|Parameter Name|Remarks|
|--------------|-------|
|Metabolites|-|


#### Invoke
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.MetabolismFlux.Invoke
```
执行一次反应


### Properties

#### _Metabolites
使用Reactants字段域与Products字段域所生成的属性，表示为本代谢流对象中所涉及到的所有的代谢底物的集合
