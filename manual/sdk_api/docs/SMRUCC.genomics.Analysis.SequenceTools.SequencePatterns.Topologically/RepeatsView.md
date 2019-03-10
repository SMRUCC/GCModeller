﻿# RepeatsView
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically](./index.md)_

正向重复位点的序列模型



### Methods

#### ToVector``1
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsView.ToVector``1(System.Collections.Generic.IEnumerable{``0},System.Int64)
```
返回的是基因组上面的每一个位点的热度的列表

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|size|-|



### Properties

#### Hot
平均距离越小，则热度越高
 位点越多，热度越高
 片段越长，热度越高
#### IntervalAverages
每个重复的片段之间平均的间隔长度
