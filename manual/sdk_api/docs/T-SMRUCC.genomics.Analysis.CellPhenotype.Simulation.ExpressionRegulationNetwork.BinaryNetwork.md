---
title: BinaryNetwork
---

# BinaryNetwork
_namespace: [SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork](N-SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.html)_

使用逻辑值来模拟计算基因表达调控网络

> 
>  这里是现实之中的约束条件：
>  
>  1. 合成速度：
>  假设对于每一个碱基而言，其合成的速度是一样的，那么很显然单位时间内，越长的核酸链其合成速度越慢，即每一次迭代循环过程之中，假若该基因的长度越长，则相较于较短的核酸链产生的分子数目越少
>  2. 降解速度：
>  假设酶水解下一个碱基都是以相同的速度，那么水解完一条较长的核酸链很显然相较于较短的核酸链会需要更加长的时间。但是当水解掉最开始的一个碱基之后，我们假设原有的有活性的核酸链将无法再被用于翻译，故而在降解速度方面，较长的核酸链和较短的核酸链都是相同的失活速度的
>  
>  


### Methods

#### __innerTicks
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.BinaryNetwork.__innerTicks(System.Int32)
```
返回调控网络之中处于表达状态的基因的数目

#### AllRegulatorInputs
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.BinaryNetwork.AllRegulatorInputs(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.RegulatesFootprints})
```
将所有的调控因子都作为蒙特卡洛实验的输入

|Parameter Name|Remarks|
|--------------|-------|
|footprints|-|


#### AnalysisMonteCarloTopLevelInput
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.BinaryNetwork.AnalysisMonteCarloTopLevelInput(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.RegulatesFootprints})
```
Monte Carlo experiment input analysis.(分析出最顶层的调控因子作为蒙特卡洛实验的输入之一)
> 由于有一些调控因子是找不到任何调控因子的，即该调控因子是位于网络的最上层，则这个调控因子的表达量就使用默认的输入值作为恒定值作为蒙特卡洛实验的输入值

#### GetGeneObjects
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.BinaryNetwork.GetGeneObjects
```
Gets the genes unique id collection.(获取本网络模型对象之中的基因的编号列表)

#### GetRegulator
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.BinaryNetwork.GetRegulator(System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.BinaryExpression},System.String)
```
DEBUG

|Parameter Name|Remarks|
|--------------|-------|
|dict|-|
|id|-|


#### SetKernelLoops
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.BinaryNetwork.SetKernelLoops(System.Int32)
```
Setup the internal kernel cycles number of this binary cellular gene expression regulation network model.(对本计算模型设置内核循环的数目。)

|Parameter Name|Remarks|
|--------------|-------|
|KelCycles|The kernel cycle vaslue that will be setup as the runtime parameter of the model simulation.|


#### SetMutationFactor
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.BinaryNetwork.SetMutationFactor(System.String,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|GeneID|-|
|Factor|
 0 - 缺失突变
 1 - 正常表达
 >1 - 过量表达
 0-1 - 调控事件以低于平常的概率发生 
 |



### Properties

#### NonRegulationHandles
Gets all of the gene nodes handles collection in this expression regulation network.(获取在网络结构之上没有受到任何调控作用的基因网络节点)
