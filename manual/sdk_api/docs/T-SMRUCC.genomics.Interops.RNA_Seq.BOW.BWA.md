---
title: BWA
---

# BWA
_namespace: [SMRUCC.genomics.Interops.RNA_Seq.BOW](N-SMRUCC.genomics.Interops.RNA_Seq.BOW.html)_

Program: bwa (alignment via Burrows-Wheeler transformation)
 Contact: Heng Li <lh3@sanger.ac.uk>

> 
>  
>  Program: bwa (alignment via Burrows-Wheeler transformation)
>  Version: 0.6.1-r104
>  Contact: Heng Li <lh3@sanger.ac.uk>
>  
>  Usage:   bwa <command> [options]
>  
>  Command: index         index sequences in the FASTA format
>           aln           gapped/ungapped alignment
>           samse         generate alignment (single ended)
>           sampe         generate alignment (paired ended)
>           bwasw         BWA-SW for long queries
>           fastmap       identify super-maximal exact matches
>  
>           fa2pac        convert FASTA to PAC format
>           pac2bwt       generate BWT from PAC
>           pac2bwtgen    alternative algorithm for generating BWT
>           bwtupdate     update .bwt to the new format
>           bwt2sa        generate SA from BWT and Occ
>           pac2cspac     convert PAC to color-space PAC
>           stdsw         standard SW/NW alignment
>  
>  


### Methods

#### Bwasw
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Bwasw
```
BWA-SW for long queries

#### Bwt2sa
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Bwt2sa
```
generate SA from BWT and Occ

#### Bwtupdate
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Bwtupdate
```
update .bwt to the new format

#### Fa2pac
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Fa2pac
```
convert FASTA to PAC format

#### Fastmap
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Fastmap
```
identify super-maximal exact matches

#### Index
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Index(System.String,System.String)
```
index sequences in the FASTA format

#### Pac2bwt
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Pac2bwt
```
generate BWT from PAC

#### Pac2bwtgen
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Pac2bwtgen
```
alternative algorithm for generating BWT

#### Pac2cspac
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Pac2cspac
```
convert PAC to color-space PAC

#### PaireEndMapping
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.PaireEndMapping(System.String,System.String,System.String,System.String)
```
双向测序的数据

|Parameter Name|Remarks|
|--------------|-------|
|Left|left fastq|
|Right|right fastq|
|Reference|参考基因组的fasta序列的文件路径|


#### Sampe
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Sampe(System.String,System.String,System.String,System.String,System.String,System.String)
```
generate alignment (paired ended)

#### Samse
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Samse(System.String,System.String,System.String,System.String)
```
Generate alignment (single ended)

#### SingleEndMapping
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.SingleEndMapping(System.String,System.String,System.String)
```
Mapping单项测序的数据

#### Stdsw
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.BWA.Stdsw
```
standard SW/NW alignment


### Properties

#### ALN_SA_COORDINATES
bwa aln -c -t 3 -f {leftreads.sai} {reference.fa} {leftreads.fastq}
