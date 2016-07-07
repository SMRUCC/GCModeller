---
title: SAM_TSSs
---

# SAM_TSSs
_namespace: [SMRUCC.genomics.SequenceModel](N-SMRUCC.genomics.SequenceModel.html)_





### Methods

#### SplitSaved
```csharp
SMRUCC.genomics.SequenceModel.SAM_TSSs.SplitSaved(SMRUCC.genomics.SequenceModel.SAM.SAM,System.Boolean,System.String)
```
将SAM文件里面的Reads数据按照正向和反向分别进行保存到两个文件之中

#### TrimForTSSs
```csharp
SMRUCC.genomics.SequenceModel.SAM_TSSs.TrimForTSSs(SMRUCC.genomics.SequenceModel.SAM.SAM)
```
将一些标签去除一应用于下游的TSS分析

|Parameter Name|Remarks|
|--------------|-------|
|doc|
 会将文档里面的@"F:SMRUCC.genomics.SequenceModel.SAM.DocumentElements.BitFLAGS.Bit0x200",
 @"F:SMRUCC.genomics.SequenceModel.SAM.DocumentElements.BitFLAGS.Bit0x4"的Reads进行剔除
 |



