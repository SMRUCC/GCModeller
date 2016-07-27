---
title: RegpreciseShellScriptAPI
---

# RegpreciseShellScriptAPI
_namespace: [SMRUCC.genomics.Analysis.Annotations.RegpreciseRegulations](N-SMRUCC.genomics.Analysis.Annotations.RegpreciseRegulations.html)_





### Methods

#### BuildPWM
```csharp
SMRUCC.genomics.Analysis.Annotations.RegpreciseRegulations.RegpreciseShellScriptAPI.BuildPWM(System.Boolean)
```
Build the pwm matrix model for the regulations sites in the regprecise database.
 (构建meme的pwm模型并且保存于GCModeller的数据库之中)

|Parameter Name|Remarks|
|--------------|-------|
|rebuildMatrix|-|


#### OrthologousFromOverview
```csharp
SMRUCC.genomics.Analysis.Annotations.RegpreciseRegulations.RegpreciseShellScriptAPI.OrthologousFromOverview(System.String,System.String)
```
从blastp日志数据之中导出regprecise数据库的注释结果

|Parameter Name|Remarks|
|--------------|-------|
|qvsPath|@"M:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Views.Overview.LoadExcel(System.String)" 物种_vs_regprecise|
|svqPath|regprecise_vs_物种|



