---
title: Site
---

# Site
_namespace: [SMRUCC.genomics.Data.Regprecise.FastaReaders](N-SMRUCC.genomics.Data.Regprecise.FastaReaders.html)_

调控位点的数据
 > geneLocusTag:position|geneVIMSSId|regulonId|score|Bacteria



### Methods

#### Load
```csharp
SMRUCC.genomics.Data.Regprecise.FastaReaders.Site.Load(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|fasta|位点Fasta文件的文件路径|



### Properties

#### geneLocusTag
locus tag of a downstream gene in GeneBank
#### geneVIMSSId
identifier of a downstream gene in MicrobesOnline database.
 (请注意这个是基因的编号，而非这个调控位点的编号，假若需要唯一确定一个调控位点，请使用locus_tag:position的组合)
#### position
position of a regulatory site relative to the start of a downstream gene
#### regulonId
identifier of regulon
#### score
score of a regualtory site
#### SequenceData
Motif sequence
