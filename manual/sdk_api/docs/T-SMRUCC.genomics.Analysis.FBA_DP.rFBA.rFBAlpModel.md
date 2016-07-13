---
title: rFBAlpModel
---

# rFBAlpModel
_namespace: [SMRUCC.genomics.Analysis.FBA_DP.rFBA](N-SMRUCC.genomics.Analysis.FBA_DP.rFBA.html)_





### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.FBA_DP.rFBA.rFBAlpModel.#ctor(SMRUCC.genomics.Model.SBML.Level2.XmlFile,System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|SBMl|-|
|objectiveFunction|UniqueId list for the target metabolism reactions.(代谢反应对象的UniqueId列表)|


#### ApplyFluxConstraint
```csharp
SMRUCC.genomics.Analysis.FBA_DP.rFBA.rFBAlpModel.ApplyFluxConstraint(System.String,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|Id|目标基因的基因号|
|UpperBound|流量上限，即该基因的当前所计算出来的表达量|


#### getMatrix
```csharp
SMRUCC.genomics.Analysis.FBA_DP.rFBA.rFBAlpModel.getMatrix
```
网络结构不会有变化


### Properties

#### FluxConstraints
{GeneId, {Reversible, UpperBound}}()，与Columns中的元素的位置一致，并且假设上限和下限在可逆反应中是一致的
#### MAT_S
S matrix in FBA model.(FBA模型中的S矩阵)
