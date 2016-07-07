---
title: OCS
---

# OCS
_namespace: [SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators](N-SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators.html)_

需要Effector



### Methods

#### Internal_getPathwayEffector
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators.OCS.Internal_getPathwayEffector(Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairData{SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.BinaryExpression[]})
```
所有的基因为AND运算

|Parameter Name|Remarks|
|--------------|-------|
|Pathway|-|



### Properties

#### Effector
对@"P:SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators.OCS.EffectorPathways"的计算结果，值用来表述概率事件的发生的可能性的高低，值越低则越难发生
#### EffectorPathways
这些代谢途径都是和Effector的合成相关的，在每一个对象之中，其Value值为该代谢途径之中的所有的基因，而Key的值则表示为该代谢途径的编号，每一个对象都做加法运算，而每一个对象内部的基因对象之间都做AND逻辑运算
#### RegulationFunctional
继承的对象和基本对象之间的实现是有差异的，基本对象直接使用state<>0来表述，因为@"P:Microsoft.VisualBasic.DataMining.Framework.DFL_Driver.I_FactorElement.FunctionalState"已经包含有模糊逻辑判断了，只要返回非零值，就表示事件发生了，而本属性则是从头计算
