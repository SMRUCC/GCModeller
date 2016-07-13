---
title: Corrects
---

# Corrects
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.Operon](N-SMRUCC.genomics.Analysis.RNA_Seq.Operon.html)_

根据转录组数据来修正操纵子



### Methods

#### __correctOperon
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.Operon.Corrects.__correctOperon(SMRUCC.genomics.Assembly.DOOR.Operon,SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|Operon|-|


#### __splitOperon
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.Operon.Corrects.__splitOperon(System.String,SMRUCC.genomics.Assembly.DOOR.GeneBrief[],SMRUCC.genomics.Assembly.DOOR.GeneBrief,System.Int32,System.Double,SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix)
```


|Parameter Name|Remarks|
|--------------|-------|
|Operon|-|
|idx|参数请从1开始设置|
|structGenes|在递归的最开始阶段需要根据链的方向进行排序操作的|


#### CorrectDoorOperon
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.Operon.Corrects.CorrectDoorOperon(SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix,SMRUCC.genomics.Assembly.DOOR.DOOR,System.Double)
```
由于假设认为DOOR里面的operon是依照基因组上面的位置来作为一个计算因素的，所以在这里的预测数据在物理位置上面都是满足的，假若基因不是一个operon里面的，则只需要分割operon就好了

|Parameter Name|Remarks|
|--------------|-------|
|PCC|-|
|DOOR|-|
|pccCutoff|应该是一个正数|



