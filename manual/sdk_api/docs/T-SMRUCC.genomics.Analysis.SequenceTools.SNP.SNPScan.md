---
title: SNPScan
---

# SNPScan
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SNP](N-SMRUCC.genomics.Analysis.SequenceTools.SNP.html)_





### Methods

#### ScanRaw
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SNP.SNPScan.ScanRaw(SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
```


|Parameter Name|Remarks|
|--------------|-------|
|nt|可以不经过任何处理，程序在这里会自动使用clustal进行对齐操作|


#### ScanSNPs
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SNP.SNPScan.ScanSNPs(SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.Int32,System.Boolean,System.Boolean)
```
Scan snp sites from the given fasta sequence file.

|Parameter Name|Remarks|
|--------------|-------|
|nt|序列必须都是已经经过clustal对齐了的，并且拥有FileName属性值|



