---
title: Extensions
---

# Extensions
_namespace: [SMRUCC.genomics.Model.Network.VirtualFootprint](N-SMRUCC.genomics.Model.Network.VirtualFootprint.html)_





### Methods

#### MergeLocis
```csharp
SMRUCC.genomics.Model.Network.VirtualFootprint.Extensions.MergeLocis(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment},System.Int32,System.Func{SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment,System.Int32},System.Func{SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment,System.String})
```
将相邻的位点进行合并

|Parameter Name|Remarks|
|--------------|-------|
|source|假设都是同一条链上面的|
|offset|-|
|getDist|-|
|getTag|-|

> 
>  Id和距离是使用一些方法来读取的，例如@"P:SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment.ID"是ID:Dist这种形式的话，就可以分别分离出编号和记录数据
>  

#### TrimStranded
```csharp
SMRUCC.genomics.Model.Network.VirtualFootprint.Extensions.TrimStranded(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment},SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Func{SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment,System.String})
```
筛选出和标记的基因相同的链方向的位点数据

|Parameter Name|Remarks|
|--------------|-------|
|sites|-|
|genome|-|
|getId|-|



