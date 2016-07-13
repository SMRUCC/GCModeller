---
title: RegulonAPI
---

# RegulonAPI
_namespace: [SMRUCC.genomics.Data.Regprecise](N-SMRUCC.genomics.Data.Regprecise.html)_

根据直系同源的方法能够真的重建出Regulon么？



### Methods

#### __getOperons
```csharp
SMRUCC.genomics.Data.Regprecise.RegulonAPI.__getOperons(SMRUCC.genomics.Data.Regprecise.RegulatedGene[],SMRUCC.genomics.Assembly.DOOR.DOOR)
```
从需要被注释的基因组之中获取得到被调控基因所在的操纵子

|Parameter Name|Remarks|
|--------------|-------|
|mappings|-|
|DOOR|-|


#### Reconstruct
```csharp
SMRUCC.genomics.Data.Regprecise.RegulonAPI.Reconstruct(System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit[]},SMRUCC.genomics.Data.Regprecise.Regulator,SMRUCC.genomics.Assembly.DOOR.DOOR)
```
分别比较调控因子和被调控的基因，被调控的基因应该是在一个操纵子里面的

|Parameter Name|Remarks|
|--------------|-------|
|mappings|-|
|regulon|RegPrecise数据库之中的数据|


#### uid
```csharp
SMRUCC.genomics.Data.Regprecise.RegulonAPI.uid(SMRUCC.genomics.Data.Regprecise.Regulator)
```
这个函数主要是用来唯一标示meme的分析结果的

|Parameter Name|Remarks|
|--------------|-------|
|regulator|-|



