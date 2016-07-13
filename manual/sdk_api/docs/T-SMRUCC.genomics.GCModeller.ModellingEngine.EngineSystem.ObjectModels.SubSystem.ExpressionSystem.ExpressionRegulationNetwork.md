---
title: ExpressionRegulationNetwork
---

# ExpressionRegulationNetwork
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.ExpressionSystem](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.ExpressionSystem.html)_





### Methods

#### SetupMutation
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork.SetupMutation(System.String,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|UniqueId|-|
|Factor|大于0的一个数字，当值为0的时候表示缺失突变，当值为1的时候表示正常表达，当值大于1的时候表示过量表达|



### Properties

#### _InternalEvent_Transcriptions
主要是为了方便进行基因突变的实验而设置的一个对象列表
#### _InternalTranscriptDisposableSystem
@"F:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork._InternalTranscriptsPool"[RNA分子]的降解系统
#### DataSource
返回每一个基因的转录量的和
#### MutatedGenes
用于调试的监视变量
#### RegulationCoverage
在本计算模型之中的基因表达调控网络之中的被调控的基因的数目
#### RegulationEntitiesCount
Gets the gene object counts value in current gene expression regulation network.(获取被调控网络对象之中的被调控的基因的节点对象的数目)
#### RibosomeAssemblyCompound
翻译所需要的核糖体复合物
#### RNAPolymerase
转录所需要的RNA聚合酶复合物
#### TranslationDataSource
获取每一种mRNA分子的实时表达量
