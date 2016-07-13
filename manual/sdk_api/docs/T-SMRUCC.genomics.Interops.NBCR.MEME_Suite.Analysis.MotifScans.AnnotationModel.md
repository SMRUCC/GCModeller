---
title: AnnotationModel
---

# AnnotationModel
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.html)_

MAST_LDM for motif annotation.



### Methods

#### Complement
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel.Complement
```
互补反向

#### LoadDocument
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel.LoadDocument(System.String,System.String)
```
从指定路径的meme.txt文档之中，解析meme.txt文档得到Motif的模型

|Parameter Name|Remarks|
|--------------|-------|
|memeText|meme_out.txt的文件路径|


#### LoadLDM
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel.LoadLDM(System.String)
```
加载自带的源里面的模型数据

#### LoadMEMEOUT
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel.LoadMEMEOUT(System.String,System.Boolean,System.Boolean)
```
Load models from meme output DIR

|Parameter Name|Remarks|
|--------------|-------|
|source|MEME.txt DIR|
|full|
 If full, then @"F:Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories"
 Else @"F:Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly"
 |



### Properties

#### _PspMatrix
在@"P:SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel.PspMatrix"属性之中进行延时加载
