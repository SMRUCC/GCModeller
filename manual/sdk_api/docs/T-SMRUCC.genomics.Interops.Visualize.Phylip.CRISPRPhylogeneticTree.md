---
title: CRISPRPhylogeneticTree
---

# CRISPRPhylogeneticTree
_namespace: [SMRUCC.genomics.Interops.Visualize.Phylip](N-SMRUCC.genomics.Interops.Visualize.Phylip.html)_





### Methods

#### GetRange
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.CRISPRPhylogeneticTree.GetRange(SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit,System.String,SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.HitCollection,SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.HitCollection,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo})
```
由于是在一个所指定的范围之内，所以没有查照到的时候，start往后移动，ends则会往前移动

|Parameter Name|Remarks|
|--------------|-------|
|besthits|假设里面的基因都是已经按照顺序排序了的|
|start|-|
|ends|-|
|subjectTag|目标基因组的标签信息|


#### TrimData
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.CRISPRPhylogeneticTree.TrimData(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.CRISPR.CRT.Output.GenomeScanResult},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo},SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit,System.String,System.String)
```
假若在进行质粒的进化树构建的时候，由于外来基因的水平转移的原因，假若包含有大片段的比对上的基因的话，会对进化树的构建结果产生很大的影响。
 故而将已经比对上的区域之中的CRISPR位点的数据进行删除，以消除影响。请注意，构建质粒的进化树一定要使用本方法进行数据的筛选

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|cds_Info|-|
|besthits|-|
|ends|片段区域结束的基因|
|start|片段区域起始的基因|



