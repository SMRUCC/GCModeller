---
title: Parameters
---

# Parameters
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.html)_

Motif相似度比较的参数输入的集合



### Methods

#### SiteScanProfile
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.Parameters.SiteScanProfile
```
由于序列的残疾出现频率是1，所以阈值过高会很容易筛掉可能的序列位点


### Properties

#### MinW
m字符的结果的位点的数量或者SW-TOM里面的高分区的最短的片段长度
#### SWOffset
创建Smith-waterman高分区的时候，计算出相似度之后向得分转换过程之中的偏移值
#### SWThreshold
筛选高分区的时候使用
