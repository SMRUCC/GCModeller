---
title: IQueryExtensions
---

# IQueryExtensions
_namespace: [SMRUCC.genomics](N-SMRUCC.genomics.html)_

Extensions for object query in the GCModeller biological components



### Methods

#### MatchGene
```csharp
SMRUCC.genomics.IQueryExtensions.MatchGene(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.String,System.Collections.Generic.IEnumerable{System.String},System.Double)
```
函数会优先按照**geneName**进行查询，假若查找不到结果，才会使用**products**列表进行模糊匹配

|Parameter Name|Remarks|
|--------------|-------|
|PTT|-|
|geneName|-|
|products|-|
|cut|字符串匹配相似度的最小的阈值|



