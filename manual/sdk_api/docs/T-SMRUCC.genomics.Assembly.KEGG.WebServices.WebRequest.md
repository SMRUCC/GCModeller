---
title: WebRequest
---

# WebRequest
_namespace: [SMRUCC.genomics.Assembly.KEGG.WebServices](N-SMRUCC.genomics.Assembly.KEGG.WebServices.html)_

KEGG web query request handler.(KEGG数据库web查询处理模块)



### Methods

#### BatchQuery
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.BatchQuery(System.String,System.UInt32)
```


|Parameter Name|Remarks|
|--------------|-------|
|keyword|-|
|LimitCount|大批量的数据查询会不会被KEGG封IP？，可以使用本参数来控制数据的返回量|


#### Download16S_rRNA
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.Download16S_rRNA(System.String)
```
http://www.genome.jp/dbget-bin/www_bget?ko:K01977

|Parameter Name|Remarks|
|--------------|-------|
|outDIR|-|


#### DownloadsBatch
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.DownloadsBatch(System.String,System.Collections.Generic.IEnumerable{System.String})
```
同一个基因组内的蛋白质的序列下载推荐使用这个方法来完成，这个方法KEGG服务器的负担会比较轻

#### DownloadSequence
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.DownloadSequence(System.String)
```
Download fasta sequence data from KEGG database, this function will automatically handles the species brief code.

|Parameter Name|Remarks|
|--------------|-------|
|Id|-|


#### FetchNt
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.FetchNt(SMRUCC.genomics.Assembly.KEGG.WebServices.QueryEntry)
```
Fetch the nucleotide sequence fasta data from the kegg database.

|Parameter Name|Remarks|
|--------------|-------|
|Entry|-|


#### FetchSeq
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.FetchSeq(SMRUCC.genomics.Assembly.KEGG.WebServices.QueryEntry)
```
Download a protein sequence data from the KEGG database.(从KEGG数据库之中下载一条蛋白质分子序列)

|Parameter Name|Remarks|
|--------------|-------|
|Entry|KEGG sequence query entry.(KEGG数据库的分子序列查询入口点)|


#### GetPageContent
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.GetPageContent(System.String)
```
好像因为没有窗体所以这段代码不能够正常的工作

|Parameter Name|Remarks|
|--------------|-------|
|url|-|


#### GetQueryEntry
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.GetQueryEntry(System.String)
```
Handle query for a gene locus from KEGG

|Parameter Name|Remarks|
|--------------|-------|
|locusId|-|


#### GetSpCode
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.GetSpCode(System.String)
```
从KEGG服务器上面得到基因组的摘要代码

|Parameter Name|Remarks|
|--------------|-------|
|locusId|-|


#### HandleQuery
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.HandleQuery(System.String,System.Int32)
```
Get an entry list from a keyword throught the KEGG database web request.{(speciesId:AccessionId), entry_description}

|Parameter Name|Remarks|
|--------------|-------|
|keyword|-|

_returns: 如果没有任何结果则返回一个空列表_


### Properties

#### _16S_rRNA
都定义在这个地方了。。。。
