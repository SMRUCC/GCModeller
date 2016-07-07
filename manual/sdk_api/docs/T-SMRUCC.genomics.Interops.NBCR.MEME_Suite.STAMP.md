---
title: STAMP
---

# STAMP
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.html)_





### Methods

#### Convert
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.STAMP.Convert(SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MEME.MEME,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|meme_log|-|
|Id|-|

> 
>  ------------------------
>  Motif 2 position-specific probability matrix
>  ------------------------
>  letter-probability matrix: alength= 4 w= 6 nsites= 31
>  0 31 0 0
>  29 0 0 2
>  0 30 0 1
>  2 1 28 0
>  0 3 0 28
>  0 0 31 0
>  

#### GetValue
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.STAMP.GetValue(System.String)
```
从MEME软件的输出文件夹中获取Motif信息

|Parameter Name|Remarks|
|--------------|-------|
|LogDir|-|


#### Invoke
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.STAMP.Invoke(System.String,System.String,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|Dir|需要进行搜索的文件夹|
|Exported|保存的文件名|
|Split|分割的块的大小，默认不分割|


#### ParseMatches
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.STAMP.ParseMatches(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|OriginalFile|输入到STAMP服务器中的原始数据|
|Matches|从STAMP服务器获取的匹配数据|



