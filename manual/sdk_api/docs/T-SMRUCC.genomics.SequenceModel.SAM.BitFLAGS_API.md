---
title: BitFLAGS_API
---

# BitFLAGS_API
_namespace: [SMRUCC.genomics.SequenceModel.SAM](N-SMRUCC.genomics.SequenceModel.SAM.html)_





### Methods

#### ComputeBitFLAGS
```csharp
SMRUCC.genomics.SequenceModel.SAM.BitFLAGS_API.ComputeBitFLAGS(System.Int32)
```
Bit Description
 0x1 template having multiple segments in sequencing
 0x2 each segment properly aligned according to the aligner
 0x4 segment unmapped
 0x8 next segment in the template unmapped
 0x10 SEQ being reverse complemented
 0x20 SEQ of the next segment in the template being reversed
 0x40 the first segment in the template
 0x80 the last segment in the template
 0x100 secondary alignment
 0x200 Not passing quality controls
 0x400 PCR Or optical duplicate
 
 先将数值从十进制转换为2进制，然后再从后面往前面取标记

|Parameter Name|Remarks|
|--------------|-------|
|Flag|-|



### Properties

#### BitFLAG
标记静态缓存
#### Descriptions
标记描述的静态缓存
