---
title: Regulator`1
---

# Regulator`1
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.html)_

对表达过程其调控作用的生物大分子，在数据模型之中，调控因子是一个调控因子对多个调控对象的，当被转换为对象模型之后，
 则变成了一个调控因子对一个调控位点或者调控对象，即在创建对象的时候，调控因子被按照调控位点进行复制拆分



### Methods

#### CreateObject
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Regulator`1.CreateObject(SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
```


|Parameter Name|Remarks|
|--------------|-------|
|ModelBase|-|
|Metabolism|-|


#### get_RegulatorValue
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Regulator`1.get_RegulatorValue
```
获取当前的这个调控因子所调控的目标位点上面的所有调控因子的数量的总和


### Properties

#### _Abs_Weight
这个权重值决定了一个调控事件的发生的概率值的高低
#### _InteractionTarget
调控因子对象是被寄生在本目标对象之上的
#### _Weight
这个权重值决定了一个调控事件的发生的概率值的高低
#### Quantity
这个调控因子的分子数量
#### RegulateValue

#### Weight
由Pcc计算而来，正负调控效果包含于Weight的符号之中了
