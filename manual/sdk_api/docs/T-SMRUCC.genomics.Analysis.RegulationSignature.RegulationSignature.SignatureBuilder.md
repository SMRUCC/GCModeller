---
title: SignatureBuilder
---

# SignatureBuilder
_namespace: [SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature](N-SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.html)_

本模块将Regprecise注释结果上面的每一个位点数据转换为一段简并的DNA序列数据用来抽象整个基因组的调控网络特征
 由于位点和下游基因构成了一条边，所以可以从整个Regprecise注释数据之中得到一个网络
 由于本全基因组调控特征可以用于表示整个基因组的调控网络，所以使用本方法将原有的三维的网络数据降维至二维DNA序列，可以很方便的使用blastn程序进行调控网络的相似度的比对工作

> 
>  采用冗余的形式来构建序列特征，这样通过blastn就可以同时比对调控网络和代谢网络，将两个三维网络降维至两个二维序列所形成的网络来进行相互比较
>  


### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.SignatureBuilder.#ctor(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint},SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway},System.Collections.Generic.IEnumerable{SMRUCC.genomics.ComponentModel.ICOGDigest})
```


|Parameter Name|Remarks|
|--------------|-------|
|VirtualFootprints|调控特征|
|PTT|全基因组|
|KEGG_Pathways|代谢网络信息|


#### __orderGenome
```csharp
SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.SignatureBuilder.__orderGenome
```
对基因组之中原件进行重新排序处理，两个基因组之间的对象应该尽量按照共有的特征进行排序

#### ToString
```csharp
SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.SignatureBuilder.ToString
```
从这里得到简并的调控网络特征序列
 简并序列的特征，总体功能分区为三个部分：调控区，未知功能区域，代谢网络区域
 在调控网络之中的每一个节点之中又划分为上面的下游基因的三个功能区域：调控区，未知功能区域，代谢网络区域
 按照这三个功能区域的划分是由于两个基因组特征比较必须要有相同的位置元素
> 
>  特征编码的规则如下：
>  特征编码得到的序列主要由3部分构成：
>  第一大部分为调控因子
>  第二大部分为没有功能被注释出来的基因
>  第三大部分为KEGG Pathway的注释结果
>  


