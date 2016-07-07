---
title: Regulator
---

# Regulator
_namespace: [SMRUCC.genomics.Data.Regprecise](N-SMRUCC.genomics.Data.Regprecise.html)_





### Methods

#### ExportMotifs
```csharp
SMRUCC.genomics.Data.Regprecise.Regulator.ExportMotifs
```
这个函数会自动移除一些表示NNNN的特殊符号

#### GetMotifSite
```csharp
SMRUCC.genomics.Data.Regprecise.Regulator.GetMotifSite(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|trace|locus_tag:position|



### Properties

#### LocusId
该Regprecise调控因子的基因号
#### lstOperon
被这个调控因子所调控的基因，按照操纵子进行分组，这个适用于推断Regulon的
