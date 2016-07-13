---
title: RepeatsSearchAPI
---

# RepeatsSearchAPI
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically](N-SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.html)_





### Methods

#### BatchSearch
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsSearchAPI.BatchSearch(SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.Int32,System.Int32,System.Int32,System.String)
```
Batch search for the repeats and reversed repeats sequence feature sites.

|Parameter Name|Remarks|
|--------------|-------|
|Mla|-|
|Min|-|
|Max|-|
|MinAppeared|-|
|saveDIR|-|


#### Density``1
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsSearchAPI.Density``1(System.String,System.Int32,System.String,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|DIR|-|
|size|-|
|ref|作为参考的原始数据之中的csv文件名|
|cutoff|-|


#### GenerateRepeats
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsSearchAPI.GenerateRepeats(System.String,System.String,System.Int32,System.Boolean)
```
请注意，这个函数是应用于生成最长的重复序列片段的方法，假若当前的序列片段在序列上已经没有重复出现了，
 则上一次迭代的序列可能为重复，所以**Segment**的长度减1的序列片段是会
 有重复的，故而会在函数之中将该目的片段缩短

|Parameter Name|Remarks|
|--------------|-------|
|Sequence|-|
|Segment|-|


#### RepeatsDensity
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsSearchAPI.RepeatsDensity(SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.Int32,System.Int32,System.Int32)
```
通过计算每一个基因组上面的每一个位点的重复片段的出现频率来计算出每一个位点的重复片段的热度

|Parameter Name|Remarks|
|--------------|-------|
|Mla|必须是经过多序列比对对齐的|


#### SearchRepeats
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsSearchAPI.SearchRepeats(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Int32,System.Int32,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|SequenceData|-|
|Min|-|
|Max|-|
|MinAppeared|最少的重复出现次数为2，也可以将这个值设置得更高一些|


#### Trim``1
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsSearchAPI.Trim``1(System.Collections.Generic.IEnumerable{``0},System.Int32,System.Int32,System.Int32)
```
原来的限定函数不起作用了？？可以使用这个函数进行剪裁，请注意剪裁是不可逆的，使用这个函数处理数据只能够收缩数据

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|min|-|
|max|-|
|minappear|-|



