---
title: DBLink
---

# DBLink
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.Schema.DBLinkManager](N-SMRUCC.genomics.Assembly.MetaCyc.Schema.DBLinkManager.html)_

与其他的数据库之间的外键链接



### Methods

#### GetFormatValue
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.DBLinkManager.DBLink.GetFormatValue
```
向Csv文件写入数据所需求的一个方法

#### GetMetaCycFormatValue
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.DBLinkManager.DBLink.GetMetaCycFormatValue
```
向MetaCyc数据库中的*.dat文件写入数据所需求

#### TryParse
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.DBLinkManager.DBLink.TryParse(System.String)
```
解析来自于Csv文件的DBLinks数据

|Parameter Name|Remarks|
|--------------|-------|
|strData|-|


#### TryParseMetaCycDBLink
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.DBLinkManager.DBLink.TryParseMetaCycDBLink(System.String)
```
解析来自于MetaCyc数据库中的*.dat文件中的DBLinks数据

|Parameter Name|Remarks|
|--------------|-------|
|strData|-|

> 本方法和@"M:SMRUCC.genomics.Assembly.MetaCyc.Schema.DBLinkManager.DBLink.TryParse(System.String)"[另外一个解析方法]的解析格式相似，仅在于TryParse方法是使用%进行分割的，由于在Csv文件中使用的是"进行分割，所以使用%符号可以避免一些不必要的字符串解析BUG


