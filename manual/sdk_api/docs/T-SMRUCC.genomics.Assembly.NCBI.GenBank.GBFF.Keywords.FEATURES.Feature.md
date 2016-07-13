---
title: Feature
---

# Feature
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES](N-SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.html)_

A sequence feature site on the genome DNA sequence.(基因组序列上面的特性区域片段)



### Methods

#### Add
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature.Add(System.String,System.String)
```
添加一个注释信息

|Parameter Name|Remarks|
|--------------|-------|
|key|-|
|value|-|


#### Query
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature.Query(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.FeatureQualifiers)
```
Get feature describ value string by key

|Parameter Name|Remarks|
|--------------|-------|
|Key|-|


#### QueryDuplicated
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature.QueryDuplicated(System.String)
```
Some key would be duplicated

|Parameter Name|Remarks|
|--------------|-------|
|key|-|



### Properties

#### __innerList
请注意在值中有拼接断行所产生的空格，在导出CDS序列的时候，请注意消除该空格
#### Item
对于已经存在的数据，本方法会覆盖原有的数据，假若不存在，或者目标对象有多个值，则会进行添加
#### KeyName
第6至第20列的小字段的字段名
#### Location
The location of this feature site on the DNA chain.(本特性位点在DNA链上面的位置)
#### SequenceData
nt sequence of this feature site.
