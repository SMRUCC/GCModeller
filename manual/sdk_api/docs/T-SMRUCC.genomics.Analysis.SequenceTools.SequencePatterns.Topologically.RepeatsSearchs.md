---
title: RepeatsSearchs
---

# RepeatsSearchs
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically](N-SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.html)_





### Methods

#### __beginInit
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsSearchs.__beginInit(Microsoft.VisualBasic.List{System.String}@)
```
只搜索出现的序列片段，假若短片段就已经不存在于序列之上，则后面的延伸序列肯定也不存在，则删除这部分的序列

|Parameter Name|Remarks|
|--------------|-------|
|seeds|-|


#### __postResult
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsSearchs.__postResult(System.String[],Microsoft.VisualBasic.List{System.String},System.Int32)
```
虽然当前的片段没有在序列上面出现，但是上一次迭代的片段却是出现的，假若序列片段没有匹配上，则上一次迭代的序列则可能为重复序列

|Parameter Name|Remarks|
|--------------|-------|
|currentStat|-|



### Properties

#### CountStatics
片段按照长度的数量上的分布情况
