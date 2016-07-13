---
title: MapsAPI
---

# MapsAPI
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.html)_





### Methods

#### __createObject
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.MapsAPI.__createObject(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.BlastnHit)
```
从blastn日志之中导出Mapping的数据

|Parameter Name|Remarks|
|--------------|-------|
|Query|-|
|hitMapping|-|


#### __setUnique
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.MapsAPI.__setUnique(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BlastnMapping[]@)
```
Unique的判断原则：
 
 1. 假若一个query之中只含有一个hit，则为unique
 2. 假若一个query之中含有多个hit的话，假若只有一个hit是perfect类型的，则为unique
 3. 同一个query之中假若为多个perfect类型的hit的话，则不为unique

|Parameter Name|Remarks|
|--------------|-------|
|data|-|


#### CreateObject
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.MapsAPI.CreateObject(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query)
```
从blastn日志之中导出Mapping的数据

|Parameter Name|Remarks|
|--------------|-------|
|Query|-|


#### Export
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.MapsAPI.Export(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228)
```
从blastn日志文件之中导出fastq对基因组的比对的结果

|Parameter Name|Remarks|
|--------------|-------|
|blastnMapping|-|


#### TrimAssembly
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.MapsAPI.TrimAssembly(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BlastnMapping})
```
按照条件 @"P:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BlastnMapping.Unique"=TRUE and @"P:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BlastnMapping.PerfectAlignment"=TRUE
 进行可用的alignment mapping结果的筛选


