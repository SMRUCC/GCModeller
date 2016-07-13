---
title: VennDataBuilder
---

# VennDataBuilder
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.html)_

The batch blast module for the preparations of the Venn diagram drawing data model.(为文氏图的绘制准备数据的批量blast模块)

> 这里面的方法都是完全的两两组合的BBH


### Methods

#### __blastpHandle
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.__blastpHandle(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus,System.String,System.String,System.Int32,System.String,System.String,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|ServiceHandle|-|
|Query|-|
|Subject|-|
|Num_Threads|-|
|Evalue|-|
|EXPORT|-|
|Overrides|当目标文件存在并且长度不为零的时候，是否进行覆盖，假若为否，则直接忽略过这个文件|


#### BuildBLASTP_InvokeHandle
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.BuildBLASTP_InvokeHandle(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus)
```
批量两两比对blastp，以用于生成文氏图的分析数据

|Parameter Name|Remarks|
|--------------|-------|
|localblast|-|


#### BuildFileName
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.BuildFileName(System.String,System.String,System.String)
```
**query** @"F:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.QUERY_LINKS_SUBJECT" **subject**.(去掉了fasta文件的后缀名)

|Parameter Name|Remarks|
|--------------|-------|
|Query|-|
|Subject|-|
|EXPORT|-|


#### LogNameParser
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.LogNameParser(System.String)
```
尝试从给出的日志文件名之中重新解析出比对的对象列表

|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### ParallelTask
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.ParallelTask(System.String,System.String,System.String,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.INVOKE_BLAST_HANDLE,System.Boolean,System.Int32)
```
这个函数相比较于@"M:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.TaskBuilder_p(System.String,System.String,System.String,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.INVOKE_BLAST_HANDLE,System.Boolean)"更加高效

|Parameter Name|Remarks|
|--------------|-------|
|inputDIR|-|
|outDIR|-|
|evalue|-|
|blastTask|-|
|[overrides]|-|
|num_threads|-|

_returns: 返回日志文件列表_

#### TaskBuilder
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.TaskBuilder(System.String,System.String,System.String,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.INVOKE_BLAST_HANDLE,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|input|输入的文件夹，fasta序列的文件拓展名必须要为*.fasta或者*.fsa|
|EXPORT|结果导出的文件夹，导出blast日志文件|
|InvokedBLASTAction|所执行的blast命令，函数返回日志文件名|


#### TaskBuilder_p
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.TaskBuilder_p(System.String,System.String,System.String,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.INVOKE_BLAST_HANDLE,System.Boolean)
```
The parallel edition for the invoke function @"M:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.TaskBuilder(System.String,System.String,System.String,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.INVOKE_BLAST_HANDLE,System.Boolean)".(@"M:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.TaskBuilder(System.String,System.String,System.String,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder.INVOKE_BLAST_HANDLE,System.Boolean)"的并行版本)

|Parameter Name|Remarks|
|--------------|-------|
|input|-|
|EXPORT|-|
|evalue|-|
|InvokedBLASTAction|-|



### Properties

#### RecommendedThreads
The recommended num_threads parameter for the blast operation base on the current system hardware information.
 (根据当前的系统硬件配置所推荐的num_threads参数)
