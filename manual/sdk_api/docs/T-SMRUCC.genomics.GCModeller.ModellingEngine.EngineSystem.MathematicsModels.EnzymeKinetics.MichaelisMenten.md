---
title: MichaelisMenten
---

# MichaelisMenten
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.EnzymeKinetics](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.EnzymeKinetics.html)_

包含有ph和温度等条件

> 
>  @"T:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.GenericKinetic"对象所计算的返回值为一个普通的反应过程的当前代谢组条件下的Vmax
>  酶促反应的动力学模型
>  v=(Vmax*[s]/(Km+[S]))*f([E])*f(pH, T)
>  


### Methods

#### Factor
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.EnzymeKinetics.MichaelisMenten.Factor(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.EnzymeKinetics.EnzymeCatalystKineticLaw,System.Double,System.Double)
```
获取和环境条件相关的动力学因子

|Parameter Name|Remarks|
|--------------|-------|
|Enzyme|-|
|pH|-|
|T|-|

> 
>  分两段计算的：当小于最最佳值的时候，使用一个动力学方程
>  当大于最佳值的时候使用另外一个动力学方程
>  

#### GetFluxValue
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.EnzymeKinetics.MichaelisMenten.GetFluxValue(System.Double,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Feature.MetabolismEnzyme)
```


|Parameter Name|Remarks|
|--------------|-------|
|Vmax|@"T:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.GenericKinetic"对象所计算的返回值为一个普通的反应过程的当前代谢组条件下的Vmax|
|Enzyme|-|



### Properties

#### Get_currentPH
指针指向代谢组的@"M:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.MetabolismCompartment.Get_currentPH"[环境PH计算函数]
