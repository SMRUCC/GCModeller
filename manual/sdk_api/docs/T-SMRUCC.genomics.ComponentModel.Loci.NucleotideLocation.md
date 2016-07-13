---
title: NucleotideLocation
---

# NucleotideLocation
_namespace: [SMRUCC.genomics.ComponentModel.Loci](N-SMRUCC.genomics.ComponentModel.Loci.html)_

Loci segment location information on an nucleotide sequence, this object added an @"P:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.Strand" 
 information on @"T:SMRUCC.genomics.ComponentModel.Loci.Location" data.(会自动根据LEFT和RIGHT的值来修正属性值)



### Methods

#### #ctor
```csharp
SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.#ctor(System.Int64,System.Int64,System.Boolean)
```
使用片段的位置进行初始化本位点对象

|Parameter Name|Remarks|
|--------------|-------|
|_start|-|
|_ends|-|
|ComplementStrand|这个片段是否位于DNA上面的互补链或者是否为反向序列|


#### CreateObject
```csharp
SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.CreateObject(System.Int64,System.Int64)
```
会自动比较**start**和**ends**这两个参数的大小来确定链的方向

|Parameter Name|Remarks|
|--------------|-------|
|Start|-|
|Ends|-|


#### Equals
```csharp
SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.Equals(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation,System.Int32)
```
和当前的这个位点位置进行允许在一定的误差范围之内的模糊匹配

|Parameter Name|Remarks|
|--------------|-------|
|Loci|-|
|AllowedOffset|当这个值为0的时候就是绝对相等|


#### GetRelationship
```csharp
SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.GetRelationship(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```
Gets the location relationship of two loci segments.(获取两个位点片段之间的位置关系，请注意，这个函数只依靠左右位置来判断关系，假若对核酸链的方向有要求在调用本函数之前请确保二者在同一条链之上)

|Parameter Name|Remarks|
|--------------|-------|
|lcl|在计算之前请先调用@"M:SMRUCC.genomics.ComponentModel.Loci.Location.Normalization"方法来修正|


#### GetUpStreamLoci
```csharp
SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.GetUpStreamLoci(System.Int32)
```
请放心，这个函数所得到的具体的位点是和链的方向相关的

|Parameter Name|Remarks|
|--------------|-------|
|Distance|-|


#### LociIsContact
```csharp
SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.LociIsContact(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```
判断目标位点片段是否与本位点片段具有相互关系，这个函数是忽略掉了链的方向了的

|Parameter Name|Remarks|
|--------------|-------|
|Loci|-|


#### Normalization
```csharp
SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.Normalization
```
@"M:SMRUCC.genomics.ComponentModel.Loci.Location.Normalization"

#### ToString
```csharp
SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.ToString
```
这个函数的输出的字符串可以使用@"M:SMRUCC.genomics.ComponentModel.Loci.LociAPI.TryParse(System.String)"方法进行解析


### Properties

#### Ends
@"T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation": 实际的物理上面的位置，与核酸链的方向相关，(假若需要使用原始的位置数据，则请使用@"P:SMRUCC.genomics.ComponentModel.Loci.Location.Left"或者@"P:SMRUCC.genomics.ComponentModel.Loci.Location.Right"属性，这两个属性值是和具体的链的方向无关的)
#### IsValid
当任意一个断点为0的时候就无效
#### Start
@"T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation": 实际的物理上面的位置，与核酸链的方向相关，(假若需要使用原始的位置数据，则请使用@"P:SMRUCC.genomics.ComponentModel.Loci.Location.Left"或者@"P:SMRUCC.genomics.ComponentModel.Loci.Location.Right"属性，这两个属性值是和具体的链的方向无关的)
#### Strand
这个位点在哪一条DNA核酸链
 Forward = 1; 
 Reverse = -1; 
 Unknown = 0
