---
title: FBAIterator
---

# FBAIterator
_namespace: [SMRUCC.genomics.Analysis.FBA_DP](N-SMRUCC.genomics.Analysis.FBA_DP.html)_

FBA状态迭代器



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.FBA_DP.FBAIterator.#ctor(SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel,System.Func{System.String,System.Double},System.Int32,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|model|-|
|getInit|Get metabolite init amount.|


#### __overridesBounds
```csharp
SMRUCC.genomics.Analysis.FBA_DP.FBAIterator.__overridesBounds(System.String,System.Int32,System.Double)
```
根据物质守恒计算出上界

|Parameter Name|Remarks|
|--------------|-------|
|rxn|-|
|dir|表示约束的方向，只有1和-1这两个值|
|curr|-|


#### __upperBounds
```csharp
SMRUCC.genomics.Analysis.FBA_DP.FBAIterator.__upperBounds(System.String,System.Double)
```
在这里根据代谢物的剩余的数量更新每一个rxn的上下限

#### Run
```csharp
SMRUCC.genomics.Analysis.FBA_DP.FBAIterator.Run
```
每迭代一个就会计算出每一种代谢物的被消耗掉的量。
 （假若FBA的代谢物的约束为等于0的话，则这个计算模型不可用，但是更改了约束条件还可以被称作为FBA么？）


### Properties

#### __lpModel
FBA计算模型
#### _iterates
迭代的次数
