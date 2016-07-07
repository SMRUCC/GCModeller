---
title: MetabolismOptimization
---

# MetabolismOptimization
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components.html)_





### Methods

#### InreversibleRule
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components.MetabolismOptimization.InreversibleRule(SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.MetabolismFlux,System.Double)
```
假若优化值小于10，则优化值为10

|Parameter Name|Remarks|
|--------------|-------|
|Model|-|
|OptimizationValue|-|


#### ReversibleRule
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components.MetabolismOptimization.ReversibleRule(SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.MetabolismFlux,System.Double)
```
如果优化值大于10或者小于-10，则另一个方向为当前方向的0.5
 假如优化值小于10或者大于-10，则两个方向的值都为10

|Parameter Name|Remarks|
|--------------|-------|
|Model|-|
|OptimizationValue|-|



