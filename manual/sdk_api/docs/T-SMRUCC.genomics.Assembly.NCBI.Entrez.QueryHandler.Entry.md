---
title: Entry
---

# Entry
_namespace: [SMRUCC.genomics.Assembly.NCBI.Entrez.QueryHandler](N-SMRUCC.genomics.Assembly.NCBI.Entrez.QueryHandler.html)_





### Methods

#### __buildQuery
```csharp
SMRUCC.genomics.Assembly.NCBI.Entrez.QueryHandler.Entry.__buildQuery(System.String,System.String,System.String@,System.String@)
```
Generates the perl process handle for download the genbank database file which was specific by the accessionID.
 (生成下载Genbank文件所需要的临时脚本文件)

|Parameter Name|Remarks|
|--------------|-------|
|AccessionID|Gene locus_tag or genome accession id.|
|Work|Working directory|
|savedGBK|The downloaded GenBank file save location.|
|TempScript|Temp Script save location.|


#### DownloadGBK
```csharp
SMRUCC.genomics.Assembly.NCBI.Entrez.QueryHandler.Entry.DownloadGBK(System.String)
```
The BioPerl is required for download the genbank file in this function.(本函数会尝试从NCBI服务器之上下载Genbank文件，这个方法的调用需要计算机之上安装有BioPerl)

|Parameter Name|Remarks|
|--------------|-------|
|work|保存文件的临时文件夹|



