---
title: API
---

# API
_namespace: [SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB](N-SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.html)_

在KEGG之中的直系同源数据



### Methods

#### HandleDownload
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.API.HandleDownload(System.String)
```
返回基因列表，之后可以使用这个基因列表来进行蛋白质或者核酸序列的下载

|Parameter Name|Remarks|
|--------------|-------|
|KO_ID|-|


#### HandleDownloads
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.API.HandleDownloads(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.KEGG.WebServices.QueryEntry})
```


|Parameter Name|Remarks|
|--------------|-------|
|EntryList|The entry point list of the kegg orthology.|



