---
title: TMod
---

# TMod
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.TMoD](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.TMoD.html)_

Tmod: toolbox of motif discovery, program assembly resources.



### Methods

#### __checkURL
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.TMoD.TMod.__checkURL(System.String,System.String,System.Collections.Generic.Dictionary{System.String,System.String})
```
由于%会造成bat文件出错，所以在这里进行检查然后提醒用户

|Parameter Name|Remarks|
|--------------|-------|
|inDIR|-|
|outDIR|-|
|res|-|


#### BatchMEMEScanning
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.TMoD.TMod.BatchMEMEScanning(System.String,System.String,System.Double,System.Int32,System.String,System.String,System.Int32)
```
Batch execute the meme program for a collection of fasta file, the source is the directory location of the collection of fasta sequence file.

|Parameter Name|Remarks|
|--------------|-------|
|inDIR|Fasta序列的存放文件|
|outDIR|-|
|evt|-|
|num_motifs|-|
|mode|-|
|type|-dna / -protein|

> 
>  要在批处理中立即生效（只是临时的，生命力最弱）加一句： 
>  直接用set命令：set path=%path%; 
>  退出批处理后，环境变量恢复原来模样;
>  

#### InitializeSession
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.TMoD.TMod.InitializeSession
```
释放内部的资源文件然后返回工作会话的路径

#### MotifSelect
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.TMoD.TMod.MotifSelect(SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.Collections.Generic.IEnumerable{System.String})
```
Select the fasta sequence from a fasta collection file which the fasta sequence its title contains a keyword in the **lstLocus**

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|lstLocus|-|



### Properties

#### MEME
The file path of the meme program from the tmod resource collection assembly.
