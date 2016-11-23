# ParallelTaskAPI
_namespace: [SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI](./index.md)_

NCBI blast parallel task



### Methods

#### __bbh
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.ParallelTaskAPI.__bbh(System.Collections.Generic.KeyValuePair{System.String,System.String},System.String,System.String,System.String,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.BlastInvoker,System.Boolean)
```
query -> hits; hits -> query

|Parameter Name|Remarks|
|--------------|-------|
|path|-|
|query|-|
|Evalue|-|
|EXPORT|-|
|handle|-|
|[overrides]|-|


#### BatchBlastp
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.ParallelTaskAPI.BatchBlastp(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.BlastInvoker,System.String,System.String,System.String,System.String,System.Boolean,System.Int32)
```
Query -> {Subjects}

|Parameter Name|Remarks|
|--------------|-------|
|Handle|-|
|query|-|
|subject|-|
|EXPORT|-|
|Evalue|-|
|[overrides]|-|


#### BatchBlastpRev
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.ParallelTaskAPI.BatchBlastpRev(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus,System.String,System.String,System.String,System.String,System.Boolean,System.Boolean,System.Int32)
```
{Queries} -> Subject
 
 .(这个方法是与@``M:SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.ParallelTaskAPI.BatchBlastp(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.BlastInvoker,System.String,System.String,System.String,System.String,System.Boolean,System.Int32)``相反的，即使用多个Query来查询一个Subject)

#### StartTask
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.ParallelTaskAPI.StartTask(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.BlastInvoker,System.String,System.String,System.String,System.Boolean,System.Boolean)
```
两两组合的双向比对用来创建文氏图所需要的数据

|Parameter Name|Remarks|
|--------------|-------|
|Handle|-|
|Input|-|
|EXPORT|-|
|Evalue|-|
|Parallel|-|
|Overrides|-|



