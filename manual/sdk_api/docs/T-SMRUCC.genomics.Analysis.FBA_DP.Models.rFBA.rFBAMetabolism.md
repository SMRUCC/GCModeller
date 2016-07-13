---
title: rFBAMetabolism
---

# rFBAMetabolism
_namespace: [SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA](N-SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA.html)_

Model builder for rFBA(FBA system with gene expression regulation) metabolism system

> 
>  关于上下限：
>  非酶促过程使用sbml文件里面的默认参数
>  拥有基因的酶促反应过程则根据调控因子的数量来计算出相应的上下限
>  假若调控因子或者酶分子被缺失，则使用@"P:SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA.rFBA_ARGVS.baseFactor"本底表达或者酶促过程最低的速率来进行
>  


### Methods

#### __calFactor
```csharp
SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA.rFBAMetabolism.__calFactor(System.String,Microsoft.VisualBasic.List{System.String})
```
计算出目标基因对于反应的流量的上下限的影响大小

|Parameter Name|Remarks|
|--------------|-------|
|locus|-|


#### __regImpact
```csharp
SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA.rFBAMetabolism.__regImpact(System.String,Microsoft.VisualBasic.List{System.String})
```
计算出调控的影响大小

|Parameter Name|Remarks|
|--------------|-------|
|locus|-|
|trace|
 因为假若两个调控因子相互之间有调控关系，则在递归的过程之中会出现回路造成死循环，所以使用这个参数来避免这个问题
 |


#### SetObjectiveGenes
```csharp
SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA.rFBAMetabolism.SetObjectiveGenes(System.Collections.Generic.IEnumerable{System.String})
```
找出和基因相关的反应过程

|Parameter Name|Remarks|
|--------------|-------|
|locus|-|


#### SetRPKM
```csharp
SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA.rFBAMetabolism.SetRPKM(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.ExprStats},System.String)
```
假若已经通过@"P:SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA.rFBAMetabolism.GeneFactors"设置了基因的突变的状态，那么笨函数将不会修改已经设置了的突变状态值

|Parameter Name|Remarks|
|--------------|-------|
|rpkm|-|
|sample|-|



### Properties

#### __regulations
{ORF, regulators}, 假设存在这个列表里面的都是受表达调控的，而不存在的则其表达是自由的
#### GeneFactors
基因的突变设置参数
 
 0 缺失突变
 1 正常表达
 0-1 低表达
 >1 高表达
