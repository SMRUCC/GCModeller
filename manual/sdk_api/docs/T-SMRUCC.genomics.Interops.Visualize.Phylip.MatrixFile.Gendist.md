---
title: Gendist
---

# Gendist
_namespace: [SMRUCC.genomics.Interops.Visualize.Phylip.MatrixFile](N-SMRUCC.genomics.Interops.Visualize.Phylip.MatrixFile.html)_

在做motif的分布密度的时候，将每一种类型的motif看作为一个等位基因



### Methods

#### GenerateDocument
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.MatrixFile.Gendist.GenerateDocument
```
主要是生成没有设置有A选项的文件数据

#### LoadDocument
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.MatrixFile.Gendist.LoadDocument(System.String)
```
加载已经生成的gendist矩阵文件之中的数据

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|


#### SubMatrix
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.MatrixFile.Gendist.SubMatrix(System.Int32,System.String)
```
采集至少**Count**数量的和**MainIndex**相近的基因组

|Parameter Name|Remarks|
|--------------|-------|
|Count|-|
|MainIndex|-|



### Properties

#### NumberOfAlleles
There then follows a line which gives the numbers of alleles at each locus, in order. This must be the full number of alleles,
 not the number of alleles which will be input: i. e. for a two-allele locus the number should be 2, not 1.
#### NumberOfLoci
motif的数目
#### SpeciesCount
基因组的数目
