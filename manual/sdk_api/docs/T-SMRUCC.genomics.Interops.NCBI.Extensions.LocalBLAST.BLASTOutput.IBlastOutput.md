---
title: IBlastOutput
---

# IBlastOutput
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.html)_

Blast程序结果对外输出的统一接口类型对象

> 
>  Reader文件夹之下为各种格式的日志文件的读取类对象
>  对于BLAST日志文件，则有一个BlastLogFile对象作为对外保存和其他程序读取的统一接口
>  


### Methods

#### ExportAllBestHist
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.IBlastOutput.ExportAllBestHist(System.Double,System.Double)
```
导出每条记录中的所有最佳的匹配结果

#### ExportBestHit
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.IBlastOutput.ExportBestHit(System.Double,System.Double)
```
仅导出每条记录的第一个最佳匹配的结果


