---
title: RegpreciseBidirectionalBh_Methods
---

# RegpreciseBidirectionalBh_Methods
_namespace: [SMRUCC.genomics.Data.Regprecise](N-SMRUCC.genomics.Data.Regprecise.html)_





### Methods

#### Convert
```csharp
SMRUCC.genomics.Data.Regprecise.RegpreciseBidirectionalBh_Methods.Convert(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit[])
```
从最佳双向BLAST比对结果之中得到所需要的基础数据

|Parameter Name|Remarks|
|--------------|-------|
|BLASTbh|-|


#### FamilyStatics
```csharp
SMRUCC.genomics.Data.Regprecise.RegpreciseBidirectionalBh_Methods.FamilyStatics(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Regprecise.RegpreciseMPBBH})
```
统计出所比对的蛋白质的家族分布,由于一个蛋白质可能会比对上对各家族，故而总和不会等于最佳比对的结果数目

#### Match
```csharp
SMRUCC.genomics.Data.Regprecise.RegpreciseBidirectionalBh_Methods.Match(SMRUCC.genomics.Data.Regprecise.RegpreciseMPBBH[],SMRUCC.genomics.Data.Regprecise.TranscriptionFactors,SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.MyvaCOG},SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString[])
```
可能会有一部分数据是在Regprecise之中没有记录的，但是在研究之中也将其添加进入数据源之中，在此时，需要一个额外的Fasta文件来匹配所缺失的数据

|Parameter Name|Remarks|
|--------------|-------|
|ResultRegpreciseBidirectionalBh|-|
|Regprecise|-|
|Myva_COG|-|
|PfamStrings|-|


#### MPCutoff
```csharp
SMRUCC.genomics.Data.Regprecise.RegpreciseBidirectionalBh_Methods.MPCutoff(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Regprecise.RegpreciseMPBBH},System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|cutoff|Cutoff value for property @"P:SMRUCC.genomics.Data.Regprecise.RegpreciseMPBBH.Similarity", a recommended value is 0.85|



