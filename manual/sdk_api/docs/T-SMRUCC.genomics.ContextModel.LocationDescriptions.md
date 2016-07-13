---
title: LocationDescriptions
---

# LocationDescriptions
_namespace: [SMRUCC.genomics.ContextModel](N-SMRUCC.genomics.ContextModel.html)_





### Methods

#### ATGDistance``1
```csharp
SMRUCC.genomics.ContextModel.LocationDescriptions.ATGDistance``1(``0,SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```
Calculate the ATG distance between the target gene and the specific feature loci.

|Parameter Name|Remarks|
|--------------|-------|
|gene|-|
|nucl|-|


#### GetATGDistance
```csharp
SMRUCC.genomics.ContextModel.LocationDescriptions.GetATGDistance(SMRUCC.genomics.ComponentModel.Loci.Location,SMRUCC.genomics.ComponentModel.IGeneBrief)
```
Calculates the ATG distance between the target gene and a loci segment on.(计算位点相对于某一个基因的ATG距离)

|Parameter Name|Remarks|
|--------------|-------|
|loci|-|
|gene|-|

_returns: 总是计算最大的距离_

#### GetLociRelations``1
```csharp
SMRUCC.genomics.ContextModel.LocationDescriptions.GetLociRelations``1(``0,SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```
Get the loci relationship between the target gene and the specific feature loci.

|Parameter Name|Remarks|
|--------------|-------|
|gene|-|
|nucl|-|


#### IsBlankSegment
```csharp
SMRUCC.genomics.ContextModel.LocationDescriptions.IsBlankSegment(SMRUCC.genomics.ComponentModel.IGeneBrief)
```
判断本对象是否是由@"M:SMRUCC.genomics.ContextModel.LocationDescriptions.BlankSegment``1(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)"方法所生成的空白片段

#### LocationDescription``1
```csharp
SMRUCC.genomics.ContextModel.LocationDescriptions.LocationDescription``1(SMRUCC.genomics.ComponentModel.Loci.SegmentRelationships,``0)
```
Gets the loci location description data.

|Parameter Name|Remarks|
|--------------|-------|
|posi|-|
|data|-|



