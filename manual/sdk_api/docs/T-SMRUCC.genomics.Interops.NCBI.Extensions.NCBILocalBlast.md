---
title: NCBILocalBlast
---

# NCBILocalBlast
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions](N-SMRUCC.genomics.Interops.NCBI.Extensions.html)_

ShoalShell API interface for ncbi localblast operations.



### Methods

#### Blastn
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.NCBILocalBlast.Blastn(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService.InteropService,System.String,System.Collections.Generic.IEnumerable{System.String},System.String,System.String,System.Boolean,System.Int32,System.Int32)
```
Invoke the batch blastn operations for the target query nt sequence.

|Parameter Name|Remarks|
|--------------|-------|
|handle|-|
|nt|-|
|GenomeSource|The fasta sequence data file path collection.(Fasta序列文件的路径的集合)|
|OutputDir|-|
|evalue|-|
|reversed|假若这个参数为真，则**nt**参数所指向的fasta序列则会作为参考库|


#### BlastX
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.NCBILocalBlast.BlastX(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService.InteropService,System.String,System.String,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|handle|-|
|nt|核酸序列的fasta文件的文件路径|
|proteins|假若目标对象为一个文件，则只进行一个BlastX，假若为一个文件夹，并且其中包含有许多蛋白质序列的fasta文件，则进行批量blastx|
|evalue|-|
|output|-|


#### CreateSession
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.NCBILocalBlast.CreateSession(System.String)
```
Initialize a local blast session handle for your program, you can specific the blast bin location on parameter **blastbin**

|Parameter Name|Remarks|
|--------------|-------|
|blastbin|
 This parameter specific the blast bin location, if this parameter is empty then the function will try to search the blastbin automatically.
 (假若本参数为空，则函数会尝试自动搜索出blast程序的文件夹)
 |

> 目前blast日志分析模块仅仅能够支持2.2.28版本的blast日志的解析

#### FastCheckIntegrityProvider
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.NCBILocalBlast.FastCheckIntegrityProvider(SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.String)
```
进行快速的字符串匹配

|Parameter Name|Remarks|
|--------------|-------|
|query|-|
|BlastOUTPUT|-|


#### LoadUltraLargeSizeBlastOutput
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.NCBILocalBlast.LoadUltraLargeSizeBlastOutput(System.String,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|path|-|
|chunk_size|是以1024*1024为基础的，本参数的值应该小于768，最大不应该超过800，否则程序会崩溃.对于1GB以内的日志文件，可以考虑100|


#### SaveBlastOutput
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.NCBILocalBlast.SaveBlastOutput(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.IBlastOutput,System.String)
```
Write the blast output data as a Xml data file.

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|SaveTo|The file path of the blast output xml data will be saved.|



