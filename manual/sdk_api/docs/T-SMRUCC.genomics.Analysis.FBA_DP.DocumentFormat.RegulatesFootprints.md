---
title: RegulatesFootprints
---

# RegulatesFootprints
_namespace: [SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat](N-SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat.html)_

简单描述调控位点和调控因子之间的关系以及该位点在基因组上面的位置



### Methods

#### TraceUid
```csharp
SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat.RegulatesFootprints.TraceUid(SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat.RegulatesFootprints)
```
非严格的

|Parameter Name|Remarks|
|--------------|-------|
|x|-|


#### TraceUidStrict
```csharp
SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat.RegulatesFootprints.TraceUidStrict(SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat.RegulatesFootprints)
```
严格的

|Parameter Name|Remarks|
|--------------|-------|
|x|-|



### Properties

#### Category
C. 目标基因对象的KEGG的代谢途径分类
#### Class
B 目标基因对象的KEGG的代谢途径分类
#### DoorId
目标基因所在的操纵子对象的Door数据库之中的编号
#### InitX
当前的这个基因是否是其所处的操纵子的第一个基因
#### MotifTrace
Trace.Site.(Motif的匹配来源)
#### Pcc
所预测的调控因子对目标基因的调控作用的权重的大小，这里的元素的顺序是和@"P:SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat.RegulatesFootprints.Regulator"之中的顺序是一一对应的
#### Regulator
这个基因对象所被预测的调控因子
#### RegulatorTrace
Trace.Regulator.(调控因子的匹配来源)
#### StructGenes
操纵子里面的结构基因，只有当@"P:SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat.RegulatesFootprints.InitX"为真的时候这个属性值才不为空
#### tag
自定义的一个标签数据
#### Type
A
