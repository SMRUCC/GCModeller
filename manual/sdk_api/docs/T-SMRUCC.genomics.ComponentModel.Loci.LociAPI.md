---
title: LociAPI
---

# LociAPI
_namespace: [SMRUCC.genomics.ComponentModel.Loci](N-SMRUCC.genomics.ComponentModel.Loci.html)_





### Methods

#### __tryParse
```csharp
SMRUCC.genomics.ComponentModel.Loci.LociAPI.__tryParse(System.String)
```
388739 ==> 389772 #Forward

|Parameter Name|Remarks|
|--------------|-------|
|s_Loci|-|


#### GetRelationship
```csharp
SMRUCC.genomics.ComponentModel.Loci.LociAPI.GetRelationship(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation,SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```
Gets the location relationship of two loci segments.(判断获取两个位点片段之间的位置关系，请注意，这个函数只依靠左右位置来判断关系，假若对核酸链的方向有要求在调用本函数之前请确保二者在同一条链之上)

|Parameter Name|Remarks|
|--------------|-------|
|lcl|在计算之前请先调用@"M:SMRUCC.genomics.ComponentModel.Loci.Location.Normalization"方法来修正|


#### GetStrand
```csharp
SMRUCC.genomics.ComponentModel.Loci.LociAPI.GetStrand(System.String)
```
Convert the string value type nucleotide strand information description data into a strand enumerate data.

|Parameter Name|Remarks|
|--------------|-------|
|str|从文本文件之中所读取出来关于链方向的字符串描述数据|


#### Merge``1
```csharp
SMRUCC.genomics.ComponentModel.Loci.LociAPI.Merge``1(System.Collections.Generic.IEnumerable{``0})
```
直接合并相邻的一个位点集合到一个新的更加长的位点

|Parameter Name|Remarks|
|--------------|-------|
|groupedData|-|


#### TryParse
```csharp
SMRUCC.genomics.ComponentModel.Loci.LociAPI.TryParse(System.String)
```
Try parse NCBI sequence dump location/@"M:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.ToString" dump location.

|Parameter Name|Remarks|
|--------------|-------|
|sLoci|-|



