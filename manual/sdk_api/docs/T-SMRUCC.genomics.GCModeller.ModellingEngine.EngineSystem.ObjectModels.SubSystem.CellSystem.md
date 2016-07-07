---
title: CellSystem
---

# CellSystem
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.html)_





### Methods

#### Initialize
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem.Initialize
```
先加载系统模块在进行系统的初始化操作


### Properties

#### CellDeathDetection
当满足下面两个条件的时候，认为细胞死亡：
 1. 转录和翻译的流量总和小于1
 2. RNA分子和多肽链分子的数量总和小于1
#### DataModel
本细胞对象的数据模型
