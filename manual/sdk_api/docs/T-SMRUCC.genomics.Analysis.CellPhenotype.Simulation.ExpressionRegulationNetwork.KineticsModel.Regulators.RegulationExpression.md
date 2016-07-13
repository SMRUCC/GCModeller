---
title: RegulationExpression
---

# RegulationExpression
_namespace: [SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators](N-SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators.html)_

表示调控因子与调控的基因之间的关系，@"P:SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators.RegulationExpression.Weight"用于表示调控的Effect出现的事件概率值的高低，
 当然，对于本类型的对象，你也可以将其当作为一个调控因子，并且这种类型的调控因子为自由类型的调控因子，即不需要任何外部的附加条件既
 可以产生调控功能的调控因子



### Methods

#### set_TargetSite
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators.RegulationExpression.set_TargetSite(SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.SiteInfo)
```
调控因子所调控的实际对象是一个调控位点


### Properties

#### Quantity
数量越多，则权重越大，则概率事件的阈值就越低，即该调控事件越容易发生
#### RegulationFunctional
返回调控的效果，TRUE表示激活，FALSE表示抑制；根据Pcc权重计算出来调控因子对目标基因的表达调控的可能性
#### Repression
@"P:SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators.RegulationExpression.Weight"的符号，调控的效应，当为真的时候，为抑制作用，当为假的时候，为激活作用，Pcc符合随机事件发生的条件的时候，目标对象将会被设置为假
#### Weight
@"P:SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators.RegulationExpression.Weight"的值有符号，其中符号表示调控效应：激活或者抑制，为了方便计算，这里取绝对值，
 @"P:SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators.RegulationExpression.Weight"值越大，则@"F:Microsoft.VisualBasic.DataMining.Framework.DFL_Driver.I_FactorElement._ABS_Weight"变量的值越小，即该调控事件越容易发生
