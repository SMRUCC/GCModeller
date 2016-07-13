---
title: API
---

# API
_namespace: [SMRUCC.genomics.Analysis.CRISPR.IBCB.sgRNAcas](N-SMRUCC.genomics.Analysis.CRISPR.IBCB.sgRNAcas.html)_

sgRNAcas9 --- a tool for fast designing CRISPR sgRNA with high specificity. 
 
 If you use this program in your research, please cite:
 Xie S, Shen B, Zhang C, Huang X * and Zhang Y *. sgRNAcas9: a software package for designing CRISPR sgRNA and evaluating potential off-target cleavage sites. PloS one, 2014
 Please send bug reports to: ssxieinfo@gmail.com



### Methods

#### sgRNAcas
```csharp
SMRUCC.genomics.Analysis.CRISPR.IBCB.sgRNAcas.API.sgRNAcas(System.String,System.String,System.String,System.Int32,System.Int32,System.Int32,System.String,System.String,System.String,System.Int32,System.Int32,System.Int32)
```
sgRNAcas9 --- a tool for fast designing CRISPR sgRNA with high specificity

|Parameter Name|Remarks|
|--------------|-------|
|InputFile|Input file|
|Length|Lenght of sgRNA|
|MinGC|-|
|MaxGC|-|
|RefGenome|-|
|Option|-|
|SearchModel|-|
|OSVersion|-|
|Mismatches|-|
|MinOffSet|-|
|MaxOffSet|-|
|Output|-|



### Properties

#### PerlScriptBin
Perl脚本程序的存放的文件夹
