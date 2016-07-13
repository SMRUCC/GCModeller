---
title: SegmentRelationships
---

# SegmentRelationships
_namespace: [SMRUCC.genomics.ComponentModel.Loci](N-SMRUCC.genomics.ComponentModel.Loci.html)_

The location relationship description enumeration for the two loci sites on the nucleotide sequence.
 (核酸链上面的位点片段之间的位置关系的描述)

> 为了能够在查询的时候对输入进行叠加，在这里采取互斥



### Properties

#### Blank
There is nothing on this location.
#### Cover
比较的目标位点包括了当前的这个位置参照
#### Equals
Target loci is on the same @"T:SMRUCC.genomics.ComponentModel.Loci.Strands" with current loci and position is also equals.
#### InnerAntiSense
指定的位点在目标位点的内部的反向序列之上
#### UpStreamOverlap
The loci is on the upstream of the target loci, but part of the loci was overlapping.
 (目标位点和当前的位点重叠在一个，但是目标位点的左端是在当前位点的上游的)
