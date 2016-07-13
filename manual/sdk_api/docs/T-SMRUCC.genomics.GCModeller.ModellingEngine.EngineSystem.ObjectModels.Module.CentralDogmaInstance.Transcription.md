---
title: Transcription
---

# Transcription
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.html)_

The transcription event for a @"T:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Feature.Gene" object.(一个基因对象的转录过程)



### Methods

#### CreateConstraintFlux
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.Transcription.CreateConstraintFlux(System.String,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.MetabolismCompartment,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Feature.MetabolismEnzyme[],SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.EnzymeKinetics.ExpressionKinetics,System.Double,System.Int32[])
```
生成转录模型的约束条件

|Parameter Name|Remarks|
|--------------|-------|
|MetabolismSystem|-|
|RNAPolymeraseEntity|-|
|EnzymeKinetics|-|
|K1|-|



### Properties

#### CoefficientFactor
1表示正常表达水平，0表示缺失突变，1~0表示低量表达，>1表示过量表达
