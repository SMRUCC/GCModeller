---
title: BestHit
---

# BestHit
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.Analysis](N-SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.html)_

元数据Xml文件



### Methods

#### ExportCsv
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit.ExportCsv(System.Boolean)
```
在这里导出Venn表

 格式
 [Description] [QueryProtein] {[] [HitProtein] [Identities] [Positive]}
> 请注意，为了保持数据之间的一一对应关系，这里不能够再使用并行化了

#### GetConservedRegions
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit.GetConservedRegions(System.Double,System.Int32)
```
根据比对数据自动的推断出保守的区域

#### GetUnConservedRegions
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit.GetUnConservedRegions(System.Collections.Generic.IReadOnlyList{System.String[]})
```
从保守的片段数据之中反向取出不保守的片段

|Parameter Name|Remarks|
|--------------|-------|
|conserved|-|


#### InternalSort
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit.InternalSort(System.Boolean)
```
按照比对的蛋白质的数目的多少对Hit之中的元素进行统一进行排序

|Parameter Name|Remarks|
|--------------|-------|
|TrimNull|将没有任何匹配的对象去除|


#### SelectSourceFromHits
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit.SelectSourceFromHits(System.String,System.String)
```
将比对上的物种的fasta文件复制到目标文件夹**copyTo**之中，目标函数返回所复制的菌株的编号列表

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|copyTo|-|


#### TrimEmpty
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit.TrimEmpty(System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|p|0-1|



### Properties

#### GetTopHits
获取能够被比对上的较多数目的物种的编号
#### Hit
通过query查找的是reference的对象
#### sp
The species name of query.(进行当前匹配操作的物种名称，这个属性不是蛋白质的名称)
