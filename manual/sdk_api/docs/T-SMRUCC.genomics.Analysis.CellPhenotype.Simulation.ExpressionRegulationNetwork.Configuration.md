---
title: Configuration
---

# Configuration
_namespace: [SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork](N-SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.html)_





### Methods

#### DefaultValue
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.Configuration.DefaultValue
```
Get the default configuration data for the DFL network simulator.

#### LoadConfiguration
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.Configuration.LoadConfiguration(System.String)
```
从文件之中读取配置数据

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|



### Properties

#### BasalThreshold
本底表达水平，数值越高，则表达量越高
#### OCS_Default_EffectValue
默认的调控值
#### OCS_NONE_Effector
没有在模型之中找到代谢物的合成的代谢途径，则可能为第二信使或者其他未知的原因，则在模型之中以很低的概率产生调控效应，这个参数配置产生活性的概率的高低
#### SiteSpecificDynamicsRegulations
这个参数值调整调控事件的发生概率阈值的高低，则阈值越低，即调控事件越容易发生
