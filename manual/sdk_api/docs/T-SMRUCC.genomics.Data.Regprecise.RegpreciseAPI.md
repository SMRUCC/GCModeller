---
title: RegpreciseAPI
---

# RegpreciseAPI
_namespace: [SMRUCC.genomics.Data.Regprecise](N-SMRUCC.genomics.Data.Regprecise.html)_





### Methods

#### __exportMotifs
```csharp
SMRUCC.genomics.Data.Regprecise.RegpreciseAPI.__exportMotifs(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{System.String,SMRUCC.genomics.Data.Regtransbase.WebServices.FastaObject}},System.String,SMRUCC.genomics.Data.Regprecise.TranscriptionFactors)
```


|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|Family|-|


#### DownloadRegulatorSequence
```csharp
SMRUCC.genomics.Data.Regprecise.RegpreciseAPI.DownloadRegulatorSequence(SMRUCC.genomics.Data.Regprecise.TranscriptionFactors,System.String)
```
Download regprecise regulator protein sequence from kegg database.

|Parameter Name|Remarks|
|--------------|-------|
|Regprecise|-|
|EXPORT|-|


#### Export
```csharp
SMRUCC.genomics.Data.Regprecise.RegpreciseAPI.Export(SMRUCC.genomics.Data.Regprecise.TranscriptionFactors,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Regprecise|-|
|outDIR|-|


#### ExportBySpecies
```csharp
SMRUCC.genomics.Data.Regprecise.RegpreciseAPI.ExportBySpecies(SMRUCC.genomics.Data.Regprecise.TranscriptionFactors,System.String)
```
还存在问题

|Parameter Name|Remarks|
|--------------|-------|
|Regprecise|-|
|ExportDir|-|


#### LoadRegulationDb
```csharp
SMRUCC.genomics.Data.Regprecise.RegpreciseAPI.LoadRegulationDb
```
加载自有的源之中的调控数据库

#### ReGenerate
```csharp
SMRUCC.genomics.Data.Regprecise.RegpreciseAPI.ReGenerate(SMRUCC.genomics.Data.Regprecise.TranscriptionFactors,System.String,System.String)
```
当有时候向RegulatorSequerncede Fasta文件之中添加了新的Regprecise数据库之中没有的蛋白质序列数据之后，可能会出现
 TFBS序列和Regulator之间的关系无法对应的情况，则这个时候可以使用本方法来重新刷新着两个Fasta序列文件
> 对于调控因子序列仅仅取出LocusTAG以及Description数据，TFBS文件是重新生成的


