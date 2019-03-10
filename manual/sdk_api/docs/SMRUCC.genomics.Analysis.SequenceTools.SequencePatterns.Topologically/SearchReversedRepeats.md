﻿# SearchReversedRepeats
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically](./index.md)_





### Methods

#### __getRemovedList
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.SearchReversedRepeats.__getRemovedList(Microsoft.VisualBasic.Language.List{System.String}@)
```
获取将要进行移除的片段列表

|Parameter Name|Remarks|
|--------------|-------|
|currentStat|-|


#### __postResult
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.SearchReversedRepeats.__postResult(System.String[],Microsoft.VisualBasic.Language.List{System.String},System.Int32)
```
虽然当前的片段没有在序列上面出现，但是上一次迭代的片段却是出现的，假若序列片段没有匹配上，则上一次迭代的序列则可能为重复序列

|Parameter Name|Remarks|
|--------------|-------|
|currentRemoves|-|
|currentStat|-|
|currLen|-|



### Properties

#### CountStatics
片段按照长度的数量上的分布情况
