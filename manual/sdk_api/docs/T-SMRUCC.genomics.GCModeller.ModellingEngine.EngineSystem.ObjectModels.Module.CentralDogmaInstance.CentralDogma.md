---
title: CentralDogma
---

# CentralDogma
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.html)_

ExpressionObject object equals to the 
 @"T:SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit"[TranscriptUnit object] 
 in the datamodels.
 一个转录对象是以一个转录单元为单位的，其可以被看作为中心法则的一个实例，描述了从基因到蛋白质的整个表达过程，一个操纵子对象的表达过程



### Methods

#### CreateInstance
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma.CreateInstance(SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem)
```
创建转录单元对象以及将蛋白质单体与相应的基因进行连接

|Parameter Name|Remarks|
|--------------|-------|
|TransUnit|-|


#### get_Regulators
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma.get_Regulators
```
将@"P:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma.MotifSites"之中的调控因子对象集合输出

#### Initialize
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma.Initialize(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem)
```
生成Transcription和Translation对象

|Parameter Name|Remarks|
|--------------|-------|
|CellSystem|初始化所需要用到的数据源|

> 根据TransUnit中的基因列表指针来生成mRNA列表并添加进入代谢组之中

#### Invoke
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma.Invoke
```
有多个调控因子的时候的表达的计算公式
> 
>  算法要点
>  
>  0. 对于所有随机试验低于阈值的调控事件，都默认为不激活(即，没有调控因子的激活的话，基因不表达)
>  1. 对于同一个位点之上，假若激活的数目多余抑制的数目，则激活的权重比较大，该位点计算为激活的可能性比较高(假设转录组数据是建立在大细胞宗系的条件之下测定的)
>  2. 在Promoter区之内，假若任意一个位点被抑制，则整个基因的表达过程被抑制(单纯的分子动力学行为)
>  


### Properties

#### ExpressionActivity
处于表达活性状态的转录单元对象
#### MotifSites
对当前的这个中心法则处理步骤过程的转录过程起调控作用的@"T:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Feature.MotifSite`1"的列表，这个仅仅是对基因的转录调控而言的
#### Transcripts
转录单元所转录出来的RNA分子
