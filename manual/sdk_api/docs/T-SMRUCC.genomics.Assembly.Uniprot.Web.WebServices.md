---
title: WebServices
---

# WebServices
_namespace: [SMRUCC.genomics.Assembly.Uniprot.Web](N-SMRUCC.genomics.Assembly.Uniprot.Web.html)_





### Methods

#### CreateQuery
```csharp
SMRUCC.genomics.Assembly.Uniprot.Web.WebServices.CreateQuery(System.String,System.String)
```
Create a protein query url.

|Parameter Name|Remarks|
|--------------|-------|
|geneId|-|
|taxonomy|-|


#### DownloadProtein
```csharp
SMRUCC.genomics.Assembly.Uniprot.Web.WebServices.DownloadProtein(System.String)
```
Download a protein sequence fasta data from http://www.uniprot.org/ using a specific **UniprotId**. （从http://www.uniprot.org/网站之上下载一条蛋白质序列）

|Parameter Name|Remarks|
|--------------|-------|
|UniprotId|The uniprot id of a protein sequence.(蛋白质在Uniprot数据库之中的编号)|


#### GetEntries
```csharp
SMRUCC.genomics.Assembly.Uniprot.Web.WebServices.GetEntries(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|url|CreateQuery(geneId, taxonomy)|



