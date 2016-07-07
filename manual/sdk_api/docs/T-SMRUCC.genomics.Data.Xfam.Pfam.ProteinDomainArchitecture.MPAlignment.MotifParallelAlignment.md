---
title: MotifParallelAlignment
---

# MotifParallelAlignment
_namespace: [SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment](N-SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.html)_

从双向比对结果之中根据PfamString结果来取等价的蛋白质



### Methods

#### AlignProteins
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.MotifParallelAlignment.AlignProteins(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.BiDirectionalBesthit},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString},System.Double)
```
使用这个函数进行比对操作

|Parameter Name|Remarks|
|--------------|-------|
|besthit|-|
|query|-|
|subject|-|
|highlyScoringThreshold|-|


#### Convert
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.MotifParallelAlignment.Convert(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.AlignmentOutput})
```
将比对结果转换为Csv文件，之后可以在Excel之中按照自己的需求进行数据筛选

|Parameter Name|Remarks|
|--------------|-------|
|Besthits|-|
|DomainAlign|-|


#### Convert``2
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.MotifParallelAlignment.Convert``2(System.Collections.Generic.IEnumerable{``0},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.AlignmentOutput},System.Func{SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.AlignmentOutput,``0,``1})
```
将比对结果转换为Csv文件，之后可以在Excel之中按照自己的需求进行数据筛选

|Parameter Name|Remarks|
|--------------|-------|
|Besthits|-|
|DomainAlign|-|


#### SelectSource``1
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.MotifParallelAlignment.SelectSource``1(System.Collections.Generic.IEnumerable{``0},SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.Func{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit,SMRUCC.genomics.SequenceModel.FASTA.FastaToken,System.Boolean})
```
这个是为了Pfam-A分析而准备的，比对Pfam-A数据库会产生很大的数据，则在比对之前先使用本方法挑选出符合条件的Subject，以减少BLASTP的时间以及日志文件的大小

|Parameter Name|Remarks|
|--------------|-------|
|Besthits|-|
|SubjectFasta|-|
|SelectMethod|-|


#### SelectUniprot
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.MotifParallelAlignment.SelectUniprot(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit},SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
```
根据基因组和Uniprot的双向比对结果，来选择需要进行Pfam-A分析的Uniprot蛋白序列

|Parameter Name|Remarks|
|--------------|-------|
|Besthits|-|
|subjects|-|



