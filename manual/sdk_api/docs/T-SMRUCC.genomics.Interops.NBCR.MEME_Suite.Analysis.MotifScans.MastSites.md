---
title: MastSites
---

# MastSites
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.html)_

使用某一个Motif的MEME模型扫描整个基因组的结果



### Methods

#### __createObject
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.MastSites.__createObject(System.Int32,SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MAST.HitResult,System.String,System.Int32,System.String)
```
不做任何筛选，直接导出数据

|Parameter Name|Remarks|
|--------------|-------|
|start|-|
|hit|-|
|sequence|-|
|OffSet|-|
|trace|-|


#### Compile
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.MastSites.Compile(SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MAST.MAST,System.String,System.String)
```
这个函数是适用于Windows版本的<meme>.txt来源的

|Parameter Name|Remarks|
|--------------|-------|
|mast|-|
|PWMDir|从MEME文本文件所输出的pwm模型的结果的文件夹|
|faDIR|生成meme模型的调控位点的fasta文件夹，
 由于调控关系是在生成位点的fasta文件的时候一同生成在fasta序列的标题之中的，这个过程是为了方便构建调控关系，所以在这里需要fasta文件来得到调控关系，

 假若所生成的模型文件之中不需要包含有调控数据，则可以将这个参数置为空值
 |


#### HasEmptyMappings
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.MastSites.HasEmptyMappings
```
只要是@"P:SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.MastSites.Regulators"或者@"P:SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.MastSites.Sites"这两个指向Regprecise数据库之中的记录的任意一个属性为空，则本函数返回真


### Properties

#### Regulators
在Regprecise数据库之中的调控因子的基因编号
#### Sites
在Regprecise之中的调控位点的记录，这个是通过meme模型来获取的，然后再根据这个就可以找到调控因子了，再结合bbh结果就可以计算出预测的调控关系了
#### Trace
Motif的来源，一般是meme的数据源
