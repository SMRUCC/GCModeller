---
title: BashShell
---

# BashShell
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.html)_

生成用于linux服务器上面批量运行的blast脚本



### Methods

#### ScriptCallSave
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.BashShell.ScriptCallSave(System.Collections.Generic.IEnumerable{System.String},System.String)
```
2. 保存脚本

|Parameter Name|Remarks|
|--------------|-------|
|batch|-|
|outDIR|-|


#### VennBatch
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.BashShell.VennBatch(System.String,System.String,System.String,System.String,System.String)
```
1. 生成两两比对的脚本调用

|Parameter Name|Remarks|
|--------------|-------|
|inDIR|-|
|inRefAs|Linux服务器上面的引用位置|
|outDIR|-|
|evalue|-|
|blastDIR|这个应该是linux服务器上面的位置，包含blastp+makeblastdb|



