---
title: KaryotypeChromosomes
---

# KaryotypeChromosomes
_namespace: [SMRUCC.genomics.Visualize.Circos.Karyotype](N-SMRUCC.genomics.Visualize.Circos.Karyotype.html)_

The very basically genome skeleton information description.(基因组的基本框架的描述信息)



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Visualize.Circos.Karyotype.KaryotypeChromosomes.#ctor(System.Int32,System.String,Microsoft.VisualBasic.ComponentModel.TripleKeyValuesPair[])
```
这个构造函数是用于单个染色体的

|Parameter Name|Remarks|
|--------------|-------|
|gSize|The genome size.|
|color|-|
|bandData|@"P:Microsoft.VisualBasic.ComponentModel.TripleKeyValuesPair.Key"为颜色，其余的两个属性分别为左端起始和右端结束|


#### FromBlastnMappings
```csharp
SMRUCC.genomics.Visualize.Circos.Karyotype.KaryotypeChromosomes.FromBlastnMappings(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BlastnMapping},System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.FASTA.FastaToken})
```
Creates the model for the multiple chromosomes genome data in circos.(使用这个函数进行创建多条染色体的)

|Parameter Name|Remarks|
|--------------|-------|
|source|Band数据|
|chrs|karyotype数据|



