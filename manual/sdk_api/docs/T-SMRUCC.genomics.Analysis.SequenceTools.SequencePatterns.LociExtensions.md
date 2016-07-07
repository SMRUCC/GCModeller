---
title: LociExtensions
---

# LociExtensions
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns](N-SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.html)_

将序列特征的搜索结果转换为@"T:SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment"对象类型



### Methods

#### ConvertsAuto
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.LociExtensions.ConvertsAuto(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
自动根据类型转换为位点数据

|Parameter Name|Remarks|
|--------------|-------|
|df|-|


#### MirrorsLoci
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.LociExtensions.MirrorsLoci(SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci)
```
---><---

|Parameter Name|Remarks|
|--------------|-------|
|x|-|


#### ToLoci
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.LociExtensions.ToLoci(SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Repeats,System.Int32)
```
对于简单的重复序列而言，正向链上面的重复片段，例如AAGTCT在反向链上面就是AGACTT，总是可以找得到对应的，所以在这里只需要记录下正向链的数据就好了

|Parameter Name|Remarks|
|--------------|-------|
|x|-|
|start|-|



