---
title: StreamIO
---

# StreamIO
_namespace: [Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream](N-Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.html)_





### Methods

#### __lazySaved
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.StreamIO.__lazySaved(System.String,Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Text.Encoding)
```
在保存大文件时为了防止在保存的过程中出现内存溢出所使用的一种保存方法

|Parameter Name|Remarks|
|--------------|-------|
|path|-|
|df|-|
|encoding|-|


#### GetType
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.StreamIO.GetType(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Type[])
```
根据文件的头部的定义，从**types**之中选取得到最合适的类型的定义

|Parameter Name|Remarks|
|--------------|-------|
|df|-|
|types|-|


#### SaveDataFrame
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.StreamIO.SaveDataFrame(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.String,System.Boolean,System.Text.Encoding)
```
Save this csv document into a specific file location **path**.

|Parameter Name|Remarks|
|--------------|-------|
|path|-|
|lazySaved|Optional, this is for the consideration of performance and memory consumption.
 When a data file is very large, then you may encounter a out of memory exception on a 32 bit platform,
 then you should set this parameter to True to avoid this problem. Defualt is False for have a better
 performance.
 (当估计到文件的数据量很大的时候，请使用本参数，以避免内存溢出致使应用程序崩溃，默认为False，不开启缓存)
 |

> 当目标保存路径不存在的时候，会自动创建文件夹


