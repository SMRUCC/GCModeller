---
title: MotifMatchMast
---

# MotifMatchMast
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI.html)_

通过MAST来得到MEME中的Motif的结果



### Methods

#### BuildVirtualFootprints
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI.MotifMatchMast.BuildVirtualFootprints(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.MotifSiteHit},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHIndex},System.Boolean,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,SMRUCC.genomics.Assembly.DOOR.DOOR,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|sites|从Csv文档之中得到的位点信息，Xml文档由于缺少位点信息所以无法进行调控关系的构建|
|bbh|-|
|hitHash|是否使用hit作为键名|


#### CompileDirectly
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI.MotifMatchMast.CompileDirectly(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|MEME|path of meme.txt|
|MAST|path of mast.xml|


#### CompileSingle
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI.MotifMatchMast.CompileSingle(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|MEME|meme.txt|
|MAST_OUT|当前的这个meme.txt对Motif数据库的fasta序列的比对输出结果的文件夹|


#### PreCompile
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI.MotifMatchMast.PreCompile(System.String,System.String,System.Boolean)
```
预编译资源数据

|Parameter Name|Remarks|
|--------------|-------|
|MEME_OUT|-|
|MAST_OUT|-|
|direct|假若这个参数为真，则不是家族比对，则每一个文件夹都直接是比对的结果，加入为False，则说明是家族的比对，则每一个文件夹下面是家族的比对结果文件夹|

_returns: 取决于参数**direct**: True -> MAST_DIR/mast.xml; False -> MAST/DIR_


