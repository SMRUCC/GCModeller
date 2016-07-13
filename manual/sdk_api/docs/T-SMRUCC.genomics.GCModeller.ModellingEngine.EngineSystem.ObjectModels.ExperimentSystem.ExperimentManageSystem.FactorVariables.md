---
title: FactorVariables
---

# FactorVariables
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.ExperimentSystem.ExperimentManageSystem](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.ExperimentSystem.ExperimentManageSystem.html)_

本类型的实验是通过控制代谢物的浓度来进行的，故而@"F:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.ExperimentSystem.ExperimentManageSystem.FactorVariables.Target"所指向的目标对象为@"P:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.MetabolismCompartment.Metabolites"



### Methods

#### CreateObject
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.ExperimentSystem.ExperimentManageSystem.FactorVariables.CreateObject(SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels.Experiment,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
```


|Parameter Name|Remarks|
|--------------|-------|
|ModelBase|目标对象之中的@"P:SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels.Experiment.TriggedCondition"[触发条件]为一个纯数字|
|MetabolismSystem|-|



### Properties

#### Id
The name Id of the target.
 (目标的名称)
#### Interval
The interval ticks between each kick.
 (每次干扰动作执行的时间间隔)
#### Kicks
The counts of the kicks.
 (执行的次数)
#### Methods
顺序与@"T:SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels.Experiment.Types"[Types]相对应：
 Increase
 Decrease
 Multiplying
 Decay
 Mod
 ChangeTo
#### Start
The start time of this disturb.
 (这个干扰动作的开始时间)
#### Target
Target
#### TargetInvoke_GetValue
Method for get the current value of target
#### TargetInvoke_SetValue
Method for set the value for target
