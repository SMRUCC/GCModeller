---
title: FluxObject
---

# FluxObject
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.DataModel](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.DataModel.html)_

一个代谢反应对象或者转录调控过程



### Methods

#### GetCoefficient
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.DataModel.FluxObject.GetCoefficient(System.String)
```
Left为消耗，负值；Right为合成项，正值，当不存在的时候返回零

|Parameter Name|Remarks|
|--------------|-------|
|Metabolite|-|



### Properties

#### AssociatedRegulationGenes
催化本反应过程的基因或者调控因子(列)，请注意，由于在前半部分为代谢流对象，故而Key的值不是从零开始的
