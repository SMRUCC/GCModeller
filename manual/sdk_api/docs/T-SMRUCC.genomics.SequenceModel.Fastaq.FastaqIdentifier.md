---
title: FastaqIdentifier
---

# FastaqIdentifier
_namespace: [SMRUCC.genomics.SequenceModel.Fastaq](N-SMRUCC.genomics.SequenceModel.Fastaq.html)_

Illumina sequence identifiers



### Methods

#### IDParser
```csharp
SMRUCC.genomics.SequenceModel.Fastaq.FastaqIdentifier.IDParser(System.String)
```
@HWUSI-EAS100R:6:73:941:1973#0/1

|Parameter Name|Remarks|
|--------------|-------|
|str|-|



### Properties

#### FlowCellLane
Flowcell lane
#### Identifier
The unique instrument name
#### MsIndex
Index number for a multiplexed sample (0 for no indexing)
#### PairMember
The member of a pair, /1 or /2 (paired-end or mate-pair reads only)
#### Tiles
Tile number within the flowcell lane
#### X
'x'-coordinate of the cluster within the tile
#### Y
'y'-coordinate of the cluster within the tile
