---
title: EngineAPI
---

# EngineAPI
_namespace: [SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork](N-SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.html)_





### Methods

#### CreateObject
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.EngineAPI.CreateObject(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.RegulatesFootprints},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.NetworkInput},System.Collections.Generic.Dictionary{System.String,System.Int32})
```
从@"T:SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.RegulatesFootprints"[调控网络预测数据]之中根据调控关系创建一个逻辑网络（本方法适用于初始化最简单的调控网络模型）

|Parameter Name|Remarks|
|--------------|-------|
|footprints|-|
|LengthMapping|{基因号，核酸链长度}|

> 
>  由于有一些调控因子是找不到任何调控因子的，即该调控因子是位于网络的最上层，则这个调控因子的表达量就使用默认的输入值作为恒定值作为蒙特卡洛实验的输入值
>  


