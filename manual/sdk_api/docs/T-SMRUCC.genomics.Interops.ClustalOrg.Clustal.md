---
title: Clustal
---

# Clustal
_namespace: [SMRUCC.genomics.Interops.ClustalOrg](N-SMRUCC.genomics.Interops.ClustalOrg.html)_

Clustal Omega(多序列比对工具)
 Clustal Omega - 1.2.0 (AndreaGiacomo)

 If you Like Clustal - Omega please cite:
 Sievers F, Wilm A, Dineen D, Gibson TJ, Karplus K, Li W, Lopez R, McWilliam H, Remmert M, Sding J, Thompson JD, Higgins DG.
 Fast, scalable generation Of high-quality protein multiple sequence alignments Using Clustal Omega.
 Mol Syst Biol. 2011 Oct 11;7:539. doi: 10.1038/msb.2011.75. PMID: 21988835.
 If you don't like Clustal-Omega, please let us know why (and cite us anyway).

 Check http : //www.clustal.org for more information And updates.



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Interops.ClustalOrg.Clustal.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Exe|Clustal可执行文件的文件路径|


#### Align
```csharp
SMRUCC.genomics.Interops.ClustalOrg.Clustal.Align(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.FASTA.FastaToken})
```
这个是通过标准输入来传递序列数据的

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


#### MultipleAlignment
```csharp
SMRUCC.genomics.Interops.ClustalOrg.Clustal.MultipleAlignment(System.String)
```
目标多序列比对文件的文件路径，出错会返回空值

|Parameter Name|Remarks|
|--------------|-------|
|source|-|



### Properties

#### CLUSTAL_ARGUMENTS

