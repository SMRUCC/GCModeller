---
title: Contig
---

# Contig
_namespace: [SMRUCC.genomics.SequenceModel.NucleotideModels](N-SMRUCC.genomics.SequenceModel.NucleotideModels.html)_

这个基础的模型对象只有在基因组上面的位置信息



### Methods

#### #ctor
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Contig.#ctor(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```
这个构造函数已经使用@"M:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.Copy"函数从数据源**mappinglocation**进行复制

|Parameter Name|Remarks|
|--------------|-------|
|MappingLocation|-|


#### GetRelatedUpstream
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Contig.GetRelatedUpstream(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Int32)
```
会同时将上游以及上游重叠的基因都找出来

|Parameter Name|Remarks|
|--------------|-------|
|PTT|-|



### Properties

#### MappingLocation
在参考基因组上面的Mapping得到的位置，假若需要修改位置，假若害怕影响到原有的数据的话，则请复写这个属性然后使用复制的方法得到新的位点数据
