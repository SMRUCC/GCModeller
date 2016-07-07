---
title: Site
---

# Site
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.html)_





### Methods

#### GetDist
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.Site.GetDist(SMRUCC.genomics.ComponentModel.Loci.Strands)
```


|Parameter Name|Remarks|
|--------------|-------|
|strand|这个位点所处在的基因@"P:SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM.Site.Name"的链的方向|


#### GetLoci
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.Site.GetLoci(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT)
```
假若这个位点的名称是基因号的话，可以使用这个方法得到基因组上面的位置，这个函数只适用于ATG上游的情况


### Properties

#### Regulators
@"P:SMRUCC.genomics.Data.Regprecise.WebServices.JSONLDM.regulator.vimssId"
