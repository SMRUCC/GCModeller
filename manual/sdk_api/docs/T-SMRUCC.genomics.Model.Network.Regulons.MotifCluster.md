---
title: MotifCluster
---

# MotifCluster
_namespace: [SMRUCC.genomics.Model.Network.Regulons](N-SMRUCC.genomics.Model.Network.Regulons.html)_

需要先将motif映射为长度一致的实体！
 先进行TOM全局比对，然后取match部分，之后就可以进行聚类了？



### Methods

#### Mappings
```csharp
SMRUCC.genomics.Model.Network.Regulons.MotifCluster.Mappings(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel},SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.Parameters)
```
首先和目标表型的LDM进行全局比对做Mapping映射为长度一致的实体对象

|Parameter Name|Remarks|
|--------------|-------|
|source|无法进行比对的结果将会被忽略掉|
|LDM|-|
|param|TOM全局比对的参数信息|



### Properties

#### NullSite
x.Bits + x.PWM.Average => 1+ATGC/4
 x.Bits + x.PWM.Average => 1+{0.25, 0.25, 0.25, 0.25}/4
