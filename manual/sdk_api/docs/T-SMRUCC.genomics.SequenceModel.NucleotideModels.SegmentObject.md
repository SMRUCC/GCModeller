---
title: SegmentObject
---

# SegmentObject
_namespace: [SMRUCC.genomics.SequenceModel.NucleotideModels](N-SMRUCC.genomics.SequenceModel.NucleotideModels.html)_

片段数据，包含有在目标核酸链之上的位置信息以及用户给这个片段的自定义的标签信息



### Methods

#### Get_GCContent
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentObject.Get_GCContent(System.String)
```
获取目标核酸片段的GC含量值，假设已经全部转换为大写字母

#### GetFasta
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentObject.GetFasta(System.String)
```
将当前的核酸片段数据对象转换为FASTA对象

|Parameter Name|Remarks|
|--------------|-------|
|title|If this value is not presents, then the function will using the location info as default.|



### Properties

#### Complement
This sequence segment object site is on the complement strand?
#### Description
User data
#### GC_Content
获取当前核酸片段的GC含量值，假设已经全部转换为大写字母
#### SequenceData
The sequence data of this site.
#### Title
User tag data
