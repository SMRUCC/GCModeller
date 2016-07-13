---
title: Regprecise
---

# Regprecise
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.RegpreciseRegulations](N-SMRUCC.genomics.Analysis.AnnotationTools.RegpreciseRegulations.html)_

Annotation for the genome wide regulation network using regprecise database.



### Methods

#### Install
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.RegpreciseRegulations.Regprecise.Install(System.String,System.String)
```
Install the regprecise database into the GCModeller repository database.

|Parameter Name|Remarks|
|--------------|-------|
|DbFile|The regulator protein fasta file path|
|RepositoryRoot|The database directory root of the GCModeller repository|


#### InternalCreateBBH
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.RegpreciseRegulations.Regprecise.InternalCreateBBH(System.String,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit[],System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit[]})
```


|Parameter Name|Remarks|
|--------------|-------|
|sp_code|The KEGG brief species code.|
|Qvs|-|
|SvqDict|-|


#### InvokeAnnotation
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.RegpreciseRegulations.Regprecise.InvokeAnnotation(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit[],SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit[],System.String)
```
Invoke the regprecise annotation from the overview cache data.

|Parameter Name|Remarks|
|--------------|-------|
|Orthologous|-|
|Paralogs|-|
|ProteinsFasta|-|


#### Orthologous
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.RegpreciseRegulations.Regprecise.Orthologous(System.String,System.String)
```
从blastp日志数据之中导出regprecise数据库的注释结果

|Parameter Name|Remarks|
|--------------|-------|
|qvsPath|-|
|svqPath|-|


#### WriteDatabase
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.RegpreciseRegulations.Regprecise.WriteDatabase(SMRUCC.genomics.Data.Model_Repository.Regprecise[])
```
Update tje database information of the regprecise repository

|Parameter Name|Remarks|
|--------------|-------|
|data|The data will be write into or updates the regprecise repository source.|



### Properties

#### _MetaDataTable
The meta data of the regprecise regulators.(Regprecise数据库的元数据)
