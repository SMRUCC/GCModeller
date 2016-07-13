---
title: MotifFootPrintsGenerates
---

# MotifFootPrintsGenerates
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.html)_

这里所定义的所有对象都是和数据解析无关了，都是用于进行数据存储的对象类型



### Methods

#### __checkMoitfCoRegulations
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.__checkMoitfCoRegulations(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint[])
```
假若目标Regulator可以被**groupedMotif**里面的多余60%的对象所具备，则认为该调控因子对目标motif的调控关系是可能存在的

|Parameter Name|Remarks|
|--------------|-------|
|groupedMotif|-|


#### __reGenerate
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.__reGenerate(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint},SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader)
```


|Parameter Name|Remarks|
|--------------|-------|
|grouped|已经按照@"P:SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.VirtualFootprints.Starts"属性进行排序|


#### CreateMotifInformations
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.CreateMotifInformations(System.String,SMRUCC.genomics.Data.Regprecise.TranscriptionFactors)
```
将Regprecise数据库之中的Moitf数据解析出来进行MEME分析之后，将MEME_OUT文件夹里面的数据整理出来

|Parameter Name|Remarks|
|--------------|-------|
|MotifMEME_OUT|-|


#### DataFilteringByPathwayCoExpression
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.DataFilteringByPathwayCoExpression(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.KEGG.Archives.Csv.Pathway},System.Boolean)
```
通过同一个代谢途径之中的所有的基因可能共表达的生物学知识，在缺乏转录组数据的条件之下进行数据的筛选，算法的要点：
 假若同一个代谢途径之中的基因，有65%或者以上的基因具有同一个motif位点，则该motif位点可能是正确的位点
 
 本方法的缺点是，仅能够大致的筛选出具有代谢途径信息的基因

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|pathwayInfo|-|


#### Diff
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.Diff(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.VirtualFootprints},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.VirtualFootprints},System.String,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|data1|-|
|data2|-|
|export|导出的文件夹|


#### ExportRegulators
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.ExportRegulators(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint})
```
从调控数据之中导出真正被使用到的调控因子

|Parameter Name|Remarks|
|--------------|-------|
|Regulations|-|


#### FootPrintMatches
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.FootPrintMatches(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|MEME_OUT|MEME html文件夹集合|
|MAST_OUT|Mast html文件夹集合|


#### FootprintMatchesTEXT
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.FootprintMatchesTEXT(System.String,System.String,SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Int32,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|MEME_Text|-|
|MAST_html|-|
|GenomeSequence|-|
|GeneBriefInformation|-|
|ATGDistance|-|
|FilterPromoter|由于某一个位点是落在某一个基因的内部或者下游区的，所以使用这个参数来过滤掉这些不在启动子区的位点，
 本参数为真，则执行过滤操作，返回的记过之中仅包含有启动子区的位点，不为真，则返回所有类型的位点。
 默认不进行过滤操作|


#### GenerateNetwork
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.GenerateNetwork(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint},System.String)
```
Export a cytoscape network file from the predicted footprint data.(从所预测的footprint调控数据之中导出一个cytoscape网络的定义文件)

|Parameter Name|Remarks|
|--------------|-------|
|Regulations|-|
|saveto|所导出的文件夹|


#### GroupMotifs
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.GroupMotifs(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint},SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,System.Int32)
```
按照家族将可能重复的motif归为一组数据

|Parameter Name|Remarks|
|--------------|-------|
|data|-|


#### Matches
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.Matches(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.HTML.MEMEOutput},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifDb.MotifFamily},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Regprecise.RegpreciseMPBBH},SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader,System.String,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.KEGG.Archives.Csv.Pathway},SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix,System.Boolean,System.Double,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|MotifDatabase|-|
|RegulatorMatches|-|
|GenomeBrief|-|
|Door|-|
|KEGG_Pathways|-|
|PccMatrix|PCC或者SPCC的混合矩阵|
|ignoreDirection|-|
|pccCutoff|-|
|ATGDistance|-|


#### MatchRegulator
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.MatchRegulator(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint,System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifDb.Motif},System.Collections.Generic.KeyValuePair{System.String,System.String}[])
```
通过@"P:SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.HTML.MEMEOutput.ObjectId"

|Parameter Name|Remarks|
|--------------|-------|
|item|-|
|MotifDb|-|
|maps|-|

_returns: {RegpreciseRegulators(), bh_Matches()}_

#### PathwayFunctionAssociation
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.PathwayFunctionAssociation(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.VirtualFootprints},System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|KEGGPathway|数据存放的文件夹|
|EXPORT|结果数据的导出文件名，当参数不为空的时候，会导出一个Csv文件|


#### PccAssumption
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.PccAssumption(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Regprecise.RegpreciseMPBBH})
```
可以使用本方法来假设Pcc数据，以方便后面的模拟计算的实验

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|RegulatorMatches|-|

> 
>  对于所有基因调控关系对，都假设为0.85，根据Regprecise数据库之中所记录的Effect来猜测可能的符号
>  


