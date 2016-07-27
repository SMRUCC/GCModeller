---
title: BinaryExpression
---

# BinaryExpression
_namespace: [SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel](N-SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.html)_

这个对象表示一个基因，即网络之中的一个节点，只有1和0这两个值的半逻辑表达式，模糊逻辑的原因是逻辑取值是基于一个随机概率的



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.BinaryExpression.#ctor(System.Boolean,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|init|-|
|d|通过核酸链长度的映射得到的增长值|


#### InternalFactorDecays
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.BinaryExpression.InternalFactorDecays
```
无论是否有调控作用，都会有降解发生
> 无论链的长短，降解的速度都要一致的，即都以0.01的速度降解

#### InternalGetMostPossibleAppearState
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.BinaryExpression.InternalGetMostPossibleAppearState(System.Boolean[])
```
Gets the most possible node regulation state in current time point.(获取当前时间点之下的最有可能的节点的调控状态值)

|Parameter Name|Remarks|
|--------------|-------|
|Status|-|


#### Regulators_DynamicsRegulation
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.BinaryExpression.Regulators_DynamicsRegulation
```
有多个调控因子的时候的表达的计算公式
> 
>  算法要点
>  
>  0. 对于所有随机试验低于阈值的调控事件，都默认为不激活(即，没有调控因子的激活的话，基因不表达)
>  1. 对于同一个位点之上，假若激活的数目多余抑制的数目，则激活的权重比较大，该位点计算为激活的可能性比较高(假设转录组数据是建立在大细胞宗系的条件之下测定的)
>  2. 在Promoter区之内，假若任意一个位点被抑制，则整个基因的表达过程被抑制(单纯的分子动力学行为)
>  

#### Regulators_SiteSpecificDynamicsRegulations
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.BinaryExpression.Regulators_SiteSpecificDynamicsRegulations(SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.Regulators.RegulationExpression[])
```
函数返回是否被激活其表达过程

|Parameter Name|Remarks|
|--------------|-------|
|RegulationSites|-|


#### set_Mutation
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.BinaryExpression.set_Mutation(System.Double)
```
0 - 缺失突变，该基因的调控事件不会发生
 1 - 正常表达
 >1 - 过量表达，该基因的调控事件总会发生，因为被设置的事件概率大于1
 0-1 - 调控事件以低于平常的概率发生

|Parameter Name|Remarks|
|--------------|-------|
|factor|-|



### Properties

#### _factor
每一次调用@"M:SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.BinaryExpression.Evaluate"方法进行迭代计算，都会更新这个值，这个值由@"T:SMRUCC.genomics.Analysis.CellPhenotype.TRN.NetworkInput"[蒙特卡洛网络输入]进行初始化
#### _InternalQuantityValue
当前的这个基因所表达的mRNA分子的数量
#### _LengthDelta
由长度映射而得来的单次增长量
#### _RegulationValue
0 -- 无调控作用
 1 -- 正调控作用
 -1 -- 负调控作用
#### _value
每一次调用@"M:SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.BinaryExpression.Evaluate"方法进行迭代计算，都会更新这个值，这个值由@"T:SMRUCC.genomics.Analysis.CellPhenotype.TRN.NetworkInput"[蒙特卡洛网络输入]进行初始化
#### Handle
Handle value of this node object in the network.
#### RegulationValue
当前的这个基因所受到的表达调控作用的表述
#### RegulatorySites
这个关系是根据footprint结果得出来的
