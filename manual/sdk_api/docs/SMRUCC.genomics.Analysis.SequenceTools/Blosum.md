﻿# Blosum
_namespace: [SMRUCC.genomics.Analysis.SequenceTools](./index.md)_

Blosum-62 substitution matrix
 # A R N D C Q E G H I L K M F P S T W Y V 
 A 4 -1 -2 -2 0 -1 -1 0 -2 -1 -1 -1 -1 -2 -1 1 0 -3 -2 0 
 R -1 5 0 -2 -3 1 0 -2 0 -3 -2 2 -1 -3 -2 -1 -1 -3 -2 -3 
 N -2 0 6 1 -3 0 0 0 1 -3 -3 0 -2 -3 -2 1 0 -4 -2 -3 
 D -2 -2 1 6 -3 0 2 -1 -1 -3 -4 -1 -3 -3 -1 0 -1 -4 -3 -3 
 C 0 -3 -3 -3 9 -3 -4 -3 -3 -1 -1 -3 -1 -2 -3 -1 -1 -2 -2 -1 
 Q -1 1 0 0 -3 5 2 -2 0 -3 -2 1 0 -3 -1 0 -1 -2 -1 -2 
 E -1 0 0 2 -4 2 5 -2 0 -3 -3 1 -2 -3 -1 0 -1 -3 -2 -2 
 G 0 -2 0 -1 -3 -2 -2 6 -2 -4 -4 -2 -3 -3 -2 0 -2 -2 -3 -3 
 H -2 0 1 -1 -3 0 0 -2 8 -3 -3 -1 -2 -1 -2 -1 -2 -2 2 -3 
 I -1 -3 -3 -3 -1 -3 -3 -4 -3 4 2 -3 1 0 -3 -2 -1 -3 -1 3 
 L -1 -2 -3 -4 -1 -2 -3 -4 -3 2 4 -2 2 0 -3 -2 -1 -2 -1 1 
 K -1 2 0 -1 -3 1 1 -2 -1 -3 -2 5 -1 -3 -1 0 -1 -3 -2 -2 
 M -1 -1 -2 -3 -1 0 -2 -3 -2 1 2 -1 5 0 -2 -1 -1 -1 -1 1 
 F -2 -3 -3 -3 -2 -3 -3 -3 -1 0 0 -3 0 6 -4 -2 -2 1 3 -1 
 P -1 -2 -2 -1 -3 -1 -1 -2 -2 -3 -3 -1 -2 -4 7 -1 -1 -4 -3 -2 
 S 1 -1 1 0 -1 0 0 0 -1 -2 -2 0 -1 -2 -1 4 1 -3 -2 -2 
 T 0 -1 0 -1 -1 -1 -1 -2 -2 -1 -1 -1 -1 -2 -1 1 5 -2 -2 0 
 W -3 -3 -4 -4 -2 -2 -3 -2 -2 -3 -2 -3 -1 1 -4 -3 -2 11 2 -3 
 Y -2 -2 -2 -3 -2 -1 -2 -3 2 -1 -1 -2 -1 3 -3 -2 -2 2 7 -1 
 V 0 -3 -3 -3 -1 -2 -2 -3 -3 3 1 -2 1 -1 -2 -2 0 -3 -1 4



### Methods

#### FromInnerBlosum62
```csharp
SMRUCC.genomics.Analysis.SequenceTools.Blosum.FromInnerBlosum62
```
Looks up a localized string similar to # Matrix made by matblas from blosum62.iij
# * column uses minimum score
# BLOSUM Clustered Scoring Matrix in 1/2 Bit Units
# Blocks Database = /data/blocks_5.0/blocks.dat
# Cluster Percentage: >= 62
# Entropy = 0.6979, Expected = -0.5209
 A R N D C Q E G H I L K M F P S T W Y V B Z X *
A 4 -1 -2 -2 0 -1 -1 0 -2 -1 -1 -1 -1 -2 -1 1 0 -3 -2 0 -2 -1 0 -4 
R -1 5 0 -2 -3 1 0 -2 0 -3 -2 2 -1 -3 -2 -1 -1 -3 -2 -3 -1 0 -1 -4 
N -2 0 6 1 -3 0 0 0 1 -3 -3 0 -2 [rest of string was truncated]";.

#### getDistance
```csharp
SMRUCC.genomics.Analysis.SequenceTools.Blosum.getDistance(System.Char,System.Char)
```
函数对字符的大小写不敏感

|Parameter Name|Remarks|
|--------------|-------|
|a1|-|
|a2|-|


#### LoadFromStream
```csharp
SMRUCC.genomics.Analysis.SequenceTools.Blosum.LoadFromStream(System.String)
```
Load Blosum matrix from the text file, and this Blosum matrix file which is available downloads from NCBI FTP site.

|Parameter Name|Remarks|
|--------------|-------|
|doc|-|


#### LoadMatrix
```csharp
SMRUCC.genomics.Analysis.SequenceTools.Blosum.LoadMatrix(System.String)
```
Load Blosum matrix from the text file, and this Blosum matrix file which is available downloads from NCBI FTP site.

|Parameter Name|Remarks|
|--------------|-------|
|path|-|



### Properties

#### Matrix
Default Blosum-62 substitution matrix from inner resource
