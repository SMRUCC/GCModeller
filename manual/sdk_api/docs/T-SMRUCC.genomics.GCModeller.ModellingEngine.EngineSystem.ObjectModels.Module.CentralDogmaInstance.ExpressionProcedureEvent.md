---
title: ExpressionProcedureEvent
---

# ExpressionProcedureEvent
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.html)_

对一个基因对象的表达过程中的一个步骤的描述



### Methods

#### CreateInstance
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.ExpressionProcedureEvent.CreateInstance(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Transcript,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
```
从一个RNA分子之上可以构建出一个转录过程和一个翻译过程，假若其为一个mRNA的话

|Parameter Name|Remarks|
|--------------|-------|
|ExpressionObject|-|
|Transcript|-|



### Properties

#### _Regulations
调控因子对本过程对象的调控作用的总和大小
#### CompositionDelayEffect
现在先假设所有的链分子的单元延伸速度都是一样的，故而对于长度越长的分子而言，其单位时间内所合成的数量就会越少，故而在所有的转录和翻译事件之中都会除以本参数用来表示由于长度所带来的效应
#### CompositionVector
转录或者翻译过程中所需要的组分向量
#### RegulationValue
调控因子对本过程对象的调控作用的总和大小
#### Template
本步骤的信息来源：模板分子对象
