---
title: Compiler
---

# Compiler
_namespace: [SMRUCC.genomics.Data.Regprecise](N-SMRUCC.genomics.Data.Regprecise.html)_





### Methods

#### __genomePartitions
```csharp
SMRUCC.genomics.Data.Regprecise.Compiler.__genomePartitions(Microsoft.VisualBasic.List{SMRUCC.genomics.Data.Regprecise.FastaReaders.Site},System.String,SMRUCC.genomics.Data.Regprecise.WebServices.JSONLDM.regulon[])
```
检查，OK

|Parameter Name|Remarks|
|--------------|-------|
|sites|-|
|repository|-|
|familyGroup|-|


#### Compile
```csharp
SMRUCC.genomics.Data.Regprecise.Compiler.Compile(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Regprecise.WebServices.JSONLDM.genome},System.String)
```
请在下载完了整个数据库之后再使用这个函数来进行编译

|Parameter Name|Remarks|
|--------------|-------|
|Regprecise|-|
|repository|-|


#### SitesFamilyCategory
```csharp
SMRUCC.genomics.Data.Regprecise.Compiler.SitesFamilyCategory(System.String,System.Boolean)
```
生成meme计算所需要的调控位点的fasta文件（按照家族分类）

|Parameter Name|Remarks|
|--------------|-------|
|repositoryDIR|为了保持简洁性，没有引用配置项目。。。需要手动设定数据源|
|genomePartitioning|当一个家族里面的序列数太多的时候是否需要按照基因组进行分组，默认不分组|



