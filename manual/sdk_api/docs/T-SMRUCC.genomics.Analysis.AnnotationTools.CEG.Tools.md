---
title: Tools
---

# Tools
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.CEG](N-SMRUCC.genomics.Analysis.AnnotationTools.CEG.html)_





### Methods

#### DeltaHomogeneity
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.CEG.Tools.DeltaHomogeneity(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.AnnotationTools.CEG.Tools.EssentialGeneCluster},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.PartitioningData})
```
利用CEG看家基因簇数据进行批量的核酸序列同质性的检测

#### ExportClusterNt
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.CEG.Tools.ExportClusterNt(SMRUCC.genomics.SequenceModel.FASTA.FastaToken,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,SMRUCC.genomics.Analysis.AnnotationTools.CEG.CEGAssembly)
```
结合CEG数据库从Ptt数据库之中导出每一个基因Cluster的核酸Nt序列

#### ExportEssentialGeneCluster
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.CEG.Tools.ExportEssentialGeneCluster(SMRUCC.genomics.SequenceModel.FASTA.FastaToken,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.String,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|Nt|-|
|Ptt|-|
|Annotation|-|
|AllowedGaps|所允许的基因簇之中的最大的基因空缺数目|


#### InternalEssentialGeneCluster
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.CEG.Tools.InternalEssentialGeneCluster(SMRUCC.genomics.SequenceModel.FASTA.FastaToken,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.AnnotationTools.CEG.Annotation},System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|Nt|-|
|Ptt|-|
|Annotation|-|
|AllowedGaps|所允许的基因簇之中的最大的基因空缺数目|


#### InternalGetPttData
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.CEG.Tools.InternalGetPttData(System.String)
```
{Ptt, Fna}

|Parameter Name|Remarks|
|--------------|-------|
|DIR|一个基因组的文件夹可能包含有染色体基因组和质粒基因组的数据|



