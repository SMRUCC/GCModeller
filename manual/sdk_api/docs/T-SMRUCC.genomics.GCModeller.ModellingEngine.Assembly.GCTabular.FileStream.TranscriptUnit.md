---
title: TranscriptUnit
---

# TranscriptUnit
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.html)_

一个转录单元对象是有多个具有调控作用的motif以及多个编码产物的编码区所构成的




### Properties

#### BasalValue
The basal expression level of this @"T:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.TranscriptUnit" object
#### OperonGenes
@"P:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.TranscriptUnit.OperonId"的这个操纵子对象之中的结构基因，请注意，在初始化基因表达调控网络的结构的时候请务必要展开这个属性，否则结构基因的调控过程会被遗漏掉了的
#### OperonId
通常为属性@"P:SMRUCC.genomics.Assembly.DOOR.GeneBrief.OperonID"的这个编号值
#### PccValues
Pcc值指的是@"P:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.TranscriptUnit.Motifs"对@"P:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.TranscriptUnit.OperonGenes"中的基因对象的调控作用的大小
#### TFPcc
Regulator对第一个基因的Pcc值
#### TU_GUID
请使用这个属性来唯一标记当前的对象
