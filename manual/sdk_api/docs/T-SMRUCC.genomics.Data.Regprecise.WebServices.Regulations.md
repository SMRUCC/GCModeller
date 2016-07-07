---
title: Regulations
---

# Regulations
_namespace: [SMRUCC.genomics.Data.Regprecise.WebServices](N-SMRUCC.genomics.Data.Regprecise.WebServices.html)_

%Repository%/Regprecise/MEME/regulations.xml
 (在进行了MEME分析之后，使用这个模块来生成所预测的调控关系)



### Methods

#### GetMotifFamily
```csharp
SMRUCC.genomics.Data.Regprecise.WebServices.Regulations.GetMotifFamily(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|site|$"{site.geneLocusTag}:{site.position}"|


#### GetRegulations
```csharp
SMRUCC.genomics.Data.Regprecise.WebServices.Regulations.GetRegulations(System.String)
```
{locus_tag:position}

|Parameter Name|Remarks|
|--------------|-------|
|uid|-|


#### GetRegulator
```csharp
SMRUCC.genomics.Data.Regprecise.WebServices.Regulations.GetRegulator(System.Int32)
```
通过vimssid编号来获取得到调控因子的数据模型

|Parameter Name|Remarks|
|--------------|-------|
|geneVIMSSId|-|


#### GetSite
```csharp
SMRUCC.genomics.Data.Regprecise.WebServices.Regulations.GetSite(System.String)
```
$"{@"P:SMRUCC.genomics.Data.Regprecise.WebServices.JSONLDM.site.geneLocusTag"}:{@"P:SMRUCC.genomics.Data.Regprecise.WebServices.JSONLDM.site.position"}"

|Parameter Name|Remarks|
|--------------|-------|
|geneLocusTag|-|



