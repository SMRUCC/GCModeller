---
title: Location
---

# Location
_namespace: [SMRUCC.genomics.ComponentModel.Loci](N-SMRUCC.genomics.ComponentModel.Loci.html)_

A location property on a sequence data. Please notice that if the loci value its left value greater than right value then this object will swap the value automaticaly.
 (一个序列片段区域的位置，请注意，当Left的大小大于Right的时候，模块会自动纠正为Left小于Right的状态，这个对象可以同时用来表示核酸序列或者蛋白质序列上面的位置)



### Methods

#### ContainSite
```csharp
SMRUCC.genomics.ComponentModel.Loci.Location.ContainSite(System.Int32)
```
Is the target site is on this location region?(目标位点是否被包含在当前的位置区域之中)

|Parameter Name|Remarks|
|--------------|-------|
|p|这个点是没有指定链的方向的|


#### CreateObject
```csharp
SMRUCC.genomics.ComponentModel.Loci.Location.CreateObject(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|strData|-|
|Delimiter|-|


#### GetResiduesLoci
```csharp
SMRUCC.genomics.ComponentModel.Loci.Location.GetResiduesLoci
```
将这个位点对象转换为每一个残基位点的位置对象，可能有些无聊

#### Inside
```csharp
SMRUCC.genomics.ComponentModel.Loci.Location.Inside(SMRUCC.genomics.ComponentModel.Loci.Location,System.Int32)
```
**loci** inside Me.(目标位点在当前的这个位点之中)

|Parameter Name|Remarks|
|--------------|-------|
|loci|-|


#### InsideOrOverlapWith
```csharp
SMRUCC.genomics.ComponentModel.Loci.Location.InsideOrOverlapWith(SMRUCC.genomics.ComponentModel.Loci.Location,System.Int32)
```
**b**在当前对象之中或者与当前对象重叠

|Parameter Name|Remarks|
|--------------|-------|
|b|-|


#### Normalization
```csharp
SMRUCC.genomics.ComponentModel.Loci.Location.Normalization
```
使用这个方法更正数据，使位置数据始终是右大于左，(Return Me: 修改自身之后返回自身)

#### op_Equality
```csharp
SMRUCC.genomics.ComponentModel.Loci.Location.op_Equality(SMRUCC.genomics.ComponentModel.Loci.Location,SMRUCC.genomics.ComponentModel.Loci.Location)
```
Position equals

|Parameter Name|Remarks|
|--------------|-------|
|a|-|
|b|-|



### Properties

#### FragmentSize
The segment length of this location object.(目标序列片段区域的片段长度)
#### IsNormalized
当前的这个位置数据是否为左边小于右边的正常状态
#### Left
@"T:SMRUCC.genomics.ComponentModel.Loci.Location": Gets or set the left start value of the segment on the target sequence.(目标片段的左端起始区域，与链的方向无关)
#### Right
@"T:SMRUCC.genomics.ComponentModel.Loci.Location": Gets or set the right ends value of the segment on the target sequence.(目标片段的右端结束区域，与链的方向无关)
