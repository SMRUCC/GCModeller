---
title: WebServices
---

# WebServices
_namespace: [SMRUCC.genomics.Assembly.MiST2](N-SMRUCC.genomics.Assembly.MiST2.html)_





### Methods

#### Download
```csharp
SMRUCC.genomics.Assembly.MiST2.WebServices.Download(System.String)
```
获取目标页面内部的所有蛋白质数据

|Parameter Name|Remarks|
|--------------|-------|
|url|-|


#### DownloadData
```csharp
SMRUCC.genomics.Assembly.MiST2.WebServices.DownloadData(System.String)
```
使用这个方法进行数据的下载：
 获取某一个物种基因组内的信号转导网络数据
 首先，在MisT2网站上搜索菌株名称，得到菌株编号
 之后在使用所得到的**speciesCode[菌株编号]**调用本方法既可以下载数据了

|Parameter Name|Remarks|
|--------------|-------|
|speciesCode|MiST2数据库内的物种编码|



