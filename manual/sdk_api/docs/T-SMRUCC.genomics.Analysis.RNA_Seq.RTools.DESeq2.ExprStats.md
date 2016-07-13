---
title: ExprStats
---

# ExprStats
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2](N-SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.html)_





### Methods

#### GetLevel
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.ExprStats.GetLevel(System.String)
```
获取同一个样本实验里面的等级映射的结果

|Parameter Name|Remarks|
|--------------|-------|
|sample|-|


#### GetLevel2
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.ExprStats.GetLevel2(System.String)
```
获取本基因对象在不同的实验样本之间计算出来的的相对活跃度

|Parameter Name|Remarks|
|--------------|-------|
|sample|-|


#### IsActive
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.ExprStats.IsActive(System.String)
```
当前的这个基因在指定的条件下是否处于表达状态？

|Parameter Name|Remarks|
|--------------|-------|
|sample|-|


#### IsLowExpression
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.ExprStats.IsLowExpression(System.String)
```
当前的这个基因在指定的条件下是否处于低表达状态？

|Parameter Name|Remarks|
|--------------|-------|
|sample|-|



### Properties

#### LEVEL2
样品值与最大值的百分比
#### Samples
@"F:SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.ExprStats.LEVEL"[[Level]] 表达等级映射;
 [stat] 状态枚举: + 活跃，- 休眠
